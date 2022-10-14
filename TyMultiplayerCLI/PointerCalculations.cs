using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class PointerCalculations
    { 
        IntPtr HProcess => ProcessHandler.HProcess;

        public PointerCalculations()
        {

        }

        public int GetPointerAddress(int baseAddress, int[] offsets, int extraOffset)
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

        public int GetPointerAddress(int baseAddress, int[] offsets)
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

        public int GetPointerAddressNegative(int baseAddress, int[] offsets, int extraOffset)
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
        

        public int AddOffset(int i)
        {
            return (int)IntPtr.Add(ProcessHandler.TyProcess.MainModule.BaseAddress, i);
        }
    }
}
