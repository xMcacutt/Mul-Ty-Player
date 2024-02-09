# Level Lock Mode

---

Level Lock Mode was created for a specific category of speedrunning for MTP. The core idea is that no one can enter another level until the current level is 100% completed. In Ty, 100% completion does not include pickup frames.

To activate Level Lock Mode the [host](./Host.md) can either press the button in the host menu (see [the lobby UI documentation](./LobbyUI.md)) or run the [level lock command](./Commands/LevelLock.md).

With this mode active, the [portals](./Syncing.md) are handled differently. At the start of the game, all portals except boss portals are made visible to all players. Once a portal is entered, all other portals close fully meaning everyone must be in either Rainbow Cliffs or the currently open level.

On completion of the level, a sound will play to indicate that the portals have reopened and everyone can move to the next level.

In the event of two player entering two different levels at the same time, the way MTP determines which level should be entered is to check the level entry when no level has been selected and compare it with its own data. This means that the player who loaded into their level first is in the active level and anyone in a different (not completed) level should exit and go into the active level. Once the active level has been completed, the other entered level will automatically be selected as the active level.

This mode is prone to softlocks if you manage to enter a level which cannot be completed in your current game state (you don't have the required rangs etc).

The boss triggers at the thunder egg machine are also made active and inactive meaning you cannot go into a boss level unless the portals are open.

Time for the speedrun ends on hearing the sound for level completion whilst in the final level of the run.
