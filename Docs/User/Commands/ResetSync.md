# /ResetSync

## Basic Information

#### Description
Reset collectible synchronisations to new game state.

#### Aliases
- rs

#### Usages
- /resetsync

#### IsHostOnly?
- True

#### CanSpectatorRun?
- True

## More Information
The resetsync command is used to reset all collectibles to the "uncollected" state. This serves two purposes:
1. If you're starting a new co-op game with someone, you'll want to reset before starting a new game. Alternatively, on all players declaring ready, a countdown will start and automatically take you into the game, resetting synchronisations in the process.
2. If you're continuing a co-op game with someone but are using a server which may have had other users since you've been away, you'll want to reset sync before loading into the save. This way, the server has its synchronisations completely reset and when you load into the game, your client will inform the server of everything that you've already collected.

The command may only be run by a host to avoid disruptions during speedruns.