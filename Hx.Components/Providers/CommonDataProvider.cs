using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Config;
using System.Web;
using System.IO;
using Hx.Components.Entity;
using Hx.Components.Query;
using System.Data;
using Hx.Tools;
using Hx.Components.Enumerations;

namespace Hx.Components.Providers
{
    public abstract class CommonDataProvider
    {
        private static CommonDataProvider _defaultprovider = null;
        private static object _lock = new object();

        #region 初始化
        /// <summary>
        /// 返回默认的数据提供者类
        /// </summary>
        /// <returns></returns>
        public static CommonDataProvider Instance()
        {
            return Instance("MSSQLCommonDataProvider");
        }

        /// <summary>
        /// 从配置文件加载数据库访问提供者类
        /// </summary>
        /// <param name="providerName">提供者名</param>
        /// <returns>漫画提供者</returns>
        public static CommonDataProvider Instance(string providerName)
        {
            string cachekey = GlobalKey.PROVIDER + "_" + providerName;
            CommonDataProvider objType = MangaCache.GetLocal(cachekey) as CommonDataProvider;//从缓存读取
            if (objType == null)
            {
                CommConfig config = CommConfig.GetConfig();
                Provider dataProvider = (Provider)config.Providers[providerName];
                objType = DataProvider.Instance(dataProvider) as CommonDataProvider;
                string path = null;
                HttpContext context = HttpContext.Current;
                if (context != null)
                    path = context.Server.MapPath("~/config/common.config");
                else
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"config\common.config");
                MangaCache.MaxLocalWithFile(cachekey, objType, path);
            }
            return objType;
        }

        /// <summary>
        ///从配置文件加载默认数据库访问提供者类
        /// </summary>
        private static void LoadDefaultProviders()
        {
            if (_defaultprovider == null)
            {
                lock (_lock)
                {
                    if (_defaultprovider == null)
                    {
                        CommConfig config = CommConfig.GetConfig();
                        Provider dataProvider = (Provider)config.Providers[GlobalKey.DEFAULT_PROVDIER_COMMON];
                        _defaultprovider = DataProvider.Instance(dataProvider) as CommonDataProvider;

                    }
                }
            }
        }

        #endregion

        #region 后台管理员

        /// <summary>
        /// 通过管理员名获取管理员
        /// </summary>
        /// <param name="name">管理员名</param>
        /// <returns></returns>
        public abstract AdminInfo GetAdminByName(string id);

        /// <summary>
        /// 管理员是否已经存在
        /// </summary>
        /// <param name="name">管理员ID</param>
        /// <returns></returns>
        public abstract bool ExistsAdmin(int id);

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>添加成功返回ID</returns>
        public abstract int AddAdmin(AdminInfo model);

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="model">后台用户实体类</param>
        /// <returns>修改是否成功</returns>
        public abstract bool UpdateAdmin(AdminInfo model);

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="AID"></param>
        /// <returns></returns>
        public abstract bool DeleteAdmin(int AID);

        /// <summary>
        /// 通过ID获取管理员
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>管理员实体信息</returns>
        public abstract AdminInfo GetAdmin(int id);

        /// <summary>
        /// 验证用户登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户ID</returns>
        public abstract int ValiAdmin(string userName, string password);

        /// <summary>
        /// 返回所有用户
        /// </summary>
        /// <returns></returns>
        public abstract List<AdminInfo> GetAllAdmins();

        /// <summary>
        /// 获取普通用户
        /// </summary>
        /// <returns></returns>
        public abstract List<AdminInfo> GetUsers();

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userID">管理员ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public abstract bool ChangeAdminPw(int userID, string oldPassword, string newPassword);

        /// <summary>
        /// 获取用于加密的值
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public abstract string GetAdminKey(int userID);

        /// <summary>
        /// 填充后台用户实体类
        /// </summary>
        /// <param name="reader">记录集</param>
        /// <returns>实体类</returns>
        protected AdminInfo PopulateAdmin(IDataReader reader)
        {
            AdminInfo admin = new AdminInfo();
            admin.ID = (int)reader["ID"];
            admin.Administrator = DataConvert.SafeBool(reader["Administrator"]);
            admin.LastLoginIP = reader["LastLoginIP"] as string;
            admin.LastLoginTime = reader["LastLoginTime"] as DateTime?;
            admin.Password = reader["Password"] as string;
            admin.UserName = reader["UserName"] as string;
            admin.UserRole = (UserRoleType)(byte)reader["UserRole"];

            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            admin.SetSerializerData(data);

            return admin;
        }

        #endregion

        #region 日志

        public abstract void WriteEventLogEntry(EventLogEntry log);

        public abstract void ClearEventLog(DateTime dt);

        public abstract List<EventLogEntry> GetEventLogs(int pageindex, int pagesize, EventLogQuery query, out int total);


        /// <summary>
        /// 填充日志信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected EventLogEntry PopulateEventLogEntry(IDataReader reader)
        {
            EventLogEntry eventlog = new EventLogEntry();
            eventlog.EntryID = DataConvert.SafeInt(reader["ID"]);
            eventlog.EventID = DataConvert.SafeInt(reader["EventID"]);
            eventlog.EventType = (EventType)(byte)(reader["EventType"]);
            eventlog.Message = reader["Message"] as string;
            eventlog.Category = reader["Category"] as string;
            eventlog.MachineName = reader["MachineName"] as string;
            eventlog.ApplicationName = reader["ApplicationName"] as string;
            eventlog.PCount = DataConvert.SafeInt(reader["PCount"]);
            eventlog.AddTime = DataConvert.SafeDate(reader["AddTime"]);
            eventlog.LastUpdateTime = reader["LastUpdateTime"] as DateTime?;
            eventlog.ApplicationType = (ApplicationType)(byte)(reader["AppType"]);
            eventlog.EntryID = DataConvert.SafeInt(reader["EntryID"]);
            return eventlog;
        }


        #endregion

        #region 银行

        public abstract List<BankInfo> GetBankList();

        public abstract void AddBank(BankInfo entity);

        public abstract void UpdateBank(BankInfo entity);

        public abstract void DeleteBank(string ids);

        protected BankInfo PopulateBank(IDataReader reader)
        {
            BankInfo entity = new BankInfo 
            { 
                ID = DataConvert.SafeInt(reader["ID"]),
                Name = reader["Name"] as string                
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 系统全局设置

        public abstract GlobalSettingInfo GetGlobalSetting();

        public abstract void AddGlobalSetting(GlobalSettingInfo entity);

        protected GlobalSettingInfo PopulateGlobalSettingInfo(IDataReader reader)
        {
            GlobalSettingInfo entity = new GlobalSettingInfo();
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 日报用户

        public abstract List<DayReportUserInfo> GetDayReportUserList();

        public abstract void AddDayReportUser(DayReportUserInfo entity);

        public abstract void UpdateDayReportUser(DayReportUserInfo entity);

        public abstract void DeleteDayReportUser(string ids);

        protected DayReportUserInfo PopulateDayReportUser(IDataReader reader)
        {
            DayReportUserInfo entity = new DayReportUserInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                UserTag = reader["UserTag"] as string,
                UserName = reader["UserName"] as string,
                DayReportDep = (DayReportDep)(byte)reader["DayReportDep"],
                CorporationID = DataConvert.SafeInt(reader["CorporationID"]),
                CorporationName = reader["CorporationName"] as string
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 日报

        public abstract List<DailyReportInfo> GetDailyReportList(DailyReportQuery query);

        public abstract void CreateAndUpdateDailyReport( string tablename,DailyReportInfo entity);

        protected DailyReportInfo PopulateDailyReport(IDataReader reader)
        {
            DailyReportInfo entity = new DailyReportInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                DayUnique = reader["DayUnique"] as string,
                CorporationID = DataConvert.SafeInt(reader["CorporationID"]),
                Creator = reader["Creator"] as string,
                CreateTime = DataConvert.SafeDate(reader["CreateTime"]),
                LastUpdateUser = reader["LastUpdateUser"] as string,
                LastUpdateTime = DataConvert.SafeDate(reader["LastUpdateTime"])
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 日报录入记录

        public abstract void CreateDailyReportHistory(DailyReportHistoryInfo entity);

        public abstract List<DailyReportHistoryInfo> GetDailyReportHistoryList(int pageindex, int pagesize, DailyReportHistoryQuery query, ref int recordcount);

        protected DailyReportHistoryInfo PopulateDailyReportHistory(IDataReader reader)
        {
            DailyReportHistoryInfo entity = new DailyReportHistoryInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                DayUnique = reader["DayUnique"] as string,
                CreatorCorporationID = DataConvert.SafeInt(reader["CreatorCorporationID"]),
                CreatorCorporationName = reader["CreatorCorporationName"] as string,
                CreatorDepartment = (DayReportDep)(byte)reader["CreatorDepartment"],
                Creator = reader["Creator"] as string,
                CreateTime = DataConvert.SafeDate(reader["CreateTime"]),
                ReportDepartment = (DayReportDep)(byte)reader["ReportDepartment"],
                ReportCorporationID = DataConvert.SafeInt(reader["ReportCorporationID"])
            };

            DailyReportInfo mondify = new DailyReportInfo();
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            mondify.SetSerializerData(data);
            entity.Modify = mondify;

            return entity;
        }


        #endregion

        #region 日报模块

        public abstract List<DailyReportModuleInfo> GetDailyReportModuleList();

        public abstract void AddDailyReportModule(DailyReportModuleInfo entity);

        public abstract void UpdateDailyReportModule(DailyReportModuleInfo entity);

        public abstract void DeleteDailyReportModule(string ids);

        protected DailyReportModuleInfo PopulateDailyReportModule(IDataReader reader)
        {
            DailyReportModuleInfo entity = new DailyReportModuleInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                Sort = DataConvert.SafeInt(reader["Sort"]),
                Name = reader["Name"] as string,
                Department = (DayReportDep)(byte)reader["Department"],
                Ismonthlytarget = (bool)reader["Ismonthlytarget"],
                Description = reader["Description"] as string,
                Mustinput = (bool)reader["Mustinput"],
                Iscount = (bool)reader["Iscount"]
            };

            return entity;
        }

        #endregion

        #region crm报表

        public abstract void CreateAndUpdateCRMReport(CRMReportInfo entity);

        public abstract List<CRMReportInfo> GetCRMReportList(CRMReportQuery query);

        public abstract void DeleteCRMReport(string ids);

        protected CRMReportInfo PopulateCRMReport(IDataReader reader)
        {
            CRMReportInfo entity = new CRMReportInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                CorporationID = DataConvert.SafeInt(reader["CorporationID"]),
                Creator = reader["Creator"] as string,
                CreateTime = DataConvert.SafeDate(reader["CreateTime"]),
                LastUpdateUser = reader["LastUpdateUser"] as string,
                LastUpdateTime = DataConvert.SafeDate(reader["LastUpdateTime"]),
                Date = DataConvert.SafeDate(reader["Date"]),
                CRMReportType = (CRMReportType)DataConvert.SafeInt(reader["CRMReportType"])
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 月度目标

        public abstract List<MonthlyTargetInfo> GetMonthTargetList(MonthTargetQuery query);

        public abstract void CreateAndUpdateMonthlyTarget(MonthlyTargetInfo entity);

        protected MonthlyTargetInfo PopulateMonthTarget(IDataReader reader)
        {
            MonthlyTargetInfo entity = new MonthlyTargetInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                MonthUnique = reader["MonthUnique"] as string,
                CorporationID = DataConvert.SafeInt(reader["CorporationID"]),
                Department = (DayReportDep)(byte)reader["Department"],
                Creator = reader["Creator"] as string,
                CreateTime = DataConvert.SafeDate(reader["CreateTime"]),
                LastUpdateUser = reader["LastUpdateUser"] as string,
                LastUpdateTime = DataConvert.SafeDate(reader["LastUpdateTime"])
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 微信活动

        #region 测试活动

        public abstract WeixinActInfo GetWeixinActInfo(string openid);

        public abstract bool AddWeixinAct(WeixinActInfo entity);

        public abstract int WeixinDianzan(string openid, string vopenid);

        public abstract List<WeixinActInfo> GetWeixinActList(int pageindex, int pagesize, WeixinActQuery query, ref int recordcount);

        protected WeixinActInfo PopulateWeixinAct(IDataReader reader)
        {
            WeixinActInfo entity = new WeixinActInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                Openid = reader["Openid"] as string,
                Nickname = reader["Nickname"] as string,
                Sex = (byte)reader["Sex"],
                City = reader["City"] as string,
                Country = reader["Country"] as string,
                Province = reader["Province"] as string,
                Subscribetime = reader["Subscribetime"] as string,
                AtcValue = DataConvert.SafeInt(reader["AtcValue"]),
                AddTime = DataConvert.SafeDate(reader["AddTime"])
            };

            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 奔驰投票活动
        
        public abstract bool AddBenzvotePothunterInfo(BenzvotePothunterInfo entity);

        public abstract void DelBenzvotePothunterInfo(string ids);

        public abstract List<BenzvotePothunterInfo> GetBenzvotePothunterList();

        protected BenzvotePothunterInfo PopulateBenzvotePothunterInfo(IDataReader reader)
        {
            BenzvotePothunterInfo entity = new BenzvotePothunterInfo
            {
                ID = DataConvert.SafeInt(reader["ID"])
            };

            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        public abstract List<BenzvoteInfo> GetBenzvoteList(int pageindex, int pagesize, BenzvoteQuery query, ref int recordcount);

        public abstract bool AddBenzvoteInfo(BenzvoteInfo entity);

        protected BenzvoteInfo PopulateBenzvote(IDataReader reader)
        {
            BenzvoteInfo entity = new BenzvoteInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                AthleteID = DataConvert.SafeInt(reader["AthleteID"]),
                AthleteName = reader["AthleteName"] as string,
                SerialNumber = DataConvert.SafeInt(reader["SerialNumber"]),
                Voter = reader["Voter"] as string,
                AddTime = DataConvert.SafeDate(reader["AddTime"])
            };

            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        public abstract void AddBenzvoteSetting(BenzvoteSettingInfo entity);

        public abstract BenzvoteSettingInfo GetBenzvoteSetting();

        protected BenzvoteSettingInfo PopulateBenzvoteSetting(IDataReader reader)
        {
            BenzvoteSettingInfo entity = new BenzvoteSettingInfo ();
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 集团投票活动

        public abstract bool AddJituanvotePothunterInfo(JituanvotePothunterInfo entity);

        public abstract void DelJituanvotePothunterInfo(string ids);

        public abstract List<JituanvotePothunterInfo> GetJituanvotePothunterList();

        protected JituanvotePothunterInfo PopulateJituanvotePothunterInfo(IDataReader reader)
        {
            JituanvotePothunterInfo entity = new JituanvotePothunterInfo
            {
                ID = DataConvert.SafeInt(reader["ID"])
            };

            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        public abstract List<JituanvoteInfo> GetJituanvoteList(int pageindex, int pagesize, JituanvoteQuery query, ref int recordcount);

        public abstract bool AddJituanvoteInfo(JituanvoteInfo entity);

        protected JituanvoteInfo PopulateJituanvote(IDataReader reader)
        {
            JituanvoteInfo entity = new JituanvoteInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                AthleteID = DataConvert.SafeInt(reader["AthleteID"]),
                AthleteName = reader["AthleteName"] as string,
                SerialNumber = DataConvert.SafeInt(reader["SerialNumber"]),
                Voter = reader["Voter"] as string,
                AddTime = DataConvert.SafeDate(reader["AddTime"])
            };

            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        public abstract void AddJituanvoteSetting(JituanvoteSettingInfo entity);

        public abstract JituanvoteSettingInfo GetJituanvoteSetting();

        protected JituanvoteSettingInfo PopulateJituanvoteSetting(IDataReader reader)
        {
            JituanvoteSettingInfo entity = new JituanvoteSettingInfo();
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #endregion

        #region 招聘管理

        public abstract List<JobOfferInfo> GetJobOfferList();

        public abstract void AddJobOffer(JobOfferInfo entity);

        public abstract void UpdateJobOffer(JobOfferInfo entity);

        public abstract void DeleteJobOffer(string ids);

        protected JobOfferInfo PopulateJobOffer(IDataReader reader)
        {
            JobOfferInfo entity = new JobOfferInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                Title = reader["Title"] as string,
                Content = reader["Content"] as string
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 地区管理

        public abstract List<PromaryInfo> GetPromaryList();

        public abstract List<CityInfo> GetCityList();

        protected PromaryInfo PopulatePromary(IDataReader reader)
        {
            PromaryInfo entity = new PromaryInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                Name = reader["Name"] as string
            };

            return entity;
        }

        protected CityInfo PopulateCity(IDataReader reader)
        {
            CityInfo entity = new CityInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                Name = reader["Name"] as string,
                PID = DataConvert.SafeInt(reader["PID"])
            };

            return entity;
        }

        #endregion

        #region MyRegion
        
        #endregion
    }
}
