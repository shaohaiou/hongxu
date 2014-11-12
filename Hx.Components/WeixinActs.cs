using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;
using Hx.Components.Query;
using Hx.Tools;

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
                lock (sync_creater)
                {
                    if (result == null)
                    {
                        BenzvoteQuery query = new BenzvoteQuery();
                        int recordcount = 0;
                        List<BenzvoteInfo> blist = GetBenzvoteList(1, int.MaxValue, query, ref recordcount);
                        result = new Dictionary<string, DateTime>();
                        foreach (BenzvoteInfo v in blist)
                        {
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
            GetAllBenzvote();
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
                    Benzvotes.Add(vote.AthleteID + "_" + vote.Openid, vote.AddTime);
                    BenzvotesCache.Add(vote);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                ExpLog.Write(ex);
                return "发生错误";
            }
        }

        public string CheckVote(string openid, string id)
        {
            BenzvoteSettingInfo setting = GetBenzvoteSetting(true);
            if (setting != null && setting.Switch == 0)
            {
                return "该活动已结束";
            }

            lock (sync_benzvote)
            {
                if (Benzvotes != null)
                {
                    if (Benzvotes.Keys.Contains(id + "_" + openid))
                    {
                        return "您已经为她投过票了，不能重复投哦。";
                    }
                    List<KeyValuePair<string, DateTime>> votes = WeixinActs.Instance.Benzvotes.TakeWhile(b => b.Key.EndsWith("_" + openid)).ToList();
                    if (votes.Count > 0)
                    {
                        DateTime ftime = votes.Min(b => b.Value);
                        int minutes = setting == null ? 30 : setting.OverdueMinutes;
                        if (DateTime.Now.AddMinutes(-1 * minutes) > ftime)
                        {
                            return "您的投票期已过。";
                        }
                        if (setting != null && setting.VoteTimes > 0 && votes.Count == setting.VoteTimes)
                        {
                            return "您已经投过" + setting.VoteTimes + "次票，不能再投了。";
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
                lock (sync_creater)
                {
                    if (result == null)
                    {
                        JituanvoteQuery query = new JituanvoteQuery();
                        int recordcount = 0;
                        List<JituanvoteInfo> blist = GetJituanvoteList(1, int.MaxValue, query, ref recordcount);
                        result = new Dictionary<string, DateTime>();
                        foreach (JituanvoteInfo v in blist)
                        {
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
            GetAllJituanvote();
        }

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

        public string Jituanvote(JituanvoteInfo vote)
        {
            try
            {
                lock (sync_benzvote)
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

            lock (sync_benzvote)
            {
                if (Jituanvotes != null)
                {
                    if (Jituanvotes.Keys.Contains(id + "_" + openid))
                    {
                        return "您已经为她投过票了，不能重复投哦。";
                    }
                    List<KeyValuePair<string, DateTime>> votes = WeixinActs.Instance.Jituanvotes.TakeWhile(b => b.Key.EndsWith("_" + openid)).ToList();
                    if (votes.Count > 0)
                    {
                        DateTime ftime = votes.Min(b => b.Value);
                        int minutes = setting == null ? 30 : setting.OverdueMinutes;
                        if (DateTime.Now.AddMinutes(-1 * minutes) > ftime)
                        {
                            return "您的投票期已过。";
                        }
                        if (setting != null && setting.VoteTimes > 0 && votes.Count == setting.VoteTimes)
                        {
                            return "您已经投过" + setting.VoteTimes + "次票，不能再投了。";
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
    }
}
