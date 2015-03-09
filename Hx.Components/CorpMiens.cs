using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;

namespace Hx.Components
{
    public class CorpMiens
    {
        #region 单例
        private static object sync_creater = new object();

        private static CorpMiens _instance;
        public static CorpMiens Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new CorpMiens();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public CorpMienInfo GetCorpMien(int id, bool fromCache = false)
        {
            List<CorpMienInfo> list = GetList(fromCache);
            return list.Find(l=>l.ID == id);
        }

        public List<CorpMienInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetCorpMienList();

            string key = GlobalKey.CORPMIEN_LIST;
            List<CorpMienInfo> list = MangaCache.Get(key) as List<CorpMienInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetCorpMienList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void Add(CorpMienInfo entity)
        {
            CommonDataProvider.Instance().AddCorpMien(entity);
            ReloadCorpMienListCache();
        }

        public void Update(CorpMienInfo entity)
        {
            CommonDataProvider.Instance().UpdateCorpMien(entity);
            ReloadCorpMienListCache();
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteCorpMien(ids);
            ReloadCorpMienListCache();
        }

        public void ReloadCorpMienListCache()
        {
            string key = GlobalKey.CORPMIEN_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }
    }
}
