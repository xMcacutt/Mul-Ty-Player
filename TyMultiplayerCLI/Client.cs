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
        static HeroHandler HeroHandler => Program.heroHandler;
        static KoalaHandler KoalaHandler => Program.koalaHandler;

        public static bool IsRunning;
        public static bool DidRun;
        private static RiptideNetworking.Client _client;
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
                    SendCoordinates();
                }
                Thread.Sleep(10);
            }

        }

        private static void Connected()
        {
            DidRun = true;
            Console.WriteLine("\nConnected to server successfully!\nPress enter to disconnect at any time.");
            Message message = Message.Create(MessageSendMode.reliable, MessageID.Connected);
            message.AddString(Program.playerName);
            _client.Send(message);
        }

        private static void Disconnected()
        {
            Program.tyDataThread.Abort();
            _loop.Abort();
        }

        static void SendCoordinates()
        {
            Message message = Message.Create(MessageSendMode.unreliable, MessageID.PlayerInfo);
            message.AddBool(HeroHandler.CheckLoading());
            message.AddInt(HeroHandler.CurrentLevelId);
            message.AddFloats(HeroHandler.CurrentPosRot);
            _client.Send(message);
        }

        [MessageHandler((ushort)MessageID.ConsoleSend)]
        public static void ConsoleSend(Message message)
        {
            Console.WriteLine(message.GetString());
        }


        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            int[] intData = message.GetInts();
            float[] coordinates = message.GetFloats();
            string name = message.GetString();

            //intData[0] = Koala ID
            //intData[1] = Current Level For Given Player (Koala ID)

            if (name == Program.playerName || HeroHandler.CheckLoading()) { return; }

            if (intData[1] != HeroHandler.CurrentLevelId)
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                IntPtr hProcess = ProcessHandler.OpenProcess(0x1F0FFF, false, ProcessHandler.TyProcess.Id);
                int bytesWritten = 0;
                byte[] buffer = BitConverter.GetBytes(coordinates[i]);
                ProcessHandler.WriteProcessMemory((int)hProcess, KoalaHandler.koalaAddrs[intData[0]][i], buffer, buffer.Length, ref bytesWritten);
            }
        }
    }
}
