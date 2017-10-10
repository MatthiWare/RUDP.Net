using Client.Handler;
using MatthiWare.Net.Sockets;
using Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class TestClient : RUdpClient
    {
        private ClientForm m_client;

        public TestClient(string host, int port, ClientForm client) : base(host, port)
        {
            m_client = client;
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();

            RegisterPacketHandler(typeof(ChatPacket), PacketHandlers.HandleChatPacket);
            RegisterPacketHandler(typeof(LoginPacket), PacketHandlers.HandleLoginPacket);
        }

        public void AddText(string msg) => m_client.AddText(msg);


    }
}
