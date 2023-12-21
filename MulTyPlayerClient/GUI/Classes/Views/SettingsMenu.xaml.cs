using System.ComponentModel;
using System.Windows;

namespace MulTyPlayerClient.GUI.Views;

public partial class SettingsMenu : Window
{
    public SettingsMenu()
    {
        InitializeComponent();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        WindowHandler.SettingsWindow.Hide();
        SettingsHandler.SaveSettingsFromSettingsMenu();
        e.Cancel = true;
    }
}