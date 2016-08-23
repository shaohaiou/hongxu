using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;

namespace Hx.Components
{
    public class ScanTypes
    {
        #region 单例
        private static object sync_creater = new object();

        private static ScanTypes _instance;
        public static ScanTypes Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new ScanTypes();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<ScanTypeInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetScanTypeList();

            string key = GlobalKey.SCANTYPE_LIST;
            List<ScanTypeInfo> list = MangaCache.Get(key) as List<ScanTypeInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetScanTypeList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public ScanTypeInfo GetModel(int id, bool fromCache = false)
        {
            List<ScanTypeInfo> list = GetList(fromCache);
            return list.Find(l=>l.ID == id);
        }

        public void ReloadScanTypeListCache()
        {
            string key = GlobalKey.SCANTYPE_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }

        public void Add(ScanTypeInfo entity)
        {
            CommonDataProvider.Instance().AddScanType(entity);
        }

        public void Update(ScanTypeInfo entity)
        {
            CommonDataProvider.Instance().UpdateScanType(entity);
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteScanType(ids);
        }
    }
}
