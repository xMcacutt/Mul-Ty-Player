using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace MulTyPlayerClient
{
    internal static class ProcessHandler
    {
        public static IntPtr HProcess;
        static HeroHandler HeroHandler => Program.HeroHandler;
        static KoalaHandler KoalaHandler => Program.KoalaHandler;

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

        public static void GetTyData()
        {
            while (!Client.IsRunning)
            {
                
            }
            while (Client.IsRunning)
            {
                if (!HeroHandler.CheckMenu() && !HeroHandler.CheckLoading())
                {
                    HeroHandler.GetCurrentLevel();
                    HeroHandler.GetTyPos();
                    KoalaHandler.SetCoordAddrs();
                }
                Thread.Sleep(10);
            }
        }
    }
}
