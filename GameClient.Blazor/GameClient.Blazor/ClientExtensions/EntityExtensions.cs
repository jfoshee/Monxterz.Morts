namespace GameClient.Blazor.ClientExtensions;

public static class EntityExtensions
{
    public static bool IsCharacter(this GameEntityState entity)
    {
        var type = entity.GetPublicValue<string>(Constants.GameMasterId, "type");
        return type == "Character";
    }

    public static IEnumerable<Character> Characters(this IEnumerable<GameEntityState> entities)
    {
        if (entities is null)
        {
            throw new ArgumentNullException(nameof(entities));
        }
        return entities.Where(IsCharacter).Select(e => new Character(e));
    }
}
