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
    }
}
