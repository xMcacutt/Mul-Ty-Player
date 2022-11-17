using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal class SaveOpalSyncer : SaveDataSyncer
    {
        readonly OpalHandler HOpal;


        public SaveOpalSyncer(OpalHandler HOpal)
        {
            this.HOpal = HOpal;

        }
    }
}
