using System;
using System.Windows.Controls;
using System.Windows;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.Models
{
    [AddINotifyPropertyChangedInterface]
    public partial class KoalaSelectEntry : UserControl
    {
        public string KoalaName
        {
            get;
            set;
        }

        public bool IsAvailable { get; set; } = true;

        public Uri LightSource { get; set; }
        public Uri DarkSource { get; set; }
        public Uri TakenSource { get; set; }
        public Uri SelectedSource { get; set; }

        public event EventHandler KoalaSelected;

        //as default
        public KoalaSelectEntry()
        {
            LightSource = new Uri(@$"pack://application:,,,/Resources/KoalaSelectionAssets/Light/Boonie.png");
            DarkSource = new Uri(@$"pack://application:,,,/Resources/KoalaSelectionAssets/Dark/{KoalaName}.jpg");
            TakenSource = new Uri(@$"pack://application:,,,/Resources/KoalaSelectionAssets/Taken/{KoalaName}.jpg");
            SelectedSource = new Uri(@$"pack://siteoforigin:,,,/Resources/KoalaSelectionAssets/Taken/{KoalaName}.mp4");
        }

        public void OnSelected()
        {
            if (!IsAvailable)
                return;

            SelectedAnimation.Position = TimeSpan.Zero;
            SelectedAnimation.Visibility = Visibility.Visible;
            SelectedAnimation.Play();
            KoalaSelected?.Invoke(this, new EventArgs());
        }
    }
}
