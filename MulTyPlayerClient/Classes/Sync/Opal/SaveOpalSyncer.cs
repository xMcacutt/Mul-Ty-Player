namespace MulTyPlayerClient
{
    internal class SaveOpalSyncer : SaveDataSyncer
    {

        public SaveOpalSyncer()
        {
        }

        public override void Save(int index, int? level)
        {
            int crateOpals = Levels.GetLevelData((int)level).CrateOpalCount;
            int newIndex = index > (299 - crateOpals) ? 300 - crateOpals + (299 - index) : index;

            int byteIndex = (newIndex / 8) + 1;
            int bitIndex = newIndex % 8;

            int address = (SyncHandler.SaveDataBaseAddress + (0x70 * (int)level) + byteIndex);
            ProcessHandler.TryRead(address, out byte b, false);
            b |= (byte)(1 << bitIndex);
            ProcessHandler.WriteData(address, new byte[] {b}, "Setting new opal save data byte value");
        }

        public override void Sync(int level, byte[] data)
        {
            for(int i = 0; i < data.Length; i++)
            {
                if (data[i] == 1 && SyncHandler.HOpal.GlobalObjectData[level][i] != 5)
                {
                    Save(i, level);
                }
            }
        }
    }
}
