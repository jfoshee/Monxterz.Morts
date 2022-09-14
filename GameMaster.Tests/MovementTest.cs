using Monxterz.StatePlatform;

namespace GameMaster.Tests;

public class MovementTest
{
    [Theory(DisplayName = "Character initial position"), MortsTest]
    public async Task Initial(IGameTestHarness game, IGameMasterService gameMasterService)
    {
        var gameMaster = await gameMasterService.GetGameMaster();
        var region = gameMaster.SystemState.ControlledRegion!;
        GameEntityState character = await game.Create.Character();
        var location = character.SystemState.Location;

        Region.IsInside(region, location).Should().BeTrue();
        Region.GetChunks(location).Skip(4).Should().Equal("00", "80", "80");
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
    public async Task MoveRight(IGameTestHarness game, IGameMasterService gameMasterService)
    {
        var gameMaster = await gameMasterService.GetGameMaster();
        var region = gameMaster.SystemState.ControlledRegion!;
        var newLocation = Region.Combine(region, "00:81:80");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move x -1"), MortsTest]
    public async Task MoveLeft(IGameTestHarness game, IGameMasterService gameMasterService)
    {
        var gameMaster = await gameMasterService.GetGameMaster();
        var region = gameMaster.SystemState.ControlledRegion!;
        var newLocation = Region.Combine(region, "00:7F:80");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move x +2"), MortsTest]
    public async Task MoveX2(IGameTestHarness game, IGameMasterService gameMasterService)
    {
        var gameMaster = await gameMasterService.GetGameMaster();
        var region = gameMaster.SystemState.ControlledRegion!;
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
    public async Task MoveUp(IGameTestHarness game, IGameMasterService gameMasterService)
    {
        var gameMaster = await gameMasterService.GetGameMaster();
        var region = gameMaster.SystemState.ControlledRegion!;
        var newLocation = Region.Combine(region, "00:80:81");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move y -1"), MortsTest]
    public async Task MoveDown(IGameTestHarness game, IGameMasterService gameMasterService)
    {
        var gameMaster = await gameMasterService.GetGameMaster();
        var region = gameMaster.SystemState.ControlledRegion!;
        var newLocation = Region.Combine(region, "00:80:7F");
        GameEntityState character = await game.Create.Character();

        await game.Move(character, newLocation);

        character.SystemState.Location.Should().Be(newLocation);
    }

    [Theory(DisplayName = "Move y +2"), MortsTest]
    public async Task MoveY2(IGameTestHarness game, IGameMasterService gameMasterService)
    {
        var gameMaster = await gameMasterService.GetGameMaster();
        var region = gameMaster.SystemState.ControlledRegion!;
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
