using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components.Enumerations;

namespace Hx.Components.Query
{
    public class MonthTargetQuery : IQuery
    {
        private string _column = "*";
        private string _tableName = "dbo.HX_MonthlyTarget";
        private string _orderby = " ID desc";

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

        public DayReportDep? DayReportDep { get; set; }

        public int? CorporationID { get; set; }

        public string MonthUnique { get; set; }

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
            if (DayReportDep.HasValue)
            {
                query.Add(string.Format("[Department] = {0}", (int)DayReportDep.Value));
            }
            if (!string.IsNullOrEmpty(MonthUnique))
            {
                query.Add(string.Format("CHARINDEX('{0}',[MonthUnique]) = 1", MonthUnique));
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
