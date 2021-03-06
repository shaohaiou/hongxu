﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;

namespace Hx.BackAdmin.weixin
{
    public partial class main_s : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator 
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.微信活动管理员) == 0
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.二手车估价器管理员) == 0
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.卡券活动管理员) == 0
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.投票活动管理员) == 0
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.场景二维码) == 0
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.广本61活动) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HXContext.Current.AdminUser.Administrator 
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.微信活动管理员) == 0)
            {
                benzvotemg.Visible = false;
                jituanvotemg.Visible = false;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.二手车估价器管理员) == 0)
            {
                escpgmg.Visible = false;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.卡券活动管理员) == 0)
            {
                cardmg.Visible = false;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.投票活动管理员) == 0)
            {
                votemg.Visible = false;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.场景二维码) == 0)
            {
                scenecodemg.Visible = false;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.广本61活动) == 0)
            {
                gb61.Visible = false;
            }
        }
    }
}