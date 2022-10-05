using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TyMultiplayerCLI
{
    internal class KoalaPos
    {
        TyPosRot tyPosRot => Program.tyPosRot;
        PointerCalculations ptrCalc = new PointerCalculations();
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

        public KoalaPos()
        {
            koalaOffsets = new int[][] 
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
                koalaAddrs.Add(i, new int[4]);
            }
        }

        public void SetCoordAddrs()
        {
            foreach (int koalaID in koalaAddrs.Keys)
            {
                koalaAddrs[koalaID][0] = ptrCalc.GetPointerAddress
                (
                    tyPosRot.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    0
                );
                koalaAddrs[koalaID][1] = ptrCalc.GetPointerAddress
                (
                    tyPosRot.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    4
                );
                koalaAddrs[koalaID][2] = ptrCalc.GetPointerAddress
                (
                    tyPosRot.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    8
                );
                koalaAddrs[koalaID][3] = ptrCalc.GetPointerAddress
                (
                    tyPosRot.AddOffset(baseAddressOfKoalaStuff),
                    koalaOffsets[koalaID],
                    10
                );
            }
        }
    }
}
