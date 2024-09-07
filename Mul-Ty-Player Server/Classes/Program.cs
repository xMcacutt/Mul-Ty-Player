using System;
using System.Diagnostics;
using System.IO;
using MulTyPlayerServer.Classes.Networking;
using MulTyPlayerServer.Classes.Networking.Commands;
using MulTyPlayerServer.Sync.Objective;
using Octokit;

namespace MulTyPlayerServer;

internal class Program
{
    public static KoalaHandler HKoala;
    public static SyncHandler HSync;
    public static CommandHandler HCommand;
    public static PlayerHandler HPlayer;
    public static ObjectiveHandler HObjective;
    public static HSD_DraftsHandler HDrafts;
    public static ChaosHandler HChaos;
    public static CollectionModeHandler HCollection;
    public static string _inputStr;

    private static void Main()
    {
        //RESTARTS THE APP WHEN IT REACHES END (DISCONNECT)
        do
        {
            Console.Clear();
            _inputStr = "n";
            RunProgram();
            if (_inputStr != "y") _inputStr = Console.ReadLine();
        } while (_inputStr == "y");
    }

    private static void RunProgram()
    {
        Console.Title = "Mul-Ty-Player Server";

        SettingsHandler.Setup();
        
        var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
        var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Mul-Ty-Player").Result;
        var latestVersion = latestRelease.TagName.Replace("v", "");
        var result = VersionHandler.Compare(SettingsHandler.ServerSettings.Version, latestVersion);
        if (result == VersionResult.SecondNewer)
        {
            Console.WriteLine("Updates are available!");
            if (SettingsHandler.ServerSettings.DoAutoUpdate
                && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mul-Ty-Player Mini Updater.exe")))
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mul-Ty-Player Mini Updater.exe"));
        }
        
        Server.StartServer();

        HSync = new SyncHandler();
        HKoala = new KoalaHandler();
        HCommand = new CommandHandler();
        HPlayer = new PlayerHandler();
        HObjective = new ObjectiveHandler();
        HDrafts = new HSD_DraftsHandler();
        HChaos = new ChaosHandler();
        HCollection = new CollectionModeHandler();
        Console.WriteLine(
            "Welcome to Mul-Ty-Player.\nThis is the server application. \nPort forward to allow connections.\n");

        var command = Console.ReadLine();
        while (command != "/stop")
        {
            Console.WriteLine(HCommand.ParseCommand(command));
            if (command != "/restart")
                command = Console.ReadLine();
            else
                command = "/stop";
        }

        Server._isRunning = false;
    }
}