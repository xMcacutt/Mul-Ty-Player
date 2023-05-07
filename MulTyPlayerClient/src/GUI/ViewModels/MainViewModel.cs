using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using System.Collections.Generic;
using System.Windows.Input;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel : BaseViewModel
    {
        public BaseViewModel CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
                CurrentViewModel.OnViewModelEntered();
            }
        }

        private static BaseViewModel _currentViewModel;

        static Dictionary<View, BaseViewModel> viewModels;

        public MainViewModel() : base()
        {
            ChangeView = new UpdateViewCommand(this);
            InitializeViewModels();
            ChangeToView(View.Splash);
        }

        public static SplashViewModel Splash;
        public static LoginPageViewModel Login;
        public static KoalasViewModel KoalaSelect;
        public static LobbyViewModel Lobby;
        public static SettingsViewModel Settings;
        public static SetupViewModel Setup;

        private void InitializeViewModels()
        {
            viewModels = new Dictionary<View, BaseViewModel>();

            SplashModel s = new();
            Splash = new(s);
            Splash.Finished += (o, e) => { };
            viewModels[View.Splash] = Splash;

            Login = new();
            Login.OnConnectionSuccessful += (o, e) => { };
            viewModels[View.Login] = Login;

            KoalaSelect = new();
            KoalaSelect.KoalaClicked += (o, e) => { };
            viewModels[View.KoalaSelect] = KoalaSelect;

            Lobby = new();
            viewModels[View.Lobby] = Lobby;

            Settings = new();
            viewModels[View.Settings] = Settings;

            Setup = new();
            viewModels[View.Setup] = Setup;

            foreach (BaseViewModel bm in viewModels.Values)
            {
                bm.OnMoveToView += ChangeToView;
            }
        }

        public ICommand ChangeView
        {
            get; set;
        }

        public void ChangeToView(View view)
        {
            CurrentViewModel = viewModels[view];
        }
        public static SFXPlayer SFXPlayer = new();
    }
}
