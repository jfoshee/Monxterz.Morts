/** Value accumulated per hour */
const activityRates = {
  'training': 1
};

// TODO: This function copied in StopActivity.js
function accumulateValue(state) {
  const now = Math.ceil(Date.now() / 1000);
  const elapsedSeconds = now - +state.activityStart;
  const elapsedHours = elapsedSeconds / 60 / 60;
  const roundedHours = Math.floor(elapsedHours);
  const valuePerHour = activityRates[state.activity];
  const value = roundedHours * valuePerHour;
  if (isNaN(state.strength) || state.strength == 'NaN') {
    // Fix up strength that somehow became NaN
    state.strength = 1;
  }
  state.strength = +state.strength + value;
  // Do not cheat the player out of partial time not yet accumulated
  const unusedHours = elapsedHours - roundedHours;
  state.activityStart = now - unusedHours * 60 * 60;
}

/** Check status and, if applicable, complete character recovery and training */
export function mutate(context) {
  if (context.entities.length != 1) {
    throw Error('CheckStatus function requires 1 Entity target');
  }
  const entity = context.entity;
  if (entity.systemState.ownerId != context.userId) {
    throw Error(`The character does not belong to the current Player. You cannot check the status of another player's character.`);
  }
  const character = entity.customStatePublic[context.authorId];
  // If not doing any activity, exit
  if (!character.activity) {
    return;
  }
  if (character.hp <= 0) {
    throw Error(`The character is dead and cannot complete ${character.activity}.`);
  }
  accumulateValue(character);
}
