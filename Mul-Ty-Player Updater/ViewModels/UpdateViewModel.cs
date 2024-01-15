using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using Mul_Ty_Player_Updater.Views;
using Newtonsoft.Json.Linq;
using Octokit;
using PropertyChanged;
using Application = System.Windows.Application;
using FileMode = System.IO.FileMode;

namespace Mul_Ty_Player_Updater.ViewModels;

[AddINotifyPropertyChangedInterface]
public class UpdateViewModel
{
    public float Progress { get; set; }
    public string ProgressMessage { get; set; }
    public string Message { get; set; }
    public string Version { get; set; }
    public Visibility VersionVisibility { get; set; }
    public Visibility ProgressVisibility { get; set; }
    public bool Success { get; set; }
    
    private BackgroundWorker worker;

    public event EventHandler UpdateCompleted; 
    
    public UpdateViewModel()
    {
        Progress = 0;
        Message = "Checking for updates...";
        VersionVisibility = Visibility.Collapsed;
        ProgressVisibility = Visibility.Collapsed;
    }

    public void GetUpdate()
    {
        var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
        var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Mul-Ty-Player").Result;
        if (VersionIsMoreRecent(SettingsHandler.Settings.Version, latestRelease.TagName.Replace("v", "")))
        {
            Message = "Update available, installing now...";
            Version = latestRelease.TagName;
            VersionVisibility = Visibility.Visible;
            InstallUpdate();
            return;
        }
        Application.Current.Dispatcher.Invoke(() =>
        {
            var newMessageBox = new NewMessageBox(
                $"Mul-Ty-Player is already up to date!\n", "\uf058");
            newMessageBox.ShowDialog();
        });
        UpdateCompleted?.Invoke(this, null);
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
    
    
    public void InstallUpdate()
    {
        Success = false;
        Progress = 0;
        ProgressMessage = "Initializing install.";
        worker = new BackgroundWorker();
        worker.WorkerReportsProgress = true;
        worker.DoWork += InstallWorker_DoWork;
        worker.ProgressChanged += InstallWorker_ProgressChanged;
        worker.RunWorkerCompleted += InstallWorker_RunWorkerCompleted;
        worker.RunWorkerAsync();
    }

    private void InstallWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        ProgressMessage = e.Result?.ToString();
        if (Success)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var newMessageBox = new NewMessageBox(
                    $"Mul-Ty-Player v{Version} was installed successfully!\n" +
                    "Any old settings have been retained.\n" +
                    "Any new settings have been initialized to a default value.\n", "\uf058");
                newMessageBox.ShowDialog();
            });
            SettingsHandler.Settings.Version = Version.Replace("v", "");
            SettingsHandler.SaveSettings();
        }
        UpdateCompleted.Invoke(this, null!);
    }

    private void InstallWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        Progress = e.ProgressPercentage;
        ProgressMessage = e.UserState?.ToString();
    }

    private void InstallWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        try
        {
            var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
            var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Mul-Ty-Player").Result;
            Version = latestRelease.TagName.Replace("v", "");
            if (SettingsHandler.Settings.UpdateClient)
                UpdateClient(latestRelease);
            if (SettingsHandler.Settings.UpdateServer)
                UpdateServer(latestRelease);
            if (SettingsHandler.Settings.UpdateRKV)
            {
                PatchExe(latestRelease.TagName);
                UpdateRKV();
            }
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var newMessageBox = new NewMessageBox($"Mul-Ty-Player update could not be installed:\n" + ex.Message, "\uf071");
                newMessageBox.ShowDialog();
            });
            return;
        }
        Success = true;
        e.Result = "Mul-Ty-Player update was successfully installed.";
    }

    private void UpdateClient(Release latest)
    {
        var clientDirPath = SettingsHandler.Settings.ClientDir;
        
        var oldSettings = File.ReadAllText(Path.Combine(clientDirPath, "ClientSettings.json"));
        
        Directory.Delete(clientDirPath, true);
        Directory.CreateDirectory(clientDirPath);
        var zipPath = Path.Combine(clientDirPath, "Client.zip");
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player.Client.zip");
        if (asset == null) throw new Exception();
        HttpClient client = new();
        var uri = new Uri(asset.BrowserDownloadUrl);
        var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;
        response.EnsureSuccessStatusCode();
        using var contentStream = response.Content.ReadAsStreamAsync().Result;
        var totalBytes = response.Content.Headers.ContentLength;
        using FileStream fileStream = new(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        var buffer = new byte[4096];
        int bytesRead;
        var bytesWritten = 0L;
        do
        {
            bytesRead = contentStream.ReadAsync(buffer, 0, buffer.Length).Result;
            fileStream.WriteAsync(buffer, 0, bytesRead).Wait();
            bytesWritten += bytesRead;

            if (!totalBytes.HasValue) continue;
            var progressPercentage = bytesWritten * 100d / totalBytes.Value;
            worker.ReportProgress((int)progressPercentage, $"Downloading Client Files: {(int)progressPercentage}%");
        } while (bytesRead > 0);
        fileStream.Close();
        using var archive = ZipFile.OpenRead(zipPath);
        archive.ExtractToDirectory(clientDirPath);
        archive.Dispose();
        File.Delete(zipPath);
        
        var newSettings = File.ReadAllText(Path.Combine(clientDirPath, "ClientSettings.json"));
        
        File.WriteAllText(Path.Combine(clientDirPath, "ClientSettings.json"), UpdateSettings(oldSettings, newSettings));
    }
    
    private void UpdateServer(Release latest)
    {
        var serverDirPath = SettingsHandler.Settings.ServerDir;
        
        var oldSettings = File.ReadAllText(Path.Combine(serverDirPath, "ServerSettings.json"));
        
        Directory.Delete(serverDirPath, true);
        Directory.CreateDirectory(serverDirPath);
        var zipPath = Path.Combine(serverDirPath, "Server.zip");
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player.Server.zip");
        if (asset == null) throw new Exception();
        HttpClient client = new();
        var uri = new Uri(asset.BrowserDownloadUrl);
        var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;
        response.EnsureSuccessStatusCode();
        using var contentStream = response.Content.ReadAsStreamAsync().Result;
        var totalBytes = response.Content.Headers.ContentLength;
        using FileStream fileStream = new(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        var buffer = new byte[4096];
        int bytesRead;
        var bytesWritten = 0L;
        do
        {
            bytesRead = contentStream.ReadAsync(buffer, 0, buffer.Length).Result;
            fileStream.WriteAsync(buffer, 0, bytesRead).Wait();
            bytesWritten += bytesRead;

            if (!totalBytes.HasValue) continue;
            var progressPercentage = bytesWritten * 100d / totalBytes.Value;
            worker.ReportProgress((int)progressPercentage, $"Downloading Server Files: {(int)progressPercentage}%");
        } while (bytesRead > 0);
        fileStream.Close();
        using var archive = ZipFile.OpenRead(zipPath);
        archive.ExtractToDirectory(serverDirPath);
        archive.Dispose();
        File.Delete(zipPath);
        
        var newSettings = File.ReadAllText(Path.Combine(serverDirPath, "ServerSettings.json"));
        
        File.WriteAllText(Path.Combine(serverDirPath, "ServerSettings.json"), UpdateSettings(oldSettings, newSettings));
    }

    private string UpdateSettings(string oldSettings, string newSettings)
    {
        var oldSettingsJ = JObject.Parse(oldSettings);
        var newSettingsJ = JObject.Parse(newSettings);

        // Remove properties from oldSettings that are not in newSettings
        foreach (var property in oldSettingsJ.Properties().ToList().Where(property => !newSettingsJ.ContainsKey(property.Name)))
            property.Remove();
        foreach (var property in newSettingsJ.Properties())
        {
            if (!oldSettingsJ.ContainsKey(property.Name))
                oldSettingsJ.Add(property.Name, property.Value);
            else
                oldSettingsJ[property.Name] = property.Value;
        }
        return oldSettingsJ.ToString();
    }
    
    private void PatchExe(string version)
    {
        var gameDirPath = SettingsHandler.Settings.GameDir;
        var destinationFilePath = Path.Combine(gameDirPath, "Mul-Ty-Player.exe");
        
        //VERSION ON TITLE SCREEN
        using FileStream fileStream = new(destinationFilePath, FileMode.Open, FileAccess.Write);
        fileStream.Seek(0x2024F8, SeekOrigin.Begin);
        using BinaryWriter binaryWriter = new(fileStream);
        var versionString = "MTP " + version + " ";
        var replacement = Encoding.ASCII.GetBytes(versionString);
        binaryWriter.Write(replacement);
        
        //MAGNET PATCH
        var magnetData = SettingsHandler.Settings.FixedMagnets ? TyData.MagnetBytesFixed : TyData.MagnetBytesOrigin;
        foreach (var entry in magnetData)
        {
            fileStream.Seek(entry.Key, SeekOrigin.Begin);
            binaryWriter.Write(entry.Value);
        }
    }

    private void UpdateRKV()
    {
        var gameDirPath = SettingsHandler.Settings.GameDir;
        const string url = "https://drive.google.com/u/0/uc?id=1OTudAOfutB458mm2qKqRlX1Hi73h8WQw&export=download&confirm";
        HttpClient client = new();
        var uri = new Uri(url);
        var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;
        response.EnsureSuccessStatusCode();
        using var contentStream = response.Content.ReadAsStreamAsync().Result;
        var totalBytes = response.Content.Headers.ContentLength;
        var destinationPath = Path.Combine(gameDirPath, "Patch_PC.rkv");
        using FileStream fileStream = new(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        var buffer = new byte[4096];
        var bytesRead = default(int);
        var bytesWritten = 0L;
        do
        {
            bytesRead = contentStream.ReadAsync(buffer, 0, buffer.Length).Result;
            fileStream.WriteAsync(buffer, 0, bytesRead).Wait();
            bytesWritten += bytesRead;

            if (!totalBytes.HasValue) continue;
            var progressPercentage = bytesWritten * 100d / totalBytes.Value;
            worker.ReportProgress((int)progressPercentage, $"Downloading Patch: {(int)progressPercentage}%");
        } while (bytesRead > 0);
    }
    
}