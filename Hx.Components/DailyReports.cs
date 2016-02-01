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
using Hx.Tools.Web;
using System.Text.RegularExpressions;

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

        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        public List<DailyReportInfo> GetList(DailyReportQuery query, bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetDailyReportList(query);

            string key = GlobalKey.DAILYREPORT_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.DayUnique;
            if (!string.IsNullOrEmpty(query.DayUniqueStart))
                key = GlobalKey.DAILYREPORT_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_r_" + query.DayUniqueStart + "_" + query.DayUniqueEnd;
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
            string key = GlobalKey.DAILYREPORT_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_" + query.DayUnique;
            MangaCache.Remove(key);
            string rkey = GlobalKey.DAILYREPORT_LIST + "_" + query.CorporationID.Value + "_" + ((int)query.DayReportDep).ToString() + "_r_";
            MangaCache.RemoveByPattern(rkey);
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

        public void CreateCheckHistory(DailyReportCheckHistoryInfo entity)
        {
            CommonDataProvider.Instance().CreateDailyReportCheckHistory(entity);
        }

        public List<DailyReportCheckHistoryInfo> GetCheckHistorys(int pageindex, int pagesize, DailyReportCheckHistoryQuery query, ref int recordcount)
        {
            return CommonDataProvider.Instance().GetDailyReportCheckHistoryList(pageindex, pagesize, query, ref recordcount);
        }

        #region 采集二手车日报数据

        private static bool hasruncollectdailyreport = false;

        /// <summary>
        /// 采集二手车日报数据
        /// </summary>
        public void CollectDailyReportInfo()
        {
            if (DateTime.Now.Hour == 1 && DateTime.Now.Minute == 5)
            {
                if (!hasruncollectdailyreport)
                {
                    hasruncollectdailyreport = true;
                    try
                    {
                        string url = "http://erp.hongxucar.com/json/information.ashx?type=E&username=sync&userpwd=hx123456&starttime={0}&endtime={1}";
                        string strdata = Http.GetPage(string.Format(url, DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")), 3);

                        List<DailyReportCollectInfo> listdata = JsonHelper.JsonDeserialize<List<DailyReportCollectInfo>>(strdata);
                        Dictionary<string, int> corpids = new Dictionary<string, int>();
                        corpids.Add("红旭智选", 32);
                        corpids.Add("温州红源", 8);
                        corpids.Add("红旭集团", 1);
                        corpids.Add("乐清红源", 17);
                        corpids.Add("瑞安红旭", 5);
                        corpids.Add("温州红旭", 9);
                        corpids.Add("温州红盈", 11);
                        corpids.Add("温州红翔", 10);
                        corpids.Add("德州丰田", 31);
                        corpids.Add("瑞安红升", 2);
                        corpids.Add("乐清红润", 18);
                        corpids.Add("乐清红通", 19);
                        corpids.Add("德州红旭", 30);
                        corpids.Add("路桥红本", 24);
                        corpids.Add("临海广本", 25);
                        corpids.Add("苍南红豪", 22);
                        corpids.Add("红旭现代", 26);
                        corpids.Add("红盛别克", 34);
                        if (listdata != null && listdata.Count > 0)
                        {
                            foreach (DailyReportCollectInfo info in listdata)
                            {
                                int corpid = 0;
                                if (corpids.ContainsKey(info.Corpname))
                                {
                                    corpid = corpids[info.Corpname];

                                    DailyReportInfo report = null;
                                    report = DailyReports.Instance.GetModel(corpid, DayReportDep.二手车部, DateTime.Today.AddDays(-1), true);
                                    if (report == null)
                                    {
                                        report = new DailyReportInfo()
                                        {
                                            CorporationID = corpid,
                                            DayUnique = DateTime.Today.AddDays(-1).ToString("yyyyMMdd"),
                                            Creator = "系统自动采集",
                                            LastUpdateUser = "系统自动采集",
                                            DailyReportCheckStatus = DailyReportCheckStatus.审核通过
                                        };
                                    }

                                    Dictionary<string, string> kvp = new Dictionary<string, string>();
                                    List<DailyReportModuleInfo> listmodule = DayReportModules.Instance.GetList(true);
                                    listmodule = listmodule.FindAll(l => l.Department == DayReportDep.二手车部);
                                    kvp.Add(listmodule.FindAll(m => m.Name == "实际评估台次").OrderBy(l => l.Sort).ToList()[2].ID.ToString(), info.xtsjpgtc);
                                    kvp.Add(listmodule.Find(m => m.Name == "置换台次").ID.ToString(), info.zhctc);
                                    kvp.Add(listmodule.Find(m => m.Name == "销售台次").ID.ToString(), info.xsctc);
                                    kvp.Add(listmodule.Find(m => m.Name == "销售推荐评估台次").ID.ToString(), info.xstjpgtc);
                                    kvp.Add(listmodule.Find(m => m.Name == "售后推荐评估台次").ID.ToString(), info.shtjpgtc);
                                    kvp.Add(listmodule.Find(m => m.Name == "其他渠道推荐评估台次").ID.ToString(), info.qtqdtjpgtc);
                                    kvp.Add(listmodule.Find(m => m.Name == "潜客回访数").ID.ToString(), info.qkhfs);
                                    kvp.Add(listmodule.Find(m => m.Name == "首次来电批次").ID.ToString(), info.scldpc);
                                    kvp.Add(listmodule.Find(m => m.Name == "首次来店批次").ID.ToString(), info.scldianpc);
                                    kvp.Add(listmodule.Find(m => m.Name == "新增有效留档数").ID.ToString(), info.xzyxlds);
                                    kvp.Add(listmodule.Find(m => m.Name == "销售回访数").ID.ToString(), info.xshfs);
                                    kvp.Add(listmodule.Find(m => m.Name == "纯收购台次").ID.ToString(), info.csgtc);
                                    kvp.Add(listmodule.Find(m => m.Name == "销售毛利").ID.ToString(), info.xsml);
                                    kvp.Add(listmodule.Find(m => m.Name == "库存数").ID.ToString(), info.kcs);
                                    kvp.Add(listmodule.Find(m => m.Name == "寄售车台次").ID.ToString(), info.jsctc);
                                    kvp.Add(listmodule.Find(m => m.Name == "在库超30天车辆").ID.ToString(), info.zkc30tcl);
                                    kvp.Add(listmodule.FindAll(m => m.Name == "新增意向客户数").OrderBy(l => l.Sort).ToList()[0].ID.ToString(), info.xzmcyxkhs);
                                    kvp.Add(listmodule.FindAll(m => m.Name == "新增意向客户数").OrderBy(l => l.Sort).ToList()[1].ID.ToString(), info.xzmaicyxkhs);
                                    report.SCReport = json.Serialize(kvp);
                                    DailyReports.Instance.CreateAndUpdate(report, DayReportDep.二手车部);

                                    DailyReportHistoryInfo reporthistory = new DailyReportHistoryInfo();
                                    reporthistory.DayUnique = report.DayUnique;
                                    reporthistory.Modify = report;
                                    reporthistory.Creator = "系统自动采集";
                                    reporthistory.CreatorCorporationID = 1;
                                    reporthistory.CreatorCorporationName = "红旭集团股份公司";
                                    reporthistory.CreatorDepartment = DayReportDep.行政部;
                                    reporthistory.ReportDepartment = DayReportDep.二手车部;
                                    reporthistory.ReportCorporationID = corpid;
                                    DailyReports.Instance.CreateHistory(reporthistory);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExpLog.Write(ex);
                    }
                }
            }
            else
                hasruncollectdailyreport = false;
        }

        #endregion
    }
}
