using MulTyPlayerClient.Classes.GamePlay;
using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml;

namespace MulTyPlayerClient
{
    internal class KoalaHandler
    {
        public KoalaTransformPtrs[] koalaTransforms;
        const int KOALA_RENDERS_PER_TICK = 3;


        static GameStateHandler HGameState => Client.HGameState;
        static LevelHandler HLevel => Client.HLevel;

        static KoalaInterpolationMode interpolationMode = KoalaInterpolationMode.None;
        static bool readyToWriteTransformData = false;

        public struct KoalaTransformPtrs
        {
            public int X, Y, Z, Pitch, Yaw, Roll, Collision, Visibility;
        }

        static int _bTimeAttackAddress = 0x28AB84;
        static int _baseKoalaAddress;

        //                      koala id
        private static Dictionary<int, KoalaTransform> playerKoalaTransforms = new();

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
            Client.HKoala.SetCoordinateAddresses();
            Client.HLevel.DoLevelSetup();
            playerKoalaTransforms.Remove((int)k);
            playerKoalaTransforms.Add((int)k, new KoalaTransform());
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

        public void SetCoordinateAddresses()
        {
            readyToWriteTransformData = false;
            //KOALAS ARE STRUCTURED DIFFERENTLY IN STUMP AND SNOW SO MODIFIER AND OFFSET ARE NECESSARY
            bool levelHasKoalas = Levels.GetLevelData(Client.HLevel.CurrentLevelId).HasKoalas;
            int modifier = levelHasKoalas ? 2 : 1;
            int offset = levelHasKoalas ? 0x518 : 0x0;

            if(_baseKoalaAddress == 0)
                SetBaseAddress();

            foreach (Player player in PlayerHandler.Players.Values)
            {
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

            MakeVisible();

            if (!SettingsHandler.Settings.DoKoalaCollision)
                RemoveCollision();

            readyToWriteTransformData = true;
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
            if (inTimeAttack == 1)
                MakeVisible();
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
            //Debug.WriteLine("Received koala coord");
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
            if (HGameState.IsAtMainMenuOrLoading())
            {
                readyToWriteTransformData = false;
                return;
            }

            //If failed to get this clients player, or received our own coordinates, return
            if (!PlayerHandler.Players.TryGetValue(Client._client.Id, out Player p) ||
                Koalas.GetInfo[p.Koala].Name == koalaName)
                return;            
            
            //If the received player is on a different level, or has finished the game, return
            if (level != HLevel.CurrentLevelId || level == Levels.EndGame.Id)
                return;

            float[] transform = message.GetFloats();
            //Debug.WriteLine($"Handle coordinates: {KoalaTransform.DebugTransform(transform)}");
            //Debug.WriteLine($"Before updating coordinates: {KoalaTransform.DebugTransform(playerKoalaTransforms[koalaID].New.Transform)}");
            KoalaTransform kt = playerKoalaTransforms[koalaID];
            PositionSnapshot ps = new PositionSnapshot(transform[0..3]);
            kt.UpdatePosition(ps);
            kt.UpdateRotation(transform[3..6]);
            //Debug.WriteLine($"After updating coordinates: {KoalaTransform.DebugTransform(playerKoalaTransforms[koalaID].New.Transform)}");
        }

        public static void RenderKoalas()
        {
            int kRenderSleepTime = (int)((float)Client.MS_PER_TICK / KOALA_RENDERS_PER_TICK);
            for (int i = 1; i < KOALA_RENDERS_PER_TICK; i++)
            {
                foreach (int koalaID in playerKoalaTransforms.Keys)
                {
                    var ts = playerKoalaTransforms[koalaID];

                    var position = ts.GetPosition(interpolationMode);
                    WritePositionToMemory(koalaID, position);

                    var rotation = ts.GetRotation();
                    WriteRotationToMemory(koalaID, rotation);
                }
                Thread.Sleep(kRenderSleepTime);
            }            
        }

        private static void WritePositionToMemory(int koalaID, float[] position)
        {
            KoalaTransformPtrs ktp = Client.HKoala.koalaTransforms[koalaID];
            ProcessHandler.WriteData(ktp.X, BitConverter.GetBytes(position[0]));
            ProcessHandler.WriteData(ktp.Y, BitConverter.GetBytes(position[1]));
            ProcessHandler.WriteData(ktp.Z, BitConverter.GetBytes(position[2]));
        }

        private static void WriteRotationToMemory(int koalaID, float[] rotation)
        {
            KoalaTransformPtrs ktp = Client.HKoala.koalaTransforms[koalaID];

            ProcessHandler.WriteData(ktp.Pitch, BitConverter.GetBytes(rotation[0]));
            ProcessHandler.WriteData(ktp.Yaw, BitConverter.GetBytes(rotation[1]));
            ProcessHandler.WriteData(ktp.Roll, BitConverter.GetBytes(rotation[2]));
        }

        private static void WriteCoordinateData(int koalaID, float[] coordinates)
        {
            KoalaTransformPtrs ktp = Client.HKoala.koalaTransforms[koalaID];

            WritePositionToMemory(koalaID, coordinates[0..3]);
            WriteRotationToMemory(koalaID, coordinates[3..6]);
        }

        internal static KoalaInterpolationMode GetInterpolationMode()
        {
            return interpolationMode;
        }
    }
}