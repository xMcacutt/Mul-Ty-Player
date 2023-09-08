using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using System;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        public IViewModel CurrentViewModel { get; set; }
        private IViewModel lastViewModel;

        private SplashViewModel splashViewModel;
        private LoginViewModel loginViewModel;
        private KoalaSelectViewModel koalaSelectViewModel;
        private LobbyViewModel lobbyViewModel;

        public MainViewModel()
        {
            splashViewModel = new SplashViewModel();
            loginViewModel = new LoginViewModel();
            koalaSelectViewModel = new KoalaSelectViewModel();
            lobbyViewModel = new LobbyViewModel();

            lastViewModel = splashViewModel;
            CurrentViewModel = splashViewModel;

            ModelController.Splash.OnComplete += () => GoToView(View.Login);
            ModelController.Login.OnLoginSuccess += () => GoToView(View.KoalaSelect);
            ModelController.KoalaSelect.OnKoalaSelected += (k) => GoToView(View.Lobby);
            ModelController.Lobby.OnLogout += () => GoToView(View.Login);
        }

        public void GoToView(View view)
        {
            lastViewModel = CurrentViewModel;
            CurrentViewModel = view switch
            {
                View.Splash => splashViewModel,
                View.Login => loginViewModel,
                View.KoalaSelect => koalaSelectViewModel,
                View.Lobby => lobbyViewModel,
                _ => throw new NotImplementedException($"Tried to switch to unsupported view: {view}"),
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
        Lobby,
    }
}
