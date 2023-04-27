using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MulTyPlayerClient.GUI
{
    /// <summary>
    /// Interaction logic for ClientGUI.xaml
    /// </summary>
    public partial class ClientGUI : Window
    {
        public ClientGUI()
        {
            InitializeComponent();
        }

        private void TextboxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BasicIoC.MainGUIViewModel.ManageInput();
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            BasicIoC.SettingsMenuViewModel.Setup();
            WindowHandler.SettingsWindow.Show();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0)
                e = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta * -1);
            base.OnPreviewMouseWheel(e);
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            Client._client.Disconnect();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Client._client?.Disconnect();
            e.Cancel = true;
            WindowHandler.ClientGUIWindow.Hide();
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            Client.HCommand.SetReady();
        }
    }
}
