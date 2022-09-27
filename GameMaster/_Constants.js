/**
 * The Game Master ID
 * This must match configuration in <monxterz.proj.json>
 */
const GameMasterEntityId = "morts-game-master";

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
  'defending'
];

const statusMessages = {
  null: 'Idle',
  'gathering': 'Gathering grass and stones',
  'training': 'Training: Gaining 1 Strength per Hour',
  'recovering': 'Recovering (15 seconds)',
  'defending': 'Defending self and prepared to counter-attack!',
  'dead': 'Dead'
}

/** Harness for accessing Game state */
const game = {
  /** Returns the public state for this game for the given entity */
  state: function(entity) {
    return entity.customStatePublic[GameMasterEntityId];
  }
};
