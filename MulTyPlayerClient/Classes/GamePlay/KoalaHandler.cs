using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MulTyPlayerClient
{
    internal class KoalaHandler
    {
        static GameStateHandler HGameState => Program.HGameState;
        static LevelHandler HLevel => Program.HLevel;

        public Dictionary<int, int[]> KoalaAddrs;
        readonly int[][] koalaOffsets;
        const int KOALA_BASE_ADDRESS = 0x00323BA4;
        readonly int[] _boonieOffsets = { 0x10, 0xC, 0x8, 0x30, 0x30, 0x30, 0x2A4 };
        readonly int[] _mimOffsets = { 0x10, 0xC, 0x8, 0x30, 0x30, 0x2A0, 0x2A4 };
        readonly int[] _kikiOffsets = { 0x10, 0xC, 0x8, 0x34, 0x30, 0x30, 0x2A4 };
        readonly int[] _katieOffsets = { 0x10, 0xC, 0x8, 0x34, 0x30, 0x2F4, 0x1FC };
        readonly int[] _snugsOffsets = { 0x10, 0xC, 0x8, 0x30, 0x34, 0x34, 0x2A4 };
        readonly int[] _dubboOffsets = { 0x10, 0xC, 0x8, 0x34, 0x34, 0x2A0, 0x2A4 };
        readonly int[] _gummyOffsets = { 0x10, 0xC, 0x8, 0x34, 0x34, 0x34, 0x2A4 };
        readonly int[] _elizabethOffsets = { 0x10, 0xC, 0x8, 0x34, 0x270, 0x0, 0x1FC };

        public KoalaHandler()
        {
            //SETUP ADDRESS OFFSETS
            koalaOffsets = new[]
            {
                _boonieOffsets, _mimOffsets,
                _kikiOffsets, _katieOffsets,
                _snugsOffsets, _dubboOffsets,
                _gummyOffsets, _elizabethOffsets
            };
        }

        public void CreateKoalas()
        {
            //SETUP KOALA ADDRESS DICTIONARY
            KoalaAddrs = new Dictionary<int, int[]>();
            for (int i = 0; i < 8; i++)
            {
                KoalaAddrs.Add(i, new int[7]);
            }
        }

        public void SetCoordAddrs()
        {
            foreach (int koalaID in KoalaAddrs.Keys)
            {
                //X COORDINATE
                KoalaAddrs[koalaID][0] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x0
                );
                //Y COORDINATE
                KoalaAddrs[koalaID][1] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x4
                );
                //Z COORDINATE
                KoalaAddrs[koalaID][2] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x8
                );
                //PITCH
                KoalaAddrs[koalaID][3] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                   koalaOffsets[koalaID],
                   0x10
                );
                //YAW
                KoalaAddrs[koalaID][4] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x14
                );
                //ROLL
                KoalaAddrs[koalaID][5] = PointerCalculations.GetPointerAddress
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0x18
                );
                //COLLISION
                KoalaAddrs[koalaID][6] = PointerCalculations.GetPointerAddressNegative
                (
                    PointerCalculations.AddOffset(KOALA_BASE_ADDRESS),
                    koalaOffsets[koalaID],
                    0xC
                );
            }
            if (!SettingsHandler.DoKoalaCollision) RemoveCollision();
        }

        public void RemoveCollision()
        {
            //WRITES 0 TO COLLISION BYTE
            foreach (int koala in KoalaAddrs.Keys)
            {
                ProcessHandler.WriteData(KoalaAddrs[koala][6], new byte[] {0});
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

            //SANITY CHECK THAT WE HAVEN'T BEEN SENT OUR OWN COORDINATES AND WE AREN'T LOADING, ON THE MENU, OR IN A DIFFERENT LEVEL 
            if (
                name == Program.PlayerName 
                || HGameState.CheckMenuOrLoading() 
                || intData[1] != HLevel.CurrentLevelId
                ) 
            { return; }
            
            //WRITE COORDINATES TO KOALA COORDINATE ADDRESSES
            for (int i = 0; i < coordinates.Length; i++)
            {
                ProcessHandler.WriteData(Program.HKoala.KoalaAddrs[intData[0]][i], BitConverter.GetBytes(coordinates[i]));
                //Console.WriteLine("Writing {0:F} to {1:X}", coordinates[i], Program.HKoala.KoalaAddrs[intData[0]][i]);
            }
        }
    }
}
