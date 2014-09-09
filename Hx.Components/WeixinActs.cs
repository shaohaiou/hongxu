using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Entity;
using Hx.Components.Providers;
using Hx.Components.Query;

namespace Hx.Components
{
    public class WeixinActs
    {
        #region 单例
        private static object sync_creater = new object();

        private static WeixinActs _instance;
        public static WeixinActs Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync_creater)
                    {
                        if (_instance == null)
                            _instance = new WeixinActs();
                    }
                }
                return _instance;
            }
        }

        #endregion

        public WeixinActInfo GetModel(string openid)
        {
            return CommonDataProvider.Instance().GetWeixinActInfo(openid);
        }

        public bool Add(WeixinActInfo entity)
        {
            return CommonDataProvider.Instance().AddWeixinAct(entity);
        }

        public int Dianzan(string openid, string vopenid)
        {
            return CommonDataProvider.Instance().WeixinDianzan(openid, vopenid);
        }

        public List<WeixinActInfo> GetList(int pageindex, int pagesize, WeixinActQuery query, ref int recordcount)
        {
            return CommonDataProvider.Instance().GetWeixinActList(pageindex, pagesize, query, ref recordcount);
        }
    }
}
