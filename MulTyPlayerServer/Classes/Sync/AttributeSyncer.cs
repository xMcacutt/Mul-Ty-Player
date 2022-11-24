using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerServer
{
    internal class AttributeSyncer : Syncer
    {
  
        public new Dictionary<int, byte> GlobalObjectSaveData;

        public AttributeSyncer()
        {
            Name = "Attribute";
            CheckState = 1;
            GlobalObjectSaveData = new Dictionary<int, byte>();
            foreach (int i in Enum.GetValues(typeof(Attributes)))
                GlobalObjectSaveData.Add(i, 0);
            GlobalObjectSaveData[(int)Attributes.GotBoom] = 1;
        }

        public override void HandleServerUpdate(int null1, int iAttribute, int null2, ushort originalSender)
        {
            //ILIVE AND LEVEL UNNECESSARY
            if (GlobalObjectSaveData[iAttribute] == 1) return;
            GlobalObjectSaveData[iAttribute] = 1;
            SendUpdatedData(null1, iAttribute, null2, originalSender);
        }

        public override void Sync(ushort player)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.ReqSync);
            message.AddString(Name);
            message.AddInt(0);
            message.AddBytes(new byte[1]);
            message.AddBytes(GlobalObjectSaveData.Values.ToArray());
            Server._Server.Send(message, player);
        }
    }
}
