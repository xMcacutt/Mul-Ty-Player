using MulTyPlayerClient.Memory;
namespace MulTyPlayerClient.Sync
{
    internal class SaveRSSyncer : SaveDataSyncer
    {
        public SaveRSSyncer()
        {
            SaveDataOffset = 0x11;
            SaveWriteValue = 1;
        }

        public override void Save(int index, int? level)
        {
            if (level != Levels.RainbowCliffs.Id)
                return;

            int byteIndex = (index / 8) + 1;
            int bitIndex = index % 8;

            int address = SyncHandler.SaveDataBaseAddress + SaveDataOffset + byteIndex;
            ProcessHandler.TryRead(address, out byte b, false);
            b |= (byte)(1 << bitIndex);
            ProcessHandler.WriteData(address, new byte[] { b }, "Setting new rainbow scale save data byte value");
        }

        public override void Sync(int level, byte[] data)
        {
            if (level != Levels.RainbowCliffs.Id)
                return;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 1 && SyncHandler.HRainbowScale.GlobalObjectData[0][i] != 5)
                {
                    Save(i, level);
                }
            }
        }
    }
}
