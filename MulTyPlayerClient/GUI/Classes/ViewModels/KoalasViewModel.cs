using PropertyChanged;
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

        //Is available
        public bool BoonieAvailable { get; set; } = true;
        public bool MimAvailable { get; set; } = true;
        public bool GummyAvailable { get; set; } = true;
        public bool SnugsAvailable { get; set; } = true;
        public bool KatieAvailable { get; set; } = true;
        public bool KikiAvailable { get; set; } = true;
        public bool ElizabethAvailable { get; set; } = true;
        public bool DubboAvailable { get; set; } = true;

        //Commands
        public ICommand BoonieClickCommand { get; set; }
        public ICommand MimClickCommand { get; set; }
        public ICommand GummyClickCommand { get; set; }
        public ICommand SnugsClickCommand { get; set; }
        public ICommand KatieClickCommand { get; set; }
        public ICommand KikiClickCommand { get; set; }
        public ICommand ElizabethClickCommand { get; set; }
        public ICommand DubboClickCommand { get; set; }

        //Constructor
        public KoalasViewModel()
        {
            BoonieClickCommand = new RelayCommandWithInputParam(KoalaClicked);
            MimClickCommand = new RelayCommandWithInputParam(KoalaClicked);
            GummyClickCommand = new RelayCommandWithInputParam(KoalaClicked);
            SnugsClickCommand = new RelayCommandWithInputParam(KoalaClicked);
            KatieClickCommand = new RelayCommandWithInputParam(KoalaClicked);
            KikiClickCommand = new RelayCommandWithInputParam(KoalaClicked);
            ElizabethClickCommand = new RelayCommandWithInputParam(KoalaClicked);
            DubboClickCommand = new RelayCommandWithInputParam(KoalaClicked);
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

        public void Setup()
        {
            Client.HPlayer = new PlayerHandler();
            Client.HKoala = new KoalaHandler();
        }

        public void KoalaClicked(object inputParameter)
        {
            PlayerHandler.AddPlayer(inputParameter.ToString(), Client.Name, Client._client.Id);
            PlayerHandler.TellServer(inputParameter, Client.Name, Client._client.Id);
        }
    }
}
