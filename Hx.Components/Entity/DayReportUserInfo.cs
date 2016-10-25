using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;
using Hx.Tools;
using Newtonsoft.Json;

namespace Hx.Components.Entity
{
    [Serializable]
    public class DayReportUserInfo : ExtendedAttributes
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [JsonProperty("id")]     
        public int ID { get; set; }

        /// <summary>
        /// oa系统用户标识
        /// </summary>   
        [JsonProperty("usertag")]     
        public string UserTag { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        [JsonProperty("username")]   
        public string UserName { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        [JsonProperty("dayreportdep")]   
        public DayReportDep DayReportDep { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        [JsonProperty("corporationid")]   
        public int CorporationID { get; set; }

        /// <summary>
        /// 分公司名
        /// </summary>
        [JsonProperty("corporationname")]   
        public string CorporationName { get; set; }

        /// <summary>
        /// 日报表录入修改权限
        /// </summary>
        [JsonIgnore]
        public string AllowModify
        {
            get { return GetString("AllowModify", ""); }
            set { SetExtendedAttribute("AllowModify", value); }
        }

        /// <summary>
        /// 年报数据录入权限
        /// </summary>
        [JsonIgnore]
        public string AllowYearGahterInput
        {
            get { return GetString("AllowYearGahterInput", ""); }
            set { SetExtendedAttribute("AllowYearGahterInput", value); }
        }

        /// <summary>
        /// 月报汇总
        /// </summary>
        [JsonIgnore]
        public string ReportGather
        {
            get { return GetString("ReportGather", ""); }
            set { SetExtendedAttribute("ReportGather", value); }
        }

        /// <summary>
        /// 年报汇总
        /// </summary>
        [JsonIgnore]
        public string YearGather
        {
            get { return GetString("YearGather", ""); }
            set { SetExtendedAttribute("YearGather", value); }
        }

        /// <summary>
        /// 日报表录入部门权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportDepPowerSetting
        {
            get { return GetString("DayReportDepPowerSetting", ""); }
            set { SetExtendedAttribute("DayReportDepPowerSetting", value); }
        }

        /// <summary>
        /// 日报表录入模块权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportModulePowerSetting
        {
            get { return GetString("DayReportModulePowerSetting", ""); }
            set { SetExtendedAttribute("DayReportModulePowerSetting", value); }
        }

        /// <summary>
        /// 日报表录入公司权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportCorpPowerSetting
        {
            get { return GetString("DayReportCorpPowerSetting", ""); }
            set { SetExtendedAttribute("DayReportCorpPowerSetting", value); }
        }

        /// <summary>
        /// 月度目标公司权限设置
        /// </summary>
        [JsonIgnore]
        public string MonthlyTargetCorpPowerSetting
        {
            get { return GetString("MonthlyTargetCorpPowerSetting", ""); }
            set { SetExtendedAttribute("MonthlyTargetCorpPowerSetting", value); }
        }

        /// <summary>
        /// 月度目标部门权限设置
        /// </summary>
        [JsonIgnore]
        public string MonthlyTargetDepPowerSetting
        {
            get { return GetString("MonthlyTargetDepPowerSetting", ""); }
            set { SetExtendedAttribute("MonthlyTargetDepPowerSetting", value); }
        }

        /// <summary>
        /// 月报查看公司权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportViewCorpPowerSetting
        {
            get { return GetString("DayReportViewCorpPowerSetting", ""); }
            set { SetExtendedAttribute("DayReportViewCorpPowerSetting", value); }
        }

        /// <summary>
        /// 月报查看部门权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportViewDepPowerSetting
        {
            get { return GetString("DayReportViewDepPowerSetting", ""); }
            set { SetExtendedAttribute("DayReportViewDepPowerSetting", value); }
        }

        /// <summary>
        /// 日报审核公司权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportCheckCorpPowerSetting
        {
            get { return GetString("DayReportCheckCorpPowerSetting", ""); }
            set { SetExtendedAttribute("DayReportCheckCorpPowerSetting", value); }
        }

        /// <summary>
        /// 日报审核部门权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportCheckDepPowerSetting
        {
            get { return GetString("DayReportCheckDepPowerSetting", ""); }
            set { SetExtendedAttribute("DayReportCheckDepPowerSetting", value); }
        }

        /// <summary>
        /// 月度预算权限设置
        /// </summary>
        [JsonIgnore]
        public string DayReportMonthTargetPrePowerSetting
        {
            get { return GetString("DayReportMonthTargetPrePowerSetting", ""); }
            set { SetExtendedAttribute("DayReportMonthTargetPrePowerSetting", value); }
        }

        /// <summary>
        /// CRM报表导出权限设置
        /// </summary>
        [JsonIgnore]
        public string CRMReportExportPowerSetting
        {
            get { return GetString("CRMReportExportPowerSetting", "0"); }
            set { SetExtendedAttribute("CRMReportExportPowerSetting", value); }
        }

        /// <summary>
        /// CRM报表录入权限设置
        /// </summary>
        [JsonIgnore]
        public string CRMReportInputPowerSetting
        {
            get { return GetString("CRMReportInputPowerSetting", ""); }
            set { SetExtendedAttribute("CRMReportInputPowerSetting", value); }
        }
    }
}
