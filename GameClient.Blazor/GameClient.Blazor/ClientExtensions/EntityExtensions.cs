namespace GameClient.Blazor.ClientExtensions;

public static class EntityExtensions
{
    public static bool IsCharacter(this GameEntityState entity)
    {
        var type = entity.GetPublicValue<string>(Constants.GameMasterId, "type");
        return type == "Character";
    }
}
