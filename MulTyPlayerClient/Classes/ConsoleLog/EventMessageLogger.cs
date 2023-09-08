﻿using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
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
            ModelController.SFXPlayer.PlaySound(SFX.MenuCancel);
            string msg = "Ty the Tasmanian Tiger has exited, ";

            if (SettingsHandler.Settings.AutoRestartTyOnCrash)
            {
                ModelController.LoggerInstance.Write(msg + "attempting to re-launch the game...");
            }
            else
            {
                ModelController.Lobby.CanLaunchGame = SettingsHandler.HasValidExePath() && SteamHelper.IsLoggedOn();
                ModelController.LoggerInstance.Write(msg + "please re-open the game to continue.");
            }
        }

        private static void OnTyProcessFound()
        {
            ModelController.Lobby.CanLaunchGame = false;
            ModelController.SFXPlayer.PlaySound(SFX.MenuAccept);
            ModelController.LoggerInstance.Write("Found game process, you're in!");
        }

        private static void OnTyProcessLaunched()
        {
            ModelController.Lobby.CanLaunchGame = false;
            ModelController.SFXPlayer.PlaySound(SFX.MenuAccept);
            ModelController.LoggerInstance.Write("Launched game successfully, you're in!");
        }

        private static void OnTyProcessLaunchFailed()
        {
            ModelController.Lobby.CanLaunchGame = SettingsHandler.HasValidExePath() && SteamHelper.IsLoggedOn();
            ModelController.SFXPlayer.PlaySound(SFX.PlayerDisconnect);
            ModelController.LoggerInstance.Write("Failed to restart Ty automatically, please manually open the game.");
        }
    }
}
