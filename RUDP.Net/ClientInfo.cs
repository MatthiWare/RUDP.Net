using System;
using System.Net;
using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Threading;
using System.Collections.Generic;

namespace MatthiWare.Net.Sockets
{
    public class ClientInfo
    {
        public EndPoint EndPoint { get; }

        internal ConcurrentQueue<Packet> SendQueue { get; } = new ConcurrentQueue<Packet>();

        internal ConcurrentList<Packet> ReliablePackets { get; } = new ConcurrentList<Packet>();

        public bool IsActive { get; internal set; }

        private long m_seq;

        public ClientInfo(EndPoint ep)
        {
            EndPoint = ep;
            m_seq = 0;
        }

        public long GetNextSeqNumber() => m_seq++;

        public void ResetSeq() => m_seq = 0;
    }
}
