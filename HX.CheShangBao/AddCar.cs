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
        public AddCar()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            wbcontent.Url = new Uri("http://jcb.hongxu.cn/inventory/addcar.aspx");
        }

        private void wbcontent_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument htmlDoc = wbcontent.Document;

            HtmlElement btnClose = htmlDoc.All["btnClose"];
            if (btnClose != null)
            {
                btnClose.Click += new HtmlElementEventHandler(btnClose_Click);
            }
        }

        private void btnClose_Click(object sender, HtmlElementEventArgs e)
        {
            this.Close();
        }
    }
}
