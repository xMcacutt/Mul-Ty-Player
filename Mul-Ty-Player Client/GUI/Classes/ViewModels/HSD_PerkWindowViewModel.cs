using System;
using System.Collections.Generic;
using System.Linq;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HSD_PerkWindowViewModel
{
    public bool ChoosePerkActive { get; set; }
    public HSPerk Perk1 { get; set; }
    public HSPerk Perk2 { get; set; }
    public HSPerk Perk3 { get; set; }
    public HSPerk Debuff1 { get; set; }
    public HSPerk Debuff2 { get; set; }
    public HSPerk Debuff3 { get; set; }

    private Random _rand = new Random();
    
    public HSD_PerkWindowViewModel()
    {
        ChoosePerkActive = true;
        if (Client.HHideSeek.Role == HSRole.Hider)
        {
            var uniquePerks = new HashSet<HSPerk>(); 
            var uniqueDebuffs = new HashSet<HSPerk>(); 
            while (uniquePerks.Count < 3)
            {
                var perk = PerkHandler.HiderPerks[_rand.Next(PerkHandler.HiderPerks.Count)];
                uniquePerks.Add(perk);
            }
            var perkArray = uniquePerks.ToArray();
            Perk1 = perkArray[0];
            Perk2 = perkArray[1];
            Perk3 = perkArray[2];
            while (uniqueDebuffs.Count < 3)
            {
                var perk = PerkHandler.HiderDebuffs[_rand.Next(PerkHandler.HiderDebuffs.Count)];
                uniqueDebuffs.Add(perk);
            }
            var debuffArray = uniqueDebuffs.ToArray();
            Debuff1 = debuffArray[0];
            Debuff2 = debuffArray[1];
            Debuff3 = debuffArray[2];
        }
        else
        {
            var uniquePerks = new HashSet<HSPerk>(); 
            var uniqueDebuffs = new HashSet<HSPerk>(); 
            while (uniquePerks.Count < 3)
            {
                var perk = PerkHandler.SeekerPerks[_rand.Next(PerkHandler.SeekerPerks.Count)];
                uniquePerks.Add(perk);
            }
            var perkArray = uniquePerks.ToArray();
            Perk1 = perkArray[0];
            Perk2 = perkArray[1];
            Perk3 = perkArray[2];
            while (uniqueDebuffs.Count < 3)
            {
                var perk = PerkHandler.SeekerDebuffs[_rand.Next(PerkHandler.SeekerDebuffs.Count)];
                uniqueDebuffs.Add(perk);
            }
            var debuffArray = uniqueDebuffs.ToArray();
            Debuff1 = debuffArray[0];
            Debuff2 = debuffArray[1];
            Debuff3 = debuffArray[2];
        }
    }
}