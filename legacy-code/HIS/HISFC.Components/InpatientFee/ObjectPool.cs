using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Components.InpatientFee
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Queue<T> _pool;

        public ObjectPool()
        {
            _pool = new Queue<T>();
        }

        public T Get()
        {
            lock (_pool)
            {
                if (_pool.Count > 0)
                {
                    return _pool.Dequeue();
                }
                else
                {
                    return new T();
                }
            }
        }

        public void Return(T item)
        {
            lock (_pool)
            {
                _pool.Enqueue(item);
            }
        }
    }

}
