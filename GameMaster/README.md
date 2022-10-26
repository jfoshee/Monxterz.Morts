# Game Master

This directory contains the scripts for the Game Master. 
The Game Master controls the Game and enforces rules around state changes.

The scripts are executed on a Monxterz.StatePlatform server which is a back-end for persistent worlds.

## Need from Game State System

- CLI
  - Init
    - creates a new gamemaster.json
    - creates the unit test project
  - New script
    - mutator
    - initializer
- Game test harness
  - Publish project when tests startup (not per test)
  - inject the current player
  - get current player (updated after custom creation or function)
