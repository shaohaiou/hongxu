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
    public partial class main_x : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}