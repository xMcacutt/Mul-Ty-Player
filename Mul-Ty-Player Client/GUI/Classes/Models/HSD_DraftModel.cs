using System;

namespace MulTyPlayerClient.GUI.Models;

public class HSD_DraftModel
{
    public HSD_LevelModel LevelZ1 { get; set; } = new HSD_LevelModel(0);
    public HSD_LevelModel LevelA1 { get; set; } = new HSD_LevelModel(4);
    public HSD_LevelModel LevelA2 { get; set; } = new HSD_LevelModel(5);
    public HSD_LevelModel LevelA3 { get; set; } = new HSD_LevelModel(6);
    public HSD_LevelModel LevelB1 { get; set; } = new HSD_LevelModel(8);
    public HSD_LevelModel LevelB2 { get; set; } = new HSD_LevelModel(9);
    public HSD_LevelModel LevelB3 { get; set; } = new HSD_LevelModel(10);
    public HSD_LevelModel LevelC1 { get; set; } = new HSD_LevelModel(12);
    public HSD_LevelModel LevelC2 { get; set; } = new HSD_LevelModel(13);
    public HSD_LevelModel LevelC3 { get; set; } = new HSD_LevelModel(14);

    public HSD_DraftModel()
    {
        LevelZ1 = new HSD_LevelModel(0);
        LevelA1 = new HSD_LevelModel(4);
        LevelA2 = new HSD_LevelModel(5);
        LevelA3 = new HSD_LevelModel(6);
        LevelB1 = new HSD_LevelModel(8);
        LevelB2 = new HSD_LevelModel(9);
        LevelB3 = new HSD_LevelModel(10);
        LevelC1 = new HSD_LevelModel(12);
        LevelC2 = new HSD_LevelModel(13);
        LevelC3 = new HSD_LevelModel(14);
        MakeAllAvailable();
    }
    
    public bool IsLevelAvailable(int level)
    {
        return level switch
        {
            0 => LevelZ1.IsAvailable,
            4 => LevelA1.IsAvailable,
            5 => LevelA2.IsAvailable,
            6 => LevelA3.IsAvailable,
            8 => LevelB1.IsAvailable,
            9 => LevelB2.IsAvailable,
            10 => LevelB3.IsAvailable,
            11 => LevelC1.IsAvailable,
            12 => LevelC2.IsAvailable,
            13 => LevelC3.IsAvailable,
            _ => throw new InvalidKoalaException(level)
        };
    }
    
    public void MakeAllAvailable()
    {
        AllowLevelSelect = true;
        LevelZ1.SetAvailability(true);
        LevelA1.SetAvailability(true);
        LevelA2.SetAvailability(true);
        LevelA3.SetAvailability(true);
        LevelB1.SetAvailability(true);
        LevelB2.SetAvailability(true);
        LevelB3.SetAvailability(true);
        LevelC1.SetAvailability(true);
        LevelC2.SetAvailability(true);
        LevelC3.SetAvailability(true);
    }
    
    public void SetAvailability(int level, bool value)
    {
        switch (level)
        {
            case 0:
                LevelZ1.SetAvailability(value);
                return;
            case 4:
                LevelA1.SetAvailability(value);
                return;
            case 5:
                LevelA2.SetAvailability(value);
                return;
            case 6:
                LevelA3.SetAvailability(value);
                return;
            case 8:
                LevelB1.SetAvailability(value);
                return;
            case 9:
                LevelB2.SetAvailability(value);
                return;
            case 10:
                LevelB3.SetAvailability(value);
                return;
            case 12:
                LevelC1.SetAvailability(value);
                return;
            case 13:
                LevelC2.SetAvailability(value);
                return;
            case 14:
                LevelC3.SetAvailability(value);
                return;
        }

        throw new InvalidKoalaException(level);
    }

    public async void LevelClicked(int level)
    {
        
    }
    
    public event Action<bool> OnBlockLevelSelectChanged;
    private bool allowLevelSelect;
    public bool AllowLevelSelect
    {
        get => allowLevelSelect;
        set
        {
            allowLevelSelect = value;
            OnBlockLevelSelectChanged?.Invoke(allowLevelSelect);
        }
    }
    
    public event Action<bool> OnSwapTeam1EnabledChanged;
    private bool swapTeam1Enabled;
    public bool SwapTeam1Enabled
    {
        get => swapTeam1Enabled;
        set
        {
            swapTeam1Enabled = value;
            OnSwapTeam1EnabledChanged?.Invoke(swapTeam1Enabled);
        }
    }
    
    public event Action<bool> OnSwapTeam2EnabledChanged;
    private bool swapTeam2Enabled;
    public bool SwapTeam2Enabled
    {
        get => swapTeam2Enabled;
        set
        {
            swapTeam2Enabled = value;
            OnSwapTeam2EnabledChanged?.Invoke(swapTeam2Enabled);
        }
    }
    
    public event Action<bool> OnReadyToStartChanged;
    private bool readyToStart;
    public bool ReadyToStart
    {
        get => readyToStart;
        set
        {
            readyToStart = value;
            OnReadyToStartChanged?.Invoke(readyToStart);
        }
    }
}
