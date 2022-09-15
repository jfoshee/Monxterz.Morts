using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Monxterz.StatePlatform;
using Monxterz.StatePlatform.Client;
using Monxterz.StatePlatform.ClientServices;

namespace GameClient.Blazor.Pages;

public partial class Index
{
    [Inject] ILocalStorageService localStorageService { get; set; } = default!;
    [Inject] IGameBearerTokenProvider gameBearerTokenProvider { get; set; } = default!;
    [Inject] IChangeUserService changeUserService { get; set; } = default!;
    [Inject] NavigationManager navigationManager { get; set; } = default!;
    [Inject] IGameTestHarness game { get; set; } = default!;
    [Inject] IGameStateClient gameStateClient { get; set; } = default!;
    [Inject] IGameMasterService gameMasterService { get; set; } = default!;

    private GameEntityState? user;
    private const byte plane = 0;
    private const int gridSize = 7;
    private (int x, int y) center = (0x80, 0x80);
    private IEnumerable<GameEntityState>? entities;
    private IEnumerable<string?>? Names => entities?.Select(e => $"{e.DisplayName} {e.SystemState.Location} ({e.Id})");
    private ILookup<string, GameEntityState>? entityMap;
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
        }
    }

    private async Task InitializeEntities()
    {
        this.gameRegion = await game.GetRegion();
        var userLocation = Region.Combine(gameRegion, $"{plane:X2}:00:00");
        await game.Move(user!, userLocation);
        var entities = await gameStateClient.GetEntitiesNearbyAsync();
        entityMap = entities!.ToLookup(e => e.SystemState.Location);
    }

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
        return Region.Combine(gameRegion, Sublocation(col, row));
    }

    void OnClick(int col, int row)
    {
        if (entityMap is null)
            return;
        string location = Location(col, row);
        entities = entityMap[location];
    }
}
