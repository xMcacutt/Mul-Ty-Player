using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MulTyPlayerClient.GUI
{
    internal class WindowHandler
    {
        public static Splash SplashWindow;
        public static Login LoginWindow;
        public static KoalaSelect KoalaSelectWindow;
        public static ClientGUI ClientGUIWindow;
        public static SettingsMenu SettingsWindow;
        public static Setup SetupWindow;

        public static void InitializeWindows(Splash splash)
        {
            SplashWindow = splash;
            LoginWindow = new();
            KoalaSelectWindow = new();
            ClientGUIWindow = new();
            SettingsWindow = new();
            SetupWindow = new();
        }
    }
}
