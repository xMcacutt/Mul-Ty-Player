using System;
using System.Diagnostics;
using System.Linq;
using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient;

public class GameStateHandler
{
    private const int MAIN_MENU_STATE_ADDRESS = 0x286641;
    private const int LOADING_SCREEN_STATE_ADDRESS = 0x27EBF0;

    private bool wasLoadingLastFrame = true;
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

    public void ToggleCollectibleLines()
    {
        ProcessHandler.TryRead(0x28AB68, out bool result, true, "ToggleInvincibility()");
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x28AB68), BitConverter.GetBytes(!result));
    }

    public void ToggleLevelSelect()
    {
        var addr = PointerCalculations.GetPointerAddress(0x286CB0, new[] { 0xCA4 });
        ProcessHandler.TryRead(addr + 2, out bool result, false, "ToggleInvincibility()");
        var toggledByte = BitConverter.GetBytes(!result)[0];
        ProcessHandler.WriteData(addr, new byte[3] {toggledByte, 0, toggledByte});
    }

    public void ForceEnterNewGameScreen()
    {
        if (!IsOnMainMenuOrLoading)
            return;
        // Force onto main menu
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x285B8C), BitConverter.GetBytes(4));
        // Force select new game
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x286640), BitConverter.GetBytes(1));
        // Force enter new game screen
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x286650), BitConverter.GetBytes(6));
    }

    public void ForceBackToMainMenu()
    {
        if (IsOnMainMenuOrLoading)
            return;
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x264248, BitConverter.GetBytes(0.01f));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x286C54, new byte[1]);
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x285B8C), BitConverter.GetBytes(4));
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x288A74), BitConverter.GetBytes(8));
    }

    public void ForcePrepareNewGame(int slot)
    {
        if (!IsOnMainMenuOrLoading)
            return;
        // Force save slot bytes
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x273838, BitConverter.GetBytes(slot));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x273840, BitConverter.GetBytes(slot));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x28E6C4, BitConverter.GetBytes(slot));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x52F2B8, BitConverter.GetBytes(slot));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27383C, BitConverter.GetBytes(0));
        // Write save pointer
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x52F2BC, BitConverter.GetBytes((int)TyProcess.BaseAddress + 0x273844));
        // Save save pointer
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x52F2A4, BitConverter.GetBytes(2));
        // Switch to save created screen
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x273F74, BitConverter.GetBytes(9));
    }

    public static void ForceNoIdle()
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x526C08, new byte[] { 0x1 });
    }
    
    public void ForceNewGame()
    {
        // Force enter new game
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2864BC, new byte[] { 0x2 });
    }
}