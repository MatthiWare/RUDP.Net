/*  Copyright (C) 2017 MatthiWare
 *  Copyright (C) 2017 Matthias Beerens
 *  For the full notice see <https://github.com/MatthiWare/RUDP.Net/blob/master/LICENSE>. 
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Security.Permissions;
using MatthiWare.Net.Sockets.Utils;

namespace MatthiWare.Net.Sockets.Base
{
    public class UdpSocket : IDisposable
    {
        private const int MAX_BUFFER_SIZE = 8 * 1024;

        private AddressFamily m_family = AddressFamily.InterNetwork;
        private byte[] m_buffer = new byte[MAX_BUFFER_SIZE];
        private Socket m_client;

        public int Available => m_client.Available;

        public bool Active { get; set; }

        public short Ttl
        {
            get { return m_client.Ttl; }
            set { m_client.Ttl = value; }
        }

        public bool ExclusiveAddressUse
        {
            get { return m_client.ExclusiveAddressUse; }
            set { m_client.ExclusiveAddressUse = value; }
        }

        public bool DontFragment
        {
            get { return m_client.DontFragment; }
            set { m_client.DontFragment = value; }
        }

        public void AllowNatTraversal(bool allowed) => m_client.SetIPProtectionLevel(allowed ? IPProtectionLevel.Unrestricted : IPProtectionLevel.EdgeRestricted);

        public UdpSocket() : this(AddressFamily.InterNetwork)
        { }

        public UdpSocket(AddressFamily family)
        {
            if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6) throw new ArgumentException("Invalid protocol family", nameof(family));

            m_family = family;

            m_client = CreateClient();
        }

        public void Connect(string hostname, int port)
        {
            if (string.IsNullOrEmpty(hostname)) throw new ArgumentNullException(nameof(hostname));
            if (!Helper.IsValidTcpPort(port)) throw new ArgumentOutOfRangeException(nameof(port));

            var addresses = Dns.GetHostAddresses(hostname);
            Exception ex = null;

            foreach (var addr in addresses)
            {
                if (addr.AddressFamily != m_family)
                    continue;

                try
                {
                    Connect(new IPEndPoint(addr, port));
                    break;
                }
                catch (Exception e)
                {
                    ex = e;
                }
            }

            if (Active)
                return;

            throw (ex != null) ? ex : new SocketException((int)SocketError.NotConnected);
        }

        public void Connect(IPAddress ip, int port)
        {
            if (ip == null) throw new ArgumentNullException(nameof(ip));
            if (!Helper.IsValidTcpPort(port)) throw new ArgumentOutOfRangeException(nameof(port));

            IPEndPoint ep = new IPEndPoint(ip, port);

            Connect(ep);
        }

        public void Connect(IPEndPoint endPoint)
        {
            if (endPoint == null) throw new ArgumentNullException(nameof(endPoint));

            m_client.Connect(endPoint);

            Active = true;
        }

        public void Bind(int port)
        {
            if (!Helper.IsValidTcpPort(port)) throw new ArgumentOutOfRangeException(nameof(port));

            Bind(Helper.GetEp(m_family, port));
        }

        public void Bind(EndPoint localEP)
        {
            if (localEP == null) throw new ArgumentNullException(nameof(localEP));

            m_client.Bind(localEP);
        }

        public void Listen() => m_client.Listen((int)SocketOptionName.MaxConnections);


        public int Send(byte[] buffer, int offset, int size)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (!Active) throw new InvalidOperationException("Not connected");

            return m_client.Send(buffer, offset, size, SocketFlags.None);
        }

        public byte[] Receive(ref IPEndPoint remoteEP)
        {
            var ep = Helper.GetEp(m_family);

            int rcvd = m_client.ReceiveFrom(m_buffer, SocketFlags.None, ref ep);

            if (rcvd < MAX_BUFFER_SIZE)
            {
                byte[] buffer = new byte[rcvd];
                Buffer.BlockCopy(m_buffer, 0, buffer, 0, rcvd);

                return buffer;
            }

            return m_buffer;
        }

        public void Disconnect(bool reuse)
        {
            m_client.Disconnect(reuse);
        }

        [HostProtection(ExternalThreading = true)]
        public IAsyncResult BeginReceive(AsyncCallback callBack, object state)
        {
            var ep = Helper.GetEp(m_family);

            return m_client.BeginReceiveFrom(m_buffer, 0, MAX_BUFFER_SIZE, SocketFlags.None, ref ep, callBack, state);
        }

        public byte[] EndReceive(IAsyncResult result, ref IPEndPoint remoteEP)
        {
            var ep = Helper.GetEp(m_family);

            int rcvd = m_client.EndReceiveFrom(result, ref ep);
            remoteEP = (IPEndPoint)ep;

            if (rcvd < MAX_BUFFER_SIZE)
            {
                byte[] buffer = new byte[rcvd];
                Buffer.BlockCopy(m_buffer, 0, buffer, 0, rcvd);

                return buffer;
            }

            return m_buffer;
        }

        [HostProtection(ExternalThreading = true)]
        public IAsyncResult BeginSend(byte[] buffer, int offset, int size, AsyncCallback callBack, object state)
        {
            if (!Active) throw new InvalidOperationException("Not connected");

            return m_client.BeginSend(buffer, offset, size, SocketFlags.None, callBack, state);
        }

        public int EndSend(IAsyncResult result)
        {
            return m_client.EndSend(result);
        }

        [HostProtection(ExternalThreading = true)]
        public Task<int> SendAsync(byte[] buffer, int offset, int size)
        {
            return Task<int>.Factory.FromAsync(BeginSend, EndSend, buffer, offset, size, null);
        }

        [HostProtection(ExternalThreading = true)]
        public Task<UdpReceiveResult> ReceiveAsync()
        {
            return Task<UdpReceiveResult>.Factory.FromAsync((cb, state) => BeginReceive(cb, state), (ar) =>
            {
                IPEndPoint remoteEP = null;
                byte[] buffer = EndReceive(ar, ref remoteEP);
                return new UdpReceiveResult(buffer, remoteEP);

            }, null);
        }

        private Socket CreateClient()
        {
            return new Socket(m_family, SocketType.Dgram, ProtocolType.Udp);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    m_client.Shutdown(SocketShutdown.Both);
                    m_client.Close();

                    m_client = null;

                    m_buffer = null;
                    Active = false;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UdpSocket() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
