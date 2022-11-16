using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal abstract class SaveDataSyncer
    {
        public virtual void Save(int index) { }

        public virtual void SyncAll(byte[] bytes) { }

    }
}
