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
        int _bTimeAttackAddress = PointerCalculations.AddOffset(0x28AB84);
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
            PlayerHandler.AddPlayer(koalaName, playerName, clientID);
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

        public async Task SetBaseAddress()
        {
            _baseKoalaAddress = await PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x26B070), new int[] { 0x0 });
        }

        public async Task SetCoordAddrs()
        {
            int modifier = (Client.HLevel.CurrentLevelId == 9 || Client.HLevel.CurrentLevelId == 13) ? 2 : 1;
            int offset = (Client.HLevel.CurrentLevelId == 9 || Client.HLevel.CurrentLevelId == 13) ? 0x518 : 0x0;
            if(_baseKoalaAddress == 0) _baseKoalaAddress = await PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x26B070), new int[] { 0x0 });
            foreach(Player player in PlayerHandler.Players.Values)
            {
                //X COORDINATE
                KoalaAddrs[player.Koala.KoalaName][0] = _baseKoalaAddress + offset + 0x2A4 + (0x518 * modifier * player.Koala.KoalaID);
                //Y COORDINATE
                KoalaAddrs[player.Koala.KoalaName][1] = _baseKoalaAddress + offset + 0x2A8 + (0x518 * modifier * player.Koala.KoalaID);
                //Z COORDINATE
                KoalaAddrs[player.Koala.KoalaName][2] = _baseKoalaAddress + offset + 0x2AC + (0x518 * modifier * player.Koala.KoalaID);
                //P COORDINATE
                KoalaAddrs[player.Koala.KoalaName][3] = _baseKoalaAddress + offset + 0x2B4 + (0x518 * modifier * player.Koala.KoalaID);
                //Y COORDINATE
                KoalaAddrs[player.Koala.KoalaName][4] = _baseKoalaAddress + offset + 0x2B8 + (0x518 * modifier * player.Koala.KoalaID);
                //R COORDINATE
                KoalaAddrs[player.Koala.KoalaName][5] = _baseKoalaAddress + offset + 0x2BC + (0x518 * modifier * player.Koala.KoalaID);
                //COLLISION
                KoalaAddrs[player.Koala.KoalaName][6] = _baseKoalaAddress + offset + 0x298 + (0x518 * modifier * player.Koala.KoalaID);
                //VISIBILITY
                KoalaAddrs[player.Koala.KoalaName][7] = _baseKoalaAddress + offset + 0x44 + (0x518 * modifier * player.Koala.KoalaID);
            }
            if (!SettingsHandler.Settings.DoKoalaCollision) await RemoveCollision();
        }

        public async Task RemoveCollision()
        {
            var _players = PlayerHandler.Players.Values;
            //WRITES 0 TO COLLISION BYTE
            foreach (Player player in _players)
            {
                await ProcessHandler.WriteDataAsync(KoalaAddrs[player.Koala.KoalaName][6], new byte[] {0});
            }
        }

        public async Task CheckTA()
        {
            if (BitConverter.ToInt32(await ProcessHandler.ReadDataAsync(_bTimeAttackAddress, 4), 0) == 1) await MakeVisible();
        }
        
        public async Task MakeVisible()
        {
            foreach (Player player in PlayerHandler.Players.Values)
            {
                await ProcessHandler.WriteDataAsync(KoalaAddrs[player.Koala.KoalaName][7], new byte[] {1});
            }
        }

        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static async Task HandleGettingCoordinates(Message message)
        {
            if (!Client.KoalaSelected) return;
            string koalaName = message.GetString();
            int level = message.GetInt();
            float[] coordinates = message.GetFloats();

            //SANITY CHECK THAT WE HAVEN'T BEEN SENT OUR OWN COORDINATES AND WE AREN'T LOADING, ON THE MENU, OR IN A DIFFERENT LEVEL 
            if (await HGameState.CheckMenuOrLoading() || level != HLevel.CurrentLevelId || PlayerHandler.Players[Client._client.Id].Koala.KoalaName == koalaName) return;

            //WRITE COORDINATES TO KOALA COORDINATE ADDRESSES
            for (int i = 0; i < coordinates.Length; i++)
            {
                await ProcessHandler.WriteDataAsync(Client.HKoala.KoalaAddrs[koalaName][i], BitConverter.GetBytes(coordinates[i]));
            }
        }
    }
}
