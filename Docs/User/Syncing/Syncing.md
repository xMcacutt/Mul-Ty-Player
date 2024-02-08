# Syncing

---

Syncing is one of the core features of MTP. It allows collectibles to be collected collaboratively between clients. Most collectibles and objectives in the game are syncable with MTP. 

Draw distance has been a large hurdle to overcome in syncing since objects only update if they are within the update distance which is affected by draw distance in game. Every object in the game has its own base draw distance which is multiplies by the draw level in game (low = 1, mid = 2, high = 3). The draw distances are defined in a file called global.model. This file is modified in MTP to extend the draw distances and update distanced of all synced objects allowing them to sync without issue.

If an object ends up far outside of the draw range for any reason, it will not longer be updatable. This is especially poignant for the player koalas when using the TP command.

---

## Koala Positions

In MTP, all other players are represented by the eight koala kids. The positions of the players are synced and written to the koalas so you can see other people running around. When a player enters a different level to you, their koala will disappear.

The koala should also disappear when their player disconnects from the server though this is bugged as of v3.4.2.

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

Information on each of the collectible "Sync Objects" with unique features can be found here

- [Thunder Egg](./Collectibles/ThunderEgg.md)

- [Bilby](./Collectibles/Bilby.md)

- [Opal & Crate](./Collectibles/Opal.md)

- [Attribute & Rainbow Cliffs Data](./Collectibles/AttributeRC.md)

- [Frame & Invisicrate](./Collectibles/Frame.md)

---

## Portals

Portals to other levels from Rainbow Cliffs are synced differently depending on the mode. Level Lock mode portal syncing is discussed in detail in [the Level Lock Mode documentation](../LevelLock.md).

In regular play, portals are only open once activated by a player. There are two ways this can happen

1. The player enters the level which the portal leads to

2. The player activates the portal by any other means in Rainbow Cliffs

The latter happens, for example, when speaking to Julius to activate the Two Up portal at the start of the game or when entering bosses during the cutscene before the load.

---

## Objectives

Objective syncing is tied strongly to [thunder egg syncing](./Collectibles/ThunderEgg.md) and will not occur unless thunder egg syncing is activated in the server. Objectives are any mission which involves collecting or activating a number of objects in Ty. 

The driving principle behind all objective syncing is the objective state and the object states. The objective state is one of

- Inactive - Before being activated

- Active - Whilst active

- Ready for Turn In - After all objects are collected/activated

- Completed - After the thunder egg has been received

By using these four states, triggers and objects can be activated and deactivated for all clients depending on the state. Every objective is registered with a level so the same objective code can be reused between levels.

Due to the nature of the objectives and the efficiency of the code, a lot of the properties are hard coded to vanilla Ty but a reasonably experienced developer could easily adjust the code to work with any modded version of the game.

Most objectives do not have an easy way to force alert the player that they have been collected/activates. To get around this, for those that don't, a sound is played to all players when someone actiates / collects an object as part of an objective.

In the vanilla game, objectives are not saved and do not persist between levels. Reloading a level or exiting a level and re-entering resets the objective to be inactive but in MTP, this is not the case and objectives do persist through reloads.

The following is a list of the synced objectives and more information about them. 

- [KoalaKidObjective](./Objectives/KoalaKid.md)

- [SeahorseObjective](./Objectives/Seahorse.md)

- [CableCarObjective](./Objectives/CableCar.md)

- [BurnerObjective](./Objectives/Burner.md)

- [TreasureChestObjective](./Objectives/Treasure.md)
