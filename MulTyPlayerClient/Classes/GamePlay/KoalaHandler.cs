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

        public Dictionary<string, int[]> KoalaAddrs;
        private static string[] _koalaNames = { "Katie", "Mim", "Elizabeth", "Snugs", "Gummy", "Dubbo", "Kiki", "Boonie" };
        int _bTimeAttackAddress = PointerCalculations.AddOffset(0x28AB84);
        int _baseKoalaAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x26B070), new int[] { 0x0 });
        public static List<Koala> Koalas;

        public KoalaHandler()
        {
            Koalas = new();
        }

        public static void AssignKoala(string KoalaName, string PlayerName, ushort ClientID)
        {
            Koalas.Add(new Koala(KoalaName, PlayerName, ClientID, Array.IndexOf(_koalaNames, KoalaName)));
        }

        public static void UnassignKoala(string KoalaName) 
        {
            Koalas.RemoveAll(x => x.KoalaName == KoalaName);
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void HandleKoalaSelected(Message message)
        {
            string KoalaName = message.GetString();
            string PlayerName = message.GetString();
            ushort ClientID = message.GetUShort();
            AssignKoala(KoalaName, PlayerName, ClientID);
        }

        public void CreateKoalaAddrArrays()
        {
            //SETUP KOALA ADDRESS DICTIONARY
            KoalaAddrs = new Dictionary<string, int[]>();
            foreach (string koala in _koalaNames)
            {
                KoalaAddrs.Add(koala, new int[8]);
            }
        }

        public void SetBaseAddress()
        {
            _baseKoalaAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x26B070), new int[] { 0x0 });
        }

        public void SetCoordAddrs()
        {
            int modifier = (Program.HLevel.CurrentLevelId == 9 || Program.HLevel.CurrentLevelId == 13) ? 2 : 1;
            int offset = (Program.HLevel.CurrentLevelId == 9 || Program.HLevel.CurrentLevelId == 13) ? 0x518 : 0x0;
            if(_baseKoalaAddress == 0) _baseKoalaAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x26B070), new int[] { 0x0 });
            foreach (Koala koala in Koalas)
            {
                //X COORDINATE
                KoalaAddrs[koala.KoalaName][0] = _baseKoalaAddress + offset + 0x2A4 + (0x518 * modifier * koala.KoalaID);
                //Y COORDINATE
                KoalaAddrs[koala.KoalaName][1] = _baseKoalaAddress + offset + 0x2A8 + (0x518 * modifier * koala.KoalaID);
                //Z COORDINATE
                KoalaAddrs[koala.KoalaName][2] = _baseKoalaAddress + offset + 0x2AC + (0x518 * modifier * koala.KoalaID);
                //P COORDINATE
                KoalaAddrs[koala.KoalaName][3] = _baseKoalaAddress + offset + 0x2B4 + (0x518 * modifier * koala.KoalaID);
                //Y COORDINATE
                KoalaAddrs[koala.KoalaName][4] = _baseKoalaAddress + offset + 0x2B8 + (0x518 * modifier * koala.KoalaID);
                //R COORDINATE
                KoalaAddrs[koala.KoalaName][5] = _baseKoalaAddress + offset + 0x2BC + (0x518 * modifier * koala.KoalaID);
                //COLLISION
                KoalaAddrs[koala.KoalaName][6] = _baseKoalaAddress + offset + 0x298 + (0x518 * modifier * koala.KoalaID);
                //VISIBILITY
                KoalaAddrs[koala.KoalaName][7] = _baseKoalaAddress + offset + 0x44 + (0x518 * modifier * koala.KoalaID);
            }
            if (!SettingsHandler.DoKoalaCollision) RemoveCollision();
        }

        public void RemoveCollision()
        {
            //WRITES 0 TO COLLISION BYTE
            foreach (Koala koala in Koalas)
            {
                ProcessHandler.WriteData(KoalaAddrs[koala.KoalaName][6], new byte[] {0});
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
            foreach (Koala koala in Koalas)
            {
                ProcessHandler.WriteData(KoalaAddrs[koala.KoalaName][7], new byte[] {1});
            }
        }

        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            string koalaName = message.GetString();
            int level = message.GetInt();
            float[] coordinates = message.GetFloats();

            //SANITY CHECK THAT WE HAVEN'T BEEN SENT OUR OWN COORDINATES AND WE AREN'T LOADING, ON THE MENU, OR IN A DIFFERENT LEVEL 
            if (HGameState.CheckMenuOrLoading() || level != HLevel.CurrentLevelId) return;
            
            //WRITE COORDINATES TO KOALA COORDINATE ADDRESSES
            for (int i = 0; i < coordinates.Length; i++)
            {
                ProcessHandler.WriteData(Program.HKoala.KoalaAddrs[koalaName][i], BitConverter.GetBytes(coordinates[i]));
                //Console.WriteLine("Writing {0:F} to {1:X}", coordinates[i], Program.HKoala.KoalaAddrs[intData[0]][i]);
            }
        }
    }
}
