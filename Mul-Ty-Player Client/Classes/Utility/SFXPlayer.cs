using System;
using System.Collections.Generic;
using System.Windows.Media;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsWPF;
using MulTyPlayerClient.Classes.Networking;

namespace MulTyPlayerClient;

public enum SFX
{
    PlayerConnect,
    PlayerDisconnect,
    MenuClick1,
    MenuClick2,
    MenuAccept,
    MenuCancel,
    Race321,
    Race10,
    RaceAbort,
    LevelComplete,
    TAOpen,
    HS_Warning,
    HS_HideStart,
    HS_SeekStart,
    Punch,
    Objective,
    Taunt,
    RangGet,
    CheatActivated,
    Freeze,
    Unfreeze,
    Flashbang,
    JumpBoost,
    Alert,
    RuleChange,
    BagCollect,
    SpeedUp,
    VIPJoinMatt,
    VIPJoinSirbeyy,
    VIPJoinBuzchy,
    VIPJoinKythol
}

public static class SFXPlayer
{
    private static readonly Dictionary<SFX, Uri> sfxResources = new()
    {
        { SFX.PlayerConnect, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/PlayerConnect.wav") },
        { SFX.PlayerDisconnect, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/PlayerDisconnect.wav") },
        { SFX.MenuClick1, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuClick1.wav") },
        { SFX.MenuClick2, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuClick2.wav") },
        { SFX.MenuAccept, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuAccept.wav") },
        { SFX.MenuCancel, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/MenuCancel.wav") },
        { SFX.Race321, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Race321.wav") },
        { SFX.Race10, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Race10.wav") },
        { SFX.RaceAbort, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/RaceAbort.wav") },
        { SFX.LevelComplete, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/LevelComplete.wav")},
        { SFX.TAOpen, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/TAOpen.wav")},
        { SFX.HS_Warning, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/HS_Warning.wav")},
        { SFX.HS_HideStart, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/HS_HideStart.wav")},
        { SFX.HS_SeekStart, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/HS_SeekStart.wav")},
        { SFX.Punch, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Punch.wav")},
        { SFX.Objective, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Objective.wav")},
        { SFX.Taunt, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Taunt.wav")},
        { SFX.RangGet, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/RangGet.wav")},
        { SFX.CheatActivated, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/CheatActivated.wav")},
        { SFX.Freeze, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Freeze.wav")},
        { SFX.Unfreeze, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Unfreeze.wav")},
        { SFX.Flashbang, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Flashbang.wav")},
        { SFX.JumpBoost, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/JumpBoost.wav")},
        { SFX.Alert, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/Alert.wav")},
        { SFX.RuleChange, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/RuleChange.wav")},
        { SFX.BagCollect, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/BagCollect.wav")},
        { SFX.SpeedUp, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/SpeedUp.wav")},
        { SFX.VIPJoinBuzchy, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/VIPJoinBuzchy.wav")},
        { SFX.VIPJoinKythol, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/VIPJoinKythol.wav")},
        { SFX.VIPJoinMatt, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/VIPJoinMatt.wav")},
        { SFX.VIPJoinSirbeyy, new Uri(@"pack://siteoforigin:,,,/GUI/Sounds/VIPJoinSirbeyy.wav")}
    };

    private static Dictionary<SFX, MediaPlayer> mediaPlayers;

    public static void Init()
    {
        mediaPlayers = new Dictionary<SFX, MediaPlayer>();

        foreach (var kvPair in sfxResources) AddSound(kvPair.Key, kvPair.Value);
    }

    public static void PlaySound(SFX sfxName)
    {
        if (mediaPlayers.TryGetValue(sfxName, out var player))
            player.Dispatcher.Invoke(() =>
            {
                player.Stop();
                player.Play();
            });
    }
    
    public static void PlaySound(SFX sfxName, float volume)
    {
        if (mediaPlayers.TryGetValue(sfxName, out var player))
            player.Dispatcher.Invoke(() =>
            {
                player.Stop();
                player.Volume = volume;
                player.Play();
            });
    }

    public static void StopAll()
    {
        foreach (var player in mediaPlayers.Values) player.Dispatcher.Invoke(player.Stop);
    }

    public static void StopSound(SFX sfxName)
    {
        if (mediaPlayers.TryGetValue(sfxName, out var player)) player.Dispatcher.Invoke(player.Stop);
    }

    private static void AddSound(SFX sfxName, Uri uri, bool overrideOldSFX = false)
    {
        if (overrideOldSFX)
        {
            //If overriding this sfxType, remove the old sound
        }
        else if (mediaPlayers.ContainsKey(sfxName))
        {
            //If not overriding and mediaplayer already contains the sfx, exit
            return;
        }

        MediaPlayer mp = new();
        mp.Open(uri);
        mp.Volume = 0.15f;
        mp.MediaEnded += delegate { mp.Volume = 0.15f; };
        mediaPlayers.Add(sfxName, mp);
    }

    //Returns true if sound successfully removed
    //False if not
    private static bool RemoveSound(SFX sfxName)
    {
        if (mediaPlayers.TryGetValue(sfxName, out var player))
        {
            player.Stop();
            player.Close();
            return mediaPlayers.Remove(sfxName);
        }

        return false;
    }
}