namespace HX.CheShangBao
{
    partial class MarketingStatus
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
            this.wbcontent = new System.Windows.Forms.WebBrowser();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.wbJob1 = new System.Windows.Forms.WebBrowser();
            this.wbJob2 = new System.Windows.Forms.WebBrowser();
            this.wbJob3 = new System.Windows.Forms.WebBrowser();
            this.wbJob4 = new System.Windows.Forms.WebBrowser();
            this.wbJob5 = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // wbcontent
            // 
            this.wbcontent.Location = new System.Drawing.Point(1, 40);
            this.wbcontent.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbcontent.Name = "wbcontent";
            this.wbcontent.ScrollBarsEnabled = false;
            this.wbcontent.Size = new System.Drawing.Size(698, 359);
            this.wbcontent.TabIndex = 0;
            this.wbcontent.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbcontent_DocumentCompleted);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImage = global::HX.CheShangBao.Properties.Resources.hclose;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(673, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(15, 15);
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "一键营销进度";
            // 
            // wbJob1
            // 
            this.wbJob1.Location = new System.Drawing.Point(703, 2);
            this.wbJob1.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbJob1.Name = "wbJob1";
            this.wbJob1.ScriptErrorsSuppressed = true;
            this.wbJob1.Size = new System.Drawing.Size(299, 85);
            this.wbJob1.TabIndex = 3;
            // 
            // wbJob2
            // 
            this.wbJob2.Location = new System.Drawing.Point(704, 260);
            this.wbJob2.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbJob2.Name = "wbJob2";
            this.wbJob2.ScriptErrorsSuppressed = true;
            this.wbJob2.Size = new System.Drawing.Size(299, 27);
            this.wbJob2.TabIndex = 3;
            // 
            // wbJob3
            // 
            this.wbJob3.Location = new System.Drawing.Point(704, 293);
            this.wbJob3.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbJob3.Name = "wbJob3";
            this.wbJob3.ScriptErrorsSuppressed = true;
            this.wbJob3.Size = new System.Drawing.Size(299, 26);
            this.wbJob3.TabIndex = 3;
            // 
            // wbJob4
            // 
            this.wbJob4.Location = new System.Drawing.Point(704, 325);
            this.wbJob4.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbJob4.Name = "wbJob4";
            this.wbJob4.ScriptErrorsSuppressed = true;
            this.wbJob4.Size = new System.Drawing.Size(299, 27);
            this.wbJob4.TabIndex = 3;
            // 
            // wbJob5
            // 
            this.wbJob5.Location = new System.Drawing.Point(703, 354);
            this.wbJob5.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbJob5.Name = "wbJob5";
            this.wbJob5.ScriptErrorsSuppressed = true;
            this.wbJob5.Size = new System.Drawing.Size(299, 45);
            this.wbJob5.TabIndex = 3;
            // 
            // MarketingStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HX.CheShangBao.Properties.Resources.dbg;
            this.ClientSize = new System.Drawing.Size(1000, 400);
            this.Controls.Add(this.wbJob5);
            this.Controls.Add(this.wbJob4);
            this.Controls.Add(this.wbJob3);
            this.Controls.Add(this.wbJob2);
            this.Controls.Add(this.wbJob1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.wbcontent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MarketingStatus";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MarketingStatus";
            this.Load += new System.EventHandler(this.MarketingStatus_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MarketingStatus_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbcontent;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.WebBrowser wbJob1;
        private System.Windows.Forms.WebBrowser wbJob2;
        private System.Windows.Forms.WebBrowser wbJob3;
        private System.Windows.Forms.WebBrowser wbJob4;
        private System.Windows.Forms.WebBrowser wbJob5;
    }
}