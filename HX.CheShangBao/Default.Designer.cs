namespace HX.CheShangBao
{
    partial class Default
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.btnMin = new System.Windows.Forms.PictureBox();
            this.pnlNav3 = new System.Windows.Forms.Panel();
            this.lblNav3 = new System.Windows.Forms.Label();
            this.pnlNav2 = new System.Windows.Forms.Panel();
            this.lblNav2 = new System.Windows.Forms.Label();
            this.pnlNav1 = new System.Windows.Forms.Panel();
            this.lblNav1 = new System.Windows.Forms.Label();
            this.wbContent = new System.Windows.Forms.WebBrowser();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).BeginInit();
            this.pnlNav3.SuspendLayout();
            this.pnlNav2.SuspendLayout();
            this.pnlNav1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::HX.CheShangBao.Properties.Resources.dbg;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnMin);
            this.panel1.Controls.Add(this.pnlNav3);
            this.panel1.Controls.Add(this.pnlNav2);
            this.panel1.Controls.Add(this.pnlNav1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1000, 126);
            this.panel1.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Image = global::HX.CheShangBao.Properties.Resources.close;
            this.btnClose.Location = new System.Drawing.Point(953, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(29, 19);
            this.btnClose.TabIndex = 7;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // btnMin
            // 
            this.btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMin.BackColor = System.Drawing.Color.Transparent;
            this.btnMin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMin.Image = global::HX.CheShangBao.Properties.Resources.min1;
            this.btnMin.Location = new System.Drawing.Point(921, 0);
            this.btnMin.Name = "btnMin";
            this.btnMin.Size = new System.Drawing.Size(32, 19);
            this.btnMin.TabIndex = 6;
            this.btnMin.TabStop = false;
            this.btnMin.Click += new System.EventHandler(this.btnMin_Click);
            this.btnMin.MouseEnter += new System.EventHandler(this.btnMin_MouseEnter);
            this.btnMin.MouseLeave += new System.EventHandler(this.btnMin_MouseLeave);
            // 
            // pnlNav3
            // 
            this.pnlNav3.BackColor = System.Drawing.Color.Transparent;
            this.pnlNav3.Controls.Add(this.lblNav3);
            this.pnlNav3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlNav3.Location = new System.Drawing.Point(366, 91);
            this.pnlNav3.Name = "pnlNav3";
            this.pnlNav3.Size = new System.Drawing.Size(144, 35);
            this.pnlNav3.TabIndex = 5;
            this.pnlNav3.Click += new System.EventHandler(this.pnlNav_Click);
            // 
            // lblNav3
            // 
            this.lblNav3.AutoSize = true;
            this.lblNav3.BackColor = System.Drawing.Color.Transparent;
            this.lblNav3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblNav3.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.lblNav3.ForeColor = System.Drawing.Color.White;
            this.lblNav3.Location = new System.Drawing.Point(28, 9);
            this.lblNav3.Name = "lblNav3";
            this.lblNav3.Size = new System.Drawing.Size(89, 19);
            this.lblNav3.TabIndex = 2;
            this.lblNav3.Text = "营销管理";
            this.lblNav3.Click += new System.EventHandler(this.lblNav_Click);
            // 
            // pnlNav2
            // 
            this.pnlNav2.BackColor = System.Drawing.Color.Transparent;
            this.pnlNav2.Controls.Add(this.lblNav2);
            this.pnlNav2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlNav2.Location = new System.Drawing.Point(205, 91);
            this.pnlNav2.Name = "pnlNav2";
            this.pnlNav2.Size = new System.Drawing.Size(144, 35);
            this.pnlNav2.TabIndex = 4;
            this.pnlNav2.Click += new System.EventHandler(this.pnlNav_Click);
            // 
            // lblNav2
            // 
            this.lblNav2.AutoSize = true;
            this.lblNav2.BackColor = System.Drawing.Color.Transparent;
            this.lblNav2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblNav2.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.lblNav2.ForeColor = System.Drawing.Color.White;
            this.lblNav2.Location = new System.Drawing.Point(28, 9);
            this.lblNav2.Name = "lblNav2";
            this.lblNav2.Size = new System.Drawing.Size(89, 19);
            this.lblNav2.TabIndex = 2;
            this.lblNav2.Text = "库存管理";
            this.lblNav2.Click += new System.EventHandler(this.lblNav_Click);
            // 
            // pnlNav1
            // 
            this.pnlNav1.BackColor = System.Drawing.Color.Transparent;
            this.pnlNav1.BackgroundImage = global::HX.CheShangBao.Properties.Resources.navbg1;
            this.pnlNav1.Controls.Add(this.lblNav1);
            this.pnlNav1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnlNav1.Location = new System.Drawing.Point(42, 91);
            this.pnlNav1.Name = "pnlNav1";
            this.pnlNav1.Size = new System.Drawing.Size(144, 35);
            this.pnlNav1.TabIndex = 3;
            this.pnlNav1.Click += new System.EventHandler(this.pnlNav_Click);
            // 
            // lblNav1
            // 
            this.lblNav1.AutoSize = true;
            this.lblNav1.BackColor = System.Drawing.Color.Transparent;
            this.lblNav1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblNav1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold);
            this.lblNav1.ForeColor = System.Drawing.Color.Black;
            this.lblNav1.Location = new System.Drawing.Point(45, 9);
            this.lblNav1.Name = "lblNav1";
            this.lblNav1.Size = new System.Drawing.Size(49, 19);
            this.lblNav1.TabIndex = 2;
            this.lblNav1.Text = "首页";
            this.lblNav1.Click += new System.EventHandler(this.lblNav_Click);
            // 
            // wbContent
            // 
            this.wbContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbContent.IsWebBrowserContextMenuEnabled = false;
            this.wbContent.Location = new System.Drawing.Point(0, 126);
            this.wbContent.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbContent.Name = "wbContent";
            this.wbContent.Size = new System.Drawing.Size(1000, 387);
            this.wbContent.TabIndex = 2;
            this.wbContent.WebBrowserShortcutsEnabled = false;
            this.wbContent.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbContent_DocumentCompleted);
            // 
            // Default
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1000, 513);
            this.Controls.Add(this.wbContent);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Default";
            this.Text = "Default";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Default_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMin)).EndInit();
            this.pnlNav3.ResumeLayout(false);
            this.pnlNav3.PerformLayout();
            this.pnlNav2.ResumeLayout(false);
            this.pnlNav2.PerformLayout();
            this.pnlNav1.ResumeLayout(false);
            this.pnlNav1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblNav1;
        private System.Windows.Forms.WebBrowser wbContent;
        private System.Windows.Forms.Panel pnlNav1;
        private System.Windows.Forms.Panel pnlNav3;
        private System.Windows.Forms.Label lblNav3;
        private System.Windows.Forms.Panel pnlNav2;
        private System.Windows.Forms.Label lblNav2;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.PictureBox btnMin;
    }
}