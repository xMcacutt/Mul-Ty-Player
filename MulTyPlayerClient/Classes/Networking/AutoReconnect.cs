using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
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
            Client.IsConnected = false;
            Client.ClientThreadToken.Cancel();
            ModelController.KoalaSelect.MakeAllAvailable();
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    ModelController.LoggerInstance.Log.Clear();
                }));
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
        }
    }
}
