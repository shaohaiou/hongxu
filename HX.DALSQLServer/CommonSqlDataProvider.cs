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
    }
}
