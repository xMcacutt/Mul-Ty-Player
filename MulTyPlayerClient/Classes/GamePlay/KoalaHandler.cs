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
        static GameStateHandler HGameState => Client.HGameState;
        static LevelHandler HLevel => Client.HLevel;
        
        static bool readyToWriteTransformData = false;

        public struct KoalaTransformAddresses
        {
            public int X, Y, Z, Pitch, Yaw, Roll, Collision, Visibility;
        }
        public static KoalaTransformAddresses[] TransformAddresses {get; private set;}

        static int _bTimeAttackAddress = 0x28AB84;
        static int _baseKoalaAddress;

        public KoalaHandler()
        {
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
            Client.HSync.RequestSync();
        }

        public void CreateKoalaAddressArray()
        {
            TransformAddresses = new KoalaTransformAddresses[8];
            for (int i = 0; i < 8; i++)
            {
                TransformAddresses[i] = new KoalaTransformAddresses();
            }
        }

        public void SetBaseAddress()
        {
            _baseKoalaAddress = PointerCalculations.GetPointerAddress(0x26B070, new int[] { 0x0 });
            ProcessHandler.CheckAddress(_baseKoalaAddress, (ushort)(17327608&0xFFFF), "Koalas base address check");
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

            for(int koalaID = 0; koalaID < 8; koalaID++)
            {
                int koalaOffset = 0x518 * modifier * koalaID + offset;
                TransformAddresses[koalaID] = new KoalaTransformAddresses();
                TransformAddresses[koalaID].X = _baseKoalaAddress + 0x2A4 + koalaOffset;
                TransformAddresses[koalaID].Y = _baseKoalaAddress + 0x2A8 + koalaOffset;
                TransformAddresses[koalaID].Z = _baseKoalaAddress + 0x2AC + koalaOffset;
                TransformAddresses[koalaID].Pitch = _baseKoalaAddress + 0x2B4 + koalaOffset;
                TransformAddresses[koalaID].Yaw = _baseKoalaAddress + 0x2B8 + koalaOffset;
                TransformAddresses[koalaID].Roll = _baseKoalaAddress + 0x2BC + koalaOffset;
                TransformAddresses[koalaID].Collision = _baseKoalaAddress + 0x298 + koalaOffset;
                TransformAddresses[koalaID].Visibility = _baseKoalaAddress + 0x44 + koalaOffset;
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
                ProcessHandler.WriteData(TransformAddresses[i].Collision, new byte[] { 0 }, "Removing collision");
            }
        }

        public void CheckTA()
        {
            ProcessHandler.TryRead(_bTimeAttackAddress, out int inTimeAttack, true, "KoalaHandler::CheckTA()");
            if (inTimeAttack == 1)
                MakeVisible();
        }
        
        public void MakeVisible()
        {
            for (int i = 0; i < 8; i++)
            {
                ProcessHandler.WriteData(TransformAddresses[i].Visibility, new byte[] { 1 }, "Making players visible");
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
            //Debug.WriteLine($"Before updating coordinates: {KoalaTransform.DebugTransform(playerTransformAddresses[koalaID].New.Transform)}");
            PlayerReplication.UpdatePlayerSnapshotData(koalaID, transform);
            //Debug.WriteLine($"After updating coordinates: {KoalaTransform.DebugTransform(playerTransformAddresses[koalaID].New.Transform)}");
        }

        
    }
}