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
        /// 销售日期
        /// </summary>
        public DateTime? SaleDay { get; set; }

        /// <summary>
        /// 订车日期
        /// </summary>
        public DateTime? PlaceDay { get; set; }

        /// <summary>
        /// 客户微信
        /// </summary>
        public string CustomerMicroletter { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        public string cCjh { get; set; }

        /// <summary>
        /// 是否老客户转介绍
        /// </summary>
        public int Islkhzjs { get; set; }

        /// <summary>
        /// 报价类型
        /// </summary>
        public CarQuotationType CarQuotationType { get; set; }

        /// <summary>
        ///  总价
        /// </summary>
        public string TotalPrinces { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int CheckStatus { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckUser { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public string CheckTime { get; set; }

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

        public string cQcysName 
        {
            get
            {
                string result = string.Empty;

                if (!string.IsNullOrEmpty(cQcys) && cQcys.IndexOf(",") > 0)
                    return cQcys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];

                return result;
            }
        }

        /// <summary>
        /// 汽车内饰颜色
        /// </summary>
        public string cNsys { get; set; }

        /// <summary>
        /// 指导价
        /// </summary>
        public string fZdj { get; set; }

        /// <summary>
        /// 成交价
        /// </summary>
        public string fCjj { get; set; }

        #endregion

        #region 现金部分
        
        /// <summary>
        /// 预收附加税
        /// </summary>
        public string cGzs { get; set; }

        /// <summary>
        /// 上牌及手续费
        /// </summary>
        public string cSpf { get; set; }

        /// <summary>
        /// 汽车用品加装费用
        /// </summary>
        public string ChoicestGoodsPrice { get; set; }

        /// <summary>
        /// 代收保险费合计
        /// </summary>
        public string Bxhj { get; set; }

        /// <summary>
        /// 无忧服务费用
        /// </summary>
        public string Wyfw { get; set; }

        /// <summary>
        /// 增值费
        /// </summary>
        public string Dbsplwf { get; set; }

        /// <summary>
        /// 其他费用描述
        /// </summary>
        public string  Qtfyms { get; set; }

        /// <summary>
        /// 其他费用
        /// </summary>
        public string Qtfy { get; set; }

        /// <summary>
        /// 车船税
        /// </summary>
        public string cCcs { get; set; }

        #endregion

        #region 无忧服务

        /// <summary>
        /// 是否选择了机油套餐
        /// </summary>
        public bool IsWyfwjytc { get; set; }

        /// <summary>
        /// 无忧服务机油套餐
        /// </summary>
        public string Wyfwjytc { get; set; }

        /// <summary>
        /// 是否选择了玻璃无忧
        /// </summary>
        public bool IsWyfwblwyfw { get; set; }

        /// <summary>
        /// 无忧服务玻璃无忧
        /// </summary>
        public string Wyfwblwyfw { get; set; }

        /// <summary>
        /// 是否选择了划痕无忧
        /// </summary>
        public bool IsWyfwhhwyfw { get; set; }

        /// <summary>
        /// 无忧服务划痕无忧
        /// </summary>
        public string Wyfwhhwyfw { get; set; }

        /// <summary>
        /// 是否选择了延保无忧
        /// </summary>
        public bool IsWyfwybwyfw { get; set; }

        /// <summary>
        /// 无忧服务延保无忧
        /// </summary>
        public string Wyfwybwyfw { get; set; }

        #endregion

        #region 代收保险

        /// <summary>
        /// 保险公司
        /// </summary>
        public string Bxgs { get; set; }

        /// <summary>
        /// 是否选择了交强险
        /// </summary>
        public bool IscJqs { get; set; }

        /// <summary>
        /// 交强险
        /// </summary>
        public string cJqs { get; set; }

        /// <summary>
        /// 折扣系数
        /// </summary>
        public string Zkxs { get; set; }

        /// <summary>
        /// 是否选择了车损险
        /// </summary>
        public bool IscCsx { get; set; }

        /// <summary>
        /// 车损险
        /// </summary>
        public string cCsx { get; set; }

        /// <summary>
        /// 三者投保金额
        /// </summary>
        public string Sztb { get; set; }

        /// <summary>
        /// 是否选择了第三者责任险
        /// </summary>
        public bool IscDszrx { get; set; }

        /// <summary>
        /// 第三者责任险
        /// </summary>
        public string cDszrx { get; set; }

        /// <summary>
        /// 是否选择了盗抢险
        /// </summary>
        public bool IscDqx { get; set; }

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
        ///  押金
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

        #region 汽车用品

        /// <summary>
        /// 精品
        /// </summary>
        public string ChoicestGoods { get; set; }

        /// <summary>
        /// 礼品
        /// </summary>
        public string Gift { get; set; }

        #endregion

        #region 其他

        /// <summary>
        /// 是否置换二手车
        /// </summary>
        public bool IsSwap { get; set; }

        /// <summary>
        /// 是否大客户
        /// </summary>
        public bool IsDkh { get; set; }

        /// <summary>
        /// 是否忠诚客户
        /// </summary>
        public bool IsZcyh { get; set; }

        /// <summary>
        /// 二手车描述
        /// </summary>
        public string SwapDetail { get; set; }

        /// <summary>
        /// 代收风险金
        /// </summary>
        public string Lyfxj { get; set; }

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
