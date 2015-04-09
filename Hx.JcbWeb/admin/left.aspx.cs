using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Entity;

namespace Hx.JcbWeb.admin
{
    public partial class left : JcbBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JcbUserInfo admin = JCBContext.Current.AdminUser;
            if (!admin.Administrator)
            {
                userindex_page.Visible = false;
            }
        }

        protected override void Check()
        {
            if (!JCBContext.Current.UserCheck)
            {
                Response.Redirect("~/admin/Login.aspx");
                return;
            }
        }
    }
}