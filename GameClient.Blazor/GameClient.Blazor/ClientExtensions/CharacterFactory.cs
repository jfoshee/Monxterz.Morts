namespace GameClient.Blazor.ClientExtensions;

public class CharacterFactory : ICharacterFactory
{
    private readonly IEntityCache entityCache;

    public CharacterFactory(IEntityCache entityCache)
    {
        this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
    }

    public Character Character(GameEntityState entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));
        if (!entity.IsCharacter())
            throw new ArgumentException("The entity is not a Character");
        return new Character(entityCache, entity);
    }

    public IEnumerable<Character> Characters(IEnumerable<GameEntityState> entities)
    {
        if (entities is null)
            throw new ArgumentNullException(nameof(entities));
        return entities.Where(e => e.IsCharacter())
                       .Select(e => new Character(entityCache, e));
    }
}
