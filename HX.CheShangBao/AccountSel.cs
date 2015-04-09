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
    public partial class AccountSel : FormBase
    {
        public string accountids { get; set; }

        public AccountSel()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AccountSel_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(accountids))
            {
                string url = "http://jcb.hongxu.cn/inventory/accountsel.aspx?ids=" + accountids;
                wbcontent.Url = new Uri(url);
                wbcontent.Focus();
                wbcontent.ObjectForScripting = new ServerJsToClient();
            }
            else
            {
                MessageBox.Show("帐号信息错误");
                this.Close();
            }
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
                btnSubmit.Click += new HtmlElementEventHandler(btnClose_Click);
            }
        }

    }
}
