using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class Player
    {
        public Koala Koala { get; set; }
        public string Name;
        public ushort Id;
        
        public Player(Koala koala, string name, ushort id)
        {
            this.Koala = koala;
            this.Name = name;
            this.Id = id;
        }
    }
}
