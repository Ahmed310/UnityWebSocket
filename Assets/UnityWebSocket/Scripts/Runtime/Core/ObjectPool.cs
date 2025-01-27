using System;
using System.Collections.Concurrent;

namespace UnityWebSocket
{
    public class ObjectPool<T> where T : class, new()
    {
        private readonly ConcurrentQueue<T> _pool;

        private readonly int _maxPoolSize;

        public ObjectPool(int initialCapacity = 10, int maxPoolSize = 100)
        {
            _pool = new ConcurrentQueue<T>();
            _maxPoolSize = maxPoolSize;

            for (int i = 0; i < initialCapacity; i++)
            {
                _pool.Enqueue(new T());
            }
        }

        /// <summary>
        /// Get an object from the pool. Creates a new one if the pool is empty.
        /// </summary>
        public T Rent()
        {
            if (_pool.TryDequeue(out var obj))
            {
                return obj;
            }

            return new T();
        }

        /// <summary>
        /// Return an object to the pool for reuse.
        /// </summary>
        public void Return(T obj)
        {
            if (_pool.Count < _maxPoolSize)
            {
                _pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Get the current size of the pool.
        /// </summary>
        public int PoolSize => _pool.Count;
    }

}

