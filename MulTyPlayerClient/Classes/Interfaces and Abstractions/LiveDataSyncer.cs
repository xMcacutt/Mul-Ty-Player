using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient
{
    internal abstract class LiveDataSyncer
    {
        public virtual void Collect(int index)
        {

        }

        public virtual void SyncAll(byte[] bytes, int amount, int checkState)
        {
            for (int i = 0; i < amount; i++)
            {
                if (bytes[i] == checkState) Collect(i);
            }
        }

        public virtual byte[] ReadData()
        {
            return new byte[2];
        }
    }
}
