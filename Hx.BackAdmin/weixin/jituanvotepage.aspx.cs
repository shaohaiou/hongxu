﻿using System;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Openid = GetString("openid");
                Code = GetString("code");
                int isback = GetInt("isback");
                if (!string.IsNullOrEmpty(Code) && isback == 0)
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
#if !DEBUG
                        Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3A%2F%2Fbj.hongxu.cn%2Fweixin%2Fjituanvotepage.aspx&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect");
                        Response.End();
#endif
                    }
                }
                else if (string.IsNullOrEmpty(Code))
                {
                    Response.Redirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx0c9b37c9d5ddf8a8&redirect_uri=http%3A%2F%2Fbj.hongxu.cn%2Fweixin%2Fjituanvotepage.aspx&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect");
                    Response.End();
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

        protected void rptData_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                JituanvotePothunterInfo entity = (JituanvotePothunterInfo)e.Item.DataItem;
                Repeater rptCommentFirstOne = (Repeater)e.Item.FindControl("rptCommentFirstOne");
                Repeater rptCommentMore = (Repeater)e.Item.FindControl("rptCommentMore");
                List<WeixinActCommentInfo> source = ListComment.FindAll(c => c.AthleteID == entity.ID && c.WeixinActType == Components.Enumerations.WeixinActType.集团活动).ToList().OrderByDescending(c => c.ID).ToList();
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