using Newtonsoft.Json;
using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Xps.Serialization;

namespace MulTyPlayerClient
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

        public static Dictionary<string, bool> SyncSettings;
        public static Settings Settings { get; private set; }

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
            };
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(Settings);
            File.WriteAllText("./ClientSettings.json", json);
        }

        [MessageHandler((ushort)MessageID.SyncSettings)]
        static void HandleSettingsUpdate(Message message)
        {
            bool[] b = message.GetBools();
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
        }
    }
}
