using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes.ConsoleLog
{
    internal class Logger
    {
        private static List<string> Log;
        private string _initTime;
        private string _fileName;
        private static string _filePath;
        private static int _maxLogMessageCount;

        public Logger(int maxMessageCount)
        {
            _initTime = DateTime.Now.ToString();
            _maxLogMessageCount = maxMessageCount;
            Log = new();
            if (SettingsHandler.CreateLogFile) CreateLogFile();
        }

        public void CreateLogFile()
        {
            _fileName = "MTP-Log " + _initTime;
            _filePath = SettingsHandler.LogFileOutputDir + "/" + _fileName + ".mtpl";
            File.Create(_filePath);
            string[] initText = {"Mul-Ty-Player Log File", "Created " + DateTime.Now.ToString() };
            File.AppendAllLines(_filePath, initText);
        }

        public static void Write(string message)
        { 
            if (SettingsHandler.CreateLogFile) File.AppendAllText(_filePath, message + "\n");
            Log.Add(message);
            if (Log.Count > _maxLogMessageCount) Log.RemoveAt(0);  
        }
    }
}
