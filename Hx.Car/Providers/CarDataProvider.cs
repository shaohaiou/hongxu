using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Config;
using Hx.Components;
using System.IO;
using System.Web;
using System.Data;
using Hx.Car.Entity;
using Hx.Car.Query;
using Hx.Tools;
using Hx.Car.Enum;

namespace Hx.Car.Providers
{
    public abstract class CarDataProvider
    {
        private static System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        private static CarDataProvider _defaultprovider = null;
        private static object _lock = new object();

        #region 初始化

        /// <summary>
        /// 返回默认的数据提供者类
        /// </summary>
        /// <returns></returns>
        public static CarDataProvider Instance()
        {
            //LoadDefaultProviders();
            //return _defaultprovider;

            #region 根据模板语言获取配置信息
            return Instance("MSSQLCarDataProvider");
            #endregion
        }

        /// <summary>
        /// 从配置文件加载数据库访问提供者类
        /// </summary>
        /// <param name="providerName">提供者名</param>
        /// <returns>漫画提供者</returns>
        public static CarDataProvider Instance(string providerName)
        {
            string cachekey = GlobalKey.PROVIDER + "_" + providerName;
            CarDataProvider objType = MangaCache.GetLocal(cachekey) as CarDataProvider;//从缓存读取
            if (objType == null)
            {
                CommConfig config = CommConfig.GetConfig();
                Provider dataProvider = (Provider)config.Providers[providerName];
                objType = DataProvider.Instance(dataProvider) as CarDataProvider;
                //当缓存中不存在时建立
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
                    // Do this again to make sure _provider is still null
                    if (_defaultprovider == null)
                    {
                        CommConfig config = CommConfig.GetConfig();
                        Provider dataProvider = (Provider)config.Providers[GlobalKey.DEFAULT_PROVDIER_CAR];
                        _defaultprovider = DataProvider.Instance(dataProvider) as CarDataProvider;

                    }
                }
            }
        }

        #endregion

        #region 车辆管理

        /// <summary>
        /// 获取厂商数据
        /// </summary>
        /// <returns></returns>
        public abstract DataTable GetCarcChangs();

        /// <summary>
        /// 获取汽车数据
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="query"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public abstract List<CarInfo> GetCars(int pageindex, int pagesize, CarQuery query, out int total);

        /// <summary>
        /// 添加车辆信息
        /// </summary>
        /// <param name="car"></param>
        public abstract void AddCar(CarInfo car);

        /// <summary>
        /// 编辑车辆信息
        /// </summary>
        /// <param name="car"></param>
        public abstract void UpdateCar(CarInfo car);

        public static CarInfo PopulateCarInfo(IDataReader reader)
        {
            return new CarInfo()
            {
                id = DataConvert.SafeInt(reader["id"]),
                fPp = DataConvert.SafeInt(reader["fPp"]),
                fXhid = DataConvert.SafeInt(reader["fXhid"]),
                fYh = DataConvert.SafeByte(reader["fYh"]),
                fZx = DataConvert.SafeByte(reader["fZx"]),
                fTj = DataConvert.SafeByte(reader["fTj"]),
                fPx = DataConvert.SafeInt(reader["fPx"]),
                cYhnr = reader["cYhnr"] as string,
                cJzsj = reader["cJzsj"] as string,
                xPic = reader["xPic"] as string,
                dPic = reader["dPic"] as string,
                fSscs = reader["fSscs"] as string,
                cKsjc = reader["cKsjc"] as string,
                cXh = reader["cXh"] as string,
                cQcys = reader["cQcys"] as string,
                cNsys = reader["cNsys"] as string,
                cXntd = reader["cXntd"] as string,
                cCxmc = reader["cCxmc"] as string,
                cJgqj = DataConvert.SafeDecimal(reader["cJgqj"]),
                fZdj = DataConvert.SafeDecimal(reader["fZdj"]),
                cChangs = reader["cChangs"] as string,
                cJibie = reader["cJibie"] as string,
                cFdj = reader["cFdj"] as string,
                cBsx = reader["cBsx"] as string,
                cCkg = reader["cCkg"] as string,
                cCsjg = reader["cCsjg"] as string,
                cZgcs = reader["cZgcs"] as string,
                cGfjs = reader["cGfjs"] as string,
                cScjs = reader["cScjs"] as string,
                cSczd = reader["cSczd"] as string,
                cScyh = reader["cScyh"] as string,
                cGxbyh = reader["cGxbyh"] as string,
                cZczb = reader["cZczb"] as string,
                cChang = reader["cChang"] as string,
                cKuan = reader["cKuan"] as string,
                cGao = reader["cGao"] as string,
                cZhouju = reader["cZhouju"] as string,
                cQnju = reader["cQnju"] as string,
                cHnju = reader["cHnju"] as string,
                cLdjx = reader["cLdjx"] as string,
                cZbzl = reader["cZbzl"] as string,
                cChesjg = reader["cChesjg"] as string,
                cCms = reader["cCms"] as string,
                cZws = reader["cZws"] as string,
                cYxrj = reader["cYxrj"] as string,
                cXlxrj = reader["cXlxrj"] as string,
                cFdjxh = reader["cFdjxh"] as string,
                fPail = reader["fPail"] as string,
                cJqxs = reader["cJqxs"] as string,
                cQgpl = reader["cQgpl"] as string,
                fQgs = DataConvert.SafeInt(reader["fQgs"]),
                cQms = reader["cQms"] as string,
                cYsb = reader["cYsb"] as string,
                cPqjg = reader["cPqjg"] as string,
                cGangj = reader["cGangj"] as string,
                cChongc = reader["cChongc"] as string,
                cZdml = reader["cZdml"] as string,
                cZdgl = reader["cZdgl"] as string,
                cZhuans = reader["cZhuans"] as string,
                cZdlz = reader["cZdlz"] as string,
                cLzzs = reader["cLzzs"] as string,
                cTyjs = reader["cTyjs"] as string,
                cRlxs = reader["cRlxs"] as string,
                cRybh = reader["cRybh"] as string,
                cGyfs = reader["cGyfs"] as string,
                cGgcl = reader["cGgcl"] as string,
                cGtcl = reader["cGtcl"] as string,
                cHbbz = reader["cHbbz"] as string,
                cJianc = reader["cJianc"] as string,
                cDwgs = reader["cDwgs"] as string,
                cBsxlx = reader["cBsxlx"] as string,
                cQdfs = reader["cQdfs"] as string,
                cQxglx = reader["cQxglx"] as string,
                cHxglx = reader["cHxglx"] as string,
                cZllx = reader["cZllx"] as string,
                cCtjg = reader["cCtjg"] as string,
                cQzdq = reader["cQzdq"] as string,
                cHzdq = reader["cHzdq"] as string,
                cZczd = reader["cZczd"] as string,
                cQnt = reader["cQnt"] as string,
                cHnt = reader["cHnt"] as string,
                cBetai = reader["cBetai"] as string,
                cJszqls = reader["cJszqls"] as string,
                cFjsqls = reader["cFjsqls"] as string,
                cQpcql = reader["cQpcql"] as string,
                cHpcql = reader["cHpcql"] as string,
                cQptb = reader["cQptb"] as string,
                cHptb = reader["cHptb"] as string,
                cQbql = reader["cQbql"] as string,
                cTyjc = reader["cTyjc"] as string,
                cLty = reader["cLty"] as string,
                cHqdts = reader["cHqdts"] as string,
                cIso = reader["cIso"] as string,
                cLatch = reader["cLatch"] as string,
                cFdjfd = reader["cFdjfd"] as string,
                cCnzks = reader["cCnzks"] as string,
                cYkys = reader["cYkys"] as string,
                cWysqd = reader["cWysqd"] as string,
                cAbs = reader["cAbs"] as string,
                cCbc = reader["cCbc"] as string,
                cBa = reader["cBa"] as string,
                cTrc = reader["cTrc"] as string,
                cVsc = reader["cVsc"] as string,
                cZdzc = reader["cZdzc"] as string,
                cDphj = reader["cDphj"] as string,
                cKbxg = reader["cKbxg"] as string,
                cKqxg = reader["cKqxg"] as string,
                cKbzxb = reader["cKbzxb"] as string,
                cZdtc = reader["cZdtc"] as string,
                cQjtc = reader["cQjtc"] as string,
                cYdwg = reader["cYdwg"] as string,
                cLhjn = reader["cLhjn"] as string,
                cDdxhm = reader["cDdxhm"] as string,
                cZpfxp = reader["cZpfxp"] as string,
                cSxtj = reader["cSxtj"] as string,
                cQhtj = reader["cQhtj"] as string,
                cDdtj = reader["cDdtj"] as string,
                cDgnfxp = reader["cDgnfxp"] as string,
                cFxphd = reader["cFxphd"] as string,
                cDsxh = reader["cDsxh"] as string,
                cBcfz = reader["cBcfz"] as string,
                cDcsp = reader["cDcsp"] as string,
                cDnxsp = reader["cDnxsp"] as string,
                cHud = reader["cHud"] as string,
                cZpzy = reader["cZpzy"] as string,
                cYdzy = reader["cYdzy"] as string,
                cZygd = reader["cZygd"] as string,
                cYbzc = reader["cYbzc"] as string,
                cJbzc = reader["cJbzc"] as string,
                cQpddtj = reader["cQpddtj"] as string,
                cEpjdtj = reader["cEpjdtj"] as string,
                cEpzyyd = reader["cEpzyyd"] as string,
                cHptj = reader["cHptj"] as string,
                cDdjy = reader["cDdjy"] as string,
                cQpzyjr = reader["cQpzyjr"] as string,
                cHpzyjr = reader["cHpzyjr"] as string,
                cZytf = reader["cZytf"] as string,
                cZyam = reader["cZyam"] as string,
                cHpztpf = reader["cHpztpf"] as string,
                cHpblpf = reader["cHpblpf"] as string,
                cSpzy = reader["cSpzy"] as string,
                cQzfs = reader["cQzfs"] as string,
                cHzfs = reader["cHzfs"] as string,
                cHphj = reader["cHphj"] as string,
                cDdhbx = reader["cDdhbx"] as string,
                cGps = reader["cGps"] as string,
                cDwfw = reader["cDwfw"] as string,
                cCsdp = reader["cCsdp"] as string,
                cRjjh = reader["cRjjh"] as string,
                cNzyp = reader["cNzyp"] as string,
                cCzdh = reader["cCzdh"] as string,
                cCzds = reader["cCzds"] as string,
                cHpyjp = reader["cHpyjp"] as string,
                cIpod = reader["cIpod"] as string,
                cMp3 = reader["cMp3"] as string,
                cDdcd = reader["cDdcd"] as string,
                cXndd = reader["cXndd"] as string,
                cDuodcd = reader["cDuodcd"] as string,
                cDddvd = reader["cDddvd"] as string,
                cDuodvd = reader["cDuodvd"] as string,
                c23lb = reader["c23lb"] as string,
                c45lb = reader["c45lb"] as string,
                c67lb = reader["c67lb"] as string,
                c8lb = reader["c8lb"] as string,
                cXqdd = reader["cXqdd"] as string,
                cLed = reader["cLed"] as string,
                cRjxcd = reader["cRjxcd"] as string,
                cZdtd = reader["cZdtd"] as string,
                cZxtd = reader["cZxtd"] as string,
                cQwd = reader["cQwd"] as string,
                cGdkt = reader["cGdkt"] as string,
                cQxzz = reader["cQxzz"] as string,
                cCnfwd = reader["cCnfwd"] as string,
                cQddcc = reader["cQddcc"] as string,
                cHddcc = reader["cHddcc"] as string,
                cFjs = reader["cFjs"] as string,
                cFzwx = reader["cFzwx"] as string,
                cHsjtj = reader["cHsjtj"] as string,
                cHsjjr = reader["cHsjjr"] as string,
                cFxm = reader["cFxm"] as string,
                cZdzd = reader["cZdzd"] as string,
                cHsjjy = reader["cHsjjy"] as string,
                cHfdz = reader["cHfdz"] as string,
                cHpcz = reader["cHpcz"] as string,
                cHzj = reader["cHzj"] as string,
                cHys = reader["cHys"] as string,
                cGyys = reader["cGyys"] as string,
                cSdkt = reader["cSdkt"] as string,
                cZdkt = reader["cZdkt"] as string,
                cDlkt = reader["cDlkt"] as string,
                cHzcfk = reader["cHzcfk"] as string,
                cWdkz = reader["cWdkz"] as string,
                cKqtj = reader["cKqtj"] as string,
                cCzbx = reader["cCzbx"] as string,
                cPcrw = reader["cPcrw"] as string,
                cBxfz = reader["cBxfz"] as string,
                cZdsc = reader["cZdsc"] as string,
                cZtzx = reader["cZtzx"] as string,
                cYsxt = reader["cYsxt"] as string,
                cFpxs = reader["cFpxs"] as string,
                cZsyxh = reader["cZsyxh"] as string,
                cQjsxt = reader["cQjsxt"] as string,
                fHit = DataConvert.SafeInt(reader["fHit"]),
                fDel = DataConvert.SafeByte(reader["fDel"])
            };
        }

        #endregion

        #region 车辆报价

        public abstract void AddCarQuotation(CarQuotationInfo entity);

        public abstract CarQuotationInfo GetCarQuotationModel(int id);

        public abstract List<CarQuotationInfo> GetCarQuotationList(string mobile, CarQuotationType type);

        public abstract List<CarQuotationInfo> GetCarQuotationList(int pageindex, int pagesize, CarQuotationQuery query, ref int recordcount);

        public abstract void CheckCarQuotation(CarQuotationInfo entity);

        public abstract void JLCheckCarQuotation(CarQuotationInfo entity);

        public abstract void ZJLCheckCarQuotation(CarQuotationInfo entity);

        public static CarQuotationInfo PopulateQuotationInfo(IDataReader reader)
        {
            return new CarQuotationInfo()
            {
                ID = DataConvert.SafeInt(reader["id"]),
                UCode = DataConvert.SafeInt(reader["UCode"]),
                CustomerMobile = reader["CustomerMobile"] as string,
                CustomerName = reader["CustomerName"] as string,
                SaleDay = reader["SaleDay"] as DateTime?,
                PlaceDay = reader["PlaceDay"] as DateTime?,
                Islkhzjs = DataConvert.SafeInt(reader["Islkhzjs"]),
                CustomerMicroletter = reader["CustomerMicroletter"] as string,
                cCjh = reader["cCjh"] as string,
                CarQuotationType = (CarQuotationType)(byte)reader["CarQuotationType"],
                TotalFirstPrinces = reader["TotalFirstPrinces"] as string,
                TotalPrinces = reader["TotalPrinces"] as string,
                cChangs = reader["cChangs"] as string,
                cCxmc = reader["cCxmc"] as string,
                cQcys = reader["cQcys"] as string,
                cNsys = reader["cNsys"] as string,
                fZdj = reader["fZdj"] as string,
                fCjj = reader["fCjj"] as string,
                cGzs = reader["cGzs"] as string,
                cSpf = reader["cSpf"] as string,
                cCcs = reader["cCcs"] as string,
                IscJqs = (bool)reader["IscJqs"],
                cJqs = reader["cJqs"] as string,
                Bxgs = reader["Bxgs"] as string,
                Bxhj = reader["Bxhj"] as string,
                Wyfw = reader["Wyfw"] as string,
                IsWyfwjytc = (bool)reader["IsWyfwjytc"],
                Wyfwjytc = reader["Wyfwjytc"] as string,
                IsWyfwblwyfw = (bool)reader["IsWyfwblwyfw"],
                Wyfwblwyfw = reader["Wyfwblwyfw"] as string,
                IsWyfwhhwyfw = (bool)reader["IsWyfwhhwyfw"],
                Wyfwhhwyfw = reader["Wyfwhhwyfw"] as string,
                IsWyfwybwyfw = (bool)reader["IsWyfwybwyfw"],
                Wyfwybwyfw = reader["Wyfwybwyfw"] as string,
                IscCsx = (bool)reader["IscCsx"],
                cCsx = reader["cCsx"] as string,
                IscDszrx = (bool)reader["IscDszrx"],
                cDszrx = reader["cDszrx"] as string,
                IscDqx = (bool)reader["IscDqx"],
                cDqx = reader["cDqx"] as string,
                IscSj = (bool)reader["IscSj"],
                cSj = reader["cSj"] as string,
                IscCk = (bool)reader["IscCk"],
                cCk = reader["cCk"] as string,
                IscZrx = (bool)reader["IscZrx"],
                cZrx = reader["cZrx"] as string,
                IscBlx = (bool)reader["IscBlx"],
                cBlx = reader["cBlx"] as string,
                IscHhx = (bool)reader["IscHhx"],
                cHhx = reader["cHhx"] as string,
                IscSsx = (bool)reader["IscSsx"],
                cSsx = reader["cSsx"] as string,
                BankingType = (BankingType)(byte)reader["BankingType"],
                BankName = reader["BankName"] as string,
                LoanType = (LoanType)(byte)reader["LoanType"],
                FirstPayment = reader["FirstPayment"] as string,
                LoanValue = reader["LoanValue"] as string,
                LoanLength = reader["LoanLength"] as string,
                RepaymentPerMonth = reader["RepaymentPerMonth"] as string,
                RemainingFund = reader["RemainingFund"] as string,
                ProfitMargin = reader["ProfitMargin"] as string,
                OtherCost = reader["OtherCost"] as string,
                AccountManagementCost = reader["AccountManagementCost"] as string,
                ChoicestGoods = reader["ChoicestGoods"] as string,
                ChoicestGoodsPrice = reader["ChoicestGoodsPrice"] as string,
                Gift = reader["Gift"] as string,
                IsSwap = (bool)reader["IsSwap"],
                IsDkh = (bool)reader["IsDkh"],
                IsZcyh = (bool)reader["IsZcyh"],
                SwapDetail = reader["SwapDetail"] as string,
                cBjmp = reader["cBjmp"] as string,
                cZdwx = reader["cZdwx"] as string,
                cXbyj = reader["cXbyj"] as string,
                Lyfxj = reader["Lyfxj"] as string,
                Dbsplwf = reader["Dbsplwf"] as string,
                Dbfqlwf = reader["Dbfqlwf"] as string,
                Zxf = reader["Zxf"] as string,
                Dcf = reader["Dcf"] as string,
                Zkxs = reader["Zkxs"] as string,
                Sztb = reader["Sztb"] as string,
                Blcd = reader["Blcd"] as string,
                cSjtb = reader["cSjtb"] as string,
                cCktb = reader["cCktb"] as string,
                IscZdwx = (bool)reader["IscZdwx"],
                cBjmptb = reader["cBjmptb"] as string,
                Creator = reader["Creator"] as string,
                CreateTime = reader["CreateTime"] as DateTime?,
                CorporationID = reader["CorporationID"] as string,
                CheckUser = reader["CheckUser"] as string,
                CheckTime = reader["CheckTime"] as string,
                CheckStatus = DataConvert.SafeInt(reader["CheckStatus"]),
                JLCheckUser = reader["JLCheckUser"] as string,
                JLCheckTime = reader["JLCheckTime"] as string,
                JLCheckStatus = DataConvert.SafeInt(reader["JLCheckStatus"]),
                JLCheckRemark = reader["JLCheckRemark"] as string,
                ZJLCheckUser = reader["ZJLCheckUser"] as string,
                ZJLCheckTime = reader["ZJLCheckTime"] as string,
                ZJLCheckStatus = DataConvert.SafeInt(reader["ZJLCheckStatus"]),
                ZJLCheckRemark = reader["ZJLCheckRemark"] as string,
                Qtfy = reader["Qtfy"] as string,
                Qtfyms = reader["Qtfyms"] as string,
            };
        }

        #endregion

        #region 车辆品牌

        public abstract List<CarBrandInfo> GetCarBrandList();

        public abstract void AddCarBrand(CarBrandInfo entity);

        public abstract void UpdateBrand(CarBrandInfo entity);

        public static CarBrandInfo PopulateCarBrandInfo(IDataReader reader)
        {
            return new CarBrandInfo
            {
                ID = DataConvert.SafeInt(reader["ID"]),
                Name = reader["Name"] as string,
                NameIndex = reader["NameIndex"] as string
            };
        }

        #endregion

        #region 商业保险

        public abstract void AddSybx(SybxInfo entity);

        public abstract void UpdateSybx(SybxInfo entity);

        public abstract void DeleteSybx(string ids);

        public abstract List<SybxInfo> GetSybxList();

        public static SybxInfo PopulateSybxInfo(IDataReader reader)
        {
            SybxInfo entity = new SybxInfo();
            entity.ID = (int)reader["ID"];
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 公司管理

        public abstract List<CorporationInfo> GetCorporationList();

        public abstract void AddCorporation(CorporationInfo entity);

        public abstract void UpdateCorporation(CorporationInfo entity);

        public abstract void DeleteCorporation(string ids);

        public static CorporationInfo PopulateCorporationInfo(IDataReader reader)
        {
            CorporationInfo entity = new CorporationInfo();
            entity.ID = (int)reader["ID"];
            entity.Name = reader["Name"] as string;
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);

            return entity;
        }

        #endregion

        #region 集车宝

        #region 车辆管理

        public abstract int AddJcbCar(JcbCarInfo entity);

        public abstract void DeleteJcbCars(string ids);

        public abstract List<JcbCarInfo> GetJcbCarList();

        public static JcbCarInfo PopulateJcbCarInfo(IDataReader reader)
        {
            JcbCarInfo entity = new JcbCarInfo()
            {
                ID = (int)reader["ID"],
                VINCode = reader["VINCode"] as string,
                Cph = reader["Cph"] as string,
                Ysj = DataConvert.SafeDecimal(reader["Ysj"]),
                LastUpdateTime = DataConvert.SafeDate(reader["LastUpdateTime"]),
                Cdjg = DataConvert.SafeDecimal(reader["Cdjg"]),
                UserID = (int)reader["UserID"]
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);
            entity.Picslist = json.Deserialize<List<JcbcarpicInfo>>(entity.Pics);
            return entity;
        }

        #endregion

        #region 帐号管理

        public abstract int AddJcbAccount(JcbAccountInfo entity);

        public abstract void DeleteAccount(string ids);

        public abstract List<JcbAccountInfo> GetJcbAccountList();

        public static JcbAccountInfo PopulateJcbAccountInfo(IDataReader reader)
        {
            JcbAccountInfo entity = new JcbAccountInfo()
            {
                ID = (int)reader["ID"],
                UserID = (int)reader["UserID"],
                AccountName = reader["AccountName"] as string,
                Password = reader["Password"] as string,
                AddTime = DataConvert.SafeDate(reader["AddTime"]),
                JcbSiteType = (JcbSiteType)(byte)reader["JcbSiteType"],
                JcbAccountType = (JcbAccountType)(byte)reader["JcbAccountType"]
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);
            return entity;
        }

        #endregion

        #region 营销记录

        public abstract int AddJcbMarketrecord(JcbMarketrecordInfo entity);

        public abstract List<JcbMarketrecordInfo> GetJcbMarketrecordList();

        public static JcbMarketrecordInfo PopulateJcbMarketrecordInfo(IDataReader reader)
        {
            JcbMarketrecordInfo entity = new JcbMarketrecordInfo()
            {
                ID = (int)reader["ID"],
                CarID = (int)reader["CarID"],
                AccountID = (int)reader["AccountID"],
                JcbSiteType = (JcbSiteType)(byte)reader["JcbSiteType"],
                UploadTime = reader["UploadTime"] as DateTime?,
                ViewUrl = reader["ViewUrl"] as string,
                IsSale = DataConvert.SafeBool(reader["IsSale"]),
                IsDel = DataConvert.SafeBool(reader["IsDel"])
            };
            SerializerData data = new SerializerData();
            data.Keys = reader["PropertyNames"] as string;
            data.Values = reader["PropertyValues"] as string;
            entity.SetSerializerData(data);
            return entity;
        }

        #endregion

        #endregion

        #region 侯牌器

        public abstract void CarNumberCommit(string code, string hp);

        #endregion
    }
}
