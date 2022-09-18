namespace GameClient.Blazor.ClientExtensions;

public class Character
{
    private readonly IEntityCache entityCache;
    public GameEntityState Entity { get; set; }
    public string Id => Entity.Id!;
    public string Name => Entity.DisplayName ?? Id;
    public string OwnerId => Entity.SystemState.OwnerId;
    public string Location => Entity.SystemState.Location;

    public Character(IEntityCache entityCache, GameEntityState entity)
    {
        this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
        Entity = entity ?? throw new ArgumentNullException(nameof(entity));
    }
}
