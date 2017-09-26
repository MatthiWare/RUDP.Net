using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Threading;
using System;
using System.Collections.Generic;
using System.Net;

namespace MatthiWare.Net.Sockets
{
    public interface IClientInfo
    {
        Guid ClientID { get; }
        EndPoint EndPoint { get; }
        ConcurrentQueue<Packet> SendQueue { get; }
        List<Packet> ReliablePackets { get; }
        long Seq { get; set; }
    }
}
