# Host System

---

The host system provides one player with additional priviledges that could be abused if multiple players had access to them. These come in two forms: host menus and host commands.

---

## Obtaining Host

The host status is given to the first player to connect to a server with no host. A hostless server is created by the player with host disconnecting. In this case, the host status is not transferred so that is the disconnect was a network drop, on reconnection, that player receives the host status again.

If the server has no host and no one is expected to join to become host, the [request host command](./Commands/RequestHost.md) can be used to claim the host status. (tldr: /rh)

Once a player has the host status, a crown icon will appear next to their name in the Player Info List.

---

## Host Menus

Once a player has the host status, they will have access to two additional menus, the host menu and the host context menu. 

Both menus are discussed in detail in [the lobby documentation](./LobbyUI.md) but here's a quick overview.

##### The Host Menu

The host menu is located in the button toolbar at the top of the lobby window and is marked by a crown. When the button is clicked, a dropdown will appear allowing the host to quickly access some commonly used functions and commands.

##### The Host Context Menu

The context menu appears when a player in the Player Info List is right clicked and allows the host to quickly remove a player from the server or transfer the host status to another player.

---

## Host Commands

The host also has access to extra commands which cannot be run by clients without the host status. To see a full list of these commands, see [the commands documentation](./Commands/Commands.md).
