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
using System.Collections;

namespace MulTyPlayerClient
{
    internal class ProcessHandler
    {
        public static IntPtr HProcess;
        public static Process TyProcess;
        private static nint TyProcessBaseAddress;
        private static bool HasProcess;

        public static bool MemoryWriteDebugLogging = false;
        public static bool MemoryReadDebugLogging = false;
        public static int i = 0;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        internal static unsafe extern bool ReadProcessMemory(
            nint hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nuint nSize,
            nuint* lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        //Attempts to find the Ty process, returns true if successfully found
        //Automatically opens handle if found
        public static bool FindTyProcess(bool overrideOld = true)
        {
            if (!overrideOld && HasProcess)
                return true;

            Process[] processes = Process.GetProcessesByName("Mul-Ty-Player");

            if (processes.Length == 0)
            {
                //No process found
                return false;
            }
            else if (processes.Length > 1)
            {
                //Multiple found
                BasicIoC.LoggerInstance.Write($"WARNING: Multiple ({processes.Length}) instances of Mul-Ty-Player are open, can and will cause fuckery.");
            }
            TyProcess = processes.First();
            TyProcess.Refresh();
            TyProcess.EnableRaisingEvents = true;

            TyProcess.Exited += (o, e) =>
            {
                TyProcess.Close();
                TyProcess.Dispose();
                HasProcess = false;
            };

            HProcess = OpenProcess(0x1F0FFF, false, TyProcess.Id);
            TyProcessBaseAddress = TyProcess.MainModule.BaseAddress;
            HasProcess = true;
            return true;
        }

        public static void WriteData(int address, byte[] bytes, string writeIndicator)
        {
            try
            {
                bool success = WriteProcessMemory(HProcess, address, bytes, bytes.Length, out nint bytesWritten);
                if (MemoryWriteDebugLogging)
                {
                    string message = BitConverter.ToString(bytes) + " to 0x" + address.ToString("X") + " For: " + writeIndicator;
                    string logMsg = (success ? "Successfully wrote " : "Failed to write") + message;
                    BasicIoC.LoggerInstance.Write(logMsg);
                }
                if (!success && !HasProcess)
                {
                    throw new TyClosedException();
                }
            }
            catch(TyClosedException ex)
            {
                throw ex;
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
                if (!HasProcess)
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
                    if(addBase) address = TyProcessBaseAddress + address;
                    nuint nSize = (nuint)sizeof(T), nRead;
                    //BasicIoC.LoggerInstance.Write(address.ToString() + " " + s);
                    return ReadProcessMemory(HProcess, (void*)address, pResult, nSize, &nRead)
                        && nRead == nSize;
                }
            }
            catch(TyClosedException ex)
            {
                throw ex;
            }
            catch(Win32Exception ex)
            {
                StackTrace stackTrace = new();
                if (stackTrace.GetFrames()
                    .Select(frame => frame.GetMethod())
                    .Any(method => method.Name == "ClientLoop"))
                {
                    BasicIoC.LoggerInstance.Write(ex.ToString());
                }
                result = default;
                return false;
            }
        }
    }
}
