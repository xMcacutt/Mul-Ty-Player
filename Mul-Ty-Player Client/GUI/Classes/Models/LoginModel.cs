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
    private ServerListing _selectedServer;
    private string _name;
    
    public bool ConnectionAttemptCompleted;
    public bool ConnectionAttemptSuccessful = false;
    
    public event Action OnLoginSuccess;
    public event Action OnLoginFailed;

    public void Connect(string ip, string name, string pass)
    {
        _selectedServer.IP = ip;
        _selectedServer.Pass = pass;
        _name = name;
        Thread connect = new(AttemptConnection);
        connect.Start();
    }

    public string GetName()
    {
        return _name;
    }

    public ServerListing GetSelectedServer()
    {
        return _selectedServer;
    }
    
    public void SetSelectedServer(ServerListing selectedItem)
    {
        _selectedServer = selectedItem;
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
            Client.Start(_selectedServer.IP, _name, _selectedServer.Pass);
            while (!ConnectionAttemptCompleted)
            {
            }
        }
        catch(Exception e)
        {
            Logger.Write(e.Message);
            LoginFailed();
            return;
        }
        if (ConnectionAttemptSuccessful)
        {
            SaveDetails();
            OnLoginSuccess?.Invoke();
        }
        else
        {
            LoginFailed();
        }
    }
    
    public void Setup()
    {
        SetName();

        if (Path.Exists("./list.servers"))
        {
            ParseServerList(File.ReadLines("./list.servers"));
        }
        else
        {
            var fs = File.Create("./list.servers");
            fs.Close();
        }
    }

    public void SetName()
    {
        //If steam name retrieval is successful, set that as the name and set the default name in settings
        if (SettingsHandler.Settings.DoGetSteamName)
        {
            _name = SteamHelper.GetSteamName();
            SettingsHandler.Settings.DefaultName = _name;
        }
        //If the steam name setting is disabled, use the previously stored default name
        else
        {
            _name = SettingsHandler.Settings.DefaultName;
        }

        //If the name is still null, generate a random one
        if (_name == null) _name = GenerateRandomUser();
    }

    private void SaveDetails()
    {
        foreach (var x in _serverList) x.ActiveDefault = false;
        //Save the currently connected server details to server list
        if (!_serverList.Any(x => x.IP == _selectedServer.IP))
        {
            _serverList.Add(new ServerListing(_selectedServer.IP, _selectedServer.Pass, true));
            return;
        }
        _serverList.First(x => x.IP == _selectedServer.IP).Pass = _selectedServer.Pass;
        _serverList.First(x => x.IP == _selectedServer.IP).ActiveDefault = true;
    
        //Save server list to file
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

        _selectedServer = _serverList.FirstOrDefault(x => x.ActiveDefault, local);
    }

    private string GenerateRandomUser()
    {
        Random random = new();
        var randomNumber = random.Next(100000, 999999);
        return "USER" + randomNumber;
    }

    private void LoginFailed()
    {
        SystemSounds.Hand.Play();
        MessageBox.Show("Connection failed!\nPlease check IPAddress & Password are correct and server is open.");
        OnLoginFailed?.Invoke();
    }


}