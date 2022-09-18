const validActivities = [
  'gather',
  'train',
  'recover'
];

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
    throw Error(`The character does not belong to the current Player. You cannot ${activity} with another player's character.`);
  }
  const character = entity.customStatePublic[context.authorId];
  if (character.hp <= 0) {
    throw Error(`The character cannot ${activity} when dead.`);
  }
  if (character.activity) {
    throw Error(`The character cannot ${activity} while ${character.activity}ing.`);
  }
  // Convert milliseconds to seconds
  const start = Math.round(Date.now() / 1000);
  character.activityStart = start;
  character.activity = activity;
}
