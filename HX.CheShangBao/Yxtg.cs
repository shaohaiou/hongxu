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
    public partial class Yxtg : FormBase
    {
        public Default defaultform = null;
        public int carid = 0;

        public Yxtg()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            string url = "http://jcb.hongxu.cn/inventory/yxtg.aspx";
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

            HtmlElement _btnClose = htmlDoc.All["btnClose"];
            if (_btnClose != null)
            {
                _btnClose.Click += new HtmlElementEventHandler(btnClose_Click);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Yxtg_Load(object sender, EventArgs e)
        {
            LoadData();

            wbcontent.Focus();
            wbcontent.ObjectForScripting = new ServerJsToClient();
        }
    }
}
