using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Hx.Components.Entity
{
    [DataContract]
    public class DailyReportCollectInfo
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [DataMember(Name="GS")]
        public string Corpname { get; set; }

        /// <summary>
        /// 系统实际评估台次
        /// </summary>
        [DataMember(Name = "SJPG")]
        public string xtsjpgtc { get; set; }

        /// <summary>
        /// 销售推荐评估台次
        /// </summary>
        [DataMember(Name = "XSTJPG")]
        public string xstjpgtc { get; set; }

        /// <summary>
        /// 售后推荐评估台次
        /// </summary>
        [DataMember(Name = "SHTJPG")]
        public string shtjpgtc { get; set; }

        /// <summary>
        /// 其他渠道推荐评估台次
        /// </summary>
        [DataMember(Name = "QTTJPG")]
        public string qtqdtjpgtc { get; set; }

        /// <summary>
        /// 潜客回访数
        /// </summary>
        [DataMember(Name = "QKHF")]
        public string qkhfs { get; set; }

        /// <summary>
        /// 新增卖车意向客户数
        /// </summary>
        [DataMember(Name = "MCKH")]
        public string xzmcyxkhs { get; set; }

        /// <summary>
        /// 首次来电批次
        /// </summary>
        [DataMember(Name = "DHPC")]
        public string scldpc { get; set; }

        /// <summary>
        /// 首次来店批次
        /// </summary>
        [DataMember(Name = "DDPC")]
        public string scldianpc { get; set; }

        /// <summary>
        /// 新增有效留档数
        /// </summary>
        [DataMember(Name = "YXKH")]
        public string xzyxlds { get; set; }

        /// <summary>
        /// 销售回访数
        /// </summary>
        [DataMember(Name = "XSHF")]
        public string xshfs { get; set; }

        /// <summary>
        /// 新增买车意向客户数
        /// </summary>
        [DataMember(Name = "MCYXKH")]
        public string xzmaicyxkhs { get; set; }

        /// <summary>
        /// 纯收购台次
        /// </summary>
        [DataMember(Name = "SGCL")]
        public string csgtc { get; set; }

        /// <summary>
        /// 置换车台次
        /// </summary>
        [DataMember(Name = "ZHCL")]
        public string zhctc { get; set; }

        /// <summary>
        /// 销售车台次
        /// </summary>
        [DataMember(Name = "XSCL")]
        public string xsctc { get; set; }

        /// <summary>
        /// 销售毛利
        /// </summary>
        [DataMember(Name = "XSML")]
        public string xsml { get; set; }

        /// <summary>
        /// 库存数
        /// </summary>
        [DataMember(Name = "KCCL")]
        public string kcs { get; set; }

        /// <summary>
        /// 寄售车台次
        /// </summary>
        [DataMember(Name = "JSCL")]
        public string jsctc { get; set; }

        /// <summary>
        /// 在库超30天车辆
        /// </summary>
        [DataMember(Name="Days")]
        public string zkc30tcl { get; set; }
    }
}
