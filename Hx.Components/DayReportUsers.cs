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

        #region 现有资料管理

        public List<PersonaldataInfo> GetPersonaldataList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetPersonaldataList();

            string key = GlobalKey.PERSONALDATA_LIST;
            List<PersonaldataInfo> list = MangaCache.Get(key) as List<PersonaldataInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetPersonaldataList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void DeletePersonaldata(string ids)
        {
            CommonDataProvider.Instance().DeletePersonaldata(ids);
        }

        public void AddPersonaldata(PersonaldataInfo entity)
        {
            CommonDataProvider.Instance().AddPersonaldata(entity);
        }

        public PersonaldataInfo GetPersonaldata(int id, bool fromCache = false)
        {
            PersonaldataInfo entity = null;

            List<PersonaldataInfo> list = GetPersonaldataList(fromCache);
            if (list.Exists(l => l.ID == id))
            {
                entity = list.Find(l => l.ID == id);
            }

            return entity;
        }

        public void UpdatePersonaldata(PersonaldataInfo entity)
        {
            CommonDataProvider.Instance().UpdatePersonaldata(entity);
            ReloadPersonaldataListCache();
        }

        public void ReloadPersonaldataListCache()
        {
            string key = GlobalKey.PERSONALDATA_LIST;
            MangaCache.Remove(key);
            GetPersonaldataList(true);
        }

        #endregion
    }
}
