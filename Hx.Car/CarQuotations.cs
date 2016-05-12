using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Entity;
using Hx.Car.Providers;
using Hx.Car.Enum;
using Hx.Components;
using Hx.Car.Query;

namespace Hx.Car
{
    public class CarQuotations
    {
        #region 单例

        private static object sync_creater = new object();

        private static CarQuotations _instance = null;
        public static CarQuotations Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new CarQuotations();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public void Add(CarQuotationInfo entity,bool refreshCache = false)
        {
            CarDataProvider.Instance().AddCarQuotation(entity);

            if (refreshCache && !string.IsNullOrEmpty(entity.CustomerMobile))
            {
                string key = GlobalKey.CARQUOTATION_KEY + "_mobile_" + entity.CustomerMobile + "_qt_" + ((int)entity.CarQuotationType).ToString();
                MangaCache.Remove(key);
                GetList(entity.CustomerMobile, entity.CarQuotationType);
            }
        }

        public CarQuotationInfo GetModel(string mobile,CarQuotationType type)
        {
            CarQuotationInfo entity = null;
            List<CarQuotationInfo> list = GetList(mobile, type);
            entity = list.First();

            return entity;
        }

        public CarQuotationInfo GetModel(int id)
        {
            return CarDataProvider.Instance().GetCarQuotationModel(id);
        }

        public List<CarQuotationInfo> GetList(string mobile, CarQuotationType type)
        {
            string key = GlobalKey.CARQUOTATION_KEY + "_mobile_" + mobile + "_qt_" + ((int)type).ToString();
            List<CarQuotationInfo> list = MangaCache.Get(key) as List<CarQuotationInfo>;
            if (list == null)
            {
                list = CarDataProvider.Instance().GetCarQuotationList(mobile, type);
                if (list != null)
                {
                    list = list.OrderByDescending(c => c.ID).ToList();
                    MangaCache.Add(key, list);
                }
            }
            return list;
        }

        public List<CarQuotationInfo> GetList(int pageindex, int pagesize, CarQuotationQuery query, ref int recordcount)
        {
            return CarDataProvider.Instance().GetCarQuotationList(pageindex, pagesize, query, ref recordcount);
        }

        public void Check(CarQuotationInfo entity)
        {
            CarDataProvider.Instance().CheckCarQuotation(entity);

            if (!string.IsNullOrEmpty(entity.CustomerMobile))
            {
                string key = GlobalKey.CARQUOTATION_KEY + "_mobile_" + entity.CustomerMobile + "_qt_" + ((int)entity.CarQuotationType).ToString();
                MangaCache.Remove(key);
                GetList(entity.CustomerMobile, entity.CarQuotationType);
            }
        }
    }
}
