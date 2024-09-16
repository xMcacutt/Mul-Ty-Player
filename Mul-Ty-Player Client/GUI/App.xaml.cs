using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
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
    private MainWindow mw;
    public static Colors AppColors;
    public static Window SettingsWindow;
    public static Minimap Minimap;

    public App()
    {
        AppColors = new Colors();
        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        HotkeyHandler.Initialize();
        SettingsHandler.Setup();
        ModelController.InstantiateModels();
        SFXPlayer.Init();
        //Minimap = new Minimap();
        //Minimap.Show();
        mw = new MainWindow();
        mw.Show();
        mw.Activate();
    }


}