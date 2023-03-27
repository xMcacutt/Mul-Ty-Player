using MulTyPlayerClient;
using MulTyPlayerClient.GUI;
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MulTyPlayerClient
{
    internal static class ProcessHandler
    {
        public static IntPtr HProcess;
        public static Process TyProcess;
        public static bool MemoryWriteDebugLogging = false;
        public static bool MemoryReadDebugLogging = false;
        public static int i = 0;

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
            TyProcess = Process.GetProcessesByName("TY").FirstOrDefault();
            return TyProcess;
        }

        public static void WriteData(int address, byte[] bytes, string writeIndicator)
        {
            if (Client.Relaunching) return;
            IntPtr bytesWritten = IntPtr.Zero;
            try
            {
                bool success = WriteProcessMemory(HProcess, address, bytes, bytes.Length, out bytesWritten);
                if (!success)
                {
                    if (MemoryWriteDebugLogging)
                    {
                        string errorMsg = "Failed to write " + BitConverter.ToString(bytes) + " to 0x" + address.ToString("X") + " For: " + writeIndicator;
                        BasicIoC.LoggerInstance.Write(errorMsg);
                    }
                    if (FindTyProcess() == null)
                    {
                        throw new TyClosedException();
                    }
                }
                else
                {
                    if (MemoryWriteDebugLogging)
                    {
                        string errorMsg = "Written " + BitConverter.ToString(bytes) + " to 0x" + address.ToString("X") + " For: " + writeIndicator;
                        BasicIoC.LoggerInstance.Write(errorMsg);
                    }
                }
            }
            catch(TyClosedException ex)
            {
                throw;
            }
        }

        public static byte[] ReadData(int address, int length, string readIndicator)
        {
            byte[] buffer = new byte[length];
            if (Client.Relaunching) return buffer;
            IntPtr bytesRead = IntPtr.Zero;
            try
            {
                bool success = ReadProcessMemory(HProcess, address, buffer, length, out bytesRead);
                if (!success)
                {
                    if (MemoryReadDebugLogging)
                    {
                        string errorMsg = "Failed to read data at 0x" + address.ToString("X") + " For: " + readIndicator;
                        BasicIoC.LoggerInstance.Write(errorMsg);
                    }
                    if (FindTyProcess() == null)
                    {
                        throw new TyClosedException();
                    }
                }
                else
                {
                    if (MemoryReadDebugLogging)
                    {
                        string errorMsg = "Read " + BitConverter.ToString(buffer) + " from address 0x" + address.ToString("X") + " For: " + readIndicator;
                        BasicIoC.LoggerInstance.Write(errorMsg);
                    }
                }
            }
            catch(TyClosedException ex)
            {
                throw;
            }
            return buffer;
        }
    }
}
