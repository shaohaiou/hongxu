using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car;
using Hx.Car.Entity;

namespace Hx.JcbWeb.inventory
{
    public partial class marketingstatus : JcbBase
    {
        private JcbCarInfo currentcarinfo = null;
        private JcbCarInfo CurrentCarInfo
        {
            get
            {
                if (currentcarinfo == null)
                {
                    int id = GetInt("id");
                    List<JcbCarInfo> carlist = JcbCars.Instance.GetList(true);
                    currentcarinfo = carlist.Find(c=>c.ID == id);
                }
                return currentcarinfo;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            string[] accounts = GetString("accounts").Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
            List<JcbAccountInfo> listAccount = Jcbs.Instance.GetAccountList(true);
            rptData.DataSource = listAccount.FindAll(a => accounts.Contains(a.ID.ToString()));
            rptData.DataBind();
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblTTL = (Label)e.Item.FindControl("lblTTL");
                if (lblTTL != null && CurrentCarInfo != null)
                {
                    lblTTL.Text = "营销-" + CurrentCarInfo.cCxmc;
                }
            }
        }
    }
}