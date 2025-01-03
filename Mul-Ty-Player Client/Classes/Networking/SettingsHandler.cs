﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MulTyPlayer;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using NAudio.Wave;
using Newtonsoft.Json;
using Riptide;

namespace MulTyPlayerClient;

internal static class SettingsHandler
{
    private static bool _doLevelLock;
    public static bool DoLevelLock
    {
        get => _doLevelLock;
        set
        {
            ModelController.Lobby.IsLevelLockEnabled = value;
            _doLevelLock = value;
        }
    }

    private static GameMode _gameMode;
    public static GameMode GameMode
    {
        get => _gameMode;
        set
        {
            if (_gameMode == value)
                return;
            switch (_gameMode)
            {
                case GameMode.HideSeek:
                    HSHandler.Deactivate();
                    break;
                case GameMode.Hardcore:
                    Client.HHardcore.Deinitialize();
                    break;
            };
            if (value != GameMode.Chaos && _gameMode == GameMode.Chaos)
                Client.HChaos.InitNewGameText(false, true);
            ModelController.Lobby.GameMode = value;
            _gameMode = value;
            switch (value)
            {
                case GameMode.HideSeek:
                    HSHandler.Activate();
                    break;
                case GameMode.Hardcore:
                    Client.HHardcore.Initialize();
                    break;
            };
        } 
    }
    
    public static float HSRange = 70f;

    #region SYNC SETTINGS
    
    public static Dictionary<string, bool> SyncSettings => _syncSettings;
    public static Dictionary<string, bool> _syncSettings;
    public static bool DoTESyncing
    {
        get => _syncSettings["TE"];
        set => _syncSettings["TE"] = value;
    }
    public static bool DoCogSyncing
    {
        get => _syncSettings["Cog"];
        set => _syncSettings["Cog"] = value;
    }
    public static bool DoBilbySyncing
    {
        get => _syncSettings["Bilby"];
        set => _syncSettings["Bilby"] = value;
    }
    public static bool DoRangSyncing
    {
        get => _syncSettings["Attribute"];
        set => _syncSettings["Attribute"] = value;
    }
    public static bool DoOpalSyncing
    {
        get => _syncSettings["Opal"];
        set
        {
            _syncSettings["Opal"] = value;
            _syncSettings["Crate"] = value;
        }
    }
    public static bool DoPortalSyncing
    {
        get => _syncSettings["Portal"];
        set => _syncSettings["Portal"] = value;
    }
    public static bool DoCliffsSyncing
    {
        get => _syncSettings["RC"];
        set => _syncSettings["RC"] = value;
    }
    public static bool DoRainbowScaleSyncing
    {
        get => _syncSettings["RainbowScale"];
        set => _syncSettings["RainbowScale"] = value;
    }
    public static bool DoFrameSyncing
    {
        get => _syncSettings["Frame"];
        set
        {
            _syncSettings["Frame"] = value;
            _syncSettings["InvisiCrate"] = value;
        }
    }
    public static bool DoMushroomSyncing
    {
        get => _syncSettings["Mushroom"];
        set => _syncSettings["Mushroom"] = value;
    }
    

    #endregion
    
    public static ClientSettings ClientSettings { get; private set; }


    public static void Setup()
    {
        //MAIN SETTINGS
        var json = File.ReadAllText("./ClientSettings.json");
        ClientSettings = JsonConvert.DeserializeObject<ClientSettings>(json);
        
        for (var i = 0; i < WaveIn.DeviceCount; i++) 
            if (string.Equals(WaveIn.GetCapabilities(i).ProductName, ClientSettings.VoiceInputDevice))
                VoiceHandler.UpdateInputDevice(i);
        
        App.AppColors.SetColors(ClientSettings.Theme);
        
        //SYNC SETTINGS
        _syncSettings = new Dictionary<string, bool>
        {
            { "TE", false },
            { "Cog", false },
            { "Opal", false },
            { "Bilby", false },
            { "Crate", false },
            { "Portal", false },
            { "RC", false },
            { "Attribute", false },
            { "RainbowScale", false },
            { "Frame", false },
            { "InvisiCrate", false },
            { "Mushroom", false }
        };
    }

    public static void Save()
    {
        var json = JsonConvert.SerializeObject(ClientSettings);
        File.WriteAllText("./ClientSettings.json", json);
    }
    
    [MessageHandler((ushort)MessageID.SyncSettings)]
    private static void HandleSettingsUpdate(Message message)
    {
        var b = message.GetBools();
        DoTESyncing = b[0];
        DoCogSyncing = b[1];
        DoBilbySyncing = b[2];
        DoRangSyncing = b[3];
        DoOpalSyncing = b[4];
        DoPortalSyncing = b[5];
        DoCliffsSyncing = b[6];
        DoRainbowScaleSyncing = b[7];
        DoFrameSyncing = b[8];
        DoMushroomSyncing = b[9];
        DoLevelLock = b[10];

        GameMode = (GameMode)message.GetInt();
        
        HSRange = message.GetFloat();

        Client.HChaos.ChaosSeed = message.GetInt();

        Client.HChaos.ShuffleOnStart = message.GetBool();

        var serverVersion = message.GetString();
        var clientVersion = ClientSettings.Version;
        switch (VersionHandler.Compare(clientVersion, serverVersion))
        {
            case null:
                Logger.Write("[WARN] Critical error, invalid version format.");
                break;
            case VersionResult.NeitherNewer:
                break;
            case VersionResult.FirstNewer or VersionResult.SecondNewer:
                Logger.Write($"[WARN] Client and server versions do not match. c{clientVersion} | s{serverVersion}");
                break;
        }

        Client.HHardcore.HardcoreRunDead = message.GetBool();
    }

    public static bool HasValidExePath()
    {
        return !string.IsNullOrEmpty(ClientSettings.MulTyPlayerFolderPath);
    }

    [MessageHandler((ushort)MessageID.HS_RangeChanged)]
    private static void HandleRangeChanged(Message message)
    {
        HSRange = message.GetFloat();
        Logger.Write($"Range changed to {HSRange}");
    }

    public static void UpdateSyncSettings()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SyncSettings);
        var b = new bool[]
        {
            DoTESyncing,
            DoCogSyncing,
            DoBilbySyncing,
            DoRangSyncing,
            DoOpalSyncing,
            DoPortalSyncing,
            DoCliffsSyncing,
            DoRainbowScaleSyncing,
            DoFrameSyncing,
            DoMushroomSyncing
        };
        message.AddBools(b);
        Client._client.Send(message);
    }

    private static void SendGameMode()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.GameMode);
        message.AddInt((int)GameMode);
        Client._client.Send(message);
    }

    [MessageHandler((ushort)MessageID.GameMode)]
    public static void HandleGameModeReceived(Message message)
    {
        GameMode = (GameMode)message.GetInt();
        Logger.Write($"Game mode changed to {Enum.GetName(typeof(GameMode), GameMode)}");
    }
}