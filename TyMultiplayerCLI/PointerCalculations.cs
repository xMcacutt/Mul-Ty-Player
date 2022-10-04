using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerCLI
{
    internal class PointerCalculations
    {
        TyPosRot tyPosRot => Program.tyPosRot;
        IntPtr tyexeHandle => Program.tyexeHandle;

        public PointerCalculations()
        {

        }

        public int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset)
        {
            int bytesRead = 0;
            IntPtr addr = (IntPtr)baseAddress;
            byte[] buffer = new byte[4];
            Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
            addr += BitConverter.ToInt32(buffer, 0);
            foreach (int i in offsets)
            {
                if (i != offsets[offsets.Length - 1]) 
                {
                    addr += i;
                    Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
                }
            }
            addr += offsets[offsets.Length - 1];
            addr += extraOffset;
            addr -= baseAddress;
            return (int)addr;
        }

        public int GetPointerAddress(int baseAddress, int[] offsets)
        {
            int bytesRead = 0;
            IntPtr addr = (IntPtr)baseAddress;
            byte[] buffer = new byte[4];
            Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
            addr += BitConverter.ToInt32(buffer, 0);
            foreach (int i in offsets)
            {
                if (i != offsets[offsets.Length - 1])
                {
                    addr += i;
                    Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
                }
            }
            addr += offsets[offsets.Length - 1];
            addr -= baseAddress;
            return (int)addr;
        }
    }
}
