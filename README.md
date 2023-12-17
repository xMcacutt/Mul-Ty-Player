![Mul-Ty-Player](/Multyplayer%20logo.png)
# Mul-Ty-Player

## About

![Liz](/Liz.png)

Mul-Ty-Player breathes new life into Ty the Tasmanian Tiger, allowing you to play online with your friends.

In just a few minutes, you can be exploring snowy mountains, swimming through shark infested waters, and fighting off Boss Cass together as your favourite koalas.

## Update & Installation

![Boonie](/Boonie.png)

3.0.0 IS HERE!!!

Over the six months, progress on MTP has been a lot slower but another complete restructuring thanks to Tinsel has gotten us huge performance improvements
as well as a ton of bug fixes and helpful new features.

![ClientGUI](/ClientGUIDemo.png)

Install the patch in less than a minute, directly from the client app.

![Patcher](/PatcherDemo.png)

Select which koala you want to play as.

![Koala Selection](/KoalaSelectionPreviewImage.png)

Before installing, make sure you remove all pre-2.0.0 versions of Mul-Ty-Player from your computer, including the game files.

If you're installing from MTP2.X.X, remove the contents of your MTP game files and rerun the setup as described below.

You can remove the applications from the "Add or Remove Programs" section of your control panel.

To remove the game files, delete the "Mul-Ty-Player" folder from your game directory.

WARNING! Until further notice, if you do not see the install patch button on selecting your Ty and Mul-Ty-Player folders in the patcher, you will need to do a clean reinstall of your clean Ty game files.

To install Mul-Ty-Player, go to the releases page and download the "Mul-Ty-Player 2.0.0 Installer.msi".

Next, run the installer to get the client and server applications.

The server application is only necessary if you wish to host sessions.

Avoid installing the client and server in protected locations such as Program Files.

The Mul-Ty-Player CLIENT should now be installed and ready to run.

### Installing Additional Files

Right click on "Mul-Ty-Player Client.exe" and select "Run as administrator".

In the top left of the splash screen, click the setup button.

Select your original, clean, Ty the Tasmanian Tiger folder. This is the folder that contains the game files.

Select your Mul-Ty-Player folder. This is the folder that contains the game files.
*If you don't have a Mul-Ty-Player folder yet, you can create and select an empty folder anywhere on your computer.*

Click "Install Patch" and wait for the installation to complete.

If the "Install Patch" button doesn't appear, ensure that the original game folder is a clean copy of the game and that the destination folder is empty or contains a version of Mul-Ty-Player 2.0.0+

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

### New Features v3
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

### Bug Fixes

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
 - [Kanesthename for Artwork](https://www.deviantart.com/kanesthename/art/Ty-The-Tasmanian-Tiger-Logo-Recreation-Render-271468546)
 - SilentKuudere and SirLaurenceNZ for stopping me from going mad and helping me to test.
 - GPT for learning with me and occasionally giving me useful code.
