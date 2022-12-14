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
        public static int[] LivePortalOrder = { 7, 5, 4, 13, 10, 23, 20, 19, 9, 21, 22, 12, 8, 6, 14, 15 };
        public static int[] FlakyPortals = { 4, 7, 15, 19, 21, 22, 23 };
        public Dictionary<int, byte> PortalsActive;
        public Dictionary<int, byte> OldPortalsActive;

        public PortalHandler()
        {
            Name = "Portal";
            WriteState = 1;
            CheckState = 3;
            CounterByteLength = 1;
            PreviousObserverState = 0;
            ObserverState = 0;
            LiveSync = new LivePortalSyncer(this);
            SetSyncClasses(LiveSync, SaveSync);
            PortalsActive = new();
            OldPortalsActive = new();
            foreach (int level in FlakyPortals)
            {
                PortalsActive.Add(level, 0);
                OldPortalsActive.Add(level, 0);
            }
        }

        public override void HandleClientUpdate(int null1, int null2, int level)
        {
            PortalsActive[level] = 1;
            if (Program.HLevel.CurrentLevelId != 0) return;
            //Console.WriteLine("Spawning Portal");
            LiveSync.Collect(level);
        }

        public override void Sync(int null1, byte[] active, byte[] null2)
        {
            for (int i = 0; i < 7; i++)
            {
                if (active[i] == 1)
                {
                    HandleClientUpdate(null1, null1, FlakyPortals[i]);
                }
            }
        }

        public override int ReadObserver(int address, int size)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[size];
            int[] portals = new int[7];
            for (int i = 0; i < 7; i++)
            {
                ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), SyncHandler.SaveDataBaseAddress + (0x70 * FlakyPortals[i]), buffer, 1, ref bytesRead);
                if (buffer[0] > 0)
                {
                    if (PortalsActive[FlakyPortals[i]] == 0) PortalsActive[FlakyPortals[i]] = 1;
                    portals[i] = 1;
                }

                if (Program.HLevel.CurrentLevelId == 0)
                {
                    int orderedIndex = Array.IndexOf(LivePortalOrder, FlakyPortals[i]);
                    ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), address + (LiveSync.ObjectLength * orderedIndex), buffer, 4, ref bytesRead);
                    if (buffer[0] == 2)
                    {
                        if (PortalsActive[FlakyPortals[i]] == 0) PortalsActive[FlakyPortals[i]] = 1;
                        portals[i] = 1;
                    }
                }
            }
            return portals.Count(i => i == 1);
        }

        public override void CheckObserverChanged()
        {
            ObserverState = ReadObserver(LiveObjectAddress + LiveSync.StateOffset , CounterByteLength);
            if (PreviousObserverState == ObserverState || ObserverState == 0) return;

            PreviousObserverState = ObserverState;
            
            foreach(int i in FlakyPortals)
            {
                if (OldPortalsActive[i] == 0 && PortalsActive[i] == 1)
                {
                    //Console.WriteLine("Portal for level " + i + " being sent to server");
                    Program.HSync.SendDataToServer(i, i, i, Name);
                } 
            }
            foreach(int i in OldPortalsActive.Keys) OldPortalsActive[i] = PortalsActive[i]; 
        }

        public override void SetMemAddrs()
        {
            LiveObjectAddress = PointerCalculations.GetPointerAddress(PointerCalculations.AddOffset(0x267408), 0x0);
        }
    }
}
