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

        #endregion

        #endregion

        #region 售后部月度目标


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

    }
}
