using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient;

public class GameStateHandler
{
    private const int MAIN_MENU_STATE_ADDRESS = 0x286641;
    private const int LOADING_SCREEN_STATE_ADDRESS = 0x27EBF0;

    private bool wasLoadingLastFrame;

    public bool IsAtMainMenuOrLoading()
    {
        ProcessHandler.TryRead(LOADING_SCREEN_STATE_ADDRESS, out long result, true,
            "GameStateHandler::IsAtMainMenuOrLoading()");
        var loading = result == 0;
        if (wasLoadingLastFrame && !loading) Client.HLevel.DoLevelSetup();
        wasLoadingLastFrame = loading;
        return loading;
    }

    public bool IsAtMainMenu()
    {
        ProcessHandler.TryRead(MAIN_MENU_STATE_ADDRESS, out bool result, true, "GameStateHandler::IsAtMainMenu()");
        var onMenu = !result;
        NotifyLobbyOfMenu(onMenu);
        return onMenu;
    }

    private void NotifyLobbyOfMenu(bool onMenu)
    {
        ModelController.Lobby.IsOnMenu = onMenu;
        if (onMenu && ModelController.Lobby.TryGetPlayerInfo(Client._client.Id, out var playerInfo))
            playerInfo.Level = "M/L";
    }
    
    
    public void ProtectLeaderboard()
    {
        var address = SyncHandler.SaveDataBaseAddress + 0xB07;
        ProcessHandler.WriteData(address, new byte[] { 1 }, "Protecting leaderboard");
    }
}