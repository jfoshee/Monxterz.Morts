namespace GameMaster.Tests;

public class StopActivityTest
{
    [Theory(DisplayName = "Stop right after start training"), MortsTest]
    public async Task StartStop(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        var expected = 1;
        await game.Call.StartActivity(trainee, "training");

        await game.Call.StopActivity(trainee);

        Assert.Equal(expected, game.State(trainee).strength);
        Assert.Null(game.State(trainee).activity);
        Assert.Null(game.State(trainee).activityStart);
        Assert.Equal("Idle", game.State(trainee).statusMessage);
    }

    [Theory(DisplayName = "Stop after training for 2 hours"), MortsTest]
    public async Task TrainTwoHours(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        game.State(trainee).strength = 40;
        var expected = 40 + 2;
        await game.Call.StartActivity(trainee, "training");
        game.State(trainee).activityStart -= 2 * 60 * 60;

        await game.Call.StopActivity(trainee);

        Assert.Equal(expected, (int)game.State(trainee).strength);
    }
}
