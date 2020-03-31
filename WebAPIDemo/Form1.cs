using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using qiyubrother;

namespace WebAPIDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SocketHttpHelper.SocketHttpPost("localhost", "5000", "/api/values", "value=100", out string status, out string data);

            Text = status;
        }
    }
}
