// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json.Linq;
using Octokit;
using FileMode = System.IO.FileMode;

namespace MiniUpdater;

public class Program
{
    public float Progress { get; set; }

    public bool ForServer;

    private string? _message;
    public string? Message
    {
        get => _message;
        set
        {
            _message = value;
            Console.SetCursorPosition(0, Console.CursorTop);  // Move the cursor to the start of the current line
            Console.Write(new string(' ', Console.WindowWidth));  // Clear the current line
            Console.SetCursorPosition(0, Console.CursorTop);  // Move back to the start of the line
            Console.Write(_message);  // Write the new message
        }
    }

    public string? Version { get; set; }
    public bool Success { get; set; }
    
    public Program()
    {
        Progress = 0;
        Message = "Checking for updates...";
    }

    public static void Main(string[] args)
    {
        var program = new Program();
        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mul-Ty-Player Server.exe")))
            program.ForServer = true;
        else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mul-Ty-Player Client.exe")))
            program.ForServer = false;
        else
        {
            Console.WriteLine("\n[ERROR] This application must be in the same directory as the client or server.");
            return;
        }
        SettingsHandler.Setup(program.ForServer);
        program.CloseApps();
        program.GetUpdate();
    }

    public void GetUpdate()
    {
        var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
        var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Mul-Ty-Player").Result;
        if (!VersionIsMoreRecent(SettingsHandler.GetSetting<string>("Version"), latestRelease.TagName.Replace("v", ""))) 
            return;
        Message = "Update available, installing now...";
        Version = latestRelease.TagName;
        InstallUpdate();
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
        Message = "Initializing install.";
        StartInstall();
        CompleteInstall();
    }

    private void CompleteInstall()
    {
        if (Success)
        {
            Message = $"Mul-Ty-Player v{Version} was installed successfully!\n" +
                      "Any old settings have been retained.\n" +
                      "Any new settings have been initialized to a default value.\n";
            if (ForServer)
                SettingsHandler.UpdateSetting("Version", Version.Replace("v", ""));
            else
                SettingsHandler.UpdateSetting("Version", Version.Replace("v", ""));
            SettingsHandler.Save(ForServer);
        }
        Task.Delay(5000);
        ReopenApps();
    }
    
    private void ReopenApps()
    {
        var appDir = AppDomain.CurrentDomain.BaseDirectory;
        if (ForServer)
        {
            var serverPath = Path.Combine(appDir, "Mul-Ty-Player Server.exe");
            if (File.Exists(serverPath))
                Process.Start(serverPath);
        }
        else
        {
            var clientPath = Path.Combine(appDir, "Mul-Ty-Player Client.exe");
            var gamePath = SettingsHandler.GetSetting<string>("MulTyPlayerFolderPath");
            if (File.Exists(gamePath))
            {
                var startInfo = new ProcessStartInfo(gamePath)
                {
                    WorkingDirectory = Path.GetDirectoryName(gamePath)
                };
                Process.Start(startInfo);
            }
            Task.Delay(1000);
            if (File.Exists(clientPath))
                Process.Start(clientPath);
        }
    }

    private void CloseApps()
    {
        var appDir = AppDomain.CurrentDomain.BaseDirectory;
        
        void CloseAppByPath(string appPath)
        {
            if (!File.Exists(appPath)) return;
            var processName = Path.GetFileNameWithoutExtension(appPath); // Get process name without extension
            var processes = Process.GetProcessesByName(processName); // Get all processes with that name
            foreach (var process in processes)
            {
                try
                {
                    process.Kill(); // Attempt to kill the process
                    process.WaitForExit(); // Wait for it to exit
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to close {processName}: {ex.Message}");
                }
            }
        }

        // Close server, client, and game
        if (ForServer)
        {
            var serverPath = Path.Combine(appDir, "Mul-Ty-Player Server.exe");
            CloseAppByPath(serverPath);
        }
        else
        {
            var clientPath = Path.Combine(appDir, "Mul-Ty-Player Client.exe");
            var gamePath = SettingsHandler.GetSetting<string>("MulTyPlayerFolderPath");
            CloseAppByPath(clientPath);
            CloseAppByPath(gamePath);
        }
    }

    private void StartInstall()
    {
        try
        {
            var github = new GitHubClient(new ProductHeaderValue("Mul-Ty-Player"));
            var latestRelease = github.Repository.Release.GetLatest("xMcacutt", "Mul-Ty-Player").Result;
            Version = latestRelease.TagName.Replace("v", "");
            if (ForServer)
                UpdateServer(latestRelease);
            else
            {
                UpdateClient(latestRelease);
                UpdateRKV(latestRelease);
                PatchVersionIntoGame(latestRelease);
            }
        }
        catch (Exception ex)
        {
            Success = false;
            Message = "Mul-Ty-Player update could not be installed:\n" + ex.Message;
            return;
        }
        Success = true;
        Message = "Mul-Ty-Player update was successfully installed.";
    }

    private void UpdateClient(Release latest)
    {
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player.Client.zip");
        if (asset == null) throw new Exception();
        
        var clientDirPath = AppDomain.CurrentDomain.BaseDirectory;
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
            Message = $"Downloading Client Files: {(int)progressPercentage}%";
        } while (bytesRead > 0);
        fileStream.Close();
        using var archive = ZipFile.OpenRead(zipPath);
        foreach (var entry in archive.Entries)
        {
            // Check if the entry is not the excluded directory and not the excluded file
            if (entry.FullName.StartsWith("GUI/", StringComparison.OrdinalIgnoreCase)) 
                continue;
            
            entry.ExtractToFile(
                !entry.FullName.Equals("ClientSettings.json", StringComparison.OrdinalIgnoreCase)
                    ? Path.Combine(clientDirPath, entry.FullName)
                    : Path.Combine(clientDirPath, "NewSettings.json"), overwrite: true);
            var destinationPath = Path.Combine(clientDirPath, entry.FullName);
        }
        archive.Dispose();
        File.Delete(zipPath);
        
        var oldSettings = File.ReadAllText(Path.Combine(clientDirPath, "ClientSettings.json"));
        var newSettings = File.ReadAllText(Path.Combine(clientDirPath, "NewSettings.json"));
        File.WriteAllText(Path.Combine(clientDirPath, "ClientSettings.json"), UpdateSettings(oldSettings, newSettings));
        File.Delete(Path.Combine(clientDirPath, "NewSettings.json"));
    }
    
    private void UpdateServer(Release latest)
    {
        var asset = latest.Assets.FirstOrDefault(asset => asset.Name == "Mul-Ty-Player.Server.zip");
        if (asset == null) throw new Exception();
        
        var serverDirPath = AppDomain.CurrentDomain.BaseDirectory;;
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
            Message = $"Downloading Server Files: {(int)progressPercentage}%";
        } while (bytesRead > 0);
        fileStream.Close();
        using var archive = ZipFile.OpenRead(zipPath);
        foreach (var entry in archive.Entries)
            entry.ExtractToFile(
                !entry.FullName.Equals("ServerSettings.json", StringComparison.OrdinalIgnoreCase)
                    ? Path.Combine(serverDirPath, entry.FullName)
                    : Path.Combine(serverDirPath, "NewSettings.json"), overwrite: true);
        archive.Dispose();
        File.Delete(zipPath);
        
        var oldSettings = File.ReadAllText(Path.Combine(serverDirPath, "ServerSettings.json"));
        var newSettings = File.ReadAllText(Path.Combine(serverDirPath, "NewSettings.json"));
        File.WriteAllText(Path.Combine(serverDirPath, "ServerSettings.json"), UpdateSettings(oldSettings, newSettings));
        File.Delete(Path.Combine(serverDirPath, "NewSettings.json"));
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
    
    private void UpdateRKV(Release latestRelease)
    {
        var gameDirPath = Path.GetDirectoryName(SettingsHandler.GetSetting<string>("MulTyPlayerFolderPath"));
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
            Message = $"Downloading Game Files: {(int)progressPercentage}%";
        } while (bytesRead > 0);
        fileStream.Close();
    }

    private void PatchVersionIntoGame(Release latestRelease)
    {
        //VERSION ON TITLE SCREEN
        using FileStream fileStream = new(SettingsHandler.GetSetting<string>("MulTyPlayerFolderPath"), FileMode.Open, FileAccess.Write);
        fileStream.Seek(0x2024F8, SeekOrigin.Begin);
        using BinaryWriter binaryWriter = new(fileStream);
        var versionString = "MTP " + latestRelease.TagName + " ";
        var replacement = Encoding.ASCII.GetBytes(versionString);
        binaryWriter.Write(replacement);
        fileStream.Close();
    }
}