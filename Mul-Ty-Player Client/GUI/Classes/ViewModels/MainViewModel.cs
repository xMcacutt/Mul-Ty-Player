using System;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class MainViewModel
{
    private readonly KoalaSelectViewModel koalaSelectViewModel;
    private IViewModel lastViewModel;
    private readonly LobbyViewModel lobbyViewModel;
    private readonly LoginViewModel loginViewModel;

    private readonly SplashViewModel splashViewModel;

    public MainViewModel()
    {
        splashViewModel = new SplashViewModel();
        loginViewModel = new LoginViewModel();
        koalaSelectViewModel = new KoalaSelectViewModel();
        lobbyViewModel = new LobbyViewModel();

        lastViewModel = splashViewModel;
        CurrentViewModel = splashViewModel;

        ModelController.Splash.OnComplete += () => GoToView(View.Login);
        ModelController.Login.OnLoginSuccess += CheckSpectator;
        ModelController.KoalaSelect.OnProceedToLobby += () => GoToView(View.Lobby);
        ModelController.Lobby.OnLogout += () => GoToView(View.Login);
    }

    public IViewModel CurrentViewModel { get; set; }

    private void CheckSpectator()
    {
        if (ModelController.Login.JoinAsSpectator) 
            GoToView(View.Lobby);
        else
            GoToView(View.KoalaSelect);
    }

    private void GoToView(View view)
    {
        lastViewModel = CurrentViewModel;
        CurrentViewModel = view switch
        {
            View.Splash => splashViewModel,
            View.Login => loginViewModel,
            View.KoalaSelect => koalaSelectViewModel,
            View.Lobby => lobbyViewModel,
            _ => throw new NotImplementedException($"Tried to switch to unsupported view: {view}")
        };
        lastViewModel.OnExited();
        CurrentViewModel.OnEntered();
    }
}

public enum View
{
    Splash,
    Login,
    KoalaSelect,
    Lobby
}