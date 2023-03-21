using MulTyPlayerClient;
using MulTyPlayerClient.GUI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal static class ProcessHandler
    {
        public static IntPtr HProcess;
        public static Process TyProcess;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        public static void OpenTyHandle()
        {
            HProcess = OpenProcess(0x1F0FFF, false, TyProcess.Id);
        }

        public static Process FindTyProcess()
        {
            Process[] processes = Process.GetProcessesByName("TY");
            if (processes.Length > 0)
            {
                TyProcess = processes[0];
                return TyProcess;
            }
            TyProcess = null;
            return null;
        }

        public static bool CheckTyProcess()
        {
            return TyProcess.HasExited;
        }

        public static async Task WriteDataAsync(int address, byte[] bytes)
        {
            IntPtr bytesWritten = IntPtr.Zero;
            try
            {
                await Task.Run(() =>
                {
                    bool success = WriteProcessMemory(HProcess, address, bytes, bytes.Length, out bytesWritten);
                    if (!success)
                    {
                        //throw new Exception("Ty the Tasmanian Tiger has crashed or stopped responding.");
                    }
                });
            }
            catch (Exception ex)
            {
                // Handle the exception here
                BasicIoC.LoggerInstance.Write(ex.Message);
            }
        }

        public static async Task<byte[]> ReadDataAsync(int address, int length)
        {
            byte[] buffer = new byte[length];
            IntPtr bytesRead = IntPtr.Zero;
            try
            {
                await Task.Run(() =>
                {
                    bool success = ReadProcessMemory(HProcess, address, buffer, length, out bytesRead);
                    if (!success)
                    {
                        //throw new Exception("Ty the Tasmanian Tiger has crashed or stopped responding.");
                    }
                });
            }
            catch (Exception ex)
            {
                // Handle the exception here
                BasicIoC.LoggerInstance.Write(ex.Message);
            }
            return buffer;
        }
    }
}
