using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;

namespace Hx.Components.Query
{
    public class WeixinActQuery : IQuery
    {
        private string _column = "*";
        private string _tableName = "dbo.HX_WeixinAct";
        private string _orderby = " AtcValue desc";

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

        public int? Sex { get; set; }

        public string Nickname { get; set; }

        /// <summary>
        /// 生成where
        /// </summary>
        /// <returns></returns>
        public string BulidQuery()
        {
            List<string> query = new List<string>();

            if (Sex.HasValue)
            {
                query.Add(string.Format("[Sex] = {0}", Sex.Value));
            }
            if (!string.IsNullOrEmpty(Nickname))
            {
                query.Add(string.Format("[Nickname] LIKE '%{0}%'", Nickname));
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
