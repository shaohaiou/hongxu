using System;
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
using Hx.Tools.Web;

namespace Hx.BackAdmin.weixin
{
    public partial class benzvotemg : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.微信活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebHelper.GetString("action") == "del")
            {
                WeixinActs.Instance.DelBenzvotePothunterInfo(WebHelper.GetString("id"));
                ResponseRedirect(FromUrl);
            }
            else
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int total = 0;
            List<BenzvotePothunterInfo> list = WeixinActs.Instance.GetBenzvotePothunterList();
            total = list.Count();
            list = list.Skip((pageindex - 1) * search_fy.PageSize).Take(search_fy.PageSize).ToList<BenzvotePothunterInfo>();
            rptdata.DataSource = list;
            rptdata.DataBind();
            search_fy.RecordCount = total;
        }
    }
}