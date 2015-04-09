using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Tools.Web;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Components.Web;

namespace Hx.JcbWeb.admin.user
{
    public partial class userlist : JcbBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (WebHelper.GetString("action") == "del")
                {
                    JcbUsers.Instance.DeleteUser(WebHelper.GetInt("id"));
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
                    List<JcbUserInfo> adminlist = JcbUsers.Instance.GetAllUsers();
                    adminlist = adminlist.FindAll(a=>!a.Administrator);
                    if (!string.IsNullOrEmpty(GetString("username")))
                        adminlist = adminlist.FindAll(l => l.UserName.IndexOf(GetString("username")) >= 0);

                    total = adminlist.Count();
                    adminlist = adminlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<JcbUserInfo>();
                    rpadmin.DataSource = adminlist;
                    rpadmin.DataBind();
                    search_fy.RecordCount = total;
                    search_fy.PageSize = pagesize;

                    if (!string.IsNullOrEmpty(GetString("username")))
                        txtUserName.Text = GetString("username");
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("userlist.aspx?username=" + txtUserName.Text);
        }

        protected override void Check()
        {
            if (!JCBContext.Current.UserCheck)
            {
                Response.Redirect("~/admin/Login.aspx");
                return;
            }
            if (!Admin.Administrator)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }
    }
}