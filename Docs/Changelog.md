# Changelog

## Most Recent

#### 4.8.1
- Added a command suggestion and completion popup.
- Added button to request player count from login screen.
- Added command to toggle specific rangs by name.
- (CHM) Fixed bilby draw distance in Chaos Mode.
- (CLM) Balance changes to collection mode.
- (CHM) Added seed logging to text file for OBS.

#### 4.8.0
- VIP system added for players who won the MTP2YAC event.
- (HSD) Ability cooldown added.
- (CHM) Overlapping collectible positions fixed.
- User guide updated.
- Servers logon overhauled to allow DNS connection and port obfuscation.

#### 4.7.0
- (CHM) Added moving collectible support.
- Hero State functions updated fixing outback death bugs.

#### 4.6.3 / 4.6.4
- (CHM) Added new positions.

#### 4.6.2
- Fixes to spectator and collection mode.
- (HSD) Major fix to Hide & Seek role checking.

#### 4.6.0
- (HSD) Balancing fixes.
- (CLM) Fixed random player not selecting correctly.
- (CLM) Added score file logging.

#### 4.5.0
- (CLM) Collection Mode fully implemented.
- (HSD) Flashbang and more perks added.
- (HSD) Drafts system progress made

#### 4.4.0
- (HSD) Added drafts system.

#### 4.3.0
- (FR) Removed Finnruns theming.
- Added game info activation to the updater.
- Added incorrect menu option on pause bug fix to the updater.
- Removed game info syncing.
- Prevented deletion of files on update error.

#### 4.2.2 - Finnruns Edition
- (CL) Modified collectible point balance.
- Added game info syncing and activation.
- Added portal swapping to chaos mode.
- Added old patch rang syncing to updater.
- Added controller camera aiming to updater.

#### 4.2.1 - Finnruns Edition
- Added Collection Mode (CL)

#### 4.2.0 - Finnruns Edition
- Added Finnruns theming
- (HS) Screen is made black during hide time to avoid cheating.
- (HS) Abort button added to menu.
- (HS) End of session (all caught) sound added.
- Outback Safari input vectoring un-patch option added to updater.
- Camera override sphere removed from spire in Ship Rex.
- Added death plane to all levels (except Outback Safari).
- (SP) Freecam is now removed on joining as non-spectator.
- (SP) Spectator no longer has access to ready status.
- (HS) Multiple Hide & Seek sessions can no longer run at the same time.
- (HS) Switched to Server-Side range check.
- (CH) Fixed cog on WiTP forcing main menu in chaos mode.
- (CH) Removed accidental debug artifact.
- (SP) Fixed spectator breaking Hide & Seek.
- (SP) Fixed IsReady not being retrieved on connect.
- Fixed collectible syncing running in invalid levels.
- Fixed collectibles not correctly resyncing on main menu.

#### 4.1.0
- Chaos mode menu added.
- Shuffle on start option added.
- Fixed seed option added.
- Chaos seed is now displayed when changed.
- Fixed everything Chaos Mode.
- Cogs now moved to the correct positions.
- Bilbies are now rotated.
- All bilbies are now moved.
- Cheat command alias changed to c from ch.
- Chaos mode command alias changed to ch from cm.

#### 4.0.0
- Added chaos mode, a randomizer for the cogs and bilbies.
- Expanded game mode system and menu in lobby.
- Server-side sync settings can now be changed from client-side settings menu.
- (HS) Added hider freeze perk to Walk in the Park
- Fixed server being marked with v instead of s in mismatched versions warning.
- Reduced number of dedicated servers.

#### 3.6.5
- Added level perks to Hide & Seek

#### 3.6.4
- Added resetsync hotkey
- Password no longer throws error on attempt to change
- Rainbow Scales and Opals now auto sync on level reload if game was not saved before reload
- Seahorse count is now updated on reload
- Seahorses now despawn on talking to Aurora instead of on collection of last seahorse
- Last bilby is now shown as collected for all players in game info

#### 3.6.2 / 3.6.3
- Cable car issue fully resolved (triggering deactivation on opal TE by mistake)
- Ground swim / Free cam interaction fixed

#### 3.6.1
- Countdown checks for players on menu later to avoid falsely aborted countdowns
- Groundswim hotkey has been disabled in freecam and in outback safari to avoid crashing
- Spectator follow distances reduced
- Picture frame syncing bugs fully resolved
- Frills killed with kaboomerang during cable cars now update correctly
- Client and Server settings are no longer overwritten on update

#### 3.6.0
- Added Spectator Mode
- Added Freecam Teleport (ctrl+shift+F)
- Added host context menu item to force a specific player to the main menu
- Fixed picture frame syncing across all levels
- Fixed koalas not being returned to their hidden location on disconnect
- Fixed cross level teleportation via /tp

#### 3.5.2
- Teleport now works cross level
- Fixed issues with crate syncing
- Fixed reset sync incorrectly resetting rangs
- Level warp all made host only

#### 3.5.1
- Added level command to warp into levels
- Added auto start to automatically force the game to start during countdown
- Added save slot setting to change slot autostart goes into
- Attempt at fixing outback desync

#### 3.5.0
- Added koala beacons showing where other players are
- Server hosting change
- Koala jitter bug fixed by removing all koala location sounds from rkv and rolling back koala state change
- Cable car syncing fix attempted by extending frill global.model range
- Server IP dropdown made bigger for longer IPs

#### 3.4.10
- Added koala skins for Ty

#### 3.4.9
- /tp to last teleported position command added
- hotkey file added for hotkey customisation
- Rang desync issue resolved

#### 3.4.8
- Hotfix to resolve koalas falling through world

#### 3.4.7
- Cheat command added
- Cheat command hotkeys added under Alt + Shift
- Sounds added on rang obtain
- Hotkey crash handled with warning box

#### 3.4.6
- Request Sync hotkey added
- Commas allowed in /tp
- Taunt system fixes
- Cable car syncing fixes

#### 3.4.5
- Global hotkeys to access functions in the client whilst in Ty.
- Taunt system for Hide & Seek
- Updater now functions correctly in most cases and can be used over 3.2.2

#### 3.4.4
- (HS) Role now updates correctly

#### 3.4.3
- Extra themes
- [Documentation](./Docs/User/User%20Guide.md)
- Server selection drop down
- Countdown abort / start command added
- Extra stability
- *Note* - Frame syncing is still bugged
- Ghost players in client resolved
- Redundant player info list removed
- Main menu or loading function issue resolved
- Koala return on disconnect / change level fixed
- (HS) Time message changed to match reduced time in 3.4.2
- Stopwatch activating on wrong TE fixed
- Seahorse collision activated
- No host set on connect to reduce duplicate host chance
- Global.model edited to increase picture frame range
- Updater no longer overwrites modifications to the GUI directory allowing cutsomization

#### 3.4.2
- (HS) Adjustable hit detection range
- (HS) Seeker speed increased to 10.05 during seek
- (HS) Seeker & Hider hit detection
- (HS) Time reduced to 75s for hide
- (HS) /hs abort added to cancel session
- Updater setup menu trigger source set correctly

#### 3.4.0
- Seahorse syncing added
- Cable car frills syncing added
- Hide & Seek mode added
- Objective sounds on collection by other clients
- Ready status overhaul and fix
- Level lock toggle fixed
- Icons incorrect state fixed
- Invisicrate / frame syncing somewhat fixed
- Koala objective syncing hand in fixed
- Fatal bug relating to objective syncing cross level fixed

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

#### 3.2.3
- Objective syncing pre-release system
- Bridge Flame Burner syncing
- Stump / Snow Koala syncing
- Shadow patch added to rkv

#### 3.2.2
- Command system overhaul
- New command info using /help
- New password reset setting for public servers to avoid lockouts
- Command recall (navigate previously called commands)

#### 3.2.1
- Interpolation mode enabled **EXPERIMENTAL**
- Attempt reconnect on timeout bugs fixed and re-enabled
- E4 / C4 game crash on Level Lock completion fixed
- Teleport command client crash fixed

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
- Bilby softlock if player collects fifth bilby without TE fixed.
- Koala selection black flicker fixed
- Auto-Reconnect disabled pending fix
- Server list saving bug fixed

#### 3.1.0
- Stopwatch syncing added
- Pickup frame syncing added (WIP will be ready for 3.2.0)
- Sync button added to lobby toolbar
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

#### v3.0.0
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
- Koala's show up in outback
- Connect button and read button always reset on countdown cancel
- Fixed sounds not playing
- Fixed issue with player coordinates being sent more than necessary
- K O A L A

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

#### 1.3.4
- Debug message removed
- Patcher folder always created fixed

#### 1.3.1
- Spawning the Final Battle portal no longer crashes the server
- Portals will appear in Rainbow Cliffs if someone activates one outside of Rainbow Cliffs
- Debug output has been stripped from the applications
- Counter Prop reset to original settings in LV2 (RKV PATCH)
- 
#### 1.3.0
- Basically everything wrong with v1.2.0
- Black stump actually works now
