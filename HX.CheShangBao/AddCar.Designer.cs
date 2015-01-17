namespace HX.CheShangBao
{
    partial class AddCar
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
            this.SuspendLayout();
            // 
            // wbcontent
            // 
            this.wbcontent.AllowWebBrowserDrop = false;
            this.wbcontent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbcontent.IsWebBrowserContextMenuEnabled = false;
            this.wbcontent.Location = new System.Drawing.Point(0, 0);
            this.wbcontent.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbcontent.Name = "wbcontent";
            this.wbcontent.ScrollBarsEnabled = false;
            this.wbcontent.Size = new System.Drawing.Size(789, 302);
            this.wbcontent.TabIndex = 0;
            this.wbcontent.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.wbcontent_DocumentCompleted);
            // 
            // AddCar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 302);
            this.Controls.Add(this.wbcontent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddCar";
            this.Text = "AddCar";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.AddCar_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbcontent;
    }
}