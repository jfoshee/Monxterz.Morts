namespace GameMaster.Tests;

public class CharacterTest
{
    [Theory(DisplayName = "Default"), MortsTest]
    public async Task DefaultCharacter(IGameTestHarness game)
    {
        GameEntityState character = await game.Create.Character();
        var characterState = game.State(character);
        Assert.Equal("Character", characterState.type);
        Assert.Equal(100, characterState.hp);
        Assert.Equal(0, characterState.xp);
        Assert.Equal(1, characterState.strength);
        Assert.Equal(10, characterState.recoveryTime);
        Assert.False(characterState.isTraining);
        Assert.False(characterState.isRecovering);
    }

    [Theory(DisplayName = "Delay between Character Creation"), MortsTest]
    public async Task Delays(IGameTestHarness game, IGameStateClient client)
    {
        await game.Create.Character();
        await game.Invoking(g => (Task)g.Create.Character())
                  .Should()
                  .ThrowAsync<Exception>()
                  .WithMessage("*wait*");
        var player = await client.GetUserAsync() ?? throw new Exception("No user");
        var nowSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
        Assert.InRange(game.State(player).characterCreatedAt, nowSeconds - 1, nowSeconds);

        // Force the creation time backwards so that we can create again
        game.State(player).characterCreatedAt = nowSeconds - 5 * 60;
        await game.Create.Character();
    }
}
