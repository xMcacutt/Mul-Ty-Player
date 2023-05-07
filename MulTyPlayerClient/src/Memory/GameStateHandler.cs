using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.ViewModels;
using MulTyPlayerClient.Networking;

namespace MulTyPlayerClient.Memory
{
    public class GameStateHandler
    {
        public bool CurrentlyLoading;
        public bool WasLoadingLastFrame;

        public bool CheckMenuOrLoading()
        {
            ProcessHandler.TryRead(0x27EBCC, out bool result, true);
            return !result;
            //IF METHOD RETURNS TRUE -> ON MENU
        }

        public bool CheckMainMenu()
        {
            ProcessHandler.TryRead(0x286641, out bool result, true);
            bool onMenu = !result;
            MainViewModel.Lobby.IsOnMenu = onMenu;
            if (PlayerHandler.TryGetLocalPlayer(out Player player))
                player.IsReady &= onMenu;
            MainViewModel.Lobby.UpdateReadyStatus();

            if (onMenu && MainViewModel.Lobby.TryGetPlayerInfo(ConnectionService.Client.Id, out PlayerInfo playerInfo))
            {
                playerInfo.Level = "M/L";
            }
            return onMenu;
        }

        public bool CheckLoaded()
        {
            CurrentlyLoading = CheckMenuOrLoading();
            CheckMainMenu();
            if (WasLoadingLastFrame && !CurrentlyLoading)
            {
                LevelHandler.DoLevelSetup();
            }
            WasLoadingLastFrame = CurrentlyLoading;
            return CurrentlyLoading;
        }
    }
}
