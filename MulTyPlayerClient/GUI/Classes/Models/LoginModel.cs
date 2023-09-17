using Microsoft.CodeAnalysis;
using MulTyPlayerClient.Classes.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;

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
                    LoginFailed();
                    return;
                }
                else if (e.Cancelled)
                {
                    LoginFailed();
                    return;
                }

                if (ConnectionAttemptSuccessful)
                {
                    SaveDetails();
                    OnLoginSuccess?.Invoke();
                }
                else
                {
                    LoginFailed();
                    return;
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
            Client.Start(ip, name, pass);
            while (!ConnectionAttemptCompleted)
            {
            }
        }

        private void Setup()
        {
            SetName();

            if (Path.Exists("./list.servers"))
            {
                ParseServerList(File.ReadLines("./list.servers"));
            }
            else
            {
                var _fs = File.Create("./list.servers");
                _fs.Close();
            }
        }

        public void SetName()
        {
            //If steam name retrieval is successful, set that as the name and set the default name in settings
            if (SettingsHandler.Settings.DoGetSteamName)
            {
                name = SteamHelper.GetSteamName();
                SettingsHandler.Settings.DefaultName = name;
            }
            //If the steam name setting is disabled, use the previously stored default name
            else
            {
                name = SettingsHandler.Settings.DefaultName;
            }
            //If the name is still null, generate a random one
            if (name == null)
            {
                name = GenerateRandomUser();
            }
        }

        private void SaveDetails()
        {
            //Save the currently connected server details to serverlist
            if (!_serverList.Where(x => x.IP == ip).Any())
                _serverList.Add(new(ip, pass, true));
            else
            {
                _serverList.Where(x => x.IP == ip).First().Pass = pass;
                _serverList.Where(x => x.IP == ip).First().ActiveDefault = true;
            }

            //Save serverlist to file
            using FileStream fs = File.Create("./list.servers");
            string servers = "";
            foreach (ServerListing server in _serverList)
            {
                servers += $"{server.IP} {server.Pass}";
                if (server.ActiveDefault)
                    servers += " *";
                servers += "\n";
            }
            byte[] serverInfo = new UTF8Encoding(true).GetBytes(servers);
            fs.Write(serverInfo, 0, serverInfo.Length);
        }

        private void ParseServerList(IEnumerable<string> file)
        {
            _serverList = new List<ServerListing>();
            ServerListing local = new ServerListing("192.168.1.1", "OPALS", false);
            foreach (string server in file)
            {
                string[] entry = server.Split(' ');
                _serverList.Add(new(entry[0], entry[1], false));
            }

            SetDetailsFromServer(_serverList.FirstOrDefault(local));
        }

        private void SetDetailsFromServer(ServerListing server)
        {
            ip = server.IP;
            pass = server.Pass;
        }

        private string GenerateRandomUser()
        {
            Random random = new();
            int randomNumber = random.Next(100000, 999999);
            return "USER" + randomNumber;
        }

        private void LoginFailed()
        {
            SystemSounds.Hand.Play();
            MessageBox.Show("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
            OnLoginFailed?.Invoke();
        }
    }
}
