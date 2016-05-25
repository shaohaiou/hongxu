using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Tools;

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
            else if (Admin.UserRole == Components.Enumerations.UserRoleType.财务出纳)
            {
                Response.Redirect("~/car/carquotationmg.aspx");
            }
        }
    }
}