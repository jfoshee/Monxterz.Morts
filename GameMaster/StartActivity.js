/// <reference path="_Shared.js" />

const validActivities = [
  'gathering',
  'training',
  'recovering',
  // protecting
  // 'counterAttacking'
  // 'defending'
];

const statusMessages = {
  null: 'Idle',
  'gathering': 'Gathering grass and stones',
  'training': 'Training: Gaining 1 Strength per Hour',
  'recovering': 'Recovering',
  // 'defending': 'Defending self and prepared to counter-attack!'
}

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
  const character = entity.customStatePublic[context.authorId];
  if (isDead(character)) {
    throw Error(`The character cannot start ${activity} when dead.`);
  }
  if (character.activity) {
    throw Error(`The character cannot start ${activity} while already ${character.activity}.`);
  }
  // Convert milliseconds to seconds
  const start = Math.round(Date.now() / 1000);
  character.activityStart = start;
  character.activity = activity;
  character.statusMessage = statusMessages[activity];
}
