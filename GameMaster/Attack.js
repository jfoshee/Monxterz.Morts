/// <reference path="_Shared.js" />

/** Attack an enemy in the same battle */
export function mutate(context) {
  if (context.entities.length != 2) {
    throw Error('Attack function requires 2 Entity targets: attacker, defender');
  }
  const attackerEntity = context.entities[0];
  const attacker = attackerEntity.customStatePublic[context.authorId];
  const defenderEntity = context.entities[1];
  const defender = defenderEntity.customStatePublic[context.authorId];
  if (attackerEntity.systemState.ownerId !== context.userId) {
    throw Error('The attacker character does not belong to the current Player. You cannot attack with another player\'s character.');
  }
  if (attackerEntity.systemState.location !== defenderEntity.systemState.location) {
    throw Error('The character cannot attack a defender in another location.');
  }
  if (isDead(attacker)) {
    throw Error('The character cannot attack when dead.');
  }
  if (attacker.activity) {
    throw Error(`The character cannot attack while ${attacker.activity}.`);
  }
  if (isDead(defender)) {
    // The character is already dead
    defender.hp = 0;
    return;
  }
  attacker.activity = 'recovering';
  attacker.statusMessage = "Recovering (15 seconds)";
  // Convert milliseconds to seconds
  const start = Math.floor(Date.now() / 1000);
  attacker.activityStart = start;
  defender.hp -= attacker.strength;
  if (defender.hp < 0) {
    defender.hp = 0;
    // TODO: To be fair to defender, should accumulate any resources that were gathered until time of death
    defender.activity = null;
    defender.statusMessage = "Dead";
  }
  defender.attackedById = attackerEntity.id;
}
