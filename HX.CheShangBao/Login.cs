using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace HX.CheShangBao
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        #region 圆角

        public void SetWindowRegion()
        {

            System.Drawing.Drawing2D.GraphicsPath FormPath;

            FormPath = new System.Drawing.Drawing2D.GraphicsPath();

            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);//this.Left-10,this.Top-10,this.Width-10,this.Height-10);

            FormPath = GetRoundedRectPath(rect, 20);

            this.Region = new Region(FormPath);

        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            int diameter = radius;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();
            //   左上角   
            path.AddArc(arcRect, 180, 90);
            //   右上角   
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);
            //   右下角   
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);
            //   左下角   
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnResize(System.EventArgs e)
        {
            this.Region = null;
            SetWindowRegion();
        }

        #endregion

        #region 拖动

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF012, 0);
        } 

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.Image = global::HX.CheShangBao.Properties.Resources.close2;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.Image = global::HX.CheShangBao.Properties.Resources.close;
        }

        private void btnMin_MouseEnter(object sender, EventArgs e)
        {
            btnMin.Image = global::HX.CheShangBao.Properties.Resources.min2;
        }

        private void btnMin_MouseLeave(object sender, EventArgs e)
        {
            btnMin.Image = global::HX.CheShangBao.Properties.Resources.min1;
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void lnkReg_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://jcb.hongxu.cn/reg.aspx");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Default formDefault = new Default();
            formDefault.Show();
        }


        
    }
}
