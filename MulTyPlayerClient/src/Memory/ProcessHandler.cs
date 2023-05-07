using MulTyPlayerClient.GUI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Steamworks;
using MulTyPlayerCommon;

namespace MulTyPlayerClient.Memory
{
    internal class ProcessHandler
    {
        public static bool MemoryWriteDebugLogging = false;
        public static bool MemoryReadDebugLogging = false;

        [DllImport("kernel32.dll")]
        internal static unsafe extern bool ReadProcessMemory(
            nint hProcess,
            void* lpBaseAddress,
            void* lpBuffer,
            nuint nSize,
            nuint* lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        //Do not check if the process is running (for now),
        //throwing this exception allows up to exit the client loop
        //may restructure in future
        public static void WriteData(int address, byte[] bytes, string writeIndicator)
        {
            try
            {
                bool success = WriteProcessMemory(TyProcess.Handle, address, bytes, bytes.Length, out nint bytesWritten);
                if (MemoryWriteDebugLogging)
                {
                    string message = BitConverter.ToString(bytes) + " to 0x" + address.ToString("X") + " For: " + writeIndicator;
                    string logMsg = (success ? "Successfully wrote " : "Failed to write") + message;
                    ChatLog.Write(logMsg);
                }
            }
            catch (Exception ex)
            {
                ChatLog.Write($"Error writing data: {ex}");
                throw new TyProcessException("ProcessHandler.WriteData()", ex);
            }
        }

        //Do not check if the process is running (for now),
        //throwing this exception allows up to exit the client loop
        //may restructure in future
        public static unsafe bool TryRead<T>(nint address, out T result, bool addBase)
        where T : unmanaged
        {
            try
            {
                fixed (T* pResult = &result)
                {
                    //string s = GetCallStackAsString();
                    if (addBase) address = TyProcess.BaseAddress + address;
                    nuint nSize = (nuint)sizeof(T), nRead;
                    //BasicIoC.LoggerInstance.Write(address.ToString() + " " + s);
                    return ReadProcessMemory(TyProcess.Handle, (void*)address, pResult, nSize, &nRead)
                        && nRead == nSize;
                }
            }
            catch (Exception ex)
            {
                ChatLog.Write(ex.ToString());
                result = default;
                throw new TyProcessException("ProcessHandler.TryRead()", ex);
            }
        }

        //Do not check if the process is running (for now),
        //throwing this exception allows up to exit the client loop
        //may restructure in future
        public static unsafe bool TryReadBytes<T>(nint address, T into, int size, bool addBase)
        {

            try
            {
                //string s = GetCallStackAsString();
                if (addBase) address = TyProcess.BaseAddress + address;
                nuint nSize = (nuint)size, nRead;
                //BasicIoC.LoggerInstance.Write(address.ToString() + " " + s);
                return ReadProcessMemory(TyProcess.Handle, (void*)address, &into, nSize, &nRead)
                    && nRead == nSize;
            }
            catch (Exception ex)
            {
                ChatLog.Write(ex.ToString());
                throw new TyProcessException("ProcessHandler.TryRead()", ex);
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
    }
}
