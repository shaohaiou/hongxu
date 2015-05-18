using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Tools.Web;
using Hx.Components.Entity;
using Hx.Tools;

namespace Hx.BackAdmin.weixin
{
    public partial class cardpg : WeixinBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Openid { get; set; }
        public string Code { get; set; }
        public int SID { get; set; }
        private CardSettingInfo currentsetting = null;
        protected CardSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                    currentsetting = WeixinActs.Instance.GetCardSetting(SID,true);
                return currentsetting;
            }
        }
        private string signature = string.Empty;
        public new string Signature
        {
            get
            {
                if (string.IsNullOrEmpty(signature))
                    signature = EncryptString.SHA1_Hash(string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", WeixinActs.Instance.GetJsapiTicket(CurrentSetting.AppID, CurrentSetting.AppSecret), NonceStr, Timestamp, Request.Url.AbsoluteUri));
                return signature;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Code = GetString("code");
                SID = GetInt("sid",1);
                if (!string.IsNullOrEmpty(Code))
                {
                    Openid = WeixinActs.Instance.GetOpenid(CurrentSetting.AppID, CurrentSetting.AppSecret, Code);
                    //if (string.IsNullOrEmpty(Openid))
                    //{
                    //    Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + CurrentSetting.AppID + "&redirect_uri=http%3A%2F%2Frb.hongxu.cn%2Fweixin%2Fcardpg.aspx%3Fwechat_card_js=1&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect");
                    //    Response.End();
                    //}
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