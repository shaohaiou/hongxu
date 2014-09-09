using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Tools;
using Newtonsoft.Json;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    /// <summary>
    /// 日报模块
    /// </summary>
    public class DailyReportModuleInfo
    {
        public int ID { get; set; }

        public int Sort { get; set; }

        public string Name { get; set; }

        public DayReportDep Department { get; set; }

        public bool Ismonthlytarget { get; set; }

        public string Description { get; set; }

        public bool Mustinput { get; set; }

        /// <summary>
        /// 是否合计
        /// </summary>
        public bool Iscount { get; set; }
    }
}
