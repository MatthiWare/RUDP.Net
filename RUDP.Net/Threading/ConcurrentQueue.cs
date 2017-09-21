using System.Collections.Generic;

namespace MatthiWare.Net.Sockets.Threading
{
    public class ConcurrentQueue<T>
    {
        private readonly Queue<T> m_queue;
        private readonly object sync = new object();

        public ConcurrentQueue()
        {
            m_queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            lock (sync)
                m_queue.Enqueue(item);
        }

        public int Count
        {
            get
            {
                lock (sync)
                    return m_queue.Count;
            }
        }

        public bool TryDequeue(out T data)
        {
            data = default(T);

            lock (sync)
            {
                if (m_queue.Count <= 0)
                    return false;

                data = m_queue.Dequeue();
                return true;
            }
        }

        public void Clear()
        {
            lock (sync)
                m_queue.Clear();
        }
    }
}
