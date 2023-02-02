﻿using PropertyChanged;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageViewModel
    {
        public string Name { get; set; } = "";
        public string Pass { get; set; } = "";
        public string ConnectingAddress { get; set; } = "";

        public bool HideName { get; set; } = true;
        public bool HidePass { get; set; } = true;
        public bool HideAddress { get; set; } = true;

        public ICommand ConnectCommand { get; set; }

        public LoginPageViewModel()
        {
            ConnectCommand = new RelayCommand(Connect);
        }

        public void SetupLogin()
        {
            SteamClient.Init(411960);
            if (SteamClient.IsValid) Name = SteamClient.Name;
            else Name = "Please Enter Name";
            SteamClient.Shutdown();
            string serverListPath = "./list.servers";
            if (Path.Exists(serverListPath))
            {
                var serverList = File.ReadLines(serverListPath);
                string server = serverList.Where(x => x.EndsWith('*')).First();
                string[] entry = server.Split(' ');
                ConnectingAddress = entry[0];
                Pass = entry[1];
                return;
            }
            ConnectingAddress = "192.168.1.1";
            Pass = "OPALS";
        }

        public void Connect()
        {
            Client.StartClient(ConnectingAddress, Name, Pass);
        }
    }
}
