using MatthiWare.Net.Sockets.Threading;
using System;
using System.Linq;

namespace MatthiWare.Net.Sockets.Utils
{
    public static class Extensions
    {
        public static void RemoveAll<T>(this ConcurrentList<T> self, Func<T, bool> predicate)
        {
            for (int i = self.Count -1; i >= 0; i--)
                if (predicate(self[i]))
                    self.RemoveAt(i);
        }
    }
}
