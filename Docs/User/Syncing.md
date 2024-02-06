# Syncing

---

Syncing is one of the core features of MTP. It allows collectibles to be collected collaboratively between clients. Most collectibles and objectives in the game are syncable with MTP. 

---

## Core Collectibles

All core collectibles are synced between both the save and live data.

Live data refers to the physical object you see in a given level while save data is global and can be modified for levels which are not currently loaded. The save data is accessed every time a level is loaded so if the save data is synced and then you enter a different level where collectibles were collected, the client will now have them as already collected. It is vital that both the live and save data is synced.

The general process for all collectible syncing is the same. When someone collects something, the server is notified and the message is passed down to all other clients. The server and client have their own data storing the states of every collectible. These states are changed via a one way system. I.E., they can only be activated, not deactivated. Once the server or client thinks something has been collected, the only way to re
