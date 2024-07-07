using System;
using System.ComponentModel;
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
    public string? ProgressMessage { get; set; }
    public string Message { get; set; }
    public string? Version { get; set; }
    public Visibility VersionVisibility { get; set; }
    public Visibility ProgressVisibility { get; set; }
    public bool Success { get; set; }
    
    private BackgroundWorker? worker;

    public event EventHandler? UpdateCompleted; 
    
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
            if (currentComponents[i] > newComponents[i])
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
                UpdateRKV(latestRelease);
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
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player.Client.zip");
        if (asset == null) throw new Exception();
        
        var clientDirPath = SettingsHandler.Settings.ClientDir;
        
        var oldSettings = File.ReadAllText(Path.Combine(clientDirPath, "ClientSettings.json"));
        
        var tempDirPath = Path.Combine(Path.GetTempPath(), "MTPClientGUI");
        Directory.CreateDirectory(tempDirPath);
        var guiDirPath = Path.Combine(clientDirPath, "GUI");
        if (Directory.Exists(guiDirPath))
            CopyDirectory(guiDirPath, tempDirPath, true);
        Directory.Delete(clientDirPath, true);
        Directory.CreateDirectory(clientDirPath);
        var zipPath = Path.Combine(clientDirPath, "Client.zip");
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
        
        if (Directory.Exists(tempDirPath))
        {
            CopyDirectory(tempDirPath, Path.Combine(clientDirPath, "GUI"), true);
            Directory.Delete(tempDirPath, true);
        }
        
        var newSettings = File.ReadAllText(Path.Combine(clientDirPath, "ClientSettings.json"));
        
        File.WriteAllText(Path.Combine(clientDirPath, "ClientSettings.json"), UpdateSettings(oldSettings, newSettings));
    }
    
    private void UpdateServer(Release latest)
    {
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player.Server.zip");
        if (asset == null) throw new Exception();
        
        var serverDirPath = SettingsHandler.Settings.ServerDir;
        
        var oldSettings = File.ReadAllText(Path.Combine(serverDirPath, "ServerSettings.json"));
        
        Directory.Delete(serverDirPath, true);
        Directory.CreateDirectory(serverDirPath);
        var zipPath = Path.Combine(serverDirPath, "Server.zip");
        
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
        {
            // Remove unused settings
            property.Remove();
        }
        // Update version property with the new value
        if (newSettingsJ.TryGetValue(nameof(Version), out var value))
        {
            oldSettingsJ[nameof(Version)] = value;
        }
        // Add missing properties from newSettings
        foreach (var property in newSettingsJ.Properties())
        {
            if (!oldSettingsJ.ContainsKey(property.Name))
            {
                oldSettingsJ.Add(property.Name, property.Value);
            }
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
        
        //OUTBACK MOVEMENT
        var outbackData = SettingsHandler.Settings.RevertOutbackMovement ? new byte[] {0x90, 0x90} : new byte[] {0x75, 0x06}; 
        fileStream.Seek(0x17251B, SeekOrigin.Begin);
        binaryWriter.Write(outbackData);
        
        //RANG SWITCHING
        var rangData = SettingsHandler.Settings.RevertRangSwitching
            ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90, 0x90 }
            : new byte[] { 0x0F, 0x85, 0xC0, 0x00, 0x00, 0x00 };
        fileStream.Seek(0x161F8A, SeekOrigin.Begin);
        binaryWriter.Write(rangData);
            
        //CAMERA AIMING
        var cameraData = SettingsHandler.Settings.FixControllerCameraAiming ? new byte[] {0x90, 0x90} : new byte[] {0x75, 0x0C}; 
        fileStream.Seek(0x169ABC, SeekOrigin.Begin);
        binaryWriter.Write(cameraData);
        
        //GAME INFO FIX
        var gameInfoData = SettingsHandler.Settings.OpenAllGameInfo
            ? new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 }
            : new byte[] { 0x80, 0x7C, 0x31, 0x10, 0x00 };
        fileStream.Seek(0xE49CD, SeekOrigin.Begin);
        binaryWriter.Write(gameInfoData);
        fileStream.Seek(0xE5E4D, SeekOrigin.Begin);
        binaryWriter.Write(gameInfoData);

        //MENU FIX
        var menuButtonPositionData = SettingsHandler.Settings.FixMenuBug
            ? BitConverter.GetBytes(-5f)
            : BitConverter.GetBytes(48f);
        fileStream.Seek(0x2019B0, SeekOrigin.Begin);
        binaryWriter.Write(menuButtonPositionData);
    }

    private void UpdateRKV(Release latestRelease)
    {
        var gameDirPath = SettingsHandler.Settings.GameDir;
        var asset = latestRelease.Assets.FirstOrDefault(asset => asset.Name == "Patch_PC.rkv");
        if (asset == null) throw new Exception();
        HttpClient client = new();
        var uri = new Uri(asset.BrowserDownloadUrl);
        var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;
        response.EnsureSuccessStatusCode();
        using var contentStream = response.Content.ReadAsStreamAsync().Result;
        var totalBytes = response.Content.Headers.ContentLength;
        using FileStream fileStream = new(Path.Combine(gameDirPath, "Patch_PC.rkv"), FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
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
            worker.ReportProgress((int)progressPercentage, $"Downloading game patch: {(int)progressPercentage}%");
        } while (bytesRead > 0);
        fileStream.Close();
    }
    
    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }
}