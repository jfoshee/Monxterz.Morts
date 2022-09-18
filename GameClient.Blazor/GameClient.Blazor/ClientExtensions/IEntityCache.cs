namespace GameClient.Blazor.ClientExtensions
{
    public interface IEntityCache
    {
        GameEntityState this[string id] { get; set; }

        GameEntityState Get(string id);
        void Set(string id, GameEntityState state);
    }
}