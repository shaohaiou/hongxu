using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Tools.Web;
using Hx.Components.Entity;
using Hx.Components;
using System.Text;

namespace Hx.BackAdmin.weixin
{
    public partial class act : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Code { get; set; }
        public string State { get; set; }
        public string Openid { get; set; }
        public int ActValue { get; set; }
        public WeixinActInfo ActInfo { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Code = GetString("code");
                State = GetString("state");

                if (!string.IsNullOrEmpty(Code))
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

                        ActInfo = WeixinActs.Instance.GetModel(Openid);
                        if (ActInfo != null)
                        {
                            ActValue = ActInfo.AtcValue;
                        }
                    }
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