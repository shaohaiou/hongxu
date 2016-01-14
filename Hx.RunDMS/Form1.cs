using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Hx.RunDMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string configfilepath = Application.StartupPath + "\\pb2.ini";
            string appPath = Application.StartupPath + "\\ds_app.exe";
            if (!File.Exists(configfilepath))
                MessageBox.Show("没有配置文件pb2.ini");
            else if (!File.Exists(appPath))
                MessageBox.Show("找不到ds_app.exe文件");
            else
            {
                File.Copy(Application.StartupPath + "\\pb2.ini", Application.StartupPath + "\\pb.ini", true);

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = new System.Diagnostics.ProcessStartInfo(appPath);
                p.EnableRaisingEvents = true;
                p.Exited += new EventHandler(p_Exited);
                p.Start();
            }
        }

        protected void p_Exited(object o, EventArgs e)
        {
            Application.Exit();
        }

    }

    
}
