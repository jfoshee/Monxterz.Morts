/** Value accumulated per hour */
const activityRates = {
  'training': 1
};

// TODO: This function copied in CheckStatus.js
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
  if (character.hp <= 0) {
    throw Error(`The character cannot complete activity when dead.`);
  }
  // Award accumulated
  accumulateValue(character);
  character.activityStart = null;
  character.activity = null;
}
