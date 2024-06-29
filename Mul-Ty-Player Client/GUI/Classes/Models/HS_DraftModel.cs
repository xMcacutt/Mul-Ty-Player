using System;

namespace MulTyPlayerClient.GUI.Models;
/*
public class HS_DraftModel
{
    public HS_PlayerModel Team1Player1 { get; set; }
    public HS_PlayerModel Team1Player2 { get; set; }
    public HS_PlayerModel Team1Player3 { get; set; }
    public HS_PlayerModel Team1Player4 { get; set; }
    public HS_PlayerModel Team2Player1 { get; set; }
    public HS_PlayerModel Team2Player2 { get; set; }
    public HS_PlayerModel Team2Player3 { get; set; }
    public HS_PlayerModel Team2Player4 { get; set; }
    
    public HS_PickModel Ban1 { get; set; }
    public HS_PickModel Ban2 { get; set; }
    public HS_PickModel Ban3 { get; set; }
    public HS_PickModel Ban4 { get; set; }
    public HS_PickModel Pick1 { get; set; }
    public HS_PickModel Pick2 { get; set; }
    public HS_PickModel Pick3 { get; set; }
    public HS_PickModel Pick4 { get; set; }
    
    public HS_LevelModel LevelZ1 { get; set; } = new HS_LevelModel(0);
    public HS_LevelModel LevelA1 { get; set; } = new HS_LevelModel(4);
    public HS_LevelModel LevelA2 { get; set; } = new HS_LevelModel(5);
    public HS_LevelModel LevelA3 { get; set; } = new HS_LevelModel(6);
    public HS_LevelModel LevelB1 { get; set; } = new HS_LevelModel(8);
    public HS_LevelModel LevelB2 { get; set; } = new HS_LevelModel(9);
    public HS_LevelModel LevelB3 { get; set; } = new HS_LevelModel(10);
    public HS_LevelModel LevelC1 { get; set; } = new HS_LevelModel(12);
    public HS_LevelModel LevelC2 { get; set; } = new HS_LevelModel(13);
    public HS_LevelModel LevelC3 { get; set; } = new HS_LevelModel(14);

    public HS_DraftModel()
    {
        LevelZ1 = new HS_LevelModel(0);
        LevelA1 = new HS_LevelModel(4);
        LevelA2 = new HS_LevelModel(5);
        LevelA3 = new HS_LevelModel(6);
        LevelB1 = new HS_LevelModel(8);
        LevelB2 = new HS_LevelModel(9);
        LevelB3 = new HS_LevelModel(10);
        LevelC1 = new HS_LevelModel(12);
        LevelC2 = new HS_LevelModel(13);
        LevelC3 = new HS_LevelModel(14);
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
        BlockLevelSelect = false;
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
}
*/