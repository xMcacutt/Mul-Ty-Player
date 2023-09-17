using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerServer
{
    internal class RainbowScaleSyncer : Syncer
    {
        public RainbowScaleSyncer()
        {
            Name = "RainbowScale";
            CheckState = 5;
            GlobalObjectData = new Dictionary<int, byte[]>();
            GlobalObjectSaveData = new Dictionary<int, byte[]>();
            GlobalObjectCounts = new Dictionary<int, int>();

            GlobalObjectData.Add(0, Enumerable.Repeat((byte)2, 25).ToArray());
            GlobalObjectSaveData.Add(0, Enumerable.Repeat((byte)2, 25).ToArray());
            GlobalObjectCounts.Add(0, 0);
        }
    }
}
