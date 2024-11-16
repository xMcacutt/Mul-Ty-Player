using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Riptide;
using MulTyPlayer;

namespace MulTyPlayerServer;

public static class Extensions
{
    public static string RemoveWhiteSpaces(this string str)
    {
        return string.Concat(str.Where(c => !char.IsWhiteSpace(c)));
    }
}

internal class SettingsHandler
{
    
    public static Dictionary<string, bool> SyncSettings;
    public static ServerSettings ServerSettings { get; private set; }
    
    public static bool DoLevelLock { get; set; }
    public static int HideSeekTime { get; set; }

    private static GameMode _gameMode;
    public static GameMode GameMode
    {
        get => _gameMode;
        set
        {
            if (_gameMode == value)
                return;
            if (value != GameMode.Collection && _gameMode == GameMode.Collection)
                Program.HCollection.StopCollectionMode();
            _gameMode = value;
        }
    }

    private static float _hsRange = 80f;
    public static float HSRange
    {
        get => _hsRange;
        set
        {
            if (Math.Abs(_hsRange - value) < 0.01)
                return;
            _hsRange = value;
            var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_RangeChanged);
            message.AddFloat(value);
            Server._Server.SendToAll(message);
        }
    }

    public static void Setup()
    {
        var json = File.ReadAllText("./ServerSettings.json");
        ServerSettings = JsonConvert.DeserializeObject<ServerSettings>(json);
        DoLevelLock = false;
        GameMode = GameMode.Normal;
        HideSeekTime = 75;
        SyncSettings = new Dictionary<string, bool>
        {
            { "TE", ServerSettings.DoSyncTEs },
            { "Cog", ServerSettings.DoSyncCogs },
            { "Bilby", ServerSettings.DoSyncBilbies },
            { "Attribute", ServerSettings.DoSyncRangs },
            { "Opal", ServerSettings.DoSyncOpals },
            { "Crate", ServerSettings.DoSyncOpals },
            { "Portal", ServerSettings.DoSyncPortals },
            { "RC", ServerSettings.DoSyncCliffs },
            { "RainbowScale", ServerSettings.DoSyncScale },
            { "Frame", ServerSettings.DoSyncFrame },
            { "InvisiCrate", ServerSettings.DoSyncFrame }
        };
    }

    public static void Save()
    {
        var json = JsonConvert.SerializeObject(ServerSettings);
        File.WriteAllText("./ServerSettings.json", json);
    }

    public static void SendSettings(ushort clientId, bool sendToAll = false)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SyncSettings);
        
        bool[] b =
        {
            ServerSettings.DoSyncTEs,
            ServerSettings.DoSyncCogs,
            ServerSettings.DoSyncBilbies,
            ServerSettings.DoSyncRangs,
            ServerSettings.DoSyncOpals,
            ServerSettings.DoSyncPortals,
            ServerSettings.DoSyncCliffs,
            ServerSettings.DoSyncScale,
            ServerSettings.DoSyncFrame,
            DoLevelLock,
        };
        message.AddBools(b);
        
        message.AddInt((int)GameMode);
        
        message.AddFloat(HSRange);

        message.AddInt(Program.HChaos.ChaosSeed);

        message.AddBool(Program.HChaos.ShuffleOnStart);
        
        message.AddString(ServerSettings.Version);

        message.AddBool(HardcoreHandler.HardcoreRunDead);
        
        if (sendToAll)
            Server._Server.SendToAll(message, clientId);
        else
            Server._Server.Send(message, clientId);
    }

    [MessageHandler((ushort)MessageID.SyncSettings)]
    public static void HandleUpdateSyncSettings(ushort fromClientId, Message message)
    {
        var b = message.GetBools();
        ServerSettings.DoSyncTEs = b[0];
        ServerSettings.DoSyncCogs = b[1];
        ServerSettings.DoSyncBilbies = b[2];
        ServerSettings.DoSyncRangs = b[3];
        ServerSettings.DoSyncOpals = b[4];
        ServerSettings.DoSyncPortals = b[5];
        ServerSettings.DoSyncCliffs = b[6];
        ServerSettings.DoSyncScale = b[7];
        ServerSettings.DoSyncFrame = b[8];
        if (b.Length > 9) 
            DoLevelLock = b[9];
        
        SyncSettings = new Dictionary<string, bool>
        {
            { "TE", ServerSettings.DoSyncTEs },
            { "Cog", ServerSettings.DoSyncCogs },
            { "Bilby", ServerSettings.DoSyncBilbies },
            { "Attribute", ServerSettings.DoSyncRangs },
            { "Opal", ServerSettings.DoSyncOpals },
            { "Crate", ServerSettings.DoSyncOpals },
            { "Portal", ServerSettings.DoSyncPortals },
            { "RC", ServerSettings.DoSyncCliffs },
            { "RainbowScale", ServerSettings.DoSyncScale },
            { "Frame", ServerSettings.DoSyncFrame },
            { "InvisiCrate", ServerSettings.DoSyncFrame }
        };

        Save();
        SendSettings(fromClientId, true);
    }

    [MessageHandler((ushort)MessageID.GameMode)]
    public static void HandleGameModeReceived(ushort fromClientId, Message message)
    {
        var mode = (GameMode)message.GetInt();
        GameMode = mode;
        Console.WriteLine($"Game mode changed to {Enum.GetName(typeof(GameMode), mode)}");
        var response = Message.Create(MessageSendMode.Reliable, MessageID.GameMode);
        response.AddInt((int)GameMode);
        Server._Server.SendToAll(response);
    }
}