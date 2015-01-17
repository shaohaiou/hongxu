using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Car.Entity
{
    /// <summary>
    /// 车辆品牌
    /// </summary>
    public class CarBrandInfo
    {
        public int ID { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 首字母
        /// </summary>
        public string NameIndex { get; set; }

        /// <summary>
        /// 列表绑定字段
        /// </summary>
        public string BindName { get { return string.Format("{0} {1}" ,NameIndex.ToUpper(),Name); } }
    }
}
