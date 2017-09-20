using MatthiWare.Net.Sockets;
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
    public delegate void PacketHandler(RUdpListener server, IPacket packet, IPEndPoint ep);

    public partial class ServerForm : Form
    {
        private bool running = false;
        private RUdpListener server;


        private PacketHandler[] handlers = new PacketHandler[byte.MaxValue];


        public ServerForm()
        {
            InitializeComponent();

            server = new RUdpListener(IPAddress.Any, 43594);
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

        private void ProcessData(Tuple<IPacket, IPEndPoint> data) => handlers[data.Item1.Id](server, data.Item1, data.Item2);

    }
}
