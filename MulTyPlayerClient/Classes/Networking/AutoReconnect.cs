using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MulTyPlayerClient
{
    internal class AutoReconnect
    {
        public static void ConnectionFailed()
        {
            Client.IsRunning = false;
            Client.cts.Cancel();
            BasicIoC.KoalaSelectViewModel.MakeAllAvailable();
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    WindowHandler.KoalaSelectWindow.Hide();
                    WindowHandler.ClientGUIWindow.Hide();
                    BasicIoC.LoggerInstance.Log.Clear();
                    BasicIoC.LoginViewModel.ConnectEnabled = true;
                    WindowHandler.LoginWindow.Show();
                }));
            BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = false;
            BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
            return;
        }
    }
}
