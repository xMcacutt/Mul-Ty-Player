using MulTyPlayerClient.GUI;
using MulTyPlayerClient.Memory;
using MulTyPlayerClient.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MulTyPlayerClient.Logging;
using System.ComponentModel;
using MulTyPlayerClient.GUI.ViewModels;
using System.Windows.Forms;
using Riptide;

namespace MulTyPlayerClient
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SettingsHandler.Setup();
            MainWindow = new GUI.MainWindow();
            MainWindow.Show();
        }

        [STAThread]
        public static void Main(string[] args)
        {
#if DEBUG
            App app = new App();
            app.InitializeComponent();
            app.Run();
#else
            try
            {
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.WriteCriticalError(ex);
            }
#endif
        }
    }
}
