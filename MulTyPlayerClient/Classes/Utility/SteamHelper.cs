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
        
        public static bool IsLoggedOn()
        {
            try
            {
                SteamClient.Init(TY_APP_ID);
                bool ok = SteamClient.IsValid && SteamClient.IsLoggedOn;
                SteamClient.Shutdown();
                return ok;

            }
            catch
            {
                SteamClient.Shutdown();
                return false;
            }
        }

        public static bool TryGetName(out string userName)
        {
            if (steamUserName != "")
            {
                userName = steamUserName;
                return true;
            }

            try
            {
                SteamClient.Init(TY_APP_ID);
                bool ok = SteamClient.IsValid && SteamClient.IsLoggedOn;
                if (ok)
                {
                    steamUserName = SteamClient.Name;
                    userName = steamUserName;
                    SteamClient.Shutdown();
                    return true;
                }
                else
                {
                    throw new Exception("Failed to get steam username");
                }
            }
            catch
            {
                userName = "";
                return false;
            }
        }
    }
}
