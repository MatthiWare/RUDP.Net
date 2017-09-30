using MatthiWare.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class TestClient : RUdpClient
    {
        public TestClient(string host, int port) : base(host, port)
        {
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();


        }
    }
}
