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
    public class CRMReportQuery : IQuery
    {
        private string _column = "*";
        private string _tableName = "HX_CRMReport";
        private string _orderby = " [Date] ASC,[ID] ASC";

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

        public string MonthStr { get; set; }

        public CRMReportType? CRMReportType { get; set; }

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
            if (!string.IsNullOrEmpty(MonthStr))
            {
                query.Add(string.Format("[MonthStr] = '{0}'", MonthStr));
            }
            if (CRMReportType.HasValue)
            {
                query.Add(string.Format("[CRMReportType] = {0}", (int)CRMReportType.Value));
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
