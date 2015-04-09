using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HX.CheShangBao
{
    public partial class Message : Form
    {
        public string Msg
        {
            set
            {

                lblMsg.Text = value;
            }
        }

        public Message()
        {
            InitializeComponent();
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
