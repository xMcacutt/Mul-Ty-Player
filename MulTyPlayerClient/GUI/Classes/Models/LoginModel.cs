using MulTyPlayerClient.Classes.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.Models
{
    public class LoginModel
    {
        public event Action OnLoginSuccess;
        public event Action OnLoginFailed;

        public bool ConnectionAttemptCompleted = false;
        public bool ConnectionAttemptSuccessful = false;

        private List<ServerListing> _serverList;

        public LoginModel()
        {
            Setup();
        }

        string ip, name, pass;

        public void Connect(string ip, string name, string pass)
        {
            this.ip = ip;
            this.name = name;
            this.pass = pass;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) => AttemptConnection();
            backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    ModelController.LoggerInstance.Write(e.Error.ToString());
                    OnLoginFailed?.Invoke();
                }
                else if (e.Cancelled)
                {                    
                    OnLoginFailed?.Invoke();
                }

                if (ConnectionAttemptSuccessful)
                {
                    SaveDetails();
                    OnLoginSuccess?.Invoke();
                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        public string GetIP() => ip;
        public string GetName() => name;
        public string GetPass() => pass;

        private void AttemptConnection()
        {
            ConnectionAttemptCompleted = false;
            Client.StartClient(ip, name, pass);
            while (!ConnectionAttemptCompleted)
            {
                //twiddle thumbs
            }
        }

        private void Setup()
        {
            if (SettingsHandler.Settings.DoGetSteamName && SteamHelper.TryGetName(out name))
            {
                //twiddle thumbs again
            }
            else
            {
                name = SettingsHandler.Settings.DefaultName == "USER" ? GenerateRandomUser() : SettingsHandler.Settings.DefaultName;
            }
            _serverList = new();
            if (Path.Exists("./list.servers"))
            {
                ParseServerList(File.ReadLines("./list.servers"));
                return;
            }
            ip = "192.168.1.1";
            pass = "OPALS";
        }

        private void SaveDetails()
        {
            using (var fileStream = File.Create("./list.servers"))
                if (!_serverList.Where(x => x.IP == ip).Any())
                    _serverList.Add(new(ip, pass, true));
                else
                {
                    _serverList.Where(x => x.IP == ip).First().Pass = pass;
                    _serverList.Where(x => x.IP == ip).First().ActiveDefault = true;
                }
            foreach (ServerListing server in _serverList)
            {
                string s = $"{server.IP} {server.Pass}";
                if (server.ActiveDefault) s += " *";
                s += "\n";
                File.AppendAllText("./list.servers", s);
            }
        }

        private void ParseServerList(IEnumerable<string> file)
        {
            foreach (string server in file)
            {
                string[] entry = server.Split(' ');
                if (entry.Length == 3)
                {
                    ip = entry[0];
                    pass = entry[1];
                }
                _serverList.Add(new(entry[0], entry[1], false));
            }
        }

        private string GenerateRandomUser()
        {
            Random random = new();
            int randomNumber = random.Next(10000, 99999);
            return "USER" + randomNumber;
        }
    }
}
