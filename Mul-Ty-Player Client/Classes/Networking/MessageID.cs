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
    ReqObjectiveSync,
    AdvancedTeleport,
    Voice,
    HS_ProxyRunHideSeek,
    HS_RoleChanged,
    HS_HideTimerStart,
    HS_StartSeek,
    HS_Warning,
    HS_Catch,
    HS_Abort,
    HS_RangeChanged,
    Crash,
    HS_Taunt,
    ForceLevelChange,
    CountdownFinishing,
    ForceMainMenu
}