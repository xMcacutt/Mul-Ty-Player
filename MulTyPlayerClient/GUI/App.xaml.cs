using MulTyPlayerClient.GUI;
using System.Windows;

namespace MulTyPlayerClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WindowHandler.InitializeWindows();
            new MainWindow().Show();
        }
    }
}
