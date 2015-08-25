using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components.Enumerations;

namespace Hx.BackAdmin
{
    public partial class left : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AdminInfo admin = HXContext.Current.AdminUser;
            if (!admin.Administrator)
            {
                index_page.Visible = false;
                globalindex_page.Visible = false;
                userindex_page.Visible = false;
                dayreport_page.Visible = false;
                weixin_page.Visible = false;
                zhaopin_page.Visible = false;
                carquotationindex_page.Visible = false;
                if (((int)Admin.UserRole & (int)UserRoleType.销售经理) > 0)
                {
                    userindex_page.Visible = true;
                    carquotationindex_page.Visible = true;
                }
                if (((int)Admin.UserRole & (int)UserRoleType.人事专员) > 0)
                {
                    zhaopin_page.Visible = true;
                }
                if (((int)Admin.UserRole & (int)UserRoleType.销售员) > 0)
                {
                    carquotationindex_page.Visible = true;
                }
                if (((int)Admin.UserRole & (int)UserRoleType.微信活动管理员) > 0 
                    || ((int)Admin.UserRole & (int)UserRoleType.二手车估价器管理员) > 0
                    || ((int)Admin.UserRole & (int)UserRoleType.卡券活动管理员) > 0
                    || ((int)Admin.UserRole & (int)UserRoleType.投票活动管理员) > 0
                    || ((int)Admin.UserRole & (int)UserRoleType.场景二维码) > 0 
                    || ((int)Admin.UserRole & (int)UserRoleType.广本61活动) > 0)
                {
                    weixin_page.Visible = true;
                }
            }
        }

        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
        }
    }
}