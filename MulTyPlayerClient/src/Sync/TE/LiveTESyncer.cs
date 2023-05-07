namespace MulTyPlayerClient.Sync
{
    internal class LiveTESyncer : LiveDataSyncer
    {
        public LiveTESyncer(TEHandler hThEg)
        {
            HSyncObject = hThEg;
            StateOffset = 0xC4;
            SeparateCollisionByte = false;
            ObjectLength = 0x144;
        }
    }
}
