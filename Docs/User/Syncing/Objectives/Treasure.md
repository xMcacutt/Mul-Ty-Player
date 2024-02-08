# Treasure Chest Objective

---

The Treasure Chest Objective is the most unique of the synced objectives. Rather than needing to collect 6 objects in any order, the 6 chests must be collected in a specific order. To account for this, the client and server hold the data of how many have been collected and then activate the next chest in the list when the previous one is collected. This all happens during the active state.

Chests' triggers are activated on exiting the previous one's trigger, not on interacting with them. This the default (weird) behaviour of the vanilla game. The behaviour is carried through to MTP but the chests themselves will also be spawned along with the trigger without needing to activate the chest.

An objective sound does play when a chest is collected.
