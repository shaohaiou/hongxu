using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Providers;
using Hx.Components;
using Hx.Car.Entity;

namespace Hx.Car
{
    public class Jcbs
    {
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

        public void ReloadAccountListCache()
        {
            string key = GlobalKey.JCBACCOUNT_LIST;
            MangaCache.Remove(key);
            GetAccountList(true);
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

        #endregion
    }
}
