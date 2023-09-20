using MulTyPlayerClient.Classes.ConsoleLog;
using PropertyChanged;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MulTyPlayerClient
{
    [AddINotifyPropertyChangedInterface]
    public static class Logger
    {
        public static event Action<string> OnLogWrite;
        private static readonly string _initTime = DateTime.Now.ToString("MM-dd-yyyy h-mm-ss");
        private static string _fileName;
        private static string _filePath;
        private static StreamWriter _writer;

        static Logger()
        {
            EventMessageLogger.Init();
            CreateLogFile();
        }

        public static void CreateLogFile()
        {
            if (!SettingsHandler.Settings.CreateLogFile)
                return;

            _fileName = "MTP-Log " + _initTime;
            Directory.CreateDirectory("./Logs/");
            _filePath = "./Logs/" + _fileName + ".mtpl";
            try
            {
                _writer = File.AppendText(_filePath);
                _writer.WriteLine($"Mul-Ty-Player Log File\nCreated: {_initTime}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create log file at {_filePath}.\nException: {ex.Message}", "Log File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Write($"Failed to create log file at {_filePath}.\nException: {ex}");
            }
        }

        public static void Write(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OnLogWrite?.Invoke(message);
            });

            if (_writer is null)
            {
                Debug.WriteLine("Log writer is null, not writing to log file");
                return;
            }
            try
            {
                _writer.WriteLine(message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to write to log file at {_filePath}.\nException: {ex}\nLog message contents:\n{message}");
            }
        }

        public static void WriteDebug(string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string memberName = "", [CallerFilePath] string callerFilePath = "")
        {
#if DEBUG
            Write("[DEBUG]: " + message);
            DevLog.WriteLine(message, lineNumber, memberName, callerFilePath);
#endif
        }

        public static void Close()
        {
            _writer?.Flush();
            _writer?.Close();
            DevLog.Close();
        }
    }
}
