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

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class LoginPageViewModel
    {
        public string Name { get; set; }
        public string Pass { get; set; }
        public string ConnectingAddress { get; set; }

        public LoginPageViewModel()
        {

        }

        /*
         list.servers example
        192.186.1.1 CRATE
        192.186.1.2 OPALS *
        192.186.1.3 FRILL
         */
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

    }
}
