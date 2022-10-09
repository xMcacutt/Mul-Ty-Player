using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerCLI
{
    internal class KoalaHandler
    {
        PointerCalculations ptrCalc => Program.ptrCalc;
        IntPtr hProcess => ProcessHandler.hProcess;

        public Dictionary<int, int[]> koalaAddrs;

        const int baseAddressOfKoalaStuff = 0x00323BA4;
        int[] boonieOffsets = { 0x10, 0xC, 0x8, 0x30, 0x30, 0x30, 0x2A4 };
        int[] mimOffsets = { 0x10, 0xC, 0x8, 0x30, 0x30, 0x2A0, 0x2A4 };
        int[] kikiOffsets = { 0x10, 0xC, 0x8, 0x34, 0x30, 0x30, 0x2A4 };
        int[] katieOffsets = { 0x10, 0xC, 0x8, 0x34, 0x30, 0x2F4, 0x1FC };
        int[] snugsOffsets = { 0x10, 0xC, 0x8, 0x30, 0x34, 0x34, 0x2A4 };
        int[] dubboOffsets = { 0x10, 0xC, 0x8, 0x34, 0x34, 0x2A0, 0x2A4 };
        int[] gummyOffsets = { 0x10, 0xC, 0x8, 0x34, 0x34, 0x34, 0x2A4 };
        int[] elizabethOffsets = { 0x10, 0xC, 0x8, 0x34, 0x270, 0x0, 0x1FC };

        int[][] koalaOffsets;

        public KoalaHandler()
        {
            koalaOffsets = new [] 
            { 
                boonieOffsets, mimOffsets, 
                kikiOffsets, katieOffsets, 
                snugsOffsets, dubboOffsets, 
                gummyOffsets, elizabethOffsets 
            };
        }

        public void CreateKoalas()
        {
            koalaAddrs = new Dictionary<int, int[]>();
            for(int i = 0; i < 8; i++)
            {
                koalaAddrs.Add(i, new int[5]);
            }
        }

        public void RemoveCollision()
        {
            int bytesWritten = 0;
            byte[] buffer = BitConverter.GetBytes(0);
            foreach (int koala in koalaAddrs.Keys)
            {
                ProcessHandler.WriteProcessMemory((int)hProcess, koalaAddrs[koala][4], buffer, buffer.Length, ref bytesWritten);
               // Console.WriteLine("addr: {0:X}", koalaAddrs[koala][4]);
            }
           // Console.WriteLine("removed collision");
        }

        public void SetCoordAddrs()
        {
            /*
             * 0 -> X
             * 1 -> Y
             * 2 -> Z
             * 3 -> Yaw
             * 4 -> Collision
             */
            foreach (int koalaID in koalaAddrs.Keys)
            {
                koalaAddrs[koalaID][0] = ptrCalc.GetPointerAddress
                (
                    ptrCalc.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    0x0
                );
                koalaAddrs[koalaID][1] = ptrCalc.GetPointerAddress
                (
                    ptrCalc.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    0x4
                );
                koalaAddrs[koalaID][2] = ptrCalc.GetPointerAddress
                (
                    ptrCalc.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    0x8
                );
                koalaAddrs[koalaID][3] = ptrCalc.GetPointerAddress
                (
                    ptrCalc.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    0x14
                );
                koalaAddrs[koalaID][4] = ptrCalc.GetPointerAddressNegative
                (
                    ptrCalc.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    12
                );
            }
        }
    }
}
