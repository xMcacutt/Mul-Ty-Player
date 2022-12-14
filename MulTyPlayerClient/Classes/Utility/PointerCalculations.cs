using System;
using System.Linq;

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
            for (int i = 0; i < offsets.Length - 1; i++)
            {
                addr += offsets[i];
                ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
                addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
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
            for(int i = 0; i < offsets.Length - 1; i++)
            { 
                addr += offsets[i];
                ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
                addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
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
            for (int i = 0; i < offsets.Length - 1; i++)
            {
                addr += offsets[i];
                ProcessHandler.ReadProcessMemory((int)HProcess, (int)addr, buffer, 4, ref bytesRead);
                addr = (IntPtr)BitConverter.ToInt32(buffer, 0);
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
