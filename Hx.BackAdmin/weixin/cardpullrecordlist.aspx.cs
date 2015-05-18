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
using Hx.Tools;

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
            if (!HXContext.Current.AdminUser.Administrator)
            {
                int sid = GetInt("sid");
                CardSettingInfo setting = WeixinActs.Instance.GetCardSetting(sid, true);
                if (!setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(AdminID.ToString()))
                {
                    Response.Clear();
                    Response.Write("您没有权限操作！");
                    Response.End();
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            rpcg.DataSource = WeixinActs.Instance.GetCardSettingList(true);
            rpcg.DataBind();
            int sid = GetInt("sid");
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int total = 0;
            List<CardPullRecordInfo> list = WeixinActs.Instance.GetCardPullRecordList(sid, true).OrderByDescending(c => c.PullResult).ThenByDescending(c => c.AddTime).ToList();
            total = list.Count();
            list = list.Skip((pageindex - 1) * search_fy.PageSize).Take(search_fy.PageSize).ToList<CardPullRecordInfo>();
            rptdata.DataSource = list;
            rptdata.DataBind();
            search_fy.RecordCount = total;
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            int sid = GetInt("sid");
            WeixinActs.Instance.ClearCardPullRecord(sid);
            WeixinActs.Instance.ReloadCardPullRecordListCache(sid);
            Response.Redirect("~/weixin/cardpullrecordlist.aspx?sid=" + GetInt("sid"));
        }

        protected string SetCardSettingStatus(string id)
        {
            string result = string.Empty;

            if (!Admin.Administrator)
            {
                CardSettingInfo setting = WeixinActs.Instance.GetCardSetting(DataConvert.SafeInt(id), true);

                if (setting != null)
                {
                    string[] powerusers = setting.PowerUser.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!powerusers.Contains(AdminID.ToString()))
                        result = "style=\"display:none;\"";
                }
            }

            return result;
        }

    }
}