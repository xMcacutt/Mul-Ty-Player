using System.Collections.Generic;

namespace MulTyPlayerClient;

public struct LevelData
{
    public int Id;
    public string Code;
    public string Name;
    public int CrateCount;
    public int CrateOpalCount;
    public int FrameCount;
    public bool IsMainStage;
    public bool HasKoalas;
}

public class Levels
{
    public static readonly LevelData RainbowCliffs = new()
    {
        Id = 0,
        Code = "Z1",
        Name = "Rainbow Cliffs",
        FrameCount = 9,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData TwoUp = new()
    {
        Id = 4,
        Code = "A1",
        Name = "Two Up",
        FrameCount = 7,
        CrateCount = 31,
        CrateOpalCount = 170,
        IsMainStage = true,
        HasKoalas = false
    };

    public static readonly LevelData WalkInThePark = new()
    {
        Id = 5,
        Code = "A2",
        Name = "Walk In The Park",
        FrameCount = 6,
        CrateCount = 18,
        CrateOpalCount = 102,
        IsMainStage = true,
        HasKoalas = false
    };

    public static readonly LevelData ShipRex = new()
    {
        Id = 6,
        Code = "A3",
        Name = "Ship Rex",
        FrameCount = 9,
        CrateCount = 24,
        CrateOpalCount = 119,
        IsMainStage = true,
        HasKoalas = false
    };

    public static readonly LevelData BullsPen = new()
    {
        Id = 7,
        Code = "A4",
        Name = "Bull's Pen",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData BridgeOnTheRiverTy = new()
    {
        Id = 8,
        Code = "B1",
        Name = "Bridge On The River Ty",
        FrameCount = 20,
        CrateCount = 24,
        CrateOpalCount = 120,
        IsMainStage = true,
        HasKoalas = false
    };

    public static readonly LevelData SnowWorries = new()
    {
        Id = 9,
        Code = "B2",
        Name = "Snow Worries",
        FrameCount = 24,
        CrateCount = 12,
        CrateOpalCount = 60,
        IsMainStage = true,
        HasKoalas = true
    };

    public static readonly LevelData OutbackSafari = new()
    {
        Id = 10,
        Code = "B3",
        Name = "Outback Safari",
        FrameCount = 0,
        CrateCount = 300,
        CrateOpalCount = 300,
        IsMainStage = true,
        HasKoalas = false
    };

    public static readonly LevelData CrikeysCove = new()
    {
        Id = 19,
        Code = "D4",
        Name = "Crikeys Cove",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData LyreLyrePantsOnFire = new()
    {
        Id = 12,
        Code = "C1",
        Name = "Lyre, Lyre Pants On Fire",
        FrameCount = 5,
        CrateCount = 6,
        CrateOpalCount = 30,
        IsMainStage = true,
        HasKoalas = false
    };

    public static readonly LevelData BeyondTheBlackStump = new()
    {
        Id = 13,
        Code = "C2",
        Name = "Beyond The Black Stump",
        FrameCount = 29,
        CrateCount = 34,
        CrateOpalCount = 170,
        IsMainStage = true,
        HasKoalas = true
    };

    public static readonly LevelData RexMarksTheSpot = new()
    {
        Id = 14,
        Code = "C3",
        Name = "Rex Marks The Spot",
        FrameCount = 18,
        CrateCount = 43,
        CrateOpalCount = 215,
        IsMainStage = true,
        HasKoalas = false
    };

    public static readonly LevelData FluffysFjord = new()
    {
        Id = 15,
        Code = "C4",
        Name = "Fluffy's Fjord",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData CassPass = new()
    {
        Id = 20,
        Code = "E1",
        Name = "Cass' Pass",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData CassCrest = new()
    {
        Id = 17,
        Code = "D2",
        Name = "Cass' Crest",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData FinalBattle = new()
    {
        Id = 23,
        Code = "E4",
        Name = "Final Battle",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData EndGame = new()
    {
        Id = 16,
        Code = "END",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData BonusWorld1 = new()
    {
        Id = 21,
        Code = "E2",
        Name = "Bonus World 1",
        FrameCount = 123,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly LevelData BonusWorld2 = new()
    {
        Id = 22,
        Code = "E3",
        Name = "Bonus World 2",
        FrameCount = 123,
        IsMainStage = false,
        HasKoalas = false
    };

    public static readonly Dictionary<int, LevelData> levels = new()
    {
        { 0, RainbowCliffs },

        { 4, TwoUp },
        { 5, WalkInThePark },
        { 6, ShipRex },
        { 7, BullsPen },

        { 8, BridgeOnTheRiverTy },
        { 9, SnowWorries },
        { 10, OutbackSafari },
        { 19, CrikeysCove },

        { 12, LyreLyrePantsOnFire },
        { 13, BeyondTheBlackStump },
        { 14, RexMarksTheSpot },
        { 15, FluffysFjord },

        { 16, EndGame },
        { 17, CassCrest },

        { 20, CassPass },
        { 21, BonusWorld1 },
        { 22, BonusWorld2 },
        { 23, FinalBattle }
    };

    public static readonly LevelData[] MainStages =
    {
        TwoUp, WalkInThePark, ShipRex,
        BridgeOnTheRiverTy, SnowWorries, OutbackSafari,
        LyreLyrePantsOnFire, BeyondTheBlackStump, RexMarksTheSpot
    };

    public static LevelData GetLevelData(int levelID)
    {
        return levels[levelID];
    }

    #region leveldata

    public static readonly LevelData RainbowCliffsDev = new()
    {
        Id = 1,
        Code = "Z2",
        FrameCount = 0,
        IsMainStage = false,
        HasKoalas = false
    };

    #endregion
}