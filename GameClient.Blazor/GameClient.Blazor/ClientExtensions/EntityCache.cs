namespace GameClient.Blazor.ClientExtensions;

public class EntityCache : IEntityCache
{
    private readonly Dictionary<string, GameEntityState> cache = new();

    public GameEntityState this[string id] { get => cache[id]; set => Set(value); }

    public GameEntityState? Get(string id) => CollectionExtensions.GetValueOrDefault(cache, id);
    public void Set(GameEntityState entity) => cache[entity.Id!] = entity;

    public void Add(IEnumerable<GameEntityState> entities)
    {
        foreach (var entity in entities)
            Set(entity);
    }
}
