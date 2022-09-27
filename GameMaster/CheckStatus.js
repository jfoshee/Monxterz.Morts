/// <reference path="_Shared.js" />

/** Check status and, if applicable, complete character recovery and training */
export function mutate(context) {
  if (context.entities.length != 1) {
    throw Error('CheckStatus function requires 1 Entity target');
  }
  const entity = context.entity;
  if (entity.systemState.ownerId != context.userId) {
    throw Error(`The character does not belong to the current Player. You cannot check the status of another player's character.`);
  }
  const character = game.state(entity);
  // If not doing any activity, exit
  if (!character.activity) {
    return;
  }
  if (isDead(character)) {
    throw Error(`The character is dead and cannot complete ${character.activity}.`);
  }
  if (character.activity === 'recovering') {
    const now = Math.ceil(Date.now() / 1000);
    const elapsedSeconds = now - +character.activityStart;
    if (elapsedSeconds >= recoverySeconds) {
      character.activityStart = null;
      character.activity = null;
      character.statusMessage = statusMessages[null];
    }
  } else {
    accumulateValue(character);
  }
}
