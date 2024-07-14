using System;

namespace MulTyPlayerClient.GUI.Models;

public class HSD_PickModel
{
    public HSD_Pick Pick;
    public HSD_Team Team;
    public int LevelId;
    public Uri BanSource;
    public Uri PickSource;

    public HSD_PickModel(int levelId, HSD_Team team, HSD_Pick pick)
    {
        Pick = pick;
        Team = team;
        LevelId = levelId;
        BanSource = new Uri(@$"pack://siteoforigin:,,,/GUI/HS_DraftAssets/{LevelId}bw.png");
        PickSource = new Uri(@$"pack://siteoforigin:,,,/GUI/HS_DraftAssets/{LevelId}.png");
    }
}

public enum HSD_Pick
{
    Pick,
    Ban
}