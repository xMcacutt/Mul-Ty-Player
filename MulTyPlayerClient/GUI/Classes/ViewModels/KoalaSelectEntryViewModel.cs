using System;
using System.Windows;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class KoalaSelectEntryViewModel
{
    public KoalaSelectEntryModel kseModel;

    public KoalaSelectEntryViewModel(KoalaSelectEntryModel kseModel)
    {
        this.kseModel = kseModel;
        this.kseModel.OnAvailabilityChanged += SetAvailability;
        KoalaName = kseModel.KoalaInfo.Name;
        IsAvailable = kseModel.IsAvailable;
        LightSource = kseModel.LightPortraitSource;
        DarkSource = kseModel.DarkPortraitSource;
        TakenSource = kseModel.TakenPortraitSource;
        SelectedSource = kseModel.SelectedAnimationSource;
        SetPortraitVisibility();
    }

    public string KoalaName { get; set; }

    public bool IsHovered { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Visibility LightPortraitVisibility { get; set; }
    public Visibility DarkPortraitVisibility { get; set; }
    public Visibility TakenPortraitVisibility { get; set; }

    public Uri LightSource { get; set; }
    public Uri DarkSource { get; set; }
    public Uri TakenSource { get; set; }
    public Uri SelectedSource { get; set; }

    public void SetHovered(bool newValue)
    {
        IsHovered = newValue;
        SetPortraitVisibility();
    }

    private void SetAvailability(bool newValue)
    {
        IsAvailable = newValue;
        SetPortraitVisibility();
    }

    public void OnClicked()
    {
        ModelController.KoalaSelect.KoalaClicked(kseModel.KoalaInfo.Koala);
    }

    private void SetPortraitVisibility()
    {
        LightPortraitVisibility = IsAvailable && IsHovered ? Visibility.Visible : Visibility.Hidden;
        DarkPortraitVisibility = IsAvailable && !IsHovered ? Visibility.Visible : Visibility.Hidden;
        TakenPortraitVisibility = !IsAvailable ? Visibility.Visible : Visibility.Hidden;
    }
}