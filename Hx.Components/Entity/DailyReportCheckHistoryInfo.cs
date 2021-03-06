﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;

namespace Hx.Components.Entity
{
    public class DailyReportCheckHistoryInfo
    {
        public int ID { get; set; }

        public string DayUnique { get; set; }

        public DailyReportInfo CheckedInfo { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public int CreatorCorporationID { get; set; }

        public string CreatorCorporationName { get; set; }

        public DayReportDep CreatorDepartment { get; set; }

        public DayReportDep ReportDepartment { get; set; }

        public int ReportCorporationID { get; set; }

        public string Detail
        {
            get
            {
                string result = string.Empty;

                if (CheckedInfo != null)
                {
                    return CheckedInfo.SCReport;
                }

                return result;
            }
        }
    }
}
