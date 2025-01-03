﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MulTyPlayer;
using MulTyPlayerClient.Classes.GamePlay;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.Models;
using Riptide;

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

    public void DisplayInGameMessage(string message, int seconds = 0)
    {
        if (IsOnMainMenuOrLoading)
            return;
        // SHOW MENU
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x286C54, new byte[] { 0x1 });
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x289048, new byte[] { 0x1 });
        
        // SET MENU TYPE TO HARD DRIVE ERROR WITH PREVIOUS STATE TO SAME VALUE TO LOCK IF REQUIRED
        var previousState = seconds == 0 ? (byte)0x0 : (byte)0xD;
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x286800, new byte[] { 0xD });
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x286804, new byte[] { previousState });

        // SET UP FRONT END POINTERS TO AVOID CRASHING
        var addr = PointerCalculations.GetPointerAddress(0x293B14, new[] { 0x4 });
        ProcessHandler.TryRead(addr, out int ptr, false, "Get pointer for pause menu");
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x286D18, BitConverter.GetBytes(ptr));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x287580, BitConverter.GetBytes(ptr));
        
        // SET TEXT ON MENU
        addr = PointerCalculations.GetPointerAddress(0x529220, new[] { 0x1D });
        var footer = seconds == 0 ? "Press ESC To Continue" : seconds.ToString();
        ProcessHandler.WriteData(addr, Encoding.ASCII.GetBytes(message + "\n\n" + footer).Concat(new byte[1]).ToArray());
        
        // REMOVE CURSOR
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2887FC, new byte[] { 0x0 });

        if (seconds == 0)
            return;

        Task.Run(async () =>
        {
            for (var t = seconds; t > 0; t--)
            {
                ProcessHandler.WriteData(addr, Encoding.ASCII.GetBytes(message + "\n\n" + t).Concat(new byte[1]).ToArray());
                await Task.Delay(1000);
            }
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x286C54, new byte[] { 0x0 });
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x289048, new byte[] { 0x0 });
        });
    }
    
    public void ProtectLeaderboard()
    {
        var address = SyncHandler.SaveDataBaseAddress + 0xB07;
        // WRITES 1 TO THE MSB OF THE TIME, SETTING IT VERY HIGH
        ProcessHandler.WriteData(address, 
            new byte[] { 1 }, 
            "Protecting leaderboard");
        address = 0x1C8D6D;
        ProcessHandler.WriteData((int)TyProcess.BaseAddress+address, 
            new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90  }, 
            "Protecting leaderboard");
    }

    public void ToggleCollectibleLines()
    {
        ProcessHandler.TryRead(0x28AB68, out bool result, true, "ToggleCollectibleLines()");
        ProcessHandler.WriteData((int)(TyProcess.BaseAddress + 0x28AB68), BitConverter.GetBytes(!result));
    }

    public void ToggleLevelSelect()
    {
        var addr = PointerCalculations.GetPointerAddress(0x286CB0, new[] { 0xCA4 });
        ProcessHandler.TryRead(addr + 2, out bool result, false, "ToggleLevelSelect()");
        var toggledByte = BitConverter.GetBytes(!result)[0];
        ProcessHandler.WriteData(addr, new byte[3] {toggledByte, 0, toggledByte});
    }
    
    public void SetMenuItemFlag(TyMenuItem item, TyMenuItemFlag flag, bool value)
    {
        var addr = PointerCalculations.GetPointerAddress(0x286CB0, new[] { 0x164 + 0x168 * (int)item + (int)flag });
        ProcessHandler.WriteData(addr, BitConverter.GetBytes(value));
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
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x52F2BC, 
            BitConverter.GetBytes((int)TyProcess.BaseAddress + 0x273844));
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

    [MessageHandler((ushort)MessageID.ForceMainMenu)]
    private static void HandleForceMenu(Message message)
    {
        Client.HGameState.ForceBackToMainMenu();
    }

    public void SetCameraState(CameraState state)
    {
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EBD0, BitConverter.GetBytes((int)state));
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x27EBC8, BitConverter.GetBytes((int)state));
    }

    public void SetNewGameText(string normalModeText, string subText, string hardcoreModeText)
    {
        var baseAddr = PointerCalculations.GetPointerAddress(0x528740, new[] { 0x0 });
        ProcessHandler.WriteData(baseAddr, Encoding.ASCII.GetBytes(normalModeText).Concat(new byte[1]).ToArray());
        ProcessHandler.WriteData(baseAddr - 0x5B, Encoding.ASCII.GetBytes(hardcoreModeText).Concat(new byte[1]).ToArray());
        ProcessHandler.WriteData(baseAddr - 0x25, Encoding.ASCII.GetBytes(subText).Concat(new byte[1]).ToArray());
    }

    public static void UpdateExeSettings()
    {
        var magnetData = SettingsHandler.ClientSettings.DoForceMagnets ? _magnetBytesFixed : _magnetBytesOrigin;
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x139077, magnetData[0]);
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x139090, magnetData[1]);
        
        var outbackMovementData = SettingsHandler.ClientSettings.DoOldOutbackMovement ? 
            new byte[] {0x90, 0x90} 
            : new byte[] {0x75, 0x06};
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x17311B, outbackMovementData);
        
        var rangSwapData = SettingsHandler.ClientSettings.DoOldRangSwap ?             
            new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
            : new byte[] { 0x0F, 0x85, 0xC0, 0x00, 0x00, 0x00 };
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x162B8A, rangSwapData);
        
        var menuPositionData = SettingsHandler.ClientSettings.DoFixMenuPositions ? 
            BitConverter.GetBytes(-5f)
            : BitConverter.GetBytes(48f);
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2025B0, menuPositionData);
        
        var cameraAimingData = SettingsHandler.ClientSettings.DoControllerCameraAiming ?             
            new byte[] {0x90, 0x90} : new byte[] {0x75, 0x0C};
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x16A6BC, cameraAimingData);
        
        var gameInfoUnlockData = SettingsHandler.ClientSettings.DoUnlockGameInfo ? 
            new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }
            : new byte[] { 0x80, 0x7C, 0x31, 0x10, 0x00 };
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0xE55CD, gameInfoUnlockData);
        ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0xE6A4D, gameInfoUnlockData);
    }

    private static byte[][] _magnetBytesOrigin = new byte[][]
    {
        new byte[] { 0x75, 0x27 },
        new byte[] { 0xe8, 0x6b, 0x51, 0xff, 0xff, 0x3b, 0xf0, 0x7d, 0x07 }
    };
    
    private static byte[][] _magnetBytesFixed = new byte[][]
    {
        new byte[] { 0x90, 0x90 },
        new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
    };

    public bool IsInTimeAttack()
    {
        ProcessHandler.TryRead(0x28CA3C, out bool result, true, "IsInTimeAttack");
        return result;
    }
}

public enum TyMenuItem
{
    Continue,
    GameInfo,
    Skins,
    SaveGame,
    Options,
    Leaderboards,
    ExitLevel,
    MainMenu
}

public enum TyMenuItemFlag
{
    Enabled,
    Selected,
    Visible
}
