﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Views;

public partial class HSD_MainWindow : Window
{
    private readonly HSD_MainViewModel mainViewModel;
    public HSD_MainWindow()
    {
        mainViewModel = new HSD_MainViewModel();
        DataContext = mainViewModel;
        InitializeComponent();
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void HSD_MainWindow_OnClosing(object sender, CancelEventArgs e)
    {
        if (Client.HDrafts is not null)
        {
            Client.HDrafts.LeaveTeam();
            ModelController.HSD_Draft.MakeAllAvailable();
            Client.HDrafts.ClearPicks();
        }
    }
}