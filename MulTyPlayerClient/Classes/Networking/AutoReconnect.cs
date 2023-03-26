using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class AutoReconnect
    {
        public static void Connected()
        {
            BasicIoC.KoalaSelectViewModel.Setup();
            Koala koala;
            if (Client.OldKoala != null && BasicIoC.KoalaSelectViewModel.KoalaAvailable(Client.OldKoala.KoalaName))
            {
                koala = Client.OldKoala;
            }
            else
            {
                var properties = BasicIoC.KoalaSelectViewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          .Where(p => p.PropertyType == typeof(bool) && p.Name.EndsWith("Available"));
                foreach (var prop in properties)
                {
                    if (!(bool)prop.GetValue(BasicIoC.KoalaSelectViewModel))
                    {
                        string s = prop.Name.SkipLast(9).ToString();
                        koala = new Koala(s, Array.IndexOf(KoalaHandler.KoalaNames, s));
                        break;
                    }
                }
            }
            PlayerHandler.Players.Add(Client._client.Id, new Player(koala, Client.Name, Client._client.Id));;
            SFXPlayer.PlaySound(SFX.PlayerConnect);
            PlayerHandler.AnnounceSelection(koala.KoalaName, Client.Name);
            BasicIoC.KoalaSelectViewModel.SwitchAvailability(koala.KoalaName);
            BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = true;
            BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
            Client.IsRunning = true;
        }

        public static void ConnectionFailed()
        {
            Client.IsRunning = false;
            BasicIoC.LoginViewModel.ConnectionAttemptSuccessful = false;
            BasicIoC.LoginViewModel.ConnectionAttemptCompleted = true;
            return;
        }
    }
}
