using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Internal;
using MatthiWare.Net.Sockets.Packets;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static MatthiWare.Net.Sockets.Internal.InternalPackets;

namespace MatthiWare.Net.Sockets
{
    public abstract class RUdpClient
    {
        public delegate void HandlePacket(RUdpClient server, Packet packet);

        private UdpClient m_client;
        private HandlePacket[] m_packetHandlers = new HandlePacket[short.MaxValue];

        private ClientInfo m_clientInfo;
        public ClientInfo Client => m_clientInfo;

        private string m_host;
        private int m_port;
        private bool m_running;

        public RUdpClient(string host, int port)
        {
            m_client = new UdpClient();

            m_host = host;
            m_port = port;
        }

        public void Start() => OnStart();

        protected virtual void OnStart()
        {
            m_client.Connect(m_host, m_port);

            m_clientInfo = new ClientInfo(m_client.LocalEndPoint);

            Task.Factory.StartNew(RunReceiver, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(RunSender, TaskCreationOptions.LongRunning);
        }

        public void Stop() => OnStop();

        protected virtual void OnStop()
        {
            m_running = false;
            m_client.Close();
            m_clientInfo = null;
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
                packet.Seq = Client.GetNextSeqNumber();
                packet.ResendTime = DateTime.Now.AddMilliseconds(50);
                Client.ReliablePackets.Add(packet);
            }

            Client.SendQueue.Enqueue(packet);
        }

        protected void OnHandlePacket(Packet packet)
        {
            if (packet.IsReliable)
                SendAck(packet);
        }

        private void SendAck(Packet packet) => Client.SendQueue.Enqueue(new AckPacket(packet));

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
