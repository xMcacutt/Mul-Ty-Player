using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class KoalaHandler
    {
        static GameStateHandler HGameState => Client.HGameState;
        static LevelHandler HLevel => Client.HLevel;

        public Dictionary<string, int[]> KoalaAddrs;
        public static string[] KoalaNames = { "Katie", "Mim", "Elizabeth", "Snugs", "Gummy", "Dubbo", "Kiki", "Boonie" };
        int _bTimeAttackAddress = 0x28AB84;
        int _baseKoalaAddress;

        public KoalaHandler()
        {
            SetBaseAddress();
            CreateKoalaAddrArrays();
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void HandleKoalaSelected(Message message)
        {
            string koalaName = message.GetString();
            string playerName = message.GetString();
            ushort clientID = message.GetUShort();
            bool isHost = message.GetBool();
            PlayerHandler.AddPlayer(koalaName, playerName, clientID, isHost);
        }

        public void CreateKoalaAddrArrays()
        {
            //SETUP KOALA ADDRESS DICTIONARY
            KoalaAddrs = new Dictionary<string, int[]>();
            foreach (string koala in KoalaNames)
            {
                KoalaAddrs.Add(koala, new int[8]);
            }
        }

        public void SetBaseAddress()
        {
            _baseKoalaAddress = PointerCalculations.GetPointerAddress(0x26B070, new int[] { 0x0 });
        }

        public void SetCoordAddrs()
        {
            //KOALAS ARE STRUCTURED DIFFERENTLY IN STUMP AND SNOW SO MODIFIER AND OFFSET ARE NECESSARY
            int modifier = (Client.HLevel.CurrentLevelId == 9 || Client.HLevel.CurrentLevelId == 13) ? 2 : 1;
            int offset = (Client.HLevel.CurrentLevelId == 9 || Client.HLevel.CurrentLevelId == 13) ? 0x518 : 0x0;
            if(_baseKoalaAddress == 0) SetBaseAddress();
            foreach (Player player in PlayerHandler.Players.Values)
            {
                if (KoalaAddrs.TryGetValue(player.Koala.KoalaName, out int[] koalaAddr))
                {
                    int koalaOffset = (0x518 * modifier * player.Koala.KoalaID);

                    // X COORDINATE
                    koalaAddr[0] = _baseKoalaAddress + offset + 0x2A4 + koalaOffset;
                    // Y COORDINATE
                    koalaAddr[1] = _baseKoalaAddress + offset + 0x2A8 + koalaOffset;
                    // Z COORDINATE
                    koalaAddr[2] = _baseKoalaAddress + offset + 0x2AC + koalaOffset;
                    // P COORDINATE
                    koalaAddr[3] = _baseKoalaAddress + offset + 0x2B4 + koalaOffset;
                    // Y COORDINATE
                    koalaAddr[4] = _baseKoalaAddress + offset + 0x2B8 + koalaOffset;
                    // R COORDINATE
                    koalaAddr[5] = _baseKoalaAddress + offset + 0x2BC + koalaOffset;
                    // COLLISION
                    koalaAddr[6] = _baseKoalaAddress + offset + 0x298 + koalaOffset;
                    // VISIBILITY
                    koalaAddr[7] = _baseKoalaAddress + offset + 0x44 + koalaOffset;
                }
            }
            if (!SettingsHandler.Settings.DoKoalaCollision) RemoveCollision();
        }

        public void RemoveCollision()
        {
            var _players = PlayerHandler.Players.Values;
            //WRITES 0 TO COLLISION BYTE
            foreach (Player player in _players)
            {
                ProcessHandler.WriteData(KoalaAddrs[player.Koala.KoalaName][6], new byte[] {0}, "Removing collision");
            }
        }

        public void CheckTA()
        {
            ProcessHandler.TryRead(_bTimeAttackAddress, out int inTimeAttack, true);
            if (inTimeAttack == 1) MakeVisible();
        }
        
        public void MakeVisible()
        {
            foreach (Player player in PlayerHandler.Players.Values)
            {
                ProcessHandler.WriteData(KoalaAddrs[player.Koala.KoalaName][7], new byte[] {1}, "Making players visible");
            }
        }

        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            if (!Client.KoalaSelected || Client.Relaunching) return;
            string koalaName = message.GetString();
            int level = message.GetInt();
            float[] coordinates = message.GetFloats();

            //SANITY CHECK THAT WE HAVEN'T BEEN SENT OUR OWN COORDINATES AND WE AREN'T LOADING, ON THE MENU, OR IN A DIFFERENT LEVEL 
            if (PlayerHandler.Players.TryGetValue(Client._client.Id, out Player p)) return;
            if (HGameState.CheckMenuOrLoading() || level != HLevel.CurrentLevelId || p.Koala.KoalaName == koalaName) return;

            //WRITE COORDINATES TO KOALA COORDINATE ADDRESSES
            for (int i = 0; i < coordinates.Length; i++)
            {
                ProcessHandler.WriteData(Client.HKoala.KoalaAddrs[koalaName][i], BitConverter.GetBytes(coordinates[i]), $"Writing coordinates {i} for koala {koalaName} in level {level}");
            }
        }
    }
}
