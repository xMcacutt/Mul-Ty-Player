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

    private void Launch_Click(object sender, RoutedEventArgs e)
    {
        TyProcess.TryLaunchGame();
        var button = sender as Button;
        button.IsEnabled = false;
    }
}