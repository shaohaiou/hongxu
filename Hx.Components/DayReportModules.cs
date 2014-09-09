using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;
using Hx.Components.Enumerations;

namespace Hx.Components
{
    public class DayReportModules
    {
        #region 单例
        private static object sync_creater = new object();

        private static DayReportModules _instance;
        public static DayReportModules Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new DayReportModules();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<DailyReportModuleInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetDailyReportModuleList();

            string key = GlobalKey.DAILYREPORTMODULE_LIST;
            List<DailyReportModuleInfo> list = MangaCache.Get(key) as List<DailyReportModuleInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetDailyReportModuleList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public List<DailyReportModuleInfo> GetList(DayReportDep dep, bool fromCache = false)
        {
            List<DailyReportModuleInfo> list = GetList(fromCache);

            return list.FindAll(l=>l.Department == dep);
        }

        public void ReloadDailyReportModuleListCache()
        {
            string key = GlobalKey.DAILYREPORTMODULE_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }

        public DailyReportModuleInfo GetModel(int id, bool fromCache = false)
        {
            DailyReportModuleInfo entity = null;

            List<DailyReportModuleInfo> list = GetList(fromCache);
            entity = list.Find(l => l.ID == id);

            return entity;
        }

        public void Add(DailyReportModuleInfo entity)
        {
            CommonDataProvider.Instance().AddDailyReportModule(entity);
        }

        public void Update(DailyReportModuleInfo entity)
        {
            CommonDataProvider.Instance().UpdateDailyReportModule(entity);
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteDailyReportModule(ids);
        }
    }
}
