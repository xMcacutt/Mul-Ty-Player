using System.Windows;
using MulTyPlayerClient.Settings;

namespace MulTyPlayerClient.GUI.Views
{
    public partial class SettingsMenu : Window
    {
        public SettingsMenu()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsHandler.Save();
            e.Cancel = true;
        }
    }
}
