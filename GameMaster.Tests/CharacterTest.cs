namespace GameMaster.Tests;

public class CharacterTest
{
    [Theory(DisplayName = "Default"), MortsTest]
    public async Task DefaultCharacter(IGameTestHarness game)
    {
        // Initialize player to outside game so character initialized at 80:80
        var player = await game.NewCurrentPlayer();
        await game.Move(player, "00:00:00:00:00");
        var region = await game.GetRegion();
        var defaultLocation = Region.Combine(region, "00:80:80");

        GameEntityState character = await game.Create.Character();

        character.SystemState.Location.Should().Be(defaultLocation);
        var characterState = game.State(character);
        Assert.Equal("Character", characterState.type);
        Assert.Equal(100, characterState.hp);
        Assert.Equal(0, characterState.xp);
        Assert.Equal(1, characterState.strength);
        Assert.Equal(10, characterState.recoveryTime);
        Assert.Null(characterState.activity);
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

    [Theory(DisplayName = "Create at User Location (if inside game region)"), MortsTest]
    public async Task AtUserLocation(IGameTestHarness game)
    {
        var player = await game.NewCurrentPlayer();
        var region = await game.GetRegion();
        var location = Region.Combine(region, "12:34:56");
        await game.Move(player, location);

        GameEntityState character = await game.Create.Character();

        character.SystemState.Location.Should().Be(location);
    }    
}
