using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Utils;
using System;
using System.Linq;
using static MatthiWare.Net.Sockets.Internal.InternalPackets;

namespace MatthiWare.Net.Sockets.Internal
{
    internal static class PacketHandlerInternal
    {
        #region AckPacket
        public static void HandleAckPacketServer(RUdpServer server, Packet packet, ClientInfo client)
        {
            client.ReliablePackets.RemoveAll(p => p.Seq == packet.Seq);
        }

        public static void HandleAckPacketClient(RUdpClient client, Packet packet)
        {
            client.Client.ReliablePackets.RemoveAll(p => p.Seq == packet.Seq);
        }
        #endregion

        #region HandshakePacket
        public static void HandleHandshakePacketServer(RUdpServer server, Packet packet, ClientInfo client)
        {
            client.IsActive = true;

            server.SendPacket(new HandshakePacket(), client);

            //Console.WriteLine($"Received connection from '{client.EndPoint}'!");
        }

        public static void HandleHandshakePacketClient(RUdpClient client, Packet packet)
        {
            client.Client.IsActive = true;

            Console.WriteLine("Server is online!");
        }
        #endregion
    }
}
