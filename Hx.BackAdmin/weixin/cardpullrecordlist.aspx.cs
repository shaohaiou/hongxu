using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.weixin
{
    public partial class cardpullrecordlist : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.卡券活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int total = 0;
            List<CardPullRecordInfo> list = WeixinActs.Instance.GetCardPullRecordList(true).OrderByDescending(c => c.PullResult).ThenByDescending(c => c.AddTime).ToList();
            total = list.Count();
            list = list.Skip((pageindex - 1) * search_fy.PageSize).Take(search_fy.PageSize).ToList<CardPullRecordInfo>();
            rptdata.DataSource = list;
            rptdata.DataBind();
            search_fy.RecordCount = total;
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            WeixinActs.Instance.ClearCardPullRecord();
            WeixinActs.Instance.ReloadCardPullRecordListCache();
            Response.Redirect("~/weixin/cardpullrecordlist.aspx");
        }

    }
}