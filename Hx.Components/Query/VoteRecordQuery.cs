using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;

namespace Hx.Components.Query
{
    public class VoteRecordQuery : IQuery
    {
        private string _column = "*";
        private string _tableName = "dbo.HX_VoteRecord";
        private string _orderby = " ID DESC";

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

        public int? SID { get; set; }

        public string AthleteName { set; get; }

        public int? SerialNumber { get; set; }

        /// <summary>
        /// 生成where
        /// </summary>
        /// <returns></returns>
        public string BulidQuery()
        {
            List<string> query = new List<string>();

            if (SID.HasValue)
            {
                query.Add(string.Format("[SID] = {0}", SID.Value));
            }
            if (SerialNumber.HasValue)
            {
                query.Add(string.Format("[SerialNumber] = {0}", SerialNumber.Value));
            }
            if (!string.IsNullOrEmpty(AthleteName))
            {
                query.Add(string.Format("[AthleteName] LIKE '%{0}%'", AthleteName));
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
