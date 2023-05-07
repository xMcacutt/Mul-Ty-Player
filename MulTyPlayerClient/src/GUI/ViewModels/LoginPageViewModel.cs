using MulTyPlayerClient.Settings;
using MulTyPlayerClient.Networking;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Riptide;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageViewModel : BaseViewModel
    {
        public string Name { get; set; } = "";
        public string Pass { get; set; } = "";
        public string ConnectingAddress { get; set; } = "";

        public bool HideName { get; set; } = false;
        public bool HidePass { get; set; } = true;
        public bool HideAddress { get; set; } = true;

        public ICommand ConnectCommand
        {
            get; set;
        }
        public bool ConnectEnabled { get; set; } = true;

        public event EventHandler OnConnectionSuccessful;

        private static List<ServerListing> _serverList;

        public LoginPageViewModel() : base()
        {
            Setup();
            ConnectCommand = new RelayCommand(Connect);
        }

        public void Connect()
        {
            ConnectEnabled = false;
            Replication.RegisterConnectionService();
            ConnectionService.ConnectionSuccessful += SaveDetails;
            ConnectionService.ConnectionFailed += () => { };            
            ConnectionService.Connect(ConnectingAddress, Pass);            
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
                if (entry.Length == 3)
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
            if (SettingsHandler.Settings.DoGetSteamName && SteamHelper.TryGetName(out string username))
            {
                Name = username;

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
