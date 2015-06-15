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
using Hx.Components.Query;
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class voterecordlist : AdminBase
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
            };
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
            rpcg.DataSource = WeixinActs.Instance.GetVoteSettingList(true);
            rpcg.DataBind();

            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int total = 0;

            VoteRecordQuery query = new VoteRecordQuery();
            query.SID = GetInt("sid");
            if (!string.IsNullOrEmpty(GetString("aname")))
                query.AthleteName = GetString("aname");
            if (GetInt("snum") > 0)
                query.SerialNumber = GetInt("snum");

            List<VoteRecordInfo> list = WeixinActs.Instance.GetVoteRecordList(pageindex, search_fy.PageSize, query, ref total);
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