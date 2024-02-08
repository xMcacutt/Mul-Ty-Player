namespace MulTyPlayerClient;

public class ServerListing
{
    public bool ActiveDefault { get; set; }
    public string IP { get; set; }
    public string Pass { get; set; }

    public ServerListing(string ip, string pass, bool active)
    {
        IP = ip;
        Pass = pass;
        ActiveDefault = active;
    }
    
    public override string ToString()
    {
        return IP;
    }
}