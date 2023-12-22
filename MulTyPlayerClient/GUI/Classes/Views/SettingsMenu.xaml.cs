using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace MulTyPlayerClient.GUI.Views;

public partial class SettingsMenu : Window
{
    public SettingsMenu()
    {
        InitializeComponent();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        WindowHandler.SettingsWindow.Hide();
        SettingsHandler.SaveSettingsFromSettingsMenu();
        e.Cancel = true;
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Use regular expression to allow only numeric input
        var regex = NumericRegex();
        e.Handled = regex.IsMatch(e.Text);
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumericRegex();

    private void ClientDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(ref GameStackPanel);
        AnimateCloseDropdown(ref DevStackPanel);
        AnimateDropdown(ref ClientStackPanel);
    }
    
    private void DevDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(ref GameStackPanel);
        AnimateCloseDropdown(ref ClientStackPanel);
        AnimateDropdown(ref DevStackPanel);
    }
    
    private void GameDropDownButton_OnClick(object sender, RoutedEventArgs e)
    {
        AnimateCloseDropdown(ref DevStackPanel);
        AnimateCloseDropdown(ref ClientStackPanel);
        AnimateDropdown(ref GameStackPanel);
    }

    private void AnimateCloseDropdown(ref StackPanel dropDown)
    {
        if (dropDown.Height == 0) return;
        var animation = new DoubleAnimation
        {
            From = dropDown.Height,
            To = 0,
            Duration = TimeSpan.FromSeconds(0.3) // You can adjust the duration
        };
        dropDown.BeginAnimation(HeightProperty, animation);
    }

    private void AnimateDropdown(ref StackPanel dropDown)
    {
        double from;
        double to;
        if (dropDown.Height == 0)
        {
            from = 0;
            to = dropDown.ActualHeight;
        }
        else
        {
            from = dropDown.Height;
            to = 0;
        }
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromSeconds(0.3) // You can adjust the duration
        };
        dropDown.BeginAnimation(HeightProperty, animation);
    }
}