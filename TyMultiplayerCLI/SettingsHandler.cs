using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerCLI
{
    internal class SettingsHandler
    {
        static string[] settingsFileLines;
        public static string _Password;
        public static bool _DoGetSteamName;
        public static string _DefaultName;
        public static string _DefaultAddress;
        public static bool _DoKoalaCollision;
        public static bool _DoPositionLogging;
        public static string _PositionLoggingOutputDir;


        public static void Setup()
        {
            settingsFileLines = File.ReadAllLines("./ClientSettings.mtps");
            _Password = settingsFileLines[2].Split('=')[1].TrimStart();
            _DoGetSteamName = settingsFileLines[6].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            _DefaultName = settingsFileLines[8].Split('=')[1].TrimStart();
            _DefaultAddress = settingsFileLines[10].Split('=')[1].TrimStart();
            _DoKoalaCollision = settingsFileLines[12].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            _DoPositionLogging = settingsFileLines[14].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            _PositionLoggingOutputDir = settingsFileLines[16].Split('=')[1].TrimStart();
        }


    }
}
