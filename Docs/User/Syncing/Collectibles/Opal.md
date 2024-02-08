# Opals & Crates

### NOTE* This documentation is quite technical as opals were one of the most complicated aspects to synchronise. As a user, you are not expected to understand this.

---

## Opals

Opals are synced through the live and save data using the level opal count as the observer. In the live data, every opal has a state which is a simple byte in memory. The key states are 

0 - Not Spawned

1 - Spawning

2 - Spawned (Normal)

3 - Collect Activate (Magnetise)

4 - Collecting

5 - Collected

By setting the state of each opal to 3, the opal will be magnetised towards the player.

Given all of this, opals should be as simple as the Cogs but two problems quickly arise when trying to sync opals in practice:

1. Opals are split in memory between those in crates and those not in crates

2. Opals are not in the order expected by the save data

Additionally, whilst almost all other collectibles have an individual byte for each collectible, the opals are stored as one bit per opal in the level's data.

This means a complicated conversion must be done to find the correct byte and then the correct bit to change when syncing the opals. 

To further complicate things, since the opals are split between those in crates and those that aren't, the save data also treats them differently, reversing the order of the opal indices from crate opals. This means that if there are 270 opals not in crates then opal 271 is the last bit in the entire opal data for the level and opal 300 will be at bit 271 (indexed from 1)... Why? I wish we knew.

---

## Crates

Luckily, crates are a little simpler to sync. When a crate is broken, a sound plays. That sound is used as an observer for both the crates and [invisicrates](./Frame.md). When the crate is detected, all crates will be checked and when one is found to be broken, a reference to each of the opals can be followed and the opal states can be set to 1 - Spawning. This makes the opals jump out of the crates as if they were broken. Then the crate collision and visual is disabled.

Collectible syncing being one way means crates will always remain broken once activated. In the vanilla game, crates are respawned on entering the level after the opal thunder egg has been collected. This behaviour is disabled by the nature of syncing in  MTP but is not seen as significant enough to warrant reimplementing.

---

## Rainbow Scales

Rainbow Scales are essentially just opals that are specific to Z1 - Rainbow Cliffs. The count is lower at 25 instead of 300 but almost everything else is the same. Rainbow Scales, Frames, and Opals all use an underlying data structure referred to by Krome as the "gem". The [Frame](./Frame.md) structure differs slightly but most of the opal code could be reused for Rainbow Scales with an altered count.

---
