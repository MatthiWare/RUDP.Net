using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Threading;
using System;
using System.Net;

namespace MatthiWare.Net.Sockets
{
    public interface IClientInfo
    {
        Guid ClientID { get; }
        EndPoint EndPoint { get; }
        ConcurrentQueue<IPacket> SendQueue { get; }
        long Seq { get; set; }
    }
}
