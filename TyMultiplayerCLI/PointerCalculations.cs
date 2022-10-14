using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal static class PointerCalculations
    { 
        static IntPtr HProcess => ProcessHandler.HProcess;

        public static int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            IntPtr addr = (IntPtr)baseAddress;
            ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
            addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            foreach (int i in offsets)
            {
                if (i != offsets[offsets.Length - 1])
                {
                    addr += i;
                    ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
                    addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
                }
            }
            addr += offsets[offsets.Length - 1];
            addr += extraOffset;
            return (int)addr;
        }

        public static int GetPointerAddress(int baseAddress, int[] offsets)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            IntPtr addr = (IntPtr)baseAddress;
            ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
            addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            foreach (int i in offsets)
            {
                if (i != offsets[offsets.Length - 1])
                {
                    addr += i;
                    ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
                    addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
                }
            }
            addr += offsets[offsets.Length - 1];
            return (int)addr;
        }

        public static int GetPointerAddress(int baseAddress, int offset)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            IntPtr addr = (IntPtr)baseAddress;
            ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
            addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            addr += offset;
            return (int)addr;
        }

        public static int GetPointerAddressNegative(int baseAddress, int[] offsets, int extraOffset)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            IntPtr addr = (IntPtr)baseAddress;
            ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
            addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            foreach (int i in offsets)
            {
                if (i != offsets[offsets.Length - 1])
                {
                    addr += i;
                    ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
                    addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
                }
            }
            addr += offsets[offsets.Length - 1];
            addr -= extraOffset;
            return (int)addr;
        }
        

        public static int AddOffset(int i)
        {
            return (int)IntPtr.Add(ProcessHandler.TyProcess.MainModule.BaseAddress, i);
        }
    }
}
