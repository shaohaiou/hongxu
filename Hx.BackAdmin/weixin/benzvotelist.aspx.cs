using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Web;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Query;

namespace Hx.BackAdmin.weixin
{
    public partial class benzvotelist : AdminBase
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
            if (!IsPostBack)
            {
                BindControlor();
                LoadData();
            }
        }

        private void BindControlor()
        {
            txtAthleteName.Text = GetString("aname");
            txtSerialNumber.Text = GetString("snum");
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int total = 0;

            BenzvoteQuery query = new BenzvoteQuery();
            if (!string.IsNullOrEmpty(GetString("aname")))
                query.AthleteName = GetString("aname");
            if (GetInt("snum") > 0)
                query.SerialNumber = GetInt("snum");

            List<BenzvoteInfo> list = WeixinActs.Instance.GetBenzvoteList(pageindex, search_fy.PageSize, query, ref total);
            rptdata.DataSource = list;
            rptdata.DataBind();
            search_fy.RecordCount = total;
        }
    }
}