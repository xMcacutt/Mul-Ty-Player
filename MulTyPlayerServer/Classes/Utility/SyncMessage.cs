using Riptide;

namespace MulTyPlayerServer
{
    internal class SyncMessage
    {
        public int index;
        public int level;
        public string type;

        public static SyncMessage Create(int index, int level, string type)
        {
            return new SyncMessage(index, level, type);
        }

        public static Message Encode(SyncMessage syncMessage)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ClientDataUpdate);
            message.AddInt(syncMessage.index);
            message.AddInt(syncMessage.level);
            message.AddString(syncMessage.type);
            return message;
        }

        public SyncMessage(int index, int level, string type)
        {
            this.index = index;
            this.level = level;
            this.type = type;
        }

        public static SyncMessage Decode(Message message)
        {
            return new SyncMessage(message.GetInt(), message.GetInt(), message.GetString());
        }
    }
}
