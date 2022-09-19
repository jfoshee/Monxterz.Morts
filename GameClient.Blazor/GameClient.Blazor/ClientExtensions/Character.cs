namespace GameClient.Blazor.ClientExtensions;

public class Character
{
    private readonly IEntityCache entityCache;
    //private readonly IGameTestHarness game;
    public string Id { get; }
    public GameEntityState Entity => entityCache[Id];
    public string Name => OwnerName ?? Entity.DisplayName ?? Id;
    public string OwnerId => Entity.SystemState.OwnerId;
    public string Location => Entity.SystemState.Location;
    public GameEntityState? Owner => entityCache.Get(Entity.SystemState.OwnerId);
    public string? OwnerName => Owner?.DisplayName;
    public bool IsActive => Entity.GetPublicValue<string>(Constants.GameMasterId, "activity") != null;

    public Character(IEntityCache entityCache, string id)
    {
        this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
        //this.game = game ?? throw new ArgumentNullException(nameof(game));
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }
}
