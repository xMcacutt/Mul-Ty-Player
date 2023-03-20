using System;
using System.Linq;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal static class PointerCalculations
    {
        public async static Task<int> GetPointerAddress(int baseAddress, int[] offsets, int extraOffset = 0)
        {
            IntPtr addr = new(baseAddress);
            for (int i = 0; i < offsets.Length; i++)
            {
                int nextAddress = BitConverter.ToInt32(await ProcessHandler.ReadDataAsync((int)addr, 4), 0);
                addr = new IntPtr(nextAddress + offsets[i]);
            }
            addr += extraOffset;
            return addr.ToInt32();
        }

        public static int AddOffset(int i)
        {
            return (int)IntPtr.Add(ProcessHandler.TyProcess.MainModule.BaseAddress, i);
        }
    }
}
