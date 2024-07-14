using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Views;

public partial class HSD_Level : UserControl
{
    public HSD_Level()
    {
        InitializeComponent();
    }

    protected void Level_OnMouseOverChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        (DataContext as HSD_LevelViewModel).SetHovered((bool)e.NewValue);
    }

    protected void Level_OnClicked(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not HSD_LevelViewModel vm) 
            return;
        if (!vm.IsAvailable)
            return;
        vm.OnClicked();
    }
    
    protected void OnSourceInitialized(EventArgs e)
    {
        var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
        var hwndTarget = hwndSource?.CompositionTarget;
        if (hwndTarget != null) hwndTarget.RenderMode = RenderMode.SoftwareOnly;
    }
}