using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;
using System.Windows;

namespace MulTyPlayerClient.GUI
{
    public partial class MainWindow : Window
    {
        private MainViewModel mainViewModel;
        public MainWindow()
        {
            SettingsHandler.Setup();
            SFXPlayer.Init();
            _ = new Logger(200);
            ModelController.InstantiateModels();
            mainViewModel = new MainViewModel();
            DataContext = mainViewModel;
            InitializeComponent();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Client._client != null && Client._client.IsConnected)
            {
                Client._client.Disconnect();
            }
            TyProcess.CloseHandle();
            SteamHelper.Shutdown();
        }
    }
}
