using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Entity;
using Hx.Car.Providers;
using Hx.Components;

namespace Hx.Car
{
    public class JcbCars
    {
        #region 单例
        private static object sync_creater = new object();

        private static JcbCars _instance;
        public static JcbCars Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new JcbCars();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public int CreateAndUpdate(JcbCarInfo entity)
        {
            return CarDataProvider.Instance().AddJcbCar(entity);
        }

        public List<JcbCarInfo> GetList(bool fromCache = false)
        {
            if (!fromCache)
                return CarDataProvider.Instance().GetJcbCarList().OrderByDescending(c=>c.LastUpdateTime).ToList();

            string key = GlobalKey.JCBCAR_LIST;
            List<JcbCarInfo> list = MangaCache.Get(key) as List<JcbCarInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetJcbCarList().OrderByDescending(c => c.LastUpdateTime).ToList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public JcbCarInfo GetModel(int id,bool fromCache = false)
        {
            List<JcbCarInfo> list = GetList(fromCache);
            return list.Find(c=>c.ID == id);
        }

        public void ReloadListCache()
        {
            string key = GlobalKey.JCBCAR_LIST;
            MangaCache.Remove(key);
            GetList(true);
        }
    }
}
