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