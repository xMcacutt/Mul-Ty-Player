﻿using System;

namespace MulTyPlayerClient.Classes;

public class GameInfoHandler
{
    private bool[] levelsActive;

    public GameInfoHandler()
    {
        levelsActive = new bool[24];
    }
    
    public void ActivateGameInfoScreen(int level)
    {
        if (levelsActive[level] || level < 0 || level > 23)
            return;
        ProcessHandler.WriteData(SyncHandler.SaveDataBaseAddress + 0xB0 * level, new byte[] { 0x1 });
        levelsActive[level] = true;
    }
}