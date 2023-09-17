using MulTyPlayerClient.GUI;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using MulTyPlayerClient.GUI.Models;
using Riptide;

namespace MulTyPlayerClient
{
    internal class PlayerHandler
    {
        public static Dictionary<ushort, Player> Players = new();

        public PlayerHandler()
        {
            Players = new();
        }

        //Adds other player to players & playerInfo
        public static void AddPlayer(Koala koala, string name, ushort clientID, bool isHost)
        {
            string koalaName = Koalas.GetInfo[koala].Name;
            Players.Add(clientID, new Player(koala, name, clientID, isHost, false));
            PlayerInfo player = new(clientID, name, koalaName);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    ModelController.Lobby.PlayerInfoList.Add(player);
                });
            ModelController.KoalaSelect.SetAvailability(koala, false);
            ModelController.SFXPlayer.PlaySound(SFX.PlayerConnect);
        }

        //Adds yourself to playerInfo
        public static void AnnounceSelection(string koalaName, string name, bool isHost)
        {
            Message message = Message.Create(MessageSendMode.Reliable, MessageID.KoalaSelected);
            message.AddString(koalaName);
            message.AddString(name);
            message.AddUShort(Client._client.Id);
            message.AddBool(isHost);
            PlayerInfo player = new(Client._client.Id, name, koalaName);
            ModelController.Lobby.PlayerInfoList.Add(player);
            Client._client.Send(message);
            Client.KoalaSelected = true;
        }

        // Linq error spam come from client loop
        // client loop calls checkloaded which calls checkmainmenu
        // which tries to set the local playerinfo level to "M/L"
        // local player info is not added until we select a koala
        // so the linq tries to find player where player.id = client.id and fails which throws an exception
        // every frame

        public static void RemovePlayer(ushort id)
        {
            ModelController.KoalaSelect.SetAvailability(Players[id].Koala, true);
            Players.Remove(id);
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                () => {
                    if (ModelController.Lobby.TryGetPlayerInfo(id, out PlayerInfo playerInfo))
                        ModelController.Lobby.PlayerInfoList.Remove(playerInfo);
                }
            );            
        }

        [MessageHandler((ushort)MessageID.AnnounceDisconnect)]
        public static void PeerDisconnected(Message message)
        {
            RemovePlayer(message.GetUShort());
            ModelController.SFXPlayer.PlaySound(SFX.PlayerDisconnect);
        }

        public static bool TryGetLocalPlayer(out Player player)
        {
            return Players.TryGetValue(Client._client.Id, out player);
        }
    }
}
