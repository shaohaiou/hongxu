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

namespace Hx.BackAdmin.user
{
    public partial class adminlist : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
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
                total = adminlist.Count();
                adminlist = adminlist.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList<AdminInfo>();
                rpadmin.DataSource = adminlist;
                rpadmin.DataBind();
                search_fy.RecordCount = total;
                search_fy.PageSize = pagesize;
            }
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