/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */

using MatthiWare.Net.Sockets.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.Net.Sockets.Base
{
    public class UdpClient
    {
        protected UdpSocket m_socket;

        public bool Active => m_socket.Active;
        public bool Available => m_socket.Available > 0;

        public UdpClient()
        {
            m_socket = new UdpSocket();
        }

        public void Connect(string hostname, int port) => m_socket.Connect(hostname, port);

        public void Connect(IPEndPoint ep) => m_socket.Connect(ep);

        public void Connect(IPAddress ip, int port) => m_socket.Connect(ip, port);

        public Task<int> SendPacket(IPacket packet)
        {
            return m_socket.SendPacketAsync(packet);
        }

        public Task<Tuple<IPacket, IPEndPoint>> ReceivePacketAsync()
        {
            return m_socket.ReceivePacketAsync();
        }

        public void Close() => m_socket.Close();
    }
}
