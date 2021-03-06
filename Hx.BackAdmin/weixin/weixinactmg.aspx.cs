﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Tools;
using Hx.Components.Query;
using Hx.Components.Web;

namespace Hx.BackAdmin.weixin
{
    public partial class weixinactmg : AdminBase
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
            if (!IsPostBack)
            {
                BindControlor();
                LoadData();
            }
        }
        private void BindControlor()
        {
            if (GetInt("sex") > 0)
                SetSelectedByValue(ddlSex, GetString("sex"));

            txtNickname.Text = GetString("nickname");
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int pagesize = GetInt("pagesize", 10);
            int total = 0;

            WeixinActQuery query = new WeixinActQuery();
            if (ddlSex.SelectedIndex > 0)
                query.Sex = DataConvert.SafeInt(ddlSex.SelectedValue);
            if (!string.IsNullOrEmpty(txtNickname.Text.Trim()))
                query.Nickname = txtNickname.Text.Trim();

            List<WeixinActInfo> list = WeixinActs.Instance.GetList(pageindex, pagesize, query, ref total);
            rptweixinact.DataSource = list;
            rptweixinact.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            List<string> query = new List<string>();
            if (!string.IsNullOrEmpty(txtNickname.Text.Trim()))
                query.Add("nickname=" + txtNickname.Text.Trim());
            if (ddlSex.SelectedIndex > 0)
                query.Add("sex=" + ddlSex.SelectedValue);

            Response.Redirect("weixinactmg.aspx?" + string.Join("&", query));
        }
    }
}