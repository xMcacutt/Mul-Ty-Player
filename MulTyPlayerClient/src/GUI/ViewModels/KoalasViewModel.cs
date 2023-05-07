using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.Views;
using MulTyPlayerClient.Networking;
using MulTyPlayerCommon;
using PropertyChanged;
using Riptide;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MulTyPlayerClient.GUI.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class KoalasViewModel : BaseViewModel
    {
        public KoalaSelectEntry Boonie { get; set; }

        public bool BoonieAvailable { get; set; } = true;
        public bool MimAvailable { get; set; } = true;
        public bool GummyAvailable { get; set; } = true;
        public bool SnugsAvailable { get; set; } = true;
        public bool KatieAvailable { get; set; } = true;
        public bool KikiAvailable { get; set; } = true;
        public bool ElizabethAvailable { get; set; } = true;
        public bool DubboAvailable { get; set; } = true;

        public bool BlockKoalaSelect { get; set; }

        public event EventHandler KoalaClicked;

        public KoalasViewModel() : base()
        {
            Replication.HPlayer = new PlayerHandler();
            Replication.KoalaHandler = new Memory.KoalaReplication();
        }

        public async void OnKoalaClicked(string koalaName)
        {
            BlockKoalaSelect = true;
            Koala koala = new(koalaName, Array.IndexOf(Memory.KoalaReplication.KoalaNames, koalaName));
            Replication.OldKoala = koala;
            bool isHost = !CommandHandler.HostExists();
            PlayerHandler.Players.Add(ConnectionService.Client.Id, new Player(koala, Replication.Name, ConnectionService.Client.Id, isHost, false));
            MainViewModel.SFXPlayer.PlaySound(SFX.PlayerConnect);
            PlayerHandler.AnnounceSelection(koalaName, Replication.Name, isHost);
            await Task.Delay(2400);
            SetAvailability(koalaName, false);
            KoalaClicked?.Invoke(koalaName, new EventArgs());
            MoveToView(View.Lobby);
            BlockKoalaSelect = false;
            
        }

        public bool IsKoalaAvailable(string koalaName)
        {
            //return koalaAvailability[koalaName];
            switch (koalaName)
            {
                case "Boonie": return BoonieAvailable;
                case "Mim": return MimAvailable;
                case "Gummy": return GummyAvailable;
                case "Snugs": return SnugsAvailable;
                case "Katie": return KatieAvailable;
                case "Kiki": return KikiAvailable;
                case "Elizabeth": return ElizabethAvailable;
                case "Dubbo": return DubboAvailable;
            }
            return false;
        }

        public void SwitchAvailability(string koalaName)
        {
            //koalaAvailability[koalaName] ^= true;
            switch (koalaName)
            {
                case "Boonie": BoonieAvailable ^= true; break;
                case "Mim": MimAvailable ^= true; break;
                case "Gummy": GummyAvailable ^= true; break;
                case "Snugs": SnugsAvailable ^= true; break;
                case "Katie": KatieAvailable ^= true; break;
                case "Kiki": KikiAvailable ^= true; break;
                case "Elizabeth": ElizabethAvailable ^= true; break;
                case "Dubbo": DubboAvailable ^= true; break;
            }
        }

        public void SetAvailability(string koalaName, bool available)
        {
            //koalaAvailability[koalaName] = available;
            switch (koalaName)
            {
                case "Boonie": BoonieAvailable = available; break;
                case "Mim": MimAvailable = available; break;
                case "Gummy": GummyAvailable = available; break;
                case "Snugs": SnugsAvailable = available; break;
                case "Katie": KatieAvailable = available; break;
                case "Kiki": KikiAvailable = available; break;
                case "Elizabeth": ElizabethAvailable = available; break;
                case "Dubbo": DubboAvailable = available; break;
            }            
        }

        public void MakeAllAvailable()
        {
            BoonieAvailable = true;
            MimAvailable = true;
            GummyAvailable = true;
            SnugsAvailable = true;
            KatieAvailable = true;
            KikiAvailable = true;
            ElizabethAvailable = true;
            DubboAvailable = true;
        }
    }
}
