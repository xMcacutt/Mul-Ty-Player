using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes.ConsoleLog
{
    static class EventMessageLogger
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
            BasicIoC.SFXPlayer.PlaySound(SFX.MenuCancel);
            string msg = "Ty the Tasmanian Tiger has exited, ";

            if (SettingsHandler.Settings.AutoRestartTyOnCrash)
            {
                BasicIoC.LoggerInstance.Write(msg + "attempting to re-launch the game...");
            }
            else
            {
                BasicIoC.MainGUIViewModel.IsLaunchGameButtonEnabled = SettingsHandler.HasValidExePath() && SteamHelper.IsLoggedOn();
                BasicIoC.LoggerInstance.Write(msg + "please re-open the game to continue.");
            }
        }

        private static void OnTyProcessFound()
        {
            BasicIoC.MainGUIViewModel.IsLaunchGameButtonEnabled = false;
            BasicIoC.SFXPlayer.PlaySound(SFX.MenuAccept);
            BasicIoC.LoggerInstance.Write("Found game process, you're in!");
        }

        private static void OnTyProcessLaunched()
        {
            BasicIoC.MainGUIViewModel.IsLaunchGameButtonEnabled = false;
            BasicIoC.SFXPlayer.PlaySound(SFX.MenuAccept);
            BasicIoC.LoggerInstance.Write("Launched game successfully, you're in!");
        }

        private static void OnTyProcessLaunchFailed()
        {
            BasicIoC.MainGUIViewModel.IsLaunchGameButtonEnabled = SettingsHandler.HasValidExePath() && SteamHelper.IsLoggedOn();
            BasicIoC.SFXPlayer.PlaySound(SFX.PlayerDisconnect);
            BasicIoC.LoggerInstance.Write("Failed to restart Ty automatically, please manually open the game.");
        }
    }
}
