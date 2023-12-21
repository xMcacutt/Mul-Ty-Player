using System.Windows;
using MulTyPlayerClient.GUI;
using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient;

public partial class App : Application
{
    private MainWindow mw;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SettingsHandler.Setup();
        ModelController.InstantiateModels();
        WindowHandler.InitializeWindows();
        SFXPlayer.Init();
        mw = new MainWindow();
        mw.Show();
        mw.Activate();
    }
}