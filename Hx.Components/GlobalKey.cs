using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components
{
    public class GlobalKey
    {
        public static readonly string COMMCONFIG = "cache-config-commconfig";                   //主配置文件缓存键值
        public static readonly string PROVIDER = "cache-provider";                              //数据访问层提供类缓存键值
        public static readonly string DEFAULT_PROVDIER_COMMON = "MSSQLCommonDataProvider";//默认通用数据访问层提供类
        public static readonly string DEFAULT_PROVDIER_CAR = "MSSQLCarDataProvider";//默认汽车数据访问层提供类

        public static readonly string EVENTLOG_KEY = "cache-sign-event";
        public static readonly string SESSION_ADMIN = "Session_Admin";//后台用户Session
        public static readonly string MACHINEKEY_COOKIENAME = "HX_MACHINEKEY";  //客户端唯一标示的cookie键值
        public static readonly string CONTEXT_KEY = "HXContext";               //当前上下文键值
        public static readonly string REWRITER_KEY = "cache-config-rewriter";

        public static readonly string CARCCHANGS_LIST = "cache-carcchangs-list"; //汽车厂商列表缓存键值
        public static readonly string CARBRAND_LIST = "cache-carbrand-list"; //汽车品牌列表缓存键值
        public static readonly string CARBRANDBYCORPORATION_LIST = "cache-carbrandbycorporation-list"; //公司相关汽车品牌列表缓存键值
        public static readonly string CAR_LIST = "cache-car-list"; //车辆信息列表缓存键值
        public static readonly string CARQUOTATION_KEY = "cache-carquotation"; //车辆报价缓存键值

        public static readonly string CUSTOMER_KEY = "cache-customer"; //客户缓存键值
        public static readonly string CUSTOMERQUOTATIONID_KEY = "cache-customerquotationid"; //客户报价单id缓存键值
        public static readonly string SYBX_LIST = "cache-sybx-list"; //商业保险列表缓存键值
        public static readonly string BANK_LIST = "cache-bank-list"; //银行列表缓存键值
        public static readonly string CORPORATION_LIST = "cache-corporation-list"; //公司列表缓存键值
        public static readonly string DAYREPORTUSER_LIST = "cache-dayreportuser-list"; //日报用户列表缓存键值
        public static readonly string DAILYREPORT_LIST = "cache-dailyreport-list"; //日报列表缓存键值
        public static readonly string MONTHTARGET_LIST = "cache-monthtarget-list"; //月度目标列表缓存键值
        public static readonly string DAILYREPORTMODULE_LIST = "cache-dailyreportmodule-list"; //日报模块列表缓存键值
        public static readonly string JOBOFFER_LIST = "cache-joboffer-list"; //招聘信息列表缓存键值

        public static readonly string QUOTATION_KEY = "cache-quotation"; //报价单缓存键值

        public static readonly string GLOBALSETTING_KEY = "cache-globalsetting"; //系统全局设置缓存键值

        public static readonly string WEIXINAPPID = "wx0c9b37c9d5ddf8a8";   //微信appid
        public static readonly string WEIXINSECRET = "f6e1f096a7e847e9775b1cc64e713a33";    //微信密钥
        public static readonly string WEIXINACCESS_TOKEN_KEY = "access_token"; //微信access_token 键值

        public static readonly string WEIXINOPENID_SESSIONKEY = "session-weixinopenid";   //微信openid的session键值

        public static readonly string BENZVOTEPOTHUNTER_LIST = "cache-benzvotepothunter-list";   //奔驰投票活动选手列表缓存键值
        public static readonly string BENZVOTE_LIST = "cache-benzvote-list";   //奔驰投票活动所有投票信息缓存键值
        public static readonly string BENZVOTESETTING = "cache-benzvotesetting"; //奔驰投票活动设置缓存键值

    }
}
