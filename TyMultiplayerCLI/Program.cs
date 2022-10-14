﻿using System;
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
        public static HeroHandler HeroHandler;
        public static KoalaHandler KoalaHandler;
        public static Thread TyDataThread;
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
                Console.ReadLine();
                Console.WriteLine("Would you like to restart Mul-Ty-Player? [y/n]");
                _inputStr = Console.ReadLine();

            } while (_inputStr == "y");
        }

        private static void RunProgram()
        {
            SettingsHandler.Setup();
            HeroHandler = new HeroHandler();
            KoalaHandler = new KoalaHandler();

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

            HeroHandler.SetMemoryAddresses();
            KoalaHandler.CreateKoalas();

            //STARTS THE THREAD THAT CONTINUOUSLY READS DATA FROM THE GAME
            TyDataThread = new Thread(new ThreadStart(ProcessHandler.GetTyData));
            TyDataThread.Start();


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
                    PlayerName = SteamFriends.GetPersonaName();
                }
            }
            else if(!string.IsNullOrWhiteSpace(SettingsHandler.DefaultName))
            {
                Console.WriteLine("Player name already found. Setting up client...");
                PlayerName = SettingsHandler.DefaultName;
            }
            if (PlayerName == null)
            {
                Console.WriteLine("Steamworks was unable to get your name and no name is defined in the settings file. \nPlease enter your name manually:");
                PlayerName = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(PlayerName) || PlayerName.Length < 3)
            {
                Console.WriteLine("Please enter a valid name");
                PlayerName = Console.ReadLine();
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
