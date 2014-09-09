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
        /// 展厅占比
        /// </summary>
        [JsonIgnore]
        public string XSztzb
        {
            get { return GetString("XSztzb", ""); }
            set { SetExtendedAttribute("XSztzb", value); }
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
        /// 上牌单台
        /// </summary>
        [JsonIgnore]
        public string XSspdt
        {
            get { return GetString("XSspdt", ""); }
            set { SetExtendedAttribute("XSspdt", value); }
        }

        /// <summary>
        /// 保险率
        /// </summary>
        [JsonIgnore]
        public string XSbxl
        {
            get { return GetString("XSbxl", ""); }
            set { SetExtendedAttribute("XSbxl", value); }
        }

        /// <summary>
        /// 保险单台
        /// </summary>
        [JsonIgnore]
        public string XSbxdt
        {
            get { return GetString("XSbxdt", ""); }
            set { SetExtendedAttribute("XSbxdt", value); }
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
        /// 美容单台
        /// </summary>
        [JsonIgnore]
        public string XSmrdt
        {
            get { return GetString("XSmrdt", ""); }
            set { SetExtendedAttribute("XSmrdt", value); }
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
        /// 展厅精品单台
        /// </summary>
        [JsonIgnore]
        public string XSztjpdt
        {
            get { return GetString("XSztjpdt", ""); }
            set { SetExtendedAttribute("XSztjpdt", value); }
        }

        /// <summary>
        /// 二网精品单台
        /// </summary>
        [JsonIgnore]
        public string XSewjpdt
        {
            get { return GetString("XSewjpdt", ""); }
            set { SetExtendedAttribute("XSewjpdt", value); }
        }

        /// <summary>
        /// 二手车评估率
        /// </summary>
        [JsonIgnore]
        public string XSescpgl
        {
            get { return GetString("XSescpgl", ""); }
            set { SetExtendedAttribute("XSescpgl", value); }
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
        /// 总销数
        /// </summary>
        [JsonIgnore]
        public string XSzxs
        {
            get { return GetString("XSzxs", ""); }
            set { SetExtendedAttribute("XSzxs", value); }
        }

        /// <summary>
        /// 留单成交率
        /// </summary>
        [JsonIgnore]
        public string XSldcjl
        {
            get { return GetString("XSldcjl", ""); }
            set { SetExtendedAttribute("XSldcjl", value); }
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
        /// 免费保养渗透率
        /// </summary>
        [JsonIgnore]
        public string XSmfbystl
        {
            get { return GetString("XSmfbystl", ""); }
            set { SetExtendedAttribute("XSmfbystl", value); }
        }

        /// <summary>
        /// 免费保养平均单台
        /// </summary>
        [JsonIgnore]
        public string XSmfbypjdt
        {
            get { return GetString("XSmfbypjdt", ""); }
            set { SetExtendedAttribute("XSmfbypjdt", value); }
        }

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

        #endregion

        #endregion

        #region 售后部月度目标

        #region 关键指标

        /// <summary>
        /// 来厂台次
        /// </summary>
        [JsonIgnore]
        public string SHlctc
        {
            get { return GetString("SH来厂台次", ""); }
            set { SetExtendedAttribute("SH来厂台次", value); }
        }

        /// <summary>
        /// 预约率
        /// </summary>
        [JsonIgnore]
        public string SHyyl
        {
            get { return GetString("SH预约率", ""); }
            set { SetExtendedAttribute("SH预约率", value); }
        }

        /// <summary>
        /// 产值达成
        /// </summary>
        [JsonIgnore]
        public string SHczdc
        {
            get { return GetString("SH产值达成", ""); }
            set { SetExtendedAttribute("SH产值达成", value); }
        }

        /// <summary>
        /// 事故产值率
        /// </summary>
        [JsonIgnore]
        public string SHsgczl
        {
            get { return GetString("SH事故产值率", ""); }
            set { SetExtendedAttribute("SH事故产值率", value); }
        }

        /// <summary>
        /// 养护比例
        /// </summary>
        [JsonIgnore]
        public string SHyhbl
        {
            get { return GetString("SH养护比例", ""); }
            set { SetExtendedAttribute("SH养护比例", value); }
        }

        /// <summary>
        /// 事故首次成功率
        /// </summary>
        [JsonIgnore]
        public string SHsgsccgl
        {
            get { return GetString("SH事故首次成功率", ""); }
            set { SetExtendedAttribute("SH事故首次成功率", value); }
        }

        /// <summary>
        /// 事故车短信成功率
        /// </summary>
        [JsonIgnore]
        public string SHsgcdxcgl
        {
            get { return GetString("SH事故车短信成功率", ""); }
            set { SetExtendedAttribute("SH事故车短信成功率", value); }
        }

        /// <summary>
        /// 美容比例
        /// </summary>
        [JsonIgnore]
        public string SHmrbl
        {
            get { return GetString("SH美容比例", ""); }
            set { SetExtendedAttribute("SH美容比例", value); }
        }

        /// <summary>
        /// 索赔成功率
        /// </summary>
        [JsonIgnore]
        public string SHspcgl
        {
            get { return GetString("SH索赔成功率", ""); }
            set { SetExtendedAttribute("SH索赔成功率", value); }
        }

        /// <summary>
        /// 内返率
        /// </summary>
        [JsonIgnore]
        public string SHnfl
        {
            get { return GetString("SH内返率", ""); }
            set { SetExtendedAttribute("SH内返率", value); }
        }

        /// <summary>
        /// 供货及时率
        /// </summary>
        [JsonIgnore]
        public string SHghjsl
        {
            get { return GetString("SH供货及时率", ""); }
            set { SetExtendedAttribute("SH供货及时率", value); }
        }

        /// <summary>
        /// 事故再次成功率
        /// </summary>
        [JsonIgnore]
        public string SHsgzccgl
        {
            get { return GetString("SH事故再次成功率", ""); }
            set { SetExtendedAttribute("SH事故再次成功率", value); }
        }

        /// <summary>
        /// 单台产值
        /// </summary>
        [JsonIgnore]
        public string SHdtcz
        {
            get { return GetString("SH单台产值", ""); }
            set { SetExtendedAttribute("SH单台产值", value); }
        }

        /// <summary>
        /// 保养单台产值
        /// </summary>
        [JsonIgnore]
        public string SHbydtcz
        {
            get { return GetString("SH保养单台产值", ""); }
            set { SetExtendedAttribute("SH保养单台产值", value); }
        }

        /// <summary>
        /// 保养台次占比
        /// </summary>
        [JsonIgnore]
        public string SHbytczb
        {
            get { return GetString("SH保养台次占比", ""); }
            set { SetExtendedAttribute("SH保养台次占比", value); }
        }

        /// <summary>
        /// 美容单台产值
        /// </summary>
        [JsonIgnore]
        public string SHmrdtcz
        {
            get { return GetString("SH美容单台产值", ""); }
            set { SetExtendedAttribute("SH美容单台产值", value); }
        }


        /// <summary>
        /// 延保达成率
        /// </summary>
        [JsonIgnore]
        public string SHybdcl
        {
            get { return GetString("SH延保达成率", ""); }
            set { SetExtendedAttribute("SH延保达成率", value); }
        }

        /// <summary>
        /// 出厂台次
        /// </summary>
        [JsonIgnore]
        public string SHcctc
        {
            get { return GetString("SH出厂台次", ""); }
            set { SetExtendedAttribute("SH出厂台次", value); }
        }

        #endregion

        #endregion

        #region 客服部月度目标

        #endregion

        #region 市场部月度目标

        /// <summary>
        /// 新增总线索量
        /// </summary>
        [JsonIgnore]
        public string SCxzzxsl
        {
            get { return GetString("SC新增总线索量", ""); }
            set { SetExtendedAttribute("SC新增总线索量", value); }
        }

        /// <summary>
        /// 新增有效线索量
        /// </summary>
        [JsonIgnore]
        public string SCxzyxxsl
        {
            get { return GetString("SC新增有效线索量", ""); }
            set { SetExtendedAttribute("SC新增有效线索量", value); }
        }

        #region 关键指标

        /// <summary>
        /// 线索达成率
        /// </summary>
        [JsonIgnore]
        public string SCxsdcl
        {
            get { return GetString("SC线索达成率", ""); }
            set { SetExtendedAttribute("SC线索达成率", value); }
        }

        /// <summary>
        /// 首次到访达成率
        /// </summary>
        [JsonIgnore]
        public string SCscdfdcl
        {
            get { return GetString("SC首次到访达成率", ""); }
            set { SetExtendedAttribute("SC首次到访达成率", value); }
        }

        /// <summary>
        /// 首访建档率
        /// </summary>
        [JsonIgnore]
        public string SCsfjdl
        {
            get { return GetString("SC首访建档率", ""); }
            set { SetExtendedAttribute("SC首访建档率", value); }
        }

        /// <summary>
        /// 网络后台线索达成率
        /// </summary>
        [JsonIgnore]
        public string SCwlhtxsdcl
        {
            get { return GetString("SC网络后台线索达成率", ""); }
            set { SetExtendedAttribute("SC网络后台线索达成率", value); }
        }

        /// <summary>
        /// 网络线索建档率
        /// </summary>
        [JsonIgnore]
        public string SCwlxsjdl
        {
            get { return GetString("SC网络线索建档率", ""); }
            set { SetExtendedAttribute("SC网络线索建档率", value); }
        }

        /// <summary>
        /// 呼入电话达成率
        /// </summary>
        [JsonIgnore]
        public string SChrdhdcl
        {
            get { return GetString("SC呼入电话达成率", ""); }
            set { SetExtendedAttribute("SC呼入电话达成率", value); }
        }

        /// <summary>
        /// 呼入电话建档率
        /// </summary>
        [JsonIgnore]
        public string SChrdhjdl
        {
            get { return GetString("SC呼入电话建档率", ""); }
            set { SetExtendedAttribute("SC呼入电话建档率", value); }
        }

        /// <summary>
        /// 触点潜客达成率
        /// </summary>
        [JsonIgnore]
        public string SCcdqkdcl
        {
            get { return GetString("SC触点潜客达成率", ""); }
            set { SetExtendedAttribute("SC触点潜客达成率", value); }
        }

        /// <summary>
        /// 活动集客达成率
        /// </summary>
        [JsonIgnore]
        public string SChdjkdcl
        {
            get { return GetString("SC活动集客达成率", ""); }
            set { SetExtendedAttribute("SC活动集客达成率", value); }
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
        
        /// <summary>
        /// 展厅占比
        /// </summary>
        [JsonIgnore]
        public string DCCztzb
        {
            get { return GetString("DCC展厅占比", ""); }
            set { SetExtendedAttribute("DCC展厅占比", value); }
        }

        /// <summary>
        /// 首电建档率
        /// </summary>
        [JsonIgnore]
        public string DCCsdjdl
        {
            get { return GetString("DCC首电建档率", ""); }
            set { SetExtendedAttribute("DCC首电建档率", value); }
        }

        /// <summary>
        /// 网络后台建档率
        /// </summary>
        [JsonIgnore]
        public string DCCwlhtjdl
        {
            get { return GetString("DCC网络后台建档率", ""); }
            set { SetExtendedAttribute("DCC网络后台建档率", value); }
        }

        /// <summary>
        /// 网络呼入建档率
        /// </summary>
        [JsonIgnore]
        public string DCCwlhrjdl
        {
            get { return GetString("DCC网络呼入建档率", ""); }
            set { SetExtendedAttribute("DCC网络呼入建档率", value); }
        }

        /// <summary>
        /// 有效呼出率
        /// </summary>
        [JsonIgnore]
        public string DCCyxhcl
        {
            get { return GetString("DCC有效呼出率", ""); }
            set { SetExtendedAttribute("DCC有效呼出率", value); }
        }

        /// <summary>
        /// 呼入呼出邀约到店率
        /// </summary>
        [JsonIgnore]
        public string DCChrhcyyddl
        {
            get { return GetString("DCC呼入呼出邀约到店率", ""); }
            set { SetExtendedAttribute("DCC呼入呼出邀约到店率", value); }
        }

        /// <summary>
        /// 再次邀约率
        /// </summary>
        [JsonIgnore]
        public string DCCzcyyl
        {
            get { return GetString("DCC再次邀约率", ""); }
            set { SetExtendedAttribute("DCC再次邀约率", value); }
        }

        /// <summary>
        /// 成交率
        /// </summary>
        [JsonIgnore]
        public string DCCcjl
        {
            get { return GetString("DCC成交率", ""); }
            set { SetExtendedAttribute("DCC成交率", value); }
        }

        #endregion

        #endregion

        #region 二手车部月度目标

        #region 关键指标
        
        /// <summary>
        /// 销售有效推荐率
        /// </summary>
        [JsonIgnore]
        public string ESCxsyxtjl
        {
            get { return GetString("ESC销售有效推荐率", ""); }
            set { SetExtendedAttribute("ESC销售有效推荐率", value); }
        }

        /// <summary>
        /// 售后有效推荐率
        /// </summary>
        [JsonIgnore]
        public string ESCshyxtjl
        {
            get { return GetString("ESC售后有效推荐率", ""); }
            set { SetExtendedAttribute("ESC售后有效推荐率", value); }
        }

        /// <summary>
        /// 评估成交率
        /// </summary>
        [JsonIgnore]
        public string ESCpgcjl
        {
            get { return GetString("ESC评估成交率", ""); }
            set { SetExtendedAttribute("ESC评估成交率", value); }
        }

        /// <summary>
        /// 销售成交率
        /// </summary>
        [JsonIgnore]
        public string ESCxscjl
        {
            get { return GetString("ESC销售成交率", ""); }
            set { SetExtendedAttribute("ESC销售成交率", value); }
        }

        /// <summary>
        /// 平均单台毛利
        /// </summary>
        [JsonIgnore]
        public string ESCpjdtml
        {
            get { return GetString("ESC平均单台毛利", ""); }
            set { SetExtendedAttribute("ESC平均单台毛利", value); }
        }

        /// <summary>
        /// 总置换率
        /// </summary>
        [JsonIgnore]
        public string ESCzzhl
        {
            get { return GetString("ESC总置换率", ""); }
            set { SetExtendedAttribute("ESC总置换率", value); }
        }

        /// <summary>
        /// 总有效评估量
        /// </summary>
        [JsonIgnore]
        public string ESCzyxpgl
        {
            get { return GetString("ESC总有效评估量", ""); }
            set { SetExtendedAttribute("ESC总有效评估量", value); }
        }

        /// <summary>
        /// 总收购量
        /// </summary>
        [JsonIgnore]
        public string ESCzsgl
        {
            get { return GetString("ESC总收购量", ""); }
            set { SetExtendedAttribute("ESC总收购量", value); }
        }

        /// <summary>
        /// 总销售量
        /// </summary>
        [JsonIgnore]
        public string ESCzxsl
        {
            get { return GetString("ESC总销售量", ""); }
            set { SetExtendedAttribute("ESC总销售量", value); }
        }

        /// <summary>
        /// 总毛利
        /// </summary>
        [JsonIgnore]
        public string ESCzml
        {
            get { return GetString("ESC总毛利", ""); }
            set { SetExtendedAttribute("ESC总毛利", value); }
        }

        #endregion

        #endregion

    }
}
