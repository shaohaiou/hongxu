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
    public class CRMReports
    {
        #region 单例
        private static object sync_creater = new object();

        private static CRMReports _instance;
        public static CRMReports Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new CRMReports();
                    }
                }
                return _instance;
            }
        }

        #endregion

        private static object sync_data = new object();

        public string GetNavUrl(CRMReportType t,string Nm,int Id,int Mm)
        {
            string result = string.Empty;

            switch (t)
            {
                case CRMReportType.客流量登记表:
                    result = string.Format("crmreportcustomerflow.aspx?Nm={0}&Id={1}&Mm={2}", Nm, Id, Mm);
                    break;
                case CRMReportType.活动外出访客信息:
                    result = string.Format("crmreportoutwardvisit.aspx?Nm={0}&Id={1}&Mm={2}", Nm, Id, Mm);
                    break;
                default:
                    break;
            }

            return result;
        }

        public void CreateAndUpdate(CRMReportInfo entity)
        {
            CommonDataProvider.Instance().CreateAndUpdateCRMReport(entity);
        }

        public void ReloadCRMReportListCache(CRMReportQuery query)
        {
            string key = GlobalKey.CRMREPORT_LIST + "_" + query.CorporationID.Value + "_" + query.MonthStr + "_" + (int)query.CRMReportType;
            MangaCache.Remove(key);
            GetList(query, true);
        }

        public List<CRMReportInfo> GetList(CRMReportQuery query, bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetCRMReportList(query);

            string key = GlobalKey.CRMREPORT_LIST + "_" + query.CorporationID.Value + "_" + query.MonthStr + "_" + (int)query.CRMReportType;
            List<CRMReportInfo> list = MangaCache.Get(key) as List<CRMReportInfo>;
            if (list == null)
            {
                lock (sync_data)
                {
                    list = MangaCache.Get(key) as List<CRMReportInfo>;
                    if (list == null)
                    {
                        list = CommonDataProvider.Instance().GetCRMReportList(query);
                        MangaCache.Max(key, list);
                    }
                }
            }
            return list;
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteCRMReport(ids);
        }
    }
}
