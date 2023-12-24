using System;
using System.Windows;
using System.Windows.Controls;

namespace MulTyPlayerClient.GUI.Views;

/// <summary>
///     Interaction logic for KoalaSelect.xaml
/// </summary>
public partial class KoalaSelect : UserControl
{
    public KoalaSelect()
    {
        InitializeComponent();
        //BackgroundVideo.Visibility = Visibility.Hidden;
        //BackgroundVideo.Play();
    }

    private void BackgroundVideo_OnMediaEnded(object sender, RoutedEventArgs e)
    {
        //BackgroundVideo.Position = TimeSpan.Zero;
        //BackgroundVideo.Play();
    }

    private void BackgroundVideo_OnMediaOpened(object sender, RoutedEventArgs e)
    {
        //BackgroundVideo.Visibility = Visibility.Visible;
    }
}