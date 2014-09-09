using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Hx.Components.Enumerations
{
    public enum ApplicationType
    {
        [Description("其他")]
        NoSet=0,
        [Description("网站")]
        WebSite=1,
        [Description("工具")]
        Tools
    }
}
