using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GBDMSAuto
{
    public partial class Form1 : Form
    {
        private static string fncompany = AppDomain.CurrentDomain.BaseDirectory + "/company.txt";
        private static string fndmsclient = AppDomain.CurrentDomain.BaseDirectory + "/DMSClient.exe";
        private Dictionary<string, string> dicCompany = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(fncompany))
            { 
                
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dicCompany.Count > 0)
            { 
                
            }
        }
    }
}
