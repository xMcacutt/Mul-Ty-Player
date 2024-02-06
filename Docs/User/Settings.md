# Settings

---

Settings for MTP are split between the client and server and are stored in JSON files called ClientSettings.Json and ServerSettings.Json respectively.

If you're happy editing raw JSON (it isn't hard at all), then you can modify your settings manually by editing these files. This is mandatory for editing server settings.

Fortunately, for those who don't want to do tedious text editing, the MTP client has a settings menu which can be accessed via the settings button in the [lobby](./LobbyUI.md).

---

## Server Settings

Since there's a lot less of them, let's go over the server settings first.

##### Password

The password setting requires a five letter sequence and offers soft protection against people joining during sessions especially for speedrunners. The full details on how the password works are discussed in [the login documentation](./Login.md).

##### Port

The port setting changes which port the server starts on. MTP does not check if the port is open. The server will still run regardless of if the port number is open or not. Please follow the port forwarding guide in the main [User Guide](./User Guide.md) document. The default port is 8750.

##### ResetPasswordOnEmpty

This setting is discussed in [the login documentation](./Login.md). It prevents lockouts in public servers by forcing the password to the default "XXXXX" when the server has no connected clients if true.

##### DoSyncOBJECT

There are several settings in the server for the different types of syncing that should occur. They are all active by default. For more information on syncing, see [the syncing documentation](./Syncing.md).

##### Version

The version settings is used exclusively for compatibility checks and warnings when clients connect. This should NEVER be altered unless you know exactly why you need to change it. It is handled automatically by the updater.

---

## Client Settings

Client settings are split into three main groups: Client, Gameplay, and Developer.

---

### Client Settings

The Client Settings submenu contains all of the settings related to the functionality of the client itself and the behaviour of the MTP game process but not any behaviour within the game.

##### Theme

The client supports themes via theme files. By default there are two theme files shipped with MTP (Dark.json and Light.json). Further customization of the themes is discussed in [the customization documentation](./Customization.md).

##### Auto Launch Ty

Automatically starts MTP whenever the client is opened meaning you don't have to launch both. It is disabled by default to ensure new users understand that both need to be running.

##### Auto Restart Ty

Automatically reopens the game if it closes for any reason. This is helpful for the memory leak in the game as well as random crashes from MTP. Keep in mind, with this setting active, the client MUST be closed before the game will close without reopening.

##### Attempt Reconnect

Automatically tries to reconnect to the server on network connection failure. This prevents the user from needing to tab out of the game and connect back in to the server.

##### Use Steam Name

Automatically attempts to fetch your steam name to use as your client display name when you launch the client. If this is disabled, another box appears in the settings asking for your default name allowing you to set a name to be used every time you launch the client.

---

### Gameplay Settings

The Gameplay Settings submenu contains all of the settings related to functionality changes within the game.

##### Koala Scale

Forces all koalas (other players) to be drawn at x times the original size of the koalas. A scale of 1 is the original size and a scale of 2 is twice the original size etc. The scale is hardcoded to be locked between 0.5 and 3. Anything outside of this range will be capped at the upper or lower bound. This is to avoid a crash when the scale is made too large. 

The scale is sometimes manipulated by the client regardless of this setting such as in [Hide & Seek Mode](./HideSeek.md) for fairness or in Outback Safari (B3) where the koala scale is made artificially bigger to account for the increased scale of almost everything in the level.

##### Interpolation Mode \**EXPERIMENTAL*\*

Interpolation mode attempts to reduce the load on the network by reducing the number of times the koala positions are received by the client. This settings is expected to experimental for some time and is currently deeply unstable. It is strongly recommended that you leave this setting on None unless a future update polishes the feature.

##### Collide Koalas

Determines whether you as the player should collide with the koalas visually representing the other players. It is turned off by default but may be toggled with instant results.

---

### Developer Settings

The Developer Settings submenu contains all of the settings that are not commonly used or altered by the general user.

##### Output Logs

When running MTP, logs are created by default with all of the messages that appear in the console. This sometimes helps with figuring out why someone crashed. Logs are output to the Logs directory in your client directory. Turning file logging off may see an improvement in performance.

##### Default Port

Used to change the port which is used for connection by default. This does ***not*** override the ip:port syntax. If a port is provided after the IP, that port will still be used for connection, this setting simply changes which port is used when no port is provided following the IP.
