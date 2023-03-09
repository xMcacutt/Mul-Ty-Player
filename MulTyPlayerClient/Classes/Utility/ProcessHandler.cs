using MulTyPlayerClient;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace MulTyPlayerClient
{
    internal static class ProcessHandler
    {
        public static IntPtr HProcess;
        public static Process TyProcess;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        public static void OpenTyHandle()
        {
            HProcess = OpenProcess(0x1F0FFF, false, TyProcess.Id);
        }

        public static Process FindTyProcess()
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

        public static bool CheckTyProcess()
        {
            return TyProcess.HasExited;
        }

        public static void WriteData(int address, byte[] bytes)
        {
            int bytesWritten = 0;
            WriteProcessMemory(checked((int)HProcess), address, bytes, bytes.Length, ref bytesWritten);
        }
    }
}
