using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;
using Hx.Car.Enum;

namespace Hx.Car.Entity
{
    [Serializable]
    public class JcbMarketrecordInfo : ExtendedAttributes
    {
        public int ID { get; set; }

        /// <summary>
        /// 车辆ID
        /// </summary>
        public int CarID { get; set; }

        /// <summary>
        /// 帐号ID
        /// </summary>
        public int AccountID { get; set; }

        /// <summary>
        /// 营销网站
        /// </summary>
        public JcbSiteType JcbSiteType { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime? UploadTime { get; set; }

        /// <summary>
        /// 查看url
        /// </summary>
        public string ViewUrl { get; set; }

        /// <summary>
        /// 是否已出售
        /// </summary>
        public bool IsSale { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDel { get; set; }
    }
}
