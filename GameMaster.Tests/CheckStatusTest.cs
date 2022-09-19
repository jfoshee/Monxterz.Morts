namespace GameMaster.Tests;

public class CheckStatusTest
{
    [Theory(DisplayName = "Check status right after start training"), MortsTest]
    public async Task StartTrain(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        var expected = 1;
        await game.Call.StartActivity(trainee, "train");

        await game.Call.CheckStatus(trainee);

        Assert.Equal(expected, game.State(trainee).strength);
    }

    [Theory(DisplayName = "Train for 2 hours"), MortsTest]
    public async Task TrainTwoHours(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        game.State(trainee).strength = 40;
        var expected = 40 + 2;
        await game.Call.StartActivity(trainee, "train");
        game.State(trainee).activityStart -= 2 * 60 * 60;

        await game.Call.CheckStatus(trainee);

        // trainee.GetPublicValue<int>("morts-game-master", "strength").Should().Be(expected);
        Assert.Equal(expected, game.State(trainee).strength);
    }

    [Theory(DisplayName = "Check Twice after 2 hours only accumulates once"), MortsTest]
    public async Task CheckTwice(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        game.State(trainee).strength = 40;
        var expected = 40 + 2;
        await game.Call.StartActivity(trainee, "train");
        game.State(trainee).activityStart -= 2 * 60 * 60;

        await game.Call.CheckStatus(trainee);
        await game.Call.CheckStatus(trainee);

        // trainee.GetPublicValue<int>("morts-game-master", "strength").Should().Be(expected);
        Assert.Equal(expected, game.State(trainee).strength);
    }

    [Theory(DisplayName = "Partial Accumulation"), MortsTest]
    public async Task PartialAccumulation(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        game.State(trainee).strength = 40;
        var expected = 40 + 6;
        await game.Call.StartActivity(trainee, "train");

        game.State(trainee).activityStart -= 2.5 * 60 * 60;
        await game.Call.CheckStatus(trainee);

        game.State(trainee).activityStart -= 3.5 * 60 * 60;
        await game.Call.CheckStatus(trainee);

        Assert.Equal(expected, game.State(trainee).strength);
    }
}
