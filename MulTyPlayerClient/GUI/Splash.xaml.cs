using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.CompilerServices;

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
    }
}
