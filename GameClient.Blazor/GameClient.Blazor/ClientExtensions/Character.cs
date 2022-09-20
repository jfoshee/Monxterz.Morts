namespace GameClient.Blazor.ClientExtensions;

public class Character : IEquatable<Character?>
{
    private readonly IEntityCache entityCache;
    //private readonly IGameTestHarness game;
    public string Id { get; }
    public GameEntityState Entity => entityCache[Id];
    public string Name => OwnerName ?? Entity.DisplayName ?? Id;
    public string OwnerId => Entity.SystemState.OwnerId;
    public string Location => Entity.SystemState.Location;
    public GameEntityState? Owner => entityCache.Get(Entity.SystemState.OwnerId);
    public string? OwnerName => Owner?.DisplayName;
    public bool IsActive => Get<string>("activity") != null;
    public float Strength => Get<float>("strength");
    public string? StatusMessage => Get<string>("statusMessage") ?? Get<string>("activity");


    public Character(IEntityCache entityCache, string id)
    {
        this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
        //this.game = game ?? throw new ArgumentNullException(nameof(game));
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    private T? Get<T>(string propertyName)
    {
        return Entity.GetPublicValue<T>(Constants.GameMasterId, propertyName);
    }

    #region Equality (for testing if is selected)
    public override bool Equals(object? obj)
    {
        return Equals(obj as Character);
    }

    public bool Equals(Character? other)
    {
        return other is not null &&
               Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public static bool operator ==(Character? left, Character? right)
    {
        return EqualityComparer<Character>.Default.Equals(left, right);
    }

    public static bool operator !=(Character? left, Character? right)
    {
        return !(left == right);
    }
    #endregion
}
