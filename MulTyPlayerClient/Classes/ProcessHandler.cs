using MulTyPlayerClient.Classes;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace MulTyPlayerClient
{
    internal static class ProcessHandler
    {
        public static IntPtr HProcess;
        static HeroHandler HHero => Program.HHero;
        static KoalaHandler HKoala => Program.HKoala;
        static LevelHandler HLevel => Program.HLevel;
        static SyncHandler HSync => Program.HSync;
        static AttributeHandler HAttribute => Program.HAttribute;
        static CollectiblesHandler HCollectibles => Program.HCollectibles;
        static OpalHandler HOpal => Program.HOpal;
        static GameStateHandler HGameState => Program.HGameState;

        public static Process TyProcess;


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public static void OpenTyProcess()
        {
            HProcess = OpenProcess(0x1F0FFF, false, TyProcess.Id);
        }

        public static Process FindTyexe()
        {
            foreach (Process p in Process.GetProcesses("."))
            {
                if (p.MainWindowTitle == "TY the Tasmanian Tiger")
                {
                    TyProcess = p;
                    return p;
                }
            }
            TyProcess = null;
            return null;
        }

        public static void WriteData(int address, byte[] bytes)
        {
            int bytesWritten = 0;
            WriteProcessMemory((int)HProcess, address, bytes, bytes.Length, ref bytesWritten);
        }

        public static void GetTyData()
        {
            while (!Client.IsRunning)
            {

            }
            while (Client.IsRunning)
            {
                HGameState.CheckLoaded();
                if (!HGameState.CheckMenu() && !HGameState.LoadingState)
                {
                    HCollectibles.CheckCountsChanged();
                    HAttribute.CheckAttributeChange();
                    HLevel.GetCurrentLevel();
                    HOpal.CheckCount();
                    HHero.GetTyPosRot();
                    HKoala.SetCoordAddrs();
                }
                Thread.Sleep(10);
            }
        }
    }
}
