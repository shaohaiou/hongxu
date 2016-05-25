using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools;
using Hx.Components.Entity;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Components.Web;

namespace Hx.BackAdmin.global
{
    public partial class bankmg : AdminBase
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
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }
        private List<CorporationInfo> corplist = null;
        private List<CorporationInfo> Corplist
        {
            get
            {
                if (corplist == null)
                    corplist = Corporations.Instance.GetList(true);
                return corplist;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
                LoadData();
            }
        }

        private void BindControler()
        {
            ddlCorporationFilter.DataSource = Corplist;
            ddlCorporationFilter.DataTextField = "Name";
            ddlCorporationFilter.DataValueField = "ID";
            ddlCorporationFilter.DataBind();
            ddlCorporationFilter.Items.Insert(0, new ListItem("系统通用", "0"));
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int pagesize = GetInt("pagesize", search_fy.PageSize);
            int total = 0;

            SetSelectedByValue(ddlCorporationFilter, GetInt("corpid").ToString());
            if (!Admin.Administrator)
            {
                SetSelectedByValue(ddlCorporationFilter, Admin.Corporation);
                ddlCorporationFilter.Enabled = false;
            }

            List<BankInfo> list = Banks.Instance.GetList(DataConvert.SafeInt(ddlCorporationFilter.SelectedValue), true);
            list = list.FindAll(l => l.CorporationID == DataConvert.SafeInt(ddlCorporationFilter.SelectedValue));
            total = list.Count();
            list = list.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<BankInfo>();

            rptData.DataSource = list;
            rptData.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                BankInfo info = (BankInfo)e.Item.DataItem;

                System.Web.UI.WebControls.DropDownList ddlCorporation = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCorporation");
                if (ddlCorporation != null)
                {
                    ddlCorporation.DataSource = Corplist;
                    ddlCorporation.DataTextField = "Name";
                    ddlCorporation.DataValueField = "ID";
                    ddlCorporation.DataBind();
                    ddlCorporation.Items.Insert(0, new ListItem("系统通用", "0"));

                    SetSelectedByValue(ddlCorporation, info.CorporationID.ToString());
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("bankmg.aspx?corpid=" + ddlCorporationFilter.SelectedValue);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                Banks.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    string bankProfitMargin3y = Request["txtBankProfitMargin3y" + i];
                    string bankProfitMargin2y = Request["txtBankProfitMargin2y" + i];
                    string bankProfitMargin1y = Request["txtBankProfitMargin1y" + i];
                    int CorporationID = DataConvert.SafeInt(Request["ddlCorporation" + i]);
                    if (!string.IsNullOrEmpty(name))
                    {
                        BankInfo entity = new BankInfo
                        {
                            Name = name,
                            CorporationID = CorporationID,
                            BankProfitMargin3y = bankProfitMargin3y,
                            BankProfitMargin2y = bankProfitMargin2y,
                            BankProfitMargin1y = bankProfitMargin1y,
                        };
                        Banks.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptData.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtName");
                    System.Web.UI.HtmlControls.HtmlInputText txtBankProfitMargin3y = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtBankProfitMargin3y");
                    System.Web.UI.HtmlControls.HtmlInputText txtBankProfitMargin2y = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtBankProfitMargin2y");
                    System.Web.UI.HtmlControls.HtmlInputText txtBankProfitMargin1y = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtBankProfitMargin1y");
                    System.Web.UI.WebControls.DropDownList ddlCorporation = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCorporation");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            BankInfo entity = new BankInfo
                            {
                                ID = id,
                                CorporationID = DataConvert.SafeInt(ddlCorporation.SelectedValue),
                                Name = txtName.Value,
                                BankProfitMargin3y = txtBankProfitMargin3y.Value,
                                BankProfitMargin2y = txtBankProfitMargin2y.Value,
                                BankProfitMargin1y = txtBankProfitMargin1y.Value,
                            };
                            Banks.Instance.Update(entity);
                        }
                    }
                }
            }
            Banks.Instance.ReloadBankListCache();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? ("~/global/bankmg.aspx?corpid=" + ddlCorporationFilter.SelectedValue)  : FromUrl);
        }
    }
}