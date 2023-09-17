using MulTyPlayerClient.Classes.Networking;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using Riptide;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MulTyPlayerClient
{
    internal class KoalaHandler
    {
        public KoalaTransformPtrs[] koalaTransforms;
        
        static GameStateHandler HGameState => Client.HGameState;
        static LevelHandler HLevel => Client.HLevel;

        static InterpolationMode interpolationMode = InterpolationMode.None;
        static bool readyToWriteTransformData = false;

        public struct KoalaTransformPtrs
        {
            public int X, Y, Z, Pitch, Yaw, Roll, Collision, Visibility;
        }

        struct TransformSnapshot
        {
            public float[] Transform;
            public DateTime Timestamp;

            public TransformSnapshot(float[] kt, DateTime dt)
            {
                Transform = kt;
                Timestamp = dt;
            }
        }

        struct TransformSnapshotPair
        {
            public TransformSnapshot Old;
            public TransformSnapshot New;
        }

        static int _bTimeAttackAddress = 0x28AB84;
        static int _baseKoalaAddress;

        //                      koala id
        private static Dictionary<int, TransformSnapshotPair> playerKoalaTransforms = new();

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
            Client.HKoala.SetCoordAddrs();
            Client.HSync.SetMemAddrs();
            Client.HSync.RequestSync();
            playerKoalaTransforms.Add((int)k, new TransformSnapshotPair());
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
            readyToWriteTransformData = false;
            //KOALAS ARE STRUCTURED DIFFERENTLY IN STUMP AND SNOW SO MODIFIER AND OFFSET ARE NECESSARY
            bool levelHasKoalas = Levels.GetLevelData(Client.HLevel.CurrentLevelId).HasKoalas;
            int modifier = levelHasKoalas ? 2 : 1;
            int offset = levelHasKoalas ? 0x518 : 0x0;

            if(_baseKoalaAddress == 0)
                SetBaseAddress();

            foreach (Player player in PlayerHandler.Players.Values)
            {
                Logger.Instance.Write("WRITING KOALA ADDRESS FOR PLAYER: " + player.Name);
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

            if (interpolationMode == InterpolationMode.None)
            {
                //Do nothing, write coords as received
                WriteCoordinateData(koalaID, transform);
            }
            else
            {
                //Writes to the transform array in-place
                InterpolateKoalaTransforms(koalaID, transform); 
            }            
        }

        private static void InterpolateKoalaTransforms(int koalaID, float[] incomingTransform)
        {
            TransformSnapshot newTS = new(incomingTransform, DateTime.Now);
            var pair = playerKoalaTransforms[koalaID];
            pair.Old = pair.New;
            pair.New = newTS;

            switch (interpolationMode)
            {
                case InterpolationMode.None:
                    {
                        //Do nothing, write coords as received
                        //Performant, reliable, but looks stuttery
                        return;
                    }
                case InterpolationMode.Interpolate:
                    {
                        // Lerp between the second last transform and the last transform received, based on how much time has passed.
                        // Stops once the last transform has been reached with no further packets received
                        // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
                        Interpolation.LerpFloatsNonAllocClamped(
                            pair.Old.Transform, pair.Old.Timestamp,
                            pair.New.Transform, pair.New.Timestamp,
                            incomingTransform, 6);
                        break;
                    }
                case InterpolationMode.Extrapolate:
                    {
                        // Lerp between the second last transform and the last transform received, based on how much time has passed.
                        // Does not stop upon reaching the latest transform, attempts to extrapolate further
                        // Looks smoother, but costs cpu, and adds latency to movement (~20-30ms)
                        // May be better than interpolate for players with shaky connections
                        Interpolation.LerpFloatsNonAlloc(
                            pair.Old.Transform, pair.Old.Timestamp,
                            pair.New.Transform, pair.New.Timestamp,
                            incomingTransform, 6);
                        break;
                    }
                case InterpolationMode.Predictive:
                    {
                        // Predict and extrapolate the koalas movement based on the last two transforms received
                        // Smoother than none, no latency, but costs cpu
                        // Could be unpredictable/innaccurate with shaky connections
                        Interpolation.PredictFloatsNonAlloc(
                            pair.Old.Transform, pair.Old.Timestamp,
                            pair.New.Transform, pair.New.Timestamp,
                            incomingTransform, 6);
                        break;
                    }
            }
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
