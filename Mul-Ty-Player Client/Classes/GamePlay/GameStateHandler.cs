using System;
using System.Linq;
using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient;

public class GameStateHandler
{
    private const int MAIN_MENU_STATE_ADDRESS = 0x286641;
    private const int LOADING_SCREEN_STATE_ADDRESS = 0x27EBF0;

    private bool wasLoadingLastFrame;
    public bool IsOnMainMenuOrLoading = true;

    public void CheckMainMenuOrLoading()
    {
        ProcessHandler.TryRead(LOADING_SCREEN_STATE_ADDRESS, out long result, true,
            "GameStateHandler::IsAtMainMenuOrLoading()");
        IsOnMainMenuOrLoading = result == 0;
        if (IsOnMainMenuOrLoading)
            wasLoadingLastFrame = true;
    }

    public void CheckLoadChanged()
    {
        if (wasLoadingLastFrame && !IsOnMainMenuOrLoading) 
            Client.HLevel.DoLevelSetup();
        wasLoadingLastFrame = IsOnMainMenuOrLoading;
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
        
        if (!onMenu) return;
        
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
        {
            Logger.Write("[ERROR] Failed to find self in player list.");
            return;
        }
        
        self.Level = "M/L";
    }
    
    
    public void ProtectLeaderboard()
    {
        var address = SyncHandler.SaveDataBaseAddress + 0xB07;
        ProcessHandler.WriteData(address, new byte[] { 1 }, "Protecting leaderboard");
    }
}