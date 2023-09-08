using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes.Utility
{
    internal static class SteamHelper
    {
        public const int TY_APP_ID = 411960;
        private static string steamUserName = "";

        //returns whether or not successfully intialized, or is already initialized
        public static bool Init()
        {
            if (SteamClient.IsValid)
                return true;

            try
            {
                SteamClient.Init(TY_APP_ID);
                return SteamClient.IsValid;
            }
            catch
            {
                return false;
            }
        }
        
        public static bool IsLoggedOn()
        {
            if (Init())
                return SteamClient.IsLoggedOn;
            return false;
        }

        public static bool TryGetName(out string userName)
        {
            if (steamUserName == "" && IsLoggedOn())
            {
                steamUserName = SteamClient.Name;
            }
            userName = steamUserName;
            return (userName == "");
        }

        public static void Shutdown()
        {
            SteamClient.Shutdown();
        }
    }
}
