using System;

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
        ProcessHandler.CheckAddress(SyncHandler.SaveDataBaseAddress - 0x10, 2864, "CheckingSaveData", out bool result);
        if (level < 0 || level > 23 || levelsActive[level] || Client.Relaunching || !result)
            return;
        ProcessHandler.WriteData(SyncHandler.SaveDataBaseAddress + 0x70 * level, new byte[] { 0x1 }, "Game Info");
        levelsActive[level] = true;
    }
}