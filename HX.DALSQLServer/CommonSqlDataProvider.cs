using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Providers;
using System.Net;
using Hx.Components.Config;
using Hx.Tools;
using Hx.Components.Entity;
using Hx.Components.Query;
using System.Data.SqlClient;
using System.Data;
using Hx.Components.Data;
using Hx.Components.Enumerations;

namespace HX.DALSQLServer
{
    public class CommonSqlDataProvider : CommonDataProvider
    {
        private string _con;
        private string _dbowner;
        private static object sync_helper = new object();

        #region 初始化
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="constr">连接字符串</param>
        /// <param name="owner">数据库所有者</param>
        public CommonSqlDataProvider(string constr, string owner)
        {
            CommConfig config = CommConfig.GetConfig();
            _con = EncryptString.DESDecode(constr, config.AppSetting["key"]);
            _dbowner = owner;
        }
        #endregion

        #region 后台管理员

        /// <summary>
        ///  获取用于加密的值
        /// </summary>
        /// <param name="userID">管理员ID</param>
        /// <returns>用于加密的值</returns>
        public override string GetAdminKey(int userID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 CheckKey from HX_AdminUser ");
            strSql.Append(" where ID=@ID ");
            object o = SqlHelper.ExecuteScalar(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", userID));
            return o as string;
        }

        /// <summary>
        /// 管理员是否已经存在
        /// </summary>
        /// <param name="name">管理员ID</param>
        /// <returns>管理员是否存在</returns>
        public override bool ExistsAdmin(int id)
        {
            string sql = "select count(1) from HX_AdminUser where ID=@ID";
            int i = Convert.ToInt32(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, new SqlParameter("@ID", id)));
            if (i > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 通过用户名获得后台管理员信息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns>管理员实体信息</returns>
        public override AdminInfo GetAdminByName(string UserName)
        {
            string sql = "select * from HX_AdminUser where UserName=@UserName";
            AdminInfo admin = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@UserName", UserName)))
            {
                if (reader.Read())
                {
                    admin = PopulateAdmin(reader);
                }
            }
            return admin;
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>添加成功返回ID</returns>
        public override int AddAdmin(AdminInfo model)
        {
            SerializerData data = model.GetSerializerData();
            string sql = @"
            INSERT INTO HX_AdminUser(UserName,Password,Administrator,LastLoginIP,LastLoginTime,[PropertyNames],[PropertyValues],[UserRole])
            VALUES (@UserName,@Password,@Administrator,@LastLoginIP,@LastLoginTime,@PropertyNames,@PropertyValues,@UserRole)
            ;SELECT @@IDENTITY";
            SqlParameter[] p = 
            {
                new SqlParameter("@UserName",model.UserName),
                new SqlParameter("@Password",model.Password),
                new SqlParameter("@Administrator",model.Administrator),
                new SqlParameter("@LastLoginIP",model.LastLoginIP),
                new SqlParameter("@LastLoginTime",model.LastLoginTime),
                new SqlParameter("@UserRole",model.UserRole),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            model.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return model.ID;
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>修改是否成功</returns>
        public override bool UpdateAdmin(AdminInfo model)
        {
            SerializerData data = model.GetSerializerData();
            string sql = @"UPDATE HX_AdminUser SET
            UserName = @UserName
            ,Password = @Password
            ,Administrator = @Administrator
            ,LastLoginIP = @LastLoginIP
            ,LastLoginTime = @LastLoginTime
            ,UserRole = @UserRole
            ,[PropertyNames] = @PropertyNames
            ,[PropertyValues] = @PropertyValues
            WHERE ID = @ID
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@UserName",model.UserName),
                new SqlParameter("@Password",model.Password),
                new SqlParameter("@Administrator",model.Administrator),
                new SqlParameter("@LastLoginIP",model.LastLoginIP),
                new SqlParameter("@LastLoginTime",model.LastLoginTime),
                new SqlParameter("@UserRole",(int)model.UserRole),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
                new SqlParameter("@ID",model.ID)
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="AID">管理员ID</param>
        /// <returns>删除是否成功</returns>
        public override bool DeleteAdmin(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from HX_AdminUser ");
            strSql.Append(" where ID=@ID ");
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", id));
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 通过ID获取管理员
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>管理员实体信息</returns>
        public override AdminInfo GetAdmin(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from HX_AdminUser ");
            strSql.Append(" where ID=@ID ");
            AdminInfo admin = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", id)))
            {
                if (reader.Read())
                {
                    admin = PopulateAdmin(reader);
                }
            }
            return admin;
        }

        /// <summary>
        /// 验证用户登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户ID</returns>
        public override int ValiAdmin(string userName, string password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID from HX_AdminUser");
            strSql.Append(" where UserName=@UserName and Password=@PassWord");

            object obj = SqlHelper.ExecuteScalar(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@UserName", userName), new SqlParameter("@PassWord", password));
            if (obj == null)
            {
                return -2;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 返回所有用户
        /// </summary>
        /// <returns>返回所有用户</returns>
        public override List<AdminInfo> GetAllAdmins()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from HX_AdminUser WHERE [UserRole] != " + (int)UserRoleType.销售员);


            List<AdminInfo> admins = new List<AdminInfo>();
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    admins.Add(PopulateAdmin(reader));
                }
            }
            return admins;
        }

        public override List<AdminInfo> GetUsers()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from HX_AdminUser WHERE [UserRole] = " + (int)UserRoleType.销售员);


            List<AdminInfo> admins = new List<AdminInfo>();
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    admins.Add(PopulateAdmin(reader));
                }
            }
            return admins;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userID">管理员ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>修改密码是否成功</returns>
        public override bool ChangeAdminPw(int userID, string oldPassword, string newPassword)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update HX_AdminUser set ");
            strSql.Append("Password=@NewPassword");
            strSql.Append(" where ID=@ID and Password=@Password ");
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", userID), new SqlParameter("@Password", oldPassword), new SqlParameter("@NewPassword", newPassword));
            if (result < 1)
                return false;
            return true;
        }

        #endregion

        #region 日志

        /// <summary>
        /// 写入日志信息
        /// </summary>
        /// <param name="log"></param>
        public override void WriteEventLogEntry(EventLogEntry log)
        {
            try
            {
                string sql = "HX_AddEvent";
                SqlParameter[] parameters = 
                    {
                        new SqlParameter("@Uniquekey", log.Uniquekey),
                        new SqlParameter("@EventType", log.EventType),
                        new SqlParameter("@EventID",log.EventID),
                        new SqlParameter("@Message",log.Message),
                        new SqlParameter("@Category",log.Category),
                        new SqlParameter("@MachineName",log.MachineName),
                        new SqlParameter("@ApplicationName",log.ApplicationName),
                        new SqlParameter("@ApplicationID",log.ApplicationID),
                        new SqlParameter("@AppType",log.ApplicationType),
                        new SqlParameter("@EntryID",log.EntryID),
                        new SqlParameter("@PCount",log.PCount),
                        new SqlParameter("@LastUpdateTime",log.LastUpdateTime)
                    };
                SqlHelper.ExecuteNonQuery(_con, CommandType.StoredProcedure, sql, parameters);
            }
            catch { }
        }

        /// <summary>
        /// 根据时间清除日志
        /// </summary>
        /// <param name="dt"></param>
        public override void ClearEventLog(DateTime dt)
        {
            throw new NotImplementedException();
        }

        public override List<EventLogEntry> GetEventLogs(int pageindex, int pagesize, EventLogQuery query, out int total)
        {
            List<EventLogEntry> eventlist = new List<EventLogEntry>();
            SqlParameter p;
            if (pageindex != -1)
            {
                using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
                {
                    while (reader.Read())
                    {
                        eventlist.Add(PopulateEventLogEntry(reader));
                    }
                }
                total = int.Parse(p.Value.ToString());
            }

            else
            {
                using (IDataReader reader = CommonSelectSql.SelectGetReader(_con, pagesize, query))
                {
                    while (reader.Read())
                    {
                        eventlist.Add(PopulateEventLogEntry(reader));
                    }
                }
                total = eventlist.Count();
            }
            return eventlist;
        }
        #endregion

        #region 银行

        public override List<BankInfo> GetBankList()
        {
            List<BankInfo> list = new List<BankInfo>();
            string sql = "SELECT * FROM HX_Bank";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateBank(reader));
                }
            }

            return list;
        }

        public override void AddBank(BankInfo entity)
        {
            string sql = @"INSERT INTO HX_Bank(
            [Name]
            ,[PropertyNames]
            ,[PropertyValues]
            )VALUES(
            @Name
            ,@PropertyNames
            ,@PropertyValues
            )";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@Name",entity.Name),
				new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void UpdateBank(BankInfo entity)
        {
            string sql = @"UPDATE HX_Bank SET 
            [Name] = @Name 
            ,PropertyNames = @PropertyNames
            ,PropertyValues = @PropertyValues
            WHERE [ID] = @ID";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Name",entity.Name),
				new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteBank(string ids)
        {
            string sql = "DELETE FROM HX_Bank WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        #endregion

        #region 系统全局设置

        public override void AddGlobalSetting(GlobalSettingInfo entity)
        {
            string sql = @"
            IF NOT EXISTS(SELECT * FROM HX_GlobalSetting)
            BEGIN
                INSERT INTO HX_GlobalSetting(
                    PropertyNames
                    ,PropertyValues
                )VALUES (
                    @PropertyNames
                    ,@PropertyValues
                )
            END
            ELSE 
            BEGIN
                UPDATE HX_GlobalSetting SET
                PropertyNames = @PropertyNames
                ,PropertyValues = @PropertyValues
            END";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
				new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override GlobalSettingInfo GetGlobalSetting()
        {
            GlobalSettingInfo entity = new GlobalSettingInfo();
            string sql = "SELECT * FROM HX_GlobalSetting";

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                if (reader.Read())
                {
                    entity = PopulateGlobalSettingInfo(reader);
                }
                else
                {
                    entity.BankProfitMargin = "306.7";
                }
            }

            return entity;
        }

        #endregion

        #region 日报用户

        public override List<DayReportUserInfo> GetDayReportUserList()
        {
            List<DayReportUserInfo> list = new List<DayReportUserInfo>();
            string sql = "SELECT * FROM HX_DayReportUser";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateDayReportUser(reader));
                }
            }

            return list;
        }

        public override void AddDayReportUser(DayReportUserInfo entity)
        {
            string sql = @"
            IF NOT EXISTS(SELECT * FROM HX_DayReportUser WHERE [UserTag] = @UserTag)
            BEGIN
                INSERT INTO HX_DayReportUser(
                [UserTag]
                ,[UserName]
                ,[DayReportDep]
                ,[CorporationID]
                ,[CorporationName]
                ,[PropertyNames]
                ,[PropertyValues]
                )VALUES(
                @UserTag
                ,@UserName
                ,@DayReportDep
                ,@CorporationID
                ,@CorporationName
                ,@PropertyNames
                ,@PropertyValues
                )
            END";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@UserTag",entity.UserTag),
                new SqlParameter("@UserName",entity.UserName),
                new SqlParameter("@DayReportDep",entity.DayReportDep),
                new SqlParameter("@CorporationID",entity.CorporationID),
                new SqlParameter("@CorporationName",entity.CorporationName),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void UpdateDayReportUser(DayReportUserInfo entity)
        {
            string sql = @"
            IF NOT EXISTS(SELECT * FROM HX_DayReportUser WHERE [UserTag] = @UserTag AND [ID] != @ID)
            BEGIN
                UPDATE HX_DayReportUser SET 
                [UserTag] = @UserTag 
                ,UserName = @UserName
                ,DayReportDep = @DayReportDep
                ,CorporationID = @CorporationID
                ,CorporationName = @CorporationName
                ,[PropertyNames] = @PropertyNames
                ,[PropertyValues] = @PropertyValues
                WHERE [ID] = @ID
            END";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@UserTag",entity.UserTag),
                new SqlParameter("@UserName",entity.UserName),
                new SqlParameter("@DayReportDep",entity.DayReportDep),
                new SqlParameter("@CorporationID",entity.CorporationID),
                new SqlParameter("@CorporationName",entity.CorporationName),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteDayReportUser(string ids)
        {
            string sql = string.Format("DELETE FROM HX_DayReportUser WHERE [ID] IN ({0})", ids);
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        #endregion

        #region 日报

        public override List<DailyReportInfo> GetDailyReportList(DailyReportQuery query)
        {
            List<DailyReportInfo> list = new List<DailyReportInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, 1, int.MaxValue, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateDailyReport(reader));
                }
            }
            return list;
        }

        public override void CreateAndUpdateDailyReport(string tablename, DailyReportInfo entity)
        {
            string sql = string.Format(@"
            IF NOT EXISTS(SELECT * FROM {0} WHERE [DayUnique] = @DayUnique AND [CorporationID] = @CorporationID)
            BEGIN
                INSERT INTO {0}(
                    [PropertyNames]
                    ,[PropertyValues]
                    ,[DayUnique]
                    ,[CorporationID]
                    ,[Creator]
                    ,[CreateTime]
                    ,[LastUpdateUser]
                    ,[LastUpdateTime]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues
                    ,@DayUnique
                    ,@CorporationID
                    ,@Creator
                    ,GETDATE()
                    ,@LastUpdateUser
                    ,GETDATE()
                )
            END
            ELSE 
            BEGIN
                UPDATE {0} SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                    ,[LastUpdateUser] = @LastUpdateUser
                    ,[LastUpdateTime] = GETDATE()
                WHERE [DayUnique] = @DayUnique AND [CorporationID] = @CorporationID
            END
            ", tablename);
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            { 
                new SqlParameter("@CorporationID",entity.CorporationID),
                new SqlParameter("@Creator",entity.Creator),
                new SqlParameter("@DayUnique",entity.DayUnique),
                new SqlParameter("@LastUpdateUser",entity.LastUpdateUser),
                new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };

            lock (sync_helper)
            {
                SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            }
        }

        #endregion

        #region 日报录入记录

        public override List<DailyReportHistoryInfo> GetDailyReportHistoryList(int pageindex, int pagesize, DailyReportHistoryQuery query, ref int recordcount)
        {
            List<DailyReportHistoryInfo> list = new List<DailyReportHistoryInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateDailyReportHistory(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override void CreateDailyReportHistory(DailyReportHistoryInfo entity)
        {
            string sql = @"
                INSERT INTO HX_DayReportHistory(
                    [PropertyNames]
                    ,[PropertyValues]
                    ,[DayUnique]
                    ,[ReportDepartment]
                    ,[ReportCorporationID]
                    ,[CreatorCorporationID]
                    ,[CreatorCorporationName]
                    ,[CreatorDepartment]
                    ,[Creator]
                    ,[CreateTime]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues
                    ,@DayUnique
                    ,@ReportDepartment
                    ,@ReportCorporationID
                    ,@CreatorCorporationID
                    ,@CreatorCorporationName
                    ,@CreatorDepartment
                    ,@Creator
                    ,GETDATE()
                )";
            SerializerData data = entity.Modify.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@DayUnique",entity.DayUnique),
                new SqlParameter("@ReportDepartment",entity.ReportDepartment),
                new SqlParameter("@ReportCorporationID",entity.ReportCorporationID),
                new SqlParameter("@CreatorCorporationID",entity.CreatorCorporationID),
                new SqlParameter("@CreatorCorporationName",entity.CreatorCorporationName),
                new SqlParameter("@CreatorDepartment",entity.CreatorDepartment),
                new SqlParameter("@Creator",entity.Creator),
                new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };

            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        #endregion

        #region 日报审核记录

        public override List<DailyReportCheckHistoryInfo> GetDailyReportCheckHistoryList(int pageindex, int pagesize, DailyReportCheckHistoryQuery query, ref int recordcount)
        {
            List<DailyReportCheckHistoryInfo> list = new List<DailyReportCheckHistoryInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateDailyReportCheckHistory(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override void CreateDailyReportCheckHistory(DailyReportCheckHistoryInfo entity)
        {
            string sql = @"
                INSERT INTO HX_DayReportCheckHistory(
                    [PropertyNames]
                    ,[PropertyValues]
                    ,[DayUnique]
                    ,[ReportDepartment]
                    ,[ReportCorporationID]
                    ,[CreatorCorporationID]
                    ,[CreatorCorporationName]
                    ,[CreatorDepartment]
                    ,[Creator]
                    ,[CreateTime]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues
                    ,@DayUnique
                    ,@ReportDepartment
                    ,@ReportCorporationID
                    ,@CreatorCorporationID
                    ,@CreatorCorporationName
                    ,@CreatorDepartment
                    ,@Creator
                    ,GETDATE()
                )";
            SerializerData data = entity.CheckedInfo.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@DayUnique",entity.DayUnique),
                new SqlParameter("@ReportDepartment",entity.ReportDepartment),
                new SqlParameter("@ReportCorporationID",entity.ReportCorporationID),
                new SqlParameter("@CreatorCorporationID",entity.CreatorCorporationID),
                new SqlParameter("@CreatorCorporationName",entity.CreatorCorporationName),
                new SqlParameter("@CreatorDepartment",entity.CreatorDepartment),
                new SqlParameter("@Creator",entity.Creator),
                new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };

            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        #endregion

        #region 日报栏目

        public override List<DailyReportModuleInfo> GetDailyReportModuleList()
        {
            List<DailyReportModuleInfo> list = new List<DailyReportModuleInfo>();
            string sql = "SELECT * FROM HX_DayReportModule";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateDailyReportModule(reader));
                }
            }

            return list;
        }

        public override void AddDailyReportModule(DailyReportModuleInfo entity)
        {
            string sql = @"INSERT INTO HX_DayReportModule(
            [Sort]
            ,[Name]
            ,[Department]
            ,[Ismonthlytarget]
            ,[Description]
            ,[Mustinput]
            ,[Iscount]
            )VALUES(
            @Sort
            ,@Name
            ,@Department
            ,@Ismonthlytarget
            ,@Description
            ,@Mustinput
            ,@Iscount
            )";
            SqlParameter[] p = 
            {
                new SqlParameter("@Sort",entity.Sort),
                new SqlParameter("@Name",entity.Name),
				new SqlParameter("@Department", entity.Department),
				new SqlParameter("@Ismonthlytarget", entity.Ismonthlytarget),
                new SqlParameter("@Description",entity.Description),
				new SqlParameter("@Mustinput", entity.Mustinput),
				new SqlParameter("@Iscount", entity.Iscount)

            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void UpdateDailyReportModule(DailyReportModuleInfo entity)
        {
            string sql = @"UPDATE HX_DayReportModule SET 
            [Sort] = @Sort
            ,[Name] = @Name 
            ,[Department] = @Department
            ,[Ismonthlytarget] = @Ismonthlytarget
            ,[Description] = @Description
            ,[Mustinput] = @Mustinput
            ,[Iscount] = @Iscount
            WHERE [ID] = @ID";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Sort",entity.Sort),
                new SqlParameter("@Name",entity.Name),
				new SqlParameter("@Department", entity.Department),
				new SqlParameter("@Ismonthlytarget", entity.Ismonthlytarget),
				new SqlParameter("@Description", entity.Description),
				new SqlParameter("@Mustinput", entity.Mustinput),
                new SqlParameter("@Iscount",entity.Iscount)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteDailyReportModule(string ids)
        {
            string sql = "DELETE FROM HX_DayReportModule WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        #endregion

        #region crm报表

        public override void CreateAndUpdateCRMReport(CRMReportInfo entity)
        {
            string sql = @"
            IF NOT EXISTS(SELECT * FROM HX_CRMReport WHERE ID=@ID)
            BEGIN
                INSERT INTO HX_CRMReport(
                    [PropertyNames]
                    ,[PropertyValues]
                    ,[CorporationID]
                    ,[Creator]
                    ,[CreateTime]
                    ,[LastUpdateUser]
                    ,[LastUpdateTime]
                    ,[Date]
                    ,[CRMReportType]
                    ,[MonthStr]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues
                    ,@CorporationID
                    ,@Creator
                    ,GETDATE()
                    ,@LastUpdateUser
                    ,GETDATE()
                    ,@Date
                    ,@CRMReportType
                    ,@MonthStr
                )
            END
            ELSE 
            BEGIN
                UPDATE HX_CRMReport SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                    ,[LastUpdateUser] = @LastUpdateUser
                    ,[LastUpdateTime] = GETDATE()
                WHERE ID=@ID
            END
            ";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            { 
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values),
                new SqlParameter("@CorporationID",entity.CorporationID),
                new SqlParameter("@Creator",entity.Creator),
                new SqlParameter("@LastUpdateUser",entity.LastUpdateUser),
                new SqlParameter("@Date",entity.Date),
                new SqlParameter("@CRMReportType",(int)entity.CRMReportType),
                new SqlParameter("@MonthStr",entity.Date.ToString("yyyyMM")),
            };

            lock (sync_helper)
            {
                SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            }
        }

        public override List<CRMReportInfo> GetCRMReportList(CRMReportQuery query)
        {
            List<CRMReportInfo> list = new List<CRMReportInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, 1, int.MaxValue, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateCRMReport(reader));
                }
            }

            return list;
        }

        public override void DeleteCRMReport(string ids)
        {
            string sql = string.Format("DELETE FROM HX_CRMReport WHERE [ID] IN ({0})", ids);
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        #endregion

        #region 月度目标

        public override List<MonthlyTargetInfo> GetMonthTargetList(MonthTargetQuery query)
        {
            List<MonthlyTargetInfo> list = new List<MonthlyTargetInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, 1, int.MaxValue, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateMonthTarget(reader));
                }
            }

            return list;
        }

        public override void CreateAndUpdateMonthlyTarget(MonthlyTargetInfo entity)
        {
            string sql = string.Format(@"
            IF NOT EXISTS(SELECT * FROM {0} WHERE [MonthUnique] = @MonthUnique AND [CorporationID] = @CorporationID AND [Department]=@Department)
            BEGIN
                INSERT INTO {0}(
                    [PropertyNames]
                    ,[PropertyValues]
                    ,[MonthUnique]
                    ,[CorporationID]
                    ,[Department]
                    ,[Creator]
                    ,[CreateTime]
                    ,[LastUpdateUser]
                    ,[LastUpdateTime]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues
                    ,@MonthUnique
                    ,@CorporationID
                    ,@Department
                    ,@Creator
                    ,GETDATE()
                    ,@LastUpdateUser
                    ,GETDATE()
                )
            END
            ELSE 
            BEGIN
                UPDATE {0} SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                    ,[LastUpdateUser] = @LastUpdateUser
                    ,[LastUpdateTime] = GETDATE()
                WHERE [MonthUnique] = @MonthUnique AND [CorporationID] = @CorporationID AND [Department]=@Department
            END
            ", "HX_MonthlyTarget");
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            { 
                new SqlParameter("@CorporationID",entity.CorporationID),
                new SqlParameter("@Department",entity.Department),
                new SqlParameter("@Creator",entity.Creator),
                new SqlParameter("@MonthUnique",entity.MonthUnique),
                new SqlParameter("@LastUpdateUser",entity.LastUpdateUser),
                new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };

            lock (sync_helper)
            {
                SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            }
        }

        #endregion

        #region 月度目标录入记录

        public override List<MonthlyTargetHistoryInfo> GetMonthlyTargetHistoryList(int pageindex, int pagesize, MonthlyTargetHistoryQuery query, ref int recordcount)
        {
            List<MonthlyTargetHistoryInfo> list = new List<MonthlyTargetHistoryInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateMonthlyTargetHistory(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override void CreateMonthlyTargetHistory(MonthlyTargetHistoryInfo entity)
        {
            string sql = @"
                INSERT INTO HX_MonthlyTargetHistory(
                    [PropertyNames]
                    ,[PropertyValues]
                    ,[MonthUnique]
                    ,[ReportDepartment]
                    ,[ReportCorporationID]
                    ,[CreatorCorporationID]
                    ,[CreatorCorporationName]
                    ,[CreatorDepartment]
                    ,[Creator]
                    ,[CreateTime]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues
                    ,@MonthUnique
                    ,@ReportDepartment
                    ,@ReportCorporationID
                    ,@CreatorCorporationID
                    ,@CreatorCorporationName
                    ,@CreatorDepartment
                    ,@Creator
                    ,GETDATE()
                )";
            SerializerData data = entity.Modify.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@MonthUnique",entity.MonthUnique),
                new SqlParameter("@ReportDepartment",entity.ReportDepartment),
                new SqlParameter("@ReportCorporationID",entity.ReportCorporationID),
                new SqlParameter("@CreatorCorporationID",entity.CreatorCorporationID),
                new SqlParameter("@CreatorCorporationName",entity.CreatorCorporationName),
                new SqlParameter("@CreatorDepartment",entity.CreatorDepartment),
                new SqlParameter("@Creator",entity.Creator),
                new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };

            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        #endregion

        #region 微信活动

        #region 测试活动

        public override List<WeixinActInfo> GetWeixinActList(int pageindex, int pagesize, WeixinActQuery query, ref int recordcount)
        {
            List<WeixinActInfo> list = new List<WeixinActInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateWeixinAct(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override WeixinActInfo GetWeixinActInfo(string openid)
        {
            string sql = "SELECT * FROM HX_WeixinAct WHERE [Openid] = @Openid";
            SqlParameter[] p = 
            { 
                new SqlParameter("@Openid",openid)
            };
            WeixinActInfo entity = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, p))
            {
                if (reader.Read())
                {
                    entity = PopulateWeixinAct(reader);
                }
            }
            return entity;
        }

        public override bool AddWeixinAct(WeixinActInfo entity)
        {
            string sql = @"INSERT INTO HX_WeixinAct(
                [Openid]
                ,[Nickname]
                ,[Sex]
                ,[City]
                ,[Country]
                ,[Province]
                ,[Subscribetime]
                )VALUES(
                @Openid
                ,@Nickname
                ,@Sex
                ,@City
                ,@Country
                ,@Province
                ,@Subscribetime
                );SELECT @@IDENTITY";
            SqlParameter[] p = 
            { 
                new SqlParameter("@Openid",entity.Openid),
                new SqlParameter("@Nickname",entity.Nickname),
                new SqlParameter("@Sex",entity.Sex),
                new SqlParameter("@City",entity.City),
                new SqlParameter("@Country",entity.Country),
                new SqlParameter("@Province",entity.Province),
                new SqlParameter("@Subscribetime",entity.Subscribetime)
            };
            entity.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return entity.ID > 0;
        }

        public override int WeixinDianzan(string openid, string vopenid)
        {
            string sql = @"
            IF EXISTS(SELECT * FROM HX_WeixinDianzan WHERE [Openid] = @Openid AND [Vopenid] = @Vopenid)
            BEGIN
                SELECT 1
            END
            ELSE
            BEGIN
                INSERT INTO HX_WeixinDianzan(
                    [Openid]
                    ,[Vopenid]
                )VALUES(
                    @Openid
                    ,@Vopenid
                );
                UPDATE HX_WeixinAct SET 
                    AtcValue = AtcValue + 1
                WHERE [Openid] = @Vopenid;
                SELECT 0
            END";
            SqlParameter[] p = 
            { 
                new SqlParameter("@Openid",openid),
                new SqlParameter("@Vopenid",vopenid)
            };
            return DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
        }

        #endregion

        #region 奔驰投票活动

        /// <summary>
        /// 添加/编辑参赛选手
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override bool AddBenzvotePothunterInfo(BenzvotePothunterInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_BenzvotePothunter WHERE @ID > 0)
            BEGIN
                UPDATE HX_BenzvotePothunter SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                WHERE ID = @ID
            END
            ELSE
            BEGIN
                INSERT INTO HX_BenzvotePothunter(
                    [PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
                new SqlParameter("@ID",entity.ID)
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public override void DelBenzvotePothunterInfo(string ids)
        {
            string sql = "DELETE FROM HX_BenzvotePothunter WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<BenzvotePothunterInfo> GetBenzvotePothunterList()
        {
            List<BenzvotePothunterInfo> list = new List<BenzvotePothunterInfo>();
            string sql = "SELECT * FROM HX_BenzvotePothunter";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateBenzvotePothunterInfo(reader));
                }
            }

            return list;
        }

        public override List<BenzvoteInfo> GetBenzvoteList(int pageindex, int pagesize, BenzvoteQuery query, ref int recordcount)
        {
            List<BenzvoteInfo> list = new List<BenzvoteInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateBenzvote(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override bool AddBenzvoteInfo(BenzvoteInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
                INSERT INTO HX_Benzvote(
                    [AthleteID]
                    ,[AthleteName]
                    ,[SerialNumber]
                    ,[Voter]
                    ,[AddTime]
                    ,[PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @AthleteID
                    ,@AthleteName
                    ,@SerialNumber
                    ,@Voter
                    ,@AddTime
                    ,@PropertyNames
                    ,@PropertyValues)
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@AthleteID",entity.AthleteID),
                new SqlParameter("@AthleteName",entity.AthleteName),
                new SqlParameter("@SerialNumber",entity.SerialNumber),
                new SqlParameter("@Voter",entity.Voter),
                new SqlParameter("@AddTime",entity.AddTime),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public override void AddBenzvoteSetting(BenzvoteSettingInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_BenzvoteSetting)
            BEGIN
                UPDATE HX_BenzvoteSetting SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
            END
            ELSE
            BEGIN
                INSERT INTO HX_BenzvoteSetting(
                    [PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override BenzvoteSettingInfo GetBenzvoteSetting()
        {
            string sql = "SELECT * FROM HX_BenzvoteSetting";
            BenzvoteSettingInfo entity = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                if (reader.Read())
                {
                    entity = PopulateBenzvoteSetting(reader);
                }
            }
            return entity;
        }

        #endregion

        #region 集团投票活动

        /// <summary>
        /// 添加/编辑参赛选手
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override bool AddJituanvotePothunterInfo(JituanvotePothunterInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_JituanvotePothunter WHERE @ID > 0)
            BEGIN
                UPDATE HX_JituanvotePothunter SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                WHERE ID = @ID
            END
            ELSE
            BEGIN
                INSERT INTO HX_JituanvotePothunter(
                    [PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
                new SqlParameter("@ID",entity.ID)
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public override void DelJituanvotePothunterInfo(string ids)
        {
            string sql = "DELETE FROM HX_JituanvotePothunter WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<JituanvotePothunterInfo> GetJituanvotePothunterList()
        {
            List<JituanvotePothunterInfo> list = new List<JituanvotePothunterInfo>();
            string sql = "SELECT * FROM HX_JituanvotePothunter";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateJituanvotePothunterInfo(reader));
                }
            }

            return list;
        }

        public override List<JituanvoteInfo> GetJituanvoteList(int pageindex, int pagesize, JituanvoteQuery query, ref int recordcount)
        {
            List<JituanvoteInfo> list = new List<JituanvoteInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateJituanvote(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override bool AddJituanvoteInfo(JituanvoteInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
                INSERT INTO HX_Jituanvote(
                    [AthleteID]
                    ,[AthleteName]
                    ,[SerialNumber]
                    ,[Voter]
                    ,[AddTime]
                    ,[PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @AthleteID
                    ,@AthleteName
                    ,@SerialNumber
                    ,@Voter
                    ,@AddTime
                    ,@PropertyNames
                    ,@PropertyValues)
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@AthleteID",entity.AthleteID),
                new SqlParameter("@AthleteName",entity.AthleteName),
                new SqlParameter("@SerialNumber",entity.SerialNumber),
                new SqlParameter("@Voter",entity.Voter),
                new SqlParameter("@AddTime",entity.AddTime),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public override void AddJituanvoteSetting(JituanvoteSettingInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_JituanvoteSetting)
            BEGIN
                UPDATE HX_JituanvoteSetting SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
            END
            ELSE
            BEGIN
                INSERT INTO HX_JituanvoteSetting(
                    [PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override JituanvoteSettingInfo GetJituanvoteSetting()
        {
            string sql = "SELECT * FROM HX_JituanvoteSetting";
            JituanvoteSettingInfo entity = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                if (reader.Read())
                {
                    entity = PopulateJituanvoteSetting(reader);
                }
            }
            return entity;
        }

        #endregion

        #region 评论管理

        public override int CreateAndUpdateComment(WeixinActCommentInfo entity)
        {
            string sql = @"
            IF @ID = 0
            BEGIN
                INSERT INTO HX_WeixinActComments(
                    [WeixinActType]
                    ,[AthleteID]
                    ,[Commenter]
                    ,[PraiseNum]
                    ,[BelittleNum]
                    ,[Comment]
                    ,[AddTime]) VALUES (
                    @WeixinActType
                    ,@AthleteID
                    ,@Commenter
                    ,@PraiseNum
                    ,@BelittleNum
                    ,@Comment
                    ,@AddTime)
                ;SELECT @@IDENTITY
            END
            ELSE 
            BEGIN
                UPDATE HX_WeixinActComments SET
                [PraiseNum] = @PraiseNum
                ,[BelittleNum] = @BelittleNum
                WHERE [ID] = @ID
                ;SELECT @ID
            END";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@WeixinActType",(byte)entity.WeixinActType),
                new SqlParameter("@AthleteID",entity.AthleteID),
                new SqlParameter("@Commenter",entity.Commenter),
                new SqlParameter("@PraiseNum",entity.PraiseNum),
                new SqlParameter("@BelittleNum",entity.BelittleNum),
                new SqlParameter("@Comment",entity.Comment),
                new SqlParameter("@AddTime",entity.AddTime)
            };
            entity.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return entity.ID;
        }

        public override List<WeixinActCommentInfo> GetWeixinActComments()
        {
            List<WeixinActCommentInfo> list = new List<WeixinActCommentInfo>();
            string sql = "SELECT * FROM HX_WeixinActComments";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateWeixinActCommentInfo(reader));
                }
            }

            return list;
        }

        #endregion

        #region 二手车评估器

        public override void AddEscpgInfo(EscpgInfo entity)
        {
            string sql = @"
                INSERT INTO HX_Escpg(
                    [Brand]
                    ,[Chexi]
                    ,[Nianfen]
                    ,[Kuanshi]
                    ,[Licheng]
                    ,[Phone]
                    ,[AddTime]
                    ,[Restore]
                )VALUES(
                    @Brand
                    ,@Chexi
                    ,@Nianfen
                    ,@Kuanshi
                    ,@Licheng
                    ,@Phone
                    ,@AddTime
                    ,@Restore)
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@Brand",entity.Brand),
                new SqlParameter("@Chexi",entity.Chexi),
                new SqlParameter("@Nianfen",entity.Nianfen),
                new SqlParameter("@Kuanshi",entity.Kuanshi),
                new SqlParameter("@Licheng",entity.Licheng),
                new SqlParameter("@Phone",entity.Phone),
                new SqlParameter("@AddTime",entity.AddTime),
                new SqlParameter("@Restore",entity.Restore)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);

        }

        public override List<EscpgInfo> GetEscpgList()
        {
            List<EscpgInfo> list = new List<EscpgInfo>();
            string sql = "SELECT * FROM HX_Escpg";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateEscpgInfo(reader));
                }
            }

            return list;
        }

        public override void UpdateEscpgRestore(string ids)
        {
            string sql = "UPDATE HX_Escpg SET [Restore] = 1 WHERE [ID] IN ('" + ids + "')";
            SqlHelper.ExecuteNonQuery(_con,CommandType.Text,sql);
        }

        #endregion

        #region 卡券活动

        #region 活动设置

        public override void AddCardSetting(CardSettingInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_CardSetting WHERE [ID] = @ID)
            BEGIN
                UPDATE HX_CardSetting SET
                    [Name] = @Name
                    ,[PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                WHERE [ID] = @ID
            END
            ELSE
            BEGIN
                INSERT INTO HX_CardSetting(
                    [Name]
                    ,[PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @Name
                    ,@PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Name",entity.Name),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteCardSetting(string ids)
        {
            string sql = "DELETE FROM HX_CardSetting WHERE [ID] IN(" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<CardSettingInfo> GetCardSettinglist()
        {
            string sql = "SELECT * FROM HX_CardSetting";
            List<CardSettingInfo> list = new List<CardSettingInfo>();
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add( PopulateCardSetting(reader));
                }
            }
            return list;
        }

        #endregion

        #region 卡券抽奖记录

        public override void AddCardPullRecord(CardPullRecordInfo entity)
        {
            string sql = @"
                INSERT INTO HX_CardPullRecords(
                    [SID]
                    ,[Openid]
                    ,[UserName]
                    ,[Cardid]
                    ,[Cardtitle]
                    ,[Cardawardname]
                    ,[Cardlogourl]
                    ,[PullResult]
                    ,[AddTime]
                )VALUES(
                    @SID
                    ,@Openid
                    ,@UserName
                    ,@Cardid
                    ,@Cardtitle
                    ,@Cardawardname
                    ,@Cardlogourl
                    ,@PullResult
                    ,@AddTime)
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@SID",entity.SID),
                new SqlParameter("@Openid",entity.Openid),
                new SqlParameter("@UserName",entity.UserName),
                new SqlParameter("@Cardid",entity.Cardid),
                new SqlParameter("@Cardtitle",entity.Cardtitle),
                new SqlParameter("@Cardawardname",entity.Cardawardname),
                new SqlParameter("@Cardlogourl",entity.Cardlogourl),
                new SqlParameter("@PullResult",entity.PullResult),
                new SqlParameter("@AddTime",entity.AddTime),
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void PullCard(string openid,int sid)
        {
            string sql = @"
                UPDATE HX_CardPullRecords SET
                [PullResult] = '2'
                WHERE [Openid] = @Openid AND [SID] = @SID";
            SqlParameter[] p = 
            {
                new SqlParameter("@Openid",openid),
                new SqlParameter("@SID",sid)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override List<CardPullRecordInfo> GetCardPullRecordList(int sid)
        {
            List<CardPullRecordInfo> list = new List<CardPullRecordInfo>();
            string sql = "SELECT * FROM HX_CardPullRecords WHERE [SID] = @SID";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql,new SqlParameter("@SID",sid)))
            {
                while (reader.Read())
                {
                    list.Add(PopulateCardPullRecord(reader));
                }
            }

            return list;
        }

        public override void ClearCardPullRecord(int sid)
        {
            string sql = "DELETE FROM HX_CardPullRecords WHERE [SID] = @SID";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql,new SqlParameter("@SID",sid));
        }

        public override void DeleteCardPullRecord(int id)
        {
            string sql = "DELETE FROM HX_CardPullRecords WHERE [ID] = @ID";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql,new SqlParameter("@ID",id));
        }

        public override List<CardidInfo> GetCardidInfolist(int sid)
        {
            List<CardidInfo> list = new List<CardidInfo>();
            string sql = "SELECT * FROM HX_CardidInfo WHERE [SID] = @SID";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@SID", sid)))
            {
                while (reader.Read())
                {
                    list.Add(PopulateCardidInfo(reader));
                }
            }

            return list;
        }

        public override void AddCardidInfo(CardidInfo entity)
        {
            string sql = @"
                INSERT INTO HX_CardidInfo(
                    [SID]
                    ,[Cardid]
                    ,[Cardtitle]
                    ,[Award]
                    ,[Num]
                    ,[ImgUrl]
                )VALUES(
                    @SID
                    ,@Cardid
                    ,@Cardtitle
                    ,@Award
                    ,@Num
                    ,@ImgUrl
                )
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@SID",entity.SID),
                new SqlParameter("@Cardid",entity.Cardid),
                new SqlParameter("@Cardtitle",entity.Cardtitle),
                new SqlParameter("@Award",entity.Award),
                new SqlParameter("@Num",entity.Num),
                new SqlParameter("@ImgUrl",entity.ImgUrl),
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteCardidInfo(string ids)
        {
            string sql = "DELETE FROM HX_CardidInfo WHERE [ID] IN (" + ids +")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override void UpdateCardidInfo(CardidInfo entity)
        {
            string sql = @"
                UPDATE HX_CardidInfo SET
                    [Cardid] = @Cardid
                    ,[Cardtitle] = @Cardtitle
                    ,[Award] = @Award
                    ,[Num] = @Num
                    ,[ImgUrl] = @ImgUrl
                WHERE [ID] = @ID
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Cardid",entity.Cardid),
                new SqlParameter("@Cardtitle",entity.Cardtitle),
                new SqlParameter("@Award",entity.Award),
                new SqlParameter("@Num",entity.Num),
                new SqlParameter("@ImgUrl",entity.ImgUrl),
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        #endregion

        #endregion

        #region 投票活动

        #region 活动设置

        public override void AddVoteSetting(VoteSettingInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_VoteSetting WHERE [ID] = @ID)
            BEGIN
                UPDATE HX_VoteSetting SET
                    [Name] = @Name
                    ,[PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                WHERE [ID] = @ID
            END
            ELSE
            BEGIN
                INSERT INTO HX_VoteSetting(
                    [Name]
                    ,[PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @Name
                    ,@PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Name",entity.Name),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteVoteSetting(string ids)
        {
            string sql = "DELETE FROM HX_VoteSetting WHERE [ID] IN(" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<VoteSettingInfo> GetVoteSettinglist()
        {
            string sql = "SELECT * FROM HX_VoteSetting";
            List<VoteSettingInfo> list = new List<VoteSettingInfo>();
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateVoteSetting(reader));
                }
            }
            return list;
        }

        #endregion

        #region 选手管理

        public override bool AddVotePothunterInfo(VotePothunterInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_VotePothunter WHERE @ID > 0)
            BEGIN
                UPDATE HX_VotePothunter SET
                    [PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                WHERE ID = @ID
            END
            ELSE
            BEGIN
                INSERT INTO HX_VotePothunter(
                    [PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
                new SqlParameter("@ID",entity.ID)
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public override void DelVotePothunterInfo(string ids)
        {
            string sql = "DELETE FROM HX_VotePothunter WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<VotePothunterInfo> GetVotePothunterList()
        {
            List<VotePothunterInfo> list = new List<VotePothunterInfo>();
            string sql = "SELECT * FROM HX_VotePothunter";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateVotePothunterInfo(reader));
                }
            }

            return list;
        }

        #endregion

        #region 投票记录

        public override List<VoteRecordInfo> GetVoteRecordList(int pageindex, int pagesize, VoteRecordQuery query, ref int recordcount)
        {
            List<VoteRecordInfo> list = new List<VoteRecordInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateVoteRecord(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override bool AddVoteRecordInfo(VoteRecordInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
                INSERT INTO HX_VoteRecord(
                    [SID]
                    ,[AthleteID]
                    ,[AthleteName]
                    ,[SerialNumber]
                    ,[Voter]
                    ,[AddTime]
                    ,[PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @SID
                    ,@AthleteID
                    ,@AthleteName
                    ,@SerialNumber
                    ,@Voter
                    ,@AddTime
                    ,@PropertyNames
                    ,@PropertyValues)
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@SID",entity.SID),
                new SqlParameter("@AthleteID",entity.AthleteID),
                new SqlParameter("@AthleteName",entity.AthleteName),
                new SqlParameter("@SerialNumber",entity.SerialNumber),
                new SqlParameter("@Voter",entity.Voter),
                new SqlParameter("@AddTime",entity.AddTime),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 评论管理

        public override int CreateAndUpdateVoteComment(VoteCommentInfo entity)
        {
            string sql = @"
            IF @ID = 0
            BEGIN
                INSERT INTO HX_VoteComments(
                    [AthleteID]
                    ,[Commenter]
                    ,[PraiseNum]
                    ,[BelittleNum]
                    ,[Comment]
                    ,[AddTime]
                    ,[CheckStatus]) VALUES (
                    @AthleteID
                    ,@Commenter
                    ,@PraiseNum
                    ,@BelittleNum
                    ,@Comment
                    ,@AddTime
                    ,@CheckStatus)
                ;SELECT @@IDENTITY
            END
            ELSE 
            BEGIN
                UPDATE HX_VoteComments SET
                [PraiseNum] = @PraiseNum
                ,[BelittleNum] = @BelittleNum
                WHERE [ID] = @ID
                ;SELECT @ID
            END";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@AthleteID",entity.AthleteID),
                new SqlParameter("@Commenter",entity.Commenter),
                new SqlParameter("@PraiseNum",entity.PraiseNum),
                new SqlParameter("@BelittleNum",entity.BelittleNum),
                new SqlParameter("@Comment",entity.Comment),
                new SqlParameter("@AddTime",entity.AddTime),
                new SqlParameter("@CheckStatus",entity.CheckStatus)
            };
            entity.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return entity.ID;
        }

        public override void DelVoteCommentInfo(int id)
        {
            string sql = "DELETE FROM HX_VoteComments WHERE [ID] = @ID";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, new SqlParameter("@ID", id));
        }

        public override void CheckVoteCommentStatus(string ids)
        {
            string sql = "UPDATE HX_VoteComments SET [CheckStatus] = 1 WHERE [ID] IN(" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<VoteCommentInfo> GetVoteComments(int aid)
        {
            List<VoteCommentInfo> list = new List<VoteCommentInfo>();
            string sql = "SELECT * FROM HX_VoteComments WHERE [AthleteID]=@AthleteID";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@AthleteID", aid)))
            {
                while (reader.Read())
                {
                    list.Add(PopulateVoteCommentInfo(reader));
                }
            }

            return list;
        }

        #endregion

        #endregion

        #region 广本61活动

        public override void AddGB61Info(GB61Info entity)
        {
            string sql = @"
            IF NOT EXISTS(SELECT * FROM HX_GB61 WHERE [Phone] = @Phone)
            BEGIN
                INSERT INTO HX_GB61(
                    [CName]
                    ,[Phone]
                    ,[SpecName]
                    ,[Status]
                )VALUES(
                    @CName
                    ,@Phone
                    ,@SpecName
                    ,@Status)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@CName",entity.CName),
                new SqlParameter("@Phone",entity.Phone),
                new SqlParameter("@SpecName",entity.SpecName),
                new SqlParameter("@Status",entity.Status)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override List<GB61Info> GetGB61InfoList()
        {
            List<GB61Info> list = new List<GB61Info>();
            string sql = "SELECT * FROM HX_GB61";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateGB61Info(reader));
                }
            }

            return list;
        }

        #endregion

        #region 场景二维码

        #region 活动设置

        public override void AddScenecodeSetting(ScenecodeSettingInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF EXISTS(SELECT * FROM HX_ScenecodeSetting WHERE [ID] = @ID)
            BEGIN
                UPDATE HX_ScenecodeSetting SET
                    [Name] = @Name
                    ,[PropertyNames] = @PropertyNames
                    ,[PropertyValues] = @PropertyValues
                WHERE [ID] = @ID
            END
            ELSE
            BEGIN
                INSERT INTO HX_ScenecodeSetting(
                    [Name]
                    ,[PropertyNames]
                    ,[PropertyValues]
                )VALUES(
                    @Name
                    ,@PropertyNames
                    ,@PropertyValues)
            END
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Name",entity.Name),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteScenecodeSetting(string ids)
        {
            string sql = "DELETE FROM HX_ScenecodeSetting WHERE [ID] IN(" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<ScenecodeSettingInfo> GetScenecodeSettinglist()
        {
            string sql = "SELECT * FROM HX_ScenecodeSetting";
            List<ScenecodeSettingInfo> list = new List<ScenecodeSettingInfo>();
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateScenecodeSetting(reader));
                }
            }
            return list;
        }

        #endregion

        #region 场景管理


        public override List<ScenecodeInfo> GetScenecodeList(int sid)
        {
            List<ScenecodeInfo> list = new List<ScenecodeInfo>();
            string sql = "SELECT * FROM HX_ScenecodeInfo WHERE [SID] = @SID";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@SID", sid)))
            {
                while (reader.Read())
                {
                    list.Add(PopulateScenecodeInfo(reader));
                }
            }

            return list;
        }

        public override void AddScenecodeInfo(ScenecodeInfo entity)
        {
            string sql = @"
                INSERT INTO HX_ScenecodeInfo(
                    [SID]
                    ,[SceneName]
                    ,[ScanNum]
                    ,[RedirectAddress]
                )VALUES(
                    @SID
                    ,@SceneName
                    ,@ScanNum
                    ,@RedirectAddress
                )
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@SID",entity.SID),
                new SqlParameter("@SceneName",entity.SceneName),
                new SqlParameter("@ScanNum",entity.ScanNum),
                new SqlParameter("@RedirectAddress",entity.RedirectAddress),
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteScenecodeInfo(string ids)
        {
            string sql = "DELETE FROM HX_ScenecodeInfo WHERE [ID] IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override void UpdateScenecodeInfo(ScenecodeInfo entity)
        {
            string sql = @"
                UPDATE HX_ScenecodeInfo SET
                    [SceneName] = @SceneName
                    ,[RedirectAddress] = @RedirectAddress
                WHERE [ID] = @ID
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@SceneName",entity.SceneName),
                new SqlParameter("@RedirectAddress",entity.RedirectAddress),
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void AddScenecodeNum(int id)
        {
            string sql = "UPDATE HX_ScenecodeInfo SET [ScanNum] = ScanNum + 1 WHERE ID=@ID";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql,new SqlParameter("@ID",id));
        }

        #endregion

        #endregion

        #endregion

        #region 招聘管理

        public override List<JobOfferInfo> GetJobOfferList()
        {
            List<JobOfferInfo> list = new List<JobOfferInfo>();
            string sql = "SELECT * FROM HX_JobOffer";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateJobOffer(reader));
                }
            }

            return list;
        }

        public override void AddJobOffer(JobOfferInfo entity)
        {
            string sql = @"INSERT INTO HX_JobOffer(
            [Title]
            ,[Content]
            ,[PropertyNames]
            ,[PropertyValues]
            )VALUES(
            @Title
            ,@Content
            ,@PropertyNames
            ,@PropertyValues
            )";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@Title",entity.Title),
                new SqlParameter("@Content",entity.Content),
				new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void UpdateJobOffer(JobOfferInfo entity)
        {
            string sql = @"UPDATE HX_JobOffer SET 
            [Title] = @Title 
            ,[Content] = @Content 
            ,PropertyNames = @PropertyNames
            ,PropertyValues = @PropertyValues
            WHERE [ID] = @ID";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Title",entity.Title),
                new SqlParameter("@Content",entity.Content),
				new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteJobOffer(string ids)
        {
            string sql = "DELETE FROM HX_JobOffer WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        #endregion

        #region 企业风采

        public override List<CorpMienInfo> GetCorpMienList()
        {
            List<CorpMienInfo> list = new List<CorpMienInfo>();
            string sql = "SELECT * FROM HX_CorpMien";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateCorpMien(reader));
                }
            }

            return list;
        }

        public override void AddCorpMien(CorpMienInfo entity)
        {
            string sql = @"
            DECLARE @RecordCount INT
            SELECT @RecordCount = COUNT(0) FROM HX_CorpMien
            INSERT INTO HX_CorpMien(
                [Pic]
                ,[Introduce]
                ,[Content]
                ,[OrderIndex]
            )VALUES(
                @Pic
                ,@Introduce
                ,@Content
                ,@RecordCount + 1
            )";
            SqlParameter[] p = 
            {
                new SqlParameter("@Pic",entity.Pic),
                new SqlParameter("@Introduce",entity.Introduce),
				new SqlParameter("@Content", entity.Content)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void UpdateCorpMien(CorpMienInfo entity)
        {
            string sql = @"UPDATE HX_CorpMien SET 
            [Pic] = @Pic 
            ,[Introduce] = @Introduce 
            ,[Content] = @Content
            WHERE [ID] = @ID";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@Pic",entity.Pic),
                new SqlParameter("@Introduce",entity.Introduce),
                new SqlParameter("@Content",entity.Content)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void DeleteCorpMien(string ids)
        {
            string sql = "DELETE FROM HX_CorpMien WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);

            sql = @"UPDATE dbo.HX_CorpMien SET
            [OrderIndex] = T.[OrderIndex]
            FROM dbo.HX_CorpMien S,(
            SELECT (SELECT COUNT(0) FROM dbo.HX_CorpMien t2 WHERE t1.[ID] >= t2.[ID]) OrderIndex, t1.[ID] 
            FROM dbo.HX_CorpMien t1
            ) T
            WHERE S.[ID] = T.[ID]";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override void MoveCorpMien(int id, int toindex)
        {
            string sql = @"UPDATE dbo.HX_CorpMien SET
            [OrderIndex] = (SELECT [OrderIndex] FROM dbo.HX_CorpMien WHERE [ID] = @ID)
            WHERE [OrderIndex] = @OrderIndex;
            UPDATE dbo.HX_CorpMien SET
            [OrderIndex] = @OrderIndex 
            WHERE [ID] = @ID";
            SqlParameter[] p = new SqlParameter[] 
            { 
                new SqlParameter("@ID",id),
                new SqlParameter("@OrderIndex",toindex)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        #endregion

        #region 地区管理

        public override List<PromaryInfo> GetPromaryList()
        {
            List<PromaryInfo> list = new List<PromaryInfo>();
            string sql = "SELECT * FROM HX_Promary";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulatePromary(reader));
                }
            }

            return list;
        }

        public override List<CityInfo> GetCityList()
        {
            List<CityInfo> list = new List<CityInfo>();
            string sql = "SELECT * FROM HX_City";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateCity(reader));
                }
            }

            return list;
        }

        #endregion

        #region Jcb用户管理

        /// <summary>
        ///  获取用于加密的值
        /// </summary>
        /// <param name="userID">管理员ID</param>
        /// <returns>用于加密的值</returns>
        public override string GetJcbUserKey(int userID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 CheckKey from HX_JcbUser ");
            strSql.Append(" where ID=@ID ");
            object o = SqlHelper.ExecuteScalar(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", userID));
            return o as string;
        }

        /// <summary>
        /// 管理员是否已经存在
        /// </summary>
        /// <param name="name">管理员ID</param>
        /// <returns>管理员是否存在</returns>
        public override bool ExistsJcbUser(int id)
        {
            string sql = "select count(1) from HX_JcbUser where ID=@ID";
            int i = Convert.ToInt32(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, new SqlParameter("@ID", id)));
            if (i > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 通过用户名获得后台管理员信息
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns>管理员实体信息</returns>
        public override JcbUserInfo GetJcbUserByName(string UserName)
        {
            string sql = "select * from HX_JcbUser where UserName=@UserName";
            JcbUserInfo admin = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, new SqlParameter("@UserName", UserName)))
            {
                if (reader.Read())
                {
                    admin = PopulateJcbUser(reader);
                }
            }
            return admin;
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>添加成功返回ID</returns>
        public override int AddJcbUser(JcbUserInfo model)
        {
            SerializerData data = model.GetSerializerData();
            string sql = @"
            INSERT INTO HX_JcbUser(UserName,Password,Administrator,LastLoginIP,LastLoginTime,[PropertyNames],[PropertyValues])
            VALUES (@UserName,@Password,@Administrator,@LastLoginIP,@LastLoginTime,@PropertyNames,@PropertyValues)
            ;SELECT @@IDENTITY";
            SqlParameter[] p = 
            {
                new SqlParameter("@UserName",model.UserName),
                new SqlParameter("@Password",model.Password),
                new SqlParameter("@Administrator",model.Administrator),
                new SqlParameter("@LastLoginIP",model.LastLoginIP),
                new SqlParameter("@LastLoginTime",model.LastLoginTime),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            model.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return model.ID;
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>修改是否成功</returns>
        public override bool UpdateJcbUser(JcbUserInfo model)
        {
            SerializerData data = model.GetSerializerData();
            string sql = @"UPDATE HX_JcbUser SET
            UserName = @UserName
            ,Password = @Password
            ,Administrator = @Administrator
            ,LastLoginIP = @LastLoginIP
            ,LastLoginTime = @LastLoginTime
            ,[PropertyNames] = @PropertyNames
            ,[PropertyValues] = @PropertyValues
            WHERE ID = @ID
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@UserName",model.UserName),
                new SqlParameter("@Password",model.Password),
                new SqlParameter("@Administrator",model.Administrator),
                new SqlParameter("@LastLoginIP",model.LastLoginIP),
                new SqlParameter("@LastLoginTime",model.LastLoginTime),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values),
                new SqlParameter("@ID",model.ID)
            };
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="AID">管理员ID</param>
        /// <returns>删除是否成功</returns>
        public override bool DeleteJcbUser(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from HX_JcbUser ");
            strSql.Append(" where ID=@ID ");
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", id));
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 通过ID获取管理员
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>管理员实体信息</returns>
        public override JcbUserInfo GetJcbUser(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from HX_JcbUser ");
            strSql.Append(" where ID=@ID ");
            JcbUserInfo admin = null;
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", id)))
            {
                if (reader.Read())
                {
                    admin = PopulateJcbUser(reader);
                }
            }
            return admin;
        }

        /// <summary>
        /// 验证用户登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户ID</returns>
        public override int ValiJcbUser(string userName, string password)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID from HX_JcbUser");
            strSql.Append(" where UserName=@UserName and Password=@PassWord");

            object obj = SqlHelper.ExecuteScalar(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@UserName", userName), new SqlParameter("@PassWord", password));
            if (obj == null)
            {
                return -2;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 返回所有用户
        /// </summary>
        /// <returns>返回所有用户</returns>
        public override List<JcbUserInfo> GetAllJcbUsers()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from HX_JcbUser");


            List<JcbUserInfo> admins = new List<JcbUserInfo>();
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    admins.Add(PopulateJcbUser(reader));
                }
            }
            return admins;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userID">管理员ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>修改密码是否成功</returns>
        public override bool ChangeJcbUserPw(int userID, string oldPassword, string newPassword)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update HX_JcbUser set ");
            strSql.Append("Password=@NewPassword");
            strSql.Append(" where ID=@ID and Password=@Password ");
            int result = SqlHelper.ExecuteNonQuery(_con, CommandType.Text, strSql.ToString(), new SqlParameter("@ID", userID), new SqlParameter("@Password", oldPassword), new SqlParameter("@NewPassword", newPassword));
            if (result < 1)
                return false;
            return true;
        }

        #endregion

        #region 调查问卷（试用版）

        public override int AddQuestionRecordInfo(QuestionRecordInfo entity)
        {
            string sql = @"
            INSERT INTO HX_QuestionRecord(PostUser,PostUserName,PostTime,QuestionType,QuestionCompanyID,[QuestionScoreInfoListJson])
            VALUES (@PostUser,@PostUserName,@PostTime,@QuestionType,@QuestionCompanyID,@QuestionScoreInfoListJson)
            ;SELECT @@IDENTITY";
            SqlParameter[] p = 
            {
                new SqlParameter("@PostUser",entity.PostUser),
                new SqlParameter("@PostUserName",entity.PostUserName),
                new SqlParameter("@PostTime",entity.PostTime),
                new SqlParameter("@QuestionType",(int)entity.QuestionType),
                new SqlParameter("@QuestionCompanyID",entity.QuestionCompanyID),
                new SqlParameter("@QuestionScoreInfoListJson",entity.QuestionScoreInfoListJson)
            };
            entity.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return entity.ID;
        }

        public override bool CheckQuestionPostUser(string postuser)
        {
            string sql = @"
            SELECT COUNT(0) FROM HX_QuestionRecord WHERE [PostUser] = @PostUser
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@PostUser",postuser)
            };
            return DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p)) == 0;
        }

        public override List<QuestionRecordInfo> GetQuestionRecordList()
        {
            List<QuestionRecordInfo> list = new List<QuestionRecordInfo>();
            string sql = "SELECT * FROM HX_QuestionRecord";
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateQuestionRecordInfo(reader));
                }
            }

            return list;
        }

        #endregion
    }
}
