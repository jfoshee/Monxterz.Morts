namespace GameClient.Blazor.ClientExtensions;

public interface ICharacterFactory
{
    Character Character(GameEntityState entity);
    IEnumerable<Character> Characters(IEnumerable<GameEntityState> entities);
}
