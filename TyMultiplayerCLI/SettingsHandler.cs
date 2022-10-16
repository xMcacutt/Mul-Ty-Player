using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal static class SettingsHandler
    {
        public static string Password;
        public static bool DoGetSteamName;
        public static string DefaultName;
        public static string DefaultAddress;
        public static bool DoKoalaCollision;
        public static bool DoPositionLogging;
        public static string PositionLoggingOutputDir;


        public static void Setup()
        {
            string[] settingsFileLines = File.ReadAllLines("./ClientSettings.mtps");
            Password = settingsFileLines[4].Split('=')[1].TrimStart();
            DoGetSteamName = settingsFileLines[6].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DefaultName = settingsFileLines[8].Split('=')[1].TrimStart();
            DefaultAddress = settingsFileLines[10].Split('=')[1].TrimStart();
            DoKoalaCollision = settingsFileLines[12].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoPositionLogging = settingsFileLines[14].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            PositionLoggingOutputDir = settingsFileLines[16].Split('=')[1].TrimStart();
        }


    }
}
