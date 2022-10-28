using RiptideNetworking;
using System;
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
        public static bool DoPositionLogging;
        public static string PositionLoggingOutputDir;
        public static bool DoTESyncing;
        public static bool DoCogSyncing;
        public static bool DoBilbySyncing;
        public static bool DoRangSyncing;
        public static bool DoOpalSyncing;

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
        
        public static void RequestServerSettings()
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.ReqSettings);
            Client._client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ReqSettings)]
        static void HandleSettingsUpdate(Message message)
        {
            bool[] b = message.GetBools();
            DoTESyncing = b[0];
            DoCogSyncing = b[1];
            DoBilbySyncing = b[2];
            DoRangSyncing = b[3];
            DoOpalSyncing = b[4];
        }
    }
}
