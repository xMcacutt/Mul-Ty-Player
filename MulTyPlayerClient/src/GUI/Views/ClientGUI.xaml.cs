using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MulTyPlayerClient.GUI.ViewModels;
using MulTyPlayerClient.Networking;

namespace MulTyPlayerClient.GUI.Views
{
    /// <summary>
    /// Interaction logic for ClientGUI.xaml
    /// </summary>
    public partial class ClientGUI : UserControl
    {
        public ClientGUI()
        {
            InitializeComponent();
        }

        private void TextboxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MainViewModel.Lobby.ManageInput();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Settings.Setup();
            //WindowHandler.SettingsWindow.Show();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
                e = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta * -1);
            base.OnPreviewMouseWheel(e);
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectionService.Client.Disconnect();
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            Replication.HCommand.SetReady();
        }

        private void LaunchGameButton_Click(object sender, RoutedEventArgs e)
        {
            Memory.TyProcess.TryLaunchGame();
        }
    }
}
