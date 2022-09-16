namespace GameClient.Blazor.ClientExtensions;

public class Character
{
    public GameEntityState Entity { get; set; }
    public string Id => Entity.Id!;
    public string Name => Entity.DisplayName ?? Id;
    public string OwnerId => Entity.SystemState.OwnerId;
    public string Location => Entity.SystemState.Location;

    public Character(GameEntityState entity)
    {
        Entity = entity ?? throw new ArgumentNullException(nameof(entity));
    }
}
