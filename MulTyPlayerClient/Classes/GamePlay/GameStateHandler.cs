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
        public bool LoadingState { get; set; }
        public bool PreviousLoadingState { get; set; }

        public async Task<bool> CheckMenuOrLoading()
        {
            return (await ProcessHandler.ReadDataAsync(PointerCalculations.AddOffset(0x25601C), 1))[0] == 0;
            //IF METHOD RETURNS TRUE -> ON MENU
        }

        public async Task CheckLoaded()
        {
            PreviousLoadingState = LoadingState;
            LoadingState = await CheckMenuOrLoading();
            if (PreviousLoadingState != LoadingState)
            {
                PreviousLoadingState = LoadingState;
                if (!LoadingState)
                {
                    Client.HLevel.bNewLevelSetup = false;
                    Client.HLevel.LoadedNewLevelNetworkingSetupDone = false;
                }
            }
        }
    }
}
