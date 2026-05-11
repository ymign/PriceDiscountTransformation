using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.BizLogic.Fee
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> _objects;
        private readonly int _maxSize;

        public ObjectPool(int maxSize)
        {
            _objects = new Stack<T>(maxSize);
            _maxSize = maxSize;
        }

        public T Get()
        {
            lock (_objects)
            {
                if (_objects.Count > 0)
                {
                    return _objects.Pop();
                }
                else
                {
                    return new T();
                }
            }
        }

        public void Return(T obj)
        {
            lock (_objects)
            {
                if (_objects.Count < _maxSize)
                {
                    _objects.Push(obj);
                }
            }
        }
    }
}
