using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Hx.Components.Data
{
    public class SqlHelper
    {
        #region 私有方法以及构造方法

        //不允许建立实例
        private SqlHelper() { }

        //获取数据库连接
        public static SqlConnection GetSqlConnection(string con)
        {

            try
            {
                return new SqlConnection(con);
            }
            catch
            {
                throw new Exception("连接出错");
            }

        }


        /// <summary>
        /// 这个方法给SqlCommand添加参数
        /// </summary>
        /// <param name="command">需要添加参数的command</param>
        /// <param name="commandParameters">参数数组</param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // 检查当为输入参数时赋予默认值
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// 这个方法使用的DataRow列的值填充SqlParameter数组
        /// </summary>
        /// <param name="commandParameters">需要填充到SqlParameter数组的DataRow</param>
        /// <param name="dataRow">需要用到的DataRow</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                return;
            }

            int i = 0;
            foreach (SqlParameter commandParameter in commandParameters)
            {
                if (commandParameter.ParameterName == null ||
                    commandParameter.ParameterName.Length <= 1)
                    throw new Exception(
                        "没有给SqlParameter赋予ParameterName");
                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                i++;
            }
        }

        /// <summary>
        /// 给SqlParameter数组赋值
        /// </summary>
        /// <param name="commandParameters">SqlParameter数组</param>
        /// <param name="parameterValues">参数值的数组</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }


            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("SqlParameter数组的长度跟参数值的数组的长度不匹配");
            }

            // Iterate through the SqlParameters, assigning the values from the corresponding position in the 
            // value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                // 如果目前的数组值来自IDbDataParameter 则填充
                if (parameterValues[i] is IDbDataParameter)
                {
                    IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        /// <summary>
        /// 为执行sql命令做准备
        /// </summary>
        /// <param name="command">sql指令</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandType">指令类型</param>
        /// <param name="commandText">sql语句或存储过程</param>
        /// <param name="commandParameters">执行sql需要传入的参数</param>
        /// <param name="mustCloseConnection">如果sql连接还没有关闭着返回true</param>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("缺少SqlCommand参数");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("缺少sql语句或存储过程语句");

            //当数据库没有连接则连接
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = true;
            }
            command.Connection = connection;
            command.CommandText = commandText;

            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("请提供一个已经打开的事务", "transaction");
                command.Transaction = transaction;
            }
            command.CommandType = commandType;

            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        ///执行的SqlCommand （返回结果并没有任何参数）
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        ///执行的SqlCommand （没有返回结果）
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 执行存储过程通过的SqlCommand （没有返回结果）
        /// </summary>
        /// <param name="connectionString">一个有效的数据库连接字符串</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （返回结果并没有任何参数）。 
        /// </summary>
        /// <param name="connection">SqlConnection连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （返回结果并没有任何参数）
        /// </summary>
        /// <param name="connection">一个现有的数据库连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("没有提供现有的数据库连接");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = true;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);
            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }

        /// <summary>
        ///执行存储过程通过的SqlCommand （没有返回结果）
        /// </summary>
        /// <param name="connection">一个现有的数据库连接</param>
        /// <param name="spName">存储过程名称</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("没有提供现有的数据库连接");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程的名字");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （没有返回结果并没有任何参数）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>A返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （没有返回结果）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("请为提供SqlTransaction提供连接", "transaction");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = true;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// 执行的SqlCommand （没有返回结果）对所提供的SqlTransaction 
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个数值表示此SqlCommand命令执行后影响的行数</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("请为提供SqlTransaction提供连接", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("请提供存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {

                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteNonQuery

        #region ExecuteDataset

        /// <summary>
        /// 执行的SqlCommand （即返回一个结果，并没有任何参数）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个结果）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString参数没有提供");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回一个结果）.
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString参数没有提供");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个结果，并没有任何参数）
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个结果）
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");

            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = true;
            cmd.CommandTimeout = connection.ConnectionTimeout;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();
                return ds;
            }
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回一个结果）.
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程的名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个结果，没有任何参数）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个结果）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = true;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回一个结果）对所提供的SqlTransaction 。.
        /// </summary>
        /// <param name="transaction">事务</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回结果集DataSet</returns>
        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteDataset

        #region ExecuteReader

        /// <summary>
        /// 这个枚举用来表是把数据库连接对象交给sqlHelper来处理还是调用者自己进行处理
        /// </summary>
        private enum SqlConnectionOwnership
        {
            /// <summary>SqlHelper来处理</summary>
            Internal,
            /// <summary>调用者自行处理</summary>
            External
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个只读结果）
        /// </summary>
        /// <param name="connection">一个有效的SqlConnection </param>
        /// <param name="transaction">一个有效的SqlTransaction,可以为null</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <param name="connectionOwnership">sqlHelper来处理还是调用者自己进行处理数据库连接对象</param>
        /// <returns>返回一个只读结果</returns>
        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");

            bool mustCloseConnection = true;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 180;
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
                SqlDataReader dataReader;

                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                bool canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }
                if (canClear)
                {
                    cmd.Parameters.Clear();
                }
                return dataReader;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个只读结果，没有参数）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个只读结果）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                if (connection != null) connection.Close();
                throw;
            }

        }

        /// <summary>
        /// 使用存储过程执行SqlCommand （即返回一个只读结果）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个只读结果，没有参数）
        /// </summary>
        /// <param name="connection">一个有效的SqlConnection </param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个只读结果）
        /// </summary>
        /// <param name="connection">一个有效的SqlConnection</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>
        /// 使用存储过程执行SqlCommand （即返回一个只读结果）
        /// </summary>
        /// <param name="connection">一个有效的SqlConnection</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个只读结果，没有传入的参数）对所提供的SqlTransaction 。.
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个只读结果）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction.", "transaction");

            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回一个只读结果）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回一个只读结果</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        /// <summary>
        /// 执行的SqlCommand （即返回第一行第一列，不带参数）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回第一行第一列）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回第一行第一列）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        ///执行的SqlCommand （即返回第一行第一列,不带参数）
        /// </summary>
        /// <param name="connection">有效的数据库连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 有效的数据库连接
        /// </summary>
        /// <param name="connection">有效的数据库连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            SqlCommand cmd = new SqlCommand();

            bool mustCloseConnection = true;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);
            object retval = cmd.ExecuteScalar();

            cmd.Parameters.Clear();

            if (mustCloseConnection)
                connection.Close();

            return retval;
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回第一行第一列）
        /// </summary>
        /// <param name="connection">有效的数据库连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行SqlCommand （即返回第一行第一列，不带参数）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SqlCommand （即返回第一行第一列）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction", "transaction");

            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = true;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            object retval = cmd.ExecuteScalar();

            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回第一行第一列）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回数据库中第一行第一列的数据</returns>
        public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteScalar

        #region ExecuteXmlReader
        /// <summary>
        /// 执行的SqlCommand （即返回一个XmlReader，并没有任何参数）对所提供的SqlConnection 。
        /// </summary>
        /// <param name="connection">有效的数据库连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回的xmlReader对象</returns>
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        ///  执行的SqlCommand （即返回一个XmlReader）对所提供的SqlConnection 。
        /// </summary>
        /// <param name="connection">有效的数据库连接</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回的xmlReader对象</returns>
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");

            bool mustCloseConnection = true;
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);
                XmlReader retval = cmd.ExecuteXmlReader();
                cmd.Parameters.Clear();

                return retval;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回一个XmlReader）对所提供的SqlConnection 。
        /// </summary>
        /// <param name="connection">有效的数据库连接</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回的xmlReader对象</returns>
        public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行SqlCommand （即返回第一行第一列，不带参数）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <returns>返回的xmlReader对象</returns>
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// 执行SqlCommand （即返回第一行第一列）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回的xmlReader对象</returns>
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction", "transaction");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = true;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            XmlReader retval = cmd.ExecuteXmlReader();

            cmd.Parameters.Clear();
            return retval;
        }

        /// <summary>
        /// 使用存储过程执行SqlCommand （即返回第一行第一列）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">一个有效的SqlTransaction</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <returns>返回的xmlReader对象</returns>
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion ExecuteXmlReader

        #region FillDataset
        /// <summary>
        /// 执行的SqlCommand （即返回一个DataSet，并没有任何参数）
        /// </summary>
        /// <param name="connectionString">数据库连接对象字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）</param>
        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("缺少数据库连接字符串");
            if (dataSet == null) throw new ArgumentNullException("dataSet不能为null");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                FillDataset(connection, commandType, commandText, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个DataSet）
        /// </summary>
        /// <param name="connectionString">数据库连接对象字符串</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>
        public static void FillDataset(string connectionString, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("缺少数据库连接字符串");
            if (dataSet == null) throw new ArgumentNullException("dataSet不能为null");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
        }

        /// <summary>
        /// 使用存储过程执行的SqlCommand （即返回一个DataSet）
        /// </summary>
        /// <param name="connectionString">数据库连接对象字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>    
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        public static void FillDataset(string connectionString, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("缺少数据库连接字符串");
            if (dataSet == null) throw new ArgumentNullException("dataSet不能为null");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个DataSet，不带参数）
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>    
        public static void FillDataset(SqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames)
        {
            FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个DataSet）
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        public static void FillDataset(SqlConnection connection, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// 执行的SqlCommand （即返回一个DataSet）
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        public static void FillDataset(SqlConnection connection, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (dataSet == null) throw new ArgumentNullException("dataSet不能为null");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程名");


            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 执行SqlCommand （即返回一个DataSet，不带参数）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>
        public static void FillDataset(SqlTransaction transaction, CommandType commandType,
            string commandText,
            DataSet dataSet, string[] tableNames)
        {
            FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
        }

        /// <summary>
        /// 执行SqlCommand （即返回一个DataSet）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        public static void FillDataset(SqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
        }

        /// <summary>
        /// 使用存储过程执行SqlCommand （即返回一个DataSet）对所提供的SqlTransaction 。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="spName">存储过程</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>
        /// <param name="parameterValues">以数组形式提供SqlCommand命令中用到的参数列表</param>
        public static void FillDataset(SqlTransaction transaction, string spName,
            DataSet dataSet, string[] tableNames,
            params object[] parameterValues)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供一个打开的transaction", "transaction");
            if (dataSet == null) throw new ArgumentNullException("dataSet不能为null");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程名");

            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);

                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
            }
            else
            {
                FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
            }
        }

        /// <summary>
        /// 私有方法，执行的SqlCommand （即返回一个结果集）对指定的SqlTransaction和SqlConnection使用提供的参数。
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="commandType">SqlCommand命令类型 (存储过程， T-SQL语句， 等等。)</param>
        /// <param name="commandText">存储过程的名字或者 T-SQL 语句</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableNames">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）
        /// </param>
        /// <param name="commandParameters">以数组形式提供SqlCommand命令中用到的参数列表</param>
        private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType,
            string commandText, DataSet dataSet, string[] tableNames,
            params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");
            if (dataSet == null) throw new ArgumentNullException("dataSet不能为null");

            SqlCommand command = new SqlCommand();
            bool mustCloseConnection = true;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
            {

                if (tableNames != null && tableNames.Length > 0)
                {
                    string tableName = "Table";
                    for (int index = 0; index < tableNames.Length; index++)
                    {
                        if (tableNames[index] == null || tableNames[index].Length == 0) throw new ArgumentException("该tableNames参数必须包含一个表格中的字段，这里提供了一个值为空或空字符串.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName += (index + 1).ToString();
                    }
                }

                dataAdapter.Fill(dataSet);
                command.Parameters.Clear();
            }

            if (mustCloseConnection)
                connection.Close();
        }
        #endregion

        #region UpdateDataset
        /// <summary>
        /// 执行各自的命令为每个插入，更新或删除的行中的数据。
        /// </summary>

        /// <param name="insertCommand">插入的SqlCommand</param>
        /// <param name="deleteCommand">删除的SqlCommand</param>
        /// <param name="updateCommand">更新的SqlCommand</param>
        /// <param name="dataSet">一个DataSet ，其中将载有结果</param>
        /// <param name="tableName">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）</param>
        public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (insertCommand == null) throw new ArgumentNullException("没有提供插入的SqlCommand");
            if (deleteCommand == null) throw new ArgumentNullException("没有提供删除的SqlCommand");
            if (updateCommand == null) throw new ArgumentNullException("没有提供更新的SqlCommand");
            if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("没有提供tableName");

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter())
            {
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;

                dataAdapter.Update(dataSet, tableName);

                dataSet.AcceptChanges();
            }
        }
        #endregion

        #region CreateCommand
        /// <summary>
        /// 简化建立一个SQL命令对象，允许存储过程和可选的参数
        /// </summary>
        /// <param name="connection">有效的SqlConnection对象</param>
        /// <param name="spName">存储过程</param>
        /// <param name="sourceColumns">该数组将被用于创建表映射允许DataTables被引用的用户定义的名称（可能是实际的表名称）</param>
        /// <returns>sql命令对象</returns>
        public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("存储过程名");

            SqlCommand cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            if ((sourceColumns != null) && (sourceColumns.Length > 0))
            {

                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                for (int index = 0; index < sourceColumns.Length; index++)
                    commandParameters[index].SourceColumn = sourceColumns[index];

                AttachParameters(cmd, commandParameters);
            }

            return cmd;
        }
        #endregion

        #region ExecuteNonQueryTypedParams
        /// <summary>
        /// 执行存储过程通过的SqlCommand （没有返回结果）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程</param>
        /// <param name="dataRow">储存参数值的DataRow.</param>
        /// <returns>返回更新的行数</returns>
        public static int ExecuteNonQueryTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程名");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行存储过程通过的SqlCommand （没有返回结果）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connection">有效的SqlConnection对象</param>
        /// <param name="spName">存储过程</param>
        /// <param name="dataRow">储存参数值的DataRow.</param>
        /// <returns>返回更新的行数</returns>
        public static int ExecuteNonQueryTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("没有提供SqlConnection对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {

                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行存储过程通过的SqlCommand （没有返回结果）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回更新的行数</returns>
        public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供有连接的SqlTransaction对象", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteDatasetTypedParams
        /// <summary>
        ///执行预存程序通过的SqlCommand （返回DataSet）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回结果集Dataset</returns>
        public static DataSet ExecuteDatasetTypedParams(string connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回DataSet）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connection">有效的SqlConnection对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回结果集Dataset</returns>
        public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回DataSet）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回结果集Dataset</returns>
        public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供有连接的SqlTransaction对象", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");


            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region ExecuteReaderTypedParams
        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回SqlDataReader）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回结果集SqlDataReader</returns>
        public static SqlDataReader ExecuteReaderTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {

                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回SqlDataReader）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connection">有效的SqlConnection对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回结果集SqlDataReader</returns>
        public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回SqlDataReader）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回结果集SqlDataReader</returns>
        public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供有连接的SqlTransaction对象.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteScalarTypedParams
        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回第一行第一列）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回第一行第一列</returns>
        public static object ExecuteScalarTypedParams(String connectionString, String spName, DataRow dataRow)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("没有提供数据库连接字符串");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回第一行第一列）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connection">有效的SqlConnection对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow</param>
        /// <returns>返回第一行第一列</returns>
        public static object ExecuteScalarTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {

                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回第一行第一列）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow.</param>
        /// <returns>返回第一行第一列</returns>
        public static object ExecuteScalarTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供有连接的SqlTransaction对象", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        #region ExecuteXmlReaderTypedParams
        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回XmlReader）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="connection">有效的SqlConnection对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow.</param>
        /// <returns>返回XmlReader</returns>
        public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, String spName, DataRow dataRow)
        {
            if (connection == null) throw new ArgumentNullException("没有提供数据库连接对象");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// 执行预存程序通过的SqlCommand （返回XmlReader）使用的DataRow列的值的存储过程的参数值。这种方法将查询数据库，发现参数的存储过程（第一次每一个存储过程被称为） ，并指定值的基础上连续值。
        /// </summary>
        /// <param name="transaction">有效的SqlTransaction对象</param>
        /// <param name="spName">存储过程名</param>
        /// <param name="dataRow">储存参数值的DataRow.</param>
        /// <returns>返回XmlReader</returns>
        public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, String spName, DataRow dataRow)
        {
            if (transaction == null) throw new ArgumentNullException("没有提供SqlTransaction对象");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("没有提供有连接的SqlTransaction对象.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("没有提供存储过程");

            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {

                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, dataRow);

                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return SqlHelper.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
            }
        }
        #endregion

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="connectionString">多条SQL语句</param>	
        /// <param name="SQLStringList">多条SQL语句</param>	
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlTran(string connectionString, List<String> SQLStringList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="connectionString">泛型列</param>
        /// <param name="SQLStringList">泛型列</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlTran(string connectionString, List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = trans;
                    int count = 0;
                    //循环
                    foreach (CommandInfo myDE in cmdList)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = myDE.CommandText;
                        cmd.CommandType = myDE.CommandType;
                        AttachParameters(cmd, myDE.Parameters);
                        try
                        {
                            int result = cmd.ExecuteNonQuery();
                            count += result;
                        }
                        catch
                        {
                            trans.Rollback();
                            return 0;
                        }
                    }
                    trans.Commit();
                    return count;

                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="connectionString">泛型列</param>
        /// <param name="SQLStringList">泛型列</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlTranWithException(string connectionString, List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = trans;
                    int count = 0;
                    //循环
                    foreach (CommandInfo myDE in cmdList)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = myDE.CommandText;
                        cmd.CommandType = myDE.CommandType;
                        AttachParameters(cmd, myDE.Parameters);
                        try
                        {
                            int result = cmd.ExecuteNonQuery();
                            count += result;
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                    trans.Commit();
                    return count;

                }
            }
        }

        ///<summary>
        ///直接通过DataSet更新数据库表
        ///</summary>
        ///<param name="ds"></param>
        ///<param name="strTblName">ds中要更新的表名</param>
        ///<param name="strConnection"></param>
        ///<returns></returns>
        public int UpdateByDataSet(DataSet ds, string strTblName, string strConnection)
        {

            SqlConnection conn = new SqlConnection(strConnection);

            SqlDataAdapter myAdapter = new SqlDataAdapter();
            SqlCommand myCommand = new SqlCommand("select  * from " + strTblName, conn);
            myAdapter.SelectCommand = myCommand;
            SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(myAdapter);
            return myAdapter.Update(ds, strTblName);
        }

        /// <summary>
        /// 判断reader里是否存在列名
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ContainsField(IDataReader reader, string name)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == name)
                    return true;
            }

            return false;
        }
    }
}
