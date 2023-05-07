using MulTyPlayerClient.GUI.ViewModels;
using MulTyPlayerClient.Networking;
using MulTyPlayerClient.Settings;
using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MulTyPlayerClient.Networking
{
    internal static class ConnectionService
    {
        public static bool IsConnected => Client.IsConnected;
        public static event Action ConnectionSuccessful = Replication.BeginReplication;
        public static event Action ConnectionFailed = Replication.Cancel;
        public static event Action Disconnected = Replication.Cancel;
        public static event Action ReconnectAttempt;

        private static string connectedIP;
        private static string connectedPass;

        public static Client Client = new Client();

        public static void Connect(string ip, string pass)
        {
            Replication.RegisterConnectionService();
            Client.Connected += (o, e) => OnConnectionSuccessful(ip, pass);
            Client.Disconnected += OnDisconnected;
            Client.ConnectionFailed += OnConnectionFailed;
            AttemptConnection(ip, pass);
        }

        private static void AttemptConnection(string ip, string pass)
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) => AttemptConnectionWork(ip, pass);
            backgroundWorker.RunWorkerCompleted += (s, attempt) =>
            {
                if (attempt.Error != null || attempt.Cancelled || !IsConnected)
                {
                    OnConnectionFailed(s, new ConnectionFailedEventArgs(Message.Create()));
                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        private static void AttemptConnectionWork(string ip, string pass)
        {
            Message authentication = Message.Create();
            authentication.AddString(pass);

            if (!ip.Contains(':'))
            {
                ip += ":" + SettingsHandler.Settings.Port;
            }            

            Client.Connect(ip, 5, 0, authentication);
            while (Client.IsConnecting)
            {
            }
        }

        private static void OnConnectionSuccessful(string ip, string pass)
        {
            connectedIP = ip;
            connectedPass = pass;
            ConnectionSuccessful?.Invoke();
        }

        private static void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            SystemSounds.Hand.Play();
            MessageBox.Show("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
            ConnectionFailed?.Invoke();
        }

        private static void OnDisconnected(object sender, Riptide.DisconnectedEventArgs e)
        {
            MainViewModel.SFXPlayer.PlaySound(SFX.PlayerDisconnect);
            Disconnected?.Invoke();

            if (e.Reason == DisconnectReason.TimedOut && SettingsHandler.Settings.AttemptReconnect)
            {
                ReconnectAttempt?.Invoke();
                AttemptConnection(connectedIP, connectedPass);
            }
        }
    }
}
