using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Steamworks;

namespace TyMultiplayerCLI
{

    internal class Program
    {
        public static HeroHandler heroHandler;
        public static KoalaHandler koalaHandler;
        public static PointerCalculations ptrCalc;
        public static Thread tyDataThread;
        public static string playerName;
        private static string _inputStr;

        static void Main(string[] args)
        {
            do
            {
                Console.Clear();
                _inputStr = "n";
                RunProgram();
                Console.ReadLine();
                Console.WriteLine("Would you like to restart Mul-Ty-Player? [y/n]");
                _inputStr = Console.ReadLine();

            } while (_inputStr == "y");
        }

        private static void RunProgram()
        {

            ptrCalc = new PointerCalculations();
            heroHandler = new HeroHandler();
            koalaHandler = new KoalaHandler();

            Console.WriteLine("Welcome to Mul-Ty-player");

            var messageShown = false;
            while (ProcessHandler.FindTyexe() == null)
            {
                if (!messageShown)
                {
                    Console.WriteLine("Ty.exe could not be found. Load Ty.exe to continue");
                    messageShown = true;
                }
            }

            Console.WriteLine("Ty.exe Found!");

            string nameMessage = playerName == null
                ? "Attempting to get player name..."
                : "Player name already found. Setting up client...";

            Console.WriteLine(nameMessage);

            ProcessHandler.OpenTyProcess();

            heroHandler.SetMemoryAddresses();

            koalaHandler.CreateKoalas();

            tyDataThread = new Thread(new ThreadStart(ProcessHandler.GetTyData));
            tyDataThread.Start();

            
            if (SteamAPI.Init())
            {
                playerName = SteamFriends.GetPersonaName();
            }

            if (playerName == null)
            {
                Console.WriteLine("Steamworks API could not be initialized, please enter your name manually:");
                playerName = Console.ReadLine();
            }

            while (string.IsNullOrWhiteSpace(playerName) || playerName.Length < 3)
            {
                Console.WriteLine("Please enter a valid name");
                playerName = Console.ReadLine();
            }

            Console.WriteLine("\nNow please enter the IP address of the server you wish to join");
            var ip = Console.ReadLine();
            Client.StartClient(ip);

        }

        static void GetTyData()
        {
            for (int i = 0; i < 2; i++)
            {
                if (Client.isRunning)
                {
                    Console.WriteLine("gettingdata");
                    heroHandler.GetTyPos();
                    heroHandler.GetCurrentLevel();
            //        koalaHandler.SetCoordAddrs();
                    
                    if (ProcessHandler.FindTyexe() == null)
                    {
                        Client.isRunning = false;
                        Console.WriteLine("Ty.exe was closed.");
                        tyDataThread.Abort();
                    }
                }
                i--;
            }
        }
    }
}
