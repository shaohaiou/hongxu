
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Collections.Specialized;
using System.Collections;
using Hx.Components.Entity;
using Hx.Tools;
using Hx.Tools.Web;
using System.Web.Security;
using Hx.Components.Config;
using Hx.Components.Enumerations;

namespace Hx.Components.Web
{
    public class JCBContext
    {
        private HybridDictionary _items = new HybridDictionary();
        private NameValueCollection _queryString = null;
        HttpContext _httpContext = null;
        private string _message = null;                                             //传递的消息
        private string _returnUrl = null;                                           //返回页面的url
        private string _messageTitle = null;                                        //返回页面的标题
        private string _returnTitle = null;                                         //返回按钮的文本
        private ShowUrlCollection _showUrl = new ShowUrlCollection();               //当前上下文传递的跳转url
        private UserPrincipal _user;                                                //当前用户标示
        private CommConfig commconfig = CommConfig.GetConfig();                     //当前配置信息
        private int bot = -1;                                                       //是否是蜘蛛爬行
        private static JCBContext _context = null;
        private UrlRuleType _urlRule = UrlRuleType.NoSet;
        private string _ip = null;
        private string _userkey = null;

        /// <summary>
        /// 当前登陆用户
        /// </summary>
        [JsonProperty("user")]
        public UserPrincipal User
        {
            get { return _user; }
            set { _user = value; }
        }
        [JsonIgnore]
        public string UserKey
        {
            get
            {
                if (string.IsNullOrEmpty(_userkey))
                {
                    //_userkey = (Context.Session == null || Context.Session[GlobalKey.MACHINEKEY_COOKIENAME] == null) ? string.Empty : Context.Session[GlobalKey.MACHINEKEY_COOKIENAME].ToString();
                    //if (string.IsNullOrEmpty(_userkey))
                    //{
                    //    _userkey = WebUtils.GetMachineKey(GlobalKey.MACHINEKEY_COOKIENAME);
                    //    if (Context.Session != null)
                    //        Context.Session[GlobalKey.MACHINEKEY_COOKIENAME] = _userkey;
                    //}
                    _userkey = GetMachineKey(GlobalKey.MACHINEKEY_COOKIENAME);
                    if (!string.IsNullOrEmpty(_userkey))
                    {
                        _userkey = _userkey.Replace(" ", "");
                    }
                }
                return _userkey;
            }
        }
        [JsonIgnore]
        public string IP
        {
            get
            {
                if (string.IsNullOrEmpty(_ip))
                {
                    _ip = WebHelper.GetClientsIP();
                }
                if (!string.IsNullOrEmpty(_ip))
                {
                    _ip = _ip.Replace(" ", "");
                }
                return _ip;
            }
        }

        //判断是否是蜘蛛爬行
        [JsonIgnore]
        public bool IsBot
        {
            get
            {
                if (bot == -1)
                {
                    if (HttpContext.Current == null)
                    {
                        return false;
                    }
                    string botstr = Utils.GetBot(HttpContext.Current);
                    if (!string.IsNullOrEmpty(botstr))
                    {
                        bot = 1;
                    }
                    else
                    {
                        bot = 0;
                    }
                }

                if (bot == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// 当前用户名
        /// </summary>
        [JsonIgnore]
        public string UserName
        {
            get
            {
                if (IsAuthenticated && User != null)
                {
                    return User.UserName;
                }
                else
                {
                    if (User != null)
                    {
                        return User.UserName;
                    }
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 当前用户ID
        /// </summary>
        [JsonIgnore]
        public int UserID
        {
            get
            {
                if (IsAuthenticated && User != null)
                {
                    return User.UserID;
                }
                else
                {
                    if (User != null)
                    {
                        return User.UserID;
                    }
                    return 0;
                }
            }
        }

        private JcbUserInfo _admin = null;
        /// <summary>
        /// 后台管理员
        /// </summary>
        [JsonIgnore]
        public JcbUserInfo AdminUser
        {
            get
            {
                if (_admin == null)
                {
                    _admin = _httpContext.Session[GlobalKey.SESSION_JCBUSER] as JcbUserInfo;
#if DEBUG
                    if (_admin == null)
                    {
                        _admin = new JcbUserInfo()
                        {
                            ID = 1,
                            Administrator = true,
                            Mobile = "13515871286",
                            Name = "红旭集团",
                            UserName = "hxjt"
                        };
                        _httpContext.Session[GlobalKey.SESSION_JCBUSER] = _admin;
                    }
#else
                    if (_admin == null && ManageCookies.GetCookie(GlobalKey.SESSION_JCBUSER) != null)
                    {
                        int id = DataConvert.SafeInt(ManageCookies.GetCookie(GlobalKey.SESSION_JCBUSER).Value);
                        if (id > 0)
                        {
                            JcbUserInfo admin = JcbUsers.Instance.GetUserInfo(id);
                            if (admin != null)
                            {
                                _httpContext.Session[GlobalKey.SESSION_JCBUSER] = admin;
                                _admin = admin;
                            }
                        }
                    }
#endif
                }
                return _admin;
            }
            set
            {
                _admin = value;
                _httpContext.Session[GlobalKey.SESSION_JCBUSER] = _admin;
            }
        }

        /// <summary>
        /// 后台管理员ID
        /// </summary>
        [JsonIgnore]
        public int AdminUserID
        {
            get
            {
                if (AdminUser != null)
                {
                    return AdminUser.ID;
                }
                return 0;
            }
        }

        /// <summary>
        /// 是否是后台管理网站
        /// </summary>
        [JsonIgnore]
        public bool IsAdminSite
        {
            get
            {
                if (DataConvert.SafeInt(commconfig.AppSetting["SiteID"]) < 1)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 后台管理员名
        /// </summary>
        [JsonIgnore]
        public string AdminUserName
        {
            get
            {
                if (AdminUser != null)
                {
                    return AdminUser.UserName;
                }
                return string.Empty;

            }
        }

        private bool HostInSetting(string host, string sets)
        {
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(sets))
            {
                return false;
            }
            string[] sarr = sets.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in sarr)
            {
                if (host.EndsWith(s))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 需要显示的链接
        /// </summary>
        [JsonProperty("showurl")]
        public ShowUrlCollection ShowUrl
        {
            get { return _showUrl; }
            set { _showUrl = value; }
        }

        private JCBContext()
        {
        }

        private JCBContext(HttpContext context)
        {
            try
            {
                this._httpContext = context;
                _queryString = _httpContext.Request.QueryString;
            }
            catch
            {
                this._httpContext = null;
            }
        }

        /// <summary>
        /// 当前上下文保存下
        /// </summary>
        [JsonIgnore]
        public IDictionary Items
        {
            get { return _items; }
        }
        [JsonProperty("items[key]")]
        public object this[string key]
        {
            get
            {
                return this.Items[key];
            }
            set
            {
                this.Items[key] = value;
            }
        }

        /// <summary>
        /// 请求字符串
        /// </summary>
        [JsonIgnore]
        public NameValueCollection QueryString
        {
            get { return _queryString; }
        }
        [JsonIgnore]
        public HttpContext Context
        {
            get
            {
                return _httpContext;
            }
        }
        [JsonIgnore]
        public static JCBContext Current
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    if (_context == null)
                    {
                        _context = new JCBContext();
                    }
                    return _context;
                }
                HttpContext httpContext = HttpContext.Current;
                JCBContext context = null;
                context = httpContext.Items[GlobalKey.JCBCONTEXT_KEY] as JCBContext;
                if (context == null)
                {
                    context = new JCBContext(httpContext);
                    SaveContextToStore(context);
                }
                return context;
            }
        }

        private string _currenthost = null;
        [JsonIgnore]
        public string CurrentHost
        {
            get
            {
                if (string.IsNullOrEmpty(_currenthost))
                {
                    _currenthost = "hongxu.com";
                }
                return _currenthost;
            }
        }

        /// <summary>
        /// 保存当前项
        /// </summary>
        /// <param name="context"></param>
        private static void SaveContextToStore(JCBContext context)
        {
            if (context.Context != null)
            {
                context.Context.Items[GlobalKey.JCBCONTEXT_KEY] = context;
            }
        }

        /// <summary>
        /// 当前消息
        /// </summary>
        [JsonProperty("message")]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// 返回Url的文本
        /// </summary>
        [JsonProperty("returntitle")]
        public string ReturnTitle
        {
            get { return _returnTitle; }
            set { _returnTitle = value; }
        }

        /// <summary>
        /// 返回的Url
        /// </summary>
        [JsonProperty("returnurl")]
        public string ReturnUrl
        {
            get { return _returnUrl; }
            set { _returnUrl = value; }
        }

        /// <summary>
        /// 当前用户是否经过前台验证
        /// </summary>
        [JsonIgnore]
        public bool IsAuthenticated
        {
            get
            {
                return _httpContext.Request.IsAuthenticated;
            }
        }

        [JsonIgnore]
        public bool UserCheck
        {
            get
            {
                if (_httpContext.Session[GlobalKey.SESSION_JCBUSER] != null || AdminUser != null)
                {
                    return true;
                }
                else if (ManageCookies.GetCookie(GlobalKey.SESSION_JCBUSER) != null)
                {
                    int id = DataConvert.SafeInt(ManageCookies.GetCookie(GlobalKey.SESSION_JCBUSER).Value);
                    if (id > 0)
                    {
                        JcbUserInfo admin = JcbUsers.Instance.GetUserInfo(id);
                        if (admin != null)
                        {
                            _httpContext.Session[GlobalKey.SESSION_JCBUSER] = admin;
                            return true;
                        }
                    }
                }
                return false;

            }
        }

        /// <summary>
        /// 消息标题
        /// </summary>
        [JsonProperty("messagetitle")]
        public string MessageTitle
        {
            get { return _messageTitle; }
            set { _messageTitle = value; }
        }

        private string _cookiedomain = null;

        /// <summary>
        /// 站点cookie域
        /// </summary>
        [JsonIgnore]
        public string CookieDomain
        {
            get
            {
                if (_cookiedomain == null)
                {
                    if (string.IsNullOrEmpty(_cookiedomain))
                    {
                        _cookiedomain = FormsAuthentication.CookieDomain;
                    }
                }
                return _cookiedomain;
            }
        }

        ///// <summary>
        ///// 当前链接是否来源于当前域的链接
        ///// </summary>
        //public bool IsCurrentHostUrl
        //{
        //    get
        //    {
        //        string str = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
        //        if (string.IsNullOrEmpty(str) || str.IndexOf("." + CurrentHost) > 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }

        //}

        /// <summary>
        /// 是否是苹果或者安卓系统访问
        /// </summary>
        [JsonIgnore]
        public bool IsIOS
        {
            get
            {
                bool flag = false;
                try
                {
                    string agent = HttpContext.Current.Request.UserAgent;
                    string[] keywords = { "Android", "iPhone", "iPod", "iPad" };

                    foreach (string item in keywords)
                    {
                        if (agent.Contains(item))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                catch { }

                return flag;
            }
        }
        [JsonIgnore]
        public bool IsIE
        {
            get
            {
                bool flag = false;
                try
                {
                    string agent = HttpContext.Current.Request.UserAgent;
                    string[] keywords = { "MSIE" };

                    foreach (string item in keywords)
                    {
                        if (agent.Contains(item))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                catch { }

                return flag;
            }
        }
        [JsonProperty("isusealternatedomain")]
        public bool IsUseAlternateDomain { get; set; }





        /// <summary>
        /// 获取机器唯一码
        /// </summary>
        /// <param name="key">唯一码命名</param>
        /// <returns></returns>
        public string GetMachineKey(string key)
        {
            string hitkey = ManageCookies.GetCookieValue(key);
            if (string.IsNullOrEmpty(hitkey))
            {
                hitkey = Guid.NewGuid().ToString();
                ManageCookies.CreateCookie(key, hitkey, true, DateTime.Now.AddDays(1), CookieDomain, true);
            }

            return hitkey;
        }
        /// <summary>
        /// uuu
        /// </summary>
        [JsonIgnore]
        public UrlRuleType UrlRuleType
        {
            get
            {
                if (_urlRule == Enumerations.UrlRuleType.NoSet)
                    _urlRule = Enumerations.UrlRuleType.Default;
                return _urlRule;
            }
            set
            {
                _urlRule = value;
            }
        }


        private GlobalSettingInfo globalsetting = null;
        [JsonIgnore]
        public GlobalSettingInfo GlobalSetting
        {
            get
            {
                if (globalsetting == null)
                    globalsetting = GlobalSettings.Instance.GetModel(true);
                return globalsetting;
            }
        }
    }
}
