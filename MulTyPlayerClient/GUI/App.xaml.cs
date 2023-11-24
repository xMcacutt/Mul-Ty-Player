using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using System.Runtime.Versioning;
using System.Windows;

namespace MulTyPlayerClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SettingsHandler.Setup();
            ModelController.InstantiateModels();
            WindowHandler.InitializeWindows();
            SFXPlayer.Init();
            mw = new MainWindow();
            mw.Show();
            mw.Activate();
        }

        MainWindow mw;
    }
}
