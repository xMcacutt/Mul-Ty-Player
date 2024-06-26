﻿using System.Threading;
using System.Windows;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class SplashViewModel : IViewModel
{
    private static SplashModel Splash => ModelController.Splash;
    
    public SplashViewModel()
    {
        Splash.OnStateChange += DisplayMessage;
    }

    public string MessageText { get; set; } = "Hello!";
    public Visibility LaunchGameButtonVisibility { get; set; }
    public bool EnableLaunchGameButton { get; set; }


    public void OnEntered()
    {
    }

    public void OnExited()
    {
    }

    private void DisplayMessage(SplashModel.SplashState state)
    {
        switch (state)
        {
            case SplashModel.SplashState.TyFound:
                DisplayMessage_TyFound();
                break;
            case SplashModel.SplashState.TyLaunched:
                DisplayMessage_TyLaunched();
                break;
            case SplashModel.SplashState.TyFailedToLaunch:
                DisplayMessage_TyFailedToLaunch();
                break;
            case SplashModel.SplashState.TyNotFound:
                DisplayMessage_TyNotFound();
                break;
        }
    }

    private void DisplayMessage_TyFound()
    {
        LaunchGameButtonVisibility = Visibility.Hidden;
        EnableLaunchGameButton = false;
        MessageText = "Mul-Ty-Player is open!";
        Thread.Sleep(1000);
    }

    private void DisplayMessage_TyNotFound()
    {
        LaunchGameButtonVisibility = SettingsHandler.HasValidExePath() ? Visibility.Visible : Visibility.Hidden;
        EnableLaunchGameButton = Splash.ShouldEnableLaunchGameButton();
        MessageText = "Mul-Ty-Player could not be found.\nPlease open the game to continue.";
    }

    private void DisplayMessage_TyLaunched()
    {
        LaunchGameButtonVisibility = Visibility.Hidden;
        EnableLaunchGameButton = false;
        MessageText = "Auto-launching Mul-Ty-Player...";
        Thread.Sleep(1000);
    }

    private void DisplayMessage_TyFailedToLaunch()
    {
        LaunchGameButtonVisibility = Visibility.Hidden;
        EnableLaunchGameButton = false;
        MessageText = "Failed to auto-launch Mul-Ty-Player, please open the game manually to continue.";
    }
}