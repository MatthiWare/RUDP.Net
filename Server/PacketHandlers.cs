using MatthiWare.Net.Sockets;
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
        public static void LoginPacketHandler(RUdpListener server, IPacket packet, IPEndPoint ep)
        {
            Console.WriteLine($"Connection from {ep}");

            var login = (LoginPacket)packet;

            Console.WriteLine($"Logged in: {login.Username}");
        }
    }
}
