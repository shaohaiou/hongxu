using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Query;
using Hx.Components.Providers;
using Hx.Components.Enumerations;
using Hx.Tools;
using System.Data;

namespace Hx.Components
{
    public class DailyReports
    {
        #region 单例
        private static object sync_creater = new object();

        private static DailyReports _instance;
        public static DailyReports Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new DailyReports();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<DailyReportInfo> GetList(DailyReportQuery query, bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetDailyReportList(query);

            string key = GlobalKey.DAILYREPORT_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.DayUnique + "_" + query.DayUniqueStart + "_" + query.DayUniqueEnd;
            List<DailyReportInfo> list = MangaCache.Get(key) as List<DailyReportInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetDailyReportList(query);
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadDailyReportListCache(DailyReportQuery query)
        {
            string key = GlobalKey.DAILYREPORT_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.DayUnique + "_" + query.DayUniqueStart + "_" + query.DayUniqueEnd;
            MangaCache.Remove(key);
            GetList(query, true);
        }

        public DailyReportInfo GetModel(int corporationid, DayReportDep dep, DateTime day, bool fromCache = false)
        {
            DailyReportInfo entity = null;

            DailyReportQuery query = new DailyReportQuery();
            query.CorporationID = corporationid;
            query.DayReportDep = dep;
            query.DayUnique = day.ToString("yyyyMM");
            List<DailyReportInfo> list = GetList(query, fromCache);
            entity = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));

            return entity;
        }

        public void CreateAndUpdate(DailyReportInfo entity, DayReportDep dep)
        {
            string tablename = EnumExtensions.GetDescription<DayReportDep>(dep.ToString());

            CommonDataProvider.Instance().CreateAndUpdateDailyReport(tablename, entity);

            DailyReportQuery query = new DailyReportQuery();
            query.DayReportDep = dep;
            query.CorporationID = entity.CorporationID;
            query.DayUnique = entity.DayUnique.Substring(0, 6);
            ReloadDailyReportListCache(query);
        }

        public void CreateHistory(DailyReportHistoryInfo entity)
        {
            CommonDataProvider.Instance().CreateDailyReportHistory(entity);
        }

        public List<DailyReportHistoryInfo> GetHistorys(int pageindex, int pagesize, DailyReportHistoryQuery query, ref int recordcount)
        {
            return CommonDataProvider.Instance().GetDailyReportHistoryList(pageindex, pagesize, query, ref recordcount);
        }

    }
}
