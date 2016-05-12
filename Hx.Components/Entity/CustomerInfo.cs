using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Entity
{
    [Serializable]
    public class CustomerInfo
    {
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户手机号
        /// </summary>
        public string CustomerMobile { get; set; }

        /// <summary>
        /// 客户微信号
        /// </summary>
        public string CustomerMicroletter { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        public string Cjh { get; set; }
    }
}
