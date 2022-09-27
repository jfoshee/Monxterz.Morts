/** Character creation */
const newCharacterCooldownMinutes = 5;

/** Max distance changing location */
const maxMoveDistance = 1;

/** Attack recovery */
const recoverySeconds = 15;

/** Value accumulated per hour */
const activityRates = {
  'training': 1
};

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
  'recovering': 'Recovering (15 seconds)',
  // 'defending': 'Defending self and prepared to counter-attack!'
}
