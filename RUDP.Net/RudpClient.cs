using MatthiWare.Net.Sockets.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.Net.Sockets
{
    public class RUdpClient
    {
        protected UdpSocket m_socket;

        public bool Active => m_socket.Active;
        public int Available => m_socket.Available;

        public RUdpClient()
        {
            m_socket = new UdpSocket();
        }

        public void Connect(string hostname, int port)
        {
            
        }
    }
}
