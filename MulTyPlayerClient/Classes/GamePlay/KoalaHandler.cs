using MulTyPlayerClient.GUI;
using Riptide;
using Steamworks;
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
            bool levelHasKoalas = Levels.GetLevelData(Client.HLevel.CurrentLevelId).HasKoalas;
            int modifier = levelHasKoalas ? 2 : 1;
            int offset = levelHasKoalas ? 0x518 : 0x0;

            if(_baseKoalaAddress == 0)
                SetBaseAddress();

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
            //If this client isnt in game, or hasnt selected a koala, return
            if (!Client.KoalaSelected || Client.Relaunching )
                return;

            bool onMenu = message.GetBool();
            ushort clientID = message.GetUShort();
            string koalaName = message.GetString();
            int level = message.GetInt();

            //Set the incoming players current level code
            if (BasicIoC.MainGUIViewModel.TryGetPlayerInfo(clientID, out PlayerInfo playerInfo))
            {
                if (onMenu)
                {
                    playerInfo.Level = "M/L";
                    return;
                }
                else
                {
                    playerInfo.Level = Levels.GetLevelData(level).Code;
                }
            }

            //Return if player is on the main menu or loading screen,
            //No need to set coords
            if (HGameState.CheckMenuOrLoading())
                return;

            //If failed to get this clients player, or received our own coordinates, return
            if (!PlayerHandler.Players.TryGetValue(Client._client.Id, out Player p) ||
                p.Koala.KoalaName == koalaName)
                return;            
            
            //If the received player is on a different level, or has finished the game, return
            if (level != HLevel.CurrentLevelId || level == Levels.EndGame.Id)
                return;

            float[] coordinates = message.GetFloats();

            int[] koalaValues = Client.HKoala.KoalaAddrs[koalaName];
            //WRITE COORDINATES TO KOALA COORDINATE ADDRESSES
            for (int i = 0; i < coordinates.Length; i++)
            {
                ProcessHandler.WriteData(koalaValues[i], BitConverter.GetBytes(coordinates[i]), $"Writing coordinates {i} for koala {koalaName} in level {level}");
            }
        }
    }
}
