using System;
using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;

namespace MatthiWare.Net.Sockets.Internal
{
    internal static class InternalPackets
    {
        #region 32765 ConPacket
        public class HandshakePacket : Packet
        {
            static HandshakePacket()
            {
                PacketReader.RegisterPacket(32765, typeof(AckPacket));
            }

            public override short Id => 32765;

            public override bool IsReliable => true;
        }
        #endregion

        #region 32766 AckPacket
        public class AckPacket : Packet
        {
            static AckPacket()
            {
                PacketReader.RegisterPacket(32766, typeof(AckPacket));
            }

            public override short Id => 32766;

            public override bool IsReliable => false;

            public AckPacket() { }

            public AckPacket(Packet packetToAck)
            {
                Seq = packetToAck.Seq;
            }
        }
        #endregion
    }
}
