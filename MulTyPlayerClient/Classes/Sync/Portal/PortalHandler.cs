using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;

namespace MulTyPlayerClient
{
    internal class PortalHandler : SyncObjectHandler
    {
        Dictionary<int, int> NumberOfTimesEnteredLevel;
        Dictionary<int, int> PreviousNumberOfTimesEnteredLevel;
        public static int[] FlakyPortals = { 4, 7, 15, 19, 22, 23, 24 };
        public Dictionary<int, bool> portalsActive;

        public PortalHandler()
        {
            Name = "Portal";
            WriteState = 1;
            CheckState = 3;
            CounterAddress = SyncHandler.SaveDataBaseAddress;
            CounterByteLength = 1;
            PreviousObserverState = 0;
            ObserverState = 0;
            LiveSync = new LivePortalSyncer(this);
            SaveSync = new SavePortalSyncer();
            SetSyncClasses(LiveSync, SaveSync);
            NumberOfTimesEnteredLevel = new();
            PreviousNumberOfTimesEnteredLevel = new();
            portalsActive = new();
            for (int i = 0; i < 24; i++) NumberOfTimesEnteredLevel.Add(i, 0);
            for (int i = 0; i< 24; i++) PreviousNumberOfTimesEnteredLevel.Add(i, 0);
            foreach(int level in FlakyPortals) portalsActive.Add(level, false);
        }

        public override void HandleClientUpdate(int iLive, int iSave, int level)
        {
            SaveSync.Save(iSave, level);
            if (level != 0) return;
            LiveSync.Collect(iLive);
        }

        public override int ReadObserver(int address, int size)
        {
            int bytesRead = 0;
            int count = 0;
            byte[] buffer = new byte[size];
            for (int i = 0; i < 24; i++)
            {
                ProcessHandler.ReadProcessMemory((int)ProcessHandler.HProcess, address + (0x70 * i), buffer, size, ref bytesRead);
                NumberOfTimesEnteredLevel[i] = buffer[0];
            }
            foreach (int i in FlakyPortals)
            {
                portalsActive[i] = NumberOfTimesEnteredLevel[i] > 0;
                if (NumberOfTimesEnteredLevel[i] > 0)
                {
                    portalsActive[i] = true;
                    count++;
                }
            }
            return count;
        }

        public override void CheckObserverChanged()
        {
            ObserverState = ReadObserver(CounterAddress, CounterByteLength);
            if (PreviousObserverState == ObserverState || ObserverState == 0) return;

            PreviousObserverState = ObserverState;
            
            foreach(int i in FlakyPortals)
            {
                if (portalsActive[i])
                {
                    Program.HSync.SendDataToServer(i, i, i, Name);
                } 
            }
        }

        public override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x267408), 0x0);
        }
    }
}
