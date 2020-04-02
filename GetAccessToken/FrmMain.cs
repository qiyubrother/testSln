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
namespace GetAccessToken
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnGetAccessToken_Click(object sender, EventArgs e)
        {
            //txtUrl.Text = "192.168.10.168";
            //txtPort.Text = "8003";
            //txtUserName.Text = "13070113116";
            //txtPassword.Text = "123456";

            try
            {
                Task.Run(() =>
                {
                    var t = TokenHelper.GetToken(txtUrl.Text, txtPort.Text, txtUserName.Text, TokenHelper.Md5(txtPassword.Text), 3);

                    Invoke(new Action(() =>
                    {
                        txtAccessToken.Text = t;
                    }));
                });
             }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtAccessToken.Clear();
            }

        }
    }
}
