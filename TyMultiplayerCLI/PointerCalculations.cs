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
        IntPtr tyexeHandle => Program.tyexeHandle;

        public PointerCalculations()
        {

        }


        public int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            IntPtr addr = (IntPtr)baseAddress;
            Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
            addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            foreach (int i in offsets)
            {
                if (i != offsets[offsets.Length - 1])
                {
                    addr += i;
                    Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
                    addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
                }
            }
            addr += offsets[offsets.Length - 1];
            addr += extraOffset;
            return (int)addr;
        }

        /*public int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset)
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
                    Console.WriteLine()
                }
            }
            addr += offsets[offsets.Length - 1];
            addr += extraOffset;
            addr -= baseAddress;
            return (int)addr;
        }*/

        public int GetPointerAddress(int baseAddress, int[] offsets)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            IntPtr addr = (IntPtr)baseAddress;
            Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
            addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            foreach (int i in offsets)
            {
                if (i != offsets[offsets.Length - 1])
                {
                    addr += i;
                    Program.ReadProcessMemory((int)tyexeHandle, (int)addr, buffer, 4, ref bytesRead);
                    addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
                }
            }
            addr += offsets[offsets.Length - 1];
            addr -= baseAddress;
            return (int)addr;
        }
    }
}
