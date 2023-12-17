using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Riptide;

namespace MulTyPlayerServer
{
    public static class Extensions
    {
        public static string RemoveWhiteSpaces(this string str)
        {
            return string.Concat(str.Where(c => !char.IsWhiteSpace(c)));
        }
    }

    internal class SettingsHandler
    {
        public static Settings Settings { get; private set; }

        public static Dictionary<string, bool> SyncSettings;

        public static void Setup()
        {
            string json = File.ReadAllText("./ServerSettings.json");
            Settings = JsonConvert.DeserializeObject<Settings>(json);

            SyncSettings = new()
            {
                {"TE", Settings.DoSyncTEs},
                {"Cog", Settings.DoSyncCogs},
                {"Bilby", Settings.DoSyncBilbies},
                {"Attribute", Settings.DoSyncRangs},
                {"Opal", Settings.DoSyncOpals},
                {"Crate", Settings.DoSyncOpals},
                {"Portal", Settings.DoSyncPortals},
                {"RC", Settings.DoSyncCliffs},
                {"RainbowScale", Settings.DoSyncScale},
                {"Frame", Settings.DoSyncFrame}
            };
        }

        public static void SendSettings(ushort clientId)
        {
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
                Settings.DoSyncFrame
            };
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.SyncSettings);
            message.AddBools(b);
            Server._Server.Send(message, clientId);
        }
    }
}
