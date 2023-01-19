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
        public string MessageText { get; set; } = "Hello!";

        public Splash()
        {
            InitializeComponent();
            DataContext = this;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) => FindTy();
            backgroundWorker.RunWorkerCompleted += (s, e) => { this.Close(); };
            backgroundWorker.RunWorkerAsync();
        }

        private void FindTy()
        {
            Thread.Sleep(2000);
            var messageShown = false;
            while (ProcessHandler.FindTyProcess() == null)
            {
                if (!messageShown)
                {
                    MessageText = "Mul-Ty-Player could not be found. Please open the game to continue";
                    messageShown = true;
                }
            }
            MessageText = "Mul-Ty-Player is open!";
            Thread.Sleep(1000);
            
        }

    }
}
