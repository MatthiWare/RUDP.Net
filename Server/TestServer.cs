using MatthiWare.Net.Sockets;
using Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class TestServer : RUdpServer
    {
        public TestServer(int port) : base(port)
        {
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();

            RegisterPacketHandler(typeof(LoginPacket), PacketHandlers.LoginPacketHandler);
            RegisterPacketHandler(typeof(ChatPacket), PacketHandlers.ChatPacketHandler);
        }
    }
}
