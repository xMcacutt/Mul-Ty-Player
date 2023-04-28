using MulTyPlayerClient.Classes.Utility;
using PropertyChanged;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MulTyPlayerClient.GUI
{
    [AddINotifyPropertyChangedInterface]
    public class SplashViewModel
    {
        public string MessageText { get; set; } = "Hello!";
        public Visibility LaunchGameButtonVisibility { get; set; } = Visibility.Hidden;
        public bool EnableLaunchGameButton { get; set; } = false;

        public SplashViewModel()
        {
            SettingsHandler.Setup();
            var backgroundWorker = new BackgroundWorker();            
            backgroundWorker.DoWork += (s, e) => FindTy();
            backgroundWorker.RunWorkerCompleted += (s, e) =>
            {
                BasicIoC.LoginViewModel.Setup();
                WindowHandler.LoginWindow.Show();
                WindowHandler.SplashWindow.Hide();
            };
            backgroundWorker.RunWorkerAsync();
        }

        private void FindTy()
        {
            LaunchGameButtonVisibility = Visibility.Hidden;
            EnableLaunchGameButton = false; 
            Thread.Sleep(2000);            

            TyProcess.OnTyProcessLaunched += TyLaunched;
            TyProcess.OnTyProcessLaunchFailed += TyLaunchFailed;
            TyProcess.OnTyProcessFound += FoundTy;
            
            //If couldn't find an existing process
            if (!TyProcess.FindProcess())
            {
                //check to see if we have the path to the exe, if we have the autolaunch setting enabled
                //and if we are logged into steam,
                //If all conditions are met, launch the game
                
                bool launchOnStartup = SettingsHandler.HasValidTyExePath() && SettingsHandler.Settings.AutoLaunchTyOnStartup && SteamHelper.IsLoggedOn();
                
                if (launchOnStartup && TyProcess.TryLaunchGame())
                {                    
                }
                else
                {
                    LaunchGameButtonVisibility = SettingsHandler.HasValidTyExePath() ? Visibility.Visible : Visibility.Hidden;
                    EnableLaunchGameButton = SettingsHandler.HasValidTyExePath() && SteamHelper.IsLoggedOn();
                    BasicIoC.SplashScreenViewModel.MessageText = "Mul-Ty-Player could not be found.\nPlease open the game to continue.";
                }
            }

            //If there was no process AND/OR failed to launch the game,
            //Periodically search for the process until its found
            //Enable the launch button if conditions are met
            while (!TyProcess.FindProcess())
            {
                EnableLaunchGameButton = SettingsHandler.HasValidTyExePath() && SteamHelper.IsLoggedOn();
                Thread.Sleep(250);
            }

            TyProcess.OnTyProcessLaunched -= TyLaunched;
            TyProcess.OnTyProcessLaunchFailed -= TyLaunchFailed;
            TyProcess.OnTyProcessFound -= FoundTy;
        }

        private void FoundTy()
        {
            LaunchGameButtonVisibility = Visibility.Hidden;
            EnableLaunchGameButton = false;
            BasicIoC.SplashScreenViewModel.MessageText = "Mul-Ty-Player is open!";
            Thread.Sleep(1000);
        }

        private void TyLaunched()
        {
            LaunchGameButtonVisibility = Visibility.Hidden;
            EnableLaunchGameButton = false;
            BasicIoC.SplashScreenViewModel.MessageText = "Auto-launching Mul-Ty-Player...";
            Thread.Sleep(1000);
        }

        private void TyLaunchFailed()
        {
            LaunchGameButtonVisibility = Visibility.Hidden;
            EnableLaunchGameButton = SettingsHandler.HasValidTyExePath() && SteamHelper.IsLoggedOn();
            BasicIoC.SplashScreenViewModel.MessageText = "Failed to auto-launch Mul-Ty-Player, please open the game manually to continue.";
            Thread.Sleep(1000);
        }
    }
}
