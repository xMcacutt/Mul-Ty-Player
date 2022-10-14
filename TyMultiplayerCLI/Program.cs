using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Steamworks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace MulTyPlayerClient
{

    internal static class Program
    {
        public static HeroHandler heroHandler;
        public static KoalaHandler koalaHandler;
        public static PointerCalculations ptrCalc;
        public static Thread tyDataThread;
        public static string playerName;
        private static string _inputStr;
        public static string PosLogPath;

        static void Main(string[] args)
        {
            //RESTARTS THE APP WHEN IT REACHES END (DISCONNECT)
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
            SettingsHandler.Setup();
            ptrCalc = new PointerCalculations();
            heroHandler = new HeroHandler();
            koalaHandler = new KoalaHandler();

            Console.WriteLine("Welcome to Mul-Ty-player");

            //TRIES TO FIND TY AND GIVES WARNING MESSAGE TO OPEN TY ON FAILURE
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
            
            //OPEN HANDLE TO PROCESS
            ProcessHandler.OpenTyProcess();

            heroHandler.SetMemoryAddresses();
            koalaHandler.CreateKoalas();

            //STARTS THE THREAD THAT CONTINUOUSLY READS DATA FROM THE GAME
            tyDataThread = new Thread(new ThreadStart(ProcessHandler.GetTyData));
            tyDataThread.Start();


            //MAKES FILE FOR POSITION LOGGING
            if (SettingsHandler.DoPositionLogging)
            {
                PosLogPath = SettingsHandler.PositionLoggingOutputDir + DateTime.Now;
              //  File.Create(posLogPath);
             //   Console.WriteLine("File created for position logging");
            }

            //ATTEMPTS TO GET STEAM NAME OR DEFAULT NAME FROM SETTINGS FILE
            if (SettingsHandler.DoGetSteamName)
            {
                Console.WriteLine("Attempting to get player name from steam...");
                if (SteamAPI.Init())
                {
                    playerName = SteamFriends.GetPersonaName();
                }
            }
            else if(!string.IsNullOrWhiteSpace(SettingsHandler.DefaultName))
            {
                Console.WriteLine("Player name already found. Setting up client...");
                playerName = SettingsHandler.DefaultName;
            }
            if (playerName == null)
            {
                Console.WriteLine("Steamworks was unable to get your name and no name is defined in the settings file. \nPlease enter your name manually:");
                playerName = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(playerName) || playerName.Length < 3)
            {
                Console.WriteLine("Please enter a valid name");
                playerName = Console.ReadLine();
            }

            //
            string ipStr = "";
            if (string.IsNullOrWhiteSpace(SettingsHandler.DefaultAddress))
            {
                Console.WriteLine("\nNo default address specified in settings file");
            }
            else
            {
                ipStr = SettingsHandler.DefaultAddress;
            }
            while (!IPAddress.TryParse(ipStr, out IPAddress ip))
            {
                Console.WriteLine("Please enter a VALID IP address to connect to...");
                ipStr = Console.ReadLine();
            }
            Client.StartClient(ipStr);

        }
    }
}
