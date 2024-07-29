using System.Timers;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient.Classes.GamePlay;

public class CollectionModeHandler
{
    [MessageHandler((ushort)MessageID.CL_UpdateScore)]
    public static void HandleScoreUpdate(Message message)
    {
        var score = message.GetInt();
        var client = message.GetUShort();
        if (!PlayerHandler.TryGetPlayer(client, out var player))
            return;
        player.Score = score;
    }

    [MessageHandler((ushort)MessageID.CL_RuleChange)]
    public static void HandleRuleChange(Message message)
    {
        SFXPlayer.PlaySound(SFX.RuleChange);
        var name = message.GetString();
        var description = message.GetString();
        Client.HGameState.DisplayInGameMessage(name + "\n" + description, 5);
        Logger.Write($"[CLM] Rule Changed: {name}");
        Logger.Write(description);
    }

    [MessageHandler((ushort)MessageID.CL_Start)]
    public static void HandleClmStart(Message message)
    {
        foreach (var entry in PlayerHandler.Players) 
            entry.IsReady = false;
        ModelController.Lobby.IsReady = false;
        ModelController.Lobby.IsReadyButtonEnabled = false;
        Client.HCommand.Commands["tp"].InitExecute(new string[] {"@s"});
        SFXPlayer.PlaySound(SFX.RuleChange);
        Client.HGameState.DisplayInGameMessage("Collection Mode Started\nGo!", 5);
        ResetScores();
    }
    
    [MessageHandler((ushort)MessageID.CL_Stop)]
    public static void HandleClmStop(Message message)
    {
        ModelController.Lobby.IsReadyButtonEnabled = true;
        SFXPlayer.PlaySound(SFX.RuleChange);
        SFXPlayer.PlaySound(SFX.TAOpen);
        Client.HGameState.DisplayInGameMessage("The Collection Mode Round Has Ended");
    }

    public static void ResetScores()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_ResetScore);
        Client._client.Send(message);
    }

    public static void RequestAbort()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_Stop);
        Client._client.Send(message);
    }
}