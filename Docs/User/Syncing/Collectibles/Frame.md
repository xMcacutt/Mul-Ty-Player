# Frames & Invisicrates

---

Frames are a type of gem in Ty so they share many properties with [opals](./Opal.md) and rainbow scales. However, their memory is significantly different and invisicrates are fairly different to crates.

The principle for syncing is still very similar to opals but a few things to note,

- Frames are stored in a contiguous block of memory in the save data with bytes having overlapping level data. The first 9 bits are for the Rainbow Cliffs frames but this takes up 1 byte and 1 bit. The next 7 bits fill out the second byte and contain all of the Two Up data. Therefore, the save data calculation is even more complicated than for opals.

- Invisicrates use the same observer as regular crates but since no observer exists for the frames, every frame must be checked in each cycle.

- Luckily, the frames do get saved to the pre-save memory when collected so unlike the opals, the save data can be used as an observer.

- Picture frames do not fall by default once spawned and as of v3.6.1, a method to make them fall has not been found

- As of v3.4.2 frame syncing is broken in the bonus worlds for unknown reasons

- As of v3.4.2 the frame draw distance is not high enough and frames are unlikely to collect when on extreme ends of most levels \*Fixed in v3.4.3

- As of v3.6.1 the picture frame syncing is fully implemented and has no bugs.
