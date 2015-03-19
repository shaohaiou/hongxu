using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Enumerations;
using Hx.Components.Interface;

namespace Hx.Components.Query
{
    public class DailyReportCheckHistoryQuery : IQuery
    {
        private string _column = "*";
        private string _tableName = "dbo.HX_DayReportCheckHistory";
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

        public int? CorporationID { get; set; }

        public int? ReportCorprationID { get; set; }

        public DayReportDep? Department { set; get; }

        public DayReportDep? ReportDepartment { set; get; }

        public string Operator { set; get; }

        public string DayUnique { get; set; }

        /// <summary>
        /// 生成where
        /// </summary>
        /// <returns></returns>
        public string BulidQuery()
        {
            List<string> query = new List<string>();

            if (ReportDepartment.HasValue)
            {
                query.Add(string.Format("[ReportDepartment] = {0}", (int)ReportDepartment.Value));
            }
            if (ReportCorprationID.HasValue)
            {
                query.Add(string.Format("[ReportCorporationID] = {0}", ReportCorprationID.Value));
            }
            if (CorporationID.HasValue)
            {
                query.Add(string.Format("[CreatorCorporationID] = {0}", CorporationID.Value));
            }
            if (Department.HasValue)
            {
                query.Add(string.Format("[CreatorDepartment] = {0}", (int)Department.Value));
            }
            if (!string.IsNullOrEmpty(Operator))
            {
                query.Add(string.Format("[Creator] = '{0}'", Operator));
            }
            if (!string.IsNullOrEmpty(DayUnique))
            {
                query.Add(string.Format("[DayUnique] = '{0}'", DayUnique));
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
