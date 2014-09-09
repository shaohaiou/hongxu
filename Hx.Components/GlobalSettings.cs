using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Providers;
using Hx.Components.Entity;

namespace Hx.Components
{
    public class GlobalSettings
    {
        #region 单例

        private static object sync_creater = new object();

        private static GlobalSettings _instance = null;
        public static GlobalSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new GlobalSettings();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public GlobalSettingInfo GetModel(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetGlobalSetting();

            string key = GlobalKey.GLOBALSETTING_KEY;
            GlobalSettingInfo entity = MangaCache.Get(key) as GlobalSettingInfo;
            if (entity == null)
            {
                entity = CommonDataProvider.Instance().GetGlobalSetting();
                MangaCache.Max(key, entity);
            }
            return entity;
        }

        public void ReloadGlobalSettingCache()
        {
            string key = GlobalKey.GLOBALSETTING_KEY;
            MangaCache.Remove(key);
            GetModel(true);
        }

        public void Add(GlobalSettingInfo entity)
        {
            CommonDataProvider.Instance().AddGlobalSetting(entity);
            ReloadGlobalSettingCache();
        }
    }
}
