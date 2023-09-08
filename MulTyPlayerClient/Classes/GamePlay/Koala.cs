using System.Collections.Generic;

namespace MulTyPlayerClient
{
    public enum Koala : int
    {
        Katie, Mim, Elizabeth, Snugs, Gummy, Dubbo, Kiki, Boonie
    }

    public class KoalaInfo
    {
        public KoalaInfo(Koala koala)
        {
            this.Koala = koala;
            Name = koala.ToString();
            Id = (int)koala;
        }
        public Koala Koala { get; private set; }
        public string Name { get; private set; }
        public int Id { get; private set; }
    }

    public static class Koalas
    {
        static KoalaInfo Katie = new KoalaInfo(Koala.Katie);
        static KoalaInfo Mim = new KoalaInfo(Koala.Mim);
        static KoalaInfo Elizabeth = new KoalaInfo(Koala.Elizabeth);
        static KoalaInfo Snugs = new KoalaInfo(Koala.Snugs);
        static KoalaInfo Gummy = new KoalaInfo(Koala.Gummy);
        static KoalaInfo Dubbo = new KoalaInfo(Koala.Dubbo);
        static KoalaInfo Kiki = new KoalaInfo(Koala.Kiki);
        static KoalaInfo Boonie = new KoalaInfo(Koala.Boonie);

        public readonly static Dictionary<Koala, KoalaInfo> GetInfo = new()
        {
            { Koala.Katie, Katie },
            { Koala.Mim, Mim },
            { Koala.Elizabeth, Elizabeth },
            { Koala.Snugs, Snugs },
            { Koala.Gummy, Gummy },
            { Koala.Dubbo, Dubbo },
            { Koala.Kiki, Kiki },
            { Koala.Boonie, Boonie },
        };
    }
}
