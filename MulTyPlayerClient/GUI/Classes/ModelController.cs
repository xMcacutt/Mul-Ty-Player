using MulTyPlayerClient.GUI.ViewModels;
using System.Windows.Media;

namespace MulTyPlayerClient.GUI.Models
{
    public class ModelController
    {
        public static SplashModel Splash = new();
        public static LoginModel Login = new();
        public static KoalaSelectModel KoalaSelect = new();
        public static LobbyModel Lobby = new();
        public static SettingsViewModel Settings = new();
        public static SetupViewModel Setup = new();
        
        //IF LOGGER INSTANCE IS NO INSTANTIATED HERE, IT WON'T SHOW UP. IDK WHY
        public static Logger LoggerInstance = new(200);
        public static SFXPlayer SFXPlayer = new();
    }
}
