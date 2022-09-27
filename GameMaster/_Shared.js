/// <reference path="_Constants.js" />

/** Returns true if the character is dead. */
export function isDead(characterState) {
  return +characterState.hp <= 0;
}

/** Returns number of whole elapsed seconds since the epoch */
export function nowSeconds() {
  // Convert milliseconds to seconds
  return Math.floor(Date.now() / 1000);
}

export function accumulateValue(state) {
  const now = nowSeconds();
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
