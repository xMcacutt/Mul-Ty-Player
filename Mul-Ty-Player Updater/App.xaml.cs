using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MulTyPlayerClient.GUI;

namespace Mul_Ty_Player_Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Colors AppColors;
        
        public App()
        {
            var settingsHandler = new SettingsHandler();
            AppColors = new Colors();
        }
    }
}