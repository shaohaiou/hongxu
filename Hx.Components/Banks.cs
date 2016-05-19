using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;

namespace Hx.Components
{
    public class Banks
    {
        #region 单例
        private static object sync_creater = new object();

        private static Banks _instance;
        public static Banks Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Banks();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<BankInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetBankList();

            string key = GlobalKey.BANK_LIST;
            List<BankInfo> list = MangaCache.Get(key) as List<BankInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetBankList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        /// <summary>
        /// 根据公司获取数据
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        public List<BankInfo> GetList(int corpid, bool fromCache = false)
        {
            List<BankInfo> list = GetList(fromCache);
            return list.FindAll(l => l.CorporationID == 0 || l.CorporationID == corpid);
        }

        public void ReloadBankListCache()
        {
            string key = GlobalKey.BANK_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }

        public void Add(BankInfo entity)
        {
            CommonDataProvider.Instance().AddBank(entity);
        }

        public void Update(BankInfo entity)
        {
            CommonDataProvider.Instance().UpdateBank(entity);
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteBank(ids);
        }
    }
}
