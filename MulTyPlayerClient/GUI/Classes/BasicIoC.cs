﻿using System.Windows.Data;

namespace MulTyPlayerClient.GUI
{
    public class BasicIoC
    {
        public static SplashViewModel SplashScreenViewModel = new();
        public static LoginPageViewModel LoginViewModel = new();
        public static KoalasViewModel KoalaSelectViewModel = new();
        public static Logger LoggerInstance = new(200);
    }
}
