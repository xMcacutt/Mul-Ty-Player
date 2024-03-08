using System;
using System.Linq;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.Objectives;

namespace MulTyPlayerClient;

internal class LevelHandler
{
    public LevelData CurrentLevelData;
    private int currentLevelId;

    public Action<int> OnLevelChange = delegate { };
    private static KoalaHandler HKoala => Client.HKoala;
    private static SyncHandler HSync => Client.HSync;

    public int CurrentLevelId
    {
        get => currentLevelId;
        set
        {
            if (currentLevelId != value)
            {
                currentLevelId = value;
                CurrentLevelData = Levels.GetLevelData(value);
            }
        }
    }

    public void DoLevelSetup()
    {
        if (Client.HGameState.IsOnMainMenuOrLoading)
            return;
        GetCurrentLevel();
        HSync.SetCurrentData(CurrentLevelData.IsMainStage, CurrentLevelData.FrameCount != 0);
        HSync.SetMemAddrs();
        HSync.RequestSync();
        if (PlayerHandler.TryGetPlayer(Client._client.Id, out var self) && SettingsHandler.Settings.UseTyKoalaTextures)
            Client.HHero.ChangeSkin((int)self.Koala + 1);
        Client.HGameState.ProtectLeaderboard();
        if (SettingsHandler.DoTESyncing &&
            HSync.SyncObjects["TE"].GlobalObjectData.ContainsKey(CurrentLevelId) &&
            (HSync.SyncObjects["TE"].SaveSync as SaveTESyncer)?.GlobalSaveData[CurrentLevelId][3] == 1)
            (HSync.SyncObjects["TE"] as TEHandler)?.ShowStopwatch();
        if (CurrentLevelData.Id != 16)
        {
            HKoala.SetBaseAddress();
            HKoala.SetCoordinateAddresses();
            Client.HGlow.SetBaseAddress();
            Client.HGlow.SetCoordinateAddresses();
        }
        if (CurrentLevelData.HasKoalas)
            ObjectiveCountSet();
        OnLevelChange?.Invoke(currentLevelId);
    }

    public void GetCurrentLevel()
    {
        ProcessHandler.TryRead(0x280594, out int levelId, true, "LevelHandler::GetCurrentLevel()");
        if (!PlayerHandler.TryGetPlayer(Client._client.Id, out var self))
        {
            Logger.Write("[ERROR] Failed to find self in player list.");
            return;
        }
        self.Level = Levels.GetLevelData(levelId).Code;
        CurrentLevelId = levelId;
    }

    public static void ObjectiveCountSet()
    {
        var currentCountMax = 16;
        while (currentCountMax != 8)
        {
            var objectiveCounterAddr = PointerCalculations.GetPointerAddress(0x26A4B0, new[] { 0x6E });
            ProcessHandler.TryRead(objectiveCounterAddr, out byte result, false, "LevelHandler::ObjectiveCountSet()");
            currentCountMax = result;
            if (currentCountMax == 16)
            {
                ProcessHandler.WriteData(objectiveCounterAddr, BitConverter.GetBytes(8),
                    "Setting koala objective koala count");
                currentCountMax = 8;
            }
        }
    }
}