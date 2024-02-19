using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;
using NHotkey;
using NHotkey.Wpf;
using Application = System.Windows.Application;

namespace MulTyPlayerClient;

public partial class App : Application
{
    private MainWindow mw;
    public static Colors AppColors;
    public static Window SettingsWindow;

    public App()
    {
        AppColors = new Colors();
        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        HotkeyHandler.SetupHotkeys();
        SettingsHandler.Setup();
        ModelController.InstantiateModels();
        SFXPlayer.Init();
        mw = new MainWindow();
        mw.Show();
        mw.Activate();
    }


}