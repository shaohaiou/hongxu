using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Hx.Components.Data
{
    public class SqlHelperParameterCache
    {
        #region 私有方法，变量，跟构造函数

        private SqlHelperParameterCache() { }

        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 返回存储过程中的参数信息
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否返回需要返回的参数@RETURN_VALUE</param>
        /// <returns>返回的参数数组.</returns>
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程名");

            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

            cmd.Parameters.CopyTo(discoveredParameters, 0);

            foreach (SqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>
        /// 复制一份参数数组
        /// </summary>
        /// <param name="originalParameters">参数数组</param>
        /// <returns>参数数组</returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion

        #region 缓存方法

        /// <summary>
        /// 将参数数组添加到缓存
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            string hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        /// 从缓存里获取参数数组
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>参数数组</returns>
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("没有提供存储过程的名字或者 T-SQL 语句");

            string hashKey = connectionString + ":" + commandText;

            SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                return null;
            }
            else
            {
                return CloneParameters(cachedParameters);
            }
        }

        #endregion caching functions

        #region 获取参数

        /// <summary>
        /// 通过存储过程名跟连接字符串获取缓存中的参数数组
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <returns>参数数组</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        /// 通过存储过程名跟连接字符串获取缓存中的参数数组
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否返回输出参数</param>
        /// <returns>参数数组</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// 通过数据库连接对象获取缓存中的参数数组
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <returns>参数数组</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        /// 通过数据库连接对象获取缓存中的参数数组
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否返回输出参数</param>
        /// <returns>参数数组</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        /// 通过数据库连接对象获取缓存中的参数数组
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="includeReturnValueParameter">是否返回输出参数</param>
        /// <returns>参数数组</returns>
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");

            string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

            SqlParameter[] cachedParameters;

            cachedParameters = paramCache[hashKey] as SqlParameter[];
            if (cachedParameters == null)
            {
                SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cachedParameters = spParameters;
            }

            return CloneParameters(cachedParameters);
        }

        #endregion
    }
}
