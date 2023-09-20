using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;
using System.Windows;

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
            WindowHandler.SettingsWindow.Hide();
            SettingsHandler.SaveSettingsFromSettingsMenu();
            e.Cancel = true;
        }
    }
}
