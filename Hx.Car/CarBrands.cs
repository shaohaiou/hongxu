using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Entity;
using Hx.Car.Providers;
using Hx.Components;
using System.Data;
using Hx.Tools;

namespace Hx.Car
{
    public class CarBrands
    {
        #region 单例

        private static object sync_creater = new object();

        private static CarBrands _instance = null;
        public static CarBrands Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new CarBrands();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public List<CarBrandInfo> GetCarBrandList(bool fromCache = false)
        {
            if (!fromCache)
                return CarDataProvider.Instance().GetCarBrandList().OrderBy(b => b.NameIndex).ToList();

            string key = GlobalKey.CARBRAND_LIST;
            List<CarBrandInfo> list = MangaCache.Get(key) as List<CarBrandInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetCarBrandList().OrderBy(b=>b.NameIndex).ToList();
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadCarBrandCache()
        {
            string key = GlobalKey.CARBRAND_LIST;
            MangaCache.Remove(key);

            GetCarBrandList(true);
        }

        public void RecheckCarBrand()
        {
            string key = GlobalKey.CARBRAND_LIST;
            MangaCache.Remove(key);

            foreach (DataRow row in Cars.Instance.GetCarcChangs().Rows)
            {
                CarBrandInfo entity = new CarBrandInfo
                {
                    Name = row["cChangs"].ToString()
                };
                entity.NameIndex = StrHelper.ConvertE(entity.Name).Substring(0, 1).ToUpper();
                CarBrands.Instance.Add(entity);
            }
            GetCarBrandList(true);
        }

        public List<CarBrandInfo> GetCarBrandListByCorporation(string cid)
        {
            if (string.IsNullOrEmpty(cid) || cid == "-1")
                return GetCarBrandList(true);
            string key = GlobalKey.CARBRANDBYCORPORATION_LIST + "_" + cid;
            List<CarBrandInfo> list = MangaCache.Get(key) as List<CarBrandInfo>;
            if (list == null)
            {
                CorporationInfo corporation = Corporations.Instance.GetModel(DataConvert.SafeInt(cid), true);
                if (corporation != null && !string.IsNullOrEmpty(corporation.CarBrand))
                {
                    string[] brands = corporation.CarBrand.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    list = GetCarBrandList(true);
                    list = list.FindAll(c=>brands.Contains(c.Name)).OrderBy(c=>c.NameIndex).ToList();
                    MangaCache.Max(key, list);
                }
            }
            return list;
        }

        public void ReloadCarBrandCacheByCorporation()
        {
            MangaCache.RemoveByPattern(GlobalKey.CARBRANDBYCORPORATION_LIST);

            foreach (CorporationInfo corporation in Corporations.Instance.GetList(true))
            {
                GetCarBrandListByCorporation(corporation.ID.ToString());
            }
        }

        public void ReloadCarBrandCacheByCorporation(string cid)
        { 
            string key = GlobalKey.CARBRANDBYCORPORATION_LIST + "_" + cid;
            MangaCache.Remove(key);
            Corporations.Instance.ReloadCorporationListCache();
            GetCarBrandListByCorporation(cid);
        }

        public void Add(CarBrandInfo entity)
        {
            CarDataProvider.Instance().AddCarBrand(entity);
        }

        public CarBrandInfo GetModel(int id, bool fromCache = false)
        {
            List<CarBrandInfo> list = GetCarBrandList(fromCache);
            return list.Find(c=>c.ID == id);
        }

        public void UpdateBrand(CarBrandInfo entity)
        {
            CarDataProvider.Instance().UpdateBrand(entity);
        }
    }
}
