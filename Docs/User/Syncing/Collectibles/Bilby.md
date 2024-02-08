## Bilby

---

Bilbies are synced using the principles described in [the syncing documentation](../Syncing.md). However, in version 2.2, a softlock was noticed where if a player collected the fifth bilby but failed to collect the thunder egg for the bilby, no one would be able to get the thunder egg since the egg was not being spawned for other players and after reloading the level, the bilby would be "synced" (despawned) for the player who initially collected the final bilby meaning they had no way of collecting it again to obtain the thunder egg. 

Many suggestions were made to fix this issue. The solution used since v3.2.0 is to not sync the fifth bilby and instead tie the fifth bilby's state in the client to the bilby thunder egg. This means that for all other players, the final bilby will not despawn or be set to collected until the bilby egg itself is collected.

Like [thunder eggs](./ThunderEgg.md), bilbies are not loaded in the same order as the save data and each bilby type has a unique id. These ids are used to sync the bilbies in the save data.

---
