using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TyMultiplayerCLI
{
    internal class ProcessHandler
    {
        public static IntPtr hProcess;
        static HeroHandler heroHandler => Program.heroHandler;
        static KoalaHandler koalaHandler => Program.koalaHandler;

        public static Process tyProcess;


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public static void OpenTyProcess()
        {
            hProcess = OpenProcess(0x1F0FFF, false, tyProcess.Id);
        }

        public Process FindTyexe()
        {
            foreach (Process p in Process.GetProcesses("."))
            {
                if (p.MainWindowTitle == "TY the Tasmanian Tiger")
                {
                    tyProcess = p;
                    return p;
                }
            }
            tyProcess = null;
            return null;
        }
    }
}
