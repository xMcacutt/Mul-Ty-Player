using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    public class GameStateHandler
    {
        IntPtr HProcess => ProcessHandler.HProcess;
        static LevelHandler HLevel => Program.HLevel;

        public bool LoadingState { get; set; }
        public bool PreviousLoadingState { get; set; }

        public bool CheckLoading()
        {
            int bytesRead = 0;
            byte[] loading = new byte[1];
            ProcessHandler.ReadProcessMemory((int)HProcess, PointerCalculations.AddOffset(0x286555), loading, 1, ref bytesRead);
            return BitConverter.ToBoolean(loading, 0);
        }

        public bool CheckMenu()
        {
            int bytesRead = 0;
            byte[] menu = new byte[1];
            ProcessHandler.ReadProcessMemory((int)HProcess, PointerCalculations.AddOffset(0x286640), menu, 1, ref bytesRead);
            return menu[0] == 0;
        }

        public void CheckLoaded()
        {
            PreviousLoadingState = LoadingState;
            LoadingState = CheckLoading();
            if (PreviousLoadingState != LoadingState)
            {
                PreviousLoadingState = LoadingState;
                if (!LoadingState) HLevel.LoadedIntoNewLevelStuffDone = false;
            }
        }
    }
}
