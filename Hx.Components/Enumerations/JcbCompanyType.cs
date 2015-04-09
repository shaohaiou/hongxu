using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hx.Components.Enumerations
{
    public enum JcbCompanyType
    {
        [Description("4s店")]
        fs店,
        [Description("经济公司")]
        经济公司,
        [Description("经纪人")]
        经纪人,
        [Description("独立品牌")]
        独立品牌,
        [Description("其他")]
        其他
    }
}
