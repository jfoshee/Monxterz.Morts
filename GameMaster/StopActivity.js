/// <reference path="_Shared.js" />

/** Stop current activity for one of player's characters */
export function mutate(context) {
  if (context.entities.length != 1) {
    throw Error('StopActivity function requires 1 Entity target.');
  }
  const entity = context.entity;
  if (entity.systemState.ownerId != context.userId) {
    throw Error(`The character does not belong to the current Player. You cannot stop activity for another player's character.`);
  }
  const character = entity.customStatePublic[context.authorId];
  // If not doing any activity, throw
  if (!character.activity) {
    throw Error(`The character has no current activity to stop.`);
  }
  if (isDead(character)) {
    throw Error(`The character cannot complete activity when dead.`);
  }
  if (character.activity === 'recovering') {
    throw Error('The character must finish recovering naturally.');
  }
  // Award accumulated
  accumulateValue(character);
  character.activityStart = null;
  character.activity = null;
  character.statusMessage = statusMessages[null];
}
