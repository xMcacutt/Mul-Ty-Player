using Microsoft.Extensions.Primitives;
using MulTyPlayerClient.Logging;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class ChatLog
    {
        public static ChatLog Instance { get; private set; }

        public ObservableCollection<string> Messages { get; set; }

        private static int maxLogMessageCount;

        public ChatLog(int maxMessageCount)
        {
            maxLogMessageCount = maxMessageCount;
            Messages = new();
            Instance = this;
        }

        public static void Write(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    () =>
                    {
                        Instance.Messages.Add(message);
                        if (Instance.Messages.Count > maxLogMessageCount) Instance.Messages.RemoveAt(0);
                    });
        }
    }
}
