using Microsoft.Extensions.Logging;

namespace GameClient.Blazor.ClientExtensions;

public class CharacterFactory : ICharacterFactory
{
    private readonly IEntityCache entityCache;
    private readonly IGameTestHarness game;

    public CharacterFactory(IEntityCache entityCache, IGameTestHarness game)
    {
        this.entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
        this.game = game ?? throw new ArgumentNullException(nameof(game));
    }

    public async Task<Character> CreateNew()
    {
        // HACK: Must initialize the game harness before calling game.Create. As long as Game Harness is not a singleton.
        await game.InitAsync();
        GameEntityState entity = await game.Create.Character();
        entityCache.Set(entity);
        return Character(entity);
    }

    public Character Character(GameEntityState entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));
        if (!entity.IsCharacter())
            throw new ArgumentException("The entity is not a Character");
        return new Character(entityCache, entity.Id!);
    }

    public IEnumerable<Character> Characters(IEnumerable<GameEntityState> entities)
    {
        if (entities is null)
            throw new ArgumentNullException(nameof(entities));
        return entities.Where(e => e.IsCharacter())
                       .Select(e => new Character(entityCache, e.Id!));
    }
}
