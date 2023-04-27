using System;

namespace MulTyPlayerClient
{
    internal class LiveRSSyncer : LiveDataSyncer
    {
        public LiveRSSyncer(RSHandler HRainbowScale)
        {
            HSyncObject = HRainbowScale;
            StateOffset = 0x78;
            SeparateCollisionByte = false;
            ObjectLength = 0x114;
        }
    }
}
