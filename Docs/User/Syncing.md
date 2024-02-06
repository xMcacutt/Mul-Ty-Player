# Syncing

---

Syncing is one of the core features of MTP. It allows collectibles to be collected collaboratively between clients. Most collectibles and objectives in the game are syncable with MTP. 

---

## Core Collectibles

All core collectibles are synced between both the save and live data.

Live data refers to the physical object you see in a given level while save data is global and can be modified for levels which are not currently loaded. The save data is accessed every time a level is loaded so if the save data is synced and then you enter a different level where collectibles were collected, the client will now have them as already collected. It is vital that both the live and save data is synced.

The general process for all collectible syncing is the same. 

For all "sync objects" which are actively synced according to the server [settings](./Settings.md), an observer is used to track whether that object might have changed. This reduces what could be hundreds of checks across all collectibles to one per type. As an example, the Cog observer is the in game Cog Count. 

When someone collects something, the observer changes so the client scans through every collectible of that type to see if any of their states have changed. Once it finds one that has, it determines the live and save indices of the collectible that changed and the server is notified with the message passed down to all other clients. 

The server and client have their own data storing the states of every collectible. These states are changed via a one way system. I.E. they can only be activated, not deactivated. Once the server or client thinks something has been collected, the only way to reset the collectible state is to run the [resetsync command](./Commands/ResetSyncCommand.md) or restart the client (assuming only the client believes a collectible is collected).  

Resetsync is automatically run when the countdown starts with [Hide & Seek Mode](./HideSeek.md) off. 

When Hide & Seek Mode is on, all syncing is halted (v3.4.2).

---

## Thunder Egg

Thunder eggs are synced using the core principles described above. Their live and save indices do not match (the order the thunder eggs are loaded is not the same as their order in the save data) but a save data index is located in the in-game memory close to the live object state.

The stopwatch object which allows the player to enter the Time Attack for any level is synced via the thunder egg syncing code. When the main objective thunderegg is collected, the server is notified and passes the message to all clients to spawn the stopwatch for that level. For this reason, the thunder egg states must be saved somewhere with their save indices as well as their live indices though this is uncommon and specific to thunder eggs.

Objective syncing is tied to the thunder eggs and is discussed later in this document.

---

## Bilby

Bilbies are also synced using the principles described above. However, in version 2.2, a softlock was noticed where if a player collected the bilby but failed to collect the thunder egg for the bilby, no one would be able to get the thunder egg since the egg was not being spawned for other players and after reloading the level, the bilby would not be present for the player who initially collected the final bilby meaning they had no way of collecting it again to obtain the thunder egg. 

Many suggestions were made to fix this issue. The solution used since v3.2.0 is to not sync the fifth bilby and instead tie the fifth bilby's state in the client to the bilby thunder egg. This means that for all other players, the final bilby will not despawn or be set to collected until the bilby egg itself is collected.

---

## Attributes & Rainbow Cliffs

Attributes describe any ability or upgrade that Ty receives during the game including swimming, diving, all of the rangs, the second life paw, and also the talismans.

Rainbow Cliffs data refers to the state of the thunder egg machines, the flaming logs, the E-Zone gate, and ice the sheet. 

Both of these categories are synced in the save data only. For the attributes, this causes the abilities to update instantly but the Rainbow Cliffs data does not update until after the level has been reloaded.

---

## Portals

---

## Frames & Invisicrates

---

## Opals & Crates

--- 

## 
