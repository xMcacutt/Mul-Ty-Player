using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class KoalaHandler
    {
        static GameStateHandler HGameState => Client.HGameState;
        static LevelHandler HLevel => Client.HLevel;

        public KoalaTransformPtrs[] koalaTransforms;

        public struct KoalaTransformPtrs
        {
            public int X;
            public int Y;
            public int Z;
            public int Pitch;
            public int Yaw;
            public int Roll;
            public int Collision;
            public int Visibility;
        }

        int _bTimeAttackAddress = 0x28AB84;
        int _baseKoalaAddress;

        public KoalaHandler()
        {
            SetBaseAddress();
            CreateKoalaAddressArray();
        }

        [MessageHandler((ushort)MessageID.KoalaSelected)]
        private static void HandleKoalaSelected(Message message)
        {
            string koalaName = message.GetString();
            string playerName = message.GetString();
            ushort clientID = message.GetUShort();
            bool isHost = message.GetBool();
            Koala k = Enum.Parse<Koala>(koalaName, true);
            PlayerHandler.AddPlayer(k, playerName, clientID, isHost);
        }

        public void CreateKoalaAddressArray()
        {
            koalaTransforms = new KoalaTransformPtrs[8];
            for (int i = 0; i < 8; i++)
            {
                koalaTransforms[i] = new KoalaTransformPtrs();
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
                ModelController.LoggerInstance.Write("WRITING KOALA ADDRESS FOR PLAYER: " + player.Name);
                int koalaID = (int)player.Koala;
                int koalaOffset = 0x518 * modifier * koalaID + offset;

                koalaTransforms[koalaID] = new KoalaTransformPtrs();
                koalaTransforms[koalaID].X = _baseKoalaAddress + 0x2A4 + koalaOffset;
                koalaTransforms[koalaID].Y = _baseKoalaAddress + 0x2A8 + koalaOffset;
                koalaTransforms[koalaID].Z = _baseKoalaAddress + 0x2AC + koalaOffset;
                koalaTransforms[koalaID].Pitch = _baseKoalaAddress + 0x2B4 + koalaOffset;
                koalaTransforms[koalaID].Yaw = _baseKoalaAddress + 0x2B8 + koalaOffset;
                koalaTransforms[koalaID].Roll = _baseKoalaAddress + 0x2BC + koalaOffset;
                koalaTransforms[koalaID].Collision = _baseKoalaAddress + 0x298 + koalaOffset;
                koalaTransforms[koalaID].Visibility = _baseKoalaAddress + 0x44 + koalaOffset;
            }

            if (!SettingsHandler.Settings.DoKoalaCollision)
                RemoveCollision();
        }

        public void RemoveCollision()
        {
            for (int i = 0; i < 8; i++)
            {
                ProcessHandler.WriteData(koalaTransforms[i].Collision, new byte[] { 0 }, "Removing collision");
            }
        }

        public void CheckTA()
        {
            ProcessHandler.TryRead(_bTimeAttackAddress, out int inTimeAttack, true);
            if (inTimeAttack == 1) MakeVisible();
        }
        
        public void MakeVisible()
        {
            for (int i = 0; i < 8; i++)
            {
                ProcessHandler.WriteData(koalaTransforms[i].Visibility, new byte[] { 1 }, "Making players visible");
            }
        }

        [MessageHandler((ushort)MessageID.KoalaCoordinates)]
        private static void HandleGettingCoordinates(Message message)
        {
            //If this client isnt in game, or hasnt selected a koala, return
            if (!Client.KoalaSelected || Client.Relaunching )
                return;
            Debug.WriteLine("Received koala coord");
            bool onMenu = message.GetBool();
            ushort clientID = message.GetUShort();
            string koalaName = message.GetString();
            Koala k = (Koala)Enum.Parse(typeof(Koala), koalaName, true);
            int koalaID = (int)k;
            int level = message.GetInt();

            //Set the incoming players current level code
            if (ModelController.Lobby.TryGetPlayerInfo(clientID, out PlayerInfo playerInfo))
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
                Koalas.GetInfo[p.Koala].Name == koalaName)
                return;            
            
            //If the received player is on a different level, or has finished the game, return
            if (level != HLevel.CurrentLevelId || level == Levels.EndGame.Id)
                return;

            WriteCoordinateData(koalaID, message.GetFloats());            
        }

        private static void WriteCoordinateData(int koalaID, float[] coordinates)
        {
            KoalaTransformPtrs ktp = Client.HKoala.koalaTransforms[koalaID];

            ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(coordinates[0]));
            ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(coordinates[1]));
            ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(coordinates[2]));

            ProcessHandler.WriteData(ktp.Pitch, BitConverter.GetBytes(coordinates[3]));
            ProcessHandler.WriteData(ktp.Yaw, BitConverter.GetBytes(coordinates[4]));
            ProcessHandler.WriteData(ktp.Roll, BitConverter.GetBytes(coordinates[5]));
        }
    }
}
