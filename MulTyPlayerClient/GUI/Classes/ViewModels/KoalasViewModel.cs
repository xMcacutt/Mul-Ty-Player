using MulTyPlayerClient.GUI;
using PropertyChanged;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MulTyPlayerClient
{
    [AddINotifyPropertyChangedInterface]
    public class KoalasViewModel
    {
        static KoalaHandler HKoala => Client.HKoala;
        static KoalaHandler HPlayer => Client.HKoala;

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

        public async void KoalaClicked(object inputParameter)
        {
            string koalaName = inputParameter.ToString();
            if (KoalaAvailable(koalaName))
            {
                PlayerHandler.AddPlayer(koalaName, Client.Name, Client._client.Id);
                PlayerHandler.AnnounceSelection(koalaName, Client.Name);

                //Set animation, show it, and wait for it to finish
                KoalaAnimationSource = @"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/" + koalaName + ".mp4";
                BlockKoalaSelect = true;
                ShowAnimation(koalaName);
                await Task.Delay(3500);

                WindowHandler.ClientGUIWindow.Show();
                //Reset back to false for when reopen the koala window
                ShowAnimation(koalaName);
                BlockKoalaSelect= false;
                WindowHandler.KoalaSelectWindow.Close();
            }
        }
    }
}
