using MulTyPlayerClient.GUI;
using PropertyChanged;
using Riptide;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        public string KoalaAnimationSource { get; set; } = "";

        //Is available
        public bool BoonieAvailable { get; set; } = true;
        public bool MimAvailable { get; set; } = true;
        public bool GummyAvailable { get; set; } = true;
        public bool SnugsAvailable { get; set; } = true;
        public bool KatieAvailable { get; set; } = true;
        public bool KikiAvailable { get; set; } = true;
        public bool ElizabethAvailable { get; set; } = true;
        public bool DubboAvailable { get; set; } = true;

        //Show Animation
        public bool BoonieShowAnimation { get; set; }
        public bool MimShowAnimation { get; set; }
        public bool GummyShowAnimation { get; set; }
        public bool SnugsShowAnimation { get; set; }
        public bool KatieShowAnimation { get; set; }
        public bool KikiShowAnimation { get; set; }
        public bool ElizabethShowAnimation { get; set; }
        public bool DubboShowAnimation { get; set; }

        public bool BlockKoalaSelect { get; set; }

        //Commands
        public ICommand KoalaClickCommand { get; set; }

        //Constructor
        public KoalasViewModel()
        {
            KoalaClickCommand = new RelayCommandWithInputParam(KoalaClicked);
        }

        public bool KoalaAvailable(string koalaName)
        {
            var prop = GetType().GetProperty(koalaName + "Available");
            if (prop != null) return (bool)prop.GetValue(this, null);
            return false;
        }

        public void SwitchAvailability(string koalaName)
        {
            var prop = GetType().GetProperty(koalaName + "Available");
            prop?.SetValue(this, !(bool)prop.GetValue(this, null), null);
        }

        public void MakeAllAvailable()
        {
            var properties = GetType().GetProperties()
                .Where(prop => prop.Name.EndsWith("Available") && prop.PropertyType == typeof(bool));

            foreach (var prop in properties)
            {
                prop.SetValue(this, true, null);
            }
        }

        public void ShowAnimation(string koalaName)
        {
            var prop = GetType().GetProperty(koalaName + "ShowAnimation");
            prop?.SetValue(this, !(bool)prop.GetValue(this, null), null);
        }

        public void Setup()
        {
            Client.HPlayer = new PlayerHandler();
            Client.HKoala = new KoalaHandler();
        }

        public void KoalaClicked(object inputParameter)
        {
            string koalaName = inputParameter.ToString();
            if (KoalaAvailable(koalaName))
            {
                Koala koala = new(koalaName, Array.IndexOf(KoalaHandler.KoalaNames, koalaName));
                Client.OldKoala = koala;
                PlayerHandler.Players.Add(Client._client.Id, new Player(koala, Client.Name, Client._client.Id));
                SFXPlayer.PlaySound(SFX.PlayerConnect);

                KoalaAnimationSource = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/" + koalaName + ".mp4";
                ShowAnimation(koalaName);
                Task.Delay(3500);
                BlockKoalaSelect = true;

                PlayerHandler.AnnounceSelection(koalaName, Client.Name);

                //Set animation, show it, and wait for it to finish


                BasicIoC.KoalaSelectViewModel.SwitchAvailability(koalaName);

                WindowHandler.ClientGUIWindow.Show();
                //Reset back to false for when reopen the koala window
                ShowAnimation(koalaName);
                BlockKoalaSelect= false;
                CollectionViewSource.GetDefaultView(BasicIoC.LoggerInstance.Log).Refresh();
                WindowHandler.KoalaSelectWindow.Hide();
            }
        }
    }
}
