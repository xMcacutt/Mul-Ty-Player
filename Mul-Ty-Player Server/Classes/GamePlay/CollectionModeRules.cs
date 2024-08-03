using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MulTyPlayer;
using MulTyPlayerServer.Classes.Networking.Commands;
using Riptide;

namespace MulTyPlayerServer;

public class CollectionModeSettings
{
    private static Dictionary<string, int> defaultScores = new Dictionary<string, int>
    {
        { "Opal", 3 },
        { "BaseThunderEgg", 50 },
        { "BilbyEgg", -25 },
        { "TimeAttackEgg", 75 },
        { "OpalEgg", -20 },
        { "Cog", 40 },
        { "Bilby", 75 },
        { "Talisman", 200 },
        { "RainbowScale", 100 },
        { "PictureFrame", 10 },
        { "Crate", 0 },
    };

    public static Dictionary<string, int> Scores = new Dictionary<string, int>(defaultScores);
    
    public static void ResetToDefaults()
    {
        Scores = new Dictionary<string, int>(defaultScores);
    }
}

public enum EggTypes
{
    Opal,
    Bilby,
    TimeAttack,
    MainObjective
}

public abstract class CollectionModeRule
{
    public string Name;
    public string Description;
    public virtual void ChangeRule() { }
    public virtual void Intercept(int level, string type, int iLive, int iSave, ref int score) { }
    public virtual bool InterceptSend(ushort originalClientId, string type, int score) { return false; }
    public virtual void RunSpecialAction() { }
    public virtual void RunSpecialEndAction() { }
}

public class CollectionModeRuleHandler
{
    private CollectionModeRule _currentRule;
    public CollectionModeRule CurrentRule
    {
        get => _currentRule;
        set
        {
            _currentRule = value;
            CollectionModeSettings.ResetToDefaults();
            CurrentRule.ChangeRule();
            if (CurrentRule is ClmRule_NoRule)
                return;
            AnnounceRuleChange(value);
        }
    }

    private void AnnounceRuleChange(CollectionModeRule rule)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_RuleChange);
        message.AddString(rule.Name);
        message.AddString(rule.Description);
        Server._Server.SendToAll(message);
    }

    public CollectionModeRuleHandler()
    {
        CurrentRule = new ClmRule_NoRule();
    }

    public CollectionModeRule[] Rules = new CollectionModeRule[]
    {
        new ClmRule_DoubleCogs(), // Cogs are worth double
        new ClmRule_OpalsAreBad(), // Opals give negative points
        new ClmRule_FrameCount(), // Frames give points equal to level count
        new ClmRule_Swapsies(), // Swap player positions
        new ClmRule_Tasty(), // Opals are worth 1 more point
        new ClmRule_GoldenGoose(), // Turkey and turkey adjacent eggs give double points
        new ClmRule_BilbyDeduct(), // Bilbies deduct points from your opponents
        new ClmRule_Run(), // At the next rule change, get points corresponding to how far you are from where you are now.
        new ClmRule_Dead(), // Die
        new ClmRule_Crate(), // Crates aren't useless now.
        new ClmRule_Half(), // Your points are gone 
        new ClmRule_Virus(), // Your points might go to someone else
        new ClmRule_Nopals(), // They gone
    };
}

public class ClmRule_NoRule : CollectionModeRule
{
    public ClmRule_NoRule()
    {
        Name = "No Rule";
        Description = "No rules are active.";
    }
}

public class ClmRule_OpalsAreBad : CollectionModeRule
{
    public ClmRule_OpalsAreBad()
    {
        Name = "These Taste Bad";
        Description = "Opals now give negative points.";
    }
    public override void ChangeRule()
    {
        CollectionModeSettings.Scores["Opal"] = -CollectionModeSettings.Scores["Opal"];
    }
}

public class ClmRule_DoubleCogs : CollectionModeRule
{
    public ClmRule_DoubleCogs()
    {
        Name = "Gold Rush";
        Description = "Cogs now give double points.";
    }
    public override void ChangeRule()
    {
        CollectionModeSettings.Scores["Cog"] *= 2;
    }
}

public class ClmRule_FrameCount : CollectionModeRule
{
    public ClmRule_FrameCount()
    {
        Name = "You've Been Framed";
        Description = "Picture Frames give points equal to the total number of them for the current level.";
    }
    public override void Intercept(int level, string type, int iLive, int iSave, ref int score)
    {
        if (type != "Frame")
            return;
        if (!SyncHandler.SFrame.GlobalObjectData.TryGetValue(level, out var frameArray))
            return;
        score = frameArray.Length;
    }
}

public class ClmRule_BilbyDeduct : CollectionModeRule
{
    public ClmRule_BilbyDeduct()
    {
        Name = "Bilby Tax";
        Description = "Bilbies cause the other players to lose points.";
    }

    public override bool InterceptSend(ushort originalClientId, string type, int score)
    {
        if (type != "Bilby")
            return false;
        foreach (var player in PlayerHandler.Players.Values.Where(x =>
                     x.ClientID != originalClientId && x.Role != HSRole.Spectator
                 ))
        {
            player.Score -= 20;
            CollectionModeHandler.SendScore(player.Score, player.ClientID);
        }

        return true;
    }
}

public class ClmRule_GoldenGoose : CollectionModeRule
{
    public ClmRule_GoldenGoose()
    {
        Name = "Golden Goose";
        Description = "Birds lay eggs right? Yeah those ones give double points.";
    }
    public override void Intercept(int level, string type, int iLive, int iSave, ref int score)
    {
        if (type != "TE")
            return;
        switch (level)
        {
                // Turkey Egg Walk
            case 5 when iSave == 6:
                // Turkey Egg Bridge
            case 8 when iSave == 5:
                // Boonie Catch Stump
            case 13 when iSave == 6:
                // Emus Outback
            case 10 when iSave == 3:
                // Nest Egg Ship
            case 6 when iSave == 7:
                // Sly Lyre
            case 12 when iSave == 3:
                // PARROT Beard Rex Marks    
            case 14 when iSave == 4:
                score *= 2;
                break;
        }
    }
}

public class ClmRule_Tasty : CollectionModeRule
{
    public ClmRule_Tasty()
    {
        Name = "Tasty";
        Description = "Opals are worth an extra point.";
    }
    public override void ChangeRule()
    {
        CollectionModeSettings.Scores["Opal"] += 1;
    }
}

public class ClmRule_Swapsies : CollectionModeRule
{
    public ClmRule_Swapsies()
    {
        Name = "Swapsies";
        Description = "Time to trade places!\nYou're now where you weren't but where they were but are not now.";
    }
    public override void RunSpecialAction()
    {
        var players = PlayerHandler.Players.Values.Where(x => x.Role != HSRole.Spectator).ToArray();
        var playerCoordinates = players.Select(player => player.Coordinates.Take(3).Select(coord => coord.ToString()).ToArray()).ToArray();
        for (var pIndex = 0; pIndex < players.Length; pIndex++)
        {
            var player = players[pIndex];
            MtpCommandTeleport.Teleport(true, player.ClientID,
                playerCoordinates[(pIndex + 1) % players.Length], 65535);
        }
    }
}

public class ClmRule_Run : CollectionModeRule
{
    public ClmRule_Run()
    {
        Name = "Run!";
        Description = "At the next rule change get points corresponding to how far you are from where you are now.";
    }

    private Dictionary<ushort, float[]> _playerCoordinates;

    public override void RunSpecialEndAction()
    {
        if (_playerCoordinates is null)
            return;

        foreach (var coords in _playerCoordinates)
        {
            if (!PlayerHandler.Players.TryGetValue(coords.Key, out var player))
                continue;
            var oldPlayerVector = new Vector3(
                _playerCoordinates[player.ClientID][0], 
                _playerCoordinates[player.ClientID][1],
                _playerCoordinates[player.ClientID][2]);
            var newPlayerVector = new Vector3(
                player.Coordinates[0], 
                player.Coordinates[1], 
                player.Coordinates[2]);
            player.Score += (int)Math.Round(Vector3.Distance(oldPlayerVector, newPlayerVector), 0) / 50;
            CollectionModeHandler.SendScore(player.Score, player.ClientID);
        }
    }
    
    public override void RunSpecialAction()
    {
        var players = PlayerHandler.Players.Where(x => x.Value.Role != HSRole.Spectator);
        _playerCoordinates = players.ToDictionary(
            x => x.Key, x => (float[])x.Value.Coordinates.Clone());
    }
}

public class ClmRule_Dead : CollectionModeRule
{
    public ClmRule_Dead()
    {
        Name = "Dead";
        Description = "Oops, that looked nasty!";
    }
    public override void RunSpecialAction()
    {
        foreach(var p in PlayerHandler.Players.Values.Where(x => x.Role != HSRole.Spectator))  
            PlayerHandler.KillPlayer(p.ClientID);
    }
}

public class ClmRule_Crate : CollectionModeRule
{
    public ClmRule_Crate()
    {
        Name = "Pandora's Box";
        Description = "Crates give points... but sadly for you, no hope.";
    }

    public override void ChangeRule()
    {
        CollectionModeSettings.Scores["Crate"] = 15;
    }
}

public class ClmRule_Half : CollectionModeRule
{
    public ClmRule_Half()
    { 
        Name = "Half Points"; 
        Description = "Check your total... it's half now."; 
    }

    public override void RunSpecialAction()
    {
        foreach (var p in PlayerHandler.Players.Values.Where(x => x.Role != HSRole.Spectator))
        {
            p.Score /= 2;
            CollectionModeHandler.SendScore(p.Score, p.ClientID);
        }
    }
}


public class ClmRule_Virus : CollectionModeRule
{
    private Random _random;
    public ClmRule_Virus()
    {
        _random = new Random();
        Name = "Malware Detected"; 
        Description = "Uh oh! Points have a chance to go to your opponents instead."; 
    }
    
    public override bool InterceptSend(ushort originalClientId, string type, int score)
    {
        var toRandom = _random.Next(10) > 8;
        Player player = null;

        if (toRandom)
        {
            var eligiblePlayers = PlayerHandler.Players.Values
                .Where(x => x.Role != HSRole.Spectator && x.ClientID != originalClientId)
                .ToList();
            player = eligiblePlayers.Any() ? eligiblePlayers[_random.Next(eligiblePlayers.Count)] : null;
        }
        if (player == null && !PlayerHandler.Players.TryGetValue(originalClientId, out player))
            return false;

        player.Score += score;
        CollectionModeHandler.SendScore(player.Score, player.ClientID);
        return true;
    }
}

public class ClmRule_Nopals : CollectionModeRule
{
    private Random _random;
    public ClmRule_Nopals()
    {
        _random = new Random();
        Name = "Nopals"; 
        Description = "Where did they go?"; 
    }

    public override void RunSpecialAction()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_Nopals);
        message.AddBool(true);
        Server._Server.SendToAll(message);
    }

    public override void RunSpecialEndAction()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_Nopals);
        message.AddBool(false);
        Server._Server.SendToAll(message);
    }
}

