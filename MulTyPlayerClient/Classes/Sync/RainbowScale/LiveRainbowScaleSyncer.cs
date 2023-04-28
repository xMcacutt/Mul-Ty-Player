using System;

namespace MulTyPlayerClient
{
    internal class LiveRSSyncer : LiveDataSyncer
    {
        RSHandler HRainbowScale => HSyncObject as RSHandler;
        static LevelHandler HLevel => Client.HLevel;

        public LiveRSSyncer(RSHandler HRainbowScale)
        {
            HSyncObject = HRainbowScale;
            StateOffset = 0x78;
            SeparateCollisionByte = false;
            ObjectLength = 0x114;
        }

        public override void Collect(int index)
        {
            //If gem has already been collected, return
            if (HRainbowScale.CurrentObjectData[index] >= 3)
                return;

            //If in menu or loading, return
            if (Client.HGameState.CheckMenuOrLoading())
                return;

            if (HLevel.CurrentLevelId != Levels.RainbowCliffs.Id)
                return;

            int address = HRainbowScale.LiveObjectAddress + StateOffset + (ObjectLength * index);
            ProcessHandler.TryRead(address, out HRainbowScale.CurrentObjectData[index], false);

            if (HRainbowScale.CurrentObjectData[index] >= 3)
                return;

            ProcessHandler.WriteData(address, BitConverter.GetBytes(3), $"Collecting opal {index}");
        }
    }
}
