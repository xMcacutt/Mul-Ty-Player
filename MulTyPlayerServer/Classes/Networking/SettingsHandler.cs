using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Riptide;

namespace MulTyPlayerServer
{
    public static class Extensions
    {
        public static string RemoveWhiteSpaces(this string str)
        {
            return string.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
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
        public static bool DoSyncOpals;
        public static bool DoSyncPortals;
        public static bool DoSyncCliffs;

        public static Dictionary<string, bool> SyncSettings;
        
        public static void Setup()
        {
            _settingsFileLines = File.ReadAllLines("./ServerSettings.mtps");
            Password = _settingsFileLines[4].Split('=')[1].TrimStart();
            KoalaOrder = _settingsFileLines[6].Split('=')[1].RemoveWhiteSpaces().Split(',');
            DoSyncTEs = _settingsFileLines[8].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncCogs = _settingsFileLines[10].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncBilbies = _settingsFileLines[12].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncRangs = _settingsFileLines[14].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncOpals = _settingsFileLines[16].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncPortals = _settingsFileLines[18].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            DoSyncCliffs = _settingsFileLines[20].Split('=')[1].TrimStart().Equals("true", StringComparison.CurrentCultureIgnoreCase);
            SyncSettings = new()
            {
                {"TE", DoSyncTEs},
                {"Cog", DoSyncCogs},
                {"Bilby", DoSyncBilbies},
                {"Attribute", DoSyncRangs},
                {"Opal", DoSyncOpals},
                {"Crate", DoSyncOpals},
                {"Portal", DoSyncPortals},
                {"RC", DoSyncCliffs}
            };
        }

        [MessageHandler((ushort)MessageID.ReqSettings)]
        static void HandleSettingsRequest(ushort fromClientId, Message message)
        {
            bool[] b = { DoSyncTEs, DoSyncCogs, DoSyncBilbies, DoSyncRangs, DoSyncOpals, DoSyncPortals, DoSyncCliffs };
            Message response = Message.Create(MessageSendMode.Reliable, MessageID.ReqSettings);
            response.AddBools(b);
            Server._Server.Send(response, fromClientId);
        }
    }
}
