﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools.Web;
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class benzvotedetail : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Code { get; set; }
        public string Openid { get; set; }
        private string subscribe = "0";
        public string Subscribe { get { return subscribe; } }
        protected BenzvotePothunterInfo CurrentPothunterInfo = null;
        private BenzvoteSettingInfo currentsetting = null;
        protected BenzvoteSettingInfo CurrentSetting 
        {
            get
            {
                if (currentsetting == null)
                    currentsetting = WeixinActs.Instance.GetBenzvoteSetting(true);
                return currentsetting;
            }
        }

        private int timestamp = 0;
        protected int Timestamp
        {
            get
            {
                if (timestamp == 0)
                    timestamp = Utils.ConvertDateTimeInt(DateTime.Now);
                return timestamp;
            }
        }

        private string nonceStr = string.Empty;
        protected string NonceStr
        {
            get
            {
                if (string.IsNullOrEmpty(nonceStr))
                {
                    nonceStr = EncryptString.MD5(DateTime.Now.ToString("yyyyMMddHHmiss") + DateTime.Now.Millisecond, true);
                }
                return nonceStr;
            }
        }

        private string signature = string.Empty;
        protected string Signature
        {
            get
            {
                if (string.IsNullOrEmpty(signature))
                    signature = EncryptString.SHA1_Hash(string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", WeixinActs.Instance.GetJsapiTicket(), NonceStr, Timestamp, Request.Url.AbsoluteUri));
                return signature;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Openid = Session[GlobalKey.BENZVOTEOPENID] as string;
                Code = GetString("code");
                if (string.IsNullOrEmpty(Openid) && !string.IsNullOrEmpty(Code))
                {
                    string url_openid = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code"
                        , GlobalKey.WEIXINAPPID
                        , GlobalKey.WEIXINSECRET
                        , Code);
                    string str_openid = Http.GetPageByWebClientDefault(url_openid);
                    Dictionary<string, string> dic_openid = new Dictionary<string, string>();
                    try
                    {
                        dic_openid = json.Deserialize<Dictionary<string, string>>(str_openid);
                    }
                    catch { }
                    if (dic_openid.ContainsKey("openid"))
                    {
                        Openid = dic_openid["openid"];
                        Session[GlobalKey.BENZVOTEOPENID] = Openid;
                    }
                }
                if (string.IsNullOrEmpty(Openid))
                {
                    Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3A%2F%2Fbj.hongxu.cn%2Fweixin%2Fbenzvotepageinuse.aspx&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect");
                    Response.End();
                }

                #region 用户是否关注

                //string accesstoken = WeixinActs.Instance.GetAccessToken();
                //if (!string.IsNullOrEmpty(accesstoken))
                //{
                //    Dictionary<string, string> openinfo = WeixinActs.Instance.GetOpeninfo(accesstoken, Openid);
                //    if (openinfo.ContainsKey("subscribe"))
                //        subscribe = openinfo["subscribe"];
                //}

                #endregion

                LoadData();
            }
        }

        private void LoadData()
        {
            int id = GetInt("id");
            CurrentPothunterInfo = WeixinActs.Instance.GetBenzvotePothunterInfo(id,true);
            if (CurrentPothunterInfo == null)
            {
                Response.Write("<script>alert(\"选手信息错误\");location.href=\"" + (string.IsNullOrEmpty(FromUrl) ? "benzvotepageinuse.aspx" : FromUrl) + "\"</script>");
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