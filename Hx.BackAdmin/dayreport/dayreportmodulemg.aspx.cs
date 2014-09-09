using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Tools;
using Hx.Components;
using Hx.Components.Enumerations;
using System.Data;
using Hx.Components.Entity;

namespace Hx.BackAdmin.dayreport
{
    public partial class dayreportmodulemg : AdminBase
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

            List<DailyReportModuleInfo> list = DayReportModules.Instance.GetList(true);
            if (GetInt("dep",-1) >= 0)
                list = list.FindAll(l => (int)l.Department == GetInt("dep")).OrderBy(l=>l.Sort).ToList();
            list = list.OrderBy(l=>(int)l.Department).ToList();
            total = list.Count();
            list = list.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<DailyReportModuleInfo>();

            rptmodule.DataSource = list;
            rptmodule.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;

            ddlDepartment.DataSource = DepartmentList;
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataValueField = "Value";
            ddlDepartment.DataBind();

            ddlDepartmentFilter.DataSource = DepartmentList;
            ddlDepartmentFilter.DataTextField = "Name";
            ddlDepartmentFilter.DataValueField = "Value";
            ddlDepartmentFilter.DataBind();
            ddlDepartmentFilter.Items.Insert(0, new ListItem("-所属部门-","-1"));

            if (GetInt("dep", -1) >= 0)
            {
                SetSelectedByValue(ddlDepartmentFilter, GetString("dep"));
                SetSelectedByValue(ddlDepartment, GetString("dep"));
            }
        }

        protected void rptmodule_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DailyReportModuleInfo info = (DailyReportModuleInfo)e.Item.DataItem;
                System.Web.UI.WebControls.TextBox txtSort = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtSort");
                txtSort.Text = info.Sort.ToString();
                txtSort.Attributes.Add("oldval", info.Sort.ToString());

                System.Web.UI.WebControls.DropDownList ddlDepartment = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlDepartment");
                if (ddlDepartment != null)
                {
                    ddlDepartment.DataSource = DepartmentList;
                    ddlDepartment.DataTextField = "Name";
                    ddlDepartment.DataValueField = "Value";
                    ddlDepartment.DataBind();

                    SetSelectedByValue(ddlDepartment, ((int)info.Department).ToString());
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("dayreportmodulemg.aspx?dep=" + ddlDepartmentFilter.SelectedValue);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string delIds = hdnDelIds.Value;
            if (!string.IsNullOrEmpty(delIds))
            {
                DayReportModules.Instance.Delete(delIds);
            }

            int addCount = DataConvert.SafeInt(hdnAddCount.Value);

            if (addCount > 0)
            {
                for (int i = 1; i <= addCount; i++)
                {
                    string name = Request["txtName" + i];
                    int sort = DataConvert.SafeInt(Request["txtSort" + i]);
                    string description = Request["txtDescription" + i];
                    DayReportDep dayreportdep = (DayReportDep)DataConvert.SafeInt(Request["ddlDepartment" + i]);
                    bool ismonthlytarget = DataConvert.SafeInt(Request["hdnIsmonthlytarget" + i]) == 1;
                    bool mustinput = DataConvert.SafeInt(Request["hdnMustinput" + i]) == 1;
                    bool iscount = DataConvert.SafeInt(Request["hdnIscount" + i]) == 1;
                    if (!string.IsNullOrEmpty(name))
                    {
                        DailyReportModuleInfo entity = new DailyReportModuleInfo
                        {
                            Sort = sort,
                            Name = name,
                            Description = description,
                            Ismonthlytarget = ismonthlytarget,
                            Department = dayreportdep,
                            Mustinput = mustinput,
                            Iscount = iscount
                        };
                        DayReportModules.Instance.Add(entity);
                    }
                }
            }

            foreach (RepeaterItem item in rptmodule.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    System.Web.UI.WebControls.TextBox txtSort = (System.Web.UI.WebControls.TextBox)item.FindControl("txtSort");
                    System.Web.UI.HtmlControls.HtmlInputText txtName = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtName");
                    System.Web.UI.HtmlControls.HtmlInputText txtDescription = (System.Web.UI.HtmlControls.HtmlInputText)item.FindControl("txtDescription");
                    System.Web.UI.WebControls.DropDownList ddlDepartment = (System.Web.UI.WebControls.DropDownList)item.FindControl("ddlDepartment");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxIsmonthlytarget = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxIsmonthlytarget");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxMustinput = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxMustinput");
                    System.Web.UI.HtmlControls.HtmlInputCheckBox cbxIscount = (System.Web.UI.HtmlControls.HtmlInputCheckBox)item.FindControl("cbxIscount");
                    System.Web.UI.HtmlControls.HtmlInputHidden hdnID = (System.Web.UI.HtmlControls.HtmlInputHidden)item.FindControl("hdnID");
                    if (hdnID != null)
                    {
                        int id = DataConvert.SafeInt(hdnID.Value);
                        if (id > 0)
                        {
                            DailyReportModuleInfo entity = new DailyReportModuleInfo
                            {
                                ID = id,
                                Sort = DataConvert.SafeInt(txtSort.Text),
                                Name = txtName.Value,
                                Description = txtDescription.Value,
                                Department = (DayReportDep)DataConvert.SafeInt(ddlDepartment.SelectedValue),
                                Ismonthlytarget = cbxIsmonthlytarget.Checked,
                                Mustinput = cbxMustinput.Checked,
                                Iscount = cbxIscount.Checked
                            };
                            DayReportModules.Instance.Update(entity);
                        }
                    }
                }
            }
            DayReportModules.Instance.ReloadDailyReportModuleListCache();

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/dayreport/dayreportmodulemg.aspx" : FromUrl);
        }
    }
}