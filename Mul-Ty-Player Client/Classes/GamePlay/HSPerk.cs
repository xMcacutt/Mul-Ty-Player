﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Classes.Views;
using PropertyChanged;
using Riptide;

namespace MulTyPlayerClient;

public class PerkHandler
{
    public static HSD_PerkWindow PerkDialog;
    
    public static readonly Dictionary<int, HSPerk> LevelPerks = new()
    {
        {0, new OpalSpeedPerk()},
        {4, new HiderGravityPerk()},
        {5, new SeekersFreezeHidersPerk()},
        {6, new SeekerSwimSpeedPerk()},
        {8, new HiderOneHitPerk()},
        {9, new SeekerGlideSpeedPerk()},
        {10, new SeekerOneHitPerk()},
        {12, new OpalSlowPerk()},
        {13, new HiderOneRangPerk()},
        {14, new SeekersSeeLightsPerk()}
    };

    public static readonly ObservableCollection<HSPerk> HiderPerks = new()
    {
        new HiderGravityPerk(),
        new HiderGlideSpeedPerk(),
        new OpalSpeedPerk(),
        new HidersSeeLightsPerk(),
        new HiderSwimSpeedPerk(),
        //new HidersFreezeSeekersPerk()
    };

    public static readonly ObservableCollection<HSPerk> HiderDebuffs = new()
    {
        new HiderOneHitPerk(),
        new OpalSlowPerk(),
        new HiderOneRangPerk()
    };

    public static readonly ObservableCollection<HSPerk> SeekerPerks = new()
    {
        new SeekerGravityPerk(),
        new SeekerGlideSpeedPerk(),
        new SeekersSeeLightsPerk(),
        new SeekersFreezeHidersPerk(),
        new SeekerSwimSpeedPerk(),
        new OpalSpeedPerk()
    };
    
    public static readonly ObservableCollection<HSPerk> SeekerDebuffs = new()
    {
        new SeekerOneHitPerk(),
        new OpalSlowPerk(),
        new SeekerOneRangPerk()
    };

    public static void DeactivateAllPerks()
    {
        foreach (var perk in LevelPerks.Values)
            perk.Deactivate();
    }
}

[AddINotifyPropertyChangedInterface]
public abstract class HSPerk
{
    public string DisplayName { get; set; }
    public string ToolTip { get; set; }

    public virtual void ApplyHider() {}
    public virtual void ApplySeeker() {}
    public virtual void ApplyTime() {}
    public virtual void ApplyAbility() {}
    public virtual void Deactivate() {}
}

public class NoPerk : HSPerk
{
    public NoPerk()
    {
        DisplayName = "None";
        ToolTip = "No perk will be applied.";
    }
}

public class OpalSlowPerk : HSPerk
{
    public OpalSlowPerk()
    {
        DisplayName = "Opal Slowness";
        ToolTip = "Every opal collected will slow you down.";
    }
    
    public override void ApplyHider()
    {
        var opalCount = CountOpals();
        var divisor = Client.HLevel.CurrentLevelId == 0 ? 10f : 42.9f;
        var speed = 10f - opalCount / divisor;
        Client.HHero.SetRunSpeed(speed);
    }

    public override void ApplySeeker()
    {
        var opalCount = CountOpals();
        var divisor = Client.HLevel.CurrentLevelId == 0 ? 10.4f : 50f;
        var speed = 10.15f - opalCount / divisor;
        Client.HHero.SetRunSpeed(speed);
    }

    public override void Deactivate()
    {
        switch (Client.HHideSeek.Role)
        {
            case HSRole.Hider:
                Client.HHero.SetRunSpeed();
                break;
            case HSRole.Seeker:
                Client.HHero.SetRunSpeed(10.15f);
                break;
            case HSRole.Spectator:
                break;
        }
    }

    private int CountOpals()
    {
        ProcessHandler.TryRead(0x26547C, out int count, true, "ReadOpalCount");
        return count;
    }
}

public class OpalSpeedPerk : HSPerk
{
    public OpalSpeedPerk()
    {
        DisplayName = "Opal Speed";
        ToolTip = "Every opal collected will speed you up.";
    }
    
    public override void ApplyHider()
    {
        var opalCount = CountOpals();
        var divisor = Client.HLevel.CurrentLevelId == 0 ? 5f : 60f;
        var speed = 10f + opalCount / divisor;   
        Client.HHero.SetRunSpeed(speed);
    }

    public override void ApplySeeker()
    {
        var opalCount = CountOpals();
        var divisor = Client.HLevel.CurrentLevelId == 0 ? 5.2f : 62.5f;
        var speed = 10.15f + opalCount / divisor;
        Client.HHero.SetRunSpeed(speed);
    }

    public override void Deactivate()
    {
        switch (Client.HHideSeek.Role)
        {
            case HSRole.Hider:
                Client.HHero.SetRunSpeed();
                break;
            case HSRole.Seeker:
                Client.HHero.SetRunSpeed(10.15f);
                break;
            case HSRole.Spectator:
                break;
        }
    }

    private int CountOpals()
    {
        ProcessHandler.TryRead(0x26547C, out int count, true, "ReadOpalCount");
        return count;
    }
}

public class SeekerGravityPerk : HSPerk
{
    public SeekerGravityPerk()
    {
        DisplayName = "Low Gravity";
        ToolTip = "Gravity has less of an effect.";
    }
    public override void ApplySeeker()
    {
        Client.HHero.SetGravity(0.45f);
    }

    public override void Deactivate()
    {
        Client.HHero.SetGravity();
    }
}

public class HiderGravityPerk : HSPerk
{
    public HiderGravityPerk()
    {
        DisplayName = "Low Gravity";
        ToolTip = "Gravity has less of an effect.";
    }
    public override void ApplyHider()
    {
        Client.HHero.SetGravity(0.45f);
    }

    public override void Deactivate()
    {
        Client.HHero.SetGravity();
    }
}

public class SeekerSwimSpeedPerk : HSPerk
{
    public SeekerSwimSpeedPerk()
    {
        DisplayName = "Swim Speed";
        ToolTip = "Swim speed is increased.";
    }
    public override void ApplySeeker()
    {
        Client.HHero.SetSwimSpeed(23f);
    }
    
    public override void Deactivate()
    {
        Client.HHero.SetSwimSpeed();
    }
}

public class HiderSwimSpeedPerk : HSPerk
{
    public HiderSwimSpeedPerk()
    {
        DisplayName = "Swim Speed";
        ToolTip = "Swim speed is increased.";
    }
    public override void ApplyHider()
    {
        Client.HHero.SetSwimSpeed(23f);
    }

    public override void Deactivate()
    {
        Client.HHero.SetSwimSpeed();
    }
}

public class HiderGlideSpeedPerk : HSPerk
{
    public HiderGlideSpeedPerk()
    {
        DisplayName = "Glide Speed";
        ToolTip = "Glide speed is increased.";
    }
    public override void ApplyHider()
    {
        Client.HHero.SetGlideSpeed(11.5f);
    }

    public override void Deactivate()
    {
        Client.HHero.SetGlideSpeed();
    }
}

public class SeekerGlideSpeedPerk : HSPerk
{
    public SeekerGlideSpeedPerk()
    {
        DisplayName = "Glide Speed";
        ToolTip = "Glide speed is increased.";
    }
    public override void ApplySeeker()
    {
        Client.HHero.SetGlideSpeed(11.5f);
    }

    public override void Deactivate()
    {
        Client.HHero.SetGlideSpeed();
    }
}

public class SeekerOneHitPerk : HSPerk
{
    public SeekerOneHitPerk()
    {
        DisplayName = "One Health";
        ToolTip = "Health is lowered to 1 (including in water).";
    }
    public override void ApplySeeker()
    {
        if (Client.HHero.GetHealth() > 1)
            Client.HHero.SetHealth(1);
        if (Client.HHero.GetWaterHealth() > 1)
            Client.HHero.SetWaterHealth(1);
        if (Client.HHero.GetOutbackHealth() > 1)
            Client.HHero.SetOutbackHealth(1);
    }

    public override void Deactivate()
    {
        Client.HHero.SetHealth(4);
    }
}

public class HiderOneHitPerk : HSPerk
{
    public HiderOneHitPerk()
    {
        DisplayName = "One Health";
        ToolTip = "Health is lowered to 1 (including in water).";
    }
    public override void ApplyHider()
    {
        if (Client.HHero.GetHealth() > 1)
            Client.HHero.SetHealth(1);
        if (Client.HHero.GetWaterHealth() > 1)
            Client.HHero.SetWaterHealth(1);
        if (Client.HHero.GetOutbackHealth() > 1)
            Client.HHero.SetOutbackHealth(1);
    }

    public override void Deactivate()
    {
        Client.HHero.SetHealth(4);
    }
}

public class SeekerOneRangPerk : HSPerk
{
    public SeekerOneRangPerk()
    {
        DisplayName = "One Rang";
        ToolTip = "Second boomerang is removed (this applies to all rangs).";
    }
    public override void ApplySeeker()
    {
        var rangAddress = PointerCalculations.GetPointerAddress(0x288730, new[] { 0 });
        ProcessHandler.WriteData(rangAddress + 0xAB6, new byte[] { 0x0 }, "Remove Second Rang");
    }

    public override void Deactivate()
    {
        var rangAddress = PointerCalculations.GetPointerAddress(0x288730, new[] { 0 });
        ProcessHandler.WriteData(rangAddress, new byte[] { 0x1 }, "Remove Second Rang");
    }
}

public class HiderOneRangPerk : HSPerk
{
    public HiderOneRangPerk()
    {
        DisplayName = "One Rang";
        ToolTip = "Second rang is removed (this applies to all rangs).";
    }
    public override void ApplyHider()
    {
        var rangAddress = PointerCalculations.GetPointerAddress(0x288730, new[] { 0 });
        ProcessHandler.WriteData(rangAddress + 0xAB6, new byte[] { 0x0 }, "Remove Second Rang");
    }

    public override void Deactivate()
    {
        var rangAddress = PointerCalculations.GetPointerAddress(0x288730, new[] { 0 });
        ProcessHandler.WriteData(rangAddress, new byte[] { 0x1 }, "Remove Second Rang");
    }
}

public class HidersSeeLightsPerk : HSPerk
{
    public HidersSeeLightsPerk()
    {
        DisplayName = "Player Lights";
        ToolTip = "Press ctrl+shift+t to show opposing players lights for 3 seconds. This can be done once every 30 seconds.";
    }
    public override void ApplyAbility()
    {
        if (Client.HHideSeek.Role != HSRole.Hider)
            return;
        Client.HHideSeek.LinesVisible = true;
    }

    public override void Deactivate()
    {
        Client.HHideSeek.LinesVisible = false;
    }
}

public class SeekersSeeLightsPerk : HSPerk
{
    public SeekersSeeLightsPerk()
    {
        DisplayName = "Player Lights";
        ToolTip = "Press ctrl+shift+t to show opposing players' lights for 3 seconds. This can be done once every 30 seconds.";
    }
    public override void ApplyAbility()
    {
        if (Client.HHideSeek.Role != HSRole.Seeker)
            return;
        var thread = new Thread(ShowLines);
        thread.Start();
    }

    private async void ShowLines()
    {
        Client.HHideSeek.LinesVisible = true;
        await Task.Delay(3000);
        Client.HHideSeek.LinesVisible = false;
    }

    public override void Deactivate()
    {
        Client.HHideSeek.LinesVisible = false;
    }
}

public class SeekersFreezeHidersPerk : HSPerk
{
    public SeekersFreezeHidersPerk()
    {
        DisplayName = "Freeze Players";
        ToolTip = "Press ctrl+shift+t to freeze opposing players in place for 3 seconds. This can be done once every 30 seconds.";
    }
    public override void ApplyAbility()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.HS_Freeze);
        Client._client.Send(message);
    }
    
    private static async void Freeze()
    {
        if (Client.HHideSeek.Role != HSRole.Hider)
            return;
        SFXPlayer.PlaySound(SFX.Freeze);
        ProcessHandler.WriteData(
            (int)TyProcess.BaseAddress + 0x264248, 
            BitConverter.GetBytes(1f));
        await Task.Delay(3000);
        ProcessHandler.WriteData(
            (int)TyProcess.BaseAddress + 0x264248, 
            BitConverter.GetBytes(0.01f));
        SFXPlayer.PlaySound(SFX.Unfreeze);
    }


    [MessageHandler((ushort)MessageID.HS_Freeze)]
    public static void ReceiveFreeze(Message message)
    {
        var thread = new Thread(Freeze);
        thread.Start();
    }
    
    public override void Deactivate()
    {
        ProcessHandler.WriteData(
            (int)TyProcess.BaseAddress + 0x264248, 
            BitConverter.GetBytes(0.01f));
    }
}