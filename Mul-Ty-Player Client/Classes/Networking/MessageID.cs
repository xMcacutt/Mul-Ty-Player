﻿namespace MulTyPlayer;

public enum MessageID : ushort
{
    PlayerInfo,
    KoalaCoordinates,
    ConsoleSend,
    ServerCollectibleDataUpdate,
    ClientCollectibleDataUpdate,
    Disconnect,
    ResetSync,
    ReqCollectibleSync,
    SyncSettings,
    ReqHost,
    HostChange,
    KoalaSelected,
    AnnounceDisconnect,
    P2PMessage,
    Ready,
    Countdown,
    StopWatch,
    GiftHost,
    SetLevelLock,
    LL_LevelEntered,
    LL_LevelCompleted,
    LL_Sync,
    DespawnAllBilbies,
    Kick,
    GetPassword,
    SetPassword,
    ObjectiveObjectActivated,
    ObjectiveStateChanged,
    ReqObjectiveSync
}