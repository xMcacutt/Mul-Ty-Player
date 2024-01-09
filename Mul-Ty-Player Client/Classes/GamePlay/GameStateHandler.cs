using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient;

public class GameStateHandler
{
    private const int MAIN_MENU_STATE_ADDRESS = 0x286641;
    private const int LOADING_SCREEN_STATE_ADDRESS = 0x27EBCC;

    private bool wasLoadingLastFrame;

    public bool IsAtMainMenuOrLoading()
    {
        ProcessHandler.TryRead(LOADING_SCREEN_STATE_ADDRESS, out bool result, true,
            "GameStateHandler::IsAtMainMenuOrLoading()");
        var loading = !result;
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
        if (PlayerHandler.TryGetLocalPlayer(out var player))
            player.IsReady &= onMenu;

        //Does this really need to be here? Loop over every player every frame just to update the ready icon?
        //Surely theres a way to update this once WHEN that player ready status changes
        ModelController.Lobby.UpdateReadyStatus();

        if (onMenu && ModelController.Lobby.TryGetPlayerInfo(Client._client.Id, out var playerInfo))
            playerInfo.Level = "M/L";
    }
}