/** Check status and, if applicable, complete character recovery and training */
export function mutate(context) {
  if (context.entities.length != 1) {
    throw Error('CheckStatus function requires 1 Entity targets: trainee');
  }
  const entity = context.entities[0];
  if (entity.systemState.ownerId != context.userId) {
    throw Error('The trainee character does not belong to the current Player. You cannot train another player\'s character.');
  }
  const state = entity.customStatePublic[context.authorId];
  if (state.hp <= 0) {
    throw Error('The character is dead and cannot complete training.');
  }
  // Convert milliseconds to seconds
  // Rounding up to give benefit to slightly early status check
  const now = Math.ceil(Date.now() / 1000);
  // state.statusCheckTime = now;
  if (state.trainingEnd && state.trainingEnd <= now) {
    // Training is complete
    state.trainingStart = 
    state.trainingEnd = 
    state.trainingAttribute = null;
    state.isTraining = false;
    // TODO: How much strength to add? training rate...
    state.strength += 1;
  }
  if (state.recoveringEnd && state.recoveringEnd <= now) {
    state.recoveringStart =
    state.recoveringEnd = null;
    state.isRecovering = false;
  }
}
