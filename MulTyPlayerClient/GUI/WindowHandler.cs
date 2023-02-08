﻿using System;
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

        public static void InitializeWindows(Splash splash)
        {
            WindowHandler.SplashWindow = splash;
            WindowHandler.LoginWindow = new();
            WindowHandler.KoalaSelectWindow = new();
            WindowHandler.ClientGUIWindow = new();
        }
    }
}
