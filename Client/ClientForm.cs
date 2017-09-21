using MatthiWare.Net.Sockets.Base;
using Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        private UdpClient client;

        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            using (var login = new LoginForm())
            {
                while (login.ShowDialog(this) != DialogResult.OK) ;

                InitializeClient(login.Username, login.ServerIP);
            }
        }

        private void InitializeClient(string username, string serverIP)
        {
            client = new UdpClient();
            client.Connect(serverIP, 43594);
            client.SendPacket(new LoginPacket { Username = username });
        }
    }
}
