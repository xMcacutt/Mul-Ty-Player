#define DEBUG
using MulTyPlayerClient.Classes.ConsoleLog;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace MulTyPlayerClient
{
    [AddINotifyPropertyChangedInterface]
    public class Logger
    {
        public static Logger Instance { get; private set; }

        public ObservableCollection<string> Log { get; set; }
        public static event Action<string> OnLogWrite;

        private static readonly string _initTime = DateTime.Now.ToString("MM-dd-yyyy h-mm-ss");
        private static string _fileName;
        private static string _filePath;
        private static int _maxLogMessageCount;

        public Logger(int maxMessageCount)
        {
            Instance = this;
            EventMessageLogger.Init();
            CreateLogFile();
            _maxLogMessageCount = maxMessageCount;
            Log = new();
        }

        public static void CreateLogFile()
        {
            if (!SettingsHandler.Settings.CreateLogFile)
                return;

            _fileName = "MTP-Log " + _initTime;
            if (!Directory.Exists("./Logs/")) Directory.CreateDirectory("./Logs/");
            _filePath = "./Logs/" + _fileName + ".mtpl";
            string[] initText = { "Mul-Ty-Player Log File", "Created " + DateTime.Now.ToString() };
            File.AppendAllLines(_filePath, initText);
        }

        public void Write(string message)
        {
            if (_filePath == null)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Log.Add(message);
                if (Log.Count > _maxLogMessageCount)
                    Log.RemoveAt(0);
                OnLogWrite?.Invoke(message);
            });

            try
            {
                File.AppendAllText(_filePath, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        public void WriteDebug(string message)
        {
#if DEBUG
            Write("[DEBUG]: " + message);
#endif
        }
    }
}
