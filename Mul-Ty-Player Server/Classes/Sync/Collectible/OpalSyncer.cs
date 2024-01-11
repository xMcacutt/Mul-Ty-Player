using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer;

internal class OpalSyncer : Syncer
{
    public OpalSyncer()
    {
        Name = "Opal";
        CheckState = 5;
        GlobalObjectData = new Dictionary<int, byte[]>();
        GlobalObjectSaveData = new Dictionary<int, byte[]>();
        GlobalObjectCounts = new Dictionary<int, int>();
        foreach (var i in SyncHandler.MainStages)
        {
            GlobalObjectData.Add(i, Enumerable.Repeat((byte)2, 300).ToArray());
            GlobalObjectSaveData.Add(i, Enumerable.Repeat((byte)2, 300).ToArray());
            GlobalObjectCounts.Add(i, 0);
        }
    }
}