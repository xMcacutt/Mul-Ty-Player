using MulTyPlayerClient.GUI.Models;
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

        public static bool Initialized { get => SteamClient.IsValid; }

        public static void Init()
        {
            if (Initialized)
            {
                return;
            }
            else
            {
                try
                {
                    SteamClient.Init(TY_APP_ID);
                }
                catch (Exception e)
                {
                    ModelController.LoggerInstance.Write("Error: SteamAPI initialization failed\n" + e.ToString);
                }
            }
        }
        
        public static bool IsLoggedOn()
        {
            if (Initialized)
                return SteamClient.IsLoggedOn;
            return false;
        }

        public static string GetSteamName()
        {
            string userName = null;
            Init();

            if (IsLoggedOn())
            {
                userName = SteamClient.Name;
            }            
            return userName;
        }

        public static void Shutdown()
        {
            SteamClient.Shutdown();
        }
    }
}
