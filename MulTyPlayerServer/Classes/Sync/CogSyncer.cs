using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer
{
    internal class CogSyncer : Syncer
    {
        public CogSyncer()
        {
            Name = "Cog";
            CheckState = 5;
            GlobalObjectData = new Dictionary<int, byte[]>();
            GlobalObjectSaveData = new Dictionary<int, byte[]>();
            GlobalObjectCounts = new Dictionary<int, int>();
            foreach (int i in SyncHandler.MainStages)
            {
                GlobalObjectData.Add(i, Enumerable.Repeat((byte)1, 10).ToArray());
                GlobalObjectSaveData.Add(i, Enumerable.Repeat((byte)1, 10).ToArray());
                GlobalObjectCounts.Add(i, 0);
            }
        }
    }
}
