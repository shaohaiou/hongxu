using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Car.Enum;
using Hx.Components.BasePage;
using System.Data;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Tools;

namespace Hx.JcbWeb.marketing
{
    public partial class accountmg : JcbBase
    {

        private List<JcbAccountInfo> _listaccount = null;
        private List<JcbAccountInfo> ListAccount
        {
            get
            {
                if (_listaccount == null)
                {
                    _listaccount = Jcbs.Instance.GetAccountList(true);
                }
                return _listaccount;
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
            List<JcbSiteType> data = new List<JcbSiteType>() 
            { 
                JcbSiteType.t_二手车之家
            };
            List<JcbSiteType> data1 = new List<JcbSiteType>() 
            { 
                JcbSiteType.赶集网,
                JcbSiteType.t_58同城
            };

            DataTable tblData = new DataTable();
            tblData.Columns.Add("Name");
            tblData.Columns.Add("Value");
            tblData.Columns.Add("Text");
            DataRow dr;
            data.ForEach(delegate(JcbSiteType s)
            {
                dr = tblData.NewRow();
                dr["Value"] = (int)s;
                dr["Name"] = Enum.GetName(typeof(JcbSiteType), s);
                dr["Text"] = GetDescription<JcbSiteType>(dr["Name"].ToString());

                tblData.Rows.Add(dr);
            });

            DataTable tblData1 = new DataTable();
            tblData1.Columns.Add("Name");
            tblData1.Columns.Add("Value");
            tblData1.Columns.Add("Text");
            data1.ForEach(delegate(JcbSiteType s)
            {
                dr = tblData1.NewRow();
                dr["Value"] = (int)s;
                dr["Name"] = Enum.GetName(typeof(JcbSiteType), s);
                dr["Text"] = GetDescription<JcbSiteType>(dr["Name"].ToString());

                tblData1.Rows.Add(dr);
            });

            rptData.DataSource = tblData;
            rptData.DataBind();

            rptData1.DataSource = tblData1;
            rptData1.DataBind();
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                HtmlImage imgStatus = (HtmlImage)e.Item.FindControl("imgStatus");
                HtmlInputButton btnConfig = (HtmlInputButton)e.Item.FindControl("btnConfig");
                HtmlAnchor btnLink = (HtmlAnchor)e.Item.FindControl("btnLink");

                btnLink.HRef = Jcbs.Instance.GetSiteUrl((JcbSiteType)DataConvert.SafeInt(row["Value"]));
                btnConfig.Attributes["val"] = DataConvert.SafeInt(row["Value"]).ToString();

                if (ListAccount.Exists(a => a.JcbSiteType == (JcbSiteType)DataConvert.SafeInt(row["Value"]) && a.UserID == UserID))
                {
                    imgStatus.Src = "../images/hasconfig.png";
                    btnConfig.Value = "修改";
                }
            }
        }

        private string GetDescription<T>(string value)
        {
            string result = string.Empty;

            Object[] obj = typeof(T).GetField(value).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (obj != null && obj.Length != 0)
            {
                DescriptionAttribute des = (DescriptionAttribute)obj[0];
                result = des.Description;
            }

            return result;
        }
    }
}