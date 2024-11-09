using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Views;

public partial class SettingsMenu : Window
{
    public SettingsMenu()
    {
        InitializeComponent();

    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void PortTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("^[0-9]+$");
        if (!regex.IsMatch(e.Text))
        {
            e.Handled = true;
            return;
        }
        if (string.IsNullOrEmpty(PortTextBox.Text)) return;
        
        if (uint.TryParse(PortTextBox.Text + e.Text, out uint portValue))
            e.Handled = portValue > ushort.MaxValue;
        else
            e.Handled = true;
        
    }
    
    private void RangeTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("^[0-9]+$");
        if (!regex.IsMatch(e.Text))
        {
            e.Handled = true;
            return;
        }
        if (string.IsNullOrEmpty(ProximityRangeTextBox.Text)) return;
        e.Handled = !int.TryParse(ProximityRangeTextBox.Text + e.Text, out _);
    }

    private void ClientDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(GameStackPanel);
        AnimateCloseDropdown(DevStackPanel);
        AnimateCloseDropdown(VoiceStackPanel);
        AnimateCloseDropdown(ServerStackPanel);
        AnimateDropdown(ClientStackPanel);
    }
    
    private void DevDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(GameStackPanel);
        AnimateCloseDropdown(ClientStackPanel);
        AnimateCloseDropdown(VoiceStackPanel);
        AnimateCloseDropdown(ServerStackPanel);
        AnimateDropdown(DevStackPanel);
    }
    
    private void GameDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(DevStackPanel);
        AnimateCloseDropdown(ClientStackPanel);
        AnimateCloseDropdown(VoiceStackPanel);
        AnimateCloseDropdown(ServerStackPanel);
        AnimateDropdown(GameStackPanel);
    }
    
    private void VoiceDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(DevStackPanel);
        AnimateCloseDropdown(ClientStackPanel);
        AnimateCloseDropdown(GameStackPanel);
        AnimateCloseDropdown(ServerStackPanel);
        AnimateDropdown(VoiceStackPanel);
    }
    
    private void ServerDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(DevStackPanel);
        AnimateCloseDropdown(ClientStackPanel);
        AnimateCloseDropdown(GameStackPanel);
        AnimateCloseDropdown(VoiceStackPanel);
        AnimateDropdown(ServerStackPanel);
    }

    private void AnimateCloseDropdown(StackPanel dropDown)
    {
        if (dropDown.Height == 0 || dropDown.Visibility == Visibility.Collapsed) return;
        var animation = new DoubleAnimation
        {
            From = dropDown.ActualHeight,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.3) // You can adjust the duration
        };
        animation.Completed += (s, e) =>
        {
            dropDown.Visibility = Visibility.Collapsed;
        };
        dropDown.BeginAnimation(HeightProperty, animation);
    }

    private void AnimateDropdown(StackPanel dropDown)
    {
        DoubleAnimation anim = new DoubleAnimation();
        anim.FillBehavior = FillBehavior.Stop;
        anim.Duration = TimeSpan.FromSeconds(0.3);

        if (dropDown.Visibility == Visibility.Collapsed)
        {
            dropDown.Visibility = Visibility.Visible;

            // Use Dispatcher to defer setting the From and To properties until the layout is updated
            dropDown.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                anim.From = 0;
                anim.To = dropDown.ActualHeight;
                dropDown.BeginAnimation(HeightProperty, anim);
            }));
        }
        else
        {
            anim.From = dropDown.ActualHeight;
            anim.To = 0;
            anim.Completed += (s, e) =>
            {
                dropDown.Visibility = Visibility.Collapsed;
            };

            dropDown.BeginAnimation(HeightProperty, anim);
        }
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        ((SettingsViewModel)DataContext).SavePropertiesBackToSettings();
        DialogResult = true;
        Close();
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void InputDeviceComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //VoiceHandler.UpdateInputDevice(InputDeviceComboBox.SelectedIndex);
    }

    private void InputDeviceComboBox_OnDropDownOpened(object sender, EventArgs e)
    {
        (DataContext as SettingsViewModel)?.UpdateInputDevices();
    }


}