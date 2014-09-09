using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components.Enumerations;
using Hx.Tools;
using System.Data;

namespace Hx.Components.Query
{
    public class DailyReportQuery : IQuery
    {
        private string _column = "*";
        private string _tableName = "HX_DailyReportXSB";
        private string _orderby = " [DayUnique] ASC";

        #region IQuery 成员

        /// <summary>
        /// 需要返回的列
        /// </summary>
        public string Column
        {
            get
            {
                return _column;
            }
            set
            {
                _column = value;
            }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy
        {
            get
            {
                return _orderby;
            }
            set
            {
                _orderby = value;
            }
        }
        private DayReportDep dayreportdep = DayReportDep.售后部;
        public DayReportDep DayReportDep
        {
            set
            {
                TableName = EnumExtensions.GetDescription<DayReportDep>(value.ToString());
                dayreportdep = value;
            }
            get
            {
                return dayreportdep;
            }
        }

        public int? CorporationID { get; set; }

        public string DayUnique { get; set; }

        public string DayUniqueStart { get; set; }

        public string DayUniqueEnd { get; set; }

        /// <summary>
        /// 生成where
        /// </summary>
        /// <returns></returns>
        public string BulidQuery()
        {
            List<string> query = new List<string>();

            if (CorporationID.HasValue)
            {
                query.Add(string.Format("[CorporationID] = {0}", CorporationID.Value));
            }
            if (!string.IsNullOrEmpty(DayUnique))
            {
                query.Add(string.Format("CHARINDEX('{0}',[DayUnique]) = 1", DayUnique));
            }
            if (!string.IsNullOrEmpty(DayUniqueStart))
            {
                query.Add(string.Format("[DayUnique] >= '{0}'", DayUniqueStart));
            }
            if (!string.IsNullOrEmpty(DayUniqueEnd))
            {
                query.Add(string.Format("[DayUnique] <= '{0}'", DayUniqueEnd));
            }

            return string.Join(" AND ", query);

        }

        /// <summary>
        /// 生成sql
        /// </summary>
        /// <returns></returns>
        public string BulidSelect(string where, string tableName = "")
        {
            return string.Empty;
        }
        #endregion
    }
}
