using PropertyChanged;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
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

        private List<ServerListing> _serverList;

        public LoginPageViewModel()
        {
            ConnectCommand = new RelayCommand(Connect);
        }

        public void Connect()
        {
            ConnectEnabled = false;
            Client.StartClient(ConnectingAddress, Name, Pass);
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
                _serverList.Add(new(entry[0], entry[1], false));
            }
        }

        public void Setup()
        {
            SteamClient.Init(411960);
            if (SteamClient.IsValid) Name = SteamClient.Name;
            else Name = "Please Enter Name";
            SteamClient.Shutdown();
            _serverList = new();
            if (Path.Exists("./list.servers"))
            {
                ParseServerList(File.ReadLines("./list.servers"));
                ServerListing current = _serverList.Where(x => x.ActiveDefault).First();
                ConnectingAddress = current.IP;
                Pass = current.Pass;
                return; 
            }
            ConnectingAddress = "192.168.1.1";
            Pass = "OPALS";
        }
    }
}
