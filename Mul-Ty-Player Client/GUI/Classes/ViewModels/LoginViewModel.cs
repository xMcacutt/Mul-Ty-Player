using System;
using System.Diagnostics;
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
        var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
        var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Mul-Ty-Player").Result;
        var latestVersion = latestRelease.TagName.Replace("v", "");
        var result = VersionHandler.Compare(SettingsHandler.Settings.Version, latestVersion);
        UpdateMessageVisible = result is VersionResult.SecondNewer;
    }

    public string Name { get; set; }
    public string Pass { get; set; }
    public string ConnectingAddress { get; set; }

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
        ConnectingAddress = Login.GetIP();
        Name = Login.GetName();
        Pass = Login.GetPass();
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
}