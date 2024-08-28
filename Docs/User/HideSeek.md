# Hide & Seek Mode

---

Hide & Seek Mode provides quality of life features for those wanting to run Hide & Seek games. Throughout MTP, Hide & Seek Mode is often referred to as just hideseek or even hs.

To activate Hide & Seek Mode, the [host](./Host.md) can either press the button in the host menu (see [the lobby UI documentation](./LobbyUI.md)) or run the [hide seek command](./Commands/HideSeek.md).

---

## HS Role

The most fundamental part of Hide & Seek is the player role. This appear in the Player Info List in the lobby as well as on the Hide & Seek menu button in the [lobby](./LobbyUI.md). If the icon is a magnifying glass, your role is Seeker. If the icon is footprints, your role is Hider. You can change role at any time using the Hide & Seek menu. 

---

## HS State

When in Hide & Seek Mode, your client tracks the current state of the Hide & Seek game. There are three states, "Hide", "Seek", and "Neutral". The game is originally in neutral until every player is [ready](./Ready.md). Once this happens, the game will enter Hide Time and a countdown of 75 seconds will start. 

##### Hide Time

During this time, Seekers are advised to pause the game or look away from their screens. To try to prevent cheating, all seekers are also locked to 0 FOV and their current position.

Hiders are teleported to the start of the level and are free to roam around and find a place to hide during this game state. 

A sound will play at 60 seconds remaining, 30 seconds remaining, and 10 seconds remaining to warn the Hiders.

##### Seek Time

Once the 75 seconds are over, the Seekers will have their positions and FOV unlocked and are free to find and chase the hiders. Seeker run speed is subtly increased during Seek Time from 10.00 to 10.05. 

Hiders will be alerted by a sound to the activation of Seek Time and their timers will start. The timer can be made visible using the Hide & Seek menu.

##### Catching

When a Seeker is in range of a Hider, the Hider will be "caught" and teleported back to the start of the level. A sound will play to both the Hider and the Seeker to alert them of the catch. 15 seconds of time are added to the Seeker's time as a reward and the Hider becomes a Seeker.

This continues until all players are caught when the game resets to neutral.

##### Taunting
Once every 30 seconds into seek time, hiders may choose to taunt, playing a sound to all seekers within 3000 units. A time bonus is awarded with more time added the closer the seeker was to you. The effect stacks with multiple seekers in range.

##### Perks, Debuffs, & Abilities
During hide & seek, you'll have a perk and a debuff to help and hinder you while hiding or seeking. These are selected when you ready up through a menu that appears in the client. If you choose a perk which has an ability, you'll be able to activate your ability with control+shift+A by default. 

##### Drafts
Upon starting a drafts session, you'll be able to choose a team and pick & ban levels to hide on. The entire system is automated just be careful not to double click the levels.

---

## Command

The /hideseek command can be used to alter Hide & Seek mode slightly. More information can be found in [the /hideseek command documentation](./Commands/HideSeek.md).
