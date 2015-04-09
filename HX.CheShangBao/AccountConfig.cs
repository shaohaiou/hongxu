using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Hx.Tools;
using Hx.Car.Enum;

namespace HX.CheShangBao
{
    public partial class AccountConfig : FormBase
    {
        public Default defaultform = null;
        public int sitetypeval { get; set; }

        private DataTable _tblsitetype = null;
        private DataTable TblSiteType
        {
            get
            {
                if (_tblsitetype == null)
                    _tblsitetype = EnumExtensions.ToTable<JcbSiteType>();
                return _tblsitetype;
            }

        }

        public AccountConfig()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            if (defaultform != null) defaultform.RefreshPage();
            this.Close();
        }

        private void AccountConfig_Load(object sender, EventArgs e)
        {
            string url = "http://jcb.hongxu.cn/marketing/accountconfig.aspx";
            if (sitetypeval >= 0)
                url += "?sitetypeval=" + sitetypeval;
            wbcontent.Url = new Uri(url);

            TblSiteType.DefaultView.RowFilter = "Value='" + sitetypeval + "'";
            lblTitle.Text = TblSiteType.DefaultView[0]["Text"].ToString() + " 帐号管理";
        }
    }
}
