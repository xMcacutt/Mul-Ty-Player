using System;

namespace MulTyPlayerClient.GUI.Models;

public class HSD_PlayerModel
{
    public HSD_PlayerModel(Player player)
    {
        if (player is { Koala: not null })
        {
            Koala = Koalas.GetInfo[(Koala)player.Koala];
            KoalaImageSource = new Uri(@$"pack://siteoforigin:,,,/GUI/HS_DraftAssets/{Koala?.Name}.png");
            PlayerName = player.Name;
            Id = player.Id;
        }
        else
            KoalaImageSource = null;
    }

    public ushort Id { get; private set; }
    public KoalaInfo Koala { get; private set; }
    public Uri KoalaImageSource { get; private set; }
    public string PlayerName { get; private set; }
}