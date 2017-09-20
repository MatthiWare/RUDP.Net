/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */

using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MatthiWare.Net.Sockets
{
    public class RUdpListener
    {
        private UdpSocket m_server;
        private EndPoint m_serverEP;

        public bool Active { get; private set; }

        public RUdpListener(IPAddress ip, int port)
        {
            m_serverEP = new IPEndPoint(ip, port);
            m_server = new UdpSocket();
        }

        public void Start()
        {
            if (Active) return;

            m_server.Bind(m_serverEP);

            //try
            //{
            //    m_server.Listen();
            //}
            //catch (SocketException)
            //{
            //    Stop();
            //    throw;
            //}

            Active = true;
        }

        public void Stop()
        {
            m_server.Dispose();

            Active = false;

            m_server = new UdpSocket();
        }

        public Task<Tuple<IPacket, IPEndPoint>> ReceivePacketAsync()
        {
            return m_server.ReceivePacketAsync();
        }

    }
}
