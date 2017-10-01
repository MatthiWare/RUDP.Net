using MatthiWare.Net.Sockets;
using MatthiWare.Net.Sockets.Packets;
using Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Handler
{
    public static class PacketHandlers
    {
        public static void HandleChatPacket(RUdpClient client, Packet packet)
        {
            var chat = (ChatPacket)packet;
            var myclient = (TestClient)client;

            myclient.AddText(chat.Message);
        }

        public static void HandleLoginPacket(RUdpClient client, Packet packet)
        {
            var login = (LoginPacket)packet;
            var myclient = (TestClient)client;

            myclient.AddText($"{login.Username} logged in!");
        }

    }
}
