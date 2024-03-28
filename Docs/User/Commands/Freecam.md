# /Freecam

## Basic Information

#### Description
Puts the player into Freecam mode. Exiting the mode causes the player to teleport to the position of the camera.

#### Aliases
- fc
- cam

#### Usages
- /freecam

#### IsHostOnly?
- False

#### CanSpectatorRun?
- False

## More Information
The freecam command is a form of teleportation. It allows the player to move the camera freely around Ty and the level, going through collision and geometry. Once toggled off, Ty will be teleported to the position of the camera.
This freecam technique is the same as what is used for [spectator mode](../Spectator.md).
Note that this is a form of teleportation so
```cmd
/tp
```
also applies after this teleport. For more information see [the teleport command documentation](./Teleport.md).