using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    /// <summary>
    /// 系统设置
    /// </summary>
    [Serializable]
    public class GlobalSettingInfo : ExtendedAttributes
    {
        /// <summary>
        /// 银行利率
        /// </summary>
        [JsonIgnore]
        public string BankProfitMargin
        {
            get { return GetString("BankProfitMargin", ""); }
            set { SetExtendedAttribute("BankProfitMargin", value); }
        }
    }
}
