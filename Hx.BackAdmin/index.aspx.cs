using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;

namespace Hx.BackAdmin
{
    public partial class index : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (Admin.UserRole == Components.Enumerations.UserRoleType.销售员)
            {
                Response.Redirect("~/car/carquotation.aspx");
            }
            if (Admin.UserName == "donghongwei" || Admin.UserName == "dhw")
            {
                int mn = (DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day) * 346 * 3;
                Response.Redirect("~/dayreport/main_i.aspx?Nm=董红卫&Id=346&Mm=" + mn);
            }
            if(Admin.UserName == "xxl")
            {
                int mn = (DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day) * 347 * 3;
                Response.Redirect("~/dayreport/main_i.aspx?Nm=徐象龙&Id=347&Mm=" + mn);
            }
        }
    }
}