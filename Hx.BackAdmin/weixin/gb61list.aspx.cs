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
    public partial class gb61list : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator
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
            List<GB61Info> list = WeixinActs.Instance.GetGB61InfoList().OrderByDescending(c => c.ID).ToList();
            total = list.Count();
            list = list.Skip((pageindex - 1) * search_fy.PageSize).Take(search_fy.PageSize).ToList<GB61Info>();
            rptdata.DataSource = list;
            rptdata.DataBind();
            search_fy.RecordCount = total;
        }
    }
}