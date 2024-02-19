# Mul-Ty-Player User Guide

---

## What is Mul-Ty-Player?

Mul-Ty-Player (MTP) is an application and modification of Ty the Tasmanian Tiger allowing up to eight people to connect online to play together. This is achieved by manually reading and writing to the game's memory to manipulate player positions, collectible states, etc. To run MTP, both the client (the application which manipulates the game's memory) and the modified game files must be run concurrently. The modified game files are installed via a sideloading method left over from Krome's development of Ty. 

---

## Installing Mul-Ty-Player

Since v3.0.0, MTP has used a dedicated updater app to make updating significantly easier. To install MTP:

1. Head over to [the releases page](https://github.com/xMcacutt/Mul-Ty-Player/releases) on the MTP github and download the most recent "Mul-Ty-Player Updater/Installer.zip".

2. Extract the files to a known location.

3. Run the updater add (Mul-Ty-Player Updater.exe)

4. Specify the path to your original Ty install. If you don't have the base game installed, you'll need to do that first.

5. Press the install button

Here you have several options. If you're wanting to host your own server, you'll need to tick the Install Server checkbox. The client and game checkboxes are checked by default. There is also a "Remove Magnet Randomization" checkbox which is checked by default and prevents quarter pie from ever showing up from a basket in favour of a magnet. This is patched directly into the exe.

6. Press the install button

The install will automatically download the Client (and Server if ticked) files and extract them to the specified location which is your Documents folder by default. It will then search for your original Ty install, copy the necessary files, patch the exe, and download the additional files for MTP.

Note that the exe is never and will never be distributed with MTP. Only the absolutely necessary files will be copied.

---

## Updating Mul-Ty-Player

Whenever a newer version of MTP is released, you'll receive a notification on the login screen warning you that there are updates available. When you see this message, it is a good idea to update. To do this, run the updater and press the update button. This will attempt to download the new files without overwriting any settings.

If the updater fails with "Could not find ClientSettings.json", you have likely moved your files since installing. In this case, you'll need to fix your install by pressing Setup and resetting the paths to your installed files. Then rerun the updater.

If the updater fails for any other reason, contact me using the details in the [contact](#contact) section of this document and perform a fresh install by deleting your files and following the instructions in the above section.

---

## Running Mul-Ty-Player

Once MTP is installed, you can run it by navigating to the install location and opening both the game and the client. The client will not proceed to the login screen until a valid MTP game process is found. The client will ****not**** detect vanilla Ty.

---

## Port Forwarding For Mul-Ty-Player

If you want to host your own server, you'll need to open port 8750 with UDP/TCP in your router settings. For more information, there are plenty of tutorials online explaining how to port forward. The default port is 8750 but this may be changed by changing the port in the ServerSettings.json file. All clients must then either change their default connecting port in their settings (or ClientSettings.json) or use the "ip:port"" syntax. 

---

## Features

There's tonnes of features in the most recent version of MTP. The following is a list with links to the respective documentation explaining them in more depth. It is recommended that you read them in order if you want a full understanding of how to get the most out of the app.

- [Login](./Login.md)

- [Koala Selection](./KoalaSelect.md)

- [Lobby UI](./LobbyUI.md)

- [Settings](./Settings.md)

- [Host System](./Host.md)

- [Commands](./Commands/Commands.md)

- [Ready System](./Ready.md)

- [Syncing](./Syncing/Syncing.md)

- [Level Lock Mode](./LevelLock.md)

- [Hide & Seek Mode](./HideSeek.md)

- [Customization](./Customization.md)

---

## Contact

If you want to contact me to help support the project, give feedback, provide details of a bug, help with testing, or anything else please reach out to me via discord @mcacutt (Mcacutt#5671).
