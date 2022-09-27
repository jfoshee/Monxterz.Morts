/// <reference path="_Constants.js" />

/** Returns true if the character is dead. */
export function isDead(characterState) {
  return +characterState.hp <= 0;
}

// TODO: This function copied in StopActivity.js
export function accumulateValue(state) {
  const now = Math.ceil(Date.now() / 1000);
  const elapsedSeconds = now - +state.activityStart;
  const elapsedHours = elapsedSeconds / 60 / 60;
  const roundedHours = Math.floor(elapsedHours);
  const valuePerHour = activityRates[state.activity];
  if (!valuePerHour) {
    return;
  }
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
