﻿using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.Net.Cache;
using System.Threading;

namespace MulTyPlayerClient
{
    internal class Client
    {
        static HeroHandler HHero => Program.HHero;
        static LevelHandler HLevel => Program.HLevel;
        static SyncHandler HSync => Program.HSync;
        static GameStateHandler HGameState => Program.HGameState;

        public static bool IsRunning;
        public static RiptideNetworking.Client _client;
        private static string _ip;
        private static string _command;
        static Thread _loop;

        public static void StartClient(string ipinput)
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
            _ip = ipinput;


            _loop = new Thread(new ThreadStart(Loop));
            _loop.Start();

            IsRunning = true;

            CommandHandler commandHandler = new CommandHandler();

            _command = "/doNothing";
            while (_command != "/stop")
            {
                //Console.WriteLine(CommandHandler.host);
                if(CommandHandler.host != 0)
                {
                    commandHandler.ParseCommand(_command);
                    _command = Console.ReadLine();
                }
            }

            if (IsRunning)
            {
                IsRunning = false;
                _client.Disconnect();
                Console.WriteLine("\nYou have been disconnected from the server.");
                Disconnected();
            }

        }

        private static void Loop()
        {
            _client = new RiptideNetworking.Client(5000);
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected();
            _client.ConnectionFailed += (s, e) => ConnectionFailed();
            _client.Connect(_ip + ":8750");

            while (IsRunning)
            {
                _client.Tick();
                //CHECK IF ON MENU OR LOADING
                if (!HGameState.CheckMenu() && !HGameState.CheckLoading())
                {
                    //IF NOT SET UP LOAD INTO LEVEL STUFF
                    if (!HLevel.LoadedIntoNewLevelStuffDone)
                    {
                        HLevel.DoLevelSetup();
                    }
                    HHero.SendCoordinates();
                }

                Thread.Sleep(10);
            }
        }

        private static void Connected()
        {
            Console.WriteLine("\nConnected to server successfully!\nType /stop to disconnect at any time.");
            SettingsHandler.RequestServerSettings();
            Message message = Message.Create(MessageSendMode.reliable, MessageID.Connected);
            message.AddString(Program.PlayerName);
            _client.Send(message);
        }

        private static void Disconnected()
        {
            Program.TyDataThread.Abort();
            _loop.Abort();
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
