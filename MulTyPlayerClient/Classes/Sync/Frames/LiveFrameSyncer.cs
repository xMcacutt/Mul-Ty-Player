using System.Collections.Generic;

namespace MulTyPlayerClient
{
    internal class LiveFrameSyncer : LiveDataSyncer
    {
        FrameHandler HFrame => HSyncObject as FrameHandler;
        static LevelHandler HLevel => Client.HLevel;
        
        public LiveFrameSyncer(FrameHandler HFrame)
        {
            HSyncObject = HFrame;
        }
        
        public Dictionary<int, byte> ReadData()
        {
            int framesInLevel = Levels.GetLevelData(HLevel.CurrentLevelId).FrameCount;
            Dictionary<int, byte> currentFrames = new();

            int address = HFrame.FrameAddress;
            int index;
            byte state;
            for (int i = 0; i < framesInLevel; i++)
            {
                ProcessHandler.TryRead(address + 0x84, out index, false, "LiveFrameSyncer::ReadData() {0}");
                ProcessHandler.TryRead(address + 0x8A, out state, false, "LiveFrameSyncer::ReadData() {1}");
                currentFrames.Add(index, state);
                ProcessHandler.TryRead(address + 0x30, out address, false, "LiveFrameSyncer::ReadData() {2}");
            }
            return currentFrames;
        }

        public override void Collect(int index)
        {
            if (HFrame.CurrentObjectData[index] == 1) return;
            if (Client.HGameState.IsAtMainMenuOrLoading()) return;
            int address = HFrame.FrameAddress;
            for (int i = 0; i < index; i++)
            {
                ProcessHandler.TryRead(address + 0x30, out address, false, "LiveFrameSyncer::Collect {0}");
            }
            ProcessHandler.WriteData(address + 0x8B, new byte[] { 0x1 }, "LiveFrameSyncer::Collected {1}");
        }
    }
}
