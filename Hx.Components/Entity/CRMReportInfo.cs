using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    [Serializable]
    public class CRMReportInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("createtime")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("lastupdateuser")]
        public string LastUpdateUser { get; set; }

        [JsonProperty("lastupdatetime")]
        public DateTime LastUpdateTime { get; set; }

        [JsonProperty("corporationid")]
        public int CorporationID { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("crmreporttype")]
        public CRMReportType CRMReportType { get; set; }

        #region 展厅来店(电)客流量登记表

        /// <summary>
        /// 到店方式
        /// </summary>
        [JsonIgnore]
        public int CFVisitway
        {
            get { return GetInt("CFVisitway", 0); }
            set { SetExtendedAttribute("CFVisitway", value.ToString()); }
        }

        /// <summary>
        /// 来访时间
        /// </summary>
        [JsonIgnore]
        public string CFVisitTime
        {
            get { return GetString("CFVisitTime", ""); }
            set { SetExtendedAttribute("CFVisitTime", value); }
        }

        /// <summary>
        /// 离去时间
        /// </summary>
        [JsonIgnore]
        public string CFLeaveTime
        {
            get { return GetString("CFLeaveTime", ""); }
            set { SetExtendedAttribute("CFLeaveTime", value); }
        }

        /// <summary>
        /// 销售顾问
        /// </summary>
        [JsonIgnore]
        public string CFReceiver
        {
            get { return GetString("CFReceiver", ""); }
            set { SetExtendedAttribute("CFReceiver", value); }
        }

        /// <summary>
        /// 客户姓名
        /// </summary>
        [JsonIgnore]
        public string CFCustomerName
        {
            get { return GetString("CFCustomerName", ""); }
            set { SetExtendedAttribute("CFCustomerName", value); }
        }

        /// <summary>
        /// 来访性质
        /// </summary>
        [JsonIgnore]
        public int CFVisitNature
        {
            get { return GetInt("CFVisitNature", 0); }
            set { SetExtendedAttribute("CFVisitNature", value.ToString()); }
        }

        /// <summary>
        /// 联系电话
        /// </summary>
        [JsonIgnore]
        public string CFPhoneNum
        {
            get { return GetString("CFPhoneNum", ""); }
            set { SetExtendedAttribute("CFPhoneNum", value); }
        }

        /// <summary>
        /// 意向车型
        /// </summary>
        [JsonIgnore]
        public string CFIntentionModel
        {
            get { return GetString("CFIntentionModel", ""); }
            set { SetExtendedAttribute("CFIntentionModel", value); }
        }

        /// <summary>
        /// 来店渠道
        /// </summary>
        [JsonIgnore]
        public int CFVisitChannel
        {
            get { return GetInt("CFVisitChannel", 0); }
            set { SetExtendedAttribute("CFVisitChannel", value.ToString()); }
        }

        /// <summary>
        /// 购入类型
        /// </summary>
        [JsonIgnore]
        public int CFBuyType
        {
            get { return GetInt("CFBuyType", 0); }
            set { SetExtendedAttribute("CFBuyType", value.ToString()); }
        }

        /// <summary>
        /// 现用车型
        /// </summary>
        [JsonIgnore]
        public string CFModelInUse
        {
            get { return GetString("CFModelInUse", ""); }
            set { SetExtendedAttribute("CFModelInUse", value); }
        }

        /// <summary>
        /// 是否贷款
        /// </summary>
        [JsonIgnore]
        public int CFIsLoan
        {
            get { return GetInt("CFIsLoan", 0); }
            set { SetExtendedAttribute("CFIsLoan", value.ToString()); }
        }

        /// <summary>
        /// 来自区域
        /// </summary>
        [JsonIgnore]
        public string CFFromArea
        {
            get { return GetString("CFFromArea", ""); }
            set { SetExtendedAttribute("CFFromArea", value); }
        }

        /// <summary>
        /// 级别
        /// </summary>
        [JsonIgnore]
        public int CFLevel
        {
            get { return GetInt("CFLevel", 0); }
            set { SetExtendedAttribute("CFLevel", value.ToString()); }
        }

        /// <summary>
        /// 建卡情况
        /// </summary>
        [JsonIgnore]
        public int CFCardInfo
        {
            get { return GetInt("CFCardInfo", 0); }
            set { SetExtendedAttribute("CFCardInfo", value.ToString()); }
        }

        /// <summary>
        /// 是否试乘试驾
        /// </summary>
        [JsonIgnore]
        public int CFIsRide
        {
            get { return GetInt("CFIsRide", 0); }
            set { SetExtendedAttribute("CFIsRide", value.ToString()); }
        }

        /// <summary>
        /// 订单
        /// </summary>
        [JsonIgnore]
        public int CFIsOrder
        {
            get { return GetInt("CFIsOrder", 0); }
            set { SetExtendedAttribute("CFIsOrder", value.ToString()); }
        }

        /// <summary>
        /// 是否开发票
        /// </summary>
        [JsonIgnore]
        public int CFIsInvoice
        {
            get { return GetInt("CFIsInvoice", 0); }
            set { SetExtendedAttribute("CFIsInvoice", value.ToString()); }
        }

        /// <summary>
        /// 是否交车
        /// </summary>
        [JsonIgnore]
        public int CFIsTurnover
        {
            get { return GetInt("CFIsTurnover", 0); }
            set { SetExtendedAttribute("CFIsTurnover", value.ToString()); }
        }

        #endregion

        #region 活动或外出访问客户信息

        /// <summary>
        /// 客户性质
        /// </summary>
        [JsonIgnore]
        public int OVCustomerNature
        {
            get { return GetInt("OVCustomerNature", 0); }
            set { SetExtendedAttribute("OVCustomerNature", value.ToString()); }
        }
        
        /// <summary>
        /// 活动名称
        /// </summary>
        [JsonIgnore]
        public string OVActiveName
        {
            get { return GetString("OVActiveName", ""); }
            set { SetExtendedAttribute("OVActiveName", value); }
        }
        
        /// <summary>
        /// 客户姓名
        /// </summary>
        [JsonIgnore]
        public string OVCustomerName
        {
            get { return GetString("OVCustomerName", ""); }
            set { SetExtendedAttribute("OVCustomerName", value); }
        }
        
        /// <summary>
        /// 客户电话
        /// </summary>
        [JsonIgnore]
        public string OVPhoneNum
        {
            get { return GetString("OVPhoneNum", ""); }
            set { SetExtendedAttribute("OVPhoneNum", value); }
        }
        
        /// <summary>
        /// 销售顾问
        /// </summary>
        [JsonIgnore]
        public string OVReceiver
        {
            get { return GetString("OVReceiver", ""); }
            set { SetExtendedAttribute("OVReceiver", value); }
        }
        
        /// <summary>
        /// 拟购车型
        /// </summary>
        [JsonIgnore]
        public string OVIntentionModel
        {
            get { return GetString("OVIntentionModel", ""); }
            set { SetExtendedAttribute("OVIntentionModel", value); }
        }

        /// <summary>
        /// 客户信息来源
        /// </summary>
        [JsonIgnore]
        public int OVVisitChannel
        {
            get { return GetInt("OVVisitChannel", 0); }
            set { SetExtendedAttribute("OVVisitChannel", value.ToString()); }
        }

        /// <summary>
        /// 意向级别
        /// </summary>
        [JsonIgnore]
        public int OVLevel
        {
            get { return GetInt("OVLevel", 0); }
            set { SetExtendedAttribute("OVLevel", value.ToString()); }
        }

        /// <summary>
        /// 是否试乘试驾
        /// </summary>
        [JsonIgnore]
        public int OVIsRide
        {
            get { return GetInt("OVIsRide", 0); }
            set { SetExtendedAttribute("OVIsRide", value.ToString()); }
        }

        /// <summary>
        /// 建卡情况
        /// </summary>
        [JsonIgnore]
        public int OVCardInfo
        {
            get { return GetInt("OVCardInfo", 0); }
            set { SetExtendedAttribute("OVCardInfo", value.ToString()); }
        }

        /// <summary>
        /// 下次跟踪时间
        /// </summary>
        [JsonIgnore]
        public string OVVisitNexttime
        {
            get { return GetString("OVVisitNexttime", ""); }
            set { SetExtendedAttribute("OVVisitNexttime", value); }
        }

        #endregion
    }
}
