

/** Returns true if the character is dead. */
export function isDead(characterState) {
    return +characterState.hp <= 0;
}
