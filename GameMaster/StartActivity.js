/// <reference path="_Shared.js" />

/** Start given activity for one of player's characters */
export function mutate(context, activity) {
  if (context.entities.length != 1) {
    throw Error('StartActivity function requires 1 Entity target.');
  }
  if (!validActivities.includes(activity)) {
    throw Error(`${activity} is not a valid activity.`);
  }
  const entity = context.entity;
  if (entity.systemState.ownerId != context.userId) {
    throw Error(`The character does not belong to the current Player. You cannot start ${activity} with another player's character.`);
  }
  const character = game.state(entity);
  if (isDead(character)) {
    throw Error(`The character cannot start ${activity} when dead.`);
  }
  if (character.activity) {
    throw Error(`The character cannot start ${activity} while already ${character.activity}.`);
  }
  // Convert milliseconds to seconds
  character.activityStart = nowSeconds();
  character.activity = activity;
  character.statusMessage = statusMessages[activity];
}
