using System;

namespace MulTyPlayerClient;

internal static class PointerCalculations
{
    public static int GetPointerAddress(int baseAddress, int[] offsets)
    {
        //MEMORY AT ADDR+OFFSET[i] IS NOT READ.
        IntPtr addr = new(baseAddress);
        for (var i = 0; i < offsets.Length; i++)
        {
            var addBase = i == 0;
            ProcessHandler.TryRead(addr, out IntPtr nextAddress, addBase, "GetPointerAddress()");
            addr = nextAddress + offsets[i];
        }
        return addr.ToInt32();
    }
}



