namespace GameMaster.Tests;

public class StartActivityTest
{
    [Theory(DisplayName = "Start training"), MortsTest]
    public async Task StartTrain(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        Assert.Null(game.State(trainee).activity);
        var expectedStart = DateTimeOffset.Now.ToUnixTimeSeconds();

        await game.Call.StartActivity(trainee, "training");
        
        Assert.Equal("training", game.State(trainee).activity);
        AssertClose(expectedStart, game.State(trainee).activityStart);
    }

    // [Theory(DisplayName = "Check unfinished 'strength' training"), MortsTest]
    // public async Task ContinueStrength(IGameTestHarness game)
    // {
    //     GameEntityState trainee = await game.Create.Character();
    //     await game.Call.Train(trainee, "strength", 4);
    //     await game.Call.CheckStatus(trainee);
    //     await Task.Delay(2_000);
        
    //     await game.Call.CheckStatus(trainee);
        
    //     Assert.True(game.State(trainee).isTraining);
    //     Assert.Equal("strength", game.State(trainee).trainingAttribute);
    //     Assert.NotNull(game.State(trainee).trainingStart);
    //     Assert.NotNull(game.State(trainee).trainingEnd);
    // }

    // [Theory(DisplayName = "Cannot Attack while training"), MortsTest]
    // public async Task CannotAttack(IGameTestHarness game)
    // {
    //     GameEntityState enemy = await game.Create.Character();
    //     await game.NewCurrentPlayer();
    //     GameEntityState trainee = await game.Create.Character();
    //     await game.Call.Train(trainee, "strength", 1);
 
    //     await game.Invoking(async g => await (Task)g.Call.Attack(trainee, enemy))
    //               .Should()
    //               .ThrowAsync<ApiException>()
    //               .WithMessage("*training*");
    // }

    [Theory(DisplayName = "Cannot Gather while Training"), MortsTest]
    public async Task CannotGather(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        await game.Call.StartActivity(trainee, "training");
 
        await game.Invoking(async g => await (Task)g.Call.StartActivity(trainee, "gathering"))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*The character cannot start gathering while already training*");
    }

    [Theory(DisplayName = "Cannot start Invalid Activity"), MortsTest]
    public async Task Invalid(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
 
        await game.Invoking(async g => await (Task)g.Call.StartActivity(trainee, "spinning"))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*spinning is not a valid activity*");
    }

    void AssertClose(long expected, dynamic actual)
    {
        var actualValue = Convert.ToInt64(actual);
        Assert.InRange(actualValue, expected - 1, expected + 1);
    }
}
