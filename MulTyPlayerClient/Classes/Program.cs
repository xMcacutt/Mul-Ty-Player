using MulTyPlayerClient;
using Steamworks;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace MulTyPlayerClient
{
    internal static class Program
    {
        public static HeroHandler HHero;
        public static KoalaHandler HKoala;
        public static LevelHandler HLevel;
        public static GameStateHandler HGameState;
        public static SyncHandler HSync;

        public static CancellationTokenSource _cts;
        public static string PlayerName;
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
                Console.WriteLine("Would you like to restart Mul-Ty-Player? [y/n]");
                _inputStr = Console.ReadLine();
            } while (_inputStr == "y");
        }

        private static void RunProgram()
        {
            SettingsHandler.Setup();

            Console.WriteLine("Welcome to Mul-Ty-player 1.3.4");

            //TRIES TO FIND TY AND GIVES WARNING MESSAGE TO OPEN TY ON FAILURE
            var messageShown = false;
            while (ProcessHandler.FindTyProcess() == null)
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

            HLevel = new LevelHandler();
            HSync = new SyncHandler();
            HGameState = new GameStateHandler();
            HHero = new HeroHandler();
            HKoala = new KoalaHandler();

            HHero.SetCoordAddrs();
            HKoala.CreateKoalas();

            //STARTS THE THREAD THAT CONTINUOUSLY READS DATA FROM THE GAME
            _cts = new CancellationTokenSource();
            Thread TyDataThread = new Thread(new ParameterizedThreadStart(HGameState.GetTyData));
            TyDataThread.Start(_cts.Token);

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
                SteamClient.Init(411960);
                PlayerName = SteamClient.Name;
                SteamClient.Shutdown();
                if (string.IsNullOrWhiteSpace(PlayerName) && !string.IsNullOrWhiteSpace(SettingsHandler.DefaultName))
                {
                    PlayerName = SettingsHandler.DefaultName;
                }
            }
            else if (!string.IsNullOrWhiteSpace(SettingsHandler.DefaultName))
            {
                Console.WriteLine("Player name already found. Setting up client...");
                PlayerName = SettingsHandler.DefaultName;
            }

            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                Console.WriteLine("SteamWorks was unable to get your name and no name is defined in the settings file. \nPlease enter your name manually:");
                PlayerName = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(PlayerName) || PlayerName.Length < 3)
            {
                Console.WriteLine("Please enter a valid name");
                PlayerName = Console.ReadLine();
            }
            Console.WriteLine($"Your name is {PlayerName} If you'd like to use a different name, specify it in ClientSettings.mtps");
            //GET DEFAULTL IP ADDRESS
            string ipStr = string.Empty;
            if (string.IsNullOrWhiteSpace(SettingsHandler.DefaultAddress))
            {
                Console.WriteLine("\nNo default address specified in settings file");
            }
            else
            {
                ipStr = SettingsHandler.DefaultAddress;
            }
            while (!IPAddress.TryParse(ipStr, out IPAddress ip) && ipStr != "localhost")
            {
                Console.WriteLine("Please enter a VALID IP address to connect to...");
                ipStr = Console.ReadLine();
            }
            Client.StartClient(ipStr);
        }
    }
}
