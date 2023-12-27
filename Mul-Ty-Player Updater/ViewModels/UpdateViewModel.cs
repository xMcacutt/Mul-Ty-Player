using System;
using System.Dynamic;
using System.Linq;
using System.Windows;
using Octokit;
using PropertyChanged;

namespace Mul_Ty_Player_Updater.ViewModels;

[AddINotifyPropertyChangedInterface]
public class UpdateViewModel
{
    public float Progress { get; set; }
    public string ProgressMessage { get; set; }
    public string Message { get; set; }
    public string Version { get; set; }
    public Visibility VersionVisibility { get; set; }
    
    public UpdateViewModel()
    {
        Progress = 0;
        Message = "Checking for updates...";
        VersionVisibility = Visibility.Collapsed;
    }

    public void GetUpdate()
    {
        var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
        var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "MTPUpdateTest").Result;
        if (VersionIsMoreRecent(SettingsHandler.Settings.Version, latestRelease.TagName.Replace("v", "")))
        {
            Message = "Update available, installing now...";
            Version = latestRelease.TagName;
            VersionVisibility = Visibility.Visible;
        }
    }

    private bool VersionIsMoreRecent(string currentVersion, string newVersion)
    {
        var currentComponents = currentVersion.Split('.').Select(int.Parse).ToArray();
        var newComponents = newVersion.Split('.').Select(int.Parse).ToArray();

        for (var i = 0; i < Math.Min(currentComponents.Length, newComponents.Length); i++)
        {
            if (currentComponents[i] < newComponents[i])
            {
                return true; // New version is more recent
            }
            else if (currentComponents[i] > newComponents[i])
            {
                return false; // Current version is more recent
            }
            // If equal, continue checking the next component
        }
        // If all components are equal up to the minimum length, the longer version is considered newer
        return currentComponents.Length < newComponents.Length;
    }
    
    
}