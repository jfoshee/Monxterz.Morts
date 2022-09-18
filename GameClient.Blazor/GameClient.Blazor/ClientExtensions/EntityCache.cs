namespace GameClient.Blazor.ClientExtensions;

public class EntityCache : IEntityCache
{
    private readonly Dictionary<string, GameEntityState> cache = new();
    private readonly Dictionary<string, DateTimeOffset> cacheTime = new();
    private readonly ILogger<EntityCache> logger;

    public EntityCache(ILogger<EntityCache> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public GameEntityState this[string id] { get => cache[id]; set => Set(value); }
    public IEnumerable<GameEntityState> Entities => cache.Values;

    public GameEntityState? Get(string id) => CollectionExtensions.GetValueOrDefault(cache, id);

    public void Set(GameEntityState entity)
    {
        cache[entity.Id!] = entity;
        cacheTime[entity.Id!] = DateTimeOffset.UtcNow;
    }

    public void Set(IEnumerable<GameEntityState> entities)
    {
        foreach (var entity in entities)
            Set(entity);
    }
}
