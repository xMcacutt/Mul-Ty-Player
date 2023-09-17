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
    }
}
