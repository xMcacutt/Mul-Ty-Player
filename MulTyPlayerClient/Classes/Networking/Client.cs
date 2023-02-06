using MulTyPlayerClient.Classes.ConsoleLog;
using MulTyPlayerClient.GUI;
using Riptide;
using Riptide.Utils;
using System;
using System.Media;
using System.Net.Cache;
using System.Threading;
using System.Windows;

namespace MulTyPlayerClient
{
    internal class Client
    {
        public static bool IsRunning;
        public static Riptide.Client _client;
        private static Thread _loop;
        private static string _ip;
        private static string _pass;
        private static string _name;

        public static void StartClient(string ip, string name, string pass)
        {
            Logger logger = new(100);
            RiptideLogger.Initialize(Logger.Write, true);
            _ip = ip;
            _pass = pass;
            _name = name;

            _client = new Riptide.Client();
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected();
            _client.ConnectionFailed += (s, e) => ConnectionFailed();

            Message authentication = Message.Create();
            authentication.AddString(_name);
            authentication.AddString(_pass);
            _client.Connect(_ip + ":8750", 5, 0, authentication);

            _loop = new Thread(new ThreadStart(Loop));
            _loop.Start();

            /* NEEDS TO BE HANDLED ON A BACKGROUND WORKER
            IsRunning = true;
            while (IsRunning)
            {
                _client.Update();
                //CHECK IF ON MENU OR LOADING
                if (!HGameState.CheckMenuOrLoading())
                {
                    //IF NOT SET UP LOAD INTO LEVEL STUFF
                    if (!HLevel.LoadedNewLevelNetworkingSetupDone)
                    {
                        Program.HKoala.SetCoordAddrs();
                        HLevel.LoadedNewLevelNetworkingSetupDone = true;
                    }
                    HHero.SendCoordinates();
                }
                Thread.Sleep(10);
            }
            */
        }

        private static void Loop()
        {
            IsRunning = true;
            while (IsRunning)
            {
                _client.Update();
                Thread.Sleep(10);
            }
        }

        private static void Connected()
        {
            Logger.Write("Password accepted. Connected to server successfully!");

        }

        private static void Disconnected()
        {
            BasicIoC.LoginViewModel.ConnectEnabled = true;
            IsRunning = false;
        }

        private static void ConnectionFailed()
        {
            BasicIoC.LoginViewModel.ConnectEnabled = true;
            SystemSounds.Hand.Play();
            MessageBox.Show("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
            IsRunning = false;
            return;
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            Logger.Write(message.GetString());
        }


        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
            Disconnected();
        }
    }
}
