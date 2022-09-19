/** Value accumulated per hour */
const activityRates = {
  'train': 1
};


/** Check status and, if applicable, complete character recovery and training */
export function mutate(context) {
  if (context.entities.length != 1) {
    throw Error('CheckStatus function requires 1 Entity target');
  }
  const entity = context.entity;
  if (entity.systemState.ownerId != context.userId) {
    throw Error('The character does not belong to the current Player. You cannot check the status of another player\'s character.');
  }
  const state = entity.customStatePublic[context.authorId];
  // TODO: If not doing any activity, exit
  if (state.hp <= 0) {
    throw Error(`The character is dead and cannot complete ${state.activity}.`);
  }
  // Convert milliseconds to seconds
  // Rounding up to give benefit to slightly early status check
  const now = Math.ceil(Date.now() / 1000);
  const elapsedSeconds = now - +state.activityStart;
  const elapsedHours = elapsedSeconds / 60 / 60;
  const roundedHours = Math.floor(elapsedHours);
  const valuePerHour = activityRates[state.activity];
  const value = roundedHours * valuePerHour;
  state.strength = +state.strength + value;
  // Do not cheat the player out of partial time not yet accumulated
  const unusedHours = elapsedHours - roundedHours;
  state.activityStart = now - unusedHours * 60 * 60;
}
