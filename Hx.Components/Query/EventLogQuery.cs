using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;

namespace Hx.Components.Query
{
    public class EventLogQuery : IQuery
    {
        private string _column = "*";
        private string _tableName = "HX_EventLog";
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
        public string ApplicationType { get; set; }
        public int EventType { get; set; }
        public int EventID { get; set; }
        public int ApplicationID { get; set; }
        public int EntryID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 生成where
        /// </summary>
        /// <returns></returns>
        public string BulidQuery()
        {
            List<string> query = new List<string>();
            if (!string.IsNullOrEmpty(ApplicationType))
            {
                query.Add(string.Format("AppType='{0}'", ApplicationType));
            }
            if (EventType >= 0)
            {
                query.Add(string.Format("EventType='{0}'", EventType));
            }
            if (EventID > 0)
            {
                query.Add(string.Format("EventID='{0}'", EventID));
            }
            if (ApplicationID > 0)
            {
                query.Add(string.Format("ApplicationID='{0}'", ApplicationID));
            }
            if (EntryID > 0)
            {
                query.Add(string.Format("EntryID='{0}'", EntryID));
            }
            if (StartTime.HasValue)
            {
                query.Add(string.Format("AddTime > '{0}'", StartTime));
            }
            if (EndTime.HasValue)
            {
                query.Add(string.Format("AddTime < '{0}' ", EndTime));
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
