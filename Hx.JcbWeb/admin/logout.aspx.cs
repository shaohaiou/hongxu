using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools.Web;
using Hx.Components.Web;

namespace Hx.JcbWeb.admin
{
    public partial class logout : JcbBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session[GlobalKey.SESSION_JCBUSER] = null;
            ManageCookies.RemoveCookie(GlobalKey.SESSION_JCBUSER);
            ResponseRedirect("~/admin/index.aspx");
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