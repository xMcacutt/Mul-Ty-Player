using MulTyPlayer;
using Riptide;

namespace MulTyPlayerServer;

public class CollectionModeHandler
{
    public static void HandleScoreUpdate(ushort fromClientId, string type, int iLive, int iSave)
    {
        var scoreIncrease = 0;
        switch (type)
        {
            case "Opal":
                scoreIncrease = 2;
                break;
            case "TE":
                scoreIncrease = iSave == 2 ? 150 : 50;                
                break;
            case "Cog":
                scoreIncrease = 40;
                break;
            case "Bilby":
                scoreIncrease = 75;
                break;
            case "Attribute":
                if (iSave < 16)
                    return;
                scoreIncrease = 200;
                break;
            case "RainbowScale":
                scoreIncrease = 100;
                break;
            case "Frame":
                scoreIncrease = 10;
                break;
            default:
                return;
        }

        if (!PlayerHandler.Players.TryGetValue(fromClientId, out Player p))
            return;

        p.Score += scoreIncrease;
        SendScore(p.Score, p.ClientID);
    }

    private static void SendScore(int score, ushort clientId)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_UpdateScore);
        message.AddInt(score);
        message.AddUShort(clientId);
        Server._Server.SendToAll(message);
    }

    [MessageHandler((ushort)MessageID.CL_ResetScore)]
    public static void HandleResetScores()
    {
        foreach (var player in PlayerHandler.Players.Values)
        {
            player.Score = 0;
            SendScore(0, player.ClientID);
        }
    }
}