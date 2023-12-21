namespace MulTyPlayerClient;

internal class ServerListing
{
    public bool ActiveDefault;
    public string IP;
    public string Pass;

    public ServerListing(string iP, string pass, bool active)
    {
        IP = iP;
        Pass = pass;
        ActiveDefault = active;
    }
}