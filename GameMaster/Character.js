/** Initializes a new Character */
function initialize(context) {
  const user = context.user;
  userState = user.customStatePublic[context.authorId];
  const delayMinutes = 5;
  const delaySeconds = 60 * delayMinutes; 
  const nowSeconds = Math.floor(Date.now() / 1000);
  if (userState.characterCreatedAt && +userState.characterCreatedAt + delaySeconds > nowSeconds) {
    throw Error('⏳ You must wait 10 minutes between creating each New Character.');
  }

  userState.characterCreatedAt = nowSeconds;
  const entity = context.entity;
  entity.displayName = 'New Character';
  const state = entity.customStatePublic[context.authorId];
  state.type = 'Character';
  state.hp = 100;
  state.xp = 0;
  state.strength = 1;
  state.isTraining = false;
  state.isRecovering = false;
  state.recoveryTime = 10;
}
