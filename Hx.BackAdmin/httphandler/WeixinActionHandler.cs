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
using Hx.Components.Enumerations;
using System.Text;

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
            else if (action == "benzvotetoupiao")
            {
                Benzvotetoupiao();
            }
            else if (action == "jituanvotetoupiao")
            {
                Jituanvotetoupiao();
            }
            else if (action == "comment")
            {
                Comment();
            }
            else if (action == "commentpraise")
            {
                CommentPraise();
            }
            else if (action == "commentbelittle")
            {
                CommentBelittle();
            }
            else if (action == "getcommentmore")
            {
                GetCommentMore();
            }
            else
            {
                result = string.Format(result, "fail", "非法操作");
            }

            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        #region 微信测试

        private void Doweixinact()
        {
            try
            {
                string openid = WebHelper.GetString("openid");

                if (!string.IsNullOrEmpty(openid))
                {
                    string access_token = WeixinActs.Instance.GetAccessToken();
                    if (!string.IsNullOrEmpty(access_token))
                    {
                        Dictionary<string, string> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
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

        #endregion

        #region 奔驰投票活动

        private void Benzvotetoupiao()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVote(openid, id);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "fail", votecheckresult);
                        return;
                    }

                    string access_token = WeixinActs.Instance.GetAccessToken();

                    if (!string.IsNullOrEmpty(access_token))
                    {
                        Dictionary<string, string> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            int pid = DataConvert.SafeInt(id);
                            BenzvotePothunterInfo pinfo = WeixinActs.Instance.GetBenzvotePothunterInfo(pid, true);
                            if (pinfo == null)
                                result = string.Format(result, "fail", "不存在此选手");
                            else
                            {
                                BenzvoteInfo entity = new BenzvoteInfo();
                                entity.AthleteID = pid;
                                entity.AthleteName = pinfo.Name;
                                entity.SerialNumber = pinfo.SerialNumber;
                                entity.Voter = dic_openinfo.ContainsKey("nickname") ? dic_openinfo["nickname"] : string.Empty;
                                entity.AddTime = DateTime.Now;
                                entity.Openid = openid;
                                entity.Nickname = entity.Voter;
                                entity.Sex = dic_openinfo.ContainsKey("sex") ? DataConvert.SafeInt(dic_openinfo["sex"]) : 0;
                                entity.City = dic_openinfo.ContainsKey("city") ? dic_openinfo["city"] : string.Empty;
                                entity.Country = dic_openinfo.ContainsKey("country") ? dic_openinfo["country"] : string.Empty;
                                entity.Province = dic_openinfo.ContainsKey("province") ? dic_openinfo["province"] : string.Empty;

                                string dianzancode = string.Empty;
                                lock (sync_helper)
                                {
                                    dianzancode = WeixinActs.Instance.Benzvote(entity);
                                }
                                if (string.IsNullOrEmpty(dianzancode))
                                    result = string.Format(result, "success", "");
                                else
                                    result = string.Format(result, "fail", dianzancode);
                            }
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
                    result = string.Format(result, "fail", "openid,vopenid为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "fail", "执行失败");
            }
        }

        #endregion

        #region 集团投票活动

        private void Jituanvotetoupiao()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckJituanVote(openid, id);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "fail", votecheckresult);
                        return;
                    }

                    string access_token = WeixinActs.Instance.GetAccessToken();

                    if (!string.IsNullOrEmpty(access_token))
                    {
                        Dictionary<string, string> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            int pid = DataConvert.SafeInt(id);
                            JituanvotePothunterInfo pinfo = WeixinActs.Instance.GetJituanvotePothunterInfo(pid, true);
                            if (pinfo == null)
                                result = string.Format(result, "fail", "不存在此选手");
                            else
                            {
                                JituanvoteInfo entity = new JituanvoteInfo();
                                entity.AthleteID = pid;
                                entity.AthleteName = pinfo.Name;
                                entity.SerialNumber = pinfo.SerialNumber;
                                entity.Voter = dic_openinfo.ContainsKey("nickname") ? dic_openinfo["nickname"] : string.Empty;
                                entity.AddTime = DateTime.Now;
                                entity.Openid = openid;
                                entity.Nickname = entity.Voter;
                                entity.Sex = dic_openinfo.ContainsKey("sex") ? DataConvert.SafeInt(dic_openinfo["sex"]) : 0;
                                entity.City = dic_openinfo.ContainsKey("city") ? dic_openinfo["city"] : string.Empty;
                                entity.Country = dic_openinfo.ContainsKey("country") ? dic_openinfo["country"] : string.Empty;
                                entity.Province = dic_openinfo.ContainsKey("province") ? dic_openinfo["province"] : string.Empty;

                                string dianzancode = string.Empty;
                                lock (sync_helper)
                                {
                                    dianzancode = WeixinActs.Instance.Jituanvote(entity);
                                }
                                if (string.IsNullOrEmpty(dianzancode))
                                    result = string.Format(result, "success", "");
                                else
                                    result = string.Format(result, "fail", dianzancode);
                            }
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
                    result = string.Format(result, "fail", "openid,vopenid为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "fail", "执行失败");
            }
        }

        #endregion

        #region 评论

        private void Comment()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                string comment = WebHelper.GetString("comment");
                int acttype = WebHelper.GetInt("acttype");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(comment))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment((WeixinActType)acttype);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "fail", votecheckresult);
                        return;
                    }

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
                            int pid = DataConvert.SafeInt(id);
                            JituanvotePothunterInfo pinfo = WeixinActs.Instance.GetJituanvotePothunterInfo(pid, true);
                            if (pinfo == null)
                                result = string.Format(result, "fail", "不存在此选手");
                            else
                            {
                                string commenter = dic_openinfo.ContainsKey("nickname") ? dic_openinfo["nickname"] : string.Empty;
                                WeixinActCommentInfo entity = new WeixinActCommentInfo()
                                {
                                    WeixinActType = (WeixinActType)acttype,
                                    AthleteID = pid,
                                    Commenter = commenter,
                                    PraiseNum = 0,
                                    BelittleNum = 0,
                                    Comment = comment,
                                    AddTime = DateTime.Now
                                };

                                string rcode = string.Empty;
                                lock (sync_helper)
                                {
                                    rcode = WeixinActs.Instance.CommentPost(entity);
                                }
                                if (string.IsNullOrEmpty(rcode))
                                    result = string.Format(result, "success", entity.ID + "," + (string.IsNullOrEmpty(commenter) ? "匿名" : commenter));
                                else
                                    result = string.Format(result, "fail", rcode);
                            }
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
                    result = string.Format(result, "fail", "openid,vopenid,chat为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "fail", "执行失败");
            }

        }

        private void CommentPraise()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                int acttype = WebHelper.GetInt("acttype");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment((WeixinActType)acttype);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "fail", votecheckresult);
                        return;
                    }

                    string rcode = string.Empty;
                    lock (sync_helper)
                    {
                        rcode = WeixinActs.Instance.CommentPraise(DataConvert.SafeInt(id));
                    }
                    if (string.IsNullOrEmpty(rcode))
                        result = string.Format(result, "success", string.Empty);
                    else
                        result = string.Format(result, "fail", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "fail", "执行失败");
            }
        }

        private void CommentBelittle()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                int acttype = WebHelper.GetInt("acttype");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment((WeixinActType)acttype);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "fail", votecheckresult);
                        return;
                    }

                    string rcode = string.Empty;
                    lock (sync_helper)
                    {
                        rcode = WeixinActs.Instance.CommentBelittle(DataConvert.SafeInt(id));
                    }
                    if (string.IsNullOrEmpty(rcode))
                        result = string.Format(result, "success", string.Empty);
                    else
                        result = string.Format(result, "fail", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "fail", "执行失败");
            }
        }

        private void GetCommentMore()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                int acttype = WebHelper.GetInt("acttype");
                int pageindex = WebHelper.GetInt("pageindex");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment((WeixinActType)acttype);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "fail", votecheckresult);
                        return;
                    }

                    string rcode = string.Empty;
                    StringBuilder htmlstr = new StringBuilder();
                    try
                    {
                        List<WeixinActCommentInfo> listcomment = WeixinActs.Instance.GetComments(true);
                        List<WeixinActCommentInfo> source = listcomment.FindAll(c => c.AthleteID == DataConvert.SafeInt(id) && c.WeixinActType == (WeixinActType)acttype).ToList().OrderByDescending(c => c.ID).ToList();
                        int skipcount = 8 * (pageindex - 1);
                        if (source.Count > 2 && source.Count > (skipcount + 2))
                        {
                            source = source.Skip(2).ToList().OrderBy(c => c.ID).ToList();
                            source = source.Skip(skipcount).ToList();
                            source = source.Count > 8 ? source.Take(8).ToList() : source;
                        }
                        foreach (WeixinActCommentInfo entity in source)
                        {
                            htmlstr.Append("<tr>");
                            htmlstr.Append("<td><p>" + entity.Comment + "</p>");
                            htmlstr.Append("<div class='dvcommentinfo'>");
                            htmlstr.Append("<span>" + entity.AddTime.ToString("yyyy-MM-dd HH:mm:ss") + "</span> <span>");
                            htmlstr.Append((string.IsNullOrEmpty(entity.Commenter) ? "匿名" : entity.Commenter) + "</span></div>");
                            htmlstr.Append("<div class='dvcommentopt'>");
                            htmlstr.Append("<a href='javascript:void(0);' class='btnPraise' val='" + entity.ID + "'>鲜花</a>(<span");
                            htmlstr.Append(" id='spPraise" + entity.ID + "'>" + entity.PraiseNum + "</span>) <a href='javascript:void(0);'");
                            htmlstr.Append(" class='btnBelittle' val='" + entity.ID + "'>鸡蛋</a>(<span id='spBelittle" + entity.ID + "'>" + entity.BelittleNum + "</span>)");
                            htmlstr.Append("</div></td></tr>");
                        }
                    }
                    catch
                    {
                        rcode = "发生错误";
                    }
                    if (string.IsNullOrEmpty(rcode))
                        result = string.Format(result, "success", htmlstr.ToString());
                    else
                        result = string.Format(result, "fail", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "fail", "执行失败");
            }
        }

        #endregion

    }
}