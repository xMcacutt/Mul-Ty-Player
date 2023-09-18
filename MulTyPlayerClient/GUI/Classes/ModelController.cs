using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Models
{
    public static class ModelController
    {
        public static SplashModel Splash {get; private set;}
        public static LoginModel Login {get; private set;}
        public static KoalaSelectModel KoalaSelect {get; private set;}
        public static LobbyModel Lobby {get; private set;}

        public static SettingsViewModel Settings {get; private set;}
        public static SetupViewModel Setup {get; private set;}

        public static void InstantiateModels()
        {
            //Settings must be instantiated first as other models rely on it.
            Settings = new();
            Splash = new();
            Login = new();
            KoalaSelect = new();
            Lobby = new();
            Setup = new();
        }
    }
}
