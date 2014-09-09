using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Hx.Tools.Web;
using Hx.Components;
using Hx.Tools;
using Hx.Components.Entity;
using System.Text.RegularExpressions;

namespace Hx.BackAdmin.HttpHandler
{
    public class WeixinActionHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        private HttpResponse Response;
        private HttpRequest Request;
        private string result = "{{\"Value\":\"{0}\",\"Msg\":\"{1}\"}}";
        private static object sync_helper = new object();
        public void ProcessRequest(HttpContext context)
        {
            Response = context.Response;
            Request = context.Request;

            string action = WebHelper.GetString("action");
            if (action == "weixindoact")
            {
                Doweixinact();
            }
            else if (action == "weixindianzan")
            {
                Doweixindianzan();
            }
            else
            {
                result = string.Format(result, "fail", "非法操作");
            }

            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        private void Doweixinact()
        {
            try
            {
                string openid = WebHelper.GetString("openid");

                if (!string.IsNullOrEmpty(openid))
                {
                    string access_token = MangaCache.Get(GlobalKey.WEIXINACCESS_TOKEN_KEY) as string;
                    if (string.IsNullOrEmpty(access_token))
                    {
                        string url_access_token = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}"
                            , GlobalKey.WEIXINAPPID
                            , GlobalKey.WEIXINSECRET);
                        string str_access_token = Http.GetPageByWebClientDefault(url_access_token);
                        Dictionary<string, string> dic_access_token = new Dictionary<string, string>();
                        try
                        {
                            dic_access_token = json.Deserialize<Dictionary<string, string>>(str_access_token);
                        }
                        catch { }
                        if (dic_access_token.ContainsKey("access_token"))
                        {
                            access_token = dic_access_token["access_token"];
                            int expires_in = 7200;
                            if (dic_access_token.ContainsKey("expires_in"))
                                expires_in = DataConvert.SafeInt(dic_access_token["expires_in"], 7200);
                            MangaCache.Add(GlobalKey.WEIXINACCESS_TOKEN_KEY, access_token, expires_in);
                        }
                    }
                    if (!string.IsNullOrEmpty(access_token))
                    {
                        string url_openinfo = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN"
                            , access_token
                            , openid);
                        string str_openinfo = Http.GetPageByWebClientUTF8(url_openinfo);
                        Dictionary<string, string> dic_openinfo = new Dictionary<string, string>();
                        try
                        {
                            dic_openinfo = json.Deserialize<Dictionary<string, string>>(str_openinfo);
                        }
                        catch { }
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            WeixinActInfo entity = new WeixinActInfo();
                            entity.Openid = openid;
                            entity.Nickname = dic_openinfo.ContainsKey("nickname") ? dic_openinfo["nickname"] : string.Empty;
                            entity.Sex = dic_openinfo.ContainsKey("sex") ? DataConvert.SafeByte(dic_openinfo["sex"]) : (byte)0;
                            entity.City = dic_openinfo.ContainsKey("city") ? dic_openinfo["city"] : string.Empty;
                            entity.Country = dic_openinfo.ContainsKey("country") ? dic_openinfo["country"] : string.Empty;
                            entity.Province = dic_openinfo.ContainsKey("province") ? dic_openinfo["province"] : string.Empty;
                            entity.Subscribetime = dic_openinfo.ContainsKey("subscribe_time") ? dic_openinfo["subscribe_time"] : string.Empty;

                            if (WeixinActs.Instance.Add(entity))
                                result = string.Format(result, "success", "");
                            else
                                result = string.Format(result, "fail", "数据添加失败");
                        }
                        else
                        {
                            result = string.Format(result, "fail", "用户信息获取失败");
                        }
                    }
                    else
                        result = string.Format(result, "fail", "access_token获取失败");
                }
                else
                {
                    result = string.Format(result, "fail", "openid为空");
                }
            }
            catch { result = string.Format(result, "fail", "执行失败"); }
        }

        private void Doweixindianzan()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string vopenid = WebHelper.GetString("vopenid");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(vopenid))
                {
                    int dianzancode = 0;
                    lock (sync_helper)
                    {
                        dianzancode = WeixinActs.Instance.Dianzan(openid, vopenid);
                    }
                    if (dianzancode == 0)
                        result = string.Format(result, "success", "");
                    else
                    {
                        string msg = string.Empty;
                        switch (dianzancode)
                        {
                            case 1:
                                msg = "您已经为他点过赞啦，不能重复赞哦";
                                break;
                            case 2:
                                msg = "数据发生错误";
                                break;
                            default:
                                break;
                        }

                        result = string.Format(result, "fail", msg);
                    }
                }
                else
                {
                    result = string.Format(result, "fail", "openid,vopenid为空");
                }
            }
            catch { result = string.Format(result, "fail", "执行失败"); }
        }
    }
}