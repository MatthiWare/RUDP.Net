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

            RegisterPacketHandlers();
        }

        public void Start() => OnStart();

        protected virtual void OnStart()
        {
            m_client.Connect(m_host, m_port);

            m_clientInfo = new ClientInfo(m_client.LocalEndPoint);

            m_running = true;

            Task.Factory.StartNew(RunReceiver, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(RunSender, TaskCreationOptions.LongRunning);

            SendPacket(new HandshakePacket());
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
            RegisterPacketHandler(typeof(HandshakePacket), PacketHandlerInternal.HandleHandshakePacketClient);
        }

        public void RegisterPacketHandler(Type packetType, HandlePacket handler)
        {
            var packet = PacketReader.PacketFromType(packetType);

            if (m_packetHandlers[packet.Id] != null)
                Debug.WriteLine($"Handler for packet 0x{packet.Id.ToString("X2")} overwritten", "Warning");

            PacketReader.RegisterPacket(packet.Id, packetType);

            m_packetHandlers[packet.Id] = handler;
        }

        public void SendPacket(Packet packet)
        {
            if (packet.IsReliable)
            {
                packet.Seq = Client.GetNextSeqNumber();
                packet.ResendTime = DateTime.Now.AddMilliseconds(200);
                Client.ReliablePackets.Add(packet);
            }

            Console.WriteLine($"Add packet to queue {packet}");

            Client.SendQueue.Enqueue(packet);
        }

        protected void OnHandlePacket(Packet packet)
        {
            if (packet.IsReliable)
                SendAck(packet);

            Console.WriteLine($"Handling packet {packet}");

            m_packetHandlers[packet.Id](this, packet);
        }

        private void SendAck(Packet packet) => Client.SendQueue.Enqueue(new AckPacket(packet));

        private async void RunReceiver()
        {
            Console.WriteLine("Starting receiver..");

            while (m_running)
            {
                var data = await m_client.ReceivePacketAsync();

                OnHandlePacket(data.Item1);

                await Task.Delay(1);

                AddLostPacketsToQueue();
            }

            Console.WriteLine("Exited receiver..");
        }

        private async void RunSender()
        {
            Console.WriteLine("Starting sender..");

            while (m_running)
            {
                Packet packet = null;

                while (Client.SendQueue.TryDequeue(out packet))
                {
                    Console.WriteLine($"Sending packet {packet}");

                    await m_client.SendPacketAsync(packet);
                }

                await Task.Delay(1);
            }

            Console.WriteLine("Exited sender..");
        }

        private void AddLostPacketsToQueue()
        {
            foreach (var packet in Client.ReliablePackets)
            {
                var now = DateTime.Now;

                if (packet.ResendTime <= now)
                {
                    Console.WriteLine($"Resending packet {packet}");

                    Client.SendQueue.Enqueue(packet);
                }
            }

        }
    }
}
