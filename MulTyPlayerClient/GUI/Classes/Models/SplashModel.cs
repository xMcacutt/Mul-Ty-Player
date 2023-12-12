﻿using MulTyPlayerClient.Classes.Utility;
using System;
using System.ComponentModel;
using System.Threading;

namespace MulTyPlayerClient.GUI.Models
{
    public class SplashModel
    {
        public event Action OnComplete;
        public event Action<SplashState> OnStateChange;

        public SplashModel()
        {            
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) => GetProcessSequence();
            backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                OnComplete?.Invoke();
            };
            backgroundWorker.RunWorkerAsync();
        }

        private void GetProcessSequence()
        {
            Thread.Sleep(2000);

            bool existingProcess = TyProcess.FindProcess();
            //if process already exists, OnTyProcessFound is called, do nothing else
            if (existingProcess)
            {
                OnStateChange?.Invoke(SplashState.TyFound);
                return;
            }

            TyProcess.OnTyProcessLaunched += TyLaunched;
            TyProcess.OnTyProcessLaunchFailed += TyFailedToLaunch;

            bool canAutoLaunchGame =
                SettingsHandler.Settings.AutoLaunchTyOnStartup
                && SettingsHandler.HasValidExePath()
                && TyProcess.CanLaunchGame;

            if (canAutoLaunchGame)
            {
                TyProcess.TryLaunchGame();
            }
            else
            {
                TyNotFound();
            }

            TyProcess.OnTyProcessLaunched -= TyLaunched;
            TyProcess.OnTyProcessLaunchFailed -= TyFailedToLaunch;
            TyProcess.OnTyProcessFound += TyFound;

            //If there was no process AND failed to launch the game,
            //Periodically search for the process until its found
            //Enable the launch button if conditions are met
            while (!TyProcess.FindProcess())
            {
                Thread.Sleep(250);
            }
            TyProcess.OnTyProcessFound -= TyFound;
        }

        public bool ShouldEnableLaunchGameButton()
        {
            return SettingsHandler.HasValidExePath() && TyProcess.CanLaunchGame && SteamHelper.IsLoggedOn();
        }

        public enum SplashState
        {
            TyFound,
            TyNotFound,
            TyLaunched,
            TyFailedToLaunch
        }

        private void TyFound()
        {
            OnStateChange?.Invoke(SplashState.TyFound);
        }
        private void TyNotFound()
        {
            OnStateChange?.Invoke(SplashState.TyNotFound);
        }
        private void TyLaunched()
        {
            OnStateChange?.Invoke(SplashState.TyLaunched);
        }
        private void TyFailedToLaunch()
        {
            OnStateChange?.Invoke(SplashState.TyFailedToLaunch);
        }
    }
}
