using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Car.Enum;

namespace Hx.Car.Query
{
    public class CarQuotationQuery : IQuery
    {
        private string _column = " *";
        private string _tableName = "HX_CarQuotation";
        private string _orderby = " ID asc";

        #region IQuery 成员

        /// <summary>
        /// 需要返回的字段
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
        /// 查询的表
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
        /// 排序
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

        public CarQuotationType? CarQuotationType { get; set; }

        public string CorporationID { get; set; }

        public string Creator { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public string CustomerName { get; set; }

        /// <summary>
        /// 生成sql
        /// </summary>
        /// <returns></returns>
        public string BulidQuery()
        {
            List<string> query = new List<string>();

            if (!string.IsNullOrEmpty(CorporationID))
            {
                query.Add(string.Format("[CorporationID] = '{0}'", CorporationID));
            }
            if (CarQuotationType.HasValue)
            {
                query.Add(string.Format("CarQuotationType = {0}", (int)CarQuotationType.Value));
            }
            if (!string.IsNullOrEmpty(Creator))
            {
                query.Add(string.Format("CHARINDEX('{0}',[Creator]) > 0", Creator));
            }
            if (DateBegin.HasValue)
            {
                query.Add(string.Format("[CreateTime] > '{0}'", DateBegin.Value));
            }
            if (DateEnd.HasValue)
            {
                query.Add(string.Format("[CreateTime] < '{0}'", DateEnd.Value));
            }
            if (!string.IsNullOrEmpty(CustomerName))
            {
                query.Add(string.Format("CHARINDEX('{0}',[CustomerName]) > 0", CustomerName));
            }

            return string.Join(" AND ", query);
        }

        #endregion

        public string BulidSelect(string where, string tableName = "")
        {
            throw new NotImplementedException();
        }
    }
}
