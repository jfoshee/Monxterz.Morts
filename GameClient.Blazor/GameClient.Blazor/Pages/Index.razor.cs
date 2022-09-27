namespace GameClient.Blazor.Pages;

public partial class Index : IDisposable
{
    private const string characterSymbol = "&#10074;";
    private const byte plane = 0;
    private const int gridSize = 7;
    private (int x, int y) center = (0x80, 0x80);
    private const string Training = "training";
    private const string Defending = "defending";
    private const string Gathering = "gathering";
    private const int RecoverySeconds = 15;

    [Inject] ILocalStorageService localStorageService { get; set; } = default!;
    [Inject] ILogoutService logoutService { get; set; } = default!;
    [Inject] IGameBearerTokenProvider gameBearerTokenProvider { get; set; } = default!;
    [Inject] IChangeUserService changeUserService { get; set; } = default!;
    [Inject] NavigationManager navigationManager { get; set; } = default!;
    [Inject] IGameTestHarness game { get; set; } = default!;
    [Inject] IGameStateClient gameStateClient { get; set; } = default!;
    [Inject] ICharacterFactory characterFactory { get; set; } = default!;
    [Inject] IEntityCache entityCache { get; set; } = default!;
    [Inject] NotificationSubscriptionService notificationSubscriptionService { get; set; } = default!;
    [Inject] IToastService toastService { get; set; } = default!;

    private string DetailTitle
    {
        get
        {
            if (selectedLocation is null)
                return "Tap a square in the map";
            return selectedCellCharacters.Any() ? "In the area:" : "Nobody in the area";
        }
    }

    private IEnumerable<Character> myCharacters => selectedCellCharacters.Where(c => c.OwnerId == user!.Id);
    private IEnumerable<Character> theirCharacters => selectedCellCharacters.Where(c => c.OwnerId != user!.Id);

    private GameEntityState? user;
    private string? selectedLocation;
    private Character? selectedCharacter;
    private Character? selectedTheirCharacter;
    private ILookup<string, Character>? characterMap;
    private List<Character> selectedCellCharacters = new();
    private string gameRegion = "";

    protected override async Task OnInitializedAsync()
    {
        var token = await localStorageService.GetItemAsync<string>("Token");
        var userId = await localStorageService.GetItemAsync<string>("UserID");
        // TODO: Break direct dependence on IGameBearerTokenProvider
        gameBearerTokenProvider.SetBearerToken(userId, token);
        if (changeUserService.CurrentUserId is null)
            navigationManager.NavigateTo("/login");
        else
        {
            await game.InitAsync();
            user = await gameStateClient.GetUserAsync() ?? throw new Exception("Failed to fetch current user entity");
            await InitializeEntities();
            toastService.ShowInfoRpg($"Welcome Back, {user.DisplayName}!");
        }
    }

    protected override void OnInitialized()
    {
        notificationSubscriptionService.EntityChanged += OnEntityChanged;
    }

    public void Dispose()
    {
        // https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-6.0#event-handlers
        notificationSubscriptionService.EntityChanged -= OnEntityChanged;
    }

    Task Logout() => logoutService.Logout();

    private void OnEntityChanged(string id, GameEntityState? stale, GameEntityState updated)
    {
        // Entity Cache should be up to date at this point
        RefreshFromCache();
        ShowToastForChange(stale, updated);
        // https://learn.microsoft.com/en-us/aspnet/core/blazor/components/rendering?view=aspnetcore-6.0#receiving-a-call-from-something-external-to-the-blazor-rendering-and-event-handling-system
        InvokeAsync(StateHasChanged);
    }

    private void ShowToastForChange(GameEntityState? stale, GameEntityState updated)
    {
        // Pop toast for attacks from other players
        if (stale is null || !stale.IsCharacter())
            return;
        // We can't use `Character` to get the stale HP because Character just wraps the entityCache which is already updated
        var staleHp = stale.GetPublicValue<float>(Constants.GameMasterId, "hp");
        var character = new Character(entityCache, updated.Id!);
        var updatedHp = character.Hp;
        if (updatedHp < staleHp)
        {
            var damage = staleHp - updatedHp;
            var deathImminent = (updatedHp > 0 && updatedHp < damage) ? "Death is imminent!" : "";
            toastService.ShowInfoRpg($"{character.Name} was {character.AttackAttribution}! {damage} Damage! {deathImminent}");
        }
    }

    private void RefreshFromCache()
    {
        var entities = entityCache.Entities;
        characterMap = characterFactory.Characters(entities!)
                                       .ToLookup(c => c.Location);
        if (selectedLocation is not null)
            selectedCellCharacters = characterMap[selectedLocation].ToList();
    }

    private async Task RefreshFromServer()
    {
        var entities = await gameStateClient.GetEntitiesNearbyAsync()
                       ?? throw new Exception("Nearby entities response was null.");
        entityCache.Set(entities);
        RefreshFromCache();
    }

    private async Task InitializeEntities()
    {
        this.gameRegion = await game.GetRegion();
        var userLocation = Region.Combine(gameRegion, $"{plane:X2}:00:00");
        await game.Move(user!, userLocation);
        await RefreshFromServer();
    }

    private IEnumerable<string?> ActiveCellNames(IEnumerable<Character>? characters) => characters?.Select(c => c.Name) ?? Array.Empty<string>();

    /// <summary>
    /// Returns the location string inside the GameMaster's region
    /// for a specific on-screen grid cell
    /// </summary>
    string Sublocation(int col, int row)
    {
        var x = col + center.x - gridSize / 2;
        var y = row + center.y - gridSize / 2;
        return $"{plane:X2}:{x:X2}:{y:X2}";
    }

    private string Location(int col, int row)
    {
        if (string.IsNullOrEmpty(gameRegion))
            return "";
        return Region.Combine(gameRegion, Sublocation(col, row));
    }

    private int Count(string location, Func<Character, bool> predicate)
    {
        if (characterMap is null || user is null)
            return 0;
        return characterMap[location].Count(predicate);
    }

    private MarkupString Symbols(string location, Func<Character, bool> predicate)
    {
        var count = Count(location, predicate);
        var repeat = Enumerable.Repeat(characterSymbol, count);
        var s = string.Join(" ", repeat);
        return new MarkupString(s);
    }

    string CssClass(Character character)
    {
        return selectedCharacter == character || selectedTheirCharacter == character ? "active" : "";
    }

    string CssClassCell(string location)
    {
        return selectedLocation == location ? "active" : "";
    }

    private async Task SelectLocation(string location, Character? character = null)
    {
        if (characterMap is null)
            return;
        selectedLocation = location;
        selectedCharacter = character;
        selectedTheirCharacter = null;
        if (characterMap is not null)
        {
            selectedCellCharacters = characterMap[selectedLocation].ToList();
            selectedCharacter ??= myCharacters.FirstOrDefault();
        }
        // Move user to the active cell so new characters will be created there
        await game.Move(user!, location);
    }

    async Task NewCharacter()
    {
        try
        {
            selectedCharacter = await characterFactory.CreateNew();
            // The new character will be in the cache already
            RefreshFromCache();
        }
        catch (ApiException apiException)
        {
            toastService.ShowErrorRpg(apiException.SimpleMessage());
        }
    }

    async Task Move(int axis, int amount)
    {
        try
        {
            if (selectedCharacter is null)
                return;
            var location = selectedCharacter.Location;
            var newLocation = Region.IncrementChunk(location, axis + 5, amount);
            // game.Move updates the Entity in place
            await game.Move(selectedCharacter.Entity, newLocation);
            RefreshFromCache();
            // Move the active cell too to make it easy to keep moving that same character
            await SelectLocation(newLocation, selectedCharacter);
        }
        catch (ApiException apiException)
        {
            toastService.ShowErrorRpg(apiException.SimpleMessage());
        }
    }

    async Task DoActivity(string activity)
    {
        if (selectedCharacter is null)
            return;
        try
        {
            await game.Call.StartActivity(selectedCharacter.Entity, activity);
            RefreshFromCache();
        }
        catch (ApiException apiException)
        {
            toastService.ShowErrorRpg(apiException.SimpleMessage());
        }
        catch (Exception exception)
        {
            toastService.ShowError(exception.Message);
        }
    }

    async Task StopActivity()
    {
        if (selectedCharacter is null)
            return;
        try
        {
            await game.Call.StopActivity(selectedCharacter.Entity);
            RefreshFromCache();
        }
        catch (ApiException apiException)
        {
            toastService.ShowErrorRpg(apiException.SimpleMessage());
        }
        catch (Exception exception)
        {
            toastService.ShowError(exception.Message);
        }
    }

    Task CheckStatus() => CheckStatus(selectedCharacter);

    async Task CheckStatus(Character? character)
    {
        if (character is null)
            return;
        try
        {
            await game.Call.CheckStatus(character.Entity);
            RefreshFromCache();
        }
        catch (ApiException apiException)
        {
            toastService.ShowErrorRpg(apiException.SimpleMessage());
        }
        catch (Exception exception)
        {
            toastService.ShowError(exception.Message);
        }
    }

    async Task Attack()
    {
        if (selectedCharacter is null || selectedTheirCharacter is null)
            return;
        try
        {
            await game.Call.Attack(selectedCharacter.Entity, selectedTheirCharacter.Entity);
            RefreshFromCache();
            // HACK: Unawaited "background" task to check recovery after it is completed
#pragma warning disable CS4014
            Task.Delay(RecoverySeconds * 1010).ContinueWith(_ => CheckStatus(selectedCharacter));
#pragma warning restore CS4014
        }
        catch (ApiException apiException)
        {
            toastService.ShowErrorRpg(apiException.SimpleMessage());
        }
        catch (Exception exception)
        {
            toastService.ShowError(exception.Message);
        }
    }
}
