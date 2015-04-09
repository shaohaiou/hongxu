using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Hx.Tools.Web;
using System.Net;

namespace HX.CheShangBao
{
    public partial class FrmTest : Form
    {
        public FrmTest()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private void FrmTest_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF012, 0);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "https://passport.ganji.com/login.php?callback=jQuery18209229161911644042_1427337487513&username=shaohaiou1&password=w958137&checkCode=&setcookie=14&second=&parentfunc=&redirect_in_iframe=&next=%2F&__hash__=yY1UQsmoCkm3IshHnCKzAXqt7sY%2FC2x8%2BU%2Fn7KPe9ouf%2FP8eBiqBLtYwEZYwB0%2Bb&_=1427337496907";
            string html = Http.GetPage(url);
            if (html.IndexOf("\"status\":1") > 0)
            {
                webBrowser1.Navigate(url);
                webBrowser1.Stop();
                url = "http://wenzhou.ganji.com/ershouche/";
                webBrowser1.Navigate(url);
            }
            else
                MessageBox.Show("登录失败");
        }


    }
}
