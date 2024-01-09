using System.Collections.Generic;

namespace MulTyPlayerClient;

public enum Koala
{
    Katie,
    Mim,
    Elizabeth,
    Snugs,
    Gummy,
    Dubbo,
    Kiki,
    Boonie
}

public class KoalaInfo
{
    public KoalaInfo(Koala koala)
    {
        Koala = koala;
        Name = koala.ToString();
        Id = (int)koala;
    }

    public Koala Koala { get; private set; }
    public string Name { get; private set; }
    public int Id { get; private set; }
}

public static class Koalas
{
    private static readonly KoalaInfo Katie = new(Koala.Katie);
    private static readonly KoalaInfo Mim = new(Koala.Mim);
    private static readonly KoalaInfo Elizabeth = new(Koala.Elizabeth);
    private static readonly KoalaInfo Snugs = new(Koala.Snugs);
    private static readonly KoalaInfo Gummy = new(Koala.Gummy);
    private static readonly KoalaInfo Dubbo = new(Koala.Dubbo);
    private static readonly KoalaInfo Kiki = new(Koala.Kiki);
    private static readonly KoalaInfo Boonie = new(Koala.Boonie);

    public static readonly Dictionary<Koala, KoalaInfo> GetInfo = new()
    {
        { Koala.Katie, Katie },
        { Koala.Mim, Mim },
        { Koala.Elizabeth, Elizabeth },
        { Koala.Snugs, Snugs },
        { Koala.Gummy, Gummy },
        { Koala.Dubbo, Dubbo },
        { Koala.Kiki, Kiki },
        { Koala.Boonie, Boonie }
    };
}