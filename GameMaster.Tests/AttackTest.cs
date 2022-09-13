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
        Assert.False(game.State(attacker).isRecovering);
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

    [Theory(DisplayName = "Recovering"), MortsTest]
    public async Task Recovering(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        // Initially nobody is recovering
        Assert.False(game.State(attacker).isRecovering);
        Assert.False(game.State(defender).isRecovering);

        await game.Call.Attack(attacker, defender);

        // After attack Attacker is recovering
        Assert.True(game.State(attacker).isRecovering);
        Assert.False(game.State(defender).isRecovering);        
        await game.Invoking(async g => await (Task)g.Call.Attack(attacker, defender))
                  .Should()
                  .ThrowAsync<ApiException>()
                  .WithMessage("*cannot attack*recovering*");
    }

    [Theory(DisplayName = "Recovered"), MortsTest]
    public async Task Recovered(IGameTestHarness game)
    {
        GameEntityState defender = await game.Create.Character();
        await game.NewCurrentPlayer();
        GameEntityState attacker = await game.Create.Character();
        Assert.Equal(10, game.State(attacker).recoveryTime);
        game.State(attacker).recoveryTime = 2;

        await game.Call.Attack(attacker, defender);
        Assert.True(game.State(attacker).isRecovering);

        await Task.Delay(2_000);
        await game.Call.CheckStatus(attacker);

        Assert.False(game.State(attacker).isRecovering);
        await game.Call.Attack(attacker, defender);
    }

    [Theory(DisplayName = "Handle missing Recovery Time"), MortsTest]
    public async Task MissingRecoveryTime(IGameTestHarness game, IGameStateClient client)
    {
        // Create an entity w/out recovery time
        GameEntityState attacker = await client.PostEntityNewAsync(null) ?? throw new Exception();
        GameEntityState defender = await game.Create.Character();
        game.State(attacker).strength = 1;

        await game.Call.Attack(attacker, defender);

        Assert.True(game.State(attacker).isRecovering);
        Assert.Equal(10, game.State(attacker).recoveryTime);
        var expectedStart = DateTimeOffset.Now.ToUnixTimeSeconds();
        var expectedEnd = expectedStart + 10;
        Assert.InRange<long>(game.State(attacker).recoveringStart, expectedStart - 1, expectedStart + 1);
        Assert.InRange<long>(game.State(attacker).recoveringEnd, expectedEnd - 1, expectedEnd + 1);
    }
}
