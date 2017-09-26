using MatthiWare.Net.Sockets.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.Net.Sockets.Internal
{
    internal static class PacketHandlerInternal
    {
        public static void HandleAckPacketServer(RUdpServer server, Packet packet, IClientInfo client)
        {
            var ack = (AckPacket)packet;

            client.ReliablePackets.RemoveAll(p => p.Seq == ack.Seq);
        }

        public static void HandleAckPacketClient(RUdpClient client, Packet packet)
        {
            var ack = (AckPacket)packet;

            client.ReliablePackets.RemoveAll(p => p.Seq == ack.Seq);
        }
    }
}
