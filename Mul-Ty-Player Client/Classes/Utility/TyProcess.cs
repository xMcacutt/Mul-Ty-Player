using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using MulTyPlayerClient.Classes.Utility;

namespace MulTyPlayerClient;

internal class TyProcess
{
    private static Process process;
    private static bool handleOpen;

    public static bool IsRunning { get; private set; }
    public static bool LaunchingGame { get; private set; }
    public static bool CanLaunchGame => !IsRunning && !LaunchingGame;

    public static nint BaseAddress { get; private set; }

    public static IntPtr Handle { get; private set; }

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr handle);

    public static event Action OnTyProcessExited = delegate { };
    public static event Action OnTyProcessFound = delegate { };
    public static event Action OnTyProcessLaunched = delegate { };
    public static event Action OnTyProcessLaunchFailed = delegate { };

    //Launches the game from the path saved in settings.
    //Returns whether or not game was succesfully launched.
    public static bool TryLaunchGame()
    {
        //If game is already running, or being launched, fail
        if (!CanLaunchGame)
            return false;
        //If we dont have a valid path to the exe, fail
        if (!SettingsHandler.HasValidExePath() || !SteamHelper.IsLoggedOn())
            return false;

        LaunchingGame = true;
        try
        {
            process = new Process();
            process.StartInfo = new ProcessStartInfo(SettingsHandler.Settings.MulTyPlayerFolderPath, "-noidle")
                { UseShellExecute = false, RedirectStandardError = true, RedirectStandardOutput = true };
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(SettingsHandler.Settings.MulTyPlayerFolderPath);
            process.Start();
            PullProcessData();
            OnTyProcessLaunched?.Invoke();
            LaunchingGame = false;
            return true;
        }
        catch
        {
            OnTyProcessLaunchFailed?.Invoke();
            LaunchingGame = false;
            return false;
        }
    }

    public static void CloseProcess()
    {
        try
        {
            process?.CloseMainWindow();
        }
        catch
        {
            
        }
    }

    //Returns true if process is running
    //Attempts to find the Ty process if not, returns true if successfully found
    //Returns false if Ty is closed or is in the process of being launched
    public static bool FindProcess()
    {
        if (IsRunning)
            return true;
        if (LaunchingGame)
            return false;

        var processes = Process.GetProcessesByName("Mul-Ty-Player");

        if (processes.Length == 0)
            //BasicIoC.Logger.WriteDebug("Found no processes");
            return false;

        if (processes.Length > 1)
        {
            //Multiple found
            Logger.WriteDebug(
                $"WARNING: Multiple ({processes.Length}) instances of Mul-Ty-Player are open, can and will cause fuckery.");

            //Use first non-exiting process
            var setProcess = false;
            for (var i = 1; i < processes.Length; i++)
                if (processes[i].HasExited)
                {
                    processes[i].Dispose();
                    processes[i].Close();
                }
                else if (!setProcess)
                {
                    process = processes[i];
                    OnTyProcessFound?.Invoke();
                    PullProcessData();
                    setProcess = true;
                }

            return setProcess;
        }

        process = processes[0];
        OnTyProcessFound?.Invoke();
        PullProcessData();
        return true;
    }

    private static void PullProcessData()
    {
        Logger.WriteDebug("Got process, pulling data");
        process.EnableRaisingEvents = true;
        process.Exited += OnExit;
        Handle = OpenProcess(0x1F0FFF, false, process.Id);
        handleOpen = true;

        //Ty takes a little while to open from process.Start(), so we wait until we can find the mainmodule before continuing
        while (process.MainModule == null)
        {
        }

        BaseAddress = process.MainModule.BaseAddress;
        SettingsHandler.Settings.MulTyPlayerFolderPath = process.MainModule.FileName;
        SettingsHandler.Save();
        GameStateHandler.ForceNoIdle();
        IsRunning = true;
    }

    private static void OnExit(object o, EventArgs e)
    {
        Logger.WriteDebug($"Process exited: {process.ExitCode}");
        if (ProcessHandler.LogMostRecentMemoryIOInfoOnProcessExit)
            Logger.WriteDebug($"Last Memory IO operation: {ProcessHandler.MostRecentIOIndicator}");
        OnTyProcessExited?.Invoke();
        IsRunning = false;
        process.Close();
        CloseHandle();
        process.Dispose();
        process.Refresh();
        if (SettingsHandler.Settings.AutoRestartTyOnCrash) TryLaunchGame();
    }

    private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Logger.WriteDebug($"Output data received: {e.Data}");
    }

    private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        Logger.WriteDebug($"Error data received: {e.Data}");
    }

    public static void CloseHandle()
    {
        if (!handleOpen)
            return;

        var successfullyClosed = CloseHandle(Handle);
        handleOpen = false;

        if (!successfullyClosed) Logger.WriteDebug("Handle was not successfully closed!!!");
        //oh well
    }
}