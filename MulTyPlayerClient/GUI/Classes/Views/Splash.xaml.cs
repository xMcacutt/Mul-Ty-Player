using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;

namespace MulTyPlayerClient.GUI.Views;

/// <summary>
///     Interaction logic for Splash.xaml
/// </summary>
public partial class Splash : UserControl
{
    public Splash()
    {
        InitializeComponent();
    }

    private void Setup_Click(object sender, RoutedEventArgs e)
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        var isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        if (isAdmin) WindowHandler.SetupWindow.Show();
        else SetupText.Text = "Run app as administrator to run setup.";
    }

    private void Launch_Click(object sender, RoutedEventArgs e)
    {
        TyProcess.TryLaunchGame();
        var button = sender as Button;
        button.IsEnabled = false;
    }
}