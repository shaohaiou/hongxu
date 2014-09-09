using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;

namespace Hx.Car.Query
{
    public class CarQuery : IQuery
    {
        private string _column = " *";
        private string _tableName = "t_QcCs";
        private string _orderby = " ID asc";

        /// <summary>
        /// 厂商
        /// </summary>
        public string cChangs { get; set; }


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

        /// <summary>
        /// 生成sql
        /// </summary>
        /// <returns></returns>
        public string BulidQuery()
        {
            List<string> query = new List<string>();
            if (!string.IsNullOrEmpty(cChangs))
            {
                query.Add(string.Format("[cChangs] = '{0}'", cChangs));
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
