using System.Windows.Data;

namespace MulTyPlayerClient.GUI
{
    public class BasicIoC
    {
        public static SplashViewModel SplashScreenViewModel = new();
        public static LoginPageViewModel LoginViewModel = new();
        public static KoalasViewModel KoalaSelectViewModel = new();
        public static MainClientViewModel MainGUIViewModel = new();
        
        //IF LOGGER INSTANCE IS NO INSTANTIATED HERE, IT WON'T SHOW UP. IDK WHY
        public static Logger LoggerInstance = new(200);
    }
}
