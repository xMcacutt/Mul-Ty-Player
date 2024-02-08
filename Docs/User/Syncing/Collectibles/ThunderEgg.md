# Thunder Egg

---

Thunder eggs are synced using the core principles described in [the syncing documentation](../Syncing.md). Their live and save indices do not match (the order the thunder eggs are loaded is not the same as their order in the save data) but a save data index is located in the in-game memory close to the live object state.

The observer used to detect changes to the thunder egg count is unique in that it auto updates in the save data due to the autosave triggered on collection. The existence of the save data count is also unique to thunder eggs when compared with opals, bilbies, and cogs which only have a live (level) count for the current level. The save data count is used to display the number of thunder eggs collected on the save when selecting a save to load into.

The stopwatch object which allows the player to enter the Time Attack for any level is synced via the thunder egg syncing code. When the main objective thunderegg is collected, the server is notified and passes the message to all clients to spawn the stopwatch for that level. For this reason, the thunder egg states must be saved somewhere with their save indices as well as their live indices though this is uncommon and specific to thunder eggs.

Objective syncing is tied to the thunder eggs and is discussed in [the syncing documentation](../Syncing.md).

---
