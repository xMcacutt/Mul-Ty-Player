using MulTyPlayer;
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

    public static void ResetScores()
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.CL_ResetScore);
        Client._client.Send(message);
    }
}