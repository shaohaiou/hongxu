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
    public class MonthlyTargetInfo : ExtendedAttributes
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("monthunique")]
        public string MonthUnique { get; set; }

        [JsonProperty("corporationid")]
        public int CorporationID { get; set; }

        [JsonProperty("department")]
        public DayReportDep Department { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("createtime")]
        public DateTime CreateTime { get; set; }

        [JsonProperty("lastupdateuser")]
        public string LastUpdateUser { get; set; }

        [JsonProperty("lastupdatetime")]
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 各项指标月度目标
        /// </summary>
        [JsonIgnore]
        public string SCReport
        {
            get { return GetString("SCReport", ""); }
            set { SetExtendedAttribute("SCReport", value); }
        }

        #region 销售部月度目标

        #region 关键指标
        
        /// <summary>
        /// 在途车辆
        /// </summary>
        [JsonIgnore]
        public string XSztcl
        {
            get { return GetString("XSztcl", ""); }
            set { SetExtendedAttribute("XSztcl", value); }
        }

        /// <summary>
        /// 车辆平均单价
        /// </summary>
        [JsonIgnore]
        public string XSclpjdj
        {
            get { return GetString("XSclpjdj", ""); }
            set { SetExtendedAttribute("XSclpjdj", value); }
        }

        /// <summary>
        /// 周转天数
        /// </summary>
        [JsonIgnore]
        public string XSzzts
        {
            get { return GetString("XSzzts", ""); }
            set { SetExtendedAttribute("XSzzts", value); }
        }

        /// <summary>
        /// 厂家虚出台次
        /// </summary>
        [JsonIgnore]
        public string XScjxctc
        {
            get { return GetString("XScjxctc", ""); }
            set { SetExtendedAttribute("XScjxctc", value); }
        }

        /// <summary>
        /// 在库超3个月台次
        /// </summary>
        [JsonIgnore]
        public string XSzkcsgytc
        {
            get { return GetString("XSzkcsgytc", ""); }
            set { SetExtendedAttribute("XSzkcsgytc", value); }
        }

        /// <summary>
        /// 在库超6个月台次
        /// </summary>
        [JsonIgnore]
        public string XSzkclgytc
        {
            get { return GetString("XSzkclgytc", ""); }
            set { SetExtendedAttribute("XSzkclgytc", value); }
        }

        /// <summary>
        /// 在库超1年台次
        /// </summary>
        [JsonIgnore]
        public string XSzkcyntc
        {
            get { return GetString("XSzkcyntc", ""); }
            set { SetExtendedAttribute("XSzkcyntc", value); }
        }

        /// <summary>
        /// 本月整车实际销售额
        /// </summary>
        [JsonIgnore]
        public string XSbyzcsjxse
        {
            get { return GetString("XSbyzcsjxse", ""); }
            set { SetExtendedAttribute("XSbyzcsjxse", value); }
        }

        /// <summary>
        /// 本月整车预算销售额
        /// </summary>
        [JsonIgnore]
        public string XSbyzcysxse
        {
            get { return GetString("XSbyzcysxse", ""); }
            set { SetExtendedAttribute("XSbyzcysxse", value); }
        }

        /// <summary>
        /// 本月整车裸车实际毛利额
        /// </summary>
        [JsonIgnore]
        public string XSbyzclcsjmle
        {
            get { return GetString("XSbyzclcsjmle", ""); }
            set { SetExtendedAttribute("XSbyzclcsjmle", value); }
        }

        /// <summary>
        /// 本月整车裸车预算毛利额
        /// </summary>
        [JsonIgnore]
        public string XSbyzclcysmle
        {
            get { return GetString("XSbyzclcysmle", ""); }
            set { SetExtendedAttribute("XSbyzclcysmle", value); }
        }

        /// <summary>
        /// 本月厂方返利实际收入
        /// </summary>
        [JsonIgnore]
        public string XSbycfflsjsr
        {
            get { return GetString("XSbycfflsjsr", ""); }
            set { SetExtendedAttribute("XSbycfflsjsr", value); }
        }

        /// <summary>
        /// 本月厂方返利预算收入
        /// </summary>
        [JsonIgnore]
        public string XSbycfflyssr
        {
            get { return GetString("XSbycfflyssr", ""); }
            set { SetExtendedAttribute("XSbycfflyssr", value); }
        }

        /// <summary>
        /// 本月厂方金融手续费净收入
        /// </summary>
        [JsonIgnore]
        public string XSbycfjrsxfjsr
        {
            get { return GetString("XSbycfjrsxfjsr", ""); }
            set { SetExtendedAttribute("XSbycfjrsxfjsr", value); }
        }

        /// <summary>
        /// 本月美容交车净收入
        /// </summary>
        [JsonIgnore]
        public string XSbymrjcjsr
        {
            get { return GetString("XSbymrjcjsr", ""); }
            set { SetExtendedAttribute("XSbymrjcjsr", value); }
        }

        /// <summary>
        /// 本月精品毛利
        /// </summary>
        [JsonIgnore]
        public string XSbyjpmlsr
        {
            get { return GetString("XSbyjpmlsr", ""); }
            set { SetExtendedAttribute("XSbyjpmlsr", value); }
        }

        /// <summary>
        /// 本月精品预算毛利
        /// </summary>
        [JsonIgnore]
        public string XSbyjpmlyssr
        {
            get { return GetString("XSbyjpmlyssr", ""); }
            set { SetExtendedAttribute("XSbyjpmlyssr", value); }
        }

        /// <summary>
        /// 本月延保毛利
        /// </summary>
        [JsonIgnore]
        public string XSbyybml
        {
            get { return GetString("XSbyybml", ""); }
            set { SetExtendedAttribute("XSbyybml", value); }
        }

        /// <summary>
        /// 本月终生免费保养毛利
        /// </summary>
        [JsonIgnore]
        public string XSbyzsmfbyml
        {
            get { return GetString("XSbyzsmfbyml", ""); }
            set { SetExtendedAttribute("XSbyzsmfbyml", value); }
        }

        /// <summary>
        /// 本月其他收入
        /// </summary>
        [JsonIgnore]
        public string XSbyqtsr
        {
            get { return GetString("XSbyqtsr", ""); }
            set { SetExtendedAttribute("XSbyqtsr", value); }
        }

        /// <summary>
        /// 展厅占比
        /// </summary>
        [JsonIgnore]
        public string XSztzb
        {
            get { return GetString("XSztzb", ""); }
            set { SetExtendedAttribute("XSztzb", value); }
        }

        /// <summary>
        /// 展厅留档率
        /// </summary>
        [JsonIgnore]
        public string XSztldl
        {
            get { return GetString("XSztldl", ""); }
            set { SetExtendedAttribute("XSztldl", value); }
        }

        /// <summary>
        /// 展厅成交率
        /// </summary>
        [JsonIgnore]
        public string XSztcjl
        {
            get { return GetString("XSztcjl", ""); }
            set { SetExtendedAttribute("XSztcjl", value); }
        }

        /// <summary>
        /// 上牌率
        /// </summary>
        [JsonIgnore]
        public string XSspl
        {
            get { return GetString("XSspl", ""); }
            set { SetExtendedAttribute("XSspl", value); }
        }

        /// <summary>
        /// 展厅保险率
        /// </summary>
        [JsonIgnore]
        public string XSztbxl
        {
            get { return GetString("XSztbxl", ""); }
            set { SetExtendedAttribute("XSztbxl", value); }
        }

        /// <summary>
        /// 美容交车率
        /// </summary>
        [JsonIgnore]
        public string XSmrjcl
        {
            get { return GetString("XSmrjcl", ""); }
            set { SetExtendedAttribute("XSmrjcl", value); }
        }

        /// <summary>
        /// 延保渗透率
        /// </summary>
        [JsonIgnore]
        public string XSybstl
        {
            get { return GetString("XSybstl", ""); }
            set { SetExtendedAttribute("XSybstl", value); }
        }

        /// <summary>
        /// 展厅精品前装率
        /// </summary>
        [JsonIgnore]
        public string XSztjpqzl
        {
            get { return GetString("XSztjpqzl", ""); }
            set { SetExtendedAttribute("XSztjpqzl", value); }
        }

        /// <summary>
        /// 按揭率
        /// </summary>
        [JsonIgnore]
        public string XSajl
        {
            get { return GetString("XSajl", ""); }
            set { SetExtendedAttribute("XSajl", value); }
        }

        /// <summary>
        /// 免费保养渗透率
        /// </summary>
        [JsonIgnore]
        public string XSmfbystl
        {
            get { return GetString("XSmfbystl", ""); }
            set { SetExtendedAttribute("XSmfbystl", value); }
        }

        /// <summary>
        /// 总销售台次
        /// </summary>
        [JsonIgnore]
        public string XSzxstc
        {
            get { return GetString("XSzxstc", ""); }
            set { SetExtendedAttribute("XSzxstc", value); }
        }

        /// <summary>
        /// 上牌单台
        /// </summary>
        [JsonIgnore]
        public string XSspdt
        {
            get { return GetString("XSspdt", ""); }
            set { SetExtendedAttribute("XSspdt", value); }
        }

        /// <summary>
        /// 展厅保险单台
        /// </summary>
        [JsonIgnore]
        public string XSztbxdt
        {
            get { return GetString("XSztbxdt", ""); }
            set { SetExtendedAttribute("XSztbxdt", value); }
        }

        /// <summary>
        /// 美容单台
        /// </summary>
        [JsonIgnore]
        public string XSmrdt
        {
            get { return GetString("XSmrdt", ""); }
            set { SetExtendedAttribute("XSmrdt", value); }
        }

        /// <summary>
        /// 展厅精品平均单台
        /// </summary>
        [JsonIgnore]
        public string XSztjppjdt
        {
            get { return GetString("XSztjppjdt", ""); }
            set { SetExtendedAttribute("XSztjppjdt", value); }
        }

        /// <summary>
        /// 二网精品平均单台
        /// </summary>
        [JsonIgnore]
        public string XSewjppjdt
        {
            get { return GetString("XSewjppjdt", ""); }
            set { SetExtendedAttribute("XSewjppjdt", value); }
        }

        /// <summary>
        /// 销售置换台次
        /// </summary>
        [JsonIgnore]
        public string XSxszhtc
        {
            get { return GetString("XSxszhtc", ""); }
            set { SetExtendedAttribute("XSxszhtc", value); }
        }

        /// <summary>
        /// 按揭平均单台
        /// </summary>
        [JsonIgnore]
        public string XSajpjdt
        {
            get { return GetString("XSajpjdt", ""); }
            set { SetExtendedAttribute("XSajpjdt", value); }
        }

        /// <summary>
        /// 免费保养单台
        /// </summary>
        [JsonIgnore]
        public string XSmfbydt
        {
            get { return GetString("XSmfbydt", ""); }
            set { SetExtendedAttribute("XSmfbydt", value); }
        }

        /// <summary>
        /// 转介绍率
        /// </summary>
        [JsonIgnore]
        public string XSzjsl
        {
            get { return GetString("XSzjsl", ""); }
            set { SetExtendedAttribute("XSzjsl", value); }
        }

        /// <summary>
        /// 转介绍率
        /// </summary>
        [JsonIgnore]
        public string XSblxstl
        {
            get { return GetString("XSblxstl", ""); }
            set { SetExtendedAttribute("XSblxstl", value); }
        }

        /// <summary>
        /// 他品牌销售台次
        /// </summary>
        [JsonIgnore]
        public string XStppxstc
        {
            get { return GetString("XStppxstc", ""); }
            set { SetExtendedAttribute("XStppxstc", value); }
        }

        /// <summary>
        /// 他品牌单车毛利
        /// </summary>
        [JsonIgnore]
        public string XStppdcml
        {
            get { return GetString("XStppdcml", ""); }
            set { SetExtendedAttribute("XStppdcml", value); }
        }

        /// <summary>
        /// 他品牌综合毛利
        /// </summary>
        [JsonIgnore]
        public string XStppzhml
        {
            get { return GetString("XStppzhml", ""); }
            set { SetExtendedAttribute("XStppzhml", value); }
        }

        /// <summary>
        /// 他品牌平均单台
        /// </summary>
        [JsonIgnore]
        public string XStpppjdt
        {
            get { return GetString("XStpppjdt", ""); }
            set { SetExtendedAttribute("XStpppjdt", value); }
        }



        #endregion

        #endregion

        #region 售后部月度目标

        #region 关键指标

        /// <summary>
        /// 微信客户总数
        /// </summary>
        [JsonIgnore]
        public string SHwxkhzs
        {
            get { return GetString("SHwxkhzs", ""); }
            set { SetExtendedAttribute("SHwxkhzs", value); }
        }

        /// <summary>
        /// 本月一般维修额
        /// </summary>
        [JsonIgnore]
        public string SHbyybwxe
        {
            get { return GetString("SHbyybwxe", ""); }
            set { SetExtendedAttribute("SHbyybwxe", value); }
        }

        /// <summary>
        /// 本月首保索赔额
        /// </summary>
        [JsonIgnore]
        public string SHbysbspe
        {
            get { return GetString("SHbysbspe", ""); }
            set { SetExtendedAttribute("SHbysbspe", value); }
        }

        /// <summary>
        /// 本月他品牌收入
        /// </summary>
        [JsonIgnore]
        public string SHbytppsr
        {
            get { return GetString("SHbytppsr", ""); }
            set { SetExtendedAttribute("SHbytppsr", value); }
        }

        /// <summary>
        /// 本月维修毛利额
        /// </summary>
        [JsonIgnore]
        public string SHbywxmle
        {
            get { return GetString("SHbywxmle", ""); }
            set { SetExtendedAttribute("SHbywxmle", value); }
        }

        /// <summary>
        /// 本月维修毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbywxmll
        {
            get { return GetString("SHbywxmll", ""); }
            set { SetExtendedAttribute("SHbywxmll", value); }
        }

        /// <summary>
        /// 本月废旧收入
        /// </summary>
        [JsonIgnore]
        public string SHbyfjsr
        {
            get { return GetString("SHbyfjsr", ""); }
            set { SetExtendedAttribute("SHbyfjsr", value); }
        }

        /// <summary>
        /// 本月含废旧的维修实际毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbyhfjdwxsjmll
        {
            get { return GetString("SHbyhfjdwxsjmll", ""); }
            set { SetExtendedAttribute("SHbyhfjdwxsjmll", value); }
        }

        /// <summary>
        /// 本月含废旧的维修预算毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbyhfjdwxysmll
        {
            get { return GetString("SHbyhfjdwxysmll", ""); }
            set { SetExtendedAttribute("SHbyhfjdwxysmll", value); }
        }

        /// <summary>
        /// 本月油漆收入
        /// </summary>
        [JsonIgnore]
        public string SHbyyqsr
        {
            get { return GetString("SHbyyqsr", ""); }
            set { SetExtendedAttribute("SHbyyqsr", value); }
        }

        /// <summary>
        /// 本月油漆成本
        /// </summary>
        [JsonIgnore]
        public string SHbyyqcb
        {
            get { return GetString("SHbyyqcb", ""); }
            set { SetExtendedAttribute("SHbyyqcb", value); }
        }

        /// <summary>
        /// 本月养护产品毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbyyhcpmll
        {
            get { return GetString("SHbyyhcpmll", ""); }
            set { SetExtendedAttribute("SHbyyhcpmll", value); }
        }

        /// <summary>
        /// 本月标准库存金额
        /// </summary>
        [JsonIgnore]
        public string SHbybzkcje
        {
            get { return GetString("SHbybzkcje", ""); }
            set { SetExtendedAttribute("SHbybzkcje", value); }
        }

        /// <summary>
        /// 本月期末实际库存额
        /// </summary>
        [JsonIgnore]
        public string SHbyqmsjkce
        {
            get { return GetString("SHbyqmsjkce", ""); }
            set { SetExtendedAttribute("SHbyqmsjkce", value); }
        }

        /// <summary>
        /// 其中一年以上库存额
        /// </summary>
        [JsonIgnore]
        public string SHqzynyskce
        {
            get { return GetString("SHqzynyskce", ""); }
            set { SetExtendedAttribute("SHqzynyskce", value); }
        }

        /// <summary>
        /// 本月标准的库存度
        /// </summary>
        [JsonIgnore]
        public string SHbybzdkcd
        {
            get { return GetString("SHbybzdkcd", ""); }
            set { SetExtendedAttribute("SHbybzdkcd", value); }
        }

        /// <summary>
        /// 本月实际库存度
        /// </summary>
        [JsonIgnore]
        public string SHbysjkcd
        {
            get { return GetString("SHbysjkcd", ""); }
            set { SetExtendedAttribute("SHbysjkcd", value); }
        }

        /// <summary>
        /// 本月配件毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbypjmll
        {
            get { return GetString("SHbypjmll", ""); }
            set { SetExtendedAttribute("SHbypjmll", value); }
        }

        /// <summary>
        /// 本月事故车毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbysgcmll
        {
            get { return GetString("SHbysgcmll", ""); }
            set { SetExtendedAttribute("SHbysgcmll", value); }
        }

        /// <summary>
        /// 本月一般维修毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbyybwxmll
        {
            get { return GetString("SHbyybwxmll", ""); }
            set { SetExtendedAttribute("SHbyybwxmll", value); }
        }

        /// <summary>
        /// 本月他牌车维修毛利率
        /// </summary>
        [JsonIgnore]
        public string SHbytppwxmll
        {
            get { return GetString("SHbytppwxmll", ""); }
            set { SetExtendedAttribute("SHbytppwxmll", value); }
        }

        /// <summary>
        /// 本月续保返利收入
        /// </summary>
        [JsonIgnore]
        public string SHbyxbflsr
        {
            get { return GetString("SHbyxbflsr", ""); }
            set { SetExtendedAttribute("SHbyxbflsr", value); }
        }

        /// <summary>
        /// 本月续保平均单台净收入
        /// </summary>
        [JsonIgnore]
        public string SHbyxbpjdtjsr
        {
            get { return GetString("SHbyxbpjdtjsr", ""); }
            set { SetExtendedAttribute("SHbyxbpjdtjsr", value); }
        }

        /// <summary>
        /// 本月延保返利收入
        /// </summary>
        [JsonIgnore]
        public string SHbyybflsr
        {
            get { return GetString("SHbyybflsr", ""); }
            set { SetExtendedAttribute("SHbyybflsr", value); }
        }

        /// <summary>
        /// 本月延保平均单台净收入
        /// </summary>
        [JsonIgnore]
        public string SHbyybpjdtjsr
        {
            get { return GetString("SHbyybpjdtjsr", ""); }
            set { SetExtendedAttribute("SHbyybpjdtjsr", value); }
        }

        /// <summary>
        /// 本月导航升级业务平均单台收入
        /// </summary>
        [JsonIgnore]
        public string SHbydhsjywpjdtsr
        {
            get { return GetString("SHbydhsjywpjdtsr", ""); }
            set { SetExtendedAttribute("SHbydhsjywpjdtsr", value); }
        }

        #endregion

        #endregion

        #region 客服部月度目标

        #endregion

        #region 市场部月度目标

        /// <summary>
        /// 上月粉丝量
        /// </summary>
        [JsonIgnore]
        public string SCsyfsl
        {
            get { return GetString("SC上月粉丝量", ""); }
            set { SetExtendedAttribute("SC上月粉丝量", value); }
        }

        #region 关键指标
        
        /// <summary>
        /// 首次到访达成率
        /// </summary>
        [JsonIgnore]
        public string SCscdfdcl
        {
            get { return GetString("SC首次到访达成率", ""); }
            set { SetExtendedAttribute("SC首次到访达成率", value); }
        }

        #endregion

        #endregion

        #region 财务部月度目标

        /// <summary>
        /// 资金余额
        /// </summary>
        [JsonIgnore]
        public string CWzjye
        {
            get { return GetString("CW资金余额", ""); }
            set { SetExtendedAttribute("CW资金余额", value); }
        }

        /// <summary>
        /// POS未到帐
        /// </summary>
        [JsonIgnore]
        public string CWposwdz
        {
            get { return GetString("CWPOS未到帐", ""); }
            set { SetExtendedAttribute("CWPOS未到帐", value); }
        }

        /// <summary>
        /// 银行帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWyhzhye
        {
            get { return GetString("CW银行帐户余额", ""); }
            set { SetExtendedAttribute("CW银行帐户余额", value); }
        }

        /// <summary>
        /// 农行帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWnhzhye
        {
            get { return GetString("CW农行帐户余额", ""); }
            set { SetExtendedAttribute("CW农行帐户余额", value); }
        }

        /// <summary>
        /// 中行帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWzhzhye
        {
            get { return GetString("CW中行帐户余额", ""); }
            set { SetExtendedAttribute("CW中行帐户余额", value); }
        }

        /// <summary>
        /// 工行帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWghzhye
        {
            get { return GetString("CW工行帐户余额", ""); }
            set { SetExtendedAttribute("CW工行帐户余额", value); }
        }

        /// <summary>
        /// 建行帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWjianhzhye
        {
            get { return GetString("CW建行帐户余额", ""); }
            set { SetExtendedAttribute("CW建行帐户余额", value); }
        }

        /// <summary>
        /// 交行帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWjhzhye
        {
            get { return GetString("CW交行帐户余额", ""); }
            set { SetExtendedAttribute("CW交行帐户余额", value); }
        }

        /// <summary>
        /// 民生帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWmszhye
        {
            get { return GetString("CW民生帐户余额", ""); }
            set { SetExtendedAttribute("CW民生帐户余额", value); }
        }

        /// <summary>
        /// 平安帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWpazhye
        {
            get { return GetString("CW平安帐户余额", ""); }
            set { SetExtendedAttribute("CW平安帐户余额", value); }
        }

        /// <summary>
        /// 中信帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWzxzhye
        {
            get { return GetString("CW中信帐户余额", ""); }
            set { SetExtendedAttribute("CW中信帐户余额", value); }
        }

        /// <summary>
        /// 华夏帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWhxzhye
        {
            get { return GetString("CW华夏帐户余额", ""); }
            set { SetExtendedAttribute("CW华夏帐户余额", value); }
        }

        /// <summary>
        /// 浙商帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWzszhye
        {
            get { return GetString("CW浙商帐户余额", ""); }
            set { SetExtendedAttribute("CW浙商帐户余额", value); }
        }

        /// <summary>
        /// 泰隆帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWtlzhye
        {
            get { return GetString("CW泰隆帐户余额", ""); }
            set { SetExtendedAttribute("CW泰隆帐户余额", value); }
        }

        /// <summary>
        /// 其他银行帐户余额
        /// </summary>
        [JsonIgnore]
        public string CWqtyhzhye
        {
            get { return GetString("CW其他银行帐户余额", ""); }
            set { SetExtendedAttribute("CW其他银行帐户余额", value); }
        }

        /// <summary>
        /// 现金合计
        /// </summary>
        [JsonIgnore]
        public string CWxjczhj
        {
            get { return GetString("CW现金合计", ""); }
            set { SetExtendedAttribute("CW现金合计", value); }
        }

        /// <summary>
        /// 留存现金
        /// </summary>
        [JsonIgnore]
        public string CWlcxj
        {
            get { return GetString("CW留存现金", ""); }
            set { SetExtendedAttribute("CW留存现金", value); }
        }

        /// <summary>
        /// 银承、贷款到期
        /// </summary>
        [JsonIgnore]
        public string CWycdkdq
        {
            get { return GetString("CW银承、贷款到期", ""); }
            set { SetExtendedAttribute("CW银承、贷款到期", value); }
        }

        #endregion

        #region 行政部月度目标

        /// <summary>
        /// 第一周迟到人数
        /// </summary>
        [JsonIgnore]
        public string XZdyzcdrs
        {
            get { return GetString("XZ第一周迟到人数", ""); }
            set { SetExtendedAttribute("XZ第一周迟到人数", value); }
        }

        /// <summary>
        /// 第二周迟到人数
        /// </summary>
        [JsonIgnore]
        public string XZdezcdrs
        {
            get { return GetString("XZ第二周迟到人数", ""); }
            set { SetExtendedAttribute("XZ第二周迟到人数", value); }
        }

        /// <summary>
        /// 第三周迟到人数
        /// </summary>
        [JsonIgnore]
        public string XZdszcdrs
        {
            get { return GetString("XZ第三周迟到人数", ""); }
            set { SetExtendedAttribute("XZ第三周迟到人数", value); }
        }

        /// <summary>
        /// 第四周迟到人数
        /// </summary>
        [JsonIgnore]
        public string XZdsizcdrs
        {
            get { return GetString("XZ第四周迟到人数", ""); }
            set { SetExtendedAttribute("XZ第四周迟到人数", value); }
        }

        /// <summary>
        /// 第一周请假人数
        /// </summary>
        [JsonIgnore]
        public string XZdyzqjrs
        {
            get { return GetString("XZ第一周请假人数", ""); }
            set { SetExtendedAttribute("XZ第一周请假人数", value); }
        }

        /// <summary>
        /// 第二周请假人数
        /// </summary>
        [JsonIgnore]
        public string XZdezqjrs
        {
            get { return GetString("XZ第二周请假人数", ""); }
            set { SetExtendedAttribute("XZ第二周请假人数", value); }
        }

        /// <summary>
        /// 第三周请假人数
        /// </summary>
        [JsonIgnore]
        public string XZdszqjrs
        {
            get { return GetString("XZ第三周请假人数", ""); }
            set { SetExtendedAttribute("XZ第三周请假人数", value); }
        }

        /// <summary>
        /// 第四周请假人数
        /// </summary>
        [JsonIgnore]
        public string XZdsizqjrs
        {
            get { return GetString("XZ第四周请假人数", ""); }
            set { SetExtendedAttribute("XZ第四周请假人数", value); }
        }

        /// <summary>
        /// 第一周旷工人数
        /// </summary>
        [JsonIgnore]
        public string XZdyzkgrs
        {
            get { return GetString("XZ第一周旷工人数", ""); }
            set { SetExtendedAttribute("XZ第一周旷工人数", value); }
        }

        /// <summary>
        /// 第二周旷工人数
        /// </summary>
        [JsonIgnore]
        public string XZdezkgrs
        {
            get { return GetString("XZ第二周旷工人数", ""); }
            set { SetExtendedAttribute("XZ第二周旷工人数", value); }
        }

        /// <summary>
        /// 第三周旷工人数
        /// </summary>
        [JsonIgnore]
        public string XZdszkgrs
        {
            get { return GetString("XZ第三周旷工人数", ""); }
            set { SetExtendedAttribute("XZ第三周旷工人数", value); }
        }

        /// <summary>
        /// 第四周旷工人数
        /// </summary>
        [JsonIgnore]
        public string XZdsizkgrs
        {
            get { return GetString("XZ第四周旷工人数", ""); }
            set { SetExtendedAttribute("XZ第四周旷工人数", value); }
        }

        /// <summary>
        /// 第一周出差培训人次
        /// </summary>
        [JsonIgnore]
        public string XZdyzccpxrc
        {
            get { return GetString("XZ第一周出差培训人次", ""); }
            set { SetExtendedAttribute("XZ第一周出差培训人次", value); }
        }

        /// <summary>
        /// 第二周出差培训人次
        /// </summary>
        [JsonIgnore]
        public string XZdezccpxrc
        {
            get { return GetString("XZ第二周出差培训人次", ""); }
            set { SetExtendedAttribute("XZ第二周出差培训人次", value); }
        }

        /// <summary>
        /// 第三周出差培训人次
        /// </summary>
        [JsonIgnore]
        public string XZdszccpxrc
        {
            get { return GetString("XZ第三周出差培训人次", ""); }
            set { SetExtendedAttribute("XZ第三周出差培训人次", value); }
        }

        /// <summary>
        /// 第四周出差培训人次
        /// </summary>
        [JsonIgnore]
        public string XZdsizccpxrc
        {
            get { return GetString("XZ第四周出差培训人次", ""); }
            set { SetExtendedAttribute("XZ第四周出差培训人次", value); }
        }

        /// <summary>
        /// 第一周安全事故损失额
        /// </summary>
        [JsonIgnore]
        public string XZdyzaqsgsse
        {
            get { return GetString("XZ第一周安全事故损失额", ""); }
            set { SetExtendedAttribute("XZ第一周安全事故损失额", value); }
        }

        /// <summary>
        /// 第二周安全事故损失额
        /// </summary>
        [JsonIgnore]
        public string XZdezaqsgsse
        {
            get { return GetString("XZ第二周安全事故损失额", ""); }
            set { SetExtendedAttribute("XZ第二周安全事故损失额", value); }
        }

        /// <summary>
        /// 第三周安全事故损失额
        /// </summary>
        [JsonIgnore]
        public string XZdszaqsgsse
        {
            get { return GetString("XZ第三周安全事故损失额", ""); }
            set { SetExtendedAttribute("XZ第三周安全事故损失额", value); }
        }

        /// <summary>
        /// 第四周安全事故损失额
        /// </summary>
        [JsonIgnore]
        public string XZdsizaqsgsse
        {
            get { return GetString("XZ第四周安全事故损失额", ""); }
            set { SetExtendedAttribute("XZ第四周安全事故损失额", value); }
        }

        /// <summary>
        /// 第一周车辆违章次数
        /// </summary>
        [JsonIgnore]
        public string XZdyzclwzcs
        {
            get { return GetString("XZ第一周车辆违章次数", ""); }
            set { SetExtendedAttribute("XZ第一周车辆违章次数", value); }
        }

        /// <summary>
        /// 第二周车辆违章次数
        /// </summary>
        [JsonIgnore]
        public string XZdezclwzcs
        {
            get { return GetString("XZ第二周车辆违章次数", ""); }
            set { SetExtendedAttribute("XZ第二周车辆违章次数", value); }
        }

        /// <summary>
        /// 第三周车辆违章次数
        /// </summary>
        [JsonIgnore]
        public string XZdszclwzcs
        {
            get { return GetString("XZ第三周车辆违章次数", ""); }
            set { SetExtendedAttribute("XZ第三周车辆违章次数", value); }
        }

        /// <summary>
        /// 第四周车辆违章次数
        /// </summary>
        [JsonIgnore]
        public string XZdsizclwzcs
        {
            get { return GetString("XZ第四周车辆违章次数", ""); }
            set { SetExtendedAttribute("XZ第四周车辆违章次数", value); }
        }

        /// <summary>
        /// 车牌、负责人
        /// </summary>
        [JsonIgnore]
        public string XZcpfzr
        {
            get { return GetString("XZcpfzr", ""); }
            set { SetExtendedAttribute("XZcpfzr", value); }
        }

        /// <summary>
        /// 上月未处理
        /// </summary>
        [JsonIgnore]
        public string XZsywcl
        {
            get { return GetString("XZsywcl", ""); }
            set { SetExtendedAttribute("XZsywcl", value); }
        }

        /// <summary>
        /// 违章
        /// </summary>
        [JsonIgnore]
        public string XZwz
        {
            get { return GetString("XZwz", ""); }
            set { SetExtendedAttribute("XZwz", value); }
        }

        #endregion

        #region DCC部月度目标

        #region 关键指标
        
        #endregion

        #endregion

        #region 二手车部月度目标

        #region 关键指标
        
        #endregion

        #endregion

        #region 精品部月度目标

        /// <summary>
        /// 预算展厅精品毛利额
        /// </summary>
        [JsonIgnore]
        public string JPysztjpmle
        {
            get { return GetString("JPysztjpmle", ""); }
            set { SetExtendedAttribute("JPysztjpmle", value); }
        }

        /// <summary>
        /// 预算网点精品毛利额
        /// </summary>
        [JsonIgnore]
        public string JPyswdjpmle
        {
            get { return GetString("JPyswdjpmle", ""); }
            set { SetExtendedAttribute("JPyswdjpmle", value); }
        }

        /// <summary>
        /// 预算售后精品毛利额
        /// </summary>
        [JsonIgnore]
        public string JPysshjpmle
        {
            get { return GetString("JPysshjpmle", ""); }
            set { SetExtendedAttribute("JPysshjpmle", value); }
        }

        /// <summary>
        /// 预算展厅单车产值
        /// </summary>
        [JsonIgnore]
        public string JPysztdccz
        {
            get { return GetString("JPysztdccz", ""); }
            set { SetExtendedAttribute("JPysztdccz", value); }
        }

        /// <summary>
        /// 预算网点单台产值
        /// </summary>
        [JsonIgnore]
        public string JPyswddtcz
        {
            get { return GetString("JPyswddtcz", ""); }
            set { SetExtendedAttribute("JPyswddtcz", value); }
        }

        /// <summary>
        /// 预算售后单台产值
        /// </summary>
        [JsonIgnore]
        public string JPysshdtcz
        {
            get { return GetString("JPysshdtcz", ""); }
            set { SetExtendedAttribute("JPysshdtcz", value); }
        }

        /// <summary>
        /// 展厅实际精品产值
        /// </summary>
        [JsonIgnore]
        public string JPztsjjpcz
        {
            get { return GetString("JPztsjjpcz", ""); }
            set { SetExtendedAttribute("JPztsjjpcz", value); }
        }

        /// <summary>
        /// 展厅实际的毛利额
        /// </summary>
        [JsonIgnore]
        public string JPztsjmle
        {
            get { return GetString("JPztsjmle", ""); }
            set { SetExtendedAttribute("JPztsjmle", value); }
        }

        /// <summary>
        /// 网点精品毛利额
        /// </summary>
        [JsonIgnore]
        public string JPwdjpmle
        {
            get { return GetString("JPwdjpmle", ""); }
            set { SetExtendedAttribute("JPwdjpmle", value); }
        }

        /// <summary>
        /// 售后精品毛利额
        /// </summary>
        [JsonIgnore]
        public string JPshjpmle
        {
            get { return GetString("JPshjpmle", ""); }
            set { SetExtendedAttribute("JPshjpmle", value); }
        }

        /// <summary>
        /// 期末库存数
        /// </summary>
        [JsonIgnore]
        public string JPqmkcs
        {
            get { return GetString("JPqmkcs", ""); }
            set { SetExtendedAttribute("JPqmkcs", value); }
        }

        /// <summary>
        /// 3个月以上滞销库存
        /// </summary>
        [JsonIgnore]
        public string JPsgyyszxkc
        {
            get { return GetString("JPsgyyszxkc", ""); }
            set { SetExtendedAttribute("JPsgyyszxkc", value); }
        }

        /// <summary>
        /// 1年以上滞销库存
        /// </summary>
        [JsonIgnore]
        public string JPynyszxkc
        {
            get { return GetString("JPynyszxkc", ""); }
            set { SetExtendedAttribute("JPynyszxkc", value); }
        }

        #endregion

        #region 粘性产品月度目标

        /// <summary>
        /// 销售机油套餐渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPxsjytcstl
        {
            get { return GetString("NXCPxsjytcstl", ""); }
            set { SetExtendedAttribute("NXCPxsjytcstl", value); }
        }

        /// <summary>
        /// 销售玻璃无忧服务渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPxsblwyfwstl
        {
            get { return GetString("NXCPxsblwyfwstl", ""); }
            set { SetExtendedAttribute("NXCPxsblwyfwstl", value); }
        }

        /// <summary>
        /// 销售划痕无忧服务渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPxshhwyfwstl
        {
            get { return GetString("NXCPxshhwyfwstl", ""); }
            set { SetExtendedAttribute("NXCPxshhwyfwstl", value); }
        }

        /// <summary>
        /// 销售延保无忧车服务渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPxsybwycfwstl
        {
            get { return GetString("NXCPxsybwycfwstl", ""); }
            set { SetExtendedAttribute("NXCPxsybwycfwstl", value); }
        }

        /// <summary>
        /// 售后机油套餐渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPshjytcstl
        {
            get { return GetString("NXCPshjytcstl", ""); }
            set { SetExtendedAttribute("NXCPshjytcstl", value); }
        }

        /// <summary>
        /// 售后玻璃无忧服务渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPshblwyfwstl
        {
            get { return GetString("NXCPshblwyfwstl", ""); }
            set { SetExtendedAttribute("NXCPshblwyfwstl", value); }
        }

        /// <summary>
        /// 售后划痕无忧服务渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPshhhwyfwstl
        {
            get { return GetString("NXCPshhhwyfwstl", ""); }
            set { SetExtendedAttribute("NXCPshhhwyfwstl", value); }
        }

        /// <summary>
        /// 售后延保无忧车服务渗透率
        /// </summary>
        [JsonIgnore]
        public string NXCPshybwycfwstl
        {
            get { return GetString("NXCPshybwycfwstl", ""); }
            set { SetExtendedAttribute("NXCPshybwycfwstl", value); }
        }

        /// <summary>
        /// 本月机油套餐来厂使用车辆数
        /// </summary>
        [JsonIgnore]
        public string NXCPbyjytclcsycls
        {
            get { return GetString("NXCPbyjytclcsycls", ""); }
            set { SetExtendedAttribute("NXCPbyjytclcsycls", value); }
        }

        /// <summary>
        /// 本月机油套餐使用金额
        /// </summary>
        [JsonIgnore]
        public string NXCPbyjytcsyje
        {
            get { return GetString("NXCPbyjytcsyje", ""); }
            set { SetExtendedAttribute("NXCPbyjytcsyje", value); }
        }

        /// <summary>
        /// 本月划痕无忧服务到期个数
        /// </summary>
        [JsonIgnore]
        public string NXCPbyhhwyfwdqgs
        {
            get { return GetString("NXCPbyhhwyfwdqgs", ""); }
            set { SetExtendedAttribute("NXCPbyhhwyfwdqgs", value); }
        }

        /// <summary>
        /// 本月划痕无忧服务到期金额
        /// </summary>
        [JsonIgnore]
        public string NXCPbyhhwyfwdqje
        {
            get { return GetString("NXCPbyhhwyfwdqje", ""); }
            set { SetExtendedAttribute("NXCPbyhhwyfwdqje", value); }
        }

        /// <summary>
        /// 本月划痕无忧服务到期内赔付个数
        /// </summary>
        [JsonIgnore]
        public string NXCPbyhhwyfwdqnpfgs
        {
            get { return GetString("NXCPbyhhwyfwdqnpfgs", ""); }
            set { SetExtendedAttribute("NXCPbyhhwyfwdqnpfgs", value); }
        }

        /// <summary>
        /// 本月划痕无忧服务到期内赔付金额
        /// </summary>
        [JsonIgnore]
        public string NXCPbyhhwyfwdqnpfje
        {
            get { return GetString("NXCPbyhhwyfwdqnpfje", ""); }
            set { SetExtendedAttribute("NXCPbyhhwyfwdqnpfje", value); }
        }

        /// <summary>
        /// 本月玻璃无忧服务到期个数
        /// </summary>
        [JsonIgnore]
        public string NXCPbyblwyfwdqgs
        {
            get { return GetString("NXCPbyblwyfwdqgs", ""); }
            set { SetExtendedAttribute("NXCPbyblwyfwdqgs", value); }
        }

        /// <summary>
        /// 本月玻璃无忧服务到期金额
        /// </summary>
        [JsonIgnore]
        public string NXCPbyblwyfwdqje
        {
            get { return GetString("NXCPbyblwyfwdqje", ""); }
            set { SetExtendedAttribute("NXCPbyblwyfwdqje", value); }
        }

        /// <summary>
        /// 本月玻璃无忧服务到期内赔付个数
        /// </summary>
        [JsonIgnore]
        public string NXCPbyblwyfwdqnpfgs
        {
            get { return GetString("NXCPbyblwyfwdqnpfgs", ""); }
            set { SetExtendedAttribute("NXCPbyblwyfwdqnpfgs", value); }
        }

        /// <summary>
        /// 本月玻璃无忧服务到期内赔付金额
        /// </summary>
        [JsonIgnore]
        public string NXCPbyblwyfwdqnpfje
        {
            get { return GetString("NXCPbyblwyfwdqnpfje", ""); }
            set { SetExtendedAttribute("NXCPbyblwyfwdqnpfje", value); }
        }

        /// <summary>
        /// 本月延保服务到期个数
        /// </summary>
        [JsonIgnore]
        public string NXCPbyybfwdqgs
        {
            get { return GetString("NXCPbyybfwdqgs", ""); }
            set { SetExtendedAttribute("NXCPbyybfwdqgs", value); }
        }

        /// <summary>
        /// 本月延保服务到期金额
        /// </summary>
        [JsonIgnore]
        public string NXCPbyybfwdqje
        {
            get { return GetString("NXCPbyybfwdqje", ""); }
            set { SetExtendedAttribute("NXCPbyybfwdqje", value); }
        }

        /// <summary>
        /// 本月延保服务到期内赔付个数
        /// </summary>
        [JsonIgnore]
        public string NXCPbyybfwdqnpfgs
        {
            get { return GetString("NXCPbyybfwdqnpfgs", ""); }
            set { SetExtendedAttribute("NXCPbyybfwdqnpfgs", value); }
        }

        /// <summary>
        /// 本月延保服务到期内赔付金额
        /// </summary>
        [JsonIgnore]
        public string NXCPbyybfwdqnpfje
        {
            get { return GetString("NXCPbyybfwdqnpfje", ""); }
            set { SetExtendedAttribute("NXCPbyybfwdqnpfje", value); }
        }

        #endregion

    }
}
