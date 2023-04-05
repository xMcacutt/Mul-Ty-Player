using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace MulTyPlayerClient.GUI
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
            WindowHandler.InitializeWindows(this);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            bool isAdmin;
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            if (isAdmin) WindowHandler.SetupWindow.Show();
            else SetupText.Text = "Run app as administrator to run setup.";
        }
    }
}
