﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;
using Hx.Components.Query;
using Hx.Tools;
using Hx.Tools.Web;
using System.Web;
using Hx.Components.Enumerations;

namespace Hx.Components
{
    public class WeixinActs
    {
        #region 单例
        private static object sync_creater = new object();

        private static WeixinActs _instance;
        public static WeixinActs Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new WeixinActs();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 公用方法

        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        /// <summary>
        /// 获取微信通信密钥
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken(string appid = "", string secret = "")
        {
            if (string.IsNullOrEmpty(appid)) appid = GlobalKey.WEIXINAPPID;
            if (string.IsNullOrEmpty(secret)) secret = GlobalKey.WEIXINSECRET;
            string access_token = MangaCache.Get(GlobalKey.WEIXINACCESS_TOKEN_KEY + "_" + appid) as string;
            if (string.IsNullOrEmpty(access_token))
            {
                lock (sync_creater)
                {
                    access_token = MangaCache.Get(GlobalKey.WEIXINACCESS_TOKEN_KEY + "_" + appid) as string;
                    if (string.IsNullOrEmpty(access_token))
                    {
                        string url_access_token = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}"
                            , appid
                            , secret);
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
                            int expires_in = 7100;
                            if (dic_access_token.ContainsKey("expires_in"))
                                expires_in = DataConvert.SafeInt(dic_access_token["expires_in"], 7100);
                            MangaCache.Add(GlobalKey.WEIXINACCESS_TOKEN_KEY + "_" + appid, access_token, expires_in);
                        }
                    }
                }
            }

            return access_token;
        }

        /// <summary>
        /// <para>获取微信用户信息（授权或者从公众号链接过来的用户才能获取全部）</para>
        /// <para>返回数据：</para>
        /// <para>subscribe       是否关注公众号 [0：代表此用户没有关注该公众号]</para>
        /// <para>openid          用户的标识，对当前公众号唯一</para>
        /// <para>nickname        用户的昵称</para>
        /// <para>sex             用户的性别，值为1时是男性，值为2时是女性，值为0时是未知</para>
        /// <para>city            用户所在城市</para>
        /// <para>country         用户所在国家</para>
        /// <para>province        用户所在省份</para>
        /// <para>language        用户的语言，简体中文为zh_CN</para>
        /// <para>headimgurl      用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空</para>
        /// <para>subscribe_time  用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间</para>
        /// <para>unionid         只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。</para>
        /// </summary>
        /// <param name="accesstoken">通信密钥</param>
        /// <param name="openid">用户标识</param>
        /// <returns></returns>
        public Dictionary<string, string> GetOpeninfo(string accesstoken, string openid)
        {
            string url_openinfo = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN"
                            , accesstoken
                            , openid);
            string str_openinfo = Http.GetPageByWebClientUTF8(url_openinfo);
            Dictionary<string, string> dic_openinfo = new Dictionary<string, string>();
            try
            {
                dic_openinfo = json.Deserialize<Dictionary<string, string>>(str_openinfo);
            }
            catch { }
            return dic_openinfo;
        }

        /// <summary>
        /// 获取用户唯一标识openid
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="secret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetOpenid(string appid = "", string secret = "", string code = "")
        {
            if (string.IsNullOrEmpty(appid)) appid = GlobalKey.WEIXINAPPID;
            if (string.IsNullOrEmpty(secret)) secret = GlobalKey.WEIXINSECRET;
            string openid = string.Empty;
            string url_openid = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code"
                        , appid
                        , secret
                        , code);
            string str_openid = Http.GetPageByWebClientDefault(url_openid);
            Dictionary<string, string> dic_openid = new Dictionary<string, string>();
            try
            {
                dic_openid = json.Deserialize<Dictionary<string, string>>(str_openid);
            }
            catch { }
            if (dic_openid.ContainsKey("openid"))
            {
                openid = dic_openid["openid"];
            }

            return openid;
        }

        /// <summary>
        /// 获取JS-SDK使用权限签名
        /// </summary>
        /// <returns></returns>
        public string GetJsapiTicket(string appid = "", string secret = "")
        {
            if (string.IsNullOrEmpty(appid)) appid = GlobalKey.WEIXINAPPID;
            if (string.IsNullOrEmpty(secret)) secret = GlobalKey.WEIXINSECRET;
            string key = GlobalKey.WEIXINJSAPI_TICKET_KEY + "_" + appid;
            string jsapi_ticket = MangaCache.Get(key) as string;
            if (string.IsNullOrEmpty(jsapi_ticket))
            {
                lock (sync_creater)
                {
                    jsapi_ticket = MangaCache.Get(key) as string;
                    if (string.IsNullOrEmpty(jsapi_ticket))
                    {
                        string accesstoken = GetAccessToken(appid, secret);
                        string url_jsapi_ticket = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi"
                            , accesstoken);
                        string str_jsapi_ticket = Http.GetPageByWebClientDefault(url_jsapi_ticket);
                        Dictionary<string, string> dic_jsapi_ticket = new Dictionary<string, string>();
                        try
                        {
                            dic_jsapi_ticket = json.Deserialize<Dictionary<string, string>>(str_jsapi_ticket);
                        }
                        catch { }
                        if (dic_jsapi_ticket.ContainsKey("errmsg") && dic_jsapi_ticket["errmsg"] == "ok")
                        {
                            jsapi_ticket = dic_jsapi_ticket["ticket"];
                            int expires_in = 7100;
                            if (dic_jsapi_ticket.ContainsKey("expires_in"))
                                expires_in = DataConvert.SafeInt(dic_jsapi_ticket["expires_in"], 7100);
                            MangaCache.Add(key, jsapi_ticket, expires_in);
                        }
                    }
                }
            }

            return jsapi_ticket;

        }

        /// <summary>
        /// 获取卡券使用权限签名
        /// </summary>
        /// <returns></returns>
        public string GetCardapiTicket(string appid = "", string secret = "")
        {
            if (string.IsNullOrEmpty(appid)) appid = GlobalKey.WEIXINAPPID;
            if (string.IsNullOrEmpty(secret)) secret = GlobalKey.WEIXINSECRET;
            string key = GlobalKey.WEIXINCARDAPI_TICKET_KEY + "_" + appid;
            string jsapi_ticket = MangaCache.Get(key) as string;
            if (string.IsNullOrEmpty(jsapi_ticket))
            {
                lock (sync_creater)
                {
                    jsapi_ticket = MangaCache.Get(key) as string;
                    if (string.IsNullOrEmpty(jsapi_ticket))
                    {
                        string accesstoken = GetAccessToken(appid, secret);
                        string url_jsapi_ticket = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=wx_card"
                            , accesstoken);
                        string str_jsapi_ticket = Http.GetPageByWebClientDefault(url_jsapi_ticket);
                        Dictionary<string, string> dic_jsapi_ticket = new Dictionary<string, string>();
                        try
                        {
                            dic_jsapi_ticket = json.Deserialize<Dictionary<string, string>>(str_jsapi_ticket);
                        }
                        catch { }
                        if (dic_jsapi_ticket.ContainsKey("errmsg") && dic_jsapi_ticket["errmsg"] == "ok")
                        {
                            jsapi_ticket = dic_jsapi_ticket["ticket"];
                            int expires_in = 7100;
                            if (dic_jsapi_ticket.ContainsKey("expires_in"))
                                expires_in = DataConvert.SafeInt(dic_jsapi_ticket["expires_in"], 7100);
                            MangaCache.Add(key, jsapi_ticket, expires_in);
                        }
                    }
                }
            }

            return jsapi_ticket;

        }

        #endregion

        #region 测试活动

        public WeixinActInfo GetModel(string openid)
        {
            return CommonDataProvider.Instance().GetWeixinActInfo(openid);
        }

        public bool Add(WeixinActInfo entity)
        {
            return CommonDataProvider.Instance().AddWeixinAct(entity);
        }

        public int Dianzan(string openid, string vopenid)
        {
            return CommonDataProvider.Instance().WeixinDianzan(openid, vopenid);
        }

        public List<WeixinActInfo> GetList(int pageindex, int pagesize, WeixinActQuery query, ref int recordcount)
        {
            return CommonDataProvider.Instance().GetWeixinActList(pageindex, pagesize, query, ref recordcount);
        }

        #endregion

        #region 奔驰投票活动

        private Dictionary<string, DateTime> benzvotes = null;
        public Dictionary<string, DateTime> Benzvotes
        {
            get
            {
                if (benzvotes == null)
                {
                    lock (sync_benzvote)
                    {
                        benzvotes = GetAllBenzvote();
                    }
                }
                return benzvotes;
            }
            set
            {
                benzvotes = value;
            }
        }

        private static object sync_benzvote = new object();
        private List<BenzvoteInfo> BenzvotesCache = new List<BenzvoteInfo>();

        /// <summary>
        /// 添加/编辑参赛选手
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddBenzvotePothunterInfo(BenzvotePothunterInfo entity)
        {
            return CommonDataProvider.Instance().AddBenzvotePothunterInfo(entity);
        }

        /// <summary>
        /// 删除参赛选手
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public void DelBenzvotePothunterInfo(string ids)
        {
            CommonDataProvider.Instance().DelBenzvotePothunterInfo(ids);
        }

        public BenzvotePothunterInfo GetBenzvotePothunterInfo(int id, bool fromCache = false)
        {
            BenzvotePothunterInfo entity = null;

            List<BenzvotePothunterInfo> list = GetBenzvotePothunterList(fromCache);
            if (list.Exists(l => l.ID == id))
            {
                entity = list.Find(l => l.ID == id);
            }

            return entity;
        }

        public List<BenzvotePothunterInfo> GetBenzvotePothunterList(bool fromCache = false)
        {
            if (!fromCache)
            {
                List<BenzvotePothunterInfo> slist = ReorderBenzvotePothunter(CommonDataProvider.Instance().GetBenzvotePothunterList());
                return slist.OrderBy(b => b.SerialNumber).ToList();
            }

            string key = GlobalKey.BENZVOTEPOTHUNTER_LIST;
            List<BenzvotePothunterInfo> list = MangaCache.Get(key) as List<BenzvotePothunterInfo>;
            if (list == null)
            {
                lock (sync_creater)
                {
                    list = MangaCache.Get(key) as List<BenzvotePothunterInfo>;
                    if (list == null)
                    {
                        list = ReorderBenzvotePothunter(CommonDataProvider.Instance().GetBenzvotePothunterList());
                        list = list.OrderBy(b => b.SerialNumber).ToList();

                        MangaCache.Max(key, list);
                    }
                }
            }
            return list;
        }

        public void ReloadBenzvotePothunterListCache()
        {
            string key = GlobalKey.BENZVOTEPOTHUNTER_LIST;
            MangaCache.Remove(key);
            GetBenzvotePothunterList(true);
        }

        /// <summary>
        /// 对选手排名
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<BenzvotePothunterInfo> ReorderBenzvotePothunter(List<BenzvotePothunterInfo> list)
        {
            if (list != null)
            {
                list = list.OrderByDescending(b => b.Ballot).ToList(); ;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Order = list.IndexOf(list[i]) + 1;
                }
            }

            return list;
        }

        /// <summary>
        /// 分页获取投票信息
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="query"></param>
        /// <param name="recordcount"></param>
        /// <returns></returns>
        public List<BenzvoteInfo> GetBenzvoteList(int pageindex, int pagesize, BenzvoteQuery query, ref int recordcount)
        {
            return CommonDataProvider.Instance().GetBenzvoteList(pageindex, pagesize, query, ref recordcount);
        }

        public bool AddBenzvoteInfo(BenzvoteInfo entity)
        {
            return CommonDataProvider.Instance().AddBenzvoteInfo(entity);
        }

        /// <summary>
        /// 获取所有投票信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DateTime> GetAllBenzvote()
        {
            string key = GlobalKey.BENZVOTE_LIST;
            Dictionary<string, DateTime> result = MangaCache.Get(key) as Dictionary<string, DateTime>;
            if (result == null)
            {
                lock (sync_benzvote)
                {
                    result = MangaCache.Get(key) as Dictionary<string, DateTime>;
                    if (result == null)
                    {
                        BenzvoteQuery query = new BenzvoteQuery();
                        int recordcount = 0;
                        List<BenzvoteInfo> blist = GetBenzvoteList(1, int.MaxValue, query, ref recordcount);
                        result = new Dictionary<string, DateTime>();
                        foreach (BenzvoteInfo v in blist)
                        {
                            if (!result.Keys.Contains(v.AthleteID + "_" + v.Openid))
                                result.Add(v.AthleteID + "_" + v.Openid, v.AddTime);
                        }
                    }
                }
            }


            return result;
        }

        public void ReloadAllBenzvote()
        {
            string key = GlobalKey.BENZVOTE_LIST;
            MangaCache.Remove(key);
            benzvotes = GetAllBenzvote();
        }

        public void AddBenzvoteSetting(BenzvoteSettingInfo entity)
        {
            CommonDataProvider.Instance().AddBenzvoteSetting(entity);
        }

        public BenzvoteSettingInfo GetBenzvoteSetting(bool fromCache = false)
        {
            if (!fromCache)
            {
                return CommonDataProvider.Instance().GetBenzvoteSetting();
            }

            string key = GlobalKey.BENZVOTESETTING;
            BenzvoteSettingInfo setting = MangaCache.Get(key) as BenzvoteSettingInfo;
            if (setting == null)
            {
                lock (sync_creater)
                {
                    setting = MangaCache.Get(key) as BenzvoteSettingInfo;
                    if (setting == null)
                    {
                        setting = CommonDataProvider.Instance().GetBenzvoteSetting();

                        MangaCache.Max(key, setting);
                    }
                }
            }
            return setting;
        }

        public void ReloadBenzvoteSetting()
        {
            string key = GlobalKey.BENZVOTESETTING;
            MangaCache.Remove(key);
            GetBenzvoteSetting(true);
        }

        public string Benzvote(BenzvoteInfo vote)
        {
            try
            {
                lock (sync_benzvote)
                {
                    if (Benzvotes == null) Benzvotes = new Dictionary<string, DateTime>();
                    if (!Benzvotes.Keys.Contains(vote.AthleteID + "_" + vote.Openid))
                    {
                        Benzvotes.Add(vote.AthleteID + "_" + vote.Openid, vote.AddTime);
                        BenzvotesCache.Add(vote);
                    }
                    else
                    {
                        Benzvotes[vote.AthleteID + "_" + vote.Openid] = vote.AddTime;
                        BenzvotesCache.Add(vote);
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                return "系统繁忙，请稍后再试";
            }
        }

        public string CheckVote(string openid, string id)
        {
            BenzvoteSettingInfo setting = GetBenzvoteSetting(true);
            if (setting != null && setting.Switch == 0)
            {
                return "该活动还未开始，敬请期待。";
            }

            lock (sync_benzvote)
            {
                if (Benzvotes != null)
                {
                    List<KeyValuePair<string, DateTime>> votes = Benzvotes.Where(b => b.Key.EndsWith("_" + openid) && b.Value.ToString("yyyyMMdd") == DateTime.Today.ToString("yyyyMMdd")).ToList();

                    if (votes.Count > 0)
                    {
                        if (setting != null && setting.VoteTimes > 0 && votes.Count >= setting.VoteTimes)
                        {
                            return "您今天已经投过" + setting.VoteTimes + "次票，请明天再来为他/她投票吧。";
                        }
                        if (votes.Exists(v => v.Key == (id + "_" + openid)))
                        {
                            return "您今天已经为他/她投过票了，不能重复投哦。";
                        }
                        DateTime ftime = votes.Min(b => b.Value);
                        int minutes = setting == null ? 30 : setting.OverdueMinutes;
                        if (minutes > 0 && DateTime.Now.AddMinutes(-1 * minutes) > ftime)
                        {
                            return "您今天的投票期限已过。";
                        }
                    }

                }
            }

            return string.Empty;
        }

        public void BenzvoteAccount()
        {
            List<BenzvoteInfo> votes = new List<BenzvoteInfo>();
            lock (sync_benzvote)
            {
                if (BenzvotesCache.Count > 0)
                {
                    votes.AddRange(BenzvotesCache);
                    BenzvotesCache.Clear();
                }
            }
            List<BenzvotePothunterInfo> plist = GetBenzvotePothunterList(true);
            foreach (BenzvoteInfo vote in votes)
            {
                if (plist.Exists(p => p.ID == vote.AthleteID))
                {
                    BenzvotePothunterInfo pinfo = plist.Find(p => p.ID == vote.AthleteID);
                    pinfo.Ballot++;
                    if (AddBenzvoteInfo(vote))
                        AddBenzvotePothunterInfo(pinfo);
                }
            }
        }

        #endregion

        #region 集团投票活动

        private Dictionary<string, DateTime> jituanvotes = null;
        public Dictionary<string, DateTime> Jituanvotes
        {
            get
            {
                if (jituanvotes == null)
                {
                    lock (sync_jituanvote)
                    {
                        jituanvotes = GetAllJituanvote();
                    }
                }
                return jituanvotes;
            }
            set
            {
                jituanvotes = value;
            }
        }

        private static object sync_jituanvote = new object();
        private List<JituanvoteInfo> JituanvotesCache = new List<JituanvoteInfo>();

        #region 选手管理

        /// <summary>
        /// 添加/编辑参赛选手
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddJituanvotePothunterInfo(JituanvotePothunterInfo entity)
        {
            return CommonDataProvider.Instance().AddJituanvotePothunterInfo(entity);
        }

        /// <summary>
        /// 删除参赛选手
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public void DelJituanvotePothunterInfo(string ids)
        {
            CommonDataProvider.Instance().DelJituanvotePothunterInfo(ids);
            ReloadJituanvotePothunterListCache();
        }

        public JituanvotePothunterInfo GetJituanvotePothunterInfo(int id, bool fromCache = false)
        {
            JituanvotePothunterInfo entity = null;

            List<JituanvotePothunterInfo> list = GetJituanvotePothunterList(fromCache);
            if (list.Exists(l => l.ID == id))
            {
                entity = list.Find(l => l.ID == id);
            }

            return entity;
        }

        public List<JituanvotePothunterInfo> GetJituanvotePothunterList(bool fromCache = false)
        {
            if (!fromCache)
            {
                List<JituanvotePothunterInfo> slist = ReorderJituanvotePothunter(CommonDataProvider.Instance().GetJituanvotePothunterList());
                return slist.OrderBy(b => b.SerialNumber).ToList();
            }

            string key = GlobalKey.JITUANVOTEPOTHUNTER_LIST;
            List<JituanvotePothunterInfo> list = MangaCache.Get(key) as List<JituanvotePothunterInfo>;
            if (list == null)
            {
                lock (sync_creater)
                {
                    list = MangaCache.Get(key) as List<JituanvotePothunterInfo>;
                    if (list == null)
                    {
                        list = ReorderJituanvotePothunter(CommonDataProvider.Instance().GetJituanvotePothunterList());
                        list = list.OrderBy(b => b.SerialNumber).ToList();

                        MangaCache.Max(key, list);
                    }
                }
            }
            return list;
        }

        public void ReloadJituanvotePothunterListCache()
        {
            string key = GlobalKey.JITUANVOTEPOTHUNTER_LIST;
            MangaCache.Remove(key);
            GetJituanvotePothunterList(true);
        }

        /// <summary>
        /// 对选手排名
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<JituanvotePothunterInfo> ReorderJituanvotePothunter(List<JituanvotePothunterInfo> list)
        {
            if (list != null)
            {
                list = list.OrderByDescending(b => b.Ballot).ToList(); ;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Order = list.IndexOf(list[i]) + 1;
                }
            }

            return list;
        }

        #endregion

        #region 活动设置管理

        public void AddJituanvoteSetting(JituanvoteSettingInfo entity)
        {
            CommonDataProvider.Instance().AddJituanvoteSetting(entity);
        }

        public JituanvoteSettingInfo GetJituanvoteSetting(bool fromCache = false)
        {
            if (!fromCache)
            {
                return CommonDataProvider.Instance().GetJituanvoteSetting();
            }

            string key = GlobalKey.JITUANVOTESETTING;
            JituanvoteSettingInfo setting = MangaCache.Get(key) as JituanvoteSettingInfo;
            if (setting == null)
            {
                lock (sync_creater)
                {
                    setting = MangaCache.Get(key) as JituanvoteSettingInfo;
                    if (setting == null)
                    {
                        setting = CommonDataProvider.Instance().GetJituanvoteSetting();

                        MangaCache.Max(key, setting);
                    }
                }
            }
            return setting;
        }

        public void ReloadJituanvoteSetting()
        {
            string key = GlobalKey.JITUANVOTESETTING;
            MangaCache.Remove(key);
            GetJituanvoteSetting(true);
        }

        #endregion

        #region 销售精英投票活动

        /// <summary>
        /// 分页获取投票信息
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="query"></param>
        /// <param name="recordcount"></param>
        /// <returns></returns>
        public List<JituanvoteInfo> GetJituanvoteList(int pageindex, int pagesize, JituanvoteQuery query, ref int recordcount)
        {
            return CommonDataProvider.Instance().GetJituanvoteList(pageindex, pagesize, query, ref recordcount);
        }

        public bool AddJituanvoteInfo(JituanvoteInfo entity)
        {
            return CommonDataProvider.Instance().AddJituanvoteInfo(entity);
        }

        /// <summary>
        /// 获取所有投票信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DateTime> GetAllJituanvote()
        {
            string key = GlobalKey.JITUANVOTE_LIST;
            Dictionary<string, DateTime> result = MangaCache.Get(key) as Dictionary<string, DateTime>;
            if (result == null)
            {
                lock (sync_jituanvote)
                {
                    result = MangaCache.Get(key) as Dictionary<string, DateTime>;
                    if (result == null)
                    {
                        JituanvoteQuery query = new JituanvoteQuery();
                        int recordcount = 0;
                        List<JituanvoteInfo> blist = GetJituanvoteList(1, int.MaxValue, query, ref recordcount);
                        result = new Dictionary<string, DateTime>();
                        foreach (JituanvoteInfo v in blist)
                        {
                            if (!result.Keys.Contains(v.AthleteID + "_" + v.Openid))
                                result.Add(v.AthleteID + "_" + v.Openid, v.AddTime);
                        }
                    }
                }
            }


            return result;
        }

        public void ReloadAllJituanvote()
        {
            string key = GlobalKey.JITUANVOTE_LIST;
            MangaCache.Remove(key);
            jituanvotes = GetAllJituanvote();
        }

        public string Jituanvote(JituanvoteInfo vote)
        {
            try
            {
                lock (sync_jituanvote)
                {
                    if (Jituanvotes == null) Jituanvotes = new Dictionary<string, DateTime>();
                    Jituanvotes.Add(vote.AthleteID + "_" + vote.Openid, vote.AddTime);
                    JituanvotesCache.Add(vote);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                return "发生错误";
            }
        }

        public string CheckJituanVote(string openid, string id)
        {
            JituanvoteSettingInfo setting = GetJituanvoteSetting(true);
            if (setting != null && setting.Switch == 0)
            {
                return "该活动已结束";
            }

            lock (sync_jituanvote)
            {
                if (Jituanvotes != null)
                {
                    if (Jituanvotes.Keys.Contains(id + "_" + openid))
                    {
                        return "您已经为他/她点过赞了哦。";
                    }
                    List<KeyValuePair<string, DateTime>> votes = Jituanvotes.Where(b => b.Key.EndsWith("_" + openid) && b.Value > DateTime.Today).ToList();
                    if (votes.Count > 0)
                    {
                        DateTime ftime = votes.Min(b => b.Value);
                        int minutes = setting == null ? 30 : setting.OverdueMinutes;
                        //if (minutes > 0 && DateTime.Now.AddMinutes(-1 * minutes) > ftime)
                        //{
                        //    return "您今日的点赞期已过，请明日再投。";
                        //}
                        if (setting != null && setting.VoteTimes > 0 && votes.Count >= setting.VoteTimes)
                        {
                            return "您的点赞次数已经用完。";
                        }
                    }
                }
            }

            return string.Empty;
        }

        public void JituanvoteAccount()
        {
            List<JituanvoteInfo> votes = new List<JituanvoteInfo>();
            lock (sync_benzvote)
            {
                if (JituanvotesCache.Count > 0)
                {
                    votes.AddRange(JituanvotesCache);
                    JituanvotesCache.Clear();
                }
            }
            List<JituanvotePothunterInfo> plist = GetJituanvotePothunterList(true);
            foreach (JituanvoteInfo vote in votes)
            {
                if (plist.Exists(p => p.ID == vote.AthleteID))
                {
                    JituanvotePothunterInfo pinfo = plist.Find(p => p.ID == vote.AthleteID);
                    pinfo.Ballot++;
                    if (AddJituanvoteInfo(vote))
                        AddJituanvotePothunterInfo(pinfo);
                }
            }
        }

        #endregion

        #endregion

        #region 评论管理

        private static object sync_comment = new object();

        public int CreateAndUpdateComment(WeixinActCommentInfo entity)
        {
            return CommonDataProvider.Instance().CreateAndUpdateComment(entity);
        }

        public List<WeixinActCommentInfo> GetComments(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetWeixinActComments();

            string key = GlobalKey.WEIXINACTCOMMENT_LIST;
            List<WeixinActCommentInfo> list = MangaCache.Get(key) as List<WeixinActCommentInfo>;
            if (list == null)
            {
                lock (sync_comment)
                {
                    list = MangaCache.Get(key) as List<WeixinActCommentInfo>;
                    if (list == null)
                    {
                        list = CommonDataProvider.Instance().GetWeixinActComments();

                        MangaCache.Max(key, list);
                    }
                }
            }
            return list;
        }

        public void ReloadComments()
        {
            string key = GlobalKey.WEIXINACTCOMMENT_LIST;
            MangaCache.Remove(key);
            GetComments(true);
        }

        public string CheckVoteComment(WeixinActType acttype)
        {
            if (acttype == WeixinActType.集团活动)
            {
                JituanvoteSettingInfo setting = GetJituanvoteSetting(true);
                if (setting != null && setting.Switch == 0)
                {
                    return "该活动已结束";
                }
            }
            else if (acttype == WeixinActType.奔驰活动)
            {
                BenzvoteSettingInfo setting = GetBenzvoteSetting(true);
                if (setting != null && setting.Switch == 0)
                {
                    return "该活动已结束";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 提交评论
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string CommentPost(WeixinActCommentInfo entity)
        {
            try
            {
                List<WeixinActCommentInfo> list = GetComments(true);
                lock (sync_comment)
                {
                    int id = CreateAndUpdateComment(entity);
                    if (id > 0)
                    {
                        string key = GlobalKey.WEIXINACTCOMMENT_LIST;
                        entity.ID = id;
                        list.Add(entity);
                        MangaCache.Max(key, list);

                        CommentCount(entity.AthleteID, entity.WeixinActType);
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                return "发生错误";
            }
        }

        /// <summary>
        /// 献鲜花
        /// </summary>
        /// <param name="id">评论ID</param>
        /// <returns></returns>
        public string CommentPraise(int id)
        {
            try
            {
                if (id > 0)
                {
                    CommentPraiseCount(id);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                return "发生错误";
            }
        }

        /// <summary>
        /// 砸鸡蛋
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string CommentBelittle(int id)
        {
            try
            {
                if (id > 0)
                {
                    CommentBelittleCount(id);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                return "发生错误";
            }
        }

        #region 被评论计数

        private static object sync_commentcounter = new object();
        private static object sync_commentcountercreater = new object();

        private List<KeyValuePair<string, int>> _commentCounter = null;
        public List<KeyValuePair<string, int>> CommentCounter
        {
            get
            {
                if (_commentCounter == null)
                {
                    lock (sync_commentcountercreater)
                    {
                        _commentCounter = new List<KeyValuePair<string, int>>();
                    }
                }
                return _commentCounter;
            }
            set
            {
                _commentCounter = value;
            }
        }

        /// <summary>
        /// 评论计数
        /// </summary>
        /// <param name="pid">选手ID</param>
        /// <param name="acttype">活动类型</param>
        public void CommentCount(int pid, WeixinActType acttype)
        {
            try
            {
                lock (sync_commentcounter)
                {
                    string key = pid.ToString() + "_" + (int)acttype;
                    if (CommentCounter.Exists(c => c.Key == key))
                        CommentCounter[CommentCounter.FindIndex(c => c.Key == key)] = new KeyValuePair<string, int>(key, CommentCounter.Find(c => c.Key == key).Value + 1);
                    else
                        CommentCounter.Add(new KeyValuePair<string, int>(key, 1));
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
            }
        }

        /// <summary>
        /// 评论计数结算
        /// </summary>
        public void CommentCountAccount()
        {
            List<KeyValuePair<string, int>> counters = new List<KeyValuePair<string, int>>();
            lock (sync_commentcounter)
            {
                if (CommentCounter.Count > 0)
                {
                    counters.AddRange(CommentCounter);
                    CommentCounter.Clear();
                }
            }
            List<JituanvotePothunterInfo> jtvplist = GetJituanvotePothunterList(true);
            List<BenzvotePothunterInfo> bzvplist = GetBenzvotePothunterList(true);
            foreach (KeyValuePair<string, int> counter in counters)
            {
                int pid = DataConvert.SafeInt(counter.Key.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                WeixinActType type = (WeixinActType)DataConvert.SafeInt(counter.Key.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                if (type == WeixinActType.集团活动)
                {
                    if (jtvplist.Exists(p => p.ID == pid))
                    {
                        JituanvotePothunterInfo pinfo = jtvplist.Find(p => p.ID == pid);
                        pinfo.Comments += counter.Value;
                        AddJituanvotePothunterInfo(pinfo);
                    }
                }
                else if (type == WeixinActType.奔驰活动)
                {
                    if (bzvplist.Exists(p => p.ID == pid))
                    {
                        BenzvotePothunterInfo pinfo = bzvplist.Find(p => p.ID == pid);
                        pinfo.Comments += counter.Value;
                        AddBenzvotePothunterInfo(pinfo);
                    }
                }
            }
        }

        #endregion

        #region 献鲜花计数


        private static object sync_commentpraisecounter = new object();
        private static object sync_commentpraisecountercreater = new object();

        private List<KeyValuePair<string, int>> _commentpraiseCounter = null;
        public List<KeyValuePair<string, int>> CommentPraiseCounter
        {
            get
            {
                if (_commentpraiseCounter == null)
                {
                    lock (sync_commentpraisecountercreater)
                    {
                        _commentpraiseCounter = new List<KeyValuePair<string, int>>();
                    }
                }
                return _commentpraiseCounter;
            }
            set
            {
                _commentpraiseCounter = value;
            }
        }

        /// <summary>
        /// 评论鲜花计数
        /// </summary>
        /// <param name="id">评论ID</param>
        public void CommentPraiseCount(int id)
        {
            try
            {
                lock (sync_commentpraisecounter)
                {
                    string key = id.ToString();
                    if (CommentPraiseCounter.Exists(c => c.Key == key))
                        CommentPraiseCounter[CommentPraiseCounter.FindIndex(c => c.Key == key)] = new KeyValuePair<string, int>(key, CommentPraiseCounter.Find(c => c.Key == key).Value + 1);
                    else
                        CommentPraiseCounter.Add(new KeyValuePair<string, int>(key, 1));
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
            }
        }

        /// <summary>
        /// 鲜花计数结算
        /// </summary>
        public void CommentPraiseCountAccount()
        {
            List<KeyValuePair<string, int>> counters = new List<KeyValuePair<string, int>>();
            lock (sync_commentpraisecounter)
            {
                if (CommentPraiseCounter.Count > 0)
                {
                    counters.AddRange(CommentPraiseCounter);
                    CommentPraiseCounter.Clear();
                }
            }
            List<WeixinActCommentInfo> commentlist = GetComments(true);
            foreach (KeyValuePair<string, int> counter in counters)
            {
                int id = DataConvert.SafeInt(counter.Key);
                if (commentlist.Exists(p => p.ID == id))
                {
                    WeixinActCommentInfo cinfo = commentlist.Find(p => p.ID == id);
                    cinfo.PraiseNum += counter.Value;
                    CreateAndUpdateComment(cinfo);
                }
            }
        }

        #endregion

        #region 砸鸡蛋计数


        private static object sync_commentbelittlecounter = new object();
        private static object sync_commentbelittlecountercreater = new object();

        private List<KeyValuePair<string, int>> _commentbelittleCounter = null;
        public List<KeyValuePair<string, int>> CommentBelittleCounter
        {
            get
            {
                if (_commentbelittleCounter == null)
                {
                    lock (sync_commentbelittlecountercreater)
                    {
                        _commentbelittleCounter = new List<KeyValuePair<string, int>>();
                    }
                }
                return _commentbelittleCounter;
            }
            set
            {
                _commentbelittleCounter = value;
            }
        }

        /// <summary>
        /// 评论鸡蛋计数
        /// </summary>
        /// <param name="id">评论ID</param>
        public void CommentBelittleCount(int id)
        {
            try
            {
                lock (sync_commentbelittlecounter)
                {
                    string key = id.ToString();
                    if (CommentBelittleCounter.Exists(c => c.Key == key))
                        CommentBelittleCounter[CommentBelittleCounter.FindIndex(c => c.Key == key)] = new KeyValuePair<string, int>(key, CommentBelittleCounter.Find(c => c.Key == key).Value + 1);
                    else
                        CommentBelittleCounter.Add(new KeyValuePair<string, int>(key, 1));
                }
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
            }
        }

        /// <summary>
        /// 鸡蛋计数结算
        /// </summary>
        public void CommentBelittleCountAccount()
        {
            List<KeyValuePair<string, int>> counters = new List<KeyValuePair<string, int>>();
            lock (sync_commentbelittlecounter)
            {
                if (CommentBelittleCounter.Count > 0)
                {
                    counters.AddRange(CommentBelittleCounter);
                    CommentBelittleCounter.Clear();
                }
            }
            List<WeixinActCommentInfo> commentlist = GetComments(true);
            foreach (KeyValuePair<string, int> counter in counters)
            {
                int id = DataConvert.SafeInt(counter.Key);
                if (commentlist.Exists(p => p.ID == id))
                {
                    WeixinActCommentInfo cinfo = commentlist.Find(p => p.ID == id);
                    cinfo.BelittleNum += counter.Value;
                    CreateAndUpdateComment(cinfo);
                }
            }
        }

        #endregion

        #endregion

        #region 二手车评估器

        public void AddEscpgInfo(EscpgInfo entity)
        {
            CommonDataProvider.Instance().AddEscpgInfo(entity);
        }

        public List<EscpgInfo> GetEscpgList()
        {
            return CommonDataProvider.Instance().GetEscpgList();
        }

        public void UpdateEscpgRestore(string ids)
        {
            CommonDataProvider.Instance().UpdateEscpgRestore(ids);
        }

        #endregion

        #region 卡券活动

        #region 活动设置

        public void AddCardSetting(CardSettingInfo entity)
        {
            CommonDataProvider.Instance().AddCardSetting(entity);
        }

        public CardSettingInfo GetCardSetting(bool fromCache = false)
        {
            if (!fromCache)
            {
                return CommonDataProvider.Instance().GetCardSetting();
            }

            string key = GlobalKey.CARDSETTING;
            CardSettingInfo setting = MangaCache.Get(key) as CardSettingInfo;
            if (setting == null)
            {
                lock (sync_creater)
                {
                    setting = MangaCache.Get(key) as CardSettingInfo;
                    if (setting == null)
                    {
                        setting = CommonDataProvider.Instance().GetCardSetting();

                        MangaCache.Max(key, setting);
                    }
                }
            }
            return setting;
        }

        public void ReloadCardSetting()
        {
            string key = GlobalKey.CARDSETTING;
            MangaCache.Remove(key);
            GetCardSetting(true);
        }

        #endregion

        #region 获取卡券列表

        /// <summary>
        /// 获取卡券详细信息列表
        /// </summary>
        /// <returns></returns>
        public List<CardpackInfo> GetCardlist()
        {
            CardSettingInfo setting = GetCardSetting(true);
            string key = GlobalKey.CARDLIST + "_" + setting.AppID;

            List<CardpackInfo> cardlist = MangaCache.Get(key) as List<CardpackInfo>;
            if (cardlist == null)
            {
                lock (sync_creater)
                {
                    cardlist = MangaCache.Get(key) as List<CardpackInfo>;
                    if (cardlist == null)
                    {
                        cardlist = new List<CardpackInfo>();
                        if (setting == null) setting = new CardSettingInfo();
                        List<CardidInfo> cardidlist = GetCardidInfolist(true);
                        foreach (CardidInfo info in cardidlist)
                        {
                            CardpackInfo card = GetCardpack(setting.AppID, setting.AppSecret, info.Cardid);
                            if (card != null)
                                cardlist.Add(card);
                        }

                        MangaCache.Max(key, cardlist);
                    }
                }
            }

            return cardlist;
        }

        public void ReloadCardlist()
        {
            CardSettingInfo setting = GetCardSetting(true);
            string key = GlobalKey.CARDLIST + "_" + setting.AppID;
            MangaCache.Remove(key);
            GetCardlist();
        }

        /// <summary>
        /// 获取公众号下的卡券ID信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
        private CardidlistpackInfo GetCardidlistpack(string appid, string appsecret)
        {
            string url_cards = string.Format("https://api.weixin.qq.com/card/batchget?access_token={0}", WeixinActs.Instance.GetAccessToken(appid, appsecret));
            string str_cards = Http.Post("{\"offset\":0,\"count\"=10}", url_cards);
            CardidlistpackInfo list_card = null;
            try
            {
                list_card = json.Deserialize<CardidlistpackInfo>(str_cards);
            }
            catch { }
            return list_card;
        }

        /// <summary>
        /// 获取卡券详细信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <param name="card_id"></param>
        /// <returns></returns>
        private CardpackInfo GetCardpack(string appid, string appsecret, string card_id)
        {
            string url = string.Format("https://api.weixin.qq.com/card/get?access_token={0}", WeixinActs.Instance.GetAccessToken(appid, appsecret));
            string str_card = Http.Post("{\"card_id\":\"" + card_id + "\"", url, "utf-8");
            CardpackInfo cardinfo = null;
            try
            {
                cardinfo = json.Deserialize<CardpackInfo>(str_card);
            }
            catch { }
            return cardinfo;
        }

        /// <summary>
        /// 获取后台卡券设置列表信息
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        public List<CardidInfo> GetCardidInfolist(bool fromCache = false)
        {
            if (!fromCache)
            {
                return CommonDataProvider.Instance().GetCardidInfolist();
            }

            string key = GlobalKey.CARDIDLIST;
            List<CardidInfo> list = MangaCache.Get(key) as List<CardidInfo>;
            if (list == null)
            {
                lock (sync_creater)
                {
                    list = MangaCache.Get(key) as List<CardidInfo>;
                    if (list == null)
                    {
                        list = CommonDataProvider.Instance().GetCardidInfolist();

                        MangaCache.Max(key, list);
                    }
                }
            }
            return list;
        }

        public void ReloadCardidListCache()
        {
            string key = GlobalKey.CARDIDLIST;
            MangaCache.Remove(key);
            GetCardidInfolist(true);
        }

        public void DeleteCardidInfo(string ids)
        {
            CommonDataProvider.Instance().DeleteCardidInfo(ids);
        }

        public void AddCardidInfo(CardidInfo entity)
        {
            CommonDataProvider.Instance().AddCardidInfo(entity);
        }

        public void UpdateCardidInfo(CardidInfo entity)
        {
            CommonDataProvider.Instance().UpdateCardidInfo(entity);
        }

        #endregion

        #region 卡券抽奖记录

        public void AddCardPullRecord(CardPullRecordInfo entity)
        {
            CommonDataProvider.Instance().AddCardPullRecord(entity);
        }

        public void PullCard(string openid)
        {
            CommonDataProvider.Instance().PullCard(openid);
        }

        public List<CardPullRecordInfo> GetCardPullRecordList(bool fromCache = false)
        {
            if (!fromCache)
            {
                return CommonDataProvider.Instance().GetCardPullRecordList();
            }

            string key = GlobalKey.CARDPULLLIST;
            List<CardPullRecordInfo> list = MangaCache.Get(key) as List<CardPullRecordInfo>;
            if (list == null)
            {
                lock (sync_creater)
                {
                    list = MangaCache.Get(key) as List<CardPullRecordInfo>;
                    if (list == null)
                    {
                        list = CommonDataProvider.Instance().GetCardPullRecordList();

                        MangaCache.Max(key, list);
                    }
                }
            }
            return list;
        }

        public void ReloadCardPullRecordListCache()
        {
            string key = GlobalKey.CARDPULLLIST;
            MangaCache.Remove(key);
            GetCardPullRecordList(true);
        }

        public void ClearCardPullRecord()
        {
            CommonDataProvider.Instance().ClearCardPullRecord();
        }

        #endregion

        #endregion
    }
}
