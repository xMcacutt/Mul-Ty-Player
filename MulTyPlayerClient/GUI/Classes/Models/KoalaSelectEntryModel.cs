using System;

namespace MulTyPlayerClient.GUI.Models
{
    public class KoalaSelectEntryModel
    {
        public KoalaInfo KoalaInfo { get; private set; }

        public Uri LightPortraitSource { get; private set; }
        public Uri DarkPortraitSource { get; private set; }
        public Uri TakenPortraitSource { get; private set; }
        public Uri SelectedAnimationSource { get; private set; }

        public bool IsAvailable { get; set; } = true;

        
        public KoalaSelectEntryModel(Koala koala)
        {
            KoalaInfo = Koalas.GetInfo[koala];
            LightPortraitSource = new Uri(@$"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Light/{KoalaInfo.Name}.png");
            DarkPortraitSource = new Uri(@$"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Dark/{KoalaInfo.Name}.jpg");
            TakenPortraitSource = new Uri(@$"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/Taken/{KoalaInfo.Name}.jpg");
            SelectedAnimationSource = new Uri(@$"pack://siteoforigin:,,,/GUI/KoalaSelectionAssets/mp4/{KoalaInfo.Name}.mp4");
        }
    }
}
