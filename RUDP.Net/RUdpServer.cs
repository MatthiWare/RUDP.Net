using MatthiWare.Net.Sockets;
using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Internal;
using MatthiWare.Net.Sockets.Packets;
using MatthiWare.Net.Sockets.Threading;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static MatthiWare.Net.Sockets.Internal.InternalPackets;

namespace MatthiWare.Net.Sockets
{
    public abstract class RUdpServer
    {
        public delegate void HandlePacket(RUdpServer server, Packet packet, ClientInfo client);

        private UdpListener m_server;
        private readonly object m_networkSync = new object();
        private bool m_running = false;

        private HandlePacket[] m_packetHandlers = new HandlePacket[short.MaxValue];

        public ConcurrentList<ClientInfo> Clients { get; } = new ConcurrentList<ClientInfo>();

        public RUdpServer(int port)
        {
            m_server = new UdpListener(IPAddress.Any, port);

            RegisterPacketHandlers();
        }

        public void Start() => OnStart();

        protected virtual void OnStart()
        {
            m_server.Start();

            m_running = true;

            Task.Factory.StartNew(RunReceiver, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(RunSender, TaskCreationOptions.LongRunning);
        }

        public void Stop() => OnStop();

        protected virtual void OnStop()
        {
            m_running = false;
            m_server.Stop();

            Clients.Clear();
        }

        protected virtual void RegisterPacketHandlers()
        {
            RegisterPacketHandler(typeof(AckPacket), PacketHandlerInternal.HandleAckPacketServer);
            RegisterPacketHandler(typeof(HandshakePacket), PacketHandlerInternal.HandleHandshakePacketServer);
        }

        public void RegisterPacketHandler(Type packetType, HandlePacket handler)
        {
            var packet = PacketReader.PacketFromType(packetType);

            if (m_packetHandlers[packet.Id] != null)
                Debug.WriteLine($"Handler for packet {packet.Id} overwritten", "Warning");

            PacketReader.RegisterPacket(packet.Id, packetType);

            m_packetHandlers[packet.Id] = handler;
        }

        private HandlePacket GetPacketHandler(short id)
        {
            var handler = m_packetHandlers[id];

            if (handler == null)
                throw new InvalidOperationException($"No handler registered for packet id {id}");

            return handler;
        }

        private async void RunReceiver()
        {
            Console.WriteLine("Starting receiver..");

            while (m_running)
            {
                var data = await m_server.ReceivePacketAsync();
                var client = GetOrAddClientInfo(data.Item2);

                OnHandlePacket(data.Item1, client);

                await Task.Delay(1);

                AddLostPacketsToQueue();
            }

            Console.WriteLine("Exited receiver..");
        }

        private void AddLostPacketsToQueue()
        {
            var clientsCopy = Clients.ToArray();

            foreach (var client in clientsCopy)
            {
                foreach (var packet in client.ReliablePackets)
                {
                    var now = DateTime.Now;

                    if (packet.ResendTime <= now)
                    {
                        Console.WriteLine($"Resending packet {packet} to {client.EndPoint}");

                        client.SendQueue.Enqueue(packet);
                    }
                }
            }
        }

        protected void OnHandlePacket(Packet packet, ClientInfo client)
        {
            if (packet.IsReliable)
                SendAck(packet, client);

            Console.WriteLine($"Handling packet {packet} from {client.EndPoint}");

            GetPacketHandler(packet.Id)(this, packet, client);
        }

        private void SendAck(Packet packet, ClientInfo client) => client.SendQueue.Enqueue(new AckPacket(packet));

        private async void RunSender()
        {
            Console.WriteLine("Starting sender..");

            while (m_running)
            {
                var clientsCopy = Clients.ToArray();

                foreach (var client in clientsCopy)
                {
                    var maxSendTime = DateTime.Now.AddMilliseconds(100);
                    Packet packet = null;

                    while (client.SendQueue.TryDequeue(out packet) && DateTime.Now < maxSendTime)
                    {
                        Console.WriteLine($"Sending packet {packet} to {client.EndPoint}");

                        await m_server.SendPacketAsync(packet, client.EndPoint);
                    }

                }

                await Task.Delay(1);
            }

            Console.WriteLine("Exited sender..");
        }

        protected ClientInfo OnMakeClientInfo(IPEndPoint ep) => new ClientInfo(ep);


        private ClientInfo GetOrAddClientInfo(IPEndPoint ep)
        {
            var client = Clients.Where(c => c.EndPoint.Equals(ep)).FirstOrDefault();

            if (client == null)
            {
                client = OnMakeClientInfo(ep);
                Clients.Add(client);

                Console.WriteLine($"New client connected from {client.EndPoint}");
            }

            return client;
        }

        public void SendPacket(Packet packet, ClientInfo client)
        {
            if (packet.IsReliable)
            {
                packet.Seq = client.GetNextSeqNumber();

                client.ReliablePackets.Add(packet);
            }

            Console.WriteLine($"Add packet to queue {packet}");

            client.SendQueue.Enqueue(packet);
        }

    }
}
