namespace GameClient.Blazor.ClientExtensions;

public interface ICharacterFactory
{
    Task<Character> CreateNew();
    Character Character(GameEntityState entity);
    IEnumerable<Character> Characters(IEnumerable<GameEntityState> entities);
}
