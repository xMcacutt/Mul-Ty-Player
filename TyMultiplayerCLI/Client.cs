using System;
using System.Net;
using RiptideNetworking.Utils;
using System.Threading;
using RiptideNetworking;

namespace TyMultiplayerCLI
{

    public enum MessageID : ushort
    {
        connected,
        playerinfo,
        koalacoordinates,
        consolesend
    }

    internal class Client
    {
        static HeroHandler heroHandler => Program.heroHandler;
        static KoalaHandler koalaHandler => Program.koalaHandler;

        public static bool isRunning;
        private static RiptideNetworking.Client client;
        private static string ip;
        static Thread loop;
        public static bool didRun;

        public static void StartClient(string ipinput)
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
            ip = ipinput;
            didRun = false;

            loop = new Thread(new ThreadStart(Loop));
            loop.Start();
            isRunning = true;

            Console.ReadLine();

            if (!didRun)
            {
                Console.WriteLine("Could not connect to server. Press return.");
                Disconnected();
                return;
            }
            if (isRunning)
            {
                isRunning = false;
                client.Disconnect();
                Console.WriteLine("\nYou have been disconnected from the server. Press return.");
                Disconnected();
            }

        }

        private static void Loop()
        {
            client = new RiptideNetworking.Client(5000);
            client.Connected += (s, e) => Connected();
            client.Disconnected += (s, e) => Disconnected();
            client.Connect(ip + ":8750");

            while (isRunning)
            {
                client.Tick();
                if (!heroHandler.CheckMenu() && !heroHandler.CheckLoading())
                {
                    koalaHandler.SetCoordAddrs();
                    if (koalaHandler.koalaAddrs[0][4] != null && !SettingsHandler._DoKoalaCollision) ;
                    {
                        koalaHandler.RemoveCollision();
                    }
                    SendCoordinates();
                }
                Thread.Sleep(10);
            }

        }

        private static void Connected()
        {
            didRun = true;
            Console.WriteLine("\nConnected to server successfully!\nPress enter to disconnect at any time.");
            Message message = Message.Create(MessageSendMode.reliable, MessageID.connected);
            message.AddString(Program.playerName);
            client.Send(message);
        }

        private static void Disconnected()
        {
            Program.tyDataThread.Abort();
            loop.Abort();
        }

        static void SendCoordinates()
        {
            Message message = Message.Create(MessageSendMode.unreliable, MessageID.playerinfo);
            message.AddBool(heroHandler.CheckLoading());
            message.AddInt(heroHandler.currentLevelID);
            message.AddFloats(heroHandler.currentPosRot);
            client.Send(message);
        }

        [MessageHandler((ushort)MessageID.consolesend)]
        public static void ConsoleSend(Message message)
        {
            Console.WriteLine(message.GetString());
        }


        [MessageHandler((ushort)MessageID.koalacoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            int[] intData = message.GetInts();
            float[] coordinates = message.GetFloats();
            string name = message.GetString();

            //intData[0] = Koala ID
            //intData[1] = Current Level For Given Player (Koala ID)

            if (name == Program.playerName || heroHandler.CheckLoading()) { return; }

            if (intData[1] != heroHandler.currentLevelID)
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                IntPtr hProcess = ProcessHandler.OpenProcess(0x1F0FFF, false, ProcessHandler.tyProcess.Id);
                int bytesWritten = 0;
                byte[] buffer = BitConverter.GetBytes(coordinates[i]);
                ProcessHandler.WriteProcessMemory((int)hProcess, koalaHandler.koalaAddrs[intData[0]][i], buffer, buffer.Length, ref bytesWritten);
            }
        }
    }
}
