using Microsoft.CodeAnalysis.CSharp.Syntax;
using MulTyPlayerClient.GUI;
using PropertyChanged;
using Riptide;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;

namespace MulTyPlayerClient
{
    [AddINotifyPropertyChangedInterface]
    public class KoalasViewModel
    {
        //Images
        public string Boonie { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Boonie.jpg";
        public string Mim { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Mim.jpg";
        public string Gummy { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Gummy.jpg";
        public string Snugs { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Snugs.jpg";
        public string Katie { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Katie.jpg";
        public string Kiki { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Kiki.jpg";
        public string Elizabeth { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Elizabeth.jpg";
        public string Dubbo { get; set; } = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/Dubbo.jpg";

        public bool BoonieAvailable => koalaAvailability["Boonie"];
        public bool MimAvailable => koalaAvailability["Mim"];
        public bool GummyAvailable => koalaAvailability["Gummy"];
        public bool SnugsAvailable => koalaAvailability["Snugs"];
        public bool KatieAvailable => koalaAvailability["Katie"]   ;
        public bool KikiAvailable => koalaAvailability["Kiki"];
        public bool ElizabethAvailable => koalaAvailability["Elizabeth"];
        public bool DubboAvailable => koalaAvailability["Dubbo"];

        Dictionary<string, bool> koalaAvailability;
        public bool BlockKoalaSelect { get; set; }

        //Constructor
        public KoalasViewModel()
        {
            MakeAllAvailable();
        }

        public void Setup()
        {
            Client.HPlayer = new PlayerHandler();
            Client.HKoala = new KoalaHandler();
        }

        public async void KoalaClicked(string koalaName)
        {
            Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
            Client.OldKoala = koala;
            bool isHost = !CommandHandler.HostExists();
            PlayerHandler.Players.Add(Client._client.Id, new Player(koala, Client.Name, Client._client.Id, isHost, false));
            BasicIoC.SFXPlayer.PlaySound(SFX.PlayerConnect);
            BlockKoalaSelect = true;
            PlayerHandler.AnnounceSelection(koalaName, Client.Name, isHost);
            await Task.Delay(2400);
            SetAvailability(koalaName, false);
            WindowHandler.ClientGUIWindow.Show();
            BlockKoalaSelect = false;
            CollectionViewSource.GetDefaultView(BasicIoC.LoggerInstance.Log).Refresh();
            WindowHandler.KoalaSelectWindow.Hide();
        }

        public bool IsKoalaAvailable(string koalaName)
        {
            return koalaAvailability[koalaName];
        }

        public void SwitchAvailability(string koalaName)
        {
            //Bitwise negation
            koalaAvailability[koalaName] ^= true;
        }

        public void SetAvailability(string koalaName, bool available)
        {
            koalaAvailability[koalaName] = available;
        }

        public void MakeAllAvailable()
        {
            koalaAvailability = new()
            {
                { "Boonie", true },
                { "Mim", true },
                { "Gummy", true },
                { "Snugs", true },
                { "Katie", true },
                { "Kiki", true },
                { "Elizabeth", true },
                { "Dubbo", true },
            };
        }

    }
}
