using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Entity;
using Hx.Car.Providers;
using Hx.Components;

namespace Hx.Car
{
    public class Sybxs
    {
        #region 单例

        private static object sync_creater = new object();

        private static Sybxs _instance = null;
        public static Sybxs Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Sybxs();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<SybxInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CarDataProvider.Instance().GetSybxList();

            string key = GlobalKey.SYBX_LIST;
            List<SybxInfo> list = MangaCache.Get(key) as List<SybxInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetSybxList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadSybxListCache()
        {
            string key = GlobalKey.SYBX_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }

        public void Add(SybxInfo entity)
        {
            CarDataProvider.Instance().AddSybx(entity);
        }

        public void Update(SybxInfo entity)
        {
            CarDataProvider.Instance().UpdateSybx(entity);
        }

        public void Delete(string ids)
        {
            CarDataProvider.Instance().DeleteSybx(ids);
        }
    }
}
