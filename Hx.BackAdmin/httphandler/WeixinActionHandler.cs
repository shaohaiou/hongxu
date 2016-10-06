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
            else if (action == "carddraw")
            {
                Carddraw();
            }
            else if (action == "cardpull")
            {
                CardPull();
            }
            else if (action == "votetoupiao")
            {
                Votetoupiao();
            }
            else if (action == "votecomment")
            {
                VoteComment();
            }
            else if (action == "votecommentpraise")
            {
                VoteCommentPraise();
            }
            else if (action == "votecommentbelittle")
            {
                VoteCommentBelittle();
            }
            else if (action == "getvotecommentmore")
            {
                GetVoteCommentMore();
            }
            else if (action == "gb61")
            {
                GB61();
            }
            else
            {
                result = string.Format(result, "failed", "非法操作");
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
                        Dictionary<string, object> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            WeixinActInfo entity = new WeixinActInfo();
                            entity.Openid = openid;
                            entity.Nickname = dic_openinfo.ContainsKey("nickname") ? (dic_openinfo["nickname"].ToString()) : string.Empty;
                            entity.Sex = dic_openinfo.ContainsKey("sex") ? DataConvert.SafeByte(dic_openinfo["sex"].ToString()) : (byte)0;
                            entity.City = dic_openinfo.ContainsKey("city") ? (dic_openinfo["city"].ToString()) : string.Empty;
                            entity.Country = dic_openinfo.ContainsKey("country") ? (dic_openinfo["country"].ToString()) : string.Empty;
                            entity.Province = dic_openinfo.ContainsKey("province") ? (dic_openinfo["province"].ToString()) : string.Empty;
                            entity.Subscribetime = dic_openinfo.ContainsKey("subscribe_time") ? (dic_openinfo["subscribe_time"].ToString()) : string.Empty;

                            if (WeixinActs.Instance.Add(entity))
                                result = string.Format(result, "success", "");
                            else
                                result = string.Format(result, "failed", "数据添加失败");
                        }
                        else
                        {
                            result = string.Format(result, "failed", "用户信息获取失败");
                        }
                    }
                    else
                        result = string.Format(result, "failed", "access_token获取失败");
                }
                else
                {
                    result = string.Format(result, "failed", "openid为空");
                }
            }
            catch { result = string.Format(result, "failed", "执行失败"); }
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

                        result = string.Format(result, "failed", msg);
                    }
                }
                else
                {
                    result = string.Format(result, "failed", "openid,vopenid为空");
                }
            }
            catch { result = string.Format(result, "failed", "执行失败"); }
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
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    string access_token = WeixinActs.Instance.GetAccessToken();

                    if (!string.IsNullOrEmpty(access_token))
                    {
                        Dictionary<string, object> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            int pid = DataConvert.SafeInt(id);
                            BenzvotePothunterInfo pinfo = WeixinActs.Instance.GetBenzvotePothunterInfo(pid, true);
                            if (pinfo == null)
                                result = string.Format(result, "failed", "不存在此选手");
                            else
                            {
                                BenzvoteInfo entity = new BenzvoteInfo();
                                entity.AthleteID = pid;
                                entity.AthleteName = pinfo.Name;
                                entity.SerialNumber = pinfo.SerialNumber;
                                entity.Voter = dic_openinfo.ContainsKey("nickname") ? (dic_openinfo["nickname"].ToString()) : string.Empty;
                                entity.AddTime = DateTime.Now;
                                entity.Openid = openid;
                                entity.Nickname = entity.Voter;
                                entity.Sex = dic_openinfo.ContainsKey("sex") ? DataConvert.SafeInt(dic_openinfo["sex"]) : 0;
                                entity.City = dic_openinfo.ContainsKey("city") ? (dic_openinfo["city"].ToString()) : string.Empty;
                                entity.Country = dic_openinfo.ContainsKey("country") ? (dic_openinfo["country"].ToString()) : string.Empty;
                                entity.Province = dic_openinfo.ContainsKey("province") ? (dic_openinfo["province"].ToString()) : string.Empty;

                                string dianzancode = string.Empty;
                                lock (sync_helper)
                                {
                                    dianzancode = WeixinActs.Instance.Benzvote(entity);
                                }
                                if (string.IsNullOrEmpty(dianzancode))
                                    result = string.Format(result, "success", "");
                                else
                                    result = string.Format(result, "failed", dianzancode);
                            }
                        }
                        else
                        {
                            result = string.Format(result, "failed", "用户信息获取失败");
                        }
                    }
                    else
                        result = string.Format(result, "failed", "access_token获取失败");
                }
                else
                {
                    result = string.Format(result, "failed", "openid,vopenid为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
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
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    string access_token = WeixinActs.Instance.GetAccessToken();

                    if (!string.IsNullOrEmpty(access_token))
                    {
                        Dictionary<string, object> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            int pid = DataConvert.SafeInt(id);
                            JituanvotePothunterInfo pinfo = WeixinActs.Instance.GetJituanvotePothunterInfo(pid, true);
                            if (pinfo == null)
                                result = string.Format(result, "failed", "不存在此选手");
                            else
                            {
                                JituanvoteInfo entity = new JituanvoteInfo();
                                entity.AthleteID = pid;
                                entity.AthleteName = pinfo.Name;
                                entity.SerialNumber = pinfo.SerialNumber;
                                entity.Voter = dic_openinfo.ContainsKey("nickname") ? (dic_openinfo["nickname"].ToString()) : string.Empty;
                                entity.AddTime = DateTime.Now;
                                entity.Openid = openid;
                                entity.Nickname = entity.Voter;
                                entity.Sex = dic_openinfo.ContainsKey("sex") ? DataConvert.SafeInt(dic_openinfo["sex"]) : 0;
                                entity.City = dic_openinfo.ContainsKey("city") ? (dic_openinfo["city"].ToString()) : string.Empty;
                                entity.Country = dic_openinfo.ContainsKey("country") ? (dic_openinfo["country"].ToString()) : string.Empty;
                                entity.Province = dic_openinfo.ContainsKey("province") ? (dic_openinfo["province"].ToString()) : string.Empty;

                                string dianzancode = string.Empty;
                                lock (sync_helper)
                                {
                                    dianzancode = WeixinActs.Instance.Jituanvote(entity);
                                }
                                if (string.IsNullOrEmpty(dianzancode))
                                    result = string.Format(result, "success", "");
                                else
                                    result = string.Format(result, "failed", dianzancode);
                            }
                        }
                        else
                        {
                            result = string.Format(result, "failed", "用户信息获取失败");
                        }
                    }
                    else
                        result = string.Format(result, "failed", "access_token获取失败");
                }
                else
                {
                    result = string.Format(result, "failed", "openid,vopenid为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        #endregion

        #region 评论

        private void Comment()
        {
            try
            {
                result = string.Format(result, "failed", "为减轻服务器压力，评论功能已被关闭");
                return;
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                string comment = WebHelper.GetString("comment");
                int acttype = WebHelper.GetInt("acttype");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(comment))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment((WeixinActType)acttype);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    string access_token = WeixinActs.Instance.GetAccessToken();

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
                                result = string.Format(result, "failed", "不存在此选手");
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
                                    result = string.Format(result, "failed", rcode);
                            }
                        }
                        else
                        {
                            result = string.Format(result, "failed", "用户信息获取失败");
                        }
                    }
                    else
                        result = string.Format(result, "failed", "access_token获取失败");
                }
                else
                {
                    result = string.Format(result, "failed", "openid,vopenid,chat为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
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
                        result = string.Format(result, "failed", votecheckresult);
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
                        result = string.Format(result, "failed", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
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
                        result = string.Format(result, "failed", votecheckresult);
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
                        result = string.Format(result, "failed", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
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
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    string rcode = string.Empty;
                    StringBuilder htmlstr = new StringBuilder();
                    try
                    {
                        List<WeixinActCommentInfo> listcomment = WeixinActs.Instance.GetComments(true);
                        List<WeixinActCommentInfo> source = listcomment.FindAll(c => c.AthleteID == DataConvert.SafeInt(id) && c.WeixinActType == (WeixinActType)acttype).ToList().OrderByDescending(c => c.ID).ToList();
                        int skipcount = 8 * (pageindex - 1);
                        if (source.Count > skipcount)
                        {
                            source = source.OrderBy(c => c.ID).ToList().Skip(skipcount).ToList();
                            source = source.Count > 8 ? source.Take(8).ToList() : source;
                        }
                        foreach (WeixinActCommentInfo entity in source)
                        {
                            htmlstr.Append("<tr>");
                            htmlstr.Append("<td><p>" + entity.Comment.Replace("\r", " ").Replace("\n", " ").Replace("\"", " ") + "</p>");
                            htmlstr.Append("<div class='dvcommentinfo'>");
                            htmlstr.Append("<span>" + entity.AddTime.ToString("HH:mm:ss") + "</span> <span>");
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
                        result = string.Format(result, "failed", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        #endregion

        #region 卡券活动
        private static object sync_card = new object();

        /// <summary>
        /// 拉取卡券
        /// </summary>
        private void Carddraw()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                int sid = WebHelper.GetInt("sid");

                if (!string.IsNullOrEmpty(openid))
                {
                    CardSettingInfo setting = WeixinActs.Instance.GetCardSetting(sid,true);
                    if (setting != null && setting.Switch == 1)
                    {
                        string accesstoken = WeixinActs.Instance.GetAccessToken(setting.AppID, setting.AppSecret);
                        Dictionary<string, object> openinfo = WeixinActs.Instance.GetOpeninfo(accesstoken, openid);
                        if (setting.MustAttention == 1 && (!openinfo.Keys.Contains("subscribe") || openinfo["subscribe"].ToString() == "0"))
                        {
                            result = string.Format(result, "attention", string.Format("{0}|{1}", setting.AppName, setting.AppNumber));
                            return;
                        }
                        List<CardPullRecordInfo> listrecord = WeixinActs.Instance.GetCardPullRecordList(sid,true);
                        if (listrecord.Exists(l => l.Openid == openid && (l.PullResult == "2" || l.PullResult == "0")))
                        {
                            result = string.Format(result, "failed", "每个微信号只能参与一次抽奖！");
                            return;
                        }
                        lock (sync_card)
                        {
                            List<CardpackInfo> cardlist = WeixinActs.Instance.GetCardlist(sid);
                            if (cardlist == null || (cardlist != null && cardlist.Count == 0))
                            {
                                result = string.Format(result, "success", "0");
                            }
                            else
                            {
                                Dictionary<int, string> cardids = new Dictionary<int, string>();
                                List<CardidInfo> cardidinfolist = WeixinActs.Instance.GetCardidInfolist(sid,true);
                                string apiticket = WeixinActs.Instance.GetCardapiTicket(setting.AppID, setting.AppSecret);
                                int timestamp = Utils.ConvertDateTimeInt(DateTime.Now);

                                int i = 0;
                                foreach (CardpackInfo cardpack in cardlist)
                                {
                                    if (cardpack.card.base_info != null && cardidinfolist.Exists(c => c.Cardid == cardpack.card.base_info.id))
                                    {
                                        int kuc = cardidinfolist.Find(c => c.Cardid == cardpack.card.base_info.id).Num;
                                        string imgurl = cardidinfolist.Find(c => c.Cardid == cardpack.card.base_info.id).ImgUrl;
                                        for (int j = 0; j < kuc; j++)
                                        {
                                            List<string> signaturevalues = new List<string>() 
                                            { 
                                                timestamp.ToString(),
                                                apiticket.ToString(),
                                                cardpack.card.base_info.id
                                            };
                                            signaturevalues = signaturevalues.OrderBy(s => s.Substring(0,1)).ToList();
                                            string signature = EncryptString.SHA1_Hash(string.Format("{0}{1}{2}", signaturevalues[0], signaturevalues[1], signaturevalues[2]));
                                            string awardname = cardidinfolist.Find(c => c.Cardid == cardpack.card.base_info.id).Award;
                                            cardids.Add(i, cardpack.card.base_info.id + "|" + cardpack.card.base_info.title + "|" + awardname + "|" + imgurl + "|" + timestamp + "|" + signature);
                                            i++;
                                        }
                                    }
                                }

                                if (cardids.Count == 0)
                                {
                                    result = string.Format(result, "success", "0");
                                }
                                else
                                {
                                    if (listrecord.Exists(l => l.Openid == openid && l.SID == sid && l.PullResult == "1"))
                                    {
                                        CardPullRecordInfo record = listrecord.Find(l => l.Openid == openid && l.SID == sid && l.PullResult == "1");
                                        List<string> signaturevalues = new List<string>() 
                                        { 
                                            timestamp.ToString(),
                                            apiticket.ToString(),
                                            record.Cardid
                                        };
                                        signaturevalues = signaturevalues.OrderBy(s => s.Substring(0,1)).ToList();
                                        string signature = EncryptString.SHA1_Hash(string.Format("{0}{1}{2}", signaturevalues[0], signaturevalues[1], signaturevalues[2]));
                                        result = string.Format(result, "success", record.Cardid + "|" + record.Cardtitle + "|" + record.Cardawardname + "|" + record.Cardlogourl + "|" + timestamp + "|" + signature);
                                    }
                                    else
                                    {
                                        string username = string.Empty;
                                        if (openinfo.ContainsKey("nickname"))
                                            username = (openinfo["nickname"].ToString());

                                        int maxrange = cardids.Count * 100 / setting.WinRate;
                                        Random r = new Random(DataConvert.SafeInt(DateTime.Now.ToString("ddmm") + DateTime.Now.Millisecond.ToString()));
                                        int index = r.Next(maxrange);
                                        if (index > cardids.Count - 1)
                                        {
                                            CardPullRecordInfo pullrecord = new CardPullRecordInfo()
                                            {
                                                SID = sid,
                                                Openid = openid,
                                                UserName = username,
                                                Cardid = string.Empty,
                                                Cardtitle = string.Empty,
                                                Cardawardname = string.Empty,
                                                Cardlogourl = string.Empty,
                                                PullResult = "0",
                                                AddTime = DateTime.Now
                                            };
                                            WeixinActs.Instance.AddCardPullRecord(pullrecord);
                                            listrecord.Add(pullrecord);
                                            result = string.Format(result, "success", "0");
                                        }
                                        else
                                        {
                                            CardPullRecordInfo pullrecord = new CardPullRecordInfo()
                                            {
                                                SID = sid,
                                                Openid = openid,
                                                UserName = username,
                                                Cardid = cardids[index].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0],
                                                Cardtitle = cardids[index].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[1],
                                                Cardawardname = cardids[index].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[2],
                                                Cardlogourl = cardids[index].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[3],
                                                PullResult = "1",
                                                AddTime = DateTime.Now
                                            };
                                            WeixinActs.Instance.AddCardPullRecord(pullrecord);
                                            listrecord.Add(pullrecord);
                                            CardidInfo cardidinfo = cardidinfolist.Find(c => c.Cardid == pullrecord.Cardid);
                                            cardidinfo.Num--;
                                            WeixinActs.Instance.UpdateCardidInfo(cardidinfo);
                                            WeixinActs.Instance.ReloadCardidListCache(sid);
                                            result = string.Format(result, "success", cardids[index]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                        result = string.Format(result, "failed", "该活动未开始,敬请期待。");
                }
                else
                {
                    result = string.Format(result, "failed", "openid为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        /// <summary>
        /// 领取卡券
        /// </summary>
        private void CardPull()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                int sid = WebHelper.GetInt("sid");

                if (!string.IsNullOrEmpty(openid))
                {
                    CardSettingInfo setting = WeixinActs.Instance.GetCardSetting(sid,true);
                    if (setting != null && setting.Switch == 1)
                    {
                        List<CardPullRecordInfo> listrecord = WeixinActs.Instance.GetCardPullRecordList(sid,true);
                        if (listrecord.Exists(l => l.Openid == openid && l.SID == sid))
                        {
                            WeixinActs.Instance.PullCard(listrecord.Find(l => l.Openid == openid && l.SID == sid).Openid, sid);
                            listrecord.Find(l => l.Openid == openid).PullResult = "2";
                            result = string.Format(result, "success", "1");
                        }
                    }
                    else
                        result = string.Format(result, "failed", "该活动未开始,敬请期待。");
                }
                else
                {
                    result = string.Format(result, "failed", "openid为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        #endregion

        #region 投票活动 

        private void Votetoupiao()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string ids = WebHelper.GetString("id");
                int sid = WebHelper.GetInt("sid");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(ids))
                {
                    VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(sid, true);
                    foreach (string id in ids.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string votecheckresult = WeixinActs.Instance.CheckVote(sid, openid, id);
                        if (!string.IsNullOrEmpty(votecheckresult))
                        {
                            result = string.Format(result, "failed", votecheckresult);
                            return;
                        }
                    }

                    string access_token = WeixinActs.Instance.GetAccessToken(setting.AppID, setting.AppSecret);

                    if (setting.MustAttention == 1)
                    {
                        Dictionary<string, object> openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!openinfo.Keys.Contains("subscribe") || openinfo["subscribe"].ToString() == "0")
                        {
                            result = string.Format(result, "failed", "请先关注");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(access_token))
                    {
                        Dictionary<string, object> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            foreach (string id in ids.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                int pid = DataConvert.SafeInt(id);
                                VotePothunterInfo pinfo = WeixinActs.Instance.GetVotePothunterInfo(pid, true);
                                if (pinfo == null)
                                    result = string.Format(result, "failed", "不存在此选手");
                                else
                                {
                                    VoteRecordInfo entity = new VoteRecordInfo();
                                    entity.SID = sid;
                                    entity.AthleteID = pid;
                                    entity.AthleteName = pinfo.Name;
                                    entity.SerialNumber = pinfo.SerialNumber;
                                    entity.Voter = dic_openinfo.ContainsKey("nickname") ? (dic_openinfo["nickname"].ToString()) : string.Empty;
                                    entity.AddTime = DateTime.Now;
                                    entity.Openid = openid;
                                    entity.Nickname = entity.Voter;
                                    entity.Sex = dic_openinfo.ContainsKey("sex") ? DataConvert.SafeInt(dic_openinfo["sex"]) : 0;
                                    entity.City = dic_openinfo.ContainsKey("city") ? (dic_openinfo["city"].ToString()) : string.Empty;
                                    entity.Country = dic_openinfo.ContainsKey("country") ? (dic_openinfo["country"].ToString()) : string.Empty;
                                    entity.Province = dic_openinfo.ContainsKey("province") ? (dic_openinfo["province"].ToString()) : string.Empty;

                                    string dianzancode = string.Empty;
                                    lock (sync_helper)
                                    {
                                        dianzancode = WeixinActs.Instance.Vote(entity);
                                    }
                                    if (!string.IsNullOrEmpty(dianzancode))
                                        result = string.Format(result, "failed", dianzancode);
                                }
                            }
                            result = string.Format(result, "success", "");
                        }
                        else
                        {
                            result = string.Format(result, "failed", "用户信息获取失败");
                        }
                    }
                    else
                        result = string.Format(result, "failed", "access_token获取失败");
                }
                else
                {
                    result = string.Format(result, "failed", "openid,vopenid为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        #region 评论

        private void VoteComment()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                int sid = WebHelper.GetInt("sid");
                string comment = WebHelper.GetString("comment");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(comment))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment(sid);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    VoteSettingInfo setting = WeixinActs.Instance.GetVoteSetting(sid, true);
                    string access_token = WeixinActs.Instance.GetAccessToken(setting.AppID, setting.AppSecret);
                    if (setting.MustAttention == 1)
                    {
                        Dictionary<string, object> openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!openinfo.Keys.Contains("subscribe") || openinfo["subscribe"].ToString() == "0")
                        {
                            result = string.Format(result, "failed", "请先关注");
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(access_token))
                    {
                        Dictionary<string, object> dic_openinfo = WeixinActs.Instance.GetOpeninfo(access_token, openid);
                        if (!dic_openinfo.ContainsKey("errcode"))
                        {
                            int pid = DataConvert.SafeInt(id);
                            VotePothunterInfo pinfo = WeixinActs.Instance.GetVotePothunterInfo(pid, true);
                            if (pinfo == null)
                                result = string.Format(result, "failed", "不存在此选手");
                            else
                            {
                                string commenter = dic_openinfo.ContainsKey("nickname") ? (dic_openinfo["nickname"].ToString()) : string.Empty;
                                VoteCommentInfo entity = new VoteCommentInfo()
                                {
                                    AthleteID = pid,
                                    Commenter = commenter,
                                    PraiseNum = 0,
                                    BelittleNum = 0,
                                    Comment = comment,
                                    AddTime = DateTime.Now,
                                    CheckStatus = 0
                                };

                                string rcode = string.Empty;
                                lock (sync_helper)
                                {
                                    rcode = WeixinActs.Instance.VoteCommentPost(entity);
                                }
                                if (string.IsNullOrEmpty(rcode))
                                    result = string.Format(result, "success", entity.ID + "," + (string.IsNullOrEmpty(commenter) ? "匿名" : commenter));
                                else
                                    result = string.Format(result, "failed", rcode);
                            }
                        }
                        else
                        {
                            result = string.Format(result, "failed", "用户信息获取失败");
                        }
                    }
                    else
                        result = string.Format(result, "failed", "access_token获取失败");
                }
                else
                {
                    result = string.Format(result, "failed", "openid,vopenid,chat为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }

        }

        private void VoteCommentPraise()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                int sid = WebHelper.GetInt("sid");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment(sid);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    string rcode = string.Empty;
                    lock (sync_helper)
                    {
                        rcode = WeixinActs.Instance.VoteCommentPraise(DataConvert.SafeInt(id));
                    }
                    if (string.IsNullOrEmpty(rcode))
                        result = string.Format(result, "success", string.Empty);
                    else
                        result = string.Format(result, "failed", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        private void VoteCommentBelittle()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                int sid = WebHelper.GetInt("sid");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment(sid);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    string rcode = string.Empty;
                    lock (sync_helper)
                    {
                        rcode = WeixinActs.Instance.VoteCommentBelittle(DataConvert.SafeInt(id));
                    }
                    if (string.IsNullOrEmpty(rcode))
                        result = string.Format(result, "success", string.Empty);
                    else
                        result = string.Format(result, "failed", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        private void GetVoteCommentMore()
        {
            try
            {
                string openid = WebHelper.GetString("openid");
                string id = WebHelper.GetString("id");
                int sid = WebHelper.GetInt("sid");
                int pageindex = WebHelper.GetInt("pageindex");

                if (!string.IsNullOrEmpty(openid) && !string.IsNullOrEmpty(id))
                {
                    string votecheckresult = WeixinActs.Instance.CheckVoteComment(sid);
                    if (!string.IsNullOrEmpty(votecheckresult))
                    {
                        result = string.Format(result, "failed", votecheckresult);
                        return;
                    }

                    string rcode = string.Empty;
                    StringBuilder htmlstr = new StringBuilder();
                    try
                    {
                        List<VoteCommentInfo> listcomment = WeixinActs.Instance.GetVoteComments(DataConvert.SafeInt(id),true);
                        List<VoteCommentInfo> source = listcomment.FindAll(c=>c.CheckStatus == 1).OrderByDescending(c => c.ID).ToList();
                        int skipcount = 8 * (pageindex - 1);
                        if (source.Count > skipcount)
                        {
                            source = source.OrderBy(c => c.ID).ToList().Skip(skipcount).ToList();
                            source = source.Count > 8 ? source.Take(8).ToList() : source;
                        }
                        foreach (VoteCommentInfo entity in source)
                        {
                            htmlstr.Append("<tr>");
                            htmlstr.Append("<td><p>" + entity.Comment.Replace("\r", " ").Replace("\n", " ").Replace("\"", " ") + "</p>");
                            htmlstr.Append("<div class='dvcommentinfo'>");
                            htmlstr.Append("<span>" + entity.AddTime.ToString("HH:mm:ss") + "</span> <span>");
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
                        result = string.Format(result, "failed", rcode);
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        #endregion

        #endregion

        #region 广本61活动

        private void GB61()
        {
            try
            {
                string cname = WebHelper.GetString("cname");
                string phone = WebHelper.GetString("phone");
                string spec_name = WebHelper.GetString("spec_name");

                if (!string.IsNullOrEmpty(cname) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(spec_name))
                {
                    GB61Info info = new GB61Info() 
                    { 
                        CName = cname,
                        Phone = phone,
                        SpecName = spec_name,
                        Status = 0
                    };
                    WeixinActs.Instance.AddGB61Info(info);
                    result = string.Format(result, "success", "1");
                }
                else
                {
                    result = string.Format(result, "failed", "cname,phone,spec_name为空");
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                result = string.Format(result, "failed", "执行失败");
            }
        }

        #endregion

    }
}