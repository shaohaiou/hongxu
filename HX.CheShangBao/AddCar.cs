using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HX.CheShangBao
{
    public partial class AddCar : Form
    {
        public Default defaultform = null;

        public int carid = 0;

        public AddCar()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            string url = "http://jcb.hongxu.cn/inventory/addcar.aspx";
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

            HtmlElement btnClose = htmlDoc.All["btnClose"];
            if (btnClose != null)
            {
                btnClose.Click += new HtmlElementEventHandler(btnClose_Click);
            }
            HtmlElement btnSubmit = htmlDoc.All["btnSubmit"];
            if (btnSubmit != null)
            {
                btnSubmit.Click += new HtmlElementEventHandler(btnSubmit_Click);
            }
        }

        private void btnClose_Click(object sender, HtmlElementEventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, HtmlElementEventArgs e)
        {
            if (defaultform != null) defaultform.RefreshPage();
            this.Close();
        }

        private void AddCar_Load(object sender, EventArgs e)
        {
            LoadData();

            wbcontent.Focus();
        }
    }
}
