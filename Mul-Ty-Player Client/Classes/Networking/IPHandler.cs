using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MulTyPlayerClient.GUI.Models;

namespace MulTyPlayerClient.Classes.Networking;

public class IPHandler
{
    public static string? ParseIPSilently(string ip)
    {
        try
        {
            ushort port = 8750;
            if (ip.Contains(':'))
            {
                var parts = ip.Split(':');
                ip = parts[0];
                if (!ushort.TryParse(parts[1], out port))
                    return null;
            }
            else if (Regex.IsMatch(ip, @"\|s\d$"))
            {
                var serverPortIndex = int.Parse(ip.Last().ToString());
                port = (ushort)(8750 + (serverPortIndex - 1) * 5);
                ip = ip[..^3];
            }

            if (IPAddress.TryParse(ip, out _))
                return $"{ip}:{port}";

            var addresses = Dns.GetHostAddresses(ip);
            return addresses.Length == 0 ? null : $"{addresses[0]}:{port}";
        }
        catch
        {
            return null;
        }
    }
    
    public static string ParseIP(string ip)
    {
        try
        {
            ushort port = 8750;
            if (ip.Contains(':'))
            {
                var parts = ip.Split(':');
                ip = parts[0];
                if (!ushort.TryParse(parts[1], out port))
                    throw new InvalidPortException();
            }
            else if (Regex.IsMatch(ip, @"\|s\d$"))
            {
                var serverPortIndex = int.Parse(ip.Last().ToString());
                port = (ushort)(8750 + (serverPortIndex - 1) * 5);
                ip = ip[..^3];
            }
            
            if (IPAddress.TryParse(ip, out _))
                return $"{ip}:{port}";
          
            var addresses = Dns.GetHostAddresses(ip);
            if (addresses.Length == 0)
                throw new NoDomainsException();
            return $"{addresses[0]}:{port}";
        }
        catch (NoDomainsException)
        {
            MessageBox.Show("No IP addresses found for the given domain name.");
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
            ModelController.Login.WasConnectionError = false;
            return "";
        }
        catch (ArgumentNullException)
        {
            MessageBox.Show("The provided IP or domain name is null.");
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
            ModelController.Login.WasConnectionError = false;
            return "";
        }
        catch (ArgumentException)
        {
            MessageBox.Show("The provided IP or domain name is not valid.");
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
            ModelController.Login.WasConnectionError = false;
            return "";
        }
        catch (SocketException ex)
        {
            MessageBox.Show($"DNS resolution failed. Error: {ex.Message}");
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
            ModelController.Login.WasConnectionError = false;
            return "";
        }
        catch (InvalidPortException)
        {
            MessageBox.Show($"Port provided was invalid.");
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
            ModelController.Login.WasConnectionError = false;
            return "";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unexpected error occurred. Error: {ex.Message}");
            ModelController.Login.ConnectionAttemptSuccessful = false;
            ModelController.Login.ConnectionAttemptCompleted = true;
            ModelController.Login.WasConnectionError = false;
            return "";
        }
    }
}

public class InvalidPortException : SystemException
{
    public InvalidPortException() : base("The specified port is invalid.")
    {
    }

    public InvalidPortException(string message) : base(message)
    {
    }

    public InvalidPortException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class InvalidIPException : SystemException
{
    public InvalidIPException() : base("The specified IP address is invalid.")
    {
    }

    public InvalidIPException(string message) : base(message)
    {
    }

    public InvalidIPException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class NoDomainsException : SystemException
{
    public NoDomainsException() : base("No addresses are pointed to by this domain.")
    {
    }

    public NoDomainsException(string message) : base(message)
    {
    }

    public NoDomainsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}