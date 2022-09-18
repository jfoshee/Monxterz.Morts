namespace GameClient.Blazor.ClientExtensions;

public class Character
{
    private readonly IEntityCache entityCache;
    public string Id { get; }
    public GameEntityState Entity => entityCache[Id];
    public string Name => OwnerName ?? Entity.DisplayName ?? Id;
    public string OwnerId => Entity.SystemState.OwnerId;
    public string Location => Entity.SystemState.Location;
    public GameEntityState? Owner => entityCache.Get(Entity.SystemState.OwnerId);
    public string? OwnerName => Owner?.DisplayName;

    public Character(IEntityCache entityCache, string id)
    {
        this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }
}
