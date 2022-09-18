namespace GameClient.Blazor.ClientExtensions;

public class EntityCache : IEntityCache
{
    private readonly Dictionary<string, GameEntityState> cache = new();

    public GameEntityState this[string id] { get => Get(id); set => Set(id, value); }
    public GameEntityState Get(string id) => cache[id];
    public void Set(string id, GameEntityState state) => cache[id] = state;
}
