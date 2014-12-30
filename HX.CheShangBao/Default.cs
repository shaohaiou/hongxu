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
    public partial class Default : Form
    {
        private static Dictionary<string,string> urls = new Dictionary<string,string>();
        public Default()
        {
            InitializeComponent();
            LoadForm();
            Init();
        }

        private void LoadForm()
        {
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
        }

        private void Init()
        {
            urls.Add("库存管理-销售中车源", "http://jcb.hongxu.cn/inventory/inventorymg.aspx");
        }

        private void pnlNav_Click(object sender, EventArgs e)
        {
            Panel pnlCur = (Panel)sender;
            if (pnlCur.BackgroundImage != null)
                return;

            SetNavStyle(pnlCur);

            if (pnlCur.Name == "pnlNav2")
                wbContent.Url = new Uri(urls["库存管理-销售中车源"]);
        }

        private void SetNavStyle(Panel pnlCur)
        {
            if (pnlNav1.BackgroundImage != null)
            {
                pnlNav1.BackgroundImage = null;
                lblNav1.ForeColor = Color.White;
            }
            if (pnlNav2.BackgroundImage != null)
            {
                pnlNav2.BackgroundImage = null;
                lblNav2.ForeColor = Color.White;
            }
            if (pnlNav3.BackgroundImage != null)
            {
                pnlNav3.BackgroundImage = null;
                lblNav3.ForeColor = Color.White;
            }

            pnlCur.BackgroundImage = global::HX.CheShangBao.Properties.Resources.navbg1;
            pnlCur.Controls[0].ForeColor = Color.Black;
        }

        private void lblNav_Click(object sender, EventArgs e)
        {
            Label lblCur = (Label)sender;
            pnlNav_Click(lblCur.Parent, null);
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.Image = global::HX.CheShangBao.Properties.Resources.close2;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.Image = global::HX.CheShangBao.Properties.Resources.close;
        }

        private void btnMin_MouseEnter(object sender, EventArgs e)
        {
            btnMin.Image = global::HX.CheShangBao.Properties.Resources.min2;
        }

        private void btnMin_MouseLeave(object sender, EventArgs e)
        {
            btnMin.Image = global::HX.CheShangBao.Properties.Resources.min1;
        }

        private void wbContent_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument htmlDoc = wbContent.Document;

            if (wbContent.Url.AbsoluteUri == urls["库存管理-销售中车源"])
            {
                HtmlElement btnAddCar = htmlDoc.All["btnAddCar"];
                if (btnAddCar != null)
                {
                    btnAddCar.Click += new HtmlElementEventHandler(btnAddCar_Click);
                }
            }
        }

        private void btnAddCar_Click(object sender, HtmlElementEventArgs e)
        {
            AddCar formAddCar = new AddCar();
            formAddCar.ShowDialog();
        }
    }
}
