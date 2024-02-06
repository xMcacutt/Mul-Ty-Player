# Ready System

---

The ready system is designed to get everyone starting speedruns or hide and seek sessions at the same time. When you press the ready button in the [lobby](./LobbyUI.md) or run the [ready command](./Commands/ReadyCommand.md), a flag will appear in the Player Info List notifying all other clients that you are ready. Once everyone is ready, a countdown will run depending on the current mode.

---

## Normal Countdown

The regular countdown lasts for 10 seconds. For the first seven seconds a bunyip sound will play. When three seconds remain, the Ty2 race countdown sounds will play to notify you that you should be ready to start on go.

The countdown can be aborted by the host using the [countdown command](./Commands/CountdownCommand.md). 

(tldr /cd abort)

---

## Hide & Seek Countdown

If Hide & Seek Mode is activated on the server, the countdown will last 75 seconds so that the hiders can hide. Please read the [Hide & Seek Mode documentation](./HideSeek.md) for more information on this countdown.

This countdown can be aborted using the [hideseek command](./Command/HideSeekCommand.md).

(tldr /hs abort)
