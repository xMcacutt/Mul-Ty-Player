using System;
using RiptideNetworking.Utils;
using System.Threading;
using RiptideNetworking;

namespace TyMultiplayerCLI
{

    public enum MessageID : ushort
    {
        connected,
        playerinfo,
        koalacoordinates
    }

    internal class Client
    {
        static TyPosRot tyPosRot => Program.tyPosRot;
        static KoalaPos koalaPos => Program.koalaPos;

        public static bool isRunning;
        private static RiptideNetworking.Client client;
        private static string ip;
        static Thread loop;

        public static void StartClient(string ipinput)
        {
            RiptideLogger.Initialize(Console.WriteLine, true);
 
            ip = ipinput;

            loop = new Thread(new ThreadStart(Loop));
            loop.Start();
            isRunning = true;

            Console.ReadLine();

            
            isRunning = false;
           
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
                if (isRunning) { SendCoordinates(); }
                Thread.Sleep(10);
            }

            client.Disconnect();
            Disconnected();

        }

        private static void Connected()
        {
            Console.WriteLine("\nConnected to server sucessfully!\nPress enter to disconnect at any time.");
            Message message = Message.Create(MessageSendMode.reliable, MessageID.connected);
            message.AddString(Program.playerName);
            client.Send(message);
        }

        private static void Disconnected()
        {
            Console.WriteLine("\nYou have been disconnected from the server.");
            Program.getPosThread.Abort();
            loop.Abort();
        }

        private static void SendCoordinates()
        {
            Message message = Message.Create(MessageSendMode.unreliable, MessageID.playerinfo);
            message.AddInt(tyPosRot.currentLevelID);
            message.AddFloats(tyPosRot.currentPosRot);
            client.Send(message);
        }

        [MessageHandler((ushort)MessageID.koalacoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            string name = message.GetString();
            int[] intData = message.GetInts();
            float[] coordinates = message.GetFloats();
            //intData[0] = Koala ID
            //intData[1] = Current Level For Given Player (Koala ID)

            if (name == Program.playerName) { return; }
            if (intData[1] != tyPosRot.currentLevelID)
            {
                return;
                //koalaPos.MagicTheKoalaAway(intData[0]); 
            }
            for(int i = 0; i < 4; i++)
            {
                koalaPos.WriteFloatToMemory(coordinates[i], koalaPos.koalaAddrs[intData[0]][i]);
            }

        }
    }
}
