using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;

namespace Hx.JcbWeb.message
{
    public partial class showmessage : JcbBase
    {

        protected override void Check()
        {

        }

        public string ReUrl = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ReUrl = System.Web.HttpUtility.UrlDecode(JCBContext.Current.ReturnUrl);
                lTitle.Text = JCBContext.Current.MessageTitle;
                lContent.Text = JCBContext.Current.Message;
                hyreturn.NavigateUrl = System.Web.HttpUtility.UrlDecode(JCBContext.Current.ReturnUrl);
            }
        }
    }
}