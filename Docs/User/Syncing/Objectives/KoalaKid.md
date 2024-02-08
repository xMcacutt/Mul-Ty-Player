# Koala Kid Objective

---

The Koala Kid Objective is the objective present in Beyond the Black Stump and Snow Worries in the base game. The goal is to collect all 8 koalas and return them to Sheila.

In MTP the objective is present in every level as it must be present for the koalas to be used as visual representations of the players. This causes two problems

1. The koala count scales with the number of koalas present leading to 16 koalas and making the mission impossible to complete without being connected to MTP at some point.

2. The koalas are loaded in a specific order by koala so even placing the koalas far apart in the level file, the koalas in memory pair up as (MTP Koala, Vanilla Koala) for each koala. 

To solve the first issue, the koala count is artificially pushed back down to 8 on loading into Snow Worries when connected to a MTP server.

The solve the second issue, each second koala is skipped over when running certain functions depending on whether the koala positions should be adjusted for players or if the koala objects should be activated for objective syncing.

The koalas are unqiue from other objectives in that setting their state to 5 causes them to collect fully so when syncing, all players will have the koala icon popup alerting them that someone has collected the koala.
