using MulTyPlayerClient.Memory;
using MulTyPlayerClient.Settings;
using System.Threading;
using System.Windows;

namespace MulTyPlayerClient.GUI.Models
{
    public class SplashModel
    {
        public string MessageText = "Hello";
        public Visibility LaunchGameButtonVisibility = Visibility.Hidden;
        public bool EnableLaunchGameButton = false;

        private void FoundTy()
        {
            LaunchGameButtonVisibility = Visibility.Hidden;
            EnableLaunchGameButton = false;
            MessageText = "Mul-Ty-Player is open!";
        }

        private void TyNotFound()
        {
            LaunchGameButtonVisibility = SettingsHandler.HasValidExePath() ? Visibility.Visible : Visibility.Hidden;
            EnableLaunchGameButton = TyProcess.CanLaunchGame;
            MessageText = "Mul-Ty-Player could not be found.\nPlease open the game to continue.";
        }

        private void TyLaunched()
        {
            LaunchGameButtonVisibility = Visibility.Hidden;
            EnableLaunchGameButton = false;
            MessageText = "Auto-launching Mul-Ty-Player...";
        }

        private void TyLaunchFailed()
        {
            LaunchGameButtonVisibility = Visibility.Hidden;
            EnableLaunchGameButton = SettingsHandler.HasValidExePath() && SteamHelper.IsLoggedOn();
            MessageText = "Failed to auto-launch Mul-Ty-Player, please open the game manually to continue.";
        }

        public void InitializeTyProcess()
        {
            Thread.Sleep(3000);

            TyProcess.OnTyProcessLaunched += TyLaunched;
            TyProcess.OnTyProcessLaunchFailed += TyLaunchFailed;

            //If couldn't find an existing process
            if (!TyProcess.FindProcess())
            {
                //check to see if we have the path to the exe, if we have the autolaunch setting enabled
                //and if we are logged into steam,
                //If all conditions are met, launch the game

                bool launchOnStartup = SettingsHandler.HasValidExePath() && SettingsHandler.Settings.AutoLaunchTyOnStartup && SteamHelper.IsLoggedOn();
                bool successfulLaunch = false;
                if (launchOnStartup)
                {
                    successfulLaunch = TyProcess.TryLaunchGame();
                }

                if (!launchOnStartup || !successfulLaunch)
                {
                    TyNotFound();
                }
            }

            //If there was no process AND/OR failed to launch the game,
            //Periodically search for the process until its found
            //Enable the launch button if conditions are met
            while (!TyProcess.FindProcess())
            {
                EnableLaunchGameButton = TyProcess.CanLaunchGame;
                Thread.Sleep(250);
            }

            FoundTy();
            Thread.Sleep(3000);

            TyProcess.OnTyProcessLaunched -= TyLaunched;
            TyProcess.OnTyProcessLaunchFailed -= TyLaunchFailed;
        }
    }
}
