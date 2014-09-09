using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    /// <summary>
    /// 银行
    /// </summary>
    [Serializable]
    public class BankInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 贷款利率（3年）
        /// </summary>
        [JsonIgnore]
        public string BankProfitMargin3y
        {
            get { return GetString("BankProfitMargin3y", ""); }
            set { SetExtendedAttribute("BankProfitMargin3y", value); }
        }

        /// <summary>
        /// 贷款利率（2年）
        /// </summary>
        [JsonIgnore]
        public string BankProfitMargin2y
        {
            get { return GetString("BankProfitMargin2y", ""); }
            set { SetExtendedAttribute("BankProfitMargin2y", value); }
        }

        /// <summary>
        /// 贷款利率（1年）
        /// </summary>
        [JsonIgnore]
        public string BankProfitMargin1y
        {
            get { return GetString("BankProfitMargin1y", ""); }
            set { SetExtendedAttribute("BankProfitMargin1y", value); }
        }
    }
}
