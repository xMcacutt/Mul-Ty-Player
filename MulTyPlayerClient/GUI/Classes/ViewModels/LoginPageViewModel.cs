using PropertyChanged;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageViewModel
    {
        public string Name { get; set; } = "";
        public string Pass { get; set; } = "";
        public string ConnectingAddress { get; set; } = "";

        public bool HideName { get; set; } = false;
        public bool HidePass { get; set; } = true;
        public bool HideAddress { get; set; } = true;

        public ICommand ConnectCommand { get; set; }
        public bool ConnectEnabled { get; set; } = true;

        public bool ConnectionAttemptCompleted = false;
        public bool ConnectionAttemptSuccessful = false;

        private List<ServerListing> _serverList;

        public LoginPageViewModel()
        {
            ConnectCommand = new RelayCommand(Connect);
        }

        public void Connect()
        {
            ConnectEnabled = false;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) => AttemptConnection();
            backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                if (ConnectionAttemptSuccessful)
                {
                    BasicIoC.LoginViewModel.SaveDetails();
                    BasicIoC.MainGUIViewModel.ResetPlayerList();
                    BasicIoC.KoalaSelectViewModel.Setup();
                    WindowHandler.KoalaSelectWindow.Show();
                    WindowHandler.LoginWindow.Hide();
                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void AttemptConnection()
        {
            ConnectionAttemptCompleted = false;
            Client.StartClient(ConnectingAddress, Name, Pass);
            while (!ConnectionAttemptCompleted)
            {

            }
        }

        public void SaveDetails()
        {
            using (var fileStream = File.Create("./list.servers"))
                if (!_serverList.Where(x => x.IP == ConnectingAddress).Any())
                    _serverList.Add(new(ConnectingAddress, Pass, true));
                else
                {
                    _serverList.Where(x => x.IP == ConnectingAddress).First().Pass = Pass;
                    _serverList.Where(x => x.IP == ConnectingAddress).First().ActiveDefault = true;
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
                if(entry.Length == 3)
                {
                    ConnectingAddress = entry[0];
                    Pass = entry[1];
                }
                _serverList.Add(new(entry[0], entry[1], false));
            }
        }

        public string GenerateRandomUser()
        {
            Random random = new();
            int randomNumber = random.Next(10000, 99999);
            return "USER" + randomNumber;
        }

        public void Setup()
        {
            if (SettingsHandler.Settings.DoGetSteamName)
            {
                SteamClient.Init(411960);
                Name = SteamClient.IsValid ? SteamClient.Name : GenerateRandomUser();
                SteamClient.Shutdown();
            }
            else
            {
                Name = SettingsHandler.Settings.DefaultName == "USER" ? GenerateRandomUser() : SettingsHandler.Settings.DefaultName;
            }
            _serverList = new();
            if (Path.Exists("./list.servers"))
            {
                ParseServerList(File.ReadLines("./list.servers"));
                return; 
            }
            ConnectingAddress = "192.168.1.1";
            Pass = "OPALS";
        }
    }
}
