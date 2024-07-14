using System;
using System.CodeDom;
using System.Windows;
using MulTyPlayer;
using MulTyPlayerClient.GUI.Models;
using PropertyChanged;
using Riptide;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class HSD_LevelViewModel
{
    public HSD_LevelModel LevelModel;

    public HSD_LevelViewModel(HSD_LevelModel levelModel)
    {
        LevelModel = levelModel;
        LevelModel.OnAvailabilityChanged += SetAvailability;
        LevelName = LevelModel.LevelData.Name;
        IsAvailable = LevelModel.IsAvailable;
        ActiveSource = LevelModel.ActiveImageSource;
        InactiveSource = LevelModel.InactiveImageSource;
        SetImageVisibility();
    }
   
    public string LevelName { get; set; }
    public bool IsHovered { get; set; }
    public bool IsAvailable { get; set; }
    
    public Visibility ActiveVisibility { get; set; }
    public Visibility InactiveVisibility { get; set; }
    
    public Uri ActiveSource { get; set; }
    public Uri InactiveSource { get; set; }
    
    private void SetAvailability(bool newValue)
    {
        IsAvailable = newValue;
        SetImageVisibility();
    }
    
    public void SetHovered(bool newValue)
    {
        IsHovered = newValue;
        SetImageVisibility();
    }
    
    public void OnClicked()
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageID.HSD_PickRequest);
        message.AddInt(LevelModel.LevelData.Id);
        Client._client.Send(message);
        ModelController.HSD_Draft.AllowLevelSelect = true;
    }

    private void SetImageVisibility()
    {
        ActiveVisibility = IsAvailable ? Visibility.Visible : Visibility.Hidden;
        InactiveVisibility = !IsAvailable ? Visibility.Visible : Visibility.Hidden;
    }
}