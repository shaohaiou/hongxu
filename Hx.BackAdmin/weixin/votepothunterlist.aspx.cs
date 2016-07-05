using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools;
using Hx.Components.Web;
using Hx.Tools.Web;

namespace Hx.BackAdmin.weixin
{
    public partial class votepothunterlist : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator 
                && ((int)HXContext.Current.AdminUser.UserRole & (int)Components.Enumerations.UserRoleType.投票活动管理员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
            if (!HXContext.Current.AdminUser.Administrator)
            {
                int sid = GetInt("sid");
                VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(sid, true);
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
            if (WebHelper.GetString("action") == "del")
            {
                WeixinActs.Instance.DelVotePothunterInfo(GetInt("sid"),WebHelper.GetString("id"));
                ResponseRedirect(FromUrl);
            }
            else if (WebHelper.GetString("action") == "clear")
            {
                List<VotePothunterInfo> list = WeixinActs.Instance.GetVotePothunterList(GetInt("sid"));
                foreach (VotePothunterInfo p in list)
                {
                    p.Ballot = 0;
                    WeixinActs.Instance.AddVotePothunterInfo(p);
                }
                WeixinActs.Instance.ReloadVotePothunterList(GetInt("sid"));
                WeixinActs.Instance.ClearVoteRecord(GetInt("sid"));
                WeixinActs.Instance.ReloadVoteRecordList(GetInt("sid"));
                ResponseRedirect(FromUrl);
            }
            else
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            rpcg.DataSource = WeixinActs.Instance.GetVoteSettingList(true);
            rpcg.DataBind();

            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int total = 0;
            List<VotePothunterInfo> list = WeixinActs.Instance.GetVotePothunterList(GetInt("sid"));
            total = list.Count();
            list = list.Skip((pageindex - 1) * search_fy.PageSize).Take(search_fy.PageSize).ToList<VotePothunterInfo>();
            rptdata.DataSource = list;
            rptdata.DataBind();
            search_fy.RecordCount = total;
        }

        protected string SetVoteSettingStatus(string id)
        {
            string result = string.Empty;

            if (!Admin.Administrator)
            {
                VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(DataConvert.SafeInt(id), true);

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