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
using System.Reflection.Metadata;
using System.CodeDom;

namespace MulTyPlayerClient
{
    internal unsafe class ProcessHandler
    {
        public static IntPtr HProcess;
        public static Process TyProcess;
        public static bool MemoryWriteDebugLogging = false;
        public static bool MemoryReadDebugLogging = false;
        public static int i = 0;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        internal static extern bool ReadProcessMemory(
            nint hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nuint nSize,
            nuint* lpNumberOfBytesRead);

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
            try
            {
                bool success = WriteProcessMemory(HProcess, address, bytes, bytes.Length, out nint bytesWritten);
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

        private static string GetCallStackAsString()
        {
            StackTrace stackTrace = new();

            // Get the frames in the call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();

            // Format the call stack as a string
            string callStack = "";
            foreach (StackFrame frame in stackFrames)
            {
                if (frame.GetMethod() != null)
                {
                    callStack += frame.GetMethod().Name + " -> ";
                }
            }
            // Remove the last "-> " from the callStack string
            if (callStack.EndsWith(" -> "))
            {
                callStack = callStack[..^4];
            }
            return callStack;
        }

        public static unsafe bool TryRead<T>(nint address, out T result, bool addBase)
        where T : unmanaged
        {
            try
            {
                if (TyProcess == null)
                {
                    StackTrace stackTrace = new();
                    if(stackTrace.GetFrames()
                        .Select(frame => frame.GetMethod())
                        .Any(method => method.Name == "ClientLoop"))
                    {
                        throw new TyClosedException();
                    }
                }
                fixed (T* pResult = &result)
                {
                    //string s = GetCallStackAsString();
                    if(addBase) address = TyProcess.MainModule.BaseAddress + address;
                    nuint nSize = (nuint)sizeof(T), nRead;
                    //BasicIoC.LoggerInstance.Write(address.ToString() + " " + s);
                    return ReadProcessMemory(HProcess, (void*)address, pResult, nSize, &nRead)
                        && nRead == nSize;
                }
            }
            catch(TyClosedException ex)
            {
                throw new TyClosedException();
            }
            catch(Win32Exception ex)
            {
                StackTrace stackTrace = new();
                if (stackTrace.GetFrames()
                    .Select(frame => frame.GetMethod())
                    .Any(method => method.Name == "ClientLoop"))
                {
                    throw new TyClosedException();
                }
                result = default;
                return false;
            }
        }
    }
}
