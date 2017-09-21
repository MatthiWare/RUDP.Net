using System;
using System.Net;
using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Threading;

namespace MatthiWare.Net.Sockets.Internal
{
    internal class ClientInfo : IClientInfo
    {
        public Guid ClientID { get; }

        public EndPoint EndPoint { get; }

        public ConcurrentQueue<IPacket> SendQueue { get; } = new ConcurrentQueue<IPacket>();

        public long Seq { get; set; }

        public ClientInfo(Guid id, EndPoint ep)
        {
            ClientID = id;
            EndPoint = ep;
            Seq = 0;
        }
    }
}
