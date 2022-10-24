using System;
using System.Threading;
using System.Collections.Generic;

namespace MulTyPlayerServer
{
    internal class Program
    {
        public static KoalaHandler HKoala;
        public static CollectiblesHandler CollectiblesHandler;
        public static CommandHandler CommandHandler;
        public static string _inputStr;

        private static void Main()
        {
            //RESTARTS THE APP WHEN IT REACHES END (DISCONNECT)
            do
            {
                Console.Clear();
                _inputStr = "n";
                RunProgram();
                if(_inputStr != "y")
                {
                    _inputStr = Console.ReadLine();
                }
            } while (_inputStr == "y");
        }

        private static void RunProgram()
        {
            Console.Title = "Mul-Ty-Player Server";

            SettingsHandler.Setup();

            Server.StartServer();

            HKoala = new KoalaHandler();
            CollectiblesHandler = new CollectiblesHandler();
            CommandHandler = new CommandHandler();
            Console.WriteLine("Welcome to Mul-Ty-Player.\nThis is the server application. \nPort forward on port 8750 to allow connections.\n");

            string command = Console.ReadLine();
            while (command != "/stop")
            {
                Console.WriteLine(CommandHandler.ParseCommand(command));
                if (command != "/restart") { command = Console.ReadLine(); }
                else { command = "/stop"; }
            }
            Server._isRunning = false;
        }
    }
}
