using System;
using System.Collections.Concurrent;

namespace UnityWebSocket
{
    public class ObjectPool<T> where T : class, new()
    {
        private readonly ConcurrentQueue<T> pool;

        private readonly int maxPoolSize;

        public ObjectPool(int initialCapacity = 16, int maxPoolSize = 128)
        {
            pool = new ConcurrentQueue<T>();
            this.maxPoolSize = maxPoolSize;

            for (int i = 0; i < initialCapacity; i++)
            {
                pool.Enqueue(new T());
            }
        }

        /// <summary>
        /// Get an object from the pool. Creates a new one if the pool is empty.
        /// </summary>
        public T Rent()
        {
            if (pool.TryDequeue(out var obj))
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
            if (pool.Count < maxPoolSize)
            {
                pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// Get the current size of the pool.
        /// </summary>
        public int PoolSize => pool.Count;
    }

}

