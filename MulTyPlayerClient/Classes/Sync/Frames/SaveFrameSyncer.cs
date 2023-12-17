namespace MulTyPlayerClient
{
    internal class SaveFrameSyncer : SaveDataSyncer
    {
        public override void Save(int index, int? level)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            int address = SyncHandler.SaveDataBaseAddress + 0xAD2 + byteIndex;
            ProcessHandler.TryRead(address, out byte b, false, "SaveFrameSyncer::Save()");
            b |= (byte)(1 << bitIndex);
            ProcessHandler.WriteData(address, new byte[] {b}, "Setting new frame save data byte value");
        }

        public void Sync(byte[] data)
        {
            ProcessHandler.WriteData(SyncHandler.SaveDataBaseAddress + 0xAD2, data, "Syncing frame save data");
        }
    }
}
