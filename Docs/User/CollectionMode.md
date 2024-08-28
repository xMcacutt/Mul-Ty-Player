# Collection Mode

---

Collection Mode is a free for all game mode in which players attempt to score more points than their opponents. Points are awarded for each collectible collected with special rules activating across the game to modify the way the game is played.

To activate Collection Mode, the [host](./Host.md) must select the Collection Mode game mode from the host menu (see [the lobby UI documentation](./LobbyUI.md)) or run the [gamemode command](./Commands/GameMode.md).

When all players [ready up](./Ready.md), the game will start and for the next 5 minutes, players will earn points according to the point breakdown:

| Collectible             | Points |
|-------------------------|--------|
| Thunder Egg             | 50     |
| Bilby                   | 75     |
| Golden Cog              | 40     |
| Time Attack Thunder Egg | 125    |
| Opal Thunder Egg        | 30     |
| Bilby Thunder Egg       | 25     |
| Opal                    | 3      |
| Picture Frame           | 10     |
| Rainbow Scale           | 100    |
| Talisman                | 200    |
| Crate                   | 0      |

## Rules

Across the 5 minutes, at roughly 50 second intervals, a rule will be introduced that changes the game until the next rule comes into play.
Here are the rules:

### These Taste Bad
Opals now give negative points.

### Gold Rush
Cogs now give double points.

### You've Been Framed
Picture Frames give points equal to the total number of them for the current level.

### Swapsies
Time to trade places! You're now where you weren't but where they were but are not now.

### Tasty
Opals are worth an extra point.

### Golden Goose
Birds lay eggs right? Yeah those ones give double points.

### Bilby Tax
Bilbies cause the other players to lose points.

### Run
At the next rule change get points corresponding to how far you are from where you are now.

### Dead!
Oops, that looked nasty!

### Pandora's Box
Crates give points... but sadly for you, no hope.

### Half Points
Check your total... it's half now.

### Error Detected
Uh oh! Points have a chance to go to your opponents instead.

### Nopals
Where did they go?