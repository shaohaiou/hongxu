using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hx.Car;
using Hx.Car.Entity;
using Hx.Tools;
using Hx.Car.Enum;

namespace HX.CheShangBao
{
    public partial class AutoLogin : Form
    {
        public int accountid { get; set; }

        private JcbAccountInfo _curretnaccount = null;
        private JcbAccountInfo CurretnAccount
        {
            get
            {
                if (_curretnaccount == null)
                    _curretnaccount = Jcbs.Instance.GetAccountModelRemote(accountid, true);
                return _curretnaccount;
            }
        }

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

        public AutoLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AutoLogin_Load(object sender, EventArgs e)
        {
            if (CurretnAccount != null)
            {
                string url = Jcbs.Instance.GetLoginUrl(CurretnAccount);
                wbcontent.Url = new Uri(url);
                TblSiteType.DefaultView.RowFilter = "Value='" + (int)CurretnAccount.JcbSiteType + "'";
                lblTitle.Text = TblSiteType.DefaultView[0]["Text"].ToString() + "二手车信息发布";
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

            HtmlDocument HtmlDoc = wbcontent.Document;
            switch (CurretnAccount.JcbSiteType)
            {
                case Hx.Car.Enum.JcbSiteType.t_二手车之家:
                    if (wbcontent.Url.ToString() == Jcbs.Instance.GetLoginUrl(CurretnAccount))
                    {
                        Jcbs.Instance.DoLogin(wbcontent, CurretnAccount);
                    }
                    break;
                case Hx.Car.Enum.JcbSiteType.t_58同城:
                    break;
                case Hx.Car.Enum.JcbSiteType.赶集网:
                    if (wbcontent.Url.ToString() == Jcbs.Instance.GetLoginUrl(CurretnAccount))
                    {
                        Jcbs.Instance.DoLogin(wbcontent, CurretnAccount);
                    }
                    break;
                default:
                    break;
            }
        }


    }
}
