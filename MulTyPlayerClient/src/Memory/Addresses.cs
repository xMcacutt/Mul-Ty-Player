using System;

namespace MulTyPlayerClient.Memory
{
    public static class Addresses
    {
        public static int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset = 0)
        {
            //READS MEMORY AT ADDRESS+OFFSETS[i] STORES IN ADDRESS AND REPEATS FOR ALL OFFSETS.
            //MEMORY AT ADDR+OFFSET[i] IS NOT READ.
            //ADDR+OFFSET[i] IS RETURNED
            IntPtr addr = new(baseAddress);
            for (int i = 0; i < offsets.Length; i++)
            {
                bool addBase = i == 0;
                ProcessHandler.TryRead(addr, out IntPtr nextAddress, addBase);
                addr = nextAddress + offsets[i];
            }
            if (extraOffset == 0)
            {
                return addr.ToInt32();
            }
            else
            {
                addr += extraOffset;
                return addr.ToInt32();
            }
        }

        public const int GEM_POINTER_LIST = 0x28AB7C;

        public const int TY_POSITION_ADDRESS = 0x270B78;
        public const int TY_ROTATION_ADDRESS = 0x271C1C;

        public const int BP_POSITION_ADDRESS = 0x254268;
        public const int BP_ROTATION_ADDRESS = 0x2545F0;

        public static int TyPositionX;
        public static int TyPositionY;
        public static int TyPositionZ;

        public static int TyRotationPitch;
        public static int TyRotationYaw;
        public static int TyRotationRoll;

        public static int CurrentLevelId;

        public const int LiveGemCount = 0x26547C;
        public static int NonCrateOpalsAddress = GetPointerAddress(GEM_POINTER_LIST, new int[] { 0x0, 0x0 });

        public static int LiveThunderEggCount;
        public static int LiveGoldenCogCount;
        public static int LiveBilbyCount;
        public static int LiveFrameCount;
    }
}
