using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HX.CheShangBao
{
    public partial class Yjyx : FormBase
    {
        public Default defaultform = null;
        public int carid = 0;

        public Yjyx()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            string url = "http://jcb.hongxu.cn/inventory/yjyx.aspx";
            if (carid > 0)
                url += "?id=" + carid;
            wbcontent.Url = new Uri(url);
        }

        private void wbcontent_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString() != wbcontent.Url.ToString())
                return;
            if (wbcontent.ReadyState != WebBrowserReadyState.Complete)
                return;

            HtmlDocument htmlDoc = wbcontent.Document;

            HtmlElement btnSubmit = htmlDoc.All["btnSubmit"];
            if (btnSubmit != null)
            {
                btnSubmit.Click += new HtmlElementEventHandler(btnSubmit_Click);
            }
        }

        private void btnSubmit_Click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement btnSubmit = (HtmlElement)sender;
            int carid = int.Parse(btnSubmit.GetAttribute("carid"));
            string accounts = btnSubmit.GetAttribute("accounts");
            if (carid > 0 && !string.IsNullOrEmpty(accounts))
            {
                MarketingStatus frmMarketing = new MarketingStatus();
                frmMarketing.CarID = carid;
                frmMarketing.Accounts = accounts;
                frmMarketing.Show();
                this.Close();
            }
        }

        private void Yjyx_Load(object sender, EventArgs e)
        {
            LoadData();

            wbcontent.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
