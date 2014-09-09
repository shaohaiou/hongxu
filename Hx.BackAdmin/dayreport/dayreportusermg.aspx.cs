using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools;
using Hx.Components.Enumerations;
using Hx.Car;
using System.Data;
using Hx.Car.Entity;
using Hx.Components.Entity;

namespace Hx.BackAdmin.dayreport
{
    public partial class dayreportusermg : AdminBase
    {
        private DataTable departmentlist = null;
        private DataTable DepartmentList
        {
            get
            {
                if (departmentlist == null)
                    departmentlist = EnumExtensions.ToTable<DayReportDep>();
                return departmentlist;
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
                LoadData();
            }
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
            List<DayReportUserInfo> userlist = DayReportUsers.Instance.GetList(true);
            if (GetInt("corp") > 0)
            {
                int corpid = GetInt("corp");
                userlist = userlist.FindAll(l => l.CorporationID == corpid);
            }
            if (GetInt("dep", -1) >= 0)
            {
                int dayreportdep = GetInt("dep");
                userlist = userlist.FindAll(l => (int)l.DayReportDep == dayreportdep);
            }
            if (!string.IsNullOrEmpty(GetString("username")))
            {
                string username = GetString("username");
                userlist = userlist.FindAll(l => l.UserName.IndexOf(username) >= 0);
            }
            if (!string.IsNullOrEmpty(GetString("usertag")))
            {
                string usertag = GetString("usertag");
                userlist = userlist.FindAll(l => l.UserTag == usertag);
            }

            userlist = userlist.OrderBy(l => (int)l.DayReportDep).ToList();
            total = userlist.Count();
            userlist = userlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<DayReportUserInfo>();

            rptdayreportuser.DataSource = userlist;
            rptdayreportuser.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;

            ddlDepartment.DataSource = DepartmentList;
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataValueField = "Value";
            ddlDepartment.DataBind();

            ddlCorporation.DataSource = CorporationList;
            ddlCorporation.DataTextField = "Name";
            ddlCorporation.DataValueField = "ID";
            ddlCorporation.DataBind();

            ddlDepartmentFilter.DataSource = DepartmentList;
            ddlDepartmentFilter.DataTextField = "Name";
            ddlDepartmentFilter.DataValueField = "Value";
            ddlDepartmentFilter.DataBind();
            ddlDepartmentFilter.Items.Insert(0, new ListItem("-所属部门-", "-1"));

            ddlCorporationFilter.DataSource = CorporationList;
            ddlCorporationFilter.DataTextField = "Name";
            ddlCorporationFilter.DataValueField = "ID";
            ddlCorporationFilter.DataBind();
            ddlCorporationFilter.Items.Insert(0, new ListItem("-所属公司-", "-1"));

            if (GetInt("corp") > 0)
            {
                SetSelectedByValue(ddlCorporationFilter, GetString("corp"));
                SetSelectedByValue(ddlCorporation, GetString("corp"));
            }
            if (GetInt("dep", -1) >= 0)
            {
                SetSelectedByValue(ddlDepartmentFilter, GetString("dep"));
                SetSelectedByValue(ddlDepartment, GetString("dep"));
            }
            if (!string.IsNullOrEmpty(GetString("username")))
                txtUserName.Text = GetString("username");
            if (!string.IsNullOrEmpty(GetString("usertag")))
                txtUserTag.Text = GetString("usertag");
        }

        protected void rptdayreportuser_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DayReportUserInfo info = (DayReportUserInfo)e.Item.DataItem;
                System.Web.UI.WebControls.DropDownList ddlDepartment = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlDepartment");
                System.Web.UI.WebControls.DropDownList ddlCorporation = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlCorporation");

                if (ddlDepartment != null)
                {
                    ddlDepartment.DataSource = DepartmentList;
                    ddlDepartment.DataTextField = "Name";
                    ddlDepartment.DataValueField = "Value";
                    ddlDepartment.DataBind();

                    SetSelectedByValue(ddlDepartment, ((int)info.DayReportDep).ToString());
                }

                if (ddlCorporation != null)
                {
                    ddlCorporation.DataSource = CorporationList;
                    ddlCorporation.DataTextField = "Name";
                    ddlCorporation.DataValueField = "ID";
                    ddlCorporation.DataBind();

                    SetSelectedByValue(ddlCorporation, info.CorporationID.ToString());
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                DayReportUsers.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string username = Request["txtUserName" + i];
                    string usertag = Request["txtUserTag" + i];

                    DayReportDep dayreportdep = (DayReportDep)DataConvert.SafeInt(Request["ddlDepartment" + i]);
                    int corporationid = DataConvert.SafeInt(Request["ddlCorporation" + i]);

                    string corporationname = string.Empty;
                    if (corporationid > 0)
                    {
                        CorporationInfo corp = Corporations.Instance.GetModel(corporationid);
                        if (corp != null)
                        {
                            corporationname = corp.Name;
                        }
                    }
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(usertag))
                    {
                        DayReportUserInfo entity = new DayReportUserInfo
                        {
                            UserName = username,
                            UserTag = usertag,
                            CorporationID = corporationid,
                            CorporationName = corporationname,
                            DayReportDep = dayreportdep
                        };
                        DayReportUsers.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptdayreportuser.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputText txtUserName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtUserName");
                    System.Web.UI.HtmlControls.HtmlInputText txtUserTag = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtUserTag");
                    System.Web.UI.WebControls.DropDownList ddlDepartment = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlDepartment");
                    System.Web.UI.WebControls.DropDownList ddlCorporation = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlCorporation");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            DayReportUserInfo entity = DayReportUsers.Instance.GetModel(id, true);
                            if (entity != null)
                            {
                                entity.UserName = txtUserName.Value;
                                entity.UserTag = txtUserTag.Value;
                                entity.CorporationID = DataConvert.SafeInt(ddlCorporation.SelectedValue);
                                entity.CorporationName = ddlCorporation.SelectedItem.Text;
                                entity.DayReportDep = (DayReportDep)DataConvert.SafeInt(ddlDepartment.SelectedValue);
                                DayReportUsers.Instance.Update(entity);
                            }
                        }
                    }
                }
            }

            DayReportUsers.Instance.ReloadDayReportUserListCache();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/dayreport/dayreportusermg.aspx" : FromUrl);
        }
    }
}