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
    public class MonthlyTargetPres
    {
        #region 单例
        private static object sync_creater = new object();

        private static MonthlyTargetPres _instance;
        public static MonthlyTargetPres Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new MonthlyTargetPres();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<MonthlyTargetInfo> GetList(MonthTargetPreQuery query, bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetMonthTargetPreList(query);

            string key = GlobalKey.MONTHTARGETPRE_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.MonthUnique;
            List<MonthlyTargetInfo> list = MangaCache.Get(key) as List<MonthlyTargetInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetMonthTargetPreList(query);
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadMonthTargetPreListCache(MonthTargetPreQuery query)
        {
            string key = GlobalKey.MONTHTARGETPRE_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.MonthUnique;
            MangaCache.Remove(key);
            GetList(query, true);
        }

        public MonthlyTargetInfo GetModel(int corporationid, DayReportDep dep, DateTime day, bool fromCache = false)
        {
            MonthlyTargetInfo entity = null;

            MonthTargetPreQuery query = new MonthTargetPreQuery()
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
            CommonDataProvider.Instance().CreateAndUpdateMonthlyTargetPre(entity);

            MonthTargetPreQuery query = new MonthTargetPreQuery();
            query.DayReportDep = entity.Department;
            query.CorporationID = entity.CorporationID;
            query.MonthUnique = entity.MonthUnique.Substring(0, 4);
            ReloadMonthTargetPreListCache(query);
        }
    }
}
