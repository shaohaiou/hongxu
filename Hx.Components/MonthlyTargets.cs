using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;
using Hx.Components.Entity;
using Hx.Components.Providers;
using Hx.Components.Query;

namespace Hx.Components
{
    public class MonthlyTargets
    {
        #region 单例
        private static object sync_creater = new object();

        private static MonthlyTargets _instance;
        public static MonthlyTargets Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new MonthlyTargets();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<MonthlyTargetInfo> GetList(MonthTargetQuery query, bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetMonthTargetList(query);

            string key = GlobalKey.MONTHTARGET_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.MonthUnique;
            List<MonthlyTargetInfo> list = MangaCache.Get(key) as List<MonthlyTargetInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetMonthTargetList(query);
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadMonthTargetListCache(MonthTargetQuery query)
        {
            string key = GlobalKey.MONTHTARGET_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.MonthUnique;
            MangaCache.Remove(key);
            GetList(query, true);
        }

        public MonthlyTargetInfo GetModel(int corporationid, DayReportDep dep, DateTime day, bool fromCache = false)
        {
            MonthlyTargetInfo entity = null;

            MonthTargetQuery query = new MonthTargetQuery()
            {
                CorporationID = corporationid,
                DayReportDep = dep,
                MonthUnique = day.ToString("yyyy")
            };
            List<MonthlyTargetInfo> list = GetList(query, fromCache);
            entity = list.Find(l => l.MonthUnique == day.ToString("yyyyMM"));

            return entity;
        }

        public void CreateAndUpdate(MonthlyTargetInfo entity)
        {
            CommonDataProvider.Instance().CreateAndUpdateMonthlyTarget(entity);

            MonthTargetQuery query = new MonthTargetQuery();
            query.DayReportDep = entity.Department;
            query.CorporationID = entity.CorporationID;
            query.MonthUnique = entity.MonthUnique.Substring(0, 4);
            ReloadMonthTargetListCache(query);
        }

        public void CreateHistory(MonthlyTargetHistoryInfo entity)
        {
            CommonDataProvider.Instance().CreateMonthlyTargetHistory(entity);
        }

        public List<MonthlyTargetHistoryInfo> GetHistorys(int pageindex, int pagesize, MonthlyTargetHistoryQuery query, ref int recordcount)
        {
            return CommonDataProvider.Instance().GetMonthlyTargetHistoryList(pageindex, pagesize, query, ref recordcount);
        }
    }
}
