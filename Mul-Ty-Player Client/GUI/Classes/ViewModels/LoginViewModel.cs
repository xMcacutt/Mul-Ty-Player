using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using MulTyPlayerClient.Classes.Utility;
using MulTyPlayerClient.GUI.Models;
using Octokit;
using PropertyChanged;

namespace MulTyPlayerClient.GUI.ViewModels;

[AddINotifyPropertyChangedInterface]
public class LoginViewModel : IViewModel
{
    public LoginViewModel()
    {
        ConnectCommand = new RelayCommand(TryConnect);
        Login.OnLoginFailed += Model_OnLoginFailed;
        Login.OnLoginSuccess += Model_OnLoginSuccess;
        Login.JoinAsSpectatorChanged += Model_JoinAsSpectatorChanged;
        Login.CurrentServerClientCountChanged += Model_CurrentServerClientCountChanged;
        var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
        var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Mul-Ty-Player").Result;
        var latestVersion = latestRelease.TagName.Replace("v", "");
        var result = VersionHandler.Compare(SettingsHandler.ClientSettings.Version, latestVersion);
        UpdateMessageVisible = result == VersionResult.SecondNewer;
        if (result == VersionResult.SecondNewer 
            && SettingsHandler.ClientSettings.DoAutoUpdate
            && File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mul-Ty-Player Mini Updater.exe")))
            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mul-Ty-Player Mini Updater.exe"));
    }

    public ObservableCollection<ServerListing> Servers { get; set; }
    public ServerListing SelectedServer { get; set; }
    public bool IsPopupOpen { get; set; }
    public string Name { get; set; }
    public string Pass { get; set; }
    public string ConnectingAddress { get; set; }
    public bool JoinAsSpectator { get; set; }
    public string CurrentServerClientCount { get; set; }
    public bool HideName { get; set; }
    public bool HidePass { get; set; } = true;
    public bool HideAddress { get; set; } = true;

    public ICommand ConnectCommand { get; set; }
    public bool ConnectButtonEnabled { get; set; } = true;

    public bool ConnectionAttemptSuccessful { get; set; }
    public bool ConnectionAttemptCompleted { get; set; }

    private static LoginModel Login => ModelController.Login;
    public bool UpdateMessageVisible { get; set; }

    public void OnEntered()
    {
        Login.Setup();
        Servers = new ObservableCollection<ServerListing>(Login.GetServers());
        ConnectingAddress = Login.GetIP();
        Pass = Login.GetPass();
        Name = Login.GetName();
        ConnectButtonEnabled = true;
    }

    public void OnExited()
    {
    }

    private void TryConnect()
    {
        ConnectButtonEnabled = false;
        Debug.WriteLine("Connecting address: " + ConnectingAddress);
        Debug.WriteLine("Pass: " + Pass);
        Login.Connect(ConnectingAddress, Name, Pass);
    }

    private void Model_OnLoginSuccess()
    {
    }

    private async void Model_OnLoginFailed()
    {
        await Task.Delay(1000);
        ConnectButtonEnabled = true;
    }
    
    private void Model_JoinAsSpectatorChanged(bool value)
    {
        JoinAsSpectator = value;
    }
    
    private void Model_CurrentServerClientCountChanged(string value)
    {
        CurrentServerClientCount = "Koalas Connected: " + value;
    }
}