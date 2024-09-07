using System;

namespace MulTyPlayerServer;

public class VersionHandler
{
    public static VersionResult? Compare(string firstVersion, string secondVersion)
    {
        if (!Version.TryParse(firstVersion, out var client) ||
            !Version.TryParse(secondVersion, out var server)) return null;
        var comparisonResult = client.CompareTo(server);
        return comparisonResult switch
        {
            < 0 => VersionResult.SecondNewer,
            > 0 => VersionResult.FirstNewer,
            _ => VersionResult.NeitherNewer
        };
    }
}

public enum VersionResult
{
    FirstNewer,
    SecondNewer,
    NeitherNewer
}