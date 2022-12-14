using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    public class GameStateHandler
    {
        static SyncHandler HSync => Program.HSync;

        public bool LoadingState { get; set; }
        public bool PreviousLoadingState { get; set; }

        public bool CheckMenuOrLoading()
        {
            byte[] buffer = new byte[1];
            int bytesRead = 0;
            ProcessHandler.ReadProcessMemory(checked((int)ProcessHandler.HProcess), PointerCalculations.AddOffset(0x25601C), buffer, 1, ref bytesRead);
            return buffer[0] == 0;
            //IF METHOD RETURNS TRUE -> ON MENU
        }

        public void CheckLoaded()
        {
            PreviousLoadingState = LoadingState;
            LoadingState = CheckMenuOrLoading();
            if (PreviousLoadingState != LoadingState)
            {
                PreviousLoadingState = LoadingState;
                if (!LoadingState)
                {
                    Program.HLevel.LoadedNewLevelGameplaySetupDone = false;
                    Program.HLevel.LoadedNewLevelNetworkingSetupDone = false;
                }
            }
        }

        public void GetTyData(object token)
        {
            while (!Client.IsRunning)
            {

            }
            while (Client.IsRunning)
            {
                CheckLoaded();
                if (!CheckMenuOrLoading())
                {
                    if (!Program.HLevel.LoadedNewLevelGameplaySetupDone)
                    {
                        Thread.Sleep(1000);
                        Program.HLevel.DoLevelSetup();
                    }
                    Program.HLevel.GetCurrentLevel();
                    if (SettingsHandler.DoOpalSyncing) {SyncHandler.HOpal.CheckObserverChanged(); SyncHandler.HCrate.CheckObserverChanged();}
                    if (SettingsHandler.DoTESyncing) SyncHandler.HThEg.CheckObserverChanged();
                    if (SettingsHandler.DoCogSyncing) SyncHandler.HCog.CheckObserverChanged();
                    if (SettingsHandler.DoBilbySyncing) SyncHandler.HBilby.CheckObserverChanged();
                    if (SettingsHandler.DoRangSyncing) SyncHandler.HAttribute.CheckObserverChanged();
                    if (SettingsHandler.DoPortalSyncing) SyncHandler.HPortal.CheckObserverChanged();
                    if (SettingsHandler.DoCliffsSyncing) SyncHandler.HCliffs.CheckObserverChanged();
                    Program.HHero.GetTyPosRot(); 
                    Program.HKoala.SetCoordAddrs();
                }
                Thread.Sleep(10);
            }
        }
    }
}
