﻿using MatthiWare.Net.Sockets;
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

namespace MatthiWare.Net.Sockets
{
    public abstract class RUdpServer
    {
        public delegate void HandlePacket(RUdpServer server, IPacket packet, IClientInfo client);

        private UdpListener m_server;
        private readonly object m_networkSync = new object();
        private bool m_running = false;

        private HandlePacket[] m_packetHandlers = new HandlePacket[byte.MaxValue];

        public ConcurrentList<IClientInfo> Clients { get; } = new ConcurrentList<IClientInfo>();

        public RUdpServer(int port)
        {
            m_server = new UdpListener(IPAddress.Any, port);

            RegisterPacketHandlers();
        }

        public void Start() => OnStart();

        protected virtual void OnStart()
        {
            m_server.Start();

            m_running = false;

            Task.Factory.StartNew(RunReceiver, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(RunSender, TaskCreationOptions.LongRunning);
        }

        public void Stop() => OnStop();

        protected virtual void OnStop() => m_server.Stop();

        protected abstract void RegisterPacketHandlers();

        public void RegisterPacketHandler(Type packetType, HandlePacket handler)
        {
            var packet = PacketReader.PacketFromType(packetType);

            if (m_packetHandlers[packet.Id] != null)
                Debug.WriteLine($"Handler for packet 0x{packet.Id.ToString("X2")} overwritten", "Warning");

            m_packetHandlers[packet.Id] = handler;
        }

        private HandlePacket GetPacketHandler(byte id)
        {
            var handler = m_packetHandlers[id];

            if (handler == null)
                throw new InvalidOperationException($"No handler registered for packet 0x{id.ToString("X2")}");

            return handler;
        }

        private async void RunReceiver()
        {
            while (m_running)
            {
                var data = await m_server.ReceivePacketAsync();
                var client = GetOrAddClientInfo(data.Item2);

                OnHandlePacket(data.Item1, client);

                await Task.Delay(1);
            }
        }

        protected void OnHandlePacket(IPacket packet, IClientInfo client) => GetPacketHandler(packet.Id)(this, packet, client);


        private async void RunSender()
        {
            while (m_running)
            {
                var clientsCopy = Clients.ToArray();

                foreach (var client in clientsCopy)
                {
                    var sendTime = DateTime.Now.AddMilliseconds(100);
                    IPacket packet = null;

                    while (client.SendQueue.TryDequeue(out packet) && DateTime.Now < sendTime)
                        await m_server.SendPacketAsync(packet, client.EndPoint);
                }

                await Task.Delay(1);
            }
        }

        protected IClientInfo OnMakeClientInfo(IPEndPoint ep) => new ClientInfo(Guid.NewGuid(), ep);


        private IClientInfo GetOrAddClientInfo(IPEndPoint ep)
        {
            var client = Clients.Where(c => c.EndPoint == ep).FirstOrDefault();

            if (client == null)
            {
                client = OnMakeClientInfo(ep);
                Clients.Add(client);
            }

            return client;



        }
    }
}