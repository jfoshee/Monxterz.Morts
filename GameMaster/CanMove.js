/** 
 * Determines whether an attempted move is allowed. 
 * Enforces 2D Cartesian grid movement.
 * Throws an error for illegal moves.
 */
function canMove(context, oldLocation, newLocation, oldRegion, newRegion) {
    if (oldRegion !== newRegion)
        throw Error('Changing regions is not implemented');
    CheckDistance(oldLocation, newLocation, 0);
    CheckDistance(oldLocation, newLocation, 1);
    // TODO: Check distance
    // TODO: Check plane (chunk[4])
}

function CheckDistance(oldLocation, newLocation, axis) {
    const oldPosition = getPosition(oldLocation, axis);
    const newPosition = getPosition(newLocation, axis);
    const distance = Math.abs(newPosition - oldPosition);
    const maxDistance = 1;
    if (distance > maxDistance)
        throw Error(`May not move more than ${maxDistance}, but attempted to move ${distance}`);
}

function getPosition(location, axis) {
    // Skip 5 chunks (4 for the region, 1 for the game plane)
    const chunks = location.split(':').slice(5);
    const positionString = chunks[axis];
    // Parse from Hex
    return parseInt(positionString, 16);
}
