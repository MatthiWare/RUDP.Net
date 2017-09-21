using MatthiWare.Net.Sockets.Base;
using MatthiWare.Net.Sockets.Packets;
using Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public delegate void PacketHandler(UdpListener server, IPacket packet, IPEndPoint ep);

    public partial class ServerForm : Form
    {
        private bool running = false;
        private UdpListener server;


        private PacketHandler[] handlers = new PacketHandler[byte.MaxValue];


        public ServerForm()
        {
            InitializeComponent();

            server = new UdpListener(IPAddress.Any, 43594);
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            RegisterPacketHandlers(typeof(LoginPacket), PacketHandlers.LoginPacketHandler);
        }

        public void RegisterPacketHandlers(Type packetType, PacketHandler handler)
        {
            var packet = PacketReader.PacketFromType(packetType);
            handlers[packet.Id] = handler;
            PacketReader.RegisterPacket(packet.Id, packetType);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            var txtstartstop = !running ? "Starting" : "Stopping";
            Log($"{txtstartstop} server..");

            StartStopServer();

            txtstartstop = running ? "started" : "stopped";
            Log($"Server {txtstartstop}..");
        }

        private void Log(string msg) => txtLog.Text += $"{msg}\r\n";

        private async Task StartStopServer()
        {
            if (!running)
                server.Start();
            else
                server.Stop();

            running = !running;

            UpdateServerStatus();

            if (running)
                await Receive();
        }

        private void UpdateServerStatus()
        {
            txtServerStatus.Text = running ? "Online" : "Offline";
            txtServerStatus.ForeColor = running ? Color.DarkGreen : Color.DarkRed;
        }

        private async Task Receive() => ProcessData(await server.ReceivePacketAsync());

        private void ProcessData(Tuple<IPacket, IPEndPoint> data)
        {
            var handler = handlers[data.Item1.Id];

            if (handler == null)
                throw new InvalidOperationException($"No packet handler set for 0x{data.Item1.Id.ToString("X2")} ({data.Item1.GetType().Name})");

            handler(server, data.Item1, data.Item2);
        }

    }
}
