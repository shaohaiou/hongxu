using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;

namespace Hx.Components
{
    public class Areas
    {
        #region 单例
        private static object sync_creater = new object();

        private static Areas _instance;
        public static Areas Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Areas();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 省份

        public List<PromaryInfo> GetPromaryList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetPromaryList();

            string key = GlobalKey.PROMARY_LIST;
            List<PromaryInfo> list = MangaCache.Get(key) as List<PromaryInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetPromaryList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadPromaryListCache()
        {
            string key = GlobalKey.PROMARY_LIST;
            MangaCache.Remove(key);
            GetPromaryList(true);
        }

        #endregion

        #region 市

        public List<CityInfo> GetCityList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetCityList();

            string key = GlobalKey.CITY_LIST;
            List<CityInfo> list = MangaCache.Get(key) as List<CityInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetCityList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadCityListCache()
        {
            string key = GlobalKey.CITY_LIST;
            MangaCache.Remove(key);
            GetCityList(true);
        }

        #endregion
    }
}
