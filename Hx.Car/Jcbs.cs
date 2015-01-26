using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Providers;
using Hx.Components;
using Hx.Car.Entity;
using Hx.Car.Enum;
using Hx.Tools.Web;
using System.Web.Script.Serialization;
using Hx.Components.Entity;
using System.Collections.Specialized;

namespace Hx.Car
{
    public class Jcbs
    {
        private static JavaScriptSerializer json = new JavaScriptSerializer();
        #region 单例
        private static object sync_creater = new object();

        private static Jcbs _instance;
        public static Jcbs Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Jcbs();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 帐号管理

        public int CreateAndUpdateAccount(JcbAccountInfo entity)
        {
            return CarDataProvider.Instance().AddJcbAccount(entity);
        }

        public List<JcbAccountInfo> GetAccountList(bool fromCache = false)
        {
            if (!fromCache)
                return CarDataProvider.Instance().GetJcbAccountList();

            string key = GlobalKey.JCBACCOUNT_LIST;
            List<JcbAccountInfo> list = MangaCache.Get(key) as List<JcbAccountInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetJcbAccountList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public JcbAccountInfo GetAccountModel(int id, bool fromCache = false)
        {
            List<JcbAccountInfo> list = GetAccountList(fromCache);
            return list.Find(c => c.ID == id);
        }

        public void DeleteAccount(string ids)
        {
            CarDataProvider.Instance().DeleteAccount(ids);
        }

        public void ReloadAccountListCache()
        {
            string key = GlobalKey.JCBACCOUNT_LIST;
            MangaCache.Remove(key);
            GetAccountList(true);
        }

        public JcbAccountInfo GetAccountModelRemote(int id, bool fromCache = false)
        {
            string url = "http://jcb.hongxu.cn/jcbapi.axd?action=getaccountinfo&id=" + id;
            string jsonstr = Http.GetPage(url);
            return json.Deserialize<JcbAccountInfo>(jsonstr);
        }

        #endregion

        #region 营销记录

        public int CreateAndUpdateMarketrecord(JcbMarketrecordInfo entity)
        {
            return CarDataProvider.Instance().AddJcbMarketrecord(entity);
        }

        public List<JcbMarketrecordInfo> GetMarketrecordList(bool fromCache = false)
        {
            if (!fromCache)
                return CarDataProvider.Instance().GetJcbMarketrecordList();

            string key = GlobalKey.JCBMARKETRECORD_LIST;
            List<JcbMarketrecordInfo> list = MangaCache.Get(key) as List<JcbMarketrecordInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetJcbMarketrecordList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public JcbMarketrecordInfo GetMarketrecordModel(int id, bool fromCache = false)
        {
            List<JcbMarketrecordInfo> list = GetMarketrecordList(fromCache);
            return list.Find(c => c.ID == id);
        }

        public void ReloadMarketrecordListCache()
        {
            string key = GlobalKey.JCBMARKETRECORD_LIST;
            MangaCache.Remove(key);
            GetMarketrecordList(true);
        }

        public void CreateAndUpdateMarketrecordRemote(JcbMarketrecordInfo entity)
        {
            string url = "http://jcb.hongxu.cn/jcbapi.axd";
            NameValueCollection postVars = new NameValueCollection();
            postVars.Add("action", "createmarketrecord");
            postVars.Add("datastr", json.Serialize(entity));
            Http.PostData(postVars,new string[]{ url});
        }

        #endregion

        #region 用户管理

        public JcbUserInfo GetJcbUserRemote(string username,string password)
        {
            string url = "http://jcb.hongxu.cn/jcbapi.axd?action=getjcbuserinfo&username=" + username + "&password=" + password;
            string jsonstr = Http.GetPage(url);
            return json.Deserialize<JcbUserInfo>(jsonstr);
        }

        #endregion

        #region 其他

        public string GetLoginUrl(JcbAccountInfo account)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://account.che168.com/login";
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://passport.58.com/login";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                default:
                    break;
            }

            return result;
        }

        public string GetSuccessUrl(JcbAccountInfo account)
        { 
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://i.che168.com/car/success/";
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://passport.58.com/login";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                default:
                    break;
            }

            return result;            
        }

        public string GetPublicUrl(JcbAccountInfo account)
        { 
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://i.che168.com/car/add/";
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://post.58.com/330/29/s5";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                default:
                    break;
            }

            return result;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="cid">车辆ID</param>
        /// <returns></returns>
        public string GetViewUrl(JcbAccountInfo account,string cid)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://www.che168.com/Personal/CarPreview_V3.aspx?infoid=" + cid;
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://post.58.com/330/29/s5";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                default:
                    break;
            }

            return result;   
        }

        public string GetListUrl(JcbAccountInfo account)
        {
            string result = string.Empty;

            switch (account.JcbSiteType)
            {
                case JcbSiteType.t_二手车之家:
                    if (account.JcbAccountType == JcbAccountType.个人帐号)
                        result = "http://account.che168.com/login";
                    else
                        result = "";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://post.58.com/330/29/s5";
                    break;
                case JcbSiteType.赶集网:
                    result = "https://passport.ganji.com/login.php";
                    break;
                default:
                    break;
            }

            return result;            
        }

        public string GetSiteUrl(JcbSiteType sitetype)
        {
            string result = string.Empty;

            switch (sitetype)
            {
                case JcbSiteType.t_二手车之家:
                    result = "http://www.che168.com/";
                    break;
                case JcbSiteType.t_58同城:
                    result = "http://www.58.com/";
                    break;
                case JcbSiteType.赶集网:
                    result = "http://www.ganji.com/";
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion
    }
}
