using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Weather
{
    public partial class Form1 : Form
    {
        private bool iswarming = false;
        const int WM_SYSCOMMAND = 0x112;
        const int SC_MINIMIZE = 0xF020;
        Form pform;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            System.Drawing.Icon fav = Icon.FromHandle(new Bitmap(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"Weather.favicon.ico"))).GetHicon());
            //System.Drawing.Icon logo = Icon.FromHandle(new Bitmap(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(@"Weather.logo.ico"))).GetHicon());
            this.Icon = fav;
            //添加右下角小图标的图片样式
            this.notifyIcon1.Icon = fav;
            //显示托盘图标3秒
            //this.notifyIcon1.ShowBalloonTip(60000, "提示", "红旭集团空调警报器", ToolTipIcon.Info);

            lblweather.Text = "loading...";

            Thread t = new Thread(new ThreadStart(DoScan));
            t.Start();

            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int x = screenWidth - this.Width - 10;
            int y = 25;
            this.Location = new Point(x, y);

            pform = this;
        }

        private void DoScan()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Interval = 30000;
            timer.Start();

            timer_Elapsed(null, null);
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Warming();
        }

        private void Warming()
        {
            try
            {
                string url = "http://bj.hongxu.cn/weather.aspx";

                string httpstr = GetPage(url);

                string msg = string.Empty;
                double curweather = 0;
                bool curwarming = false;
                if (double.TryParse(httpstr, out curweather))
                {
                    if (curweather >= 30)
                    {
                        msg = ", 可以开空调啦！";
                        curwarming = true;
                        Color c = Color.FromArgb(171, 217, 250);
                        pform.BackColor = c;
                    }
                    else
                    {
                        msg = ", 开空调有罚款风险哦！";
                        curwarming = false;
                        Color c = Color.FromArgb(255, 246, 148);
                        pform.BackColor = c;
                    }
                }
                lblweather.Text = "当前室外温度：" + curweather + "°C" + msg;
                if (Convert.ToInt32(DateTime.Now.ToString("HH")) >= 17)
                {
                    lblweather.Text = "温馨提示： 快下班了，请关空调！";
                    curwarming = false;
                    Color c = Color.FromArgb(255, 246, 148);
                    pform.BackColor = c;
                }

                if (iswarming != curwarming)
                {
                    iswarming = curwarming;
                    this.notifyIcon1.ShowBalloonTip(60000, "提示", lblweather.Text.Replace(",", "\n").Replace(" ", ""), ToolTipIcon.Info);
                }
            }
            catch { }
        }


        public static string GetPage(string url)
        {
            string content = "";
            HttpWebRequest myHttpWebRequest1 = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest1.KeepAlive = false;
            HttpWebResponse myHttpWebResponse1;

            myHttpWebResponse1 = (HttpWebResponse)myHttpWebRequest1.GetResponse();
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
            Stream streamResponse = myHttpWebResponse1.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse, utf8);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                content += outputData;
                count = streamRead.Read(readBuff, 0, 256);
            }
            myHttpWebResponse1.Close();
            return (content);

        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.WindowState = FormWindowState.Normal;
                this.Visible = true;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam.ToInt32() == SC_MINIMIZE)
                {
                    this.Visible = false;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblweather.Text = "loading...";
            Thread t = new Thread(new ThreadStart(Warming));
            t.Start();
        }
    }
}
