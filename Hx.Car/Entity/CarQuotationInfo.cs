using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Enum;

namespace Hx.Car.Entity
{
    /// <summary>
    /// 车辆报价类
    /// </summary>
    [Serializable]
    public class CarQuotationInfo
    {
        public int ID { get; set; }

        public int UCode { get; set; }

        public string UCodeName
        {
            get
            {
                return string.Format("{0}({1})", UCode.ToString(), cCxmc);
            }
        }

        /// <summary>
        /// 报价人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 报价时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CorporationID { get; set; }

        /// <summary>
        /// 客户手机
        /// </summary>
        public string CustomerMobile { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户QQ
        /// </summary>
        public string CustomerQQ { get; set; }

        /// <summary>
        /// 客户邮箱
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// 客户微信
        /// </summary>
        public string CustomerMicroletter { get; set; }

        /// <summary>
        /// 报价类型
        /// </summary>
        public CarQuotationType CarQuotationType { get; set; }

        /// <summary>
        ///  总价
        /// </summary>
        public string TotalPrinces { get; set; }

        #region 基本信息

        /// <summary>
        /// 汽车厂商
        /// </summary>
        public string cChangs { get; set; }

        /// <summary>
        /// 车型名称
        /// </summary>
        public string cCxmc { get; set; }

        /// <summary>
        /// 汽车颜色
        /// </summary>
        public string cQcys { get; set; }

        /// <summary>
        /// 成交价
        /// </summary>
        public string fCjj { get; set; }

        #endregion

        #region 必要花费
        
        /// <summary>
        /// 购置税
        /// </summary>
        public string cGzs { get; set; }

        /// <summary>
        /// 上牌费
        /// </summary>
        public string cSpf { get; set; }

        /// <summary>
        /// 车船税
        /// </summary>
        public string cCcs { get; set; }

        /// <summary>
        /// 交强险
        /// </summary>
        public string cJqs { get; set; }

        #endregion

        #region 商业保险

        /// <summary>
        /// 保险公司
        /// </summary>
        public string Bxgs { get; set; }

        /// <summary>
        /// 折扣系数
        /// </summary>
        public string Zkxs { get; set; }

        /// <summary>
        /// 车损险
        /// </summary>
        public string cCsx { get; set; }

        /// <summary>
        /// 三者投保金额
        /// </summary>
        public string Sztb { get; set; }

        /// <summary>
        /// 第三者责任险
        /// </summary>
        public string cDszrx { get; set; }

        /// <summary>
        /// 盗抢险
        /// </summary>
        public string cDqx { get; set; }

        /// <summary>
        /// 是否投保人员险（司机）
        /// </summary>
        public bool IscSj { get; set; }

        /// <summary>
        /// 人员险（司机）
        /// </summary>
        public string cSj { get; set; }

        /// <summary>
        /// 人员险（司机） 投保金额
        /// </summary>
        public string cSjtb { get; set; }

        /// <summary>
        /// 是否投保人员险（乘客）
        /// </summary>
        public bool IscCk { get; set; }

        /// <summary>
        /// 人员险（乘客）
        /// </summary>
        public string cCk { get; set; }

        /// <summary>
        /// 人员险（乘客） 投保金额
        /// </summary>
        public string cCktb { get; set; }

        /// <summary>
        /// 是否投保附加险：自燃险
        /// </summary>
        public bool IscZrx { get; set; }

        /// <summary>
        /// 自燃险
        /// </summary>
        public string cZrx { get; set; }

        /// <summary>
        /// 玻璃产地
        /// </summary>
        public string Blcd { get; set; }

        /// <summary>
        /// 是否投保附加险：玻璃险
        /// </summary>
        public bool IscBlx { get; set; }

        /// <summary>
        /// 玻璃险
        /// </summary>
        public string cBlx { get; set; }

        /// <summary>
        /// 是否投保附加险：划痕险
        /// </summary>
        public bool IscHhx { get; set; }

        /// <summary>
        /// 划痕险
        /// </summary>
        public string cHhx { get; set; }

        /// <summary>
        /// 是否投保附加险：涉水险
        /// </summary>
        public bool IscSsx { get; set; }

        /// <summary>
        /// 涉水险
        /// </summary>
        public string cSsx { get; set; }

        /// <summary>
        /// 不计免赔投保
        /// </summary>
        public string cBjmptb { get; set; }

        /// <summary>
        /// 不计免赔
        /// </summary>
        public string cBjmp { get; set; }

        /// <summary>
        /// 是否投保指定维修
        /// </summary>
        public bool IscZdwx { get; set; }

        /// <summary>
        /// 指定维修
        /// </summary>
        public string cZdwx { get; set; }

        /// <summary>
        ///  续保押金
        /// </summary>
        public string cXbyj { get; set; }

        #endregion

        #region 金融方案

        public BankingType BankingType { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 贷款方案
        /// </summary>
        public LoanType LoanType { get; set; }

        /// <summary>
        /// 首付款
        /// </summary>
        public string FirstPayment { get; set; }

        /// <summary>
        /// 贷款额
        /// </summary>
        public string LoanValue { get; set; }

        /// <summary>
        /// 贷款期（年）
        /// </summary>
        public string LoanLength { get; set; }

        /// <summary>
        /// 月还款额
        /// </summary>
        public string RepaymentPerMonth { get; set; }

        /// <summary>
        /// 尾款
        /// </summary>
        public string RemainingFund { get; set; }

        /// <summary>
        /// 利率
        /// </summary>
        public string ProfitMargin { get; set; }

        /// <summary>
        /// 其他费用
        /// </summary>
        public string OtherCost { get; set; }

        /// <summary>
        /// 账户管理费
        /// </summary>
        public string AccountManagementCost { get; set; }

        #endregion

        /// <summary>
        /// 精品
        /// </summary>
        public string ChoicestGoods { get; set; }

        /// <summary>
        /// 精品价格
        /// </summary>
        public string ChoicestGoodsPrice { get; set; }

        /// <summary>
        /// 礼品
        /// </summary>
        public string Gift { get; set; }

        #region 其他

        /// <summary>
        /// 是否置换二手车
        /// </summary>
        public bool IsSwap { get; set; }

        /// <summary>
        /// 二手车描述
        /// </summary>
        public string SwapDetail { get; set; }

        /// <summary>
        /// 履约风险金
        /// </summary>
        public string Lyfxj { get; set; }

        /// <summary>
        /// 代办上牌劳务费
        /// </summary>
        public string Dbsplwf { get; set; }

        /// <summary>
        /// 代办分期付款手续费、劳务费
        /// </summary>
        public string Dbfqlwf { get; set; }

        /// <summary>
        /// 资信费
        /// </summary>
        public string Zxf { get; set; }

        /// <summary>
        /// 调查费
        /// </summary>
        public string Dcf { get; set; }

        /// <summary>
        /// 首付金额合计
        /// </summary>
        public string TotalFirstPrinces { get; set; }

        #endregion
    }
}
