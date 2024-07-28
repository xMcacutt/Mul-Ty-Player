using System;
using System.Timers;
using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

public class CollectionModeHandler
{
    private CollectionModeRuleHandler _clmRuleHandler;
    private static Random _rand;
    private static bool _isClmRunning = false;
    
    public CollectionModeHandler()
    {
        _clmRuleHandler = new CollectionModeRuleHandler();
        _rand = new Random();
    }
    
    public static void HandleScoreUpdate(ushort fromClientId, int level, string type, int iLive, int iSave)
    {
        if (!_isClmRunning)
            return;
        var scoreIncrease = 0;
        switch (type)
        {
            case "Opal":
                scoreIncrease = CollectionModeSettings.Scores["Opal"];
                break;
            case "TE":
                scoreIncrease += CollectionModeSettings.Scores["BaseThunderEgg"];
                switch (iSave)
                {
                    case (int)EggTypes.Opal:
                        scoreIncrease += CollectionModeSettings.Scores["OpalEgg"];
                        break;
                    case (int)EggTypes.Bilby:
                        scoreIncrease += CollectionModeSettings.Scores["BilbyEgg"];
                        break;
                    case (int)EggTypes.TimeAttack:
                        scoreIncrease += CollectionModeSettings.Scores["TimeAttackEgg"];
                        break;
                }
                break;
            case "Cog":
                scoreIncrease = CollectionModeSettings.Scores["Cog"];
                break;
            case "Bilby":
                scoreIncrease = CollectionModeSettings.Scores["Bilby"];
                break;
            case "Attribute":
                // Ignore everything except talismans
                if (iSave < 16)
                    return;
                scoreIncrease = CollectionModeSettings.Scores["Talisman"];
                break;
            case "RainbowScale":
                scoreIncrease = CollectionModeSettings.Scores["RainbowScale"];
                break;
            case "Frame":
                scoreIncrease = CollectionModeSettings.Scores["PictureFrame"];
                break;
            default:
                return;
        }
        
        Program.HCollection._clmRuleHandler.CurrentRule.Intercept(level, type, iLive, iSave, ref scoreIncrease);

        if (Program.HCollection._clmRuleHandler.CurrentRule.InterceptSend(fromClientId, type))
            return;
        
        if (!PlayerHandler.Players.TryGetValue(fromClientId, out Player p))
            return;

        p.Score += scoreIncrease;
        SendScore(p.Score, p.ClientID);
    }

    public static void SendScore(int score, ushort clientId)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_UpdateScore);
        message.AddInt(score);
        message.AddUShort(clientId);
        Server._Server.SendToAll(message);
    }

    [MessageHandler((ushort)MessageID.CL_ResetScore)]
    public static void HandleResetScores(ushort fromClientId, Message message)
    {
        foreach (var player in PlayerHandler.Players.Values)
        {
            player.Score = 0;
            SendScore(0, player.ClientID);
        }
    }
    
    [MessageHandler((ushort)MessageID.CL_Stop)]
    public static void HandleAbortRequest(ushort fromClientId, Message message)
    {
        Program.HCollection.StopCollectionMode();
    }
    
    private static Timer _mainTimer;
    private static Timer _intervalTimer;

    public static void RunCollectionMode()
    {
        Program.HCollection.DisposeOfTimers();
        _mainTimer = new Timer(5 * 60 * 1000);
        _intervalTimer = new Timer(45 * 1000);
        _mainTimer.Elapsed += OnMainTimerElapsed;
        _mainTimer.AutoReset = false;
        _mainTimer.Start();
        _intervalTimer.Elapsed += OnIntervalTimerElapsed;
        _intervalTimer.AutoReset = true;
        _intervalTimer.Start();
        _isClmRunning = true;
        Program.HCollection._clmRuleHandler.CurrentRule = new ClmRule_NoRule();
        var startClmMessage = Message.Create(MessageSendMode.Reliable, MessageID.CL_Start);
        Server._Server.SendToAll(startClmMessage);
    }

    public void DisposeOfTimers()
    {
        if (_mainTimer != null)
        {
            _mainTimer.Stop();
            _mainTimer.Dispose();
            _mainTimer = null;
        }
        if (_intervalTimer != null)
        {
            _intervalTimer.Stop();
            _intervalTimer.Dispose();
            _intervalTimer = null;
        }
    }

    public void StopCollectionMode()
    {
        DisposeOfTimers();
        _isClmRunning = false;
        var stopClmMessage = Message.Create(MessageSendMode.Reliable, MessageID.CL_Stop);
        Server._Server.SendToAll(stopClmMessage);
    }
    
    private static void OnMainTimerElapsed(object sender, ElapsedEventArgs e)
    {
        _mainTimer.Dispose();
        _intervalTimer.Dispose();
        Program.HCollection.StopCollectionMode();
    }

    private static void OnIntervalTimerElapsed(object sender, ElapsedEventArgs e)
    {
        Program.HCollection._clmRuleHandler.CurrentRule.RunSpecialEndAction();
        var randomRuleIndex = _rand.Next(Program.HCollection._clmRuleHandler.Rules.Length - 1);
        Program.HCollection._clmRuleHandler.CurrentRule = Program.HCollection._clmRuleHandler.Rules[randomRuleIndex];
        Program.HCollection._clmRuleHandler.CurrentRule.RunSpecialAction();
    }
}

