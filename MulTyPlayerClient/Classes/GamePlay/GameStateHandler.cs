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
            byte[] buffer = ProcessHandler.ReadData("Menu or Loading", PointerCalculations.AddOffset(0x284DD8), 1);
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
                if (!LoadingState) Program.HLevel.LoadedIntoNewLevelStuffDone = false;
            }
        }

        public void GetTyData(Object token)
        {
            while (!Client.IsRunning)
            {

            }
            while (Client.IsRunning)
            {
                CheckLoaded();
                if (!CheckMenuOrLoading() && !LoadingState)
                {
                    Program.HLevel.GetCurrentLevel();
                    if (SettingsHandler.DoOpalSyncing) SyncHandler.HOpal.CheckObserverChanged();
                    if (SettingsHandler.DoTESyncing) SyncHandler.HThEg.CheckObserverChanged();
                    if (SettingsHandler.DoCogSyncing) SyncHandler.HCog.CheckObserverChanged();
                    if (SettingsHandler.DoBilbySyncing) SyncHandler.HBilby.CheckObserverChanged();
                    Program.HHero.GetTyPosRot();
                    Program.HKoala.SetCoordAddrs();
                }
                Thread.Sleep(10);
            }
        }
    }
}
