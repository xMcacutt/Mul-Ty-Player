using MulTyPlayerClient.GUI.Classes.Views;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Models;

public static class ModelController
{
    public static SplashModel Splash { get; private set; }
    public static LoginModel Login { get; private set; }
    public static KoalaSelectModel KoalaSelect { get; private set; }
    public static LobbyModel Lobby { get; private set; }
    public static SettingsViewModel Settings { get; private set; }
    public static HS_ChooseTeamModel HS_ChooseTeam { get; private set; }
    //public static HS_DraftModel HS_Draft { get; private set; }
    public static Minimap Minimap = new Minimap();

    public static void InstantiateModels()
    {
        //Settings must be instantiated first as other models rely on it.
        Settings = new SettingsViewModel();
        Splash = new SplashModel();
        Login = new LoginModel();
        KoalaSelect = new KoalaSelectModel();
        Lobby = new LobbyModel();
        HS_ChooseTeam = new HS_ChooseTeamModel();
        //HS_Draft = new HS_DraftModel();
    }
}