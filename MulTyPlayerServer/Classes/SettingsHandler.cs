using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MulTyPlayerServer
{
    public static class Extensions
    {
        public static string RemoveWhiteSpaces(this string str)
        {
            return String.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
        }
    }

    internal class SettingsHandler
    {
        private static string[] _settingsFileLines;
        public static string Password;
        public static string[] KoalaOrder;
        public static bool DoSyncTEs;
        public static bool DoSyncCogs;
        public static bool DoSyncBilbies;
        public static bool DoSyncRangs;


        public static void Setup()
        {
            _settingsFileLines = File.ReadAllLines("./ServerSettings.mtps");
            Password = _settingsFileLines[4].Split('=')[1].TrimStart();
            KoalaOrder = _settingsFileLines[6].Split('=')[1].RemoveWhiteSpaces().Split(',');
            DoSyncTEs = _settingsFileLines[8].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncCogs = _settingsFileLines[10].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncBilbies = _settingsFileLines[12].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncRangs = _settingsFileLines[14].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
