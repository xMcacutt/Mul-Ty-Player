using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal abstract class Syncer
    {
        public string Name { get; set; }
        public int CheckState { get; set; }
        public Dictionary<int, byte[]> GlobalObjectData;
        public Dictionary<int, int> GlobalObjectCounts;

        public virtual void HandleServerUpdate(int index, int level, ushort originalSender)
        {
            if (!GlobalObjectData.Keys.Contains(level)) return;
            if (GlobalObjectData[level][index] == CheckState) return;
            GlobalObjectData[level][index] = (byte)CheckState;
            GlobalObjectCounts[level] = GlobalObjectData[level].Count(i => i == CheckState);
            SendUpdatedData(index, level, originalSender);
        }

        public virtual void SendUpdatedData(int index, int level, ushort originalSender)
        {
            SyncMessage syncMessage = SyncMessage.Create(index, level, Name);
            Server._Server.SendToAll(SyncMessage.Encode(syncMessage), originalSender);
        }

        public virtual void Sync(ushort player)
        {
            foreach (int i in GlobalObjectData.Keys)
            {
                Message message = Message.Create(MessageSendMode.Reliable, MessageID.ReqSync);
                message.AddString(Name);
                message.AddInt(i);
                message.AddBytes(GlobalObjectData[i]);
                Server._Server.Send(message, player);
            }
        }
    }
}
