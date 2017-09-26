using System;
using System.Net;
using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Threading;
using System.Collections.Generic;

namespace MatthiWare.Net.Sockets.Internal
{
    internal class ClientInfo : IClientInfo
    {
        public Guid ClientID { get; }

        public EndPoint EndPoint { get; }

        public ConcurrentQueue<Packet> SendQueue { get; } = new ConcurrentQueue<Packet>();

        public List<Packet> ReliablePackets { get; } = new List<Packet>();

        public long Seq { get; set; }

        public ClientInfo(Guid id, EndPoint ep)
        {
            ClientID = id;
            EndPoint = ep;
            Seq = 0;
        }
    }
}
