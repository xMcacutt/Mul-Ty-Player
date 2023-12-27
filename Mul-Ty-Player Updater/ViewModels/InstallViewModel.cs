using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using Octokit;
using PropertyChanged;
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
    public bool RemoveMagnetRandom { get; set; }
    public float Progress { get; set; }
    public string? ProgressMessage { get; set; }
    public bool Installing { get; set; }

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
            if (InstallServer || InstallClient)
            {
                var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
                var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Nul-Ty-Player").Result;
                if (InstallClient)
                    CopyClientFiles(latestRelease);
                if (InstallServer)
                    CopyServerFiles(latestRelease);
            }
            if (InstallGameFiles)
            {
                CopyTyFiles();
                CopyPatch();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Mul-Ty-Player could not be installed.");
            return;
        }
        e.Result = "Mul-Ty-Player was successfully installed.";
    }

    private void CopyClientFiles(Release latest)
    {
        var clientDirPath = Path.Combine(MTPPath, "Client");
        Directory.CreateDirectory(clientDirPath);
        var zipPath = Path.Combine(clientDirPath, "Client.zip");
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player Client.zip");
        if (asset == null) throw new Exception();
        HttpClient client = new();
        var uri = new Uri(asset.BrowserDownloadUrl);
        var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;
        response.EnsureSuccessStatusCode();
        using var contentStream = response.Content.ReadAsStreamAsync().Result;
        var totalBytes = response.Content.Headers.ContentLength;
        using FileStream fileStream = new(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
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
            worker.ReportProgress((int)progressPercentage, $"Downloading Client Files: {(int)progressPercentage}%");
        } while (bytesRead > 0);

        using var archive = ZipFile.OpenRead(zipPath);
        archive.ExtractToDirectory(clientDirPath);
        File.Delete(zipPath);
    }

    private void CopyServerFiles(Release latest)
    {
        var serverDirPath = Path.Combine(MTPPath, "Server");
        Directory.CreateDirectory(serverDirPath);
        var zipPath = Path.Combine(serverDirPath, "Server.Zip");
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player Server.zip");
        if (asset == null) throw new Exception();
        HttpClient client = new();
        var uri = new Uri(asset.BrowserDownloadUrl);
        var response = client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead).Result;
        response.EnsureSuccessStatusCode();
        using var contentStream = response.Content.ReadAsStreamAsync().Result;
        var totalBytes = response.Content.Headers.ContentLength;
        using FileStream fileStream = new(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
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
            worker.ReportProgress((int)progressPercentage, $"Downloading Server Files: {(int)progressPercentage}%");
        } while (bytesRead > 0);

        using var archive = ZipFile.OpenRead(zipPath);
        archive.ExtractToDirectory(serverDirPath);
        File.Delete(zipPath);
    }

    private void CopyTyFiles()
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
                
            const int versionStringOffset = 0x2024F8;
            const string version = "";
            const string versionString = "MTP v" + version;
            var replacement = Encoding.ASCII.GetBytes(versionString);
            using FileStream fileStream = new(destinationFilePath, FileMode.Open, FileAccess.Write);
            fileStream.Seek(versionStringOffset, SeekOrigin.Begin);
            using BinaryWriter binaryWriter = new(fileStream);
            binaryWriter.Write(replacement);
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
}