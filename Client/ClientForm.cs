using MatthiWare.Net.Sockets;
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
        private RUdpClient client;

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
            client = new TestClient(serverIP, 43594, this);
            client.Start();
            client.SendPacket(new LoginPacket { Username = username });
        }

        private void txtMsg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                btnSend.PerformClick();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SuspendLayout();

            client.SendPacket(new ChatPacket { Message = txtMsg.Text });

            //AddText(text);
            txtMsg.Clear();

            ResumeLayout();
        }

        public void AddText(string msg)
        {
            if (txtChat.InvokeRequired)
            {
                txtChat.Invoke(new Action<string>(AddText), msg);
                return;
            }

            txtChat.Text += $"{msg}\n";
        }
    }
}
