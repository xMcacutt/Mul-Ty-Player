using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows;
using MulTyPlayerClient.Classes.Utility;

namespace MulTyPlayerClient.GUI.Models;

public class LoginModel
{
    private List<ServerListing> _serverList;
    public bool ConnectionAttemptCompleted;
    public bool ConnectionAttemptSuccessful = false;
    public bool WasConnectionError = true;

    
    private string ip, name, pass;
    public event Action OnLoginSuccess;
    public event Action OnLoginFailed;

    public void Connect(string ip, string name, string pass)
    {
        this.ip = ip;
        this.name = name;
        this.pass = pass;
        Thread connect = new(AttemptConnection);
        connect.Start();
    }

    public string GetIP()
    {
        return ip;
    }

    public string GetName()
    {
        return name;
    }

    public string GetPass()
    {
        return pass;
    }
    
    public IEnumerable<ServerListing> GetServers()
    {
        return _serverList;
    }

    private void AttemptConnection()
    {
        try
        {
            ConnectionAttemptCompleted = false;
            Client.Start(ip, name, pass);
            while (!ConnectionAttemptCompleted)
            {
            }
        }
        catch(Exception e)
        {
            Logger.Write(e.Message);
            OnLoginFailed?.Invoke();
            if (WasConnectionError)
                LoginFailed(e.Message);
            return;
        }
        if (ConnectionAttemptSuccessful)
        {
            SaveDetails();
            OnLoginSuccess?.Invoke();
        }
        else 
        {
            OnLoginFailed?.Invoke();
            if (WasConnectionError) 
                LoginFailed();
        }
    }
    
    public void Setup()
    {
        SetName();
        Client.SteamId = SteamHelper.GetSteamId();

        if (Path.Exists("./list.servers"))
        {
            ParseServerList(File.ReadLines("./list.servers"));
        }
        else
        {
            var _fs = File.Create("./list.servers");
            _fs.Close();
        }
    }

    public void SetName()
    {
        //If steam name retrieval is successful, set that as the name and set the default name in settings
        if (SettingsHandler.Settings.DoGetSteamName)
        {
            name = SteamHelper.GetSteamName();
            SettingsHandler.Settings.DefaultName = name;
        }
        //If the steam name setting is disabled, use the previously stored default name
        else
        {
            name = SettingsHandler.Settings.DefaultName;
        }

        //If the name is still null, generate a random one
        if (name == null) name = GenerateRandomUser();
    }

    private void SaveDetails()
    {
        foreach (var x in _serverList) x.ActiveDefault = false;
        //Save the currently connected server details to serverlist
        if (!_serverList.Where(x => x.IP == ip).Any())
        {
            _serverList.Add(new ServerListing(ip, pass, true));
        }
        else
        {
            _serverList.Where(x => x.IP == ip).First().Pass = pass;
            _serverList.Where(x => x.IP == ip).First().ActiveDefault = true;
        }

        //Save serverlist to file
        using var fs = File.Create("./list.servers");
        var servers = "";
        foreach (var server in _serverList)
        {
            servers += $"{server.IP} {server.Pass}";
            if (server.ActiveDefault)
                servers += " *";
            servers += "\n";
        }

        var serverInfo = new UTF8Encoding(true).GetBytes(servers);
        fs.Write(serverInfo, 0, serverInfo.Length);
    }

    private void ParseServerList(IEnumerable<string> file)
    {
        _serverList = new List<ServerListing>();
        var local = new ServerListing("192.168.1.1", "OPALS", false);
        foreach (var server in file)
        {
            var entry = server.Split(' ');
            _serverList.Add(new ServerListing(entry[0], entry[1], entry.Length == 3));
        }

        SetDetailsFromServer(_serverList.FirstOrDefault(x => x.ActiveDefault, local));
    }

    private void SetDetailsFromServer(ServerListing server)
    {
        ip = server.IP;
        pass = server.Pass;
    }

    private string GenerateRandomUser()
    {
        Random random = new();
        var randomNumber = random.Next(100000, 999999);
        return "USER" + randomNumber;
    }

    private void LoginFailed(string? message = null)
    {
        SystemSounds.Hand.Play();
        message ??= "Check the IP and password.";
        MessageBox.Show($"Connection attempt failed! {message}");
        OnLoginFailed?.Invoke();
    }
    
    
    public event Action<bool> JoinAsSpectatorChanged;
    public bool JoinAsSpectator
    {
        get => joinAsSpectator;
        set
        {
            joinAsSpectator = value;
            JoinAsSpectatorChanged(joinAsSpectator);
        }
    }
    private bool joinAsSpectator;

    public event Action<string> CurrentServerClientCountChanged;
    public string CurrentServerClientCount
    {
        get => currentServerClientCount;
        set
        {
            currentServerClientCount = value;
            CurrentServerClientCountChanged(currentServerClientCount);
        }
    }
    private string currentServerClientCount;
}