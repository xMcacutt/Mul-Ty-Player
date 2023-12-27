using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Mul_Ty_Player_Updater.Views;
using Octokit;
using PropertyChanged;
using Application = System.Windows.Application;
using FileMode = System.IO.FileMode;

namespace Mul_Ty_Player_Updater.ViewModels;

[AddINotifyPropertyChangedInterface]
public class InstallViewModel
{
    
    public bool InstallClient { get; set; }
    public string ClientPath { get; set; }
    public bool InstallServer { get; set; }
    public string ServerPath { get; set; }
    public bool InstallGameFiles { get; set; }
    public string TyPath { get; set; }
    public string MTPPath { get; set; }
    public string Version { get; set; }
    public bool RemoveMagnetRandom { get; set; }
    public float Progress { get; set; }
    public string? ProgressMessage { get; set; }
    public bool Installing { get; set; }
    public bool Success { get; set; }
    public event EventHandler InstallCompleted; 

    private BackgroundWorker worker;
    
    public InstallViewModel()
    {
        InstallClient = true;
        InstallGameFiles = true;
        RemoveMagnetRandom = true;
        Progress = 0;

        var tyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Steam\steamapps\common\TY the Tasmanian Tiger");
        if (Directory.Exists(tyPath))
            TyPath = tyPath;
        else
        {
            tyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Steam\steamapps\common\TY the Tasmanian Tiger");
            if (Directory.Exists(tyPath))
                TyPath = tyPath;
        }
        
        var documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (!string.IsNullOrWhiteSpace(documentsFolderPath))
        {
            var mtpPath = Path.Combine(documentsFolderPath, "Mul-Ty-Player");
            if (!Directory.Exists(mtpPath)) Directory.CreateDirectory(mtpPath);
            MTPPath = mtpPath;
            ClientPath = mtpPath;
            ServerPath = mtpPath;
        }
    }

    public void Install()
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
                NewMessageBox newMessageBox = new NewMessageBox(
                    "Mul-Ty-Player was installed successfully!\n" +
                    "Settings have been saved for future updates.\n" +
                    "You can change these in Setup.\n", "\uf058");
                newMessageBox.ShowDialog();
            });
            Installing = false;
            SaveSettings();
            InstallCompleted.Invoke(this, null!);
        }
        Installing = false;

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
            var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "MTPUpdateTest").Result;
            Version = latestRelease.TagName.Replace("v", "");
            if (InstallServer || InstallClient)
            {
                if (InstallClient)
                    CopyClientFiles(latestRelease);
                if (InstallServer)
                    CopyServerFiles(latestRelease);
            }
            if (InstallGameFiles)
            {
                CopyTyFiles(latestRelease.TagName);
                CopyPatch();
            }
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                NewMessageBox newMessageBox = new NewMessageBox("Mul-Ty-Player could not be installed:\n" + ex.Message, "\uf071");
                newMessageBox.ShowDialog();
            });
            return;
        }
        Success = true;
        e.Result = "Mul-Ty-Player was successfully installed.";
    }

    private void CopyClientFiles(Release latest)
    {
        var clientDirPath = Path.Combine(MTPPath, "Client");
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
    }

    private void CopyServerFiles(Release latest)
    {
        var serverDirPath = Path.Combine(MTPPath, "Server");
        Directory.CreateDirectory(serverDirPath);
        var zipPath = Path.Combine(serverDirPath, "Server.Zip");
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
        File.Delete(zipPath);
    }

    private void CopyTyFiles(string version)
    {
        var gameDirPath = Path.Combine(MTPPath, "Game");
        Directory.CreateDirectory(gameDirPath);
        foreach (var file in TyData.TyFileNames)
        {
            var sourceFilePath = Path.Combine(TyPath, file);
            var destinationFilePath = Path.Combine(gameDirPath, file);
            if (file == "TY.exe") destinationFilePath = Path.Combine(gameDirPath, "Mul-Ty-Player.exe");
            var fileInfo = new FileInfo(sourceFilePath);
            var totalBytes = fileInfo.Length;

            using var sourceStream = File.OpenRead(sourceFilePath);
            using var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write);

            var buffer = new byte[81920];
            int bytesRead;
            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                destinationStream.Write(buffer, 0, bytesRead);

                var bytesWritten = destinationStream.Position;
                var progressPercentage = (double)bytesWritten / totalBytes * 100;
                worker.ReportProgress((int)progressPercentage, $"Copying file {file}: {(int)progressPercentage}%");
            }
            destinationStream.Close();
                
            if (file != "TY.exe") continue;
                
            //VERSION ON TITLE SCREEN
            using FileStream fileStream = new(destinationFilePath, FileMode.Open, FileAccess.Write);
            fileStream.Seek(0x2024F8, SeekOrigin.Begin);
            using BinaryWriter binaryWriter = new(fileStream);
            var versionString = "MTP " + version + " ";
            var replacement = Encoding.ASCII.GetBytes(versionString);
            binaryWriter.Write(replacement);
            
            //MAGNET PATCH
            var magnetData = RemoveMagnetRandom ? TyData.MagnetBytesFixed : TyData.MagnetBytesOrigin;
            foreach (var entry in magnetData)
            {
                fileStream.Seek(entry.Key, SeekOrigin.Begin);
                binaryWriter.Write(entry.Value);
            }
        }
    }

    private void CopyPatch()
    {
        var gameDirPath = Path.Combine(MTPPath, "Game");
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

    private void SaveSettings()
    {
        if (SettingsHandler.Settings == null) return;
        SettingsHandler.Settings.UpdateClient = InstallClient;
        SettingsHandler.Settings.UpdateServer = InstallServer;
        SettingsHandler.Settings.UpdateRKV = InstallGameFiles;
        SettingsHandler.Settings.ClientDir = ClientPath;
        SettingsHandler.Settings.ServerDir = ServerPath;
        SettingsHandler.Settings.GameDir = MTPPath;
        SettingsHandler.Settings.FixedMagnets = RemoveMagnetRandom;
        SettingsHandler.Settings.Version = Version;
        SettingsHandler.SaveSettings();
    }
}