﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI;

public partial class MainWindow : Window
{
    private readonly MainViewModel mainViewModel;

    public MainWindow()
    {
        mainViewModel = new MainViewModel();
        DataContext = mainViewModel;
        InitializeComponent();
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (Client._client != null && Client._client.IsConnected) Client._client.Disconnect();
        TyProcess.CloseHandle();
        SteamHelper.Shutdown();
        Logger.Close();
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }
}