![Mul-Ty-Player](/GitImages/Multyplayer%20logo.png)

# Mul-Ty-Player

## About

![Liz](/GitImages/Liz.png)

Mul-Ty-Player breathes new life into Ty the Tasmanian Tiger, allowing you to play online with your friends.

In just a few minutes, you can be exploring snowy mountains, swimming through shark infested waters, and fighting off
Boss Cass together as your favourite koalas.

## Update & Installation

![Boonie](/GitImages/Boonie.png)

3.4.0 has been released. 
If you were on a version earlier than v3 read the below instructions for information and installation.

If you have a 3.0+ version installed, please delete all client, server, and updater files and reinstall using the new updater.

This version is the start of objective syncing. Treasure Chests, Koalas, and Flame Burners are all now synced
between clients. More to come. 

The Lyre shadow bug has also been fixed by permanently disabling shadows in Patch_PC. A flexible option
is being looked into.

![ClientGUI](/GitImages/Lobby.png)

A new settings window with plenty of options for extra comfort and customization.

![Settings](/GitImages/Settings.png)

Select which koala you want to play as.

![Koala Selection](/GitImages/Koala%20Select.png)

Finally, to make the whole update process for the future, a dedicated updater app
has been created using Octokit to search for updates on github and install them without
having to visit this site again.

![Updater](/GitImages/Updater%20Main.png)

Before installing, make sure you remove all pre 3.2.0 versions of Mul-Ty-Player from your computer, including the game
files.

You can remove the applications from the "Add or Remove Programs" section of your control panel.

To remove the game files, delete the "Mul-Ty-Player" folder from your game directory.

To install Mul-Ty-Player, go to the releases page and download the "Mul-Ty-Player Updater" application.

Extract the files and run the application. You'll need to install the files
on your first use. After that, you can update your files simply by using the update button.
If you move any of your files or want to change your update settings, you can use the setup window.

![Installer](/GitImages/Installer.png)

The server application is only necessary if you wish to host sessions.

Avoid installing the client and server in protected locations such as Program Files.

Mul-Ty-Player should now be installed and ready to run.

### Port Forwarding For Server Hosts.

To run the server you'll need to open port 8750 on UDP.

1. Open CMD (Command Prompt).
2. Type 'ipconfig' and press return.
3. Find IPv4 address and copy this into browser search bar.
4. Log in to your router.
5. Set up a new port forwarding rule.
6. Paste IPv4 address into forwarding address, set port to 8750, set rule to UDP.
7. *Read the above sentence again and make sure the rule is set to UDP.*
8. Go to https://www.whatismypublicip.com/ this is your public IP address.

With your port forwarding rule set up, people can now join your server using your public IP address.

## Changelog

### New Features v3.0.0

- GUI mostly functions in a single window now so no more swapping windows.
- New Launch Game button added to splash screen and main GUI.
- Countdown abort system fully functional.
- Rainbow Scale syncing.
- Semi-functional interpolation system.
- Koalas now appear in Outback Safari.
- Optional auto restart Ty on crash.
- Huge memory management optimisations.
- Crash stability. No more constant crashing.
- Rang syncing issues fixed.
- K O A L A

#### 3.1.0

- Stopwatch syncing added
- Pickup frame syncing added (WIP will be ready for 3.2.0)
- Sync button added to lobby toolbar

#### 3.2.0

- Level Lock mode has been added for speedrunners. Use "/levellock true" to activate it.
- Pickup Frame syncing is completed
- Gift host option added by right clicking a player in the list
- Icon system upgraded
- Koala icons upgraded
- Settings window massively redesigned
- Dark / Light UI themes
- Auto Updater application repalces installer and RKV patcher
- /tp and /where commands added
- Option to scale koalas to be closer to Ty's size added
- All UI heavily restructured and made prettier
- New [collectible tracker](https://github.com/xMcacutt/Ty1-Collectible-Tracker) app created
![ColTrack](/GitImages/ColTrack.png)

#### 3.2.2
- Command system overhaul
- New command info using /help
- New password reset setting for public servers to avoid lockouts
- Command recall (navigate previously called commands)

#### 3.2.3
- Objective syncing pre-release system
- Bridge Flame Burner syncing
- Stump / Snow Koala syncing
- Shadow patch added to rkv

#### 3.3.0
- Objective syncing polished
- Treasure chest syncing added
- Extra teleport options added
- Host options button added
- Kick added to host context menu
- Sound on level complete
- Sound on time attack open
- Update message added to client
- Version comparison added to client
- Copy paste allowed from console

#### 3.4.0
- Seahorse syncing added
- Cable car frills syncing added
- Hide & Seek mode added
- Objective sounds on collection by other clients
- Ready status overhaul and fix

### Bug Fixes

#### 3.4.0

- Level lock toggle fixed
- Icons incorrect state fixed
- Invisicrate / frame syncing somewhat fixed
- Koala objective syncing hand in fixed
- Fatal bug relating to objective syncing cross level fixed

#### 3.3.0
- Koalas are now returned on disconnect / level change
- Triggers should be fixed in level lock mode
- Critical desync error from invalid loading byte fixed
- Text input in lobby caret moved to end of line on command recall
- Updater RKV moved into project
- Updater paths fixed
- Updater server filestream multi access fixed
- Updater text input boxes update dynamically
- Github action updated
- Password now resets on no players connected

#### 3.2.1
- Interpolation mode enabled **EXPERIMENTAL**
- Attempt reconnect on timeout bugs fixed and re-enabled
- E4 / C4 game crash on Level Lock completion fixed
- Teleport command client crash fixed

#### 3.2.0
- Bilby softlock if player collects fifth bilby without TE fixed.
- Koala selection black flicker fixed
- Auto-Reconnect disabled pending fix
- Server list saving bug fixed

#### 3.1.0

- Splash screen text moved to avoid overlap
- Steam overlay no longer appears inside controls
- Countdown ready button is no longer set to enabled on first join when in level
- Countdown ready status of all other players is correct on join
- Koala coordinates no longer being written if in same level
- Level setup is only occurring at correct times
- Memory addresses are being set per level depending on level data types
- Debugging code added to help diagnose and fix memory corruption crashing
- Bilby state check corrected to fix random desync
- Login text box font changed to a monospace to fix type spacing issue

#### 3.0.0

- Rang syncing on reconnect
- Koala's show up in outback
- Connect button and read button always reset on countdown cancel
- Fixed sounds not playing
- Fixed issue with player coordinates being sent more than necessary

#### 2.2.2

- Fixed sounds not playing
- Massive memory optimisation

#### 2.2.1

- Process shuts down fully now (client)
- Support added for custom ports
- Ready button disabled when not on main menu
- Prevented multiple countdowns triggering at once
- Added level to playerinfo
- Sync is reset during countdown

#### 2.1.1

- Install patch button is far less picky and no longer checks file sizes of clean files
- Ready indicators update visually when the countdown resets them
- /restart command removed from host privileges and is only available from server app
- Upscaled koala textures included in new RKV

#### 2.1.0

- Opal live -> save conversion fixed
- Game no longer crashes when multiple people hit the credits screen
- Long names are now truncated in the player list (tool tip still shows full name)

#### 2.0.1

- Portal syncing fixed

#### 2.0.0

- Opal duplication fixed
- Opals now spawn correctly from crates
- Safer menu checks

#### 1.3.4

- Debug message removed
- Patcher folder always created fixed

#### 1.3.3

- Koalas fixed in Snow Worries and BtBS
- Debug messages stripped
- Added version message

#### 1.3.2

- Rangs always sync after finishing boss
- Koala Kid count is no longer stuck at 16 in Snow Worries and Beyond the Black Stump
- Installer actually installs the new version
- Correct Koalas now appear in Bull's Pen
- Opals are more likely to correctly spawn from crates

#### 1.3.1

- Spawning the Final Battle portal no longer crashes the server
- Portals will appear in Rainbow Cliffs if someone activates one outside of Rainbow Cliffs
- Debug output has been stripped from the applications
- Counter Prop reset to original settings in LV2 (RKV PATCH)

#### 1.3.0

- Basically everything wrong with v1.2.0
- Black stump actually works now

### Future Plans

- Objectives Synchronisation
- Picture Frames & Rainbow Scales Synchronisation

### Known Issues

- Sound effects sometimes fail to play
- Client can very rarely crash if the game crashes

## Support

Join the Ty [Modding Discord](https://discord.gg/ENTV72BWru) for help and support or to help work on the project.

You can also leave feedback for future updates in my DMs on Discord Mcacutt#5671.

## Acknowledgements

- [ElusiveFluffy (Kana Miyoshi)](https://github.com/ElusiveFluffy)
- [Tinsel](https://github.com/ConnorDaytonaHolmes)
- [Riptide Networking](https://github.com/RiptideNetworking/Riptide)
- [Facepunch.Steamworks.Net](https://wiki.facepunch.com/steamworks/)
- SilentKuudere and SirLaurenceNZ for stopping me from going mad and helping me to test.
- GPT for learning with me and occasionally giving me useful code.
