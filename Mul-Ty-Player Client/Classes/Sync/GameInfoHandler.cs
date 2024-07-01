namespace MulTyPlayerClient.Classes;

public class GameInfoHandler
{
    private bool[] levelsActive = new bool[24];

    public GameInfoHandler()
    {
        levelsActive = new bool[24];
    }
    
    public void ActivateGameInfoScreen(int level)
    {
        if (levelsActive[level])
            return;
        ProcessHandler.WriteData(SyncHandler.SaveDataBaseAddress + 0xB0 * level, new byte[] { 0x1 });
    }
}