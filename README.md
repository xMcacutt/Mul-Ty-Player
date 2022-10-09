
# Mul-Ty-Player

A Multiplayer Mod for Ty the Tasmanian Tiger



## Installation

There are three parts to running Mul-Ty-Player
  - Mul-Ty-Player Client CLI (The client application)
  - Mul-Ty-Player Server CLI (The server application)
  - Data_PC.rkv (The modified files that the client app interacts with)
Installation for each of these is detailed below

### Data_PC.rkv
1. First download "Data_PC Multiplayer V1.0.0.rkv" from the releases page.
2. Since core parts of the game are modified without the client app running, you'll want to create a separate installation of Ty. To do this, go to your steam library and right click on Ty. Then go to manage -> browse local files
3. Now copy the contents of the folder that has been opened into a new folder placed somewhere accessible.
4. Replace Data_PC.rkv with the downloaded rkv.
5. Return to your steam library and click "add a game" in the bottom-left corner. Go to "add a non-steam game" -> "browse" and locate the TY.exe in the folder you created in step 4
6. Name this Mul-Ty-Player or something similar. Always load this from steam when running multiplayer.

### Mul-Ty-Player Client Command Line Interface
1. First download "Mul-Ty-Player Client CLI.zip" from the releases page.
2. Unzip the folder to an easily accessible place.
3. Run the exe.

### Mul-Ty-Player Server Command Line Interface
This app is only required if you are running a server for others to join.
To install simply download and place the exe in the same folder as the client CLI.

#### Port Forwarding For Server Hosts.
To run the server you'll need to open port 8750 on UDP.

1. Open CMD (Command Prompt).
2. Type 'ipconfig' and press return.
3. Find IPv4 address and copy this into browser search bar.
4. Log in to your router.
5. Set up a new port forwarding rule.
6. Paste IPv4 address into forwarding address, set port to 8750, set rule to UDP.
7. Go to https://www.whatismypublicip.com/ this is your public IP address.

With your port forwarding rule set up, people can now join your server using your public IP address.

## Features

#### Current Version Changes
- Koala kids representing other players
- Server and client apps
- Steamworks API implementation
- Koala collision removed

#### Future Plans
- Collectible synchronisation
- Proximity chat
- Dedicated servers
- Name Tags

#### Known Issues
- Boonie turns pink in Black Stump
- Splash screen displays 100% GS
- Speaking to Ken to start cable car mission in Black Stump crashes the game
- Collision active upon re-entry into the same level or on loading into Rainbow Cliffs from startup
- Koalas are affected by draw distance
- Black Stump time attack thunderegg becomes bricked and unobtainable if collected with client connected
- Black Stump 'Catch Boonie' thunderegg does not get collected if client is connected
- Fluffy's Fjord sometimes inaccessible with client connected ? 


## Support

Join the Ty [Speedrunning Discord](https://discord.gg/YvGMBMM36V) for help and support.

You can also leave feedback for future updates in my DMs on Discord Mcacutt#5671.


## Acknowledgements

 - [Riptide Networking](https://github.com/RiptideNetworking/Riptide)
 - [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET)
 - [Modding Team From Speedrunning Discord](https://discord.gg/YvGMBMM36V)
 
