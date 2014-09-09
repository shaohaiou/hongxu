using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Providers;
using Hx.Components.Entity;

namespace Hx.Components
{
    public class DayReportUsers
    {
        #region 单例
        private static object sync_creater = new object();

        private static DayReportUsers _instance;
        public static DayReportUsers Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new DayReportUsers();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public DayReportUserInfo GetModel(string usertag,bool fromCache = false)
        {
            DayReportUserInfo entity = null;
            
            List<DayReportUserInfo> list = GetList(fromCache);
            if (list.Exists(l => l.UserTag == usertag))
            {
                entity = list.Find(l => l.UserTag == usertag);
            }

            return entity;
        }

        public DayReportUserInfo GetModel(int id, bool fromCache = false)
        {
            DayReportUserInfo entity = null;

            List<DayReportUserInfo> list = GetList(fromCache);
            if (list.Exists(l => l.ID == id))
            {
                entity = list.Find(l => l.ID == id);
            }

            return entity;
        }

        public List<DayReportUserInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetDayReportUserList();

            string key = GlobalKey.DAYREPORTUSER_LIST;
            List<DayReportUserInfo> list = MangaCache.Get(key) as List<DayReportUserInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetDayReportUserList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadDayReportUserListCache()
        {
            string key = GlobalKey.DAYREPORTUSER_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }

        public void Add(DayReportUserInfo entity)
        {
            CommonDataProvider.Instance().AddDayReportUser(entity);
        }

        public void Update(DayReportUserInfo entity)
        {
            CommonDataProvider.Instance().UpdateDayReportUser(entity);
            ReloadDayReportUserListCache();
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteDayReportUser(ids);
        }
    }
}
