using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer
{
    internal class BilbySyncer : Syncer
    {
        public BilbySyncer()
        {
            Name = "Bilby";
            CheckState = 0;
            GlobalObjectData = new Dictionary<int, byte[]>();
            GlobalObjectSaveData = new Dictionary<int, byte[]>();
            GlobalObjectCounts = new Dictionary<int, int>();
            foreach (int i in SyncHandler.MainStages)
            {
                GlobalObjectData.Add(i, Enumerable.Repeat((byte)1, 5).ToArray());
                GlobalObjectSaveData.Add(i, Enumerable.Repeat((byte)1, 5).ToArray());
                GlobalObjectCounts.Add(i, 0);
            }
        }
        
        public virtual void HandleServerUpdate(int iLive, int iSave, int level, ushort originalSender)
        {
            if (!GlobalObjectData.Keys.Contains(level)) return;
            //Console.WriteLine("Sending " + Name + " LiveNumber: " + iLive + " SaveNumber: " + iSave + " For Level: " + level);
            GlobalObjectData[level][iLive] = (byte)CheckState;
            GlobalObjectCounts[level] = GlobalObjectData[level].Count(i => i == CheckState);
            GlobalObjectSaveData[level][iSave] = GlobalObjectCounts[level] == 5 ? (byte)5 : (byte)1;
            SendUpdatedData(iLive, iSave, level, originalSender);
        }
    }
}
