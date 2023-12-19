using System;
using System.Windows;
using System.Windows.Controls;

namespace MulTyPlayerClient.GUI.Views
{
    /// <summary>
    /// Interaction logic for KoalaSelect.xaml
    /// </summary>
    public partial class KoalaSelect : UserControl
    {
        public KoalaSelect()
        {
            InitializeComponent();
            backgroundVideo.Visibility = Visibility.Hidden;
            backgroundVideo.Play();
        }

        private void BackgroundVideo_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void BackgroundVideo_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Visibility = Visibility.Visible;
        }
    }
}
