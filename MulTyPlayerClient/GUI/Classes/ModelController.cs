using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Models
{
    public static class ModelController
    {
        public static SplashModel Splash;
        public static LoginModel Login;
        public static KoalaSelectModel KoalaSelect;
        public static LobbyModel Lobby;
        public static SettingsViewModel Settings;
        public static SetupViewModel Setup;

        public static void InstantiateModels()
        {
            Settings = new();
            Splash = new();
            Login = new();
            KoalaSelect = new();
            Lobby = new();
            Setup = new();
        }
    }
}
