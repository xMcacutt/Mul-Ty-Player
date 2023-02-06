using PropertyChanged;
using System.Windows.Input;

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

        public void Setup()
        {

        }

        //Just placeholder functionallity
        public void KoalaClicked(object inputParameter)
        {
            switch (inputParameter)
            {
                case "Boonie":
                    {
                        BoonieAvailable = false;
                        break;
                    }
                case "Mim":
                    {
                        MimAvailable = false;
                        break;
                    }
                case "Gummy":
                    {
                        GummyAvailable = false;
                        break;
                    }
                case "Snugs":
                    {
                        SnugsAvailable = false;
                        break;
                    }
                case "Katie":
                    {
                        KatieAvailable = false;
                        break;
                    }
                case "Kiki":
                    {
                        KikiAvailable = false;
                        break;
                    }
                case "Elizabeth":
                    {
                        ElizabethAvailable = false;
                        break;
                    }
                case "Dubbo":
                    {
                        DubboAvailable = false;
                        break;
                    }
            }
        }
    }
}
