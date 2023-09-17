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
        
        //IF LOGGER INSTANCE IS NO INSTANTIATED HERE, IT WON'T SHOW UP. IDK WHY
        public static Logger LoggerInstance;
        public static SFXPlayer SFXPlayer;

        public static void InstantiateModels()
        {
            Settings = new();
            LoggerInstance = new(200);

            Splash = new();
            Login = new();
            KoalaSelect = new();
            Lobby = new();
            Setup = new();
            SFXPlayer = new();
        }
    }
}
