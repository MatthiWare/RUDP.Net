using System;
using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;

namespace MatthiWare.Net.Sockets.Internal
{
    internal static class InternalPackets
    {
        #region 32766 ConPacket
        public class ConPacket : Packet
        {
            public ConPacket() : base(32766, true) { }
        }
        #endregion

        #region 32767 AckPacket
        public class AckPacket : Packet
        {
            public AckPacket(Packet packetToAck) : base(32767, false)
            {
                Seq = packetToAck.Seq;
            }
        }
        #endregion
    }
}
