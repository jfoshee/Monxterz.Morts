/** Start training for one of player's characters */
export function mutate(context, attribute, seconds) {
  if (context.entities.length != 1) {
    throw Error('Train function requires 1 Entity targets: trainee');
  }
  const traineeEntity = context.entities[0];
  if (traineeEntity.systemState.ownerId != context.userId) {
    throw Error('The trainee character does not belong to the current Player. You cannot train another player\'s character.');
  }
  const trainee = traineeEntity.customStatePublic[context.authorId];
  if (trainee.hp <= 0) {
    throw Error('The character cannot train when dead.');
  }
  if (trainee.isRecovering) {
    throw Error('The character cannot train while recovering.');
  }
  // Force seconds to a number
  seconds = +seconds;
  // Convert milliseconds to seconds
  const start = Math.round(Date.now() / 1000);
  trainee.trainingStart = start;
  trainee.trainingEnd = start + seconds;
  trainee.trainingAttribute = attribute;
  trainee.isTraining = true;
}
