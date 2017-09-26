using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Internal;
using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MatthiWare.Net.Sockets.Internal.InternalPackets;

namespace MatthiWare.Net.Sockets
{
    public abstract class RUdpClient
    {
        public delegate void HandlePacket(RUdpClient server, Packet packet);

        private UdpClient m_client;
        private HandlePacket[] m_packetHandlers = new HandlePacket[short.MaxValue];
        private IClientInfo m_self;

        private string m_host;
        private int m_port;
        private bool m_running;

        public RUdpClient(string host, int port)
        {
            m_client = new UdpClient();

            m_host = host;
            m_port = port;

            m_self = new ClientInfo(Guid.NewGuid(), null);
        }

        public void Start() => OnStart();

        protected virtual void OnStart()
        {
            m_client.Connect(m_host, m_port);

            Task.Factory.StartNew(RunReceiver, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(RunSender, TaskCreationOptions.LongRunning);
        }

        public void Stop() => OnStop();

        protected virtual void OnStop()
        {
            m_running = false;
            m_client.Close();
        }

        protected virtual void RegisterPacketHandlers()
        {
            RegisterPacketHandler(typeof(AckPacket), PacketHandlerInternal.HandleAckPacketClient);
        }

        public void RegisterPacketHandler(Type packetType, HandlePacket handler)
        {
            var packet = PacketReader.PacketFromType(packetType);

            if (m_packetHandlers[packet.Id] != null)
                Debug.WriteLine($"Handler for packet 0x{packet.Id.ToString("X2")} overwritten", "Warning");

            m_packetHandlers[packet.Id] = handler;
        }

        public void AddPacket(Packet packet)
        {
            if (packet.IsReliable)
            {
                var cast = (Packet)packet;
                cast.ResendTime = DateTime.Now.AddMilliseconds(50);
                m_self.ReliablePackets.Add(packet);
            }

            m_self.SendQueue.Enqueue(packet);
        }

        protected void OnHandlePacket(Packet packet)
        {
            if (packet.IsReliable)
                SendAckPacket(packet);


        }

        private void SendAckPacket(Packet packet)
        {
            m_self.SendQueue.Enqueue(new AckPacket
            {
                Seq = packet.Seq,
                ClientID = m_self.ClientID
            });
        }

        private async void RunReceiver()
        {
            while (m_running)
            {
                var data = await m_client.ReceivePacketAsync();

            }
        }

        private void RunSender()
        {

        }
    }
}
