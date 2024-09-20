using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.ViewModels;

namespace MulTyPlayerClient.GUI.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel mainViewModel;

    public MainWindow()
    {
        InitializeComponent();
        mainViewModel = new MainViewModel();
        DataContext = mainViewModel;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        try
        {
            if (Client._client != null && Client._client.IsConnected) Client._client.Disconnect();
            TyProcess.CloseHandle();
            SteamHelper.Shutdown();
            Logger.Close();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
}