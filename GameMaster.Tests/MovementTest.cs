namespace GameMaster.Tests;

public class MovementTest
{
    public async Task<GameEntityState> NewPlayer(IGameTestHarness game)
    {
        // Initialize player to outside game so character initialized at 80:80
        GameEntityState player = await game.NewCurrentPlayer();
        await game.Move(player, "00:00:00:00:00");
        return player;
    }

    [Theory(DisplayName = "Character initial position"), MortsTest]
    public async Task Initial(IGameTestHarness game)
    {
        await NewPlayer(game);
        var region = await game.GetRegion();
        GameEntityState character = await game.Create.Character();
        var location = character.SystemState.Location;

        Region.IsInside(region, location).Should().BeTrue();
        Region.GetChunks(location).Skip(4).Should().Equal("00", "80", "80");
    }

    [Theory(DisplayName = "Player can move anywhere"), MortsTest]
    public async Task MovePlayer(IGameTestHarness game)
    {
        // At this time players can "occupy" any cell... Might need to change this later.
        var player = await NewPlayer(game);
        var region = await game.GetRegion();
        var newLocation = Region.Combine(region, "00:42:24");

        await game.Move(player, newLocation);

        player.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move nowhere"), MortsTest]
    public async Task MoveNowhere(IGameTestHarness game)
    {
        GameEntityState character = await game.Create.Character();
        var location = character.SystemState.Location;

        await game.Move(character, location);

        character.SystemState.Location.Should().Be(location);
    }

    [Theory(DisplayName = "Move x +1"), MortsTest]
    public async Task MoveRight(IGameTestHarness game)
    {
        var player = await NewPlayer(game);
        var region = await game.GetRegion();
        var newLocation = Region.Combine(region, "00:81:80");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move x -1"), MortsTest]
    public async Task MoveLeft(IGameTestHarness game)
    {
        var player = await NewPlayer(game);
        var region = await game.GetRegion();
        var newLocation = Region.Combine(region, "00:7F:80");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move x +2"), MortsTest]
    public async Task MoveX2(IGameTestHarness game)
    {
        var player = await NewPlayer(game);
        var region = await game.GetRegion();
        var newLocation = Region.Combine(region, "00:82:80");
        GameEntityState character = await game.Create.Character();
        var location = character.SystemState.Location;

        await game.Invoking(g => (Task)g.Move(character, newLocation))
                  .Should()
                  .ThrowAsync<Exception>()
                  .WithMessage("*May not move more than 1*");
        character.SystemState.Location.Should().Be(location);
    }

    [Theory(DisplayName = "Move y +1"), MortsTest]
    public async Task MoveUp(IGameTestHarness game)
    {
        var player = await NewPlayer(game);
        var region = await game.GetRegion();
        var newLocation = Region.Combine(region, "00:80:81");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move y -1"), MortsTest]
    public async Task MoveDown(IGameTestHarness game)
    {
        var player = await NewPlayer(game);
        var region = await game.GetRegion();
        var newLocation = Region.Combine(region, "00:80:7F");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move y +2"), MortsTest]
    public async Task MoveY2(IGameTestHarness game)
    {
        var player = await NewPlayer(game);
        var region = await game.GetRegion();
        var newLocation = Region.Combine(region, "00:80:82");
        GameEntityState character = await game.Create.Character();
        var location = character.SystemState.Location;

        await game.Invoking(g => (Task)g.Move(character, newLocation))
                  .Should()
                  .ThrowAsync<Exception>()
                  .WithMessage("*May not move more than 1*");
        character.SystemState.Location.Should().Be(location);
    }

    // Cannot move to another "plane"
}
