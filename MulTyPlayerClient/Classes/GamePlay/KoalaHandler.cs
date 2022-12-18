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

        int _bTimeAttackAddress = PointerCalculations.AddOffset(0x28AB84);

        public Dictionary<int, int[]> KoalaAddrs;
        readonly int[][] koalaOffsets;
        int _baseKoalaAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x26B070), new int[] { 0x0 });

        public KoalaHandler()
        {
        }

        public void CreateKoalas()
        {
            //SETUP KOALA ADDRESS DICTIONARY
            KoalaAddrs = new Dictionary<int, int[]>();
            for (int i = 0; i < 8; i++)
            {
                KoalaAddrs.Add(i, new int[8]);
            }
        }

        public void SetCoordAddrs()
        {
            int modifier = (Program.HLevel.CurrentLevelId == 9 || Program.HLevel.CurrentLevelId == 13) ? 2 : 1;
            foreach (int koalaID in KoalaAddrs.Keys)
            {
                //X COORDINATE
                KoalaAddrs[koalaID][0] = _baseKoalaAddress + 0x2A4 + (0x518 * modifier * koalaID);
                //Y COORDINATE
                KoalaAddrs[koalaID][1] = _baseKoalaAddress + 0x2A8 + (0x518 * modifier * koalaID);
                //Z COORDINATE
                KoalaAddrs[koalaID][2] = _baseKoalaAddress + 0x2AC + (0x518 * modifier * koalaID);
                //P COORDINATE
                KoalaAddrs[koalaID][3] = _baseKoalaAddress + 0x2B4 + (0x518 * modifier * koalaID);
                //Y COORDINATE
                KoalaAddrs[koalaID][4] = _baseKoalaAddress + 0x2B8 + (0x518 * modifier * koalaID);
                //R COORDINATE
                KoalaAddrs[koalaID][5] = _baseKoalaAddress + 0x2BC + (0x518 * modifier * koalaID);
                //COLLISION
                KoalaAddrs[koalaID][6] = _baseKoalaAddress + 0x298 + (0x518 * modifier * koalaID);
                //VISIBILITY
                KoalaAddrs[koalaID][7] = _baseKoalaAddress + 0x44 + (0x518 * modifier * koalaID);
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

        public void CheckTA()
        {
            byte[] buffer = new byte[4];
            int bytesRead = 0;
            ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), _bTimeAttackAddress, buffer, 4, ref bytesRead);
            if (BitConverter.ToInt32(buffer, 0) == 1) MakeVisible();
        }
        
        public void MakeVisible()
        {
            foreach (int koala in KoalaAddrs.Keys)
            {
                ProcessHandler.WriteData(KoalaAddrs[koala][7], new byte[] {1});
            }
        }

        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            int koalaID = message.GetInt();
            int level = message.GetInt();
            float[] coordinates = message.GetFloats();
            string name = message.GetString();

            //SANITY CHECK THAT WE HAVEN'T BEEN SENT OUR OWN COORDINATES AND WE AREN'T LOADING, ON THE MENU, OR IN A DIFFERENT LEVEL 
            if (name == Program.PlayerName || HGameState.CheckMenuOrLoading() || level != HLevel.CurrentLevelId) return;
            
            //WRITE COORDINATES TO KOALA COORDINATE ADDRESSES
            for (int i = 0; i < coordinates.Length; i++)
            {
                ProcessHandler.WriteData(Program.HKoala.KoalaAddrs[koalaID][i], BitConverter.GetBytes(coordinates[i]));
                //Console.WriteLine("Writing {0:F} to {1:X}", coordinates[i], Program.HKoala.KoalaAddrs[intData[0]][i]);
            }
        }
    }
}
