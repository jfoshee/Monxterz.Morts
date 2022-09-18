﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace GameClient.Blazor.Pages;

public partial class Index
{
    private const string characterSymbol = "&#10074;";
    private const byte plane = 0;
    private const int gridSize = 7;
    private (int x, int y) center = (0x80, 0x80);

    [Inject] ILocalStorageService localStorageService { get; set; } = default!;
    [Inject] IGameBearerTokenProvider gameBearerTokenProvider { get; set; } = default!;
    [Inject] IChangeUserService changeUserService { get; set; } = default!;
    [Inject] NavigationManager navigationManager { get; set; } = default!;
    [Inject] IGameTestHarness game { get; set; } = default!;
    [Inject] IGameStateClient gameStateClient { get; set; } = default!;
    [Inject] IGameMasterService gameMasterService { get; set; } = default!;
    [Inject] IConfiguration configuration { get; set; } = default!;
    [Inject] ILogger<Index> logger { get; set; } = default!;
    [Inject] ICharacterFactory characterFactory { get; set; } = default!;
    [Inject] IEntityCache entityCache { get; set; } = default!;

    private string DetailTitle
    {
        get
        {
            if (activeCell is null)
                return "Tap a square in the map";
            return cellCharacters.Any() ? "In the area:" : "Nobody in the area";
        }
    }

    private IEnumerable<Character>? myCharacters => cellCharacters.Where(c => c.OwnerId == user!.Id);
    private IEnumerable<Character>? theirCharacters => cellCharacters.Where(c => c.OwnerId != user!.Id);

    private GameEntityState? user;
    private ILookup<string, Character>? characterMap;
    private List<Character> cellCharacters = new();
    private string gameRegion = "";
    private string? activeCell = null;
    private HubConnection hubConnection = default!;

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
        await Refresh();
    }

    private async Task Refresh()
    {
        var entities = await gameStateClient.GetEntitiesNearbyAsync() ?? throw new Exception("Nearby entities response was null.");
        entityCache.Set(entities);
        characterMap = characterFactory.Characters(entities!).ToLookup(c => c.Location);
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

    string ActiveClass(string location)
    {
        return activeCell == location ? "active" : "";
    }

    async Task OnClick(string location)
    {
        if (characterMap is null)
            return;
        await Activate(location);
    }

    private async Task Activate(string location)
    {
        activeCell = location;
        cellCharacters = characterMap[location].ToList();
        // Move user to the active cell so new characters will be created there
        await game.Move(user!, location);
    }

    async Task NewCharacter()
    {
        var character = await characterFactory.CreateNew();
        cellCharacters.Add(character);
        // HACK: Update characterMap
        await Refresh();
    }

    async Task Move(int axis, int amount)
    {
        var activeCharacter = myCharacters.First();
        var location = activeCharacter.Location;
        var newLocation = Region.IncrementChunk(location, axis + 5, amount);
        await game.Move(activeCharacter.Entity, newLocation);
        cellCharacters.Remove(activeCharacter);
        // HACK: Update characterMap
        await Refresh();
        // Move the active cell too to make it easy to keep moving that same character
        await Activate(newLocation);
    }
}
