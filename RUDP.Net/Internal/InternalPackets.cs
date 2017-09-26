using System;
using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;

namespace MatthiWare.Net.Sockets.Internal
{
    internal static class InternalPackets
    {
        internal class AckPacket : Packet
        {
            public AckPacket() : base(255, false) { }

            public Guid ClientID { get; set; }

            public override void ReadPacket(ref RawPacket data)
            {
                base.ReadPacket(ref data);

                ClientID = data.ReadGuid();
            }

            public override void WritePacket(ref RawPacket data)
            {
                base.WritePacket(ref data);

                data.Write(ClientID);
            }
        }
    }
}
