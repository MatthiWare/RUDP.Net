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
        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            var login = new LoginForm();
            while (login.ShowDialog(this) != DialogResult.OK) ;

            InitializeClient(login.Username, login.ServerIP);
        }

        private void InitializeClient(string username, string serverIP)
        {
            
        }
    }
}
