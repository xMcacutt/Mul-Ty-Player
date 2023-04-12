![Mul-Ty-Player](/Multyplayer%20logo.png)
# Mul-Ty-Player

## About

![Liz](/Liz.png)

Mul-Ty-Player breathes new life into Ty the Tasmanian Tiger, allowing you to play online with your friends.

In just a few minutes, you can be exploring snowy mountains, swimming through shark infested waters, and fighting off Boss Cass together as your favourite koalas.

## Update & Installation

![Boonie](/Boonie.png)

2.2.1 has been released!!!!

This update focuses on polishing the ready up system, adding more player info to the sidebar, and supporting dedicated 24/7 servers.
Check the changelog below for more details.

2.0.0 Update:

ElusiveFluffy (Kana) jumped on board to bring us a brand new GUI to make Mul-Ty-Player foolproof.

![ClientGUI](/ClientGUIDemo.png)

The rkv patcher is no more. Now, you can install the patch in less than a minute, directly from the client app.

![Patcher](/PatcherDemo.png)

Finally, you can also select which koala you want to play as.

![Koala Selection](/KoalaSelectionPreviewImage.png)

Before installing, make sure you remove all pre-2.0.0 versions of Mul-Ty-Player from your computer, including the game files.

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

### New Features
- New GUI.
- Ready up system and countdown.
- Player list in client app.
- Integrated install into client app.
- Auto reconnect on network time out.
- No disconnect if game crashes.
- Functional password system.
- Choose which koala to play as.
- Crash stability. No more constant crashing.
- Desync issues fixed.
- Faster code.
- K O A L A

### Bug Fixes

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
 - [Riptide Networking](https://github.com/RiptideNetworking/Riptide)
 - [Facepunch.Steamworks.Net](https://wiki.facepunch.com/steamworks/)
 - [Modding Team From Speedrunning Discord](https://discord.gg/ENTV72BWru)
 - [Kanesthename for Artwork](https://www.deviantart.com/kanesthename/art/Ty-The-Tasmanian-Tiger-Logo-Recreation-Render-271468546)
 - SilentKuudere and SirLaurenceNZ for stopping me from going mad and helping me to test.
 - GPT for learning with me and occasionally giving me useful code.
