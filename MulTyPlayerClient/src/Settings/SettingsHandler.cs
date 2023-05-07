using MulTyPlayerCommon;
using Newtonsoft.Json;
using Riptide;
using System;
using System.Collections.Generic;
using System.IO;

namespace MulTyPlayerClient.Settings
{
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

        public static Dictionary<string, bool> SyncSettings;
        public static Settings Settings
        {
            get; private set;
        }

        public static void Setup()
        {
            //MAIN SETTINGS
            string json = File.ReadAllText("./ClientSettings.json");
            Settings = JsonConvert.DeserializeObject<Settings>(json);

            //SYNC SETTINGS
            SyncSettings = new()
            {
                {"TE", false},
                {"Opal", false},
                {"Cog", false},
                {"Bilby", false},
                {"Crate", false},
                {"Portal", false},
                {"RC", false},
                {"Attribute", false},
                {"RainbowScale", false },
            };
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Settings);
            File.WriteAllText("./ClientSettings.json", json);
        }

        [MessageHandler((ushort)MessageID.SyncSettings)]
        public static void HandleSettingsUpdate(Message message)
        {
            bool[] bools = message.GetBools();
            DoTESyncing = bools[0];
            SyncSettings["TE"] = DoTESyncing;
            DoCogSyncing = bools[1];
            SyncSettings["Cog"] = DoCogSyncing;
            DoBilbySyncing = bools[2];
            SyncSettings["Bilby"] = DoBilbySyncing;
            DoRangSyncing = bools[3];
            SyncSettings["Attribute"] = DoRangSyncing;
            DoOpalSyncing = bools[4];
            SyncSettings["Opal"] = DoOpalSyncing;
            SyncSettings["Crate"] = DoOpalSyncing;
            DoPortalSyncing = bools[5];
            SyncSettings["Portal"] = DoPortalSyncing;
            DoCliffsSyncing = bools[6];
            SyncSettings["RC"] = DoCliffsSyncing;
            if (bools.Length > 7)
            {
                DoRainbowScaleSyncing = bools[7];
                SyncSettings["RainbowScale"] = DoRainbowScaleSyncing;
            }
        }

        public static bool HasValidExePath() => Settings.MulTyPlayerFolderPath != "" && Settings.MulTyPlayerFolderPath != null;
    }
}
