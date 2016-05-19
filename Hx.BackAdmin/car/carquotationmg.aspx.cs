using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Car;
using Hx.Car.Entity;
using Hx.Car.Query;
using Hx.Tools;
using Hx.Car.Enum;

namespace Hx.BackAdmin.car
{
    public partial class carquotationmg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.总经理) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0 
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.财务出纳) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private List<CorporationInfo> corporationlist = null;
        private List<CorporationInfo> CorporationList
        {
            get
            {
                if (corporationlist == null)
                    corporationlist = Corporations.Instance.GetList(true);
                return corporationlist;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControlor();
                LoadData();
            }
        }

        private void BindControlor()
        {
            hyName.Text = Admin.Name;
            ddlCarQuotationType.DataSource = EnumExtensions.ToTable<CarQuotationType>();
            ddlCarQuotationType.DataTextField = "Name";
            ddlCarQuotationType.DataValueField = "Value";
            ddlCarQuotationType.DataBind();
            ddlCarQuotationType.Items.Insert(0, new ListItem("-提交类型-", "-1"));

            ddlCorporationFilter.DataSource = CorporationList;
            ddlCorporationFilter.DataTextField = "Name";
            ddlCorporationFilter.DataValueField = "ID";
            ddlCorporationFilter.DataBind();
            ddlCorporationFilter.Items.Insert(0, new ListItem("-所属公司-", "-1"));

            if (!string.IsNullOrEmpty(GetString("cqtype")))
                SetSelectedByValue(ddlCarQuotationType, GetString("cqtype"));

            if (!string.IsNullOrEmpty(GetString("corp")))
                SetSelectedByValue(ddlCorporationFilter, GetString("corp"));

            if (!Admin.Administrator)
            {
                SetSelectedByValue(ddlCorporationFilter,Admin.Corporation);
                ddlCorporationFilter.Enabled = false;
            }

            if (!string.IsNullOrEmpty(GetString("creator")))
                txtCreator.Text = GetString("creator");
            if (!string.IsNullOrEmpty(GetString("datebegin")))
                txtDate.Text = GetString("datebegin");
            if (!string.IsNullOrEmpty(GetString("dateend")))
                txtDateEnd.Text = GetString("dateend");
            if (!string.IsNullOrEmpty(GetString("cusname")))
                txtCustomerName.Text = GetString("cusname");
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int pagesize = GetInt("pagesize", 10);
            int total = 0;
            CarQuotationQuery query = new CarQuotationQuery();
            if (ddlCarQuotationType.SelectedIndex > 0)
                query.CarQuotationType = (CarQuotationType)DataConvert.SafeInt(ddlCarQuotationType.SelectedValue);
            if (ddlCorporationFilter.SelectedIndex > 0)
                query.CorporationID = ddlCorporationFilter.SelectedValue;
            if (!string.IsNullOrEmpty(txtCreator.Text.Trim()))
                query.Creator = txtCreator.Text.Trim();
            if (!string.IsNullOrEmpty(txtDate.Text))
            {
                DateTime datebegin = DateTime.Now;
                if (DateTime.TryParse(txtDate.Text, out datebegin))
                {
                    query.DateBegin = datebegin;
                }
            }
            if (!string.IsNullOrEmpty(txtDateEnd.Text))
            {
                DateTime dateend = DateTime.Now;
                if (DateTime.TryParse(txtDateEnd.Text, out dateend))
                {
                    query.DateEnd = dateend.AddDays(1);
                }
            }
            if (!string.IsNullOrEmpty(txtCustomerName.Text))
                query.CustomerName = txtCustomerName.Text.Trim();
            query.OrderBy = " ID DESC";

            List<CarQuotationInfo> list = CarQuotations.Instance.GetList(pageindex, pagesize, query, ref total);
            rptCarQuotation.DataSource = list;
            rptCarQuotation.DataBind();
            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("carquotationmg.aspx?corp=" + ddlCorporationFilter.SelectedValue + "&cqtype=" + ddlCarQuotationType.SelectedValue + "&creator=" + txtCreator.Text + "&datebegin=" + txtDate.Text + "&dateend=" + txtDateEnd.Text + "&cusname=" + txtCustomerName.Text);
        }

        protected string GetCorpName(object corpid)
        {
            string result = string.Empty;

            int id = DataConvert.SafeInt(corpid);
            if (id > 0)
            {
                CorporationInfo corp = Corporations.Instance.GetModel(id,true);
                if (corp != null)
                    result = corp.Name;
            }

            return result;
        }
    }
}