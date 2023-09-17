using System.Windows;
using System.Windows.Controls;
using System.Security.Principal;

namespace MulTyPlayerClient.GUI.Views
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : UserControl
    {
        public Splash()
        {
            InitializeComponent();            
        }

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (isAdmin) WindowHandler.SetupWindow.Show();
            else SetupText.Text = "Run app as administrator to run setup.";
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            TyProcess.TryLaunchGame();
            Button button = sender as Button;
            button.IsEnabled = false;
        }
    }
}
