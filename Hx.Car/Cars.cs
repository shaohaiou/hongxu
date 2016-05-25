using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Hx.Car.Providers;
using Hx.Components;
using Hx.Car.Entity;
using Hx.Car.Query;

namespace Hx.Car
{
    public class Cars
    {
        #region 单例

        private static object sync_creater = new object();

        private static Cars _instance = null;
        public static Cars Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new Cars();
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region 车辆品牌

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        public DataTable GetCarcChangs()
        {
            return CarDataProvider.Instance().GetCarcChangs();
        }

        #endregion

        #region 车辆数据

        /// <summary>
        /// 获取所有汽车数据
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        public List<CarInfo> GetAllCarList(bool fromCache = false)
        {
            if (!fromCache)
            {
                int recordcount = 0;
                return CarDataProvider.Instance().GetCars(1, int.MaxValue, new CarQuery(), out recordcount);
            }

            string key = GlobalKey.CAR_LIST;
            List<CarInfo> list = MangaCache.Get(key) as List<CarInfo>;
            if (list == null)
            {
                int recordcount = 0;
                list = CarDataProvider.Instance().GetCars(1, int.MaxValue, new CarQuery(), out recordcount);
                MangaCache.Max(key, list);
            }
            return list;
        }

        public void ReloadAllCarList()
        {
            string key = GlobalKey.CAR_LIST;
            MangaCache.Remove(key);
            GetAllCarList(true);
        }

        public CarInfo GetCarInfo(int id, bool fromCache = false)
        {
            List<CarInfo> list = GetAllCarList(fromCache);
            return list.Find(c => c.id == id);
        }

        public List<CarInfo> GetCarList(int pageindex, int pagesize, CarQuery query, out int recordcount)
        {
            return CarDataProvider.Instance().GetCars(pageindex, pagesize, query, out recordcount);
        }

        /// <summary>
        /// 获取指定厂商的汽车数据列表
        /// </summary>
        /// <param name="cChanges"></param>
        /// <returns></returns>
        public List<CarInfo> GetCarListBycChangs(string cChangs, bool fromCache = false)
        {
            if (!fromCache)
            {
                int recordcount = 0;
                CarQuery query = new CarQuery();
                query.cChangs = cChangs;
                return GetCarList(1, int.MaxValue, query, out recordcount);
            }
            string key = GlobalKey.CAR_LIST + "_cChangs_" + cChangs;
            List<CarInfo> list = MangaCache.Get(key) as List<CarInfo>;
            if (list == null)
            {
                List<CarInfo> listAll = GetAllCarList(true);
                list = listAll.FindAll(c => c.cChangs == cChangs).OrderByDescending(c=>c.cCxmc).ToList();
                MangaCache.Max(key, list);
            }

            return list;
        }

        public void ReloadCarListBycChangs()
        {
            string key = GlobalKey.CAR_LIST + "_cChangs_";
            MangaCache.RemoveByPattern(key);
            foreach (CarBrandInfo brand in CarBrands.Instance.GetCarBrandList())
            {
                GetCarListBycChangs(brand.Name, true);
            }
        }

        public void Add(CarInfo car)
        {
            CarDataProvider.Instance().AddCar(car);
            Cars.Instance.ReloadAllCarList();
            Cars.Instance.ReloadCarListBycChangs();
        }

        public void Update(CarInfo car, bool haschangebrand)
        {
            CarDataProvider.Instance().UpdateCar(car);
            List<CarInfo> listcar = GetAllCarList(true);
            if (listcar != null && listcar.Exists(c => c.id == car.id))
                listcar[listcar.FindIndex(c => c.id == car.id)] = car;
            if (haschangebrand)
                ReloadCarListBycChangs();
            else
            {
                listcar = GetCarListBycChangs(car.cChangs, true);
                if(listcar != null && listcar.Exists(c => c.id == car.id))
                    listcar[listcar.FindIndex(c => c.id == car.id)] = car;
            }
        }

        #endregion

        #region 侯牌器

        public void CarNumberCommit(string code, string hp)
        {
            CarDataProvider.Instance().CarNumberCommit(code, hp);
        }

        #endregion
    }
}
