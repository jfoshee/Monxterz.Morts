/// <reference path="_Shared.js" />

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
  userState = game.state(user);
  const delayMinutes = newCharacterCooldownMinutes;
  const delaySeconds = 60 * delayMinutes;
  if (userState.characterCreatedAt && +userState.characterCreatedAt + delaySeconds > nowSeconds()) {
    throw Error(`⏳ You must wait ${newCharacterCooldownMinutes} minutes between creating each New Character.`);
  }
  userState.characterCreatedAt = nowSeconds();
}

function initState(context, entity) {
  let state = game.state(entity);
  state.type = 'Character';
  state.hp = 100;
  state.xp = 0;
  state.strength = 1;
  state.recoveryTime = 10;
  state.activity = null;
  state.statusMessage = statusMessages[null];
}

function initLocation(context, entity) {
  const author = context.author;
  let region = author.systemState.controlledRegion;
  if (!region) {
    throw Error('The Game Master does not have a Controlled Region.');
  }
  region = region.replace('::', ':');
  const user = context.user;
  // TODO: Check user location has 3 chunks in subregion
  if (!user.systemState.location.startsWith(region)) {
    entity.systemState.location = region + '00:80:80';
  }
}
