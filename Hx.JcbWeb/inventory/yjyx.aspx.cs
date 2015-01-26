using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Components.BasePage;
using Hx.Tools;
using System.Text.RegularExpressions;
using System.Data;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Enumerations;
using System.Web.UI.HtmlControls;
using Hx.Car.Enum;

namespace Hx.JcbWeb.inventory
{
    public partial class yjyx : JcbBase
    {
        private int CurrentCarID
        {
            get
            {
                return GetInt("id");
            }
        }

        private List<JcbAccountInfo> listaccount = null;
        public List<JcbAccountInfo> ListAccount
        {
            get
            {
                if (listaccount == null)
                    listaccount = Jcbs.Instance.GetAccountList(true);
                return listaccount;
            }
        }

        private List<JcbMarketrecordInfo> listmarketrecords = null;
        public List<JcbMarketrecordInfo> ListMarketrecords
        {
            get
            {
                if (listmarketrecords == null)
                    listmarketrecords = Jcbs.Instance.GetMarketrecordList(true);
                return listmarketrecords;
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
#if DEBUG
            DataTable t = EnumExtensions.ToTable<JcbSiteType>();
            t.DefaultView.RowFilter = "[Text] <> '58同城' and [Text] <> '赶集网'";
            rptData.DataSource = t.DefaultView;
            rptData.DataBind();
#else
            rptData.DataSource = EnumExtensions.ToTable<JcbSiteType>();
            rptData.DataBind();
#endif
            btnSubmit.Attributes["carid"] = CurrentCarID.ToString();
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                DropDownList ddlAccounts = (DropDownList)e.Item.FindControl("ddlAccounts");
                Label lblIsMarketing = (Label)e.Item.FindControl("lblIsMarketing");
                if (ListAccount.Exists(a => a.UserID == UserID && a.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"])))
                {
                    ddlAccounts.DataSource = ListAccount.FindAll(a => a.UserID == UserID && a.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]));
                    ddlAccounts.DataTextField = "AccountName";
                    ddlAccounts.DataValueField = "ID";
                    ddlAccounts.DataBind();
                    lblIsMarketing.Text = ListMarketrecords.Exists(r => !r.IsDel && r.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]) && r.CarID == CurrentCarID) ? "已营销过" : string.Empty;
                }
            }
        }
    }
}