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
        private string subscribe = "0";
        public string Subscribe { get { return subscribe; } }
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
                        string accesstoken = WeixinActs.Instance.GetAccessToken(CurrentSetting.AppID, CurrentSetting.AppSecret);
                        Dictionary<string, string> openinfo = WeixinActs.Instance.GetOpeninfo(accesstoken, Openid);
                        if (!openinfo.Keys.Contains("subscribe") || openinfo["subscribe"] == "0")
                        {
                            NeedAttention = true;
                        }
                    }
                }

                LoadData();
            }
        }

        private void LoadData()
        {
            int id = GetInt("id");
            CurrentPothunterInfo = WeixinActs.Instance.GetVotePothunterInfo(id, true);
            if (CurrentPothunterInfo == null)
            {
                Response.Write("<script>alert(\"选手信息错误\");location.href=\"" + (string.IsNullOrEmpty(FromUrl) ? ("votepg.aspx?sid=" + SID) : FromUrl) + "\"</script>");
                Response.End();
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