using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Car.Entity
{
    /// <summary>
    /// 公司
    /// </summary>
    [Serializable]
    public class CorporationInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 相关车辆品牌
        /// </summary>
        [JsonIgnore]
        public string CarBrand
        {
            get { return GetString("CarBrand", ""); }
            set { SetExtendedAttribute("CarBrand", value); }
        }

        /// <summary>
        /// 默认银行
        /// </summary>
        [JsonIgnore]
        public string Bank
        {
            get { return GetString("Bank", ""); }
            set { SetExtendedAttribute("Bank", value); }
        }

        /// <summary>
        /// 默认利率
        /// </summary>
        [JsonIgnore]
        public string BankProfitMargin
        {
            get { return GetString("BankProfitMargin", ""); }
            set { SetExtendedAttribute("BankProfitMargin", value); }
        }

        /// <summary>
        /// 有效车型设置
        /// </summary>
        [JsonIgnore]
        public string AutomotivetypeSetting
        {
            get { return GetString("AutomotivetypeSetting", ""); }
            set { SetExtendedAttribute("AutomotivetypeSetting", value); }
        }

        /// <summary>
        /// 默认上牌费
        /// </summary>
        [JsonIgnore]
        public string AutoCspf
        {
            get { return GetString("AutoSpf", ""); }
            set { SetExtendedAttribute("AutoSpf", value); }
        }

        /// <summary>
        /// 默认履约风险金点数
        /// </summary>
        [JsonIgnore]
        public string AutoClyfxj
        {
            get { return GetString("AutoClyfxj", ""); }
            set { SetExtendedAttribute("AutoClyfxj", value); }
        }

        /// <summary>
        /// 默认续保押金点数
        /// </summary>
        [JsonIgnore]
        public string AutoCxbyj
        {
            get { return GetString("AutoCxbyj", ""); }
            set { SetExtendedAttribute("AutoCxbyj", value); }
        }

        /// <summary>
        /// 默认第三责任险金额（万）
        /// </summary>
        [JsonIgnore]
        public string AutoCdszrxje
        {
            get { return GetString("AutoCdszrxje", ""); }
            set { SetExtendedAttribute("AutoCdszrxje", value); }
        }

        /// <summary>
        /// 默认交车费金额
        /// </summary>
        [JsonIgnore]
        public string AutoCjcf
        {
            get { return GetString("AutoCjcf", ""); }
            set { SetExtendedAttribute("AutoCjcf", value); }
        }

        /// <summary>
        /// 默认代办分期付款手续费、劳务费
        /// </summary>
        [JsonIgnore]
        public string AutoCdbfqfksxf
        {
            get { return GetString("AutoCdbfqfksxf", ""); }
            set { SetExtendedAttribute("AutoCdbfqfksxf", value); }
        }

        /// <summary>
        /// 默认银行利率
        /// </summary>
        [JsonIgnore]
        public string AutoCyhll
        {
            get { return GetString("AutoCyhll", ""); }
            set { SetExtendedAttribute("AutoCyhll", value); }
        }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public int Sort
        {
            get { return GetInt("Sort", 0); }
            set { SetExtendedAttribute("Sort", value.ToString()); }
        }
    }
}
