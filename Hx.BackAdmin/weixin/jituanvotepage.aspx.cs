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

namespace Hx.BackAdmin.weixin
{
    public partial class jituanvotepage : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Code { get; set; }
        public string Openid { get; set; }
        protected int PageIndex = 1;
        protected int PageSize = 10;
        protected int PageCount = 1;
        private JituanvoteSettingInfo currentsetting = null;
        protected JituanvoteSettingInfo CurrentSetting
        {
            get
            {
                if (currentsetting == null)
                    currentsetting = WeixinActs.Instance.GetJituanvoteSetting(true);
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
                    if (key != "page" && key != "openid")
                        querys.Add(key + "=" + Request.QueryString[key]);
                }
                querys.Add("page=" + prev);
                querys.Add("openid=" + Openid);
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
                    if (key != "page" && key != "openid")
                        querys.Add(key + "=" + Request.QueryString[key]);
                }
                querys.Add("page=" + next);
                querys.Add("openid=" + Openid);
                return "?" + string.Join("&", querys); ;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Openid = GetString("openid");
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
                        Response.Redirect("jituanvotepage.aspx?openid=" + Openid + "&code=" + Code);
                        Response.End();
                    }
                }

                LoadData();
            }
        }

        private void LoadData()
        {
            List<JituanvotePothunterInfo> plist = WeixinActs.Instance.GetJituanvotePothunterList(true);
            rptData.DataSource = plist;
            rptData.DataBind();
        }

        /// <summary>
        /// 登录页面不需要验证
        /// </summary>
        protected override void Check()
        {

        }
    }
}