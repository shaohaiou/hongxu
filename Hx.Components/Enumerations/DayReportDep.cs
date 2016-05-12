using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hx.Components.Enumerations
{
    public enum DayReportDep
    {
        [Description("HX_DailyReportXSB")]
        销售部,
        [Description("HX_DailyReportSHB")]
        售后部,
        [Description("HX_DailyReportSCB")]
        市场部,
        [Description("HX_DailyReportCWB")]
        财务部,
        [Description("HX_DailyReportXZB")]
        行政部,
        [Description("HX_DailyReportJPB")]
        精品部,
        [Description("HX_DailyReportKFB")]
        客服部,
        [Description("HX_DailyReportESCB")]
        二手车部,
        [Description("HX_DailyReportJRB")]
        金融部,
        [Description("HX_DailyReportDCCB")]
        DCC部,
        [Description("HX_DailyReportNXCP")]
        粘性产品
    }
}
