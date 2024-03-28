# /Tp

## Basic Information

#### Description
Teleport using various target selectors.

#### Aliases
- teleport
- tele

#### Usages
- /tp (teleports to last teleported POSITION regardless of method)
- /tp \<x> \<y> \<z>
- /tp \<clientId>
- /tp \<@1> \<@2>
- /tp \<@1> \<x> \<y> \<z>
- /tp \<@1> \<posId>
- /tp \<posId>

#### Arguments
- \<x> - x-coordinate to teleport to. Relative to current with ~x.,
- \<y> - x-coordinate to teleport to. Relative to current with ~y.,
- \<z> - z-coordinate to teleport to. Relative to current with ~z.,
- \<clientId> - client to teleport to.,
- \<@1> - Target selector from @a, @r, or clientId,
- \<@2> - Target selector to @r, or clientId,
- \<posId> - Level position identifier @s (start), @e (end)

#### IsHostOnly?
- False

#### CanSpectatorRun?
- False

## More Information
The teleport command is a complex system mimicking the system used by minecraft. There are several different types of teleport.
### Teleport to position
If you know the specific position you want to teleport to, you can specify the x, y, and z coordinates to teleport to
```
/tp 1000 250 2000
```
The position may also be defined relative to your own current position using the ~ key. For example, to teleport up by 1000 units, you'd use
```
/tp ~ ~1000 ~
```
Each ~ (tilde) represents Ty's position for that coordinate and by specifying a value after the ~, it will add that value to the coordinate.

### Teleport to client
If you want to teleport to a specific player, you can specify their client id. If the player is in a different level, you will first be warped to that level.
If you fail to be teleported to the player once in the level, re-run the command and it should teleport you correctly.
```
/tp 2
```
### Teleport with target selectors and position identifiers
Target selectors are a way of specifying which players to teleport. 
- @a is used to teleport all players
- @r is used to teleport a random player
- clientId is used to teleport a specific player 

Additionally, both @r and the clientId can be used as the destination target. 
Any combination of these that is definable should be possible.

There are also two position identifiers for each level
- @e is used to teleport to the end of the level.
- @s is used to teleport to the start of the level.

These may be used as the destination alongside any of the target selectors.

Here are some examples

Teleport all players to client 2
```
/tp @a 2
```

Teleport a random player to a different random player
```
/tp @r @r
```

Teleport all players to the start of their current level
```
/tp @a @s
```

Teleport client 2 to the end of their current level
```
/tp 2 @e
```

Teleport all players to a specific position
```
/tp @a 2000 500 3000
```

If you wish to target yourself, you only need specify your id as the destination. If you want to teleport yourself, simply specify the destination.

### Teleport to the held position
Every time you are teleported by any means, the position you were teleported to is stored. When
```
/tp
```
is run with no arguments, you will be teleported back to the last position you were teleported to.
