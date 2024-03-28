# Login

---

Logging into MTP can seem complex but it is a very simple process designed to be fast.

---

## Average UseCase

For the average user, wanting to connect to the dedicated server with their steam name as their display name, simply press connect and you'll be taken through to the [Koala Select view](./KoalaSelect.md).

---

## IP

The first box on the login screen is the IP address box. This box must contain a valid IP address and may ***not*** be a DNS address (example.com). If you wish to connect to a specific port, it may be specified after a colon following the address. For example, to connect to port 9876, you would use

```
12.34.56.78:9876
```

---

## Server Hosting

If you wish to host your own server you'll need to port forward. More information about this was explained on the [user guide document](./UserGuide.md).

---

## Password

The password system is complex and functions in several different ways depending on the use case. It is a soft password, meaning it can be relatively easily overridden to avoid blocking the dedicated servers.

Firstly, for server hosts, note the setting "ResetPasswordOnEmpty" in ServerSettings.json. This setting is on for all dedicated servers and means that if the server has no connected clients at any given point, the server will be reset to a default value "XXXXX". The server will also always allow a client to connect when empty to avoid empty, password locked, servers.

A new password can be set via [the password command](./Commands/Password.md).

The default password for a server is "XXXXX" which will by default allow all connections.

All passwords must be exactly five letters.

Casing is ignored.

---

## Name

The name field on the login screen is, by default, automatically populated with your steam name. This can be changed in the settings menu in the lobby or by modifying the ClientSettings.json file. If the steam server connection fails to initialize, you will be given a default name UserXXXX where XXXX is replaced with a random number.

---

## List.Servers

List.servers is a file belonging to the MTP client which is used to get the default server IP to fill the IP box on launching the client. Each line in list.servers follows the same pattern

```
ip pass (*)
```

The password is the last password which was used to successfully login to the corresponding IP address and the asterisk represents the active server. The server which is loaded into the box will always be the server with the asterisk by default.

To switch the default server simply remove the asterisk and add it to the line with the IP you want to be used by default.

---

## Dedicated Servers

As of 05/02/24, MTP has dedicated servers running 24/7. The default server IP on install will always be the IP of the dedicated server. If you have lost the IP, you can switch back to the dedicated IP using the list.servers method described above.

## Join As Spectator

Since version 3.6.0, a spectating mode has been added to the app. In this mode, you will be forced into freecam whenever you load into a level.
More information on the mode can be found in [the Spectator Mode documentation](Spectator.md).