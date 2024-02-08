# Seahorse Objective

---

The Seahorse Objective is unique in the fact that it is the only statically allocated objective data structure in memory. The reason for this is unknown but it means obtaining the address of the objective in memory is easier than for other objectives.

However, activating the objective is harder and alerting the game that the objective has been finished is impossible with current knowledge in the vanilla game.

To get around this, the level file has been modified in MTP to add an additional trigger which can be activated when the objective is "Ready for Turn In".

The seahorses only trail behind the player to collect them. The other players will have these seahorses disappear entirely in their games.

The seahorses also must have their collision manually activated which is not the case in v3.4.2. The subsequent update should fix this problem.
