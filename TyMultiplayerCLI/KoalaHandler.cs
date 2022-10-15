using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class KoalaHandler
    {
        IntPtr HProcess => ProcessHandler.HProcess;
        static HeroHandler HeroHandler => Program.HeroHandler;
        static KoalaHandler rKoalaHandler => Program.KoalaHandler;

        public Dictionary<int, int[]> KoalaAddrs;

        const int KOALA_BASE_ADDRESS = 0x00323BA4;
        readonly int[] _boonieOffsets = { 0x10, 0xC, 0x8, 0x30, 0x30, 0x30, 0x2A4 };
        readonly int[] _mimOffsets = { 0x10, 0xC, 0x8, 0x30, 0x30, 0x2A0, 0x2A4 };
        readonly int[] _kikiOffsets = { 0x10, 0xC, 0x8, 0x34, 0x30, 0x30, 0x2A4 };
        readonly int[] _katieOffsets = { 0x10, 0xC, 0x8, 0x34, 0x30, 0x2F4, 0x1FC };
        readonly int[] _snugsOffsets = { 0x10, 0xC, 0x8, 0x30, 0x34, 0x34, 0x2A4 };
        readonly int[] _dubboOffsets = { 0x10, 0xC, 0x8, 0x34, 0x34, 0x2A0, 0x2A4 };
        readonly int[] _gummyOffsets = { 0x10, 0xC, 0x8, 0x34, 0x34, 0x34, 0x2A4 };
        readonly int[] _elizabethOffsets = { 0x10, 0xC, 0x8, 0x34, 0x270, 0x0, 0x1FC };

        readonly int[][] koalaOffsets;

        public KoalaHandler()
        {
            koalaOffsets = new []
            { 
                _boonieOffsets, _mimOffsets,
                _kikiOffsets, _katieOffsets,
                _snugsOffsets, _dubboOffsets,
                _gummyOffsets, _elizabethOffsets
            };
        }

        public void CreateKoalas()
        {
            KoalaAddrs = new Dictionary<int, int[]>();
            for(int i = 0; i < 8; i++)
            {
                KoalaAddrs.Add(i, new int[5]);
            }
        }

        public void RemoveCollision()
        {
            int bytesWritten = 0;
            byte[] buffer = BitConverter.GetBytes(0);
            foreach (int koala in KoalaAddrs.Keys)
            {
                ProcessHandler.WriteProcessMemory((int)HProcess, KoalaAddrs[koala][4], buffer, buffer.Length, ref bytesWritten);
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
            foreach (int koalaID in KoalaAddrs.Keys)
            {
                KoalaAddrs[koalaID][0] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x0
                );
                KoalaAddrs[koalaID][1] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x4
                );
                KoalaAddrs[koalaID][2] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x8
                );
                KoalaAddrs[koalaID][3] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x14
                );
                KoalaAddrs[koalaID][4] = PointerCalculations.GetPointerAddressNegative
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    12
                );
            }
        }

        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            int[] intData = message.GetInts();
            float[] coordinates = message.GetFloats();
            string name = message.GetString();

            //intData[0] = Koala ID
            //intData[1] = Current Level For Given Player (Koala ID)

            if (name == Program.PlayerName || HeroHandler.CheckLoading()) { return; }

            if (intData[1] != HeroHandler.CurrentLevelId)
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                IntPtr hProcess = ProcessHandler.OpenProcess(0x1F0FFF, false, ProcessHandler.TyProcess.Id);
                int bytesWritten = 0;
                byte[] buffer = BitConverter.GetBytes(coordinates[i]);
                ProcessHandler.WriteProcessMemory((int)hProcess, rKoalaHandler.KoalaAddrs[intData[0]][i], buffer, buffer.Length, ref bytesWritten);
               // Console.WriteLine("Writing {0:F} to {1:X}", coordinates[i], rKoalaHandler.KoalaAddrs[intData[0]][i]);
            }
        }
    }
}
