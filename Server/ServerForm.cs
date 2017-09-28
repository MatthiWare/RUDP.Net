using MatthiWare.Net.Sockets;
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
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
            
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
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

    }
}
