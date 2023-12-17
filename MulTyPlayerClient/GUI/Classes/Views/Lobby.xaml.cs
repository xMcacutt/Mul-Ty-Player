using MulTyPlayerClient.GUI.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.Views
{
    /// <summary>
    /// Interaction logic for ClientGUI.xaml
    /// </summary>
    public partial class Lobby : UserControl
    {
        public Lobby()
        {
            InitializeComponent();
        }

        private void TextboxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && e.IsDown)
            {                
                LobbyModel.ProcessInput(TextboxInput.Text);
                TextboxInput.Text = "";
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ModelController.Settings.SetCheckboxesFromSettings();
            WindowHandler.SettingsWindow.Show();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
                e = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta * -1);
            base.OnPreviewMouseWheel(e);
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            Client.HCommand.SetReady();
        }

        private void LaunchGameButton_Click(object sender, RoutedEventArgs e)
        {
            TyProcess.TryLaunchGame();
        }

        private void RequestSyncButton_Click(object sender, RoutedEventArgs e)
        {
            Client.HSync.RequestSync();
            Logger.Write("Sync request sent to server.");
        }
    }
}
