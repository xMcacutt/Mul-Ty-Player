using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MulTyPlayerClient.Memory
{
    public class MemoryObserver<T> where T : unmanaged
    {
        public event Action<T, T> OnValueChange = delegate { };

        protected nint address;
        protected Func<T, T, bool> equator;
        protected T currentValue;
        protected T previousValue;

        public MemoryObserver(nint address, Func<T, T, bool> equator)
        {
            this.address = address;
            this.equator = equator;
        }

        public void Peek()
        {
            if (ProcessHandler.TryRead(address, out T currentValue, false))
            {
                if (!equator(currentValue, previousValue))
                {
                    OnValueChange?.Invoke(previousValue, currentValue);
                    previousValue = currentValue;
                }
            }
        }

        public T GetCurrentValue() => currentValue;

        public void MoveAddress(nint newAddress)
        {
            address = newAddress;
        }
    }

    public class IntObserver : MemoryObserver<int>
    {
        public IntObserver(nint address) : base(address, (int i, int j) => i == j)
        {            
        }
    }

    public class FloatObserver : MemoryObserver<float>
    {
        public FloatObserver(nint address) : base(address, (float i, float j) => i == j)
        {
        }
    }

    public class ByteObserver : MemoryObserver<byte>
    {
        public ByteObserver(nint address) : base(address, (byte i, byte j) => i == j)
        {
        }
    }
}
