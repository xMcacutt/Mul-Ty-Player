using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Views;

public partial class KoalaSelectEntry : UserControl
{
    public KoalaSelectEntry()
    {
        InitializeComponent();
    }

    protected void KoalaEntry_OnMouseOverChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        (DataContext as KoalaSelectEntryViewModel).SetHovered((bool)e.NewValue);
    }

    protected void KoalaEntry_OnClicked(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is KoalaSelectEntryViewModel vm)
        {
            if (!vm.IsAvailable)
                return;
            SelectedAnimation.Position = TimeSpan.Zero;
            SelectedAnimation.Visibility = Visibility.Visible;
            SelectedAnimation.Play();
            Thread.Sleep(200);
            vm.OnClicked();
        }
    }
}