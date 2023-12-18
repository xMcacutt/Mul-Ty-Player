namespace MulTyPlayerClient
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

        public void Spawn(int index)
        {
            if (HSyncObject.CurrentObjectData[index] != 0) return;
            ProcessHandler.WriteData(HSyncObject.LiveObjectAddress + StateOffset + (ObjectLength * index), new byte[] { 1 }, "Spawning TE");
        }
    }
}
