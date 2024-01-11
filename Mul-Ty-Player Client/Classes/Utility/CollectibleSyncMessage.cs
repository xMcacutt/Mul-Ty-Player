using MulTyPlayer;
using Riptide;

namespace MulTyPlayerClient;

internal class CollectibleSyncMessage
{
    public int iLive;
    public int iSave;
    public int level;
    public string type;

    public CollectibleSyncMessage(int iLive, int iSave, int level, string type)
    {
        this.iLive = iLive;
        this.iSave = iSave;
        this.level = level;
        this.type = type;
    }

    public static CollectibleSyncMessage Create(int iLive, int iSave, int level, string type)
    {
        return new CollectibleSyncMessage(iLive, iSave, level, type);
    }

    public static Message Encode(CollectibleSyncMessage collectibleSyncMessage)
    {
        var message = Message.Create(MessageSendMode.Reliable, MessageID.ServerCollectibleDataUpdate);
        message.AddInt(collectibleSyncMessage.iLive);
        message.AddInt(collectibleSyncMessage.iSave);
        message.AddInt(collectibleSyncMessage.level);
        message.AddString(collectibleSyncMessage.type);
        return message;
    }

    public static CollectibleSyncMessage Decode(Message message)
    {
        return new CollectibleSyncMessage(message.GetInt(), message.GetInt(), message.GetInt(), message.GetString());
    }
}