namespace GameMaster.Tests;

public class TrainingTest
{
    [Theory(DisplayName = "Start 'strength' training"), MortsTest]
    public async Task StartStrength(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        Assert.False(game.State(trainee).isTraining);
        var expectedStart = DateTimeOffset.Now.ToUnixTimeSeconds();
        var expectedEnd = expectedStart + 30;

        await game.Call.Train(trainee, "strength", 30);
        
        Assert.True(game.State(trainee).isTraining);
        Assert.Equal("strength", game.State(trainee).trainingAttribute);
        AssertClose(expectedStart, game.State(trainee).trainingStart);
        AssertClose(expectedEnd, game.State(trainee).trainingEnd);
    }

    [Theory(DisplayName = "Check unfinished 'strength' training"), MortsTest]
    public async Task ContinueStrength(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        await game.Call.Train(trainee, "strength", 4);
        await game.Call.CheckStatus(trainee);
        await Task.Delay(2_000);
        
        await game.Call.CheckStatus(trainee);
        
        Assert.True(game.State(trainee).isTraining);
        Assert.Equal("strength", game.State(trainee).trainingAttribute);
        Assert.NotNull(game.State(trainee).trainingStart);
        Assert.NotNull(game.State(trainee).trainingEnd);
    }

    [Theory(DisplayName = "Cannot Attack while training"), MortsTest]
    public async Task CannotAttack(IGameTestHarness game)
    {
        GameEntityState enemy = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState trainee = await game.Create.Character();
        await game.Call.Train(trainee, "strength", 1);
 
        await game.Invoking(async g => await (Task)g.Call.Attack(trainee, enemy))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*training*");
    }

    [Theory(DisplayName = "Complete 'strength' training"), MortsTest]
    public async Task CompleteStrength(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        await game.Call.Train(trainee, "strength", 2);
        await Task.Delay(2_000);
        
        await game.Call.CheckStatus(trainee);

        Assert.False(game.State(trainee).isTraining);
        Assert.Equal(2, game.State(trainee).strength);
        Assert.Null(game.State(trainee).trainingAttribute);
        Assert.Null(game.State(trainee).trainingStart);
        Assert.Null(game.State(trainee).trainingEnd);
    }

    [Theory(DisplayName = "Already complete 'strength' training"), MortsTest]
    public async Task AlreadyCompleteStrength(IGameTestHarness game)
    {
        GameEntityState trainee = await game.Create.Character();
        await game.Call.Train(trainee, "strength", 1);
        await Task.Delay(1_500);
        await game.Call.CheckStatus(trainee);
        
        await game.Call.CheckStatus(trainee);

        Assert.False(game.State(trainee).isTraining);
        Assert.Equal(2, game.State(trainee).strength);
        Assert.Null(game.State(trainee).trainingAttribute);
        Assert.Null(game.State(trainee).trainingStart);
        Assert.Null(game.State(trainee).trainingEnd);
    }

    [Theory(DisplayName = "Recovering from Attack"), MortsTest]
    public async Task Recovering(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        await game.Call.Attack(attacker, defender);
        Assert.True(game.State(attacker).isRecovering);

        await game.Invoking(async g => await (Task)g.Call.Train(attacker, "strength", 1))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*cannot train*recovering*");
    }

    void AssertClose(long expected, dynamic actual)
    {
        var actualValue = Convert.ToInt64(actual);
        Assert.InRange(actualValue, expected - 1, expected + 1);
    }
}
