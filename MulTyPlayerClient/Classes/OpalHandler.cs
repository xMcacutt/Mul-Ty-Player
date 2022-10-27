using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Classes
{
    internal class OpalHandler
    {
        IntPtr HProcess => ProcessHandler.HProcess;
        static SyncHandler HSync => Program.HSync;
        static LevelHandler HLevel => Program.HLevel;

        int currentOpalDataBaseAddr = 0x0028AB7C;
        int[] currentOpalDataOffsets = { 0x180, 0x150, 0x3C, 0x14C, 0x120, 0x88 };
        int levelOpalDataBaseAddr = 0x00288730;
        int levelOpalDataOffset = 0x1D1;

        static public int currentOpalDataAddress;
        static public int levelOpalDataAddress;

        public byte[] CurrentOpalData;
        public byte[] PreviousOpalData;
        public Dictionary<int, byte[]> LevelOpalData;
        public int OpalCount;
        public int PreviousOpalCount;
        static int serverOpalCount;

        //GET ALL 300 OPALS FROM CURRENT OPAL DATA AND STORE IN BYTE ARRAY
        //GET OPAL COUNT FROM GAME
        //IF OPAL COUNT CHANGES AND OPAL COUNT IS NOT EQUAL TO OPAL COUNT RECEIVED FROM SERVER,
            //GET ALL 300 OPALS FROM CURRENT OPAL DATA AND STORE IN BYTE ARRAY
            //SEND ARRAY TO SERVER
        //MESSAGE HANDLER RECEIVES DATA BACK AND
        //IF IN SAME LEVEL
            //WRITE THE RETURNED CURRENT OPALS INTO THE GAME
        //CONVERT TO SAVE GAME OPAL DATA AND WRITE TO SAVE GAME DATA

        public OpalHandler()
        {
            currentOpalDataAddress = PointerCalculations.GetPointerAddressNegative(
                PointerCalculations.AddOffset(currentOpalDataAddress), 
                currentOpalDataOffsets, 
                0x6B58);
            levelOpalDataAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(levelOpalDataBaseAddr), levelOpalDataOffset);

            LevelOpalData = new Dictionary<int, byte[]>
            {
                { 4, ReadLevelData(0) },
                { 5, ReadLevelData(1) },
                { 6, ReadLevelData(2) },
                { 8, ReadLevelData(4) },
                { 9, ReadLevelData(5) },
                { 10, ReadLevelData(6) },
                { 12, ReadLevelData(8) },
                { 13, ReadLevelData(9) },
                { 14, ReadLevelData(10) }
            };

            PreviousOpalData = new byte[300];
            CurrentOpalData = new byte[300];
        }

        public void CheckCount()
        {
            ReadOpalCount();
            if (PreviousOpalCount == OpalCount) return;  
            PreviousOpalCount = OpalCount;
            if (OpalCount == serverOpalCount) return;
            CurrentOpalData = ReadCurrentOpals();
            for (int i = 0; i < 300; i++)
            {
                if (PreviousOpalData[i] != CurrentOpalData[i])
                {
                    SendCollected(HLevel.CurrentLevelId, i);
                    PreviousOpalData[i] = CurrentOpalData[i];
                    i = 300;
                }
            }
        }

        public void WriteCurrentOpalsSync()
        {
            byte[] collected = new byte[1] { 3 };
            for (int i = 0; i < 300; i++)
            {
                if (CurrentOpalData[i] == 5)
                {
                    ProcessHandler.WriteData(currentOpalDataAddress + 0x114 * (i - 4), collected);
                }
            }
        }

        [MessageHandler((ushort)MessageID.OpalCollected)]
        static void HandleOpalCollected(Message message)
        {
            int level = message.GetInt();
            int opal = message.GetInt();
            byte[] saveDataBytes = message.GetBytes();
            int opalCountForLevel = message.GetInt();

            if (level == HLevel.CurrentLevelId)
            {
                serverOpalCount = opalCountForLevel;
                ProcessHandler.WriteData(currentOpalDataAddress + 0x114 * opal, BitConverter.GetBytes(3));
            }
            ProcessHandler.WriteData(levelOpalDataAddress + 70 * (level - 4), saveDataBytes);
        }

        void SendCollected(int levelId, int opalIndex)
        {
            Message message = Message.Create(MessageSendMode.reliable, MessageID.OpalCollected);
            message.AddInt(levelId);
            message.AddInt(opalIndex);
            Client._client.Send(message);
        }

        void ReadOpalCount()
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            ProcessHandler.ReadProcessMemory((int)HProcess, PointerCalculations.AddOffset(0x2888B0), buffer, 4, ref bytesRead);
            OpalCount = BitConverter.ToInt32(buffer, 0);
        }

        byte[] ReadLevelData(int levelIndex)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[38];
            ProcessHandler.ReadProcessMemory((int)HProcess, levelOpalDataAddress + (0x70 * levelIndex), buffer, 38, ref bytesRead);
            return buffer;
        }

        byte[] ReadCurrentOpals()
        {
            byte[] currentOpals = new byte[300];
            int bytesRead = 0;
            byte[] buffer = new byte[1];
            for(int i = 0; i < 300; i++)
            {
                ProcessHandler.ReadProcessMemory((int)HProcess, currentOpalDataAddress, buffer, 1, ref bytesRead);
                currentOpals[i] = buffer[0];
            }
            return currentOpals;
        }

    }
}
