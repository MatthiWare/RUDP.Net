/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.Net.Sockets.Utils
{
    internal static class Helper
    {
        public static bool IsValidTcpPort(int port) => port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort;

        public static EndPoint GetEp(AddressFamily family) => GetEp(family, 0);

        public static EndPoint GetEp(AddressFamily family, int port) => new IPEndPoint(family == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, port);
    }
}

