using System;

namespace MulTyPlayerClient.GUI.Models;

public class HSD_LevelModel
{
    public HSD_LevelModel(int levelId)
    {
        LevelData = Levels.GetLevelData(levelId);
        ActiveImageSource = new Uri(@$"pack://siteoforigin:,,,/GUI/HS_DraftAssets/{LevelData.Id}.png");
        InactiveImageSource = new Uri(@$"pack://siteoforigin:,,,/GUI/HS_DraftAssets/{LevelData.Id}bw.png");
    }
    
    public LevelData LevelData { get; }
    
    public Uri ActiveImageSource { get; private set; }
    public Uri InactiveImageSource { get; private set; }
    
    public bool IsAvailable { get; set; }
    
    public event Action<bool> OnAvailabilityChanged;

    public void SetAvailability(bool available)
    {
        IsAvailable = available;
        OnAvailabilityChanged?.Invoke(available);
    }
}