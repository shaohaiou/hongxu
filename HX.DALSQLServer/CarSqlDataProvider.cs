using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Car.Providers;
using Hx.Components.Config;
using Hx.Tools;
using System.Net;
using System.Data;
using Hx.Car.Entity;
using Hx.Components.Data;
using System.Data.SqlClient;
using Hx.Car.Enum;

namespace HX.DALSQLServer
{
    public class CarSqlDataProvider : CarDataProvider
    {
        private string _con;
        private string _dbowner;

        #region 初始化
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="constr">连接字符串</param>
        /// <param name="owner">数据库所有者</param>
        public CarSqlDataProvider(string constr, string owner)
        {
            CommConfig config = CommConfig.GetConfig();
            _con = EncryptString.DESDecode(constr, config.AppSetting["key"]);
            _dbowner = owner;
        }
        #endregion

        #region 车辆管理

        public override DataTable GetCarcChangs()
        {
            string sql = "SELECT DISTINCT [cChangs] FROM  dbo.t_QcCs ORDER BY [cChangs]";

            return SqlHelper.ExecuteDataset(_con, CommandType.Text, sql).Tables[0];
        }

        public override List<CarInfo> GetCars(int pageindex, int pagesize, Hx.Car.Query.CarQuery query, out int total)
        {
            List<CarInfo> carlist = new List<CarInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    carlist.Add(PopulateCarInfo(reader));
                }
            }
            total = int.Parse(p.Value.ToString());
            return carlist;
        }

        public override void AddCar(CarInfo car)
        {
            string sql = @"
                IF NOT EXISTS(SELECT ID FROM t_QcCs WHERE cCxmc = @cCxmc)
                BEGIN
                    INSERT INTO t_QcCs(
                    [cCxmc],[fZdj],[cChangs],[cJibie],[cFdj],[cBsx],[cCkg],[cCsjg],[cZgcs],[cGfjs],[cScjs],[cSczd],[cScyh],[cGxbyh],[cZczb],[cChang],[cKuan],[cGao],[cZhouju],[cQnju],[cHnju],[cLdjx],[cZbzl],[cChesjg]
                    ,[cCms],[cZws],[cYxrj],[cXlxrj],[cFdjxh],[fPail],[cJqxs],[cQgpl],[fQgs],[cQms],[cYsb],[cPqjg],[cGangj],[cChongc],[cZdml],[cZdgl],[cZhuans],[cZdlz],[cLzzs],[cTyjs],[cRlxs],[cRybh],[cGyfs],[cGgcl],[cGtcl],[cHbbz]
                    ,[cJianc],[cDwgs],[cBsxlx],[cQdfs],[cQxglx],[cHxglx],[cZllx],[cCtjg],[cQzdq],[cHzdq],[cZczd],[cQnt],[cHnt],[cBetai]
                    ,[cJszqls],[cFjsqls],[cQpcql],[cHpcql],[cQptb],[cHptb],[cQbql],[cTyjc],[cLty],[cHqdts],[cIso],[cLatch],[cFdjfd],[cCnzks]
                    ,[cYkys],[cWysqd],[cAbs],[cCbc],[cBa],[cTrc],[cVsc],[cZdzc],[cDphj],[cKbxg],[cKqxg],[cKbzxb],[cZdtc],[cQjtc],[cYdwg],[cLhjn]
                    ,[cDdxhm],[cZpfxp],[cSxtj],[cQhtj],[cDdtj],[cDgnfxp],[cFxphd],[cDsxh],[cBcfz],[cDcsp],[cDnxsp],[cHud],[cZpzy],[cYdzy],[cZygd]
                    ,[cYbzc],[cJbzc],[cQpddtj],[cEpjdtj],[cEpzyyd],[cHptj],[cDdjy],[cQpzyjr],[cHpzyjr],[cZytf],[cZyam],[cHpztpf],[cHpblpf],[cSpzy]
                    ,[cQzfs],[cHzfs],[cHphj],[cDdhbx],[cGps],[cDwfw],[cCsdp],[cNzyp],[cCzdh],[cCzds],[cHpyjp],[cIpod],[cMp3],[cDdcd],[cXndd],[cDuodcd]
                    ,[cDddvd],[cDuodvd],[c23lb],[c45lb],[c67lb],[c8lb],[cXqdd],[cLed],[cRjxcd],[cZdtd],[cZxtd],[cQwd],[cGdkt],[cQxzz],[cCnfwd]
                    ,[cQddcc],[cHddcc],[cFjs],[cFzwx],[cHsjtj],[cHsjjr],[cFxm],[cZdzd],[cHsjjy],[cHfdz],[cHpcz],[cHzj],[cHys],[cGyys],[cSdkt]
                    ,[cZdkt],[cDlkt],[cHzcfk],[cWdkz],[cKqtj],[cCzbx],[cPcrw],[cBxfz],[cZdsc],[cZtzx],[cYsxt],[cFpxs],[cZsyxh],[cQjsxt],[cQcys],[cNsys]
                    )VALUES(
                    @cCxmc,@fZdj,@cChangs,@cJibie,@cFdj,@cBsx,@cCkg,@cCsjg,@cZgcs,@cGfjs,@cScjs,@cSczd,@cScyh,@cGxbyh,@cZczb,@cChang,@cKuan,@cGao,@cZhouju,@cQnju,@cHnju,@cLdjx,@cZbzl,@cChesjg
                    ,@cCms,@cZws,@cYxrj,@cXlxrj,@cFdjxh,@fPail,@cJqxs,@cQgpl,@fQgs,@cQms,@cYsb,@cPqjg,@cGangj,@cChongc,@cZdml,@cZdgl,@cZhuans,@cZdlz,@cLzzs,@cTyjs,@cRlxs,@cRybh,@cGyfs,@cGgcl,@cGtcl,@cHbbz
                    ,@cJianc,@cDwgs,@cBsxlx,@cQdfs,@cQxglx,@cHxglx,@cZllx,@cCtjg,@cQzdq,@cHzdq,@cZczd,@cQnt,@cHnt,@cBetai
                    ,@cJszqls,@cFjsqls,@cQpcql,@cHpcql,@cQptb,@cHptb,@cQbql,@cTyjc,@cLty,@cHqdts,@cIso,@cLatch,@cFdjfd,@cCnzks
                    ,@cYkys,@cWysqd,@cAbs,@cCbc,@cBa,@cTrc,@cVsc,@cZdzc,@cDphj,@cKbxg,@cKqxg,@cKbzxb,@cZdtc,@cQjtc,@cYdwg,@cLhjn
                    ,@cDdxhm,@cZpfxp,@cSxtj,@cQhtj,@cDdtj,@cDgnfxp,@cFxphd,@cDsxh,@cBcfz,@cDcsp,@cDnxsp,@cHud,@cZpzy,@cYdzy,@cZygd
                    ,@cYbzc,@cJbzc,@cQpddtj,@cEpjdtj,@cEpzyyd,@cHptj,@cDdjy,@cQpzyjr,@cHpzyjr,@cZytf,@cZyam,@cHpztpf,@cHpblpf,@cSpzy
                    ,@cQzfs,@cHzfs,@cHphj,@cDdhbx,@cGps,@cDwfw,@cCsdp,@cNzyp,@cCzdh,@cCzds,@cHpyjp,@cIpod,@cMp3,@cDdcd,@cXndd,@cDuodcd
                    ,@cDddvd,@cDuodvd,@c23lb,@c45lb,@c67lb,@c8lb,@cXqdd,@cLed,@cRjxcd,@cZdtd,@cZxtd,@cQwd,@cGdkt,@cQxzz,@cCnfwd
                    ,@cQddcc,@cHddcc,@cFjs,@cFzwx,@cHsjtj,@cHsjjr,@cFxm,@cZdzd,@cHsjjy,@cHfdz,@cHpcz,@cHzj,@cHys,@cGyys,@cSdkt
                    ,@cZdkt,@cDlkt,@cHzcfk,@cWdkz,@cKqtj,@cCzbx,@cPcrw,@cBxfz,@cZdsc,@cZtzx,@cYsxt,@cFpxs,@cZsyxh,@cQjsxt,@cQcys,@cNsys
                    )
                END
                ELSE 
                BEGIN
                    UPDATE t_QcCs SET
                        [fZdj] = @fZdj
                        ,[cChangs] = @cChangs
                        ,[cQcys] = @cQcys
                        ,[cNsys] = @cNsys
                    WHERE [cCxmc] = @cCxmc
                END
                ";
            SqlParameter[] p = 
            { 
                new SqlParameter("@fPp",car.fPp),
                new SqlParameter("@fXhid",car.fXhid),
                new SqlParameter("@fYh",car.fYh),
                new SqlParameter("@fZx",car.fZx),
                new SqlParameter("@fTj",car.fTj),
                new SqlParameter("@fPx",car.fPx),
                new SqlParameter("@cYhnr",car.cYhnr),
                new SqlParameter("@cJzsj",car.cJzsj),
                new SqlParameter("@xPic",car.xPic),
                new SqlParameter("@dPic",car.dPic),
                new SqlParameter("@fSscs",car.fSscs),
                new SqlParameter("@cKsjc",car.cKsjc),
                new SqlParameter("@cXh",car.cXh),
                new SqlParameter("@cQcys",car.cQcys),
                new SqlParameter("@cNsys",car.cNsys),
                new SqlParameter("@cXntd",car.cXntd),
                new SqlParameter("@cCxmc",car.cCxmc),
                new SqlParameter("@cJgqj",car.cJgqj),
                new SqlParameter("@fZdj",car.fZdj),
                new SqlParameter("@cChangs",car.cChangs),
                new SqlParameter("@cJibie",car.cJibie),
                new SqlParameter("@cFdj",car.cFdj),
                new SqlParameter("@cBsx",car.cBsx),
                new SqlParameter("@cCkg",car.cCkg),
                new SqlParameter("@cCsjg",car.cCsjg),
                new SqlParameter("@cZgcs",car.cZgcs),
                new SqlParameter("@cGfjs",car.cGfjs),
                new SqlParameter("@cScjs",car.cScjs),
                new SqlParameter("@cSczd",car.cSczd),
                new SqlParameter("@cScyh",car.cScyh),
                new SqlParameter("@cGxbyh",car.cGxbyh),
                new SqlParameter("@cZczb",car.cZczb),
                new SqlParameter("@cChang",car.cChang),
                new SqlParameter("@cKuan",car.cKuan),
                new SqlParameter("@cGao",car.cGao),
                new SqlParameter("@cZhouju",car.cZhouju),
                new SqlParameter("@cQnju",car.cQnju),
                new SqlParameter("@cHnju",car.cHnju),
                new SqlParameter("@cLdjx",car.cLdjx),
                new SqlParameter("@cZbzl",car.cZbzl),
                new SqlParameter("@cChesjg",car.cChesjg),
                new SqlParameter("@cCms",car.cCms),
                new SqlParameter("@cZws",car.cZws),
                new SqlParameter("@cYxrj",car.cYxrj),
                new SqlParameter("@cXlxrj",car.cXlxrj),
                new SqlParameter("@cFdjxh",car.cFdjxh),
                new SqlParameter("@fPail",car.fPail),
                new SqlParameter("@cJqxs",car.cJqxs),
                new SqlParameter("@cQgpl",car.cQgpl),
                new SqlParameter("@fQgs",car.fQgs),
                new SqlParameter("@cQms",car.cQms),
                new SqlParameter("@cYsb",car.cYsb),
                new SqlParameter("@cPqjg",car.cPqjg),
                new SqlParameter("@cGangj",car.cGangj),
                new SqlParameter("@cChongc",car.cChongc),
                new SqlParameter("@cZdml",car.cZdml),
                new SqlParameter("@cZdgl",car.cZdgl),
                new SqlParameter("@cZhuans",car.cZhuans),
                new SqlParameter("@cZdlz",car.cZdlz),
                new SqlParameter("@cLzzs",car.cLzzs),
                new SqlParameter("@cTyjs",car.cTyjs),
                new SqlParameter("@cRlxs",car.cRlxs),
                new SqlParameter("@cRybh",car.cRybh),
                new SqlParameter("@cGyfs",car.cGyfs),
                new SqlParameter("@cGgcl",car.cGgcl),
                new SqlParameter("@cGtcl",car.cGtcl),
                new SqlParameter("@cHbbz",car.cHbbz),
                new SqlParameter("@cJianc",car.cJianc),
                new SqlParameter("@cDwgs",car.cDwgs),
                new SqlParameter("@cBsxlx",car.cBsxlx),
                new SqlParameter("@cQdfs",car.cQdfs),
                new SqlParameter("@cQxglx",car.cQxglx),
                new SqlParameter("@cHxglx",car.cHxglx),
                new SqlParameter("@cZllx",car.cZllx),
                new SqlParameter("@cCtjg",car.cCtjg),
                new SqlParameter("@cQzdq",car.cQzdq),
                new SqlParameter("@cHzdq",car.cHzdq),
                new SqlParameter("@cZczd",car.cZczd),
                new SqlParameter("@cQnt",car.cQnt),
                new SqlParameter("@cHnt",car.cHnt),
                new SqlParameter("@cBetai",car.cBetai),
                new SqlParameter("@cJszqls",car.cJszqls),
                new SqlParameter("@cFjsqls",car.cFjsqls),
                new SqlParameter("@cQpcql",car.cQpcql),
                new SqlParameter("@cHpcql",car.cHpcql),
                new SqlParameter("@cQptb",car.cQptb),
                new SqlParameter("@cHptb",car.cHptb),
                new SqlParameter("@cQbql",car.cQbql),
                new SqlParameter("@cTyjc",car.cTyjc),
                new SqlParameter("@cLty",car.cLty),
                new SqlParameter("@cHqdts",car.cHqdts),
                new SqlParameter("@cIso",car.cIso),
                new SqlParameter("@cLatch",car.cLatch),
                new SqlParameter("@cFdjfd",car.cFdjfd),
                new SqlParameter("@cCnzks",car.cCnzks),
                new SqlParameter("@cYkys",car.cYkys),
                new SqlParameter("@cWysqd",car.cWysqd),
                new SqlParameter("@cAbs",car.cAbs),
                new SqlParameter("@cCbc",car.cCbc),
                new SqlParameter("@cBa",car.cBa),
                new SqlParameter("@cTrc",car.cTrc),
                new SqlParameter("@cVsc",car.cVsc),
                new SqlParameter("@cZdzc",car.cZdzc),
                new SqlParameter("@cDphj",car.cDphj),
                new SqlParameter("@cKbxg",car.cKbxg),
                new SqlParameter("@cKqxg",car.cKqxg),
                new SqlParameter("@cKbzxb",car.cKbzxb),
                new SqlParameter("@cZdtc",car.cZdtc),
                new SqlParameter("@cQjtc",car.cQjtc),
                new SqlParameter("@cYdwg",car.cYdwg),
                new SqlParameter("@cLhjn",car.cLhjn),
                new SqlParameter("@cDdxhm",car.cDdxhm),
                new SqlParameter("@cZpfxp",car.cZpfxp),
                new SqlParameter("@cSxtj",car.cSxtj),
                new SqlParameter("@cQhtj",car.cQhtj),
                new SqlParameter("@cDdtj",car.cDdtj),
                new SqlParameter("@cDgnfxp",car.cDgnfxp),
                new SqlParameter("@cFxphd",car.cFxphd),
                new SqlParameter("@cDsxh",car.cDsxh),
                new SqlParameter("@cBcfz",car.cBcfz),
                new SqlParameter("@cDcsp",car.cDcsp),
                new SqlParameter("@cDnxsp",car.cDnxsp),
                new SqlParameter("@cHud",car.cHud),
                new SqlParameter("@cZpzy",car.cZpzy),
                new SqlParameter("@cYdzy",car.cYdzy),
                new SqlParameter("@cZygd",car.cZygd),
                new SqlParameter("@cYbzc",car.cYbzc),
                new SqlParameter("@cJbzc",car.cJbzc),
                new SqlParameter("@cQpddtj",car.cQpddtj),
                new SqlParameter("@cEpjdtj",car.cEpjdtj),
                new SqlParameter("@cEpzyyd",car.cEpzyyd),
                new SqlParameter("@cHptj",car.cHptj),
                new SqlParameter("@cDdjy",car.cDdjy),
                new SqlParameter("@cQpzyjr",car.cQpzyjr),
                new SqlParameter("@cHpzyjr",car.cHpzyjr),
                new SqlParameter("@cZytf",car.cZytf),
                new SqlParameter("@cZyam",car.cZyam),
                new SqlParameter("@cHpztpf",car.cHpztpf),
                new SqlParameter("@cHpblpf",car.cHpblpf),
                new SqlParameter("@cSpzy",car.cSpzy),
                new SqlParameter("@cQzfs",car.cQzfs),
                new SqlParameter("@cHzfs",car.cHzfs),
                new SqlParameter("@cHphj",car.cHphj),
                new SqlParameter("@cDdhbx",car.cDdhbx),
                new SqlParameter("@cGps",car.cGps),
                new SqlParameter("@cDwfw",car.cDwfw),
                new SqlParameter("@cCsdp",car.cCsdp),
                new SqlParameter("@cRjjh",car.cRjjh),
                new SqlParameter("@cNzyp",car.cNzyp),
                new SqlParameter("@cCzdh",car.cCzdh),
                new SqlParameter("@cCzds",car.cCzds),
                new SqlParameter("@cHpyjp",car.cHpyjp),
                new SqlParameter("@cIpod",car.cIpod),
                new SqlParameter("@cMp3",car.cMp3),
                new SqlParameter("@cDdcd",car.cDdcd),
                new SqlParameter("@cXndd",car.cXndd),
                new SqlParameter("@cDuodcd",car.cDuodcd),
                new SqlParameter("@cDddvd",car.cDddvd),
                new SqlParameter("@cDuodvd",car.cDuodvd),
                new SqlParameter("@c23lb",car.c23lb),
                new SqlParameter("@c45lb",car.c45lb),
                new SqlParameter("@c67lb",car.c67lb),
                new SqlParameter("@c8lb",car.c8lb),
                new SqlParameter("@cXqdd",car.cXqdd),
                new SqlParameter("@cLed",car.cLed),
                new SqlParameter("@cRjxcd",car.cRjxcd),
                new SqlParameter("@cZdtd",car.cZdtd),
                new SqlParameter("@cZxtd",car.cZxtd),
                new SqlParameter("@cQwd",car.cQwd),
                new SqlParameter("@cGdkt",car.cGdkt),
                new SqlParameter("@cQxzz",car.cQxzz),
                new SqlParameter("@cCnfwd",car.cCnfwd),
                new SqlParameter("@cQddcc",car.cQddcc),
                new SqlParameter("@cHddcc",car.cHddcc),
                new SqlParameter("@cFjs",car.cFjs),
                new SqlParameter("@cFzwx",car.cFzwx),
                new SqlParameter("@cHsjtj",car.cHsjtj),
                new SqlParameter("@cHsjjr",car.cHsjjr),
                new SqlParameter("@cFxm",car.cFxm),
                new SqlParameter("@cZdzd",car.cZdzd),
                new SqlParameter("@cHsjjy",car.cHsjjy),
                new SqlParameter("@cHfdz",car.cHfdz),
                new SqlParameter("@cHpcz",car.cHpcz),
                new SqlParameter("@cHzj",car.cHzj),
                new SqlParameter("@cHys",car.cHys),
                new SqlParameter("@cGyys",car.cGyys),
                new SqlParameter("@cSdkt",car.cSdkt),
                new SqlParameter("@cZdkt",car.cZdkt),
                new SqlParameter("@cDlkt",car.cDlkt),
                new SqlParameter("@cHzcfk",car.cHzcfk),
                new SqlParameter("@cWdkz",car.cWdkz),
                new SqlParameter("@cKqtj",car.cKqtj),
                new SqlParameter("@cCzbx",car.cCzbx),
                new SqlParameter("@cPcrw",car.cPcrw),
                new SqlParameter("@cBxfz",car.cBxfz),
                new SqlParameter("@cZdsc",car.cZdsc),
                new SqlParameter("@cZtzx",car.cZtzx),
                new SqlParameter("@cYsxt",car.cYsxt),
                new SqlParameter("@cFpxs",car.cFpxs),
                new SqlParameter("@cZsyxh",car.cZsyxh),
                new SqlParameter("@cQjsxt",car.cQjsxt),
                new SqlParameter("@fHit",car.fHit),
                new SqlParameter("@fDel",car.fDel)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        #endregion

        #region 车辆品牌

        public override List<CarBrandInfo> GetCarBrandList()
        {
            List<CarBrandInfo> list = new List<CarBrandInfo>();
            string sql = "SELECT * FROM HX_CarBrand";

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateCarBrandInfo(reader));
                }
            }

            return list;
        }

        public override void AddCarBrand(CarBrandInfo entity)
        {
            string sql = "HX_AddCarBrand";
            SqlParameter[] p = 
            { 
                new SqlParameter("@Name",entity.Name),
                new SqlParameter("@NameIndex",entity.NameIndex)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.StoredProcedure, sql, p);
        }

        #endregion

        #region 车辆报价

        public override void AddCarQuotation(CarQuotationInfo entity)
        {
            string sql = "HX_AddCarQuotation";
            SqlParameter[] p = 
            { 
                new SqlParameter("@CorporationID",entity.CorporationID),
                new SqlParameter("@Creator",entity.Creator),
                new SqlParameter("@CustomerMobile",entity.CustomerMobile),
                new SqlParameter("@CustomerName",entity.CustomerName),
                new SqlParameter("@CustomerMicroletter",entity.CustomerMicroletter),
                new SqlParameter("@cCjh",entity.cCjh),
                new SqlParameter("@SaleDay",entity.SaleDay),
                new SqlParameter("@PlaceDay",entity.PlaceDay),
                new SqlParameter("@Islkhzjs",entity.Islkhzjs),
                new SqlParameter("@CarQuotationType",entity.CarQuotationType),
                new SqlParameter("@TotalFirstPrinces",entity.TotalFirstPrinces),
                new SqlParameter("@TotalPrinces",entity.TotalPrinces),
                new SqlParameter("@cChangs",entity.cChangs),
                new SqlParameter("@cCxmc",entity.cCxmc),
                new SqlParameter("@cQcys",entity.cQcys),
                new SqlParameter("@cNsys",entity.cNsys),
                new SqlParameter("@fZdj",entity.fZdj),
                new SqlParameter("@fCjj",entity.fCjj),
                new SqlParameter("@cGzs",entity.cGzs),
                new SqlParameter("@cSpf",entity.cSpf),
                new SqlParameter("@cCcs",entity.cCcs),
                new SqlParameter("@IscJqs",entity.IscJqs),
                new SqlParameter("@cJqs",entity.cJqs),
                new SqlParameter("@Bxgs",entity.Bxgs),
                new SqlParameter("@Bxhj",entity.Bxhj),
                new SqlParameter("@Wyfw",entity.Wyfw),
                new SqlParameter("@IsWyfwjytc",entity.IsWyfwjytc),
                new SqlParameter("@Wyfwjytc",entity.Wyfwjytc),
                new SqlParameter("@IsWyfwblwyfw",entity.IsWyfwblwyfw),
                new SqlParameter("@Wyfwblwyfw",entity.Wyfwblwyfw),
                new SqlParameter("@IsWyfwhhwyfw",entity.IsWyfwhhwyfw),
                new SqlParameter("@Wyfwhhwyfw",entity.Wyfwhhwyfw),
                new SqlParameter("@IsWyfwybwyfw",entity.IsWyfwybwyfw),
                new SqlParameter("@Wyfwybwyfw",entity.Wyfwybwyfw),
                new SqlParameter("@IscCsx",entity.IscCsx),
                new SqlParameter("@cCsx",entity.cCsx),
                new SqlParameter("@IscDszrx",entity.IscDszrx),
                new SqlParameter("@cDszrx",entity.cDszrx),
                new SqlParameter("@IscDqx",entity.IscDqx),
                new SqlParameter("@cDqx",entity.cDqx),
                new SqlParameter("@IscSj",entity.IscSj),
                new SqlParameter("@cSj",entity.cSj),
                new SqlParameter("@IscCk",entity.IscCk),
                new SqlParameter("@cCk",entity.cCk),
                new SqlParameter("@IscZrx",entity.IscZrx),
                new SqlParameter("@cZrx",entity.cZrx),
                new SqlParameter("@IscBlx",entity.IscBlx),
                new SqlParameter("@cBlx",entity.cBlx),
                new SqlParameter("@IscHhx",entity.IscHhx),
                new SqlParameter("@cHhx",entity.cHhx),
                new SqlParameter("@IscSsx",entity.IscSsx),
                new SqlParameter("@cSsx",entity.cSsx),
                new SqlParameter("@BankingType",entity.BankingType),
                new SqlParameter("@BankName",entity.BankName),
                new SqlParameter("@LoanType",entity.LoanType),
                new SqlParameter("@FirstPayment",entity.FirstPayment),
                new SqlParameter("@LoanValue",entity.LoanValue),
                new SqlParameter("@LoanLength",entity.LoanLength),
                new SqlParameter("@RepaymentPerMonth",entity.RepaymentPerMonth),
                new SqlParameter("@RemainingFund",entity.RemainingFund),
                new SqlParameter("@ProfitMargin",entity.ProfitMargin),
                new SqlParameter("@OtherCost",entity.OtherCost),
                new SqlParameter("@AccountManagementCost",entity.AccountManagementCost),
                new SqlParameter("@ChoicestGoods",entity.ChoicestGoods),
                new SqlParameter("@ChoicestGoodsPrice",entity.ChoicestGoodsPrice),
                new SqlParameter("@Gift",entity.Gift),
                new SqlParameter("@IsSwap",entity.IsSwap),
                new SqlParameter("@IsDkh",entity.IsDkh),
                new SqlParameter("@IsZcyh",entity.IsZcyh),
                new SqlParameter("@SwapDetail",entity.SwapDetail),
                new SqlParameter("@cBjmp",entity.cBjmp),
                new SqlParameter("@cZdwx",entity.cZdwx),
                new SqlParameter("@cXbyj",entity.cXbyj),
                new SqlParameter("@Lyfxj",entity.Lyfxj),
                new SqlParameter("@Dbsplwf",entity.Dbsplwf),
                new SqlParameter("@Dbfqlwf",entity.Dbfqlwf),
                new SqlParameter("@Zxf",entity.Zxf),
                new SqlParameter("@Dcf",entity.Dcf),
                new SqlParameter("@Zkxs",entity.Zkxs),
                new SqlParameter("@Sztb",entity.Sztb),
                new SqlParameter("@Blcd",entity.Blcd),
                new SqlParameter("@cSjtb",entity.cSjtb),
                new SqlParameter("@cCktb",entity.cCktb),
                new SqlParameter("@IscZdwx",entity.IscZdwx),
                new SqlParameter("@cBjmptb",entity.cBjmptb),
                new SqlParameter("@Qtfy",entity.Qtfy),
                new SqlParameter("@Qtfyms",entity.Qtfyms)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.StoredProcedure, sql, p);
        }

        public override CarQuotationInfo GetCarQuotationModel(int id)
        {
            CarQuotationInfo model = null;
            string sql = "SELECT * FROM dbo.HX_CarQuotation WHERE ID = @ID";
            SqlParameter[] p = 
            { 
                new SqlParameter("@ID",id)
            };
            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, p))
            {
                if (reader.Read())
                {
                    model = PopulateQuotationInfo(reader);
                }
            }

            return model;
        }

        public override List<CarQuotationInfo> GetCarQuotationList(string mobile, CarQuotationType type)
        {
            List<CarQuotationInfo> list = new List<CarQuotationInfo>();

            string sql = "SELECT * FROM dbo.HX_CarQuotation WHERE [CustomerMobile] = @CustomerMobile AND [CarQuotationType] = @CarQuotationType";

            SqlParameter[] p = 
            { 
                new SqlParameter("@CustomerMobile",mobile),
                new SqlParameter("@CarQuotationType", type)
            };

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql, p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateQuotationInfo(reader));
                }
            }

            return list;
        }

        public override List<CarQuotationInfo> GetCarQuotationList(int pageindex, int pagesize, Hx.Car.Query.CarQuotationQuery query, ref int recordcount)
        {
            List<CarQuotationInfo> list = new List<CarQuotationInfo>();
            SqlParameter p;
            using (IDataReader reader = CommonPageSql.GetDataReaderByPager(_con, pageindex, pagesize, query, out p))
            {
                while (reader.Read())
                {
                    list.Add(PopulateQuotationInfo(reader));
                }
            }
            recordcount = DataConvert.SafeInt(p.Value);

            return list;
        }

        public override void CheckCarQuotation(CarQuotationInfo entity)
        {
            string sql = @"
            UPDATE HX_CarQuotation SET
                [CheckStatus] = @CheckStatus
                ,[CheckTime] = @CheckTime
                ,[CheckUser] = @CheckUser
            WHERE [ID] = @ID
            ";
            SqlParameter[] p = 
            {
                new SqlParameter("@CheckStatus",entity.CheckStatus),
                new SqlParameter("@CheckTime",entity.CheckTime),
                new SqlParameter("@CheckUser",entity.CheckUser),
                new SqlParameter("@ID",entity.ID)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        #endregion

        #region 商业保险

        public override void AddSybx(SybxInfo entity)
        {
            string sql = @"
            INSERT INTO HX_Sybx(
                PropertyNames
                ,PropertyValues
            )VALUES (
                @PropertyNames
                ,@PropertyValues
            )
            SELECT @@IDENTITY";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
				new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override void UpdateSybx(SybxInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            UPDATE HX_Sybx set
                PropertyNames=@PropertyNames
                ,PropertyValues=@PropertyValues
            WHERE ID=@ID";
            SqlParameter[] parameters = 
            {
			    new SqlParameter("@ID", entity.ID),
			    new SqlParameter("@PropertyNames", data.Keys),
			    new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, parameters);
        }

        public override void DeleteSybx(string ids)
        {
            string sql = "DELETE FROM HX_Sybx WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<SybxInfo> GetSybxList()
        {
            List<SybxInfo> list = new List<SybxInfo>();
            string sql = "SELECT * FROM HX_Sybx";

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateSybxInfo(reader));
                }
            }

            return list;
        }

        #endregion

        #region 公司管理

        public override void AddCorporation(CorporationInfo entity)
        {
            string sql = "INSERT INTO HX_Corporation([Name],[PropertyNames],[PropertyValues])VALUES(@Name,@PropertyNames,@PropertyValues)";
            SerializerData data = entity.GetSerializerData();
            SqlParameter[] p = 
            {
                new SqlParameter("@Name",entity.Name),
				new SqlParameter("@PropertyNames", data.Keys),
				new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, p);
        }

        public override List<CorporationInfo> GetCorporationList()
        {
            List<CorporationInfo> list = new List<CorporationInfo>();
            string sql = "SELECT * FROM HX_Corporation";

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateCorporationInfo(reader));
                }
            }

            return list;
        }

        public override void UpdateCorporation(CorporationInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            UPDATE HX_Corporation set
                Name = @Name
                ,PropertyNames=@PropertyNames
                ,PropertyValues=@PropertyValues
            WHERE ID=@ID";
            SqlParameter[] parameters = 
            {
			    new SqlParameter("@ID", entity.ID),
			    new SqlParameter("@Name", entity.Name),
			    new SqlParameter("@PropertyNames", data.Keys),
			    new SqlParameter("@PropertyValues", data.Values)
            };
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql, parameters);
        }

        public override void DeleteCorporation(string ids)
        {
            string sql = "DELETE FROM HX_Corporation WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        #endregion

        #region 集车宝

        #region 车辆管理

        public override int AddJcbCar(JcbCarInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF @ID = 0
            BEGIN
                INSERT INTO HX_JcbCar(
                    [VINCode]
                    ,[Cph]
                    ,[Ysj]
                    ,[LastUpdateTime]
                    ,[Cdjg]
                    ,[UserID]
                    ,[PropertyNames]
                    ,[PropertyValues]) VALUES (
                    @VINCode
                    ,@Cph
                    ,@Ysj
                    ,@LastUpdateTime
                    ,@Cdjg
                    ,@UserID
                    ,@PropertyNames
                    ,@PropertyValues)
                ;SELECT @@IDENTITY
            END
            ELSE 
            BEGIN
                UPDATE HX_JcbCar SET
                [VINCode] = @VINCode
                ,[Cph] = @Cph
                ,[Ysj] = @Ysj
                ,[LastUpdateTime] = @LastUpdateTime
                ,[Cdjg] = @Cdjg
                ,[UserID] = @UserID
                ,[PropertyNames] = @PropertyNames
                ,[PropertyValues] = @PropertyValues
                WHERE [ID] = @ID
                ;SELECT @ID
            END";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@VINCode",entity.VINCode),
                new SqlParameter("@Cph",entity.Cph),
                new SqlParameter("@Ysj",entity.Ysj),
                new SqlParameter("@LastUpdateTime",entity.LastUpdateTime),
                new SqlParameter("@Cdjg",entity.Cdjg),
                new SqlParameter("@UserID",entity.UserID),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            entity.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return entity.ID;
        }

        public override void DeleteJcbCars(string ids)
        {
            string sql = "DELETE FROM HX_JcbCar WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        public override List<JcbCarInfo> GetJcbCarList()
        {
            List<JcbCarInfo> list = new List<JcbCarInfo>();
            string sql = "SELECT * FROM HX_JcbCar";

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateJcbCarInfo(reader));
                }
            }

            return list;
        }
        
        #endregion

        #region 帐号管理

        public override int AddJcbAccount(JcbAccountInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF @ID = 0
            BEGIN
                INSERT INTO HX_JcbAccount(
                    [UserID]
                    ,[AccountName]
                    ,[Password]
                    ,[JcbSiteType]
                    ,[JcbAccountType]
                    ,[AddTime]
                    ,[PropertyNames]
                    ,[PropertyValues]) VALUES (
                    @UserID
                    ,@AccountName
                    ,@Password
                    ,@JcbSiteType
                    ,@JcbAccountType
                    ,@AddTime
                    ,@PropertyNames
                    ,@PropertyValues)
                ;SELECT @@IDENTITY
            END
            ELSE 
            BEGIN
                UPDATE HX_JcbAccount SET
                [UserID] = @UserID
                ,[AccountName] = @AccountName
                ,[Password] = @Password
                ,[JcbSiteType] = @JcbSiteType
                ,[JcbAccountType] = @JcbAccountType
                ,[PropertyNames] = @PropertyNames
                ,[PropertyValues] = @PropertyValues
                WHERE [ID] = @ID
                ;SELECT @ID
            END";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@UserID",entity.UserID),
                new SqlParameter("@AccountName",entity.AccountName),
                new SqlParameter("@Password",entity.Password),
                new SqlParameter("@JcbSiteType",(byte)entity.JcbSiteType),
                new SqlParameter("@JcbAccountType",(byte)entity.JcbAccountType),
                new SqlParameter("@AddTime",entity.AddTime),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            entity.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return entity.ID;
        }

        public override List<JcbAccountInfo> GetJcbAccountList()
        {
            List<JcbAccountInfo> list = new List<JcbAccountInfo>();
            string sql = "SELECT * FROM HX_JcbAccount";

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateJcbAccountInfo(reader));
                }
            }

            return list;
        }

        public override void DeleteAccount(string ids)
        {
            string sql = "DELETE FROM HX_JcbAccount WHERE ID IN (" + ids + ")";
            SqlHelper.ExecuteNonQuery(_con, CommandType.Text, sql);
        }

        #endregion

        #region 营销记录

        public override int AddJcbMarketrecord(JcbMarketrecordInfo entity)
        {
            SerializerData data = entity.GetSerializerData();
            string sql = @"
            IF @ID = 0
            BEGIN
                INSERT INTO HX_JcbMarketrecord(
                    [CarID]
                    ,[AccountID]
                    ,[JcbSiteType]
                    ,[UploadTime]
                    ,[ViewUrl]
                    ,[IsSale]
                    ,[IsDel]
                    ,[PropertyNames]
                    ,[PropertyValues]) VALUES (
                    @CarID
                    ,@AccountID
                    ,@JcbSiteType
                    ,@UploadTime
                    ,@ViewUrl
                    ,@IsSale
                    ,@IsDel
                    ,@PropertyNames
                    ,@PropertyValues)
                ;SELECT @@IDENTITY
            END
            ELSE 
            BEGIN
                UPDATE HX_JcbMarketrecord SET
                [CarID] = @CarID
                ,[AccountID] = @AccountID
                ,[JcbSiteType] = @JcbSiteType
                ,[UploadTime] = @UploadTime
                ,[ViewUrl] = @ViewUrl
                ,[IsSale] = @IsSale
                ,[IsDel] = @IsDel
                ,[PropertyNames] = @PropertyNames
                ,[PropertyValues] = @PropertyValues
                WHERE [ID] = @ID
                ;SELECT @ID
            END";
            SqlParameter[] p = 
            {
                new SqlParameter("@ID",entity.ID),
                new SqlParameter("@CarID",entity.CarID),
                new SqlParameter("@AccountID",entity.AccountID),
                new SqlParameter("@JcbSiteType",(byte)entity.JcbSiteType),
                new SqlParameter("@UploadTime",entity.UploadTime),
                new SqlParameter("@ViewUrl",entity.ViewUrl),
                new SqlParameter("@IsSale",entity.IsSale),
                new SqlParameter("@IsDel",entity.IsDel),
                new SqlParameter("@PropertyNames",data.Keys),
                new SqlParameter("@PropertyValues",data.Values)
            };
            entity.ID = DataConvert.SafeInt(SqlHelper.ExecuteScalar(_con, CommandType.Text, sql, p));
            return entity.ID;
        }

        public override List<JcbMarketrecordInfo> GetJcbMarketrecordList()
        {
            List<JcbMarketrecordInfo> list = new List<JcbMarketrecordInfo>();
            string sql = "SELECT * FROM HX_JcbMarketrecord";

            using (IDataReader reader = SqlHelper.ExecuteReader(_con, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    list.Add(PopulateJcbMarketrecordInfo(reader));
                }
            }

            return list;
        }

        #endregion

        #endregion

        #region 侯牌器

        public override void CarNumberCommit(string code, string hp)
        {
            string sql = @"INSERT INTO CarNumber_Records(
                [Code]
                ,[Hp]
                )VALUES(
                @Code
                ,@Hp
                )";
            SqlParameter[] p = 
            {
                new SqlParameter("@Code",code),
                new SqlParameter("@Hp",hp)
            };
            SqlHelper.ExecuteNonQuery(_con,CommandType.Text,sql,p);
        }

        #endregion
    }
}
