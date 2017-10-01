using MatthiWare.Net.Sockets;
using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;
using Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal static class PacketHandlers
    {
        public static void LoginPacketHandler(RUdpServer server, Packet packet, ClientInfo client)
        {
            // Console.WriteLine($"Connection from {client.EndPoint}");

            var login = (LoginPacket)packet;

            Console.WriteLine($"Logged in: {login.Username}");

            foreach (ClientInfo c in server.Clients)
                if (!c.EndPoint.Equals(client.EndPoint))
                    server.SendPacket(packet, c);
        }
    }
}
