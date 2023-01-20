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

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            //Run send button logic here
            return;
        }

        private void TextboxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            SendButtonClick(sender, null);
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MicButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
