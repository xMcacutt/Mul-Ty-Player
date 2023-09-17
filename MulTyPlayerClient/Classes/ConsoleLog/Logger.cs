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
        private static readonly string _initTime = DateTime.Now.ToString("MM-dd-yyyy h-mm-ss");
        private static string _fileName;
        private static string _filePath;
        private static int _maxLogMessageCount;

        public Logger(int maxMessageCount)
        {
            Instance = this;
            EventMessageLogger.Init();
            if (SettingsHandler.Settings.CreateLogFile) CreateLogFile();
            _maxLogMessageCount = maxMessageCount;
            Log = new();
        }

        public static void CreateLogFile()
        {
            _fileName = "MTP-Log " + _initTime;
            if (!Directory.Exists("./Logs/")) Directory.CreateDirectory("./Logs/");
            _filePath = "./Logs/" + _fileName + ".mtpl";
            using (var fileStream = File.Create(_filePath))
            {
                fileStream.Close();
            }
            string[] initText = { "Mul-Ty-Player Log File", "Created " + DateTime.Now.ToString() };
            File.AppendAllLines(_filePath, initText);
        }

        public void Write(string message)
        {
            if (_filePath == null)
            {
                CreateLogFile();
            }
            if (SettingsHandler.Settings.CreateLogFile)
            {
                try
                {
                    using FileStream fileStream = new(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    using StreamWriter writer = new(fileStream);
                    writer.WriteLine(message);
                }
                catch (Exception ex)
                {
                    // Handle the exception here
                    Debug.WriteLine($"Error writing to log file: {ex.Message}");
                }
            }
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                    () => {
                        Log.Add(message);
                        if (Log.Count > _maxLogMessageCount) Log.RemoveAt(0);
                    });
        }


        public void WriteDebug(string message)
        {
#if DEBUG
            Write("[DEBUG]: " + message);
#endif
        }
    }
}
