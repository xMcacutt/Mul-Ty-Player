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
    public static HSD_ChooseTeamModel HSD_ChooseTeam { get; private set; }
    public static HSD_DraftModel HSD_Draft { get; private set; }

    public static void InstantiateModels()
    {
        //Settings must be instantiated first as other models rely on it.
        Settings = new SettingsViewModel();
        Splash = new SplashModel();
        Login = new LoginModel();
        KoalaSelect = new KoalaSelectModel();
        Lobby = new LobbyModel();
        HSD_ChooseTeam = new HSD_ChooseTeamModel();
        HSD_Draft = new HSD_DraftModel();
    }
}