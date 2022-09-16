/** Initializes a new Character */
function initialize(context) {
  Cooldown(context);
  const entity = context.entity;
  entity.displayName = 'New Character';
  initLocation(context, entity);
  initState(context, entity);
}

/** Cooldown for character creation */
function Cooldown(context) {
  const user = context.user;
  userState = user.customStatePublic[context.authorId];
  const delayMinutes = 5;
  const delaySeconds = 60 * delayMinutes;
  const nowSeconds = Math.floor(Date.now() / 1000);
  if (userState.characterCreatedAt && +userState.characterCreatedAt + delaySeconds > nowSeconds) {
    throw Error('⏳ You must wait 5 minutes between creating each New Character.');
  }
  userState.characterCreatedAt = nowSeconds;
}

function initState(context, entity) {
  let state = entity.customStatePublic[context.authorId];
  state.type = 'Character';
  state.hp = 100;
  state.xp = 0;
  state.strength = 1;
  state.isTraining = false;
  state.isRecovering = false;
  state.recoveryTime = 10;
}

function initLocation(context, entity) {
  const author = context.author;
  let region = author.systemState.controlledRegion;
  region = region.replace('::', ':');
  const user = context.user;
  // TODO: Check user location has 3 chunks in subregion
  if (!user.systemState.location.startsWith(region)) {
    entity.systemState.location = region + '00:80:80';
  }
}
