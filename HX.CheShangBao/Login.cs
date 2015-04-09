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
using Hx.Car;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Hx.Tools;

namespace HX.CheShangBao
{
    public partial class Login : FormBase
    {
        private string arFilePath = AppDomain.CurrentDomain.BaseDirectory + "/data/ar.db";
        private AccountRemember ar = new AccountRemember();

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

        private void SaveAR()
        {
            try
            {
                if (ar != null)
                {
                    if (!Directory.Exists(new FileInfo(arFilePath).Directory.FullName))
                        Directory.CreateDirectory(new FileInfo(arFilePath).Directory.FullName);
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(arFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, ar);
                    stream.Close();
                }
                else
                {
                    if (File.Exists(arFilePath))
                        File.Delete(arFilePath);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
            }
        }

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
            DoLogin();
        }

        private void DoLogin()
        {
            string error = string.Empty;
            if (string.IsNullOrEmpty(txtUserName.Text))
                error = "请输入登录名";
            else if (string.IsNullOrEmpty(txtPassword.Text))
                error = "请输入密码";
            else
            {
                Global.CurrentUser = Jcbs.Instance.GetJcbUserRemote(txtUserName.Text, txtPassword.Text);
                if (Global.CurrentUser != null)
                {
                    this.Visible = false;
                    Default formDefault = new Default();
                    formDefault.Show();

                    if (ar == null)
                        ar = new AccountRemember();
                    ar.UserName = txtUserName.Text;
                    ar.Password = txtPassword.Text;
                    ar.IsAutoLogin = cbxAutoLogin.Checked;
                    ar.IsRemember = cbxRemember.Checked;
                    SaveAR();
                }
                else
                {
                    error = "登录失败，用户名或密码错误";
                }
            }
            if (!string.IsNullOrEmpty(error))
            {
                Message frmMsg = new Message();
                frmMsg.Msg = error;
                frmMsg.ShowDialog();
            }
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(arFilePath))
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(arFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    ar = (AccountRemember)formatter.Deserialize(stream);
                    stream.Close();
                }
            }
            catch { }
            if (ar.IsRemember)
            {
                cbxRemember.Checked = ar.IsRemember;
                cbxAutoLogin.Checked = ar.IsAutoLogin;
                txtUserName.Text = ar.UserName;
                txtPassword.Text = ar.Password;
                if (ar.IsAutoLogin)
                {
                    DoLogin();
                }
            }
        }
    }

    [Serializable]
    public class AccountRemember
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsRemember { get; set; }

        public bool IsAutoLogin { get; set; }
    }
}
