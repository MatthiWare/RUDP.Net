using MatthiWare.Net.Sockets.Packets;

namespace MatthiWare.Net.Sockets.Internal
{
    internal static class PacketHandlerInternal
    {
        public static void HandleAckPacketServer(RUdpServer server, Packet packet, ClientInfo client)
        {
            client.ReliablePackets.RemoveAll(p => p.Seq == packet.Seq);
        }

        public static void HandleAckPacketClient(RUdpClient client, Packet packet)
        {
            client.Client.ReliablePackets.RemoveAll(p => p.Seq == packet.Seq);
        }
    }
}
