namespace GameClient.Blazor.ClientExtensions;

public interface IEntityCache
{
    GameEntityState this[string id] { get; set; }

    IEnumerable<GameEntityState> Entities { get; }

    GameEntityState? Get(string id);
    void Set(GameEntityState entity);
    void Set(IEnumerable<GameEntityState> entities);
}
