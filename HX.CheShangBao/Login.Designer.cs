using System.Runtime.InteropServices;
using System;
namespace HX.CheShangBao
{
    partial class Login
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnMin = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.picLoglog = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxRemember = new System.Windows.Forms.CheckBox();
            this.cbxAutoLogin = new System.Windows.Forms.CheckBox();
            this.btnLogin = new System.Windows.Forms.PictureBox();
            this.btnLogout = new System.Windows.Forms.PictureBox();
            this.lnkReg = new System.Windows.Forms.Label();
            this.lnkJcb = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoglog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMin
            // 
            this.btnMin.BackColor = System.Drawing.Color.Transparent;
            this.btnMin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMin.Image = global::HX.CheShangBao.Properties.Resources.min1;
            this.btnMin.Location = new System.Drawing.Point(720, 0);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(32, 19);
            this.btnMin.TabIndex = 4;
            this.btnMin.TabStop = false;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            this.btnMin.MouseEnter += new System.EventHandler(this.btnMin_MouseEnter);
            this.btnMin.MouseLeave += new System.EventHandler(this.btnMin_MouseLeave);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = global::HX.CheShangBao.Properties.Resources.close;
            this.btnClose.Location = new System.Drawing.Point(752, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(29, 19);
            this.btnClose.TabIndex = 5;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(25, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 36);
            this.label1.TabIndex = 6;
            this.label1.Text = "集车宝";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(129, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "－红旭二手车";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(576, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(189, 14);
            this.label3.TabIndex = 8;
            this.label3.Text = "客服电话：400 - 808 - 8888";
            // 
            // picLoglog
            // 
            this.picLoglog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.picLoglog.BackColor = System.Drawing.Color.Transparent;
            this.picLoglog.Image = global::HX.CheShangBao.Properties.Resources.loglog1;
            this.picLoglog.Location = new System.Drawing.Point(323, 144);
            this.picLoglog.Name = "picLoglog";
            this.picLoglog.Size = new System.Drawing.Size(142, 83);
            this.picLoglog.TabIndex = 9;
            this.picLoglog.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HX.CheShangBao.Properties.Resources.txtboder;
            this.pictureBox1.Location = new System.Drawing.Point(315, 257);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(240, 40);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // txtUserName
            // 
            this.txtUserName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserName.Font = new System.Drawing.Font("宋体", 12F);
            this.txtUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtUserName.Location = new System.Drawing.Point(324, 268);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(222, 19);
            this.txtUserName.TabIndex = 11;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::HX.CheShangBao.Properties.Resources.txtboder;
            this.pictureBox2.Location = new System.Drawing.Point(315, 310);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(240, 40);
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // txtPassword
            // 
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Font = new System.Drawing.Font("宋体", 12F);
            this.txtPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.txtPassword.Location = new System.Drawing.Point(323, 321);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(218, 19);
            this.txtPassword.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.label4.Location = new System.Drawing.Point(237, 268);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 16);
            this.label4.TabIndex = 14;
            this.label4.Text = "用户名：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("宋体", 12F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.label5.Location = new System.Drawing.Point(237, 321);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 15;
            this.label5.Text = "密  码：";
            // 
            // cbxRemember
            // 
            this.cbxRemember.AutoSize = true;
            this.cbxRemember.BackColor = System.Drawing.Color.Transparent;
            this.cbxRemember.Font = new System.Drawing.Font("宋体", 10F);
            this.cbxRemember.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.cbxRemember.Location = new System.Drawing.Point(315, 361);
            this.cbxRemember.Name = "cbxRemember";
            this.cbxRemember.Size = new System.Drawing.Size(82, 18);
            this.cbxRemember.TabIndex = 16;
            this.cbxRemember.Text = "记住密码";
            this.cbxRemember.UseVisualStyleBackColor = false;
            // 
            // cbxAutoLogin
            // 
            this.cbxAutoLogin.AutoSize = true;
            this.cbxAutoLogin.BackColor = System.Drawing.Color.Transparent;
            this.cbxAutoLogin.Font = new System.Drawing.Font("宋体", 10F);
            this.cbxAutoLogin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.cbxAutoLogin.Location = new System.Drawing.Point(433, 361);
            this.cbxAutoLogin.Name = "cbxAutoLogin";
            this.cbxAutoLogin.Size = new System.Drawing.Size(82, 18);
            this.cbxAutoLogin.TabIndex = 16;
            this.cbxAutoLogin.Text = "自动登录";
            this.cbxAutoLogin.UseVisualStyleBackColor = false;
            // 
            // btnLogin
            // 
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.Image = global::HX.CheShangBao.Properties.Resources.btnlogin;
            this.btnLogin.Location = new System.Drawing.Point(315, 403);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(115, 41);
            this.btnLogin.TabIndex = 17;
            this.btnLogin.TabStop = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.Image = global::HX.CheShangBao.Properties.Resources.btnlogout;
            this.btnLogout.Location = new System.Drawing.Point(440, 403);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(115, 41);
            this.btnLogout.TabIndex = 18;
            this.btnLogout.TabStop = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lnkReg
            // 
            this.lnkReg.AutoSize = true;
            this.lnkReg.BackColor = System.Drawing.Color.Transparent;
            this.lnkReg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkReg.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lnkReg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lnkReg.Location = new System.Drawing.Point(343, 483);
            this.lnkReg.Name = "lnkReg";
            this.lnkReg.Size = new System.Drawing.Size(63, 14);
            this.lnkReg.TabIndex = 19;
            this.lnkReg.Text = "申请帐号";
            this.lnkReg.Click += new System.EventHandler(this.lnkReg_Click);
            // 
            // lnkJcb
            // 
            this.lnkJcb.AutoSize = true;
            this.lnkJcb.BackColor = System.Drawing.Color.Transparent;
            this.lnkJcb.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkJcb.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lnkJcb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lnkJcb.Location = new System.Drawing.Point(419, 483);
            this.lnkJcb.Name = "lnkJcb";
            this.lnkJcb.Size = new System.Drawing.Size(105, 14);
            this.lnkJcb.TabIndex = 19;
            this.lnkJcb.Text = "进入红旭集车宝";
            // 
            // Login
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HX.CheShangBao.Properties.Resources.logback1;
            this.ClientSize = new System.Drawing.Size(800, 599);
            this.Controls.Add(this.lnkJcb);
            this.Controls.Add(this.lnkReg);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.cbxAutoLogin);
            this.Controls.Add(this.cbxRemember);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.picLoglog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "集车宝";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Login_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoglog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLogout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox btnMin;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox picLoglog;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbxRemember;
        private System.Windows.Forms.CheckBox cbxAutoLogin;
        private System.Windows.Forms.PictureBox btnLogin;
        private System.Windows.Forms.PictureBox btnLogout;
        private System.Windows.Forms.Label lnkReg;
        private System.Windows.Forms.Label lnkJcb;



    }
}

