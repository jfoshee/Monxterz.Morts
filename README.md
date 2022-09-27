# Monxterz.Morts

A "MMORTS" game built on my Monxterz.StatePlatform back-end.  The back-end is responsible for maintaining state and enforcing state mutation rules.
However the back-end remains agnostic to specifics about the particular game.

JS scripts define state mutation rules. These scripts run on the server in a sandbox. Check out the following examples for how state is mutated and maintained.

### Back-End Game Script Examples

#### [Character.js](GameMaster/Character.js)
```js
let state = game.state(entity);
state.type = 'Character';
state.hp = 100;
state.xp = 0;
```

#### [Attack.js](GameMaster/Attack.js)
```js
const attackerEntity = context.entities[0];
const attacker = game.state(attackerEntity);
const defenderEntity = context.entities[1];
const defender = game.state(defenderEntity);
defender.hp -= attacker.strength;
if (isDead(defender)) {
  defender.hp = 0;
  defender.statusMessage = 'Dead';
}
```

### Back-End Script Integration Tests

The platform makes testing these scripts straightforward. 
The tests are written in C# to match the rest of the stack. 
This might seem a little strange, but these tests use a generated C# GameStateClient which can also be used in the Game UI itself.

#### [CharacterTest.cs](GameMaster.Tests/CharacterTest.cs)
```cs
GameEntityState character = await game.Create.Character();
var characterState = game.State(character);
Assert.Equal("Character", characterState.type);
Assert.Equal(100, characterState.hp);
Assert.Equal(0, characterState.xp);
```

#### [AttackTest.cs](GameMaster.Tests/AttackTest.cs)
```cs
game.State(attacker).strength = 13;
game.State(defender).hp = 17;

await game.Call.Attack(attacker, defender);

// Enemy's hp should be reduced by the attacker's strength
Assert.Equal(4, game.State(defender).hp);
```

### Front-End Game Client

You can use the game engine of your choice for the front-end. 
This example uses Blazor because it makes for very rapid development for me. 
I have Unity examples as well.

#### [Index.razor.cs](GameClient.Blazor/GameClient.Blazor/Pages/Index.razor.cs)
```cs
async Task Attack()
{
    ...
    await game.Call.Attack(selectedCharacter.Entity, selectedTheirCharacter.Entity);
    ...
}
```

### Terminology

MORTS: Multiplayer Online Real-Time Strategy (Game)
