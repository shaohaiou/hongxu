using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;

namespace Hx.Components
{
    public class JobOffers
    {
        #region 单例
        private static object sync_creater = new object();

        private static JobOffers _instance;
        public static JobOffers Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new JobOffers();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<JobOfferInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetJobOfferList();

            string key = GlobalKey.JOBOFFER_LIST;
            List<JobOfferInfo> list = MangaCache.Get(key) as List<JobOfferInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetJobOfferList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadJobOfferListCache()
        {
            string key = GlobalKey.JOBOFFER_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }

        public void Add(JobOfferInfo entity)
        {
            CommonDataProvider.Instance().AddJobOffer(entity);
            ReloadJobOfferListCache();
        }

        public void Update(JobOfferInfo entity)
        {
            CommonDataProvider.Instance().UpdateJobOffer(entity);
            ReloadJobOfferListCache();
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteJobOffer(ids);
            ReloadJobOfferListCache();
        }


    }
}
