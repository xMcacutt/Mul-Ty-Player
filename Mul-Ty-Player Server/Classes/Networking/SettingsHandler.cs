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
    public static Settings Settings { get; private set; }
    
    public static bool DoLevelLock { get; set; }
    public static int HideSeekTime { get; set; }
    public static GameMode GameMode { get; set; }

    private static float _hsRange = 65f;
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
        Settings = JsonConvert.DeserializeObject<Settings>(json);
        DoLevelLock = false;
        GameMode = GameMode.Normal;
        HideSeekTime = 75;
        SyncSettings = new Dictionary<string, bool>
        {
            { "TE", Settings.DoSyncTEs },
            { "Cog", Settings.DoSyncCogs },
            { "Bilby", Settings.DoSyncBilbies },
            { "Attribute", Settings.DoSyncRangs },
            { "Opal", Settings.DoSyncOpals },
            { "Crate", Settings.DoSyncOpals },
            { "Portal", Settings.DoSyncPortals },
            { "RC", Settings.DoSyncCliffs },
            { "RainbowScale", Settings.DoSyncScale },
            { "Frame", Settings.DoSyncFrame },
            { "InvisiCrate", Settings.DoSyncFrame }
        };
    }

    public static void Save()
    {
        var json = JsonConvert.SerializeObject(Settings);
        File.WriteAllText("./ServerSettings.json", json);
    }

    public static void SendSettings(ushort clientId, bool sendToAll = false)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.SyncSettings);
        
        bool[] b =
        {
            Settings.DoSyncTEs,
            Settings.DoSyncCogs,
            Settings.DoSyncBilbies,
            Settings.DoSyncRangs,
            Settings.DoSyncOpals,
            Settings.DoSyncPortals,
            Settings.DoSyncCliffs,
            Settings.DoSyncScale,
            Settings.DoSyncFrame,
            DoLevelLock,
        };
        message.AddBools(b);
        
        message.AddInt((int)GameMode);
        
        message.AddFloat(HSRange);

        message.AddInt(Program.HChaos.ChaosSeed);
        
        message.AddString(Settings.Version);
        
        if (sendToAll)
            Server._Server.SendToAll(message, clientId);
        else
            Server._Server.Send(message, clientId);
    }

    [MessageHandler((ushort)MessageID.SyncSettings)]
    public static void HandleUpdateSyncSettings(ushort fromClientId, Message message)
    {
        var b = message.GetBools();
        Settings.DoSyncTEs = b[0];
        Settings.DoSyncCogs = b[1];
        Settings.DoSyncBilbies = b[2];
        Settings.DoSyncRangs = b[3];
        Settings.DoSyncOpals = b[4];
        Settings.DoSyncPortals = b[5];
        Settings.DoSyncCliffs = b[6];
        Settings.DoSyncScale = b[7];
        Settings.DoSyncFrame = b[8];
        if (b.Length > 9) 
            DoLevelLock = b[9];
        
        SyncSettings = new Dictionary<string, bool>
        {
            { "TE", Settings.DoSyncTEs },
            { "Cog", Settings.DoSyncCogs },
            { "Bilby", Settings.DoSyncBilbies },
            { "Attribute", Settings.DoSyncRangs },
            { "Opal", Settings.DoSyncOpals },
            { "Crate", Settings.DoSyncOpals },
            { "Portal", Settings.DoSyncPortals },
            { "RC", Settings.DoSyncCliffs },
            { "RainbowScale", Settings.DoSyncScale },
            { "Frame", Settings.DoSyncFrame },
            { "InvisiCrate", Settings.DoSyncFrame }
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