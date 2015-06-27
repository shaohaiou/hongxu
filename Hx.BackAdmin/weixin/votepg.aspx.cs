using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools.Web;

namespace Hx.BackAdmin.weixin
{
    public partial class votepg : WeixinBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Code { get; set; }
        public string Openid { get; set; }
        public int SID { get; set; }
        protected int PageIndex = 1;
        protected int PageSize = 30;
        protected int PageCount = 1;
        public bool NeedAttention { get; set; }
        private VoteSettingInfo currentsetting = null;
        protected VoteSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                    currentsetting = WeixinActs.Instance.GetVoteSetting(SID, true);
                return currentsetting;
            }
        }
        public string PrevUrl
        {
            get
            {
                List<string> querys = new List<string>();
                int prev = 1;
                if (PageIndex > 1) prev = PageIndex - 1;
                foreach (string key in Request.QueryString.AllKeys)
                {
                    if (key != "page")
                        querys.Add(key + "=" + Request.QueryString[key]);
                }
                querys.Add("page=" + prev);
                return "?" + string.Join("&", querys); ;
            }
        }

        public string NextUrl
        {
            get
            {
                List<string> querys = new List<string>();
                int next = PageCount;
                if (PageIndex < PageCount) next = PageIndex + 1;
                foreach (string key in Request.QueryString.AllKeys)
                {
                    if (key != "page")
                        querys.Add(key + "=" + Request.QueryString[key]);
                }
                querys.Add("page=" + next);
                return "?" + string.Join("&", querys); ;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NeedAttention = false;
                Code = GetString("code");
                SID = GetInt("sid", 1);
                Openid = Session[GlobalKey.VOTEOPENID] as string;
                if (string.IsNullOrEmpty(Openid) && !string.IsNullOrEmpty(Code))
                {
                    Openid = WeixinActs.Instance.GetOpenid(CurrentSetting.AppID, CurrentSetting.AppSecret, Code);
                    if (!string.IsNullOrEmpty(Openid))
                    {
                        Session[GlobalKey.VOTEOPENID] = Openid;
                    }
                }
                if (!string.IsNullOrEmpty(Openid))
                {
                    string accesstoken = WeixinActs.Instance.GetAccessToken(CurrentSetting.AppID, CurrentSetting.AppSecret);
                    Dictionary<string, string> openinfo = WeixinActs.Instance.GetOpeninfo(accesstoken, Openid);
                    if (!openinfo.Keys.Contains("subscribe") || openinfo["subscribe"] == "0")
                    {
                        NeedAttention = true;
                    }
                }
                LoadData();
            }
        }

        private void LoadData()
        {
            PageIndex = GetInt("page", 1);
            if (PageIndex < 1)
            {
                PageIndex = 1;
            }
            int total = 0;

            List<VotePothunterInfo> plist = WeixinActs.Instance.GetVotePothunterList(SID, true);
            plist = plist.OrderByDescending(l => l.Ballot).ToList();
            total = plist.Count();
            plist = plist.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList<VotePothunterInfo>();
            rptData.DataSource = plist;
            rptData.DataBind();

            PageCount = (total % PageSize > 0 ? 1 : 0) + total / PageSize;
        }

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                VotePothunterInfo entity = (VotePothunterInfo)e.Item.DataItem;
                Repeater rptCommentFirstOne = (Repeater)e.Item.FindControl("rptCommentFirstOne");
                Repeater rptCommentMore = (Repeater)e.Item.FindControl("rptCommentMore");
                List<VoteCommentInfo> listComment = WeixinActs.Instance.GetVoteComments(entity.ID, true);
                List<VoteCommentInfo> source = listComment.FindAll(c=>c.CheckStatus == 1).OrderByDescending(c => c.ID).ToList();
                if (rptCommentFirstOne != null)
                {
                    rptCommentFirstOne.DataSource = source.Count > 1 ? source.Take(1) : source;
                    rptCommentFirstOne.DataBind();
                }
                if (rptCommentMore != null && source.Count > 1)
                {
                    source = source.Skip(1).ToList();
                    rptCommentMore.DataSource = source.Count > 4 ? source.Take(4) : source;
                    rptCommentMore.DataBind();
                }
            }
        }

        /// <summary>
        /// 登录页面不需要验证
        /// </summary>
        protected override void Check()
        {

        }
    }
}