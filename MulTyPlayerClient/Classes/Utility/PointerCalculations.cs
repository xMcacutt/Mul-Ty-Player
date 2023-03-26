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
            //READS MEMORY AT ADDRESS+OFFSETS[i] STORES IN ADDRESS AND REPEATS FOR ALL OFFSETS.
            //MEMORY AT ADDR+OFFSET[i] IS NOT READ.
            //ADDR+OFFSET[i] IS RETURNED
            IntPtr addr = new(baseAddress);
            for (int i = 0; i < offsets.Length; i++)
            {
                int nextAddress = BitConverter.ToInt32(ProcessHandler.ReadData((int)addr, 4, $"Following pointer path {i+1} / {offsets.Length}"), 0);
                addr = new IntPtr(nextAddress + offsets[i]);
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

        public static int AddOffset(int i)
        {
            try
            {
                if (ProcessHandler.TyProcess == null) throw new TyClosedException();
                int addr = (int)IntPtr.Add(ProcessHandler.BaseAddress, i);
                return addr;
            }
            catch(TyClosedException ex)
            {
                return 0;
            }
        }
    }
}
