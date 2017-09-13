using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.Net.Sockets.Utils
{
    internal static class Helper
    {

        public static bool IsValidTcpPort(int port)
        {
            return port >= IPEndPoint.MinPort && port <= IPEndPoint.MinPort;
        }

    }
}
