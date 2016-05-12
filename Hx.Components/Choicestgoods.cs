using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;

namespace Hx.Components
{
    public class Choicestgoods
    {
        #region 单例
        private static object sync_creater = new object();

        private static Choicestgoods _instance;
        public static Choicestgoods Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Choicestgoods();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<ChoicestgoodsInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CommonDataProvider.Instance().GetChoicestgoodsList();

            string key = GlobalKey.CHOICESTGOODS_LIST;
            List<ChoicestgoodsInfo> list = MangaCache.Get(key) as List<ChoicestgoodsInfo>;
            if (list == null)
            {
                list = CommonDataProvider.Instance().GetChoicestgoodsList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadChoicestgoodsListCache()
        {
            string key = GlobalKey.CHOICESTGOODS_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }

        public void Add(ChoicestgoodsInfo entity)
        {
            CommonDataProvider.Instance().AddChoicestgoods(entity);
        }

        public void Update(ChoicestgoodsInfo entity)
        {
            CommonDataProvider.Instance().UpdateChoicestgoods(entity);
        }

        public void Delete(string ids)
        {
            CommonDataProvider.Instance().DeleteChoicestgoods(ids);
        }
    }
}
