namespace GameMaster.Tests;

public class AttackTest
{
    [Theory(DisplayName = "Basic"), MortsTest]
    public async Task BasicAttack(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        // Initialize attacker strength & defender hp
        game.State(attacker).strength = 13;
        game.State(defender).hp = 17;

        await game.Call.Attack(attacker, defender);

        // Enemy's hp should be reduced by the attacker's strength
        Assert.Equal(4, game.State(defender).hp);
        Assert.Equal(attacker.Id, game.State(defender).attackedById);
    }

    [Theory(DisplayName = "Missing Attacked & Defender"), MortsTest]
    public async Task MissingAttacker(IGameTestHarness game)
    {
        await game.Invoking(async g => await (Task)g.Call.Attack())
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*attacker*");
    }

    [Theory(DisplayName = "Missing Defender"), MortsTest]
    public async Task MissingDefender(IGameTestHarness game)
    {
        GameEntityState attacker = await game.Create.Character();

        await game.Invoking(async g => await (Task)g.Call.Attack(attacker))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*defender*");
    }

    [Theory(DisplayName = "Kill"), MortsTest]
    public async Task Kill(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        // Initialize attacker strength & defender hp
        game.State(attacker).strength = 20;
        game.State(defender).hp = 17;

        await game.Call.Attack(attacker, defender);

        // Enemy's hp should be reduced by the attacker's strength
        Assert.Equal(0, game.State(defender).hp);
        Assert.Null(game.State(defender).activity);
        Assert.Equal("Dead", game.State(defender).statusMessage);
    }

    [Theory(DisplayName = "Already Dead"), MortsTest]
    public async Task AlreadyDead(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        // Initialize attacker strength & defender hp
        game.State(defender).hp = 0;
        game.State(defender).attackedById = "expected";

        await game.Call.Attack(attacker, defender);

        // Enemy's hp should remain 0
        Assert.Equal(0, game.State(defender).hp);
        // Final attacked ID should not be replaced
        Assert.Equal("expected", game.State(defender).attackedById);
        // The attacker is not recovering (?)
        Assert.Null(game.State(attacker).activity);
    }

    [Theory(DisplayName = "Cannot Kill after Death"), MortsTest]
    public async Task CannotKillPostDeath(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        // Initialize attacker strength & defender hp
        game.State(attacker).hp = 0;
        game.State(attacker).strength = 20;
        game.State(defender).hp = 25;

        await game.Invoking(async g => await (Task)g.Call.Attack(attacker, defender))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*cannot attack*dead*");

        // Enemy's hp not should be reduced
        Assert.Equal(25, game.State(defender).hp);
    }

    [Theory(DisplayName = "Cannot attack w/ another player's character"), MortsTest]
    public async Task AnotherPlayersCharacter(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        await game.NewCurrentPlayer();

        await game.Invoking(async g => await (Task)g.Call.Attack(attacker, defender))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*cannot attack*player*");
    }

    [Theory(DisplayName = "Cannot attack character in another location"), MortsTest]
    public async Task DifferentLocations(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        var newLocation = Region.IncrementChunk(attacker.SystemState.Location, 6, 1);
        await game.Move(attacker, newLocation);

        await game.Invoking(async g => await (Task)g.Call.Attack(attacker, defender))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*cannot attack* location*");
    }

    [Theory(DisplayName = "Recovering"), MortsTest]
    public async Task Recovering(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        // Initially nobody is recovering
        Assert.Null(game.State(attacker).activity);
        Assert.Null(game.State(defender).activity);

        await game.Call.Attack(attacker, defender);

        // After attack Attacker is recovering
        Assert.Equal("recovering", game.State(attacker).activity);
        Assert.Null(game.State(defender).activity);
        await game.Invoking(async g => await (Task)g.Call.Attack(attacker, defender))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*cannot attack while recovering*");
    }

    [Theory(DisplayName = "Recovered"), MortsTest]
    public async Task Recovered(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        // Assert.Equal(10, game.State(attacker).recoveryTime);
        // game.State(attacker).recoveryTime = 2;

        await game.Call.Attack(attacker, defender);
        Assert.Equal("recovering", game.State(attacker).activity);

        // Modify recovery start time so recovery period has elapsed
        game.State(attacker).activityStart -= 15;
        await game.Call.CheckStatus(attacker);

        Assert.Null(game.State(attacker).activity);
        await game.Call.Attack(attacker, defender);
    }

    [Theory(DisplayName = "Defender Auto Counter-Attack"), MortsTest]
    public async Task AutoCounterAttack(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        game.State(defender).strength = 95;
        await game.Call.StartActivity(defender, "defending");
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();

        await game.Call.Attack(attacker, defender);

        Assert.Equal(99, game.State(defender).hp);
        Assert.Equal(5, game.State(attacker).hp);
        Assert.Equal(defender.Id, game.State(attacker).attackedById);
    }

    [Theory(DisplayName = "Counter-Attack Kills Attacker"), MortsTest]
    public async Task FatalCounterAttack(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        game.State(defender).strength = 101;
        await game.Call.StartActivity(defender, "defending");
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();

        await game.Call.Attack(attacker, defender);

        Assert.Equal(99, game.State(defender).hp);
        Assert.Equal(0, game.State(attacker).hp);
        Assert.Null(game.State(attacker).activity);
        Assert.Equal("Dead", game.State(attacker).statusMessage);
        Assert.Equal(defender.Id, game.State(attacker).attackedById);
    }

    [Theory(DisplayName = "Dead Defender Cannot Counter-Attack"), MortsTest]
    public async Task CannotCounterAttack(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        game.State(defender).strength = 95;
        game.State(defender).hp = 1;
        await game.Call.StartActivity(defender, "defending");
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();

        await game.Call.Attack(attacker, defender);

        Assert.Equal(0, game.State(defender).hp);
        Assert.Equal("Dead", game.State(defender).statusMessage);
        Assert.Equal(100, game.State(attacker).hp);
        Assert.Null(game.State(attacker).attackedById);
    }
}
