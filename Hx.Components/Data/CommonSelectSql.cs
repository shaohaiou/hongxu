using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Hx.Components.Interface;

namespace Hx.Components.Data
{
    public class CommonSelectSql
    {
        #region DataReader
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="con">数据库连接字符串</param>
        /// <param name="top">前几条</param>
        /// <param name="Query">查询构建类</param>
        /// <returns>DataReader数据表</returns>
        public static IDataReader SelectGetReader(string con, int top, IQuery Query)
        {
            return GetDataReader(con, top, Query.TableName, Query.Column, Query.BulidQuery(), Query.OrderBy);
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        /// <param name="connstring">数据库连接字符串</param>
        /// <param name="Top">几条记录</param>
        /// <param name="tableName">表名</param>
        /// <param name="Column">列名</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序方式</param>
        /// <returns>DataReader数据表</returns>
        private static IDataReader GetDataReader(string connstring, int Top, string tableName, string Column, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("SELECT * FROM(", filedOrder);
            if (!string.IsNullOrEmpty(Column))
            {
                strSql.Append(Column);
            }
            else
            {
                strSql.Append("*");
            }
            strSql.Append(" FROM " + tableName);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere + ") T");
            }
            else
            {
                strSql.Append(") T");
            }
            if (Top > 0)
            {
                strSql.AppendFormat(" WHERE T.ROWNUM <= {0}", Top);
            }
            return SqlHelper.ExecuteReader(connstring, CommandType.Text, strSql.ToString());
        }
        #endregion

        #region DataTable

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="con">数据库连接字符串</param>
        /// <param name="top">前几条</param>
        /// <param name="Query">查询构建类</param>
        /// <returns>DataTable数据表</returns>
        public static DataTable SelectGetTable(string con, int top, IQuery Query)
        {
            return GetDataTable(con, top, Query.TableName, Query.Column, Query.BulidQuery(), Query.OrderBy);
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        /// <param name="con">数据库连接字符串</param>
        /// <param name="Top">几条记录</param>
        /// <param name="tableName">表名</param>
        /// <param name="Column">列名</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序方式</param>
        /// <returns>DataTable数据表</returns>
        private static DataTable GetDataTable(string con, int Top, string tableName, string Column, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(Column);
            strSql.Append(" FROM " + tableName);
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return SqlHelper.ExecuteDataset(con, CommandType.Text, strSql.ToString()).Tables[0];
        }
        #endregion
    }
}
