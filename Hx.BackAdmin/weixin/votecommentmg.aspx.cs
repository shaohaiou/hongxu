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
    public partial class votecommentmg : AdminBase
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

        private List<VotePothunterInfo> votepothunterlist = null;
        public List<VotePothunterInfo> VotePothunterlist
        {
            get
            {
                if (votepothunterlist == null)
                    votepothunterlist = WeixinActs.Instance.GetVotePothunterList(GetInt("sid"), true);
                return votepothunterlist;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (GetString("action") == "del")
            {
                WeixinActs.Instance.DelVoteCommentInfo(GetInt("id"));
                WeixinActs.Instance.ReloadVoteCommentsCache(GetInt("aid"));
                ResponseRedirect(FromUrl);
            }
            else if (GetString("action") == "check")
            {
                List<string> ids = new List<string>();
                List<string> aids = new List<string>();
                foreach (string idstr in GetString("ids").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string id = idstr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    string aid = idstr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    if (!ids.Contains(id)) ids.Add(id);
                    if (!aids.Contains(aid)) aids.Add(aid);
                }
                WeixinActs.Instance.CheckVoteCommentStatus(string.Join(",", ids));
                foreach (string aid in aids)
                {
                    WeixinActs.Instance.ReloadVoteCommentsCache(DataConvert.SafeInt(aid));
                }
                ResponseRedirect(FromUrl);
            }
            if (!IsPostBack)
            {
                BindControlor();
                LoadData();
            }
        }

        private void BindControlor()
        {
            rpcg.DataSource = WeixinActs.Instance.GetVoteSettingList(true);
            rpcg.DataBind();

            ddlPothunterName.DataSource = VotePothunterlist;
            ddlPothunterName.DataTextField = "Name";
            ddlPothunterName.DataValueField = "ID";
            ddlPothunterName.DataBind();

            ddlPothunterName.Items.Insert(0, new ListItem("-选手姓名-", ""));

            if (GetInt("aid") > 0)
                ddlPothunterName.SelectedValue = GetInt("aid").ToString();
            if (GetInt("cks", -1) >= 0)
                ddlCheckStatus.SelectedValue = GetInt("cks").ToString();
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            List<VoteCommentInfo> list = new List<VoteCommentInfo>();
            int aid = GetInt("aid");
            if (aid > 0)
                list.AddRange(WeixinActs.Instance.GetVoteComments(aid, true));
            else
            {
                foreach (VotePothunterInfo pinfo in VotePothunterlist)
                {
                    list.AddRange(WeixinActs.Instance.GetVoteComments(pinfo.ID, true));
                }
            }
            if (GetInt("cks") > 0)
                list = list.FindAll(p => p.CheckStatus == GetInt("cks"));
            list = list.OrderBy(p => p.CheckStatus).ThenByDescending(p => p.ID).ToList();
            int total = list.Count;
            int skipcount = search_fy.PageSize * (pageindex - 1);
            if (list.Count > skipcount)
            {
                list = list.Skip(skipcount).ToList();
                list = list.Count > search_fy.PageSize ? list.Take(search_fy.PageSize).ToList() : list;
            }
            rptdata.DataSource = list;
            rptdata.DataBind();


            search_fy.RecordCount = total;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Response.Redirect("votecommentmg.aspx?sid=" + GetInt("sid") + "&aid=" + ddlPothunterName.SelectedValue + "&cks=" + ddlCheckStatus.SelectedValue);
        }

        protected string GetPothunterName(string id)
        {
            string result = string.Empty;

            if (VotePothunterlist.Exists(p => p.ID.ToString() == id))
            {
                result = VotePothunterlist.Find(p => p.ID.ToString() == id).Name;
            }

            return result;
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