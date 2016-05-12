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
        public static readonly string CHOICESTGOODS_LIST = "cache-choicestgoods-list"; //精品用品缓存键值
        public static readonly string CORPORATION_LIST = "cache-corporation-list"; //公司列表缓存键值
        public static readonly string DAYREPORTUSER_LIST = "cache-dayreportuser-list"; //日报用户列表缓存键值
        public static readonly string DAILYREPORT_LIST = "cache-dailyreport-list"; //日报列表缓存键值
        public static readonly string PERSONALDATA_LIST = "cache-personaldata-list"; //现有资料列表缓存键值
        public static readonly string MONTHTARGET_LIST = "cache-monthtarget-list"; //月度目标列表缓存键值
        public static readonly string DAILYREPORTMODULE_LIST = "cache-dailyreportmodule-list"; //日报模块列表缓存键值
        public static readonly string JOBOFFER_LIST = "cache-joboffer-list"; //招聘信息列表缓存键值
        public static readonly string CORPMIEN_LIST = "cache-corpmien-list"; //企业风采列表缓存键值
        public static readonly string CRMREPORT_LIST = "cache-crmreport-list"; //crm报表列表缓存键值

        public static readonly string QUOTATION_KEY = "cache-quotation"; //报价单缓存键值

        public static readonly string GLOBALSETTING_KEY = "cache-globalsetting"; //系统全局设置缓存键值

        public static readonly string WEIXINAPPID = "wx0c9b37c9d5ddf8a8";   //微信appid
        public static readonly string WEIXINSECRET = "f6e1f096a7e847e9775b1cc64e713a33";    //微信密钥
        public static readonly string WEIXINACCESS_TOKEN_KEY = "access_token"; //微信access_token 键值
        public static readonly string WEIXINOPENID_SESSIONKEY = "session-weixinopenid";   //微信openid的session键值
        public static readonly string WEIXINJSAPI_TICKET_KEY = "jsapi_ticket"; //微信jsapi_ticket 缓存键值
        public static readonly string WEIXINCARDAPI_TICKET_KEY = "cardapi_ticket"; //微信卡券api_ticket 缓存键值

        public static readonly string BENZVOTEPOTHUNTER_LIST = "cache-benzvotepothunter-list";   //奔驰投票活动选手列表缓存键值
        public static readonly string BENZVOTE_LIST = "cache-benzvote-list";   //奔驰投票活动所有投票信息缓存键值
        public static readonly string BENZVOTESETTING = "cache-benzvotesetting"; //奔驰投票活动设置缓存键值
        public static readonly string BENZVOTEOPENID = "session-benzvoteopenid"; //奔驰投票活动openid键值

        public static readonly string JITUANVOTEPOTHUNTER_LIST = "cache-jituanvotepothunter-list";   //集团投票活动选手列表缓存键值
        public static readonly string JITUANVOTE_LIST = "cache-jituanvote-list";   //集团投票活动所有投票信息缓存键值
        public static readonly string JITUANVOTESETTING = "cache-jituanvotesetting"; //集团投票活动设置缓存键值

        public static readonly string CARDSETTINGLIST = "cache-cardsetting-list"; //卡券活动设置列表缓存键值
        public static readonly string CARDLIST = "cache-card-list"; //卡券列表缓存键值
        public static readonly string CARDPULLLIST = "cache-cardpull-list"; //卡券抽奖记录列表缓存键值
        public static readonly string CARDIDLIST = "cache-cardid-list"; //卡券抽奖记录列表缓存键值

        public static readonly string VOTESETTINGLIST = "cache-votesetting-list"; //投票活动设置列表缓存键值
        public static readonly string VOTEOPENID = "session-voteopenid"; //投票活动openid键值
        public static readonly string VOTELIST = "cache-vote-list"; //投票列表缓存键值
        public static readonly string VOTERECORDLIST = "cache-voterecord-list"; //投票记录列表缓存键值
        public static readonly string VOTERECORDLISTCACHE = "cache-voterecordcache-list"; //投票记录临时列表缓存键值
        public static readonly string VOTEPOTHUNTERLIST = "cache-votepothunter-list"; //投票记录列表缓存键值
        public static readonly string VOTECOMMENT_LIST = "cache-votecomment-list"; //微信活动评论列表缓存键值

        public static readonly string SCENECODESETTINGLIST = "cache-scenecodesetting-list"; //场景二维码设置列表缓存键值
        public static readonly string SCENECODELIST = "cache-scenecode-list"; //场景列表缓存键值


        public static readonly string WEIXINACTCOMMENT_LIST = "cache-weixincomment-list"; //微信活动评论列表缓存键值

        public static readonly string PROMARY_LIST = "cache-promary-list"; //省份缓存键值
        public static readonly string CITY_LIST = "cache-city-list"; //市缓存键值
        public static readonly string JCBCAR_LIST = "cache-jcbcar-list"; //集车宝车辆信息列表缓存键值
        public static readonly string JCBACCOUNT_LIST = "cache-jcbaccount-list"; //集车宝账户信息列表缓存键值
        public static readonly string JCBMARKETRECORD_LIST = "cache-jcbmarketrecord-list"; //集车宝营销记录信息列表缓存键值
        public static readonly string SESSION_JCBUSER = "Session_JcbUser";//jcb用户session
        public static readonly string JCBCONTEXT_KEY = "JCBContext";               //JCB当前上下文键值
    }
}
