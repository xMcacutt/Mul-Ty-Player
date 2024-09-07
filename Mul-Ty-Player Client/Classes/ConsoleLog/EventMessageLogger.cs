using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient.Classes.ConsoleLog;

internal static class EventMessageLogger
{
    public static void Init()
    {
        TyProcess.OnTyProcessExited += OnTyProcessExited;
        TyProcess.OnTyProcessFound += OnTyProcessFound;
        TyProcess.OnTyProcessLaunched += OnTyProcessLaunched;
        TyProcess.OnTyProcessLaunchFailed += OnTyProcessLaunchFailed;
    }

    private static void OnTyProcessExited()
    {
        SFXPlayer.PlaySound(SFX.MenuCancel);
        var msg = "Ty the Tasmanian Tiger has exited, ";

        if (SettingsHandler.ClientSettings.AutoRestartTyOnCrash)
        {
            Logger.Write(msg + "attempting to re-launch the game...");
        }
        else
        {
            ModelController.Lobby.CanLaunchGame = SettingsHandler.HasValidExePath() && SteamHelper.IsLoggedOn();
            Logger.Write(msg + "please re-open the game to continue.");
        }
    }

    private static void OnTyProcessFound()
    {
        ModelController.Lobby.CanLaunchGame = false;
        SFXPlayer.PlaySound(SFX.MenuAccept);
        Logger.Write("Found game process, you're in!");
    }

    private static void OnTyProcessLaunched()
    {
        ModelController.Lobby.CanLaunchGame = false;
        SFXPlayer.PlaySound(SFX.MenuAccept);
        Logger.Write("Launched game successfully, you're in!");
    }

    private static void OnTyProcessLaunchFailed()
    {
        ModelController.Lobby.CanLaunchGame = SettingsHandler.HasValidExePath() && SteamHelper.IsLoggedOn();
        SFXPlayer.PlaySound(SFX.PlayerDisconnect);
        Logger.Write("Failed to restart Ty automatically, please manually open the game.");
    }
}