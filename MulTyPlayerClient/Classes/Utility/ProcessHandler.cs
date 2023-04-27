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
using System.IO;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace MulTyPlayerClient
{
    internal class ProcessHandler
    {
        public static IntPtr HProcess;
        public static Process TyProcess;
        public static bool HasTyProcess;
        private static nint TyProcessBaseAddress;

        public static bool MemoryWriteDebugLogging = false;
        public static bool MemoryReadDebugLogging = false;

        private static bool HasFolderPath = false;

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

        public static void Initialize()
        {
            HasFolderPath = SettingsHandler.Settings.MulTyPlayerFolderPath != "";
            if (HasFolderPath && SettingsHandler.Settings.AutoLaunchTyOnStartup)
            {
                if(FindTyProcess())
                    RestartTy();
            }
        }



        public static bool RestartTy()
        {
            TyProcess = new Process();
            TyProcess.StartInfo = new ProcessStartInfo(SettingsHandler.Settings.MulTyPlayerFolderPath) { UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true };
            TyProcess.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(SettingsHandler.Settings.MulTyPlayerFolderPath);
            TyProcess.Start();
            BasicIoC.LoggerInstance.Write("Ty has been restarted. You're back in!");
            BasicIoC.SFXPlayer.PlaySound(SFX.MenuAccept);
            return SetupProcess();
        }

        private static bool SetupProcess()
        {
            TyProcess.EnableRaisingEvents = true;
            TyProcess.Exited += (o, e) =>
            {
                var exitedProcess = o as Process;
                BasicIoC.LoggerInstance.Write("Ty has exited with code " + exitedProcess.ExitCode + ", restarting...");
                TyProcess.Close();
                TyProcess.Dispose();
                HasTyProcess = false;                
            };

            HProcess = OpenProcess(0x1F0FFF, false, TyProcess.Id);
            while (TyProcess.MainModule == null)
            {
            }
            TyProcessBaseAddress = TyProcess.MainModule.BaseAddress;
            if (!HasFolderPath)
            {
                SettingsHandler.Settings.MulTyPlayerFolderPath = TyProcess.MainModule.FileName;
                SettingsHandler.Save();
                HasFolderPath = true;
            }
            HasTyProcess = true;
            
            return true;
        }

        //Returns true if currently has a handle to the Ty process
        //Attempts to find the Ty process if not, returns true if successfully found
        //Returns false if Ty is closed
        public static bool FindTyProcess()
        {
            if (HasTyProcess)
                return true;            

            Process[] processes = Process.GetProcessesByName("Mul-Ty-Player");

            if (processes.Length == 0)
            {
                if(HasFolderPath && SettingsHandler.Settings.AutoRestartTyOnCrash)
                    return RestartTy();
                return false;
            }
            else if (processes.Length > 1)
            {
                //Multiple found
                BasicIoC.LoggerInstance.Write($"WARNING: Multiple ({processes.Length}) instances of Mul-Ty-Player are open, can and will cause fuckery.");
            }
            TyProcess = processes.First();
            SetupProcess();
            return true;
        }

        private static void TyProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            BasicIoC.LoggerInstance.Write($"Output data received: {e.Data}");
        }

        private static void TyProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            BasicIoC.LoggerInstance.Write($"Error data received: {e.Data}");
        }

        public static void WriteData(int address, byte[] bytes, string writeIndicator)
        {
            if (!HasTyProcess)
                return;
            try
            {
                bool success = WriteProcessMemory(HProcess, address, bytes, bytes.Length, out nint bytesWritten);
                if (MemoryWriteDebugLogging)
                {
                    string message = BitConverter.ToString(bytes) + " to 0x" + address.ToString("X") + " For: " + writeIndicator;
                    string logMsg = (success ? "Successfully wrote " : "Failed to write") + message;
                    BasicIoC.LoggerInstance.Write(logMsg);
                }
            }
            catch (Exception ex)
            {
                BasicIoC.LoggerInstance.Write($"Error writing data: {ex}");
                throw new TyProcessException("ProcessHandler.WriteData()", ex);
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
            catch(Exception ex)
            {
                BasicIoC.LoggerInstance.Write(ex.ToString());
                result = default;
                throw new TyProcessException("ProcessHandler.TryRead()", ex);
            }
        }

        
    }
}
