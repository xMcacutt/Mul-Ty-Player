using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Classes.Views;
using MulTyPlayerClient.GUI.Models;
using MulTyPlayerClient.GUI.Views;
using NHotkey;
using NHotkey.Wpf;
using Application = System.Windows.Application;

namespace MulTyPlayerClient;

public partial class App : Application
{
    private MainWindow _mainWindow;
    public static Colors AppColors;
    public static Window SettingsWindow;

    public App()
    {
        InitializeComponent();
        AppColors = Resources["AppColors"] as Colors;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        HotkeyHandler.Initialize();
        SettingsHandler.Setup();
        ModelController.InstantiateModels();
        SFXPlayer.Init();
        _mainWindow = new MainWindow();
        _mainWindow.Show();
        _mainWindow.Activate();
        #if DEBUG
            var editor = new ThemeEditor();
            editor.Show();
        #endif
    }
    
    
}