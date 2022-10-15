using System;
using System.Net;
using RiptideNetworking.Utils;
using System.Threading;
using RiptideNetworking;

namespace MulTyPlayerClient
{
    public enum MessageID : ushort
    {
        Connected,
        PlayerInfo,
        KoalaCoordinates,
        ConsoleSend,
        ServerDataUpdate,
        ClientLevelDataUpdate,
        ClientAttributeDataUpdate
    }

    internal static class Client
    {
        static HeroHandler HeroHandler => Program.HeroHandler;
        static KoalaHandler KoalaHandler => Program.KoalaHandler;
        public static bool IsRunning;
        public static bool DidRun;
        public static RiptideNetworking.Client _client;
        private static string _ip;
        static Thread _loop;

        public static void StartClient(string ipinput)
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
            _ip = ipinput;
            DidRun = false;

            _loop = new Thread(new ThreadStart(Loop));
            _loop.Start();
            IsRunning = true;

            Console.ReadLine();

            if (!DidRun)
            {
                Console.WriteLine("Could not connect to server. Press return.");
                Disconnected();
                return;
            }
            if (IsRunning)
            {
                IsRunning = false;
                _client.Disconnect();
                Console.WriteLine("\nYou have been disconnected from the server. Press return.");
                Disconnected();
            }

        }

        private static void Loop()
        {
            _client = new RiptideNetworking.Client(5000);
            _client.Connected += (s, e) => Connected();
            _client.Disconnected += (s, e) => Disconnected();
            _client.Connect(_ip + ":8750");

            while (IsRunning)
            {
                _client.Tick();
                if (!HeroHandler.CheckMenu() && !HeroHandler.CheckLoading())
                {
                    KoalaHandler.SetCoordAddrs();
                    if (!SettingsHandler.DoKoalaCollision)
                    {
                        KoalaHandler.RemoveCollision();
                    }
                    HeroHandler.SendCoordinates();
                }
                Thread.Sleep(10);
            }

        }

        private static void Connected()
        {
            DidRun = true;
            Console.WriteLine("\nConnected to server successfully!\nPress enter to disconnect at any time.");
            Message message = Message.Create(MessageSendMode.reliable, MessageID.Connected);
            message.AddString(Program.PlayerName);
            _client.Send(message);
        }

        private static void Disconnected()
        {
            Program.TyDataThread.Abort();
            _loop.Abort();
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            Console.WriteLine(message.GetString());
        }



    }
}
