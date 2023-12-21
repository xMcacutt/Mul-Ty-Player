using MulTyPlayerClient.Classes.ConsoleLog;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI
{
    public partial class MainWindow : Window
    {
        private MainViewModel mainViewModel;
        public MainWindow()
        {
            
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
            Logger.Close();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
