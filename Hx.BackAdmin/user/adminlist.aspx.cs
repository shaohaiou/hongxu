using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools.Web;
using Hx.Components.Entity;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Tools;
using Hx.Components.Enumerations;
using Hx.Components.Web;

namespace Hx.BackAdmin.user
{
    public partial class adminlist : AdminBase
    {
        private List<CorporationInfo> corporationlist = null;
        private List<CorporationInfo> CorporationList
        {
            get
            {
                if (corporationlist == null)
                    corporationlist = Corporations.Instance.GetList();
                return corporationlist;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (WebHelper.GetString("action") == "del")
                {
                    Admins.Instance.DeleteAdmin(WebHelper.GetInt("id"));
                    ResponseRedirect(FromUrl);
                }
                else
                {
                    int pageindex = GetInt("page", 1);
                    if (pageindex < 1)
                    {
                        pageindex = 1;
                    }
                    int pagesize = GetInt("pagesize", 10);
                    int total = 0;
                    List<AdminInfo> adminlist = Admins.Instance.GetAllAdmins();
                    if (GetInt("r") > 0)
                        adminlist = adminlist.FindAll(l => ((int)l.UserRole & GetInt("r")) > 0);
                    if (GetInt("corp") > 0)
                        adminlist = adminlist.FindAll(l => l.Corporation == GetString("corp"));
                    if (!string.IsNullOrEmpty(GetString("username")))
                        adminlist = adminlist.FindAll(l => l.UserName.IndexOf(GetString("username")) >= 0);

                    total = adminlist.Count();
                    adminlist = adminlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<AdminInfo>();
                    rpadmin.DataSource = adminlist;
                    rpadmin.DataBind();
                    search_fy.RecordCount = total;
                    search_fy.PageSize = pagesize;

                    ddlCorporationFilter.DataSource = CorporationList;
                    ddlCorporationFilter.DataTextField = "Name";
                    ddlCorporationFilter.DataValueField = "ID";
                    ddlCorporationFilter.DataBind();
                    ddlCorporationFilter.Items.Insert(0, new ListItem("-所属公司-", "-1"));

                    ddlUserRoleTypeFilter.DataSource = EnumExtensions.ToTable<UserRoleType>();
                    ddlUserRoleTypeFilter.DataTextField = "Name";
                    ddlUserRoleTypeFilter.DataValueField = "Value";
                    ddlUserRoleTypeFilter.DataBind();
                    ddlUserRoleTypeFilter.Items.Insert(0, new ListItem("-用户角色-", "-1"));

                    if (GetInt("r") > 0)
                        SetSelectedByValue(ddlUserRoleTypeFilter, GetString("r"));
                    if (GetInt("corp") > 0)
                        SetSelectedByValue(ddlCorporationFilter, GetString("corp"));
                    if (!string.IsNullOrEmpty(GetString("username")))
                        txtUserName.Text = GetString("username");
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("adminlist.aspx?corp=" + ddlCorporationFilter.SelectedValue + "&username=" + txtUserName.Text + "&r=" + ddlUserRoleTypeFilter.SelectedValue);
        }

        protected string GetCorporationName(string id)
        {
            string result = string.Empty;

            CorporationInfo corp = Corporations.Instance.GetModel(DataConvert.SafeInt(id), true);
            if (corp != null)
                result = corp.Name;

            return result;
        }
    }
}