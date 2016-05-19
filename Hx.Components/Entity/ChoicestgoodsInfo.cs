using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Entity
{
    public class ChoicestgoodsInfo
    {
        public int ID { get; set; }

        /// <summary>
        /// 所属公司ID
        /// </summary>
        public int CorporationID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string Picpath { get; set; }

        /// <summary>
        /// 适用车型
        /// </summary>
        public string Inpoint { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
