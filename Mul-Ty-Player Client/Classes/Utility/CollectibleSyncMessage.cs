using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

internal class CollectibleSyncMessage
{
    public int LiveIndex;
    public int SaveIndex;
    public int Level;
    public string Type;

    public CollectibleSyncMessage(int iLive, int iSave, int level, string type)
    {
        LiveIndex = iLive;
        SaveIndex = iSave;
        Level = level;
        Type = type;
    }

    public static CollectibleSyncMessage Create(int iLive, int iSave, int level, string type)
    {
        return new CollectibleSyncMessage(iLive, iSave, level, type);
    }

    public static Message Encode(CollectibleSyncMessage syncMessage)
    {
        var riptideMessage = Message.Create(MessageSendMode.Reliable, 
            MessageID.ServerCollectibleDataUpdate
            );
        riptideMessage.AddInt(syncMessage.LiveIndex);
        riptideMessage.AddInt(syncMessage.SaveIndex);
        riptideMessage.AddInt(syncMessage.Level);
        riptideMessage.AddString(syncMessage.Type);
        return riptideMessage;
    }

    public static CollectibleSyncMessage Decode(Message message)
    {
        return new CollectibleSyncMessage(
            message.GetInt(), 
            message.GetInt(), 
            message.GetInt(), 
            message.GetString()
            );
    }
}