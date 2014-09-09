using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Hx.Components.Interface;

namespace Hx.Components.Data
{
    public class CommonPageSql
    {
        /// <summary>
        /// 分页获取数据列表 适用于SQL2000、SQL2005和SQL2008
        /// </summary>
        /// <param name="con">数据库连接字符串</param>
        /// <param name="pageindex">页索引 从0开始</param>
        /// <param name="pagesize">每页记录数</param>
        /// <param name="query">查询接口</param>
        /// <param name="p">输出参数</param>
        /// <returns>DataRead数据集</returns>
        public static IDataReader GetDataReaderByPager(string con, int pageindex, int pagesize, IQuery query, out SqlParameter p)
        {
            string cmd = "RecordFromPage";
            SqlParameter[] para = SqlHelperParameterCache.GetSpParameterSet(con, cmd);
            para[0].Value = query.Column;
            para[1].Value = query.TableName;
            para[2].Value = query.BulidQuery();
            para[3].Value = query.OrderBy;
            para[4].Value = "ID";
            para[5].Value = pageindex;
            para[6].Value = pagesize;
            para[7].Direction = ParameterDirection.Output;
            IDataReader reader = SqlHelper.ExecuteReader(con, CommandType.StoredProcedure, cmd, para);
            p = para[7];
            return reader;
        }

        /// <summary>
        /// 分页获取数据列表 适用于SQL2000、SQL2005和SQL2008
        /// </summary>
        /// <param name="con">数据库连接字符串</param>
        /// <param name="pageindex">页索引 从0开始</param>
        /// <param name="pagesize">每页记录数</param>
        /// <param name="query">查询接口</param>
        /// <param name="p">输出参数</param>
        /// <returns>DataTable数据集</returns>
        public static DataTable GetDataByPager(string con, int pageindex, int pagesize, IQuery query, out SqlParameter p)
        {
            string cmd = "RecordFromPage";
            SqlParameter[] para = SqlHelperParameterCache.GetSpParameterSet(con, cmd);
            para[0].Value = query.Column;
            para[1].Value = query.TableName;
            para[2].Value = query.BulidQuery();
            para[3].Value = query.OrderBy;
            para[4].Value = "ID";
            para[5].Value = pageindex;
            para[6].Value = pagesize;
            para[7].Direction = ParameterDirection.Output;
            DataTable datatable = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, cmd, para).Tables[0];
            p = para[7];
            return datatable;
        }
    }
}
