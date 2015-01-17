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
    public partial class MarketingStatus : Form
    {
        public int CarID { get; set; }

        public string Accounts { get; set; }

        public MarketingStatus()
        {
            InitializeComponent();
        }
    }
}
