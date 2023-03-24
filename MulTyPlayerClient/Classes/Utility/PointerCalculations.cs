using MulTyPlayerClient.GUI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal static class PointerCalculations
    {
        public static int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset = 0)
        {
            IntPtr addr = new(baseAddress);
            for (int i = 0; i < offsets.Length; i++)
            {
                int nextAddress = BitConverter.ToInt32(ProcessHandler.ReadData((int)addr, 4, $"Following pointer path {i} / {offsets.Length}"), 0);
                addr = new IntPtr(nextAddress + offsets[i]);
            }
            addr += extraOffset;
            return addr.ToInt32();
        }

        public static int AddOffset(int i)
        {
            try
            {
                return (int)IntPtr.Add(ProcessHandler.TyProcess.MainModule.BaseAddress, i);
            }
            catch
            {
                BasicIoC.LoggerInstance.Write("Failed to add offset, maybe you disconnected?");
                return 0;
            }
        }
    }
}
