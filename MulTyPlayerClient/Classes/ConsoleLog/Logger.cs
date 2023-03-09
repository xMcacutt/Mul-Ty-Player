using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    [AddINotifyPropertyChangedInterface]
    public class Logger
    {
        public ObservableCollection<string> Log { get; set; }
        public string Test { get; set; } = "hello test";
        private string _initTime;
        private string _fileName;
        private static string _filePath;
        private static int _maxLogMessageCount;

        public Logger(int maxMessageCount)
        {
            _initTime = DateTime.Now.ToString("MM-dd-yyyy h-mm-ss");
            if (SettingsHandler.CreateLogFile) CreateLogFile();
            _maxLogMessageCount = maxMessageCount;
            Log = new ObservableCollection<string> { "test1", "test2", "test3", "test3", "test3", "test3", "test3", "test3", "test3", "test3aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"};
        }

        public void CreateLogFile()
        {
            _fileName = "MTP-Log " + _initTime;
            _filePath = "./" + _fileName + ".mtpl";
            using (var fileStream = File.Create(_filePath))
            {
                fileStream.Close();
            }
            string[] initText = { "Mul-Ty-Player Log File", "Created " + DateTime.Now.ToString() };
            File.AppendAllLines(_filePath, initText);
        }

        public void Write(string message)
        {
            if (SettingsHandler.CreateLogFile) File.AppendAllText(_filePath, message + "\n");
            Log.Add(message);
            if (Log.Count > _maxLogMessageCount) Log.RemoveAt(0);  
        }
    }
}
