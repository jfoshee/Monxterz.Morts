/// <reference path="_Shared.js" />

/** Attack an enemy in the same battle */
export function mutate(context) {
  if (context.entities.length != 2) {
    throw Error('Attack function requires 2 Entity targets: attacker, defender');
  }
  const attackerEntity = context.entities[0];
  const attacker = game.state(attackerEntity);
  const defenderEntity = context.entities[1];
  const defender = game.state(defenderEntity);
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
  attacker.statusMessage = statusMessages[attacker.activity];
  attacker.activityStart = nowSeconds();
  // Attack: attacker => defender
  updateAttackStats(attackerEntity, defenderEntity);
  // If defender is not dead, has chance to defend self
  if (!isDead(defender) && defender.activity === 'defending') {
    // Counter-Attack: defender => attacker
    updateAttackStats(defenderEntity, attackerEntity);
  }
}

function updateAttackStats(attackerEntity, defenderEntity) {
  const attacker = game.state(attackerEntity);
  const defender = game.state(defenderEntity);
  defender.hp -= attacker.strength;
  defender.attackedById = attackerEntity.id;
  if (isDead(defender)) {
    defender.hp = 0;
    // TODO: To be fair to defender, should accumulate any resources that were gathered until time of death
    defender.activity = null;
    defender.statusMessage = statusMessages['dead'];
  }
}
