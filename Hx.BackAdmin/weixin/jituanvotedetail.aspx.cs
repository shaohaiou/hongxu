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

namespace Hx.BackAdmin.weixin
{
    public partial class jituanvotedetail : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        public string Openid { get; set; }
        public string Code { get; set; }
        protected JituanvotePothunterInfo CurrentPothunterInfo = null;
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
        private static object synchelper = new object();
        private List<WeixinActCommentInfo> _listcomment = null;
        public List<WeixinActCommentInfo> ListComment
        {
            get
            {
                if (_listcomment == null)
                {
                    lock (synchelper)
                    {
                        _listcomment = WeixinActs.Instance.GetComments(true);
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
                Code = GetString("code");
                Openid = GetString("openid");
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
                    }
                    else
                    {
                        Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3A%2F%2Fbj.hongxu.cn%2Fweixin%2Fjituanvotedetail.aspx%3fid=" + GetInt("id") +"&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect");
                        Response.End();
                    }
                }

                LoadData();
            }
        }

        private void LoadData()
        {
            CommentPageCount = 1;
            int id = GetInt("id");
            CurrentPothunterInfo = WeixinActs.Instance.GetJituanvotePothunterInfo(id, true);
            if (CurrentPothunterInfo == null)
            {
                Response.Write("<script>alert(\"员工信息错误\");location.href=\"" + (string.IsNullOrEmpty(FromUrl) ? ("jituanvotepage.aspx?openid=" + Openid + "&code=" + Code) : FromUrl) + "\"</script>");
                Response.End();
            }

            List<WeixinActCommentInfo> source = ListComment.FindAll(c => c.AthleteID == CurrentPothunterInfo.ID && c.WeixinActType == Components.Enumerations.WeixinActType.集团活动).ToList().OrderByDescending(c => c.ID).ToList();

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