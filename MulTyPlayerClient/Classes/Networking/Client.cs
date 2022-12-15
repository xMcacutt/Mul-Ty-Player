using Riptide;
using Riptide.Utils;
using System;
using System.Net.Cache;
using System.Threading;

namespace MulTyPlayerClient
{
    internal class Client
    {
        static HeroHandler HHero => Program.HHero;
        static LevelHandler HLevel => Program.HLevel;
        static GameStateHandler HGameState => Program.HGameState;

        public static bool IsRunning;
        public static Riptide.Client _client;
        private static string _ip;
        private static string _command;

        public static void StartClient(string ipinput)
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
            _ip = ipinput;

            IsRunning = true;

            _client = new Riptide.Client();
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected();
            _client.ConnectionFailed += (s, e) => ConnectionFailed();
            _client.Connect(_ip + ":8750");

            Thread commandThread = new(new ParameterizedThreadStart(CommandThread));
            commandThread.Start(Program._cts.Token);

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
        }

        private static void CommandThread(Object token)
        {
            CommandHandler commandHandler = new();
            _command = "/doNothing";
            while (_command != "/stop")
            {
                //Console.WriteLine(CommandHandler.host);
                if (CommandHandler.host != 0 && _command != "/doNothing")
                {
                    commandHandler.ParseCommand(_command);
                }
                _command = Console.ReadLine();
            }

            if (IsRunning)
            {
                IsRunning = false;
                Console.WriteLine("\nYou have been disconnected from the server.");
                _client.Disconnect();
            }
        }

        private static void Connected()
        {
            Console.WriteLine("\nConnected to server successfully!\nType /stop to disconnect at any time.");
            SettingsHandler.RequestServerSettings();
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.Connected);
            message.AddString(Program.PlayerName);
            _client.Send(message);
        }

        private static void Disconnected()
        {
            Program._cts.Cancel();
            Program._cts.Dispose();
        }

        private static void ConnectionFailed()
        {
            Console.WriteLine("Could not connect to server...");
            _command = "/stop";
            IsRunning = false;
            Disconnected();
            return;
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            Console.WriteLine(message.GetString());
        }


        [MessageHandler((ushort)MessageID.Disconnect)]
        public static void GetDisconnectedScrub(Message message)
        {
            _client.Disconnect();
            Disconnected();
        }
    }
}
