﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;

namespace Hx.BackAdmin.user
{
    public partial class main_s : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Admin.Administrator)
            {
                adminlist.Visible = false;
                userlistzjl.Visible = false;
                userlist1.Attributes["class"] = "current";
            }
            if (!Admin.Administrator && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.总经理) == 0)
            {
                userlist3.Visible = false;
                userlistjl.Visible = false;
            }
        }

        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator 
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.总经理) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }
    }
}