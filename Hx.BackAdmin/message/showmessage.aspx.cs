using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;

namespace Hx.BackAdmin.message
{
    public partial class showmessage : AdminBase
    {
        public string ReUrl = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ReUrl = HXContext.Current.ReturnUrl;
                lTitle.Text = HXContext.Current.MessageTitle;
                lContent.Text = HXContext.Current.Message;
                hyreturn.NavigateUrl = HXContext.Current.ReturnUrl;
            }
        }

        protected override void Check()
        {
            //if (!HXContext.Current.AdminCheck)
            //{
            //    Response.Redirect("~/Login.aspx");
            //    return;
            //}
        }
    }
}