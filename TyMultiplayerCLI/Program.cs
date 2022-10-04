using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace TyMultiplayerCLI
{

/*
    CURRENT ISSUES
    - CODE FLOW (DON'T KNOW WHY APP WONT RESTART)
    - KOALAS DON'T GO BACK IF ANOTHER PLAYER DISCONNECTS OR CHANGED LEVEL
    - ENTER INCORRECT IP AND CODE KILLS ITSELF
    - NEEDS COMMENTS
*/

    internal class Program
    {
        public static TyPosRot tyPosRot;
        public static IntPtr tyexeHandle;
        public static KoalaPos koalaPos;
        public static Thread getPosThread;
        static private string inputStr;

        public static string playerName;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);


        static void Main(string[] args)
        {
            do
            {
                RunProgram();

                Console.WriteLine("Would you like to restart Mul-Ty-Player? [y/n]");
                inputStr = Console.ReadLine();

            } while (inputStr == "y");
        }

        static void RunProgram()
        {
            tyPosRot = new TyPosRot();
            koalaPos = new KoalaPos();

            Console.WriteLine("Welcome to Mul-Ty-player");

            bool messageShown = false;
            while (tyPosRot.FindTyexe() == null)
            {
                if (!messageShown)
                {
                    Console.WriteLine("Ty.exe could not be found. Load Ty.exe to continue");
                    messageShown = true;
                }
            }

            tyPosRot.SetMemoryAddresses();
            tyexeHandle = OpenProcess(0x0010, false, tyPosRot.tyProcess.Id);
            koalaPos.CreateKoalas();
            getPosThread = new Thread(new ThreadStart(GetTyPos));
            getPosThread.Start();

            Console.WriteLine("Ty.exe Found! Please enter your player name.");
            playerName = Console.ReadLine();
            while (playerName == "" || playerName == "null" || playerName == null || playerName.Length < 3)
            {
                Console.WriteLine("Please enter a valid name");
                playerName = Console.ReadLine();
            }
            Console.WriteLine("\nThank you! \nNow please enter the IP address of the server you wish to join");
            while (!Client.isRunning)
            {
                string ip = Console.ReadLine();
                Client.StartClient(ip);
            }
            Console.WriteLine("works up to here i guess?");
        }

        static void GetTyPos()
        {
            for (int i = 0; i < 2; i++)
            {
                if (Client.isRunning)
                {
                    tyPosRot.GetTyPos();
                    tyPosRot.GetCurrentLevel();
                    koalaPos.SetCoordAddrs();
                    if(tyPosRot.FindTyexe() == null)
                    {
                        Client.isRunning = false;
                        Console.WriteLine("Ty.exe was closed.");
                        getPosThread.Abort();
                    }
                }
                if (!Client.isRunning) { getPosThread.Abort(); }
                i--;
            }
        }
    }
}
