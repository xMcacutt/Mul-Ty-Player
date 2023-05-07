using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using MulTyPlayerClient.Networking;
using MulTyPlayerCommon;

namespace MulTyPlayerClient.Memory
{
    internal class KoalaReplication
    {
        static GameStateHandler HGameState => Replication.HGameState;
        static LevelHandler HLevel => Replication.HLevel;

        public Dictionary<string, int[]> KoalaAddrs;
        public static string[] KoalaNames = { "Katie", "Mim", "Elizabeth", "Snugs", "Gummy", "Dubbo", "Kiki", "Boonie" };
        int _bTimeAttackAddress = 0x28AB84;
        int _baseKoalaAddress;

        public KoalaReplication()
        {
            SetBaseAddress();
            CreateKoalaAddrArrays();
        }

        [Riptide.MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void HandleKoalaSelected(Riptide.Message message)
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

        //Called once in setup
        public void SetBaseAddress()
        {
            _baseKoalaAddress = Memory.Addresses.GetPointerAddress(0x26B070, new int[] { 0x0 });
        }

        //Called once per level setup
        public void SetCoordAddrs()
        {
            //KOALAS ARE STRUCTURED DIFFERENTLY IN STUMP AND SNOW SO MODIFIER AND OFFSET ARE NECESSARY
            bool levelHasKoalas = Levels.GetLevelData(LevelHandler.CurrentLevelId).HasKoalas;
            int modifier = levelHasKoalas ? 2 : 1;
            int offset = levelHasKoalas ? 0x518 : 0x0;

            if (_baseKoalaAddress == 0)
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
            if (!Settings.SettingsHandler.Settings.DoKoalaCollision) RemoveCollision();
        }

        public void RemoveCollision()
        {
            var _players = PlayerHandler.Players.Values;
            //WRITES 0 TO COLLISION BYTE
            foreach (Player player in _players)
            {
                ProcessHandler.WriteData(KoalaAddrs[player.Koala.KoalaName][6], new byte[] { 0 }, "Removing collision");
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
                ProcessHandler.WriteData(KoalaAddrs[player.Koala.KoalaName][7], new byte[] { 1 }, "Making players visible");
            }
        }

        [Riptide.MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Riptide.Message message)
        {
            //If this client isnt in game, or hasnt selected a koala, return
            if (!Replication.KoalaSelected || Replication.Relaunching)
                return;

            string koalaName = message.GetString();
            int level = message.GetInt();

            //Return if player is on the main menu or loading screen,
            //No need to set coords
            if (HGameState.CheckMenuOrLoading())
                return;

            //If failed to get this clients player, or received our own coordinates, return
            if (!PlayerHandler.Players.TryGetValue(ConnectionService.Client.Id, out Player p) ||
                p.Koala.KoalaName == koalaName)
                return;

            //If the received player is on a different level, or has finished the game, return
            if (level != LevelHandler.CurrentLevelId || level == Levels.EndGame.Id)
                return;

            float[] coordinates = message.GetFloats();

            int[] koalaValues = Replication.KoalaHandler.KoalaAddrs[koalaName];
            //WRITE COORDINATES TO KOALA COORDINATE ADDRESSES
            for (int i = 0; i < coordinates.Length; i++)
            {
                ProcessHandler.WriteData(koalaValues[i], BitConverter.GetBytes(coordinates[i]), $"Writing coordinates {i} for koala {koalaName} in level {level}");
            }
        }
    }
}
