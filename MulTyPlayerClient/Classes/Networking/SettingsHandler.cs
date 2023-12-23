using System.Collections.Generic;
using System.IO;
using System.Windows;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Newtonsoft.Json;
using Riptide;

namespace MulTyPlayerClient;

internal static class SettingsHandler
{
    //GLOBAL SETTINGS [RECEIVED FROM SERVER]
    public static bool DoTESyncing;
    public static bool DoCogSyncing;
    public static bool DoBilbySyncing;
    public static bool DoRangSyncing;
    public static bool DoOpalSyncing;
    public static bool DoPortalSyncing;
    public static bool DoCliffsSyncing;
    public static bool DoRainbowScaleSyncing;
    public static bool DoFrameSyncing;

    public static Dictionary<string, bool> SyncSettings;
    public static Settings Settings { get; private set; }

    public static void Setup()
    {
        //MAIN SETTINGS
        var json = File.ReadAllText("./ClientSettings.json");
        Settings = JsonConvert.DeserializeObject<Settings>(json);
        
        App.AppColors.SetColors(Settings.Theme);
        
        //SYNC SETTINGS
        SyncSettings = new Dictionary<string, bool>
        {
            { "TE", false },
            { "Opal", false },
            { "Cog", false },
            { "Bilby", false },
            { "Crate", false },
            { "Portal", false },
            { "RC", false },
            { "Attribute", false },
            { "RainbowScale", false },
            { "Frame", false }
        };
    }

    public static void Save()
    {
        var json = JsonConvert.SerializeObject(Settings);
        File.WriteAllText("./ClientSettings.json", json);
    }

    [MessageHandler((ushort)MessageID.SyncSettings)]
    private static void HandleSettingsUpdate(Message message)
    {
        var b = message.GetBools();
        DoTESyncing = b[0];
        SyncSettings["TE"] = DoTESyncing;
        DoCogSyncing = b[1];
        SyncSettings["Cog"] = DoCogSyncing;
        DoBilbySyncing = b[2];
        SyncSettings["Bilby"] = DoBilbySyncing;
        DoRangSyncing = b[3];
        SyncSettings["Attribute"] = DoRangSyncing;
        DoOpalSyncing = b[4];
        SyncSettings["Opal"] = DoOpalSyncing;
        SyncSettings["Crate"] = DoOpalSyncing;
        DoPortalSyncing = b[5];
        SyncSettings["Portal"] = DoPortalSyncing;
        DoCliffsSyncing = b[6];
        SyncSettings["RC"] = DoCliffsSyncing;
        if (b.Length > 7)
        {
            DoRainbowScaleSyncing = b[7];
            SyncSettings["RainbowScale"] = DoRainbowScaleSyncing;
        }

        if (b.Length > 8)
        {
            DoFrameSyncing = b[8];
            SyncSettings["Frame"] = DoFrameSyncing;
        }
    }

    public static bool HasValidExePath()
    {
        return Settings.MulTyPlayerFolderPath != "" && Settings.MulTyPlayerFolderPath != null;
    }
    
}