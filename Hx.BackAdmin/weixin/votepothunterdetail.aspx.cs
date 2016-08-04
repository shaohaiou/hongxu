using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.weixin
{
    public partial class votepothunterdetail : WeixinBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Code { get; set; }
        public string Openid { get; set; }
        public int SID { get; set; }
        public bool NeedAttention { get; set; }
        protected VotePothunterInfo CurrentPothunterInfo = null;
        private VoteSettingInfo currentsetting = null;
        protected VoteSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                    currentsetting = WeixinActs.Instance.GetVoteSetting(SID,true);
                return currentsetting;
            }
        }
        private string signature = string.Empty;
        public string CurrentSignature
        {
            get
            {
                if (string.IsNullOrEmpty(signature))
                    signature = Hx.Tools.EncryptString.SHA1_Hash(string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", WeixinActs.Instance.GetJsapiTicket(CurrentSetting.AppID, CurrentSetting.AppSecret), NonceStr, Timestamp, Request.Url.AbsoluteUri));
                return signature;
            }
        }
        private static object synchelper = new object();
        private List<VoteCommentInfo> _listcomment = null;
        public List<VoteCommentInfo> ListComment
        {
            get
            {
                if (_listcomment == null)
                {
                    lock (synchelper)
                    {
                        _listcomment = WeixinActs.Instance.GetVoteComments(CurrentPothunterInfo.ID,true);
                    }
                }
                return _listcomment;
            }
        }

        public int CommentPageCount { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NeedAttention = false;
                Code = GetString("code");
                SID = GetInt("sid", 1);
                string key = GlobalKey.VOTEOPENID + "_" + SID;
                Openid = Session[key] as string;
                if (string.IsNullOrEmpty(Openid) && !string.IsNullOrEmpty(Code))
                {
                    Openid = WeixinActs.Instance.GetOpenid(CurrentSetting.AppID, CurrentSetting.AppSecret, Code);
                    if (!string.IsNullOrEmpty(Openid))
                    {
                        Session[key] = Openid;
                    }
                }
                if (!string.IsNullOrEmpty(Openid))
                {
                    string accesstoken = WeixinActs.Instance.GetAccessToken(CurrentSetting.AppID, CurrentSetting.AppSecret);
                    Dictionary<string, object> openinfo = WeixinActs.Instance.GetOpeninfo(accesstoken, Openid);
                    if (!openinfo.Keys.Contains("subscribe") || (openinfo["subscribe"].ToString()) == "0")
                    {
                        NeedAttention = true;
                    }
                }
                LoadData();
            }
        }

        private void LoadData()
        {
            CommentPageCount = 1;
            int id = GetInt("id");
            CurrentPothunterInfo = WeixinActs.Instance.ReorderVotePothunter(WeixinActs.Instance.GetVotePothunterList(SID, true)).Find(p=>p.ID == id);
            if (CurrentPothunterInfo == null)
            {
                Response.Write("<script>alert(\"选手信息错误\");location.href=\"" + (string.IsNullOrEmpty(FromUrl) ? ("votepg.aspx?sid=" + SID) : FromUrl) + "\"</script>");
                Response.End();
            }

            List<VoteCommentInfo> source = ListComment.FindAll(c=>c.CheckStatus == 1).OrderByDescending(c => c.ID).ToList();

            rptCommentFirstTwo.DataSource = source.Count > 2 ? source.Take(2) : source;
            rptCommentFirstTwo.DataBind();

            source = source.OrderBy(c => c.ID).ToList();
            rptComment.DataSource = source.Count > 8 ? source.Take(8) : source;
            rptComment.DataBind();
            CommentPageCount = (source.Count / 8) + (source.Count % 8 > 0 ? 1 : 0);
        }

        /// <summary>
        /// 登录页面不需要验证
        /// </summary>
        protected override void Check()
        {

        }
    }
}