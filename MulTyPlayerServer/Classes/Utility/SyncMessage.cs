using Riptide;

namespace MulTyPlayerServer
{
    internal class SyncMessage
    {
        public int iLive;
        public int iSave;
        public int level;
        public string type;

        public static SyncMessage Create(int iLive, int iSave, int level, string type)
        {
            return new SyncMessage(iLive, iSave, level, type);
        }

        public static Message Encode(SyncMessage syncMessage)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ClientDataUpdate);
            message.AddInt(syncMessage.iLive);
            message.AddInt(syncMessage.iSave);
            message.AddInt(syncMessage.level);
            message.AddString(syncMessage.type);
            return message;
        }

        public SyncMessage(int iLive, int iSave, int level, string type)
        {
            this.iLive = iLive;
            this.iSave = iSave;
            this.level = level;
            this.type = type;
        }

        public static SyncMessage Decode(Message message)
        {
            return new SyncMessage(message.GetInt(), message.GetInt(), message.GetInt(), message.GetString());
        }
    }
}
