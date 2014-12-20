using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hx.Components.Enumerations
{
    public enum CRMReportType
    {
        [Description("展厅来店(电)客流量登记表")]
        客流量登记表,
        [Description("活动或外出访问客户信息")]
        活动外出访客信息,
        [Description("战胜(订单)记录表")]
        战胜记录表,
        [Description("战败(失控)记录表")]
        战败记录表,
        [Description("月末销售顾问别的A卡留存数据")]
        月末其他A卡留存数据
    }
}
