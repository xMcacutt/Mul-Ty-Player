using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MulTyPlayerClient
{
    internal static class SettingsHandler
    {
        public static string Password;
        public static bool DoGetSteamName;
        public static string DefaultName;
        public static string DefaultAddress;
        public static bool DoKoalaCollision;
        public static bool CreateLogFile;
        public static string LogFileOutputDir;
        public static bool DoTESyncing;
        public static bool DoCogSyncing;
        public static bool DoBilbySyncing;
        public static bool DoRangSyncing;
        public static bool DoOpalSyncing;
        public static bool DoPortalSyncing;
        public static bool DoCliffsSyncing;

        public static Dictionary<string, bool> SyncSettings;

        public static void Setup()
        {
            string[] settingsFileLines = File.ReadAllLines("./ClientSettings.mtps");
            Password = settingsFileLines[4].Split('=')[1].TrimStart();
            DoGetSteamName = settingsFileLines[6].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DefaultName = settingsFileLines[8].Split('=')[1].TrimStart();
            DefaultAddress = settingsFileLines[10].Split('=')[1].TrimStart();
            DoKoalaCollision = settingsFileLines[12].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            CreateLogFile = settingsFileLines[14].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            LogFileOutputDir = settingsFileLines[16].Split('=')[1].TrimStart();
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
