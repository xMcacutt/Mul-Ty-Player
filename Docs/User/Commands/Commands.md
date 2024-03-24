# Commands

---

Commands are used to interact with the server directly from the client as well as triggering several actions that can also be triggered by parts of the UI. They are always called via the user input box in the [lobby](../LobbyUI.md).

---

## Command Anatomy

Commands are made up of several parts. 

Firstly there is the name which is what is used to call the command. For example, in the teleport command /tp is the call or name. 

The call can also use an alias which is an alternative name to use to call the command. Continuing the teleport example, the same command can be called using either /teleport or /tp. 

All commands must begin with a call and all calls must begin with a ‘/‘. 

Most commands also have some amount of arguments which are values that the command needs to be able to run. This could be the coordinates to teleport to for example.

Some commands have keywords that notifying MTP that a specific action should be taken. For example, to change the hit detection range  in Hide & Seek Mode, the /hs command can be used but since this command is used for multiple functions, the word range must also be given so the full command would be /hs range \<value\>.

Some commands are also host only meaning only a user with host status can call that command.

---

## Command Help

The way commands are defined and registered in MTP means that help information is automatically generated for all commands. The help command can be used to see this information. If you ever use a command incorrectly, you’ll be prompted to use the help command for more information. 

It is strongly recommended that you use this feature if you are ever stuck. All command information for each command can be found in the relevant document listed below but if you only have time to read a couple, please read the Help and CList documentation.

---

## Standard Commands

The following is a list of all of the commands linking to documents with their specific details.

- [Help](./HelpCommand.md)
- [CommandList](./CommandListCommand.md)
- [Password](./PasswordCommand.md)
- [Ready](./ReadyCommand.md)
- [RequestHost](./RequestHostCommand.md)
- [Teleport](./TeleportCommand.md)
- [Where](./WhereCommand.md)
- [Whisper](./WhisperCommand.md)

___

## Host Commands

- [Countdown](./CountdownCommand.md)
- [Kick](./KickCommand.md)
- [LevelLock](./LevelLockCommand.md)
- [HideSeek](./HideSeekCommand.md)
- [ResetSync](./ResetSyncCommand.md)

___

## Server Commands

Server commands must be called through the command line interface in the server app. Below is a list of all server commands. Note that not every client command is a server command.

- [Help](./HelpCommand.md)

- [CommandList](./CommandListCommand.md)

- [HideSeek](./HideSeekCommand.md)

- [Kick](./KickCommand.md)

- [LevelLock](./LevelLockCommand.md)

- [Password](./PasswordCommand.md)

- [ResetSync](./ResetSyncCommand.md)

- [Restart](./RestartCommand.md)

- [Teleport](./TeleportCommand.md)

- [Whisper](./WhisperCommand.md)

---

## Global Hotkeys

As of v3.4.5, global hotkeys have been activated allowing certain functions to be called using shortcuts without the client needing to be the focussed window.
These are the shortcuts available
- Ctrl + Shift + G - Runs the /groundswim command
- Ctrl + Shift + H - Runs the /requesthost command
- Ctrl + Shift + S - Runs the /countdown command with the "start" argument
- Ctrl + Shift + T - Runs the /taunt command for Hide & Seek mode
- Ctrl + Shift + R - Runs the /ready command
- Ctrl + Shift + Alt + C - Runs the /crash command
- Ctrl + Shift + P - Runs the last command typed into the console
- Ctrl + Shift + Q - Calls request sync (for de-synchronization issues)
- Alt + Shift + T - Runs cheat to give technorangs
- Alt + Shift + E - Runs cheat to give elemental rangs
- Alt + Shift + L - Runs cheat to toggle lines in sky
- Alt + Shift + M - Runs cheat to toggle level select menu
- Alt + Shift + I - Runs cheat to toggle invincibility
- Ctrl + Shift + F - Puts player info freecam teleport mode. When pressed a second time, the player teleports to the position of the camera.
- Ctrl + Shift + 8 - \[SPECTATOR ONLY] Disengage from following player
- Ctrl + Shift + 9 - \[SPECTATOR ONLY] Find previous player to follow
- Ctrl + Shift + 0 - \[SPECTATOR ONLY] Find next player to follow
