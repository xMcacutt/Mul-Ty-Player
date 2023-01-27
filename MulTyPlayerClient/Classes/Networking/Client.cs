using MulTyPlayerClient.Classes.ConsoleLog;
using Riptide;
using Riptide.Utils;
using System;
using System.Net.Cache;
using System.Threading;

namespace MulTyPlayerClient
{
    internal class Client
    {
        public static bool IsRunning;
        public static Riptide.Client _client;
        private static string _ip;
        private static string _pass;

        public static void StartClient(string ipinput)
        {
            Logger logger = new(100);
            RiptideLogger.Initialize(Logger.Write, true);
            _ip = ipinput;

            IsRunning = true;

            _client = new Riptide.Client();
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected();
            _client.ConnectionFailed += (s, e) => ConnectionFailed();

            Message authentication = Message.Create(MessageSendMode.Reliable, MessageID.Authentication);
            authentication.AddString(Program.PlayerName);
            authentication.AddString(_pass);
            _client.Connect(_ip + ":8750", 5, 0, authentication);

            /* NEEDS TO BE HANDLED ON A BACKGROUND WORKER
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

        private static void Connected()
        {
            Logger.Write("Password accepted. Connected to server successfully!");
        }

        private static void Disconnected()
        {
        }

        private static void ConnectionFailed()
        {
            //"Could not connect to server. Please try again."
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
