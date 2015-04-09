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

namespace Hx.JcbWeb.admin.user
{
    public partial class adminlist : JcbBase
    {
        protected void Page_Load(object sender, EventArgs e)
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
                adminlist = adminlist.FindAll(a=>a.Administrator);
                total = adminlist.Count();
                adminlist = adminlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<JcbUserInfo>();
                rpadmin.DataSource = adminlist;
                rpadmin.DataBind();
                search_fy.RecordCount = total;
                search_fy.PageSize = pagesize;
            }
        }
    }
}