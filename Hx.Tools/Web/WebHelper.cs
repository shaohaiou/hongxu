using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Management;
using System.Configuration;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Hx.Tools.Web
{
    public class WebHelper
    {
        #region 网络操作

        /// <summary>
        /// 获取用户真实IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientsIP()
        {

            string ip = ManageCookies.GetCookieValue("test_ip_cookie");
            if (!string.IsNullOrEmpty(ip))
            {
                return ip;
            }

            try
            {
                HttpRequest request = HttpContext.Current.Request;

                if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_ARR_IP"]) && !string.IsNullOrEmpty(request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
                {
                    ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
                }

                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(ip))
                {
                    ip = request.UserHostAddress;
                }
            }
            catch
            {
                return string.Empty;
            }

            return ip;
        }

        /// <summary>
        /// 是否使用了代理
        /// </summary>
        /// <returns></returns>
        public static bool IsProxy()
        {
            try
            {
                HttpRequest request = HttpContext.Current.Request;

                if (!string.IsNullOrEmpty(request.ServerVariables["HTTP_VIA"]))
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string GetBrowserLanguage()
        {
            string userlanugages = null;

            string langeuage = ManageCookies.GetCookieValue("test_language_cookie");
            if (!string.IsNullOrEmpty(langeuage))
            {
                return langeuage;
            }

            if (HttpContext.Current.Request.UserLanguages != null && HttpContext.Current.Request.UserLanguages.Length > 0)
            {
                userlanugages = HttpContext.Current.Request.UserLanguages[0];
            }
            return userlanugages;
        }

        /// <summary>
        /// 获取用户代理IP
        /// </summary>
        /// <returns></returns>
        public static string GetViaIP()
        {
            string viaIp = "";

            try
            {
                HttpRequest request = HttpContext.Current.Request;

                if (request.ServerVariables["HTTP_VIA"] != null)
                {
                    viaIp = request.UserHostAddress;
                }

            }
            catch (Exception e)
            {

                throw e;
            }

            return viaIp;
        }

        /// <summary>
        /// 获取服务器IP
        /// </summary>
        /// <returns></returns>
        public static string GetServerIP()
        {
            string stringIP = string.Empty;
            ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection MOC = MC.GetInstances();
            foreach (ManagementObject MO in MOC)
            {
                if ((bool)MO["IPEnabled"] == true)
                {
                    string[] IPAddresses = (string[])MO["IPAddress"];
                    if (IPAddresses.Length > 0)
                        stringIP = IPAddresses[0];
                }
            }
            return stringIP;
        }

        /// <summary>
        /// 获取服务器IP
        /// </summary>
        /// <returns></returns>
        public static string[] GetServerIPs()
        {

            ManagementClass MC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection MOC = MC.GetInstances();
            foreach (ManagementObject MO in MOC)
            {
                if ((bool)MO["IPEnabled"] == true)
                {
                    string[] IPAddresses = (string[])MO["IPAddress"];
                    if (IPAddresses.Length > 0)
                    {
                        return IPAddresses;
                    }
                }
            }
            return new string[0];
        }
        #endregion

        #region 其他相关

        /// <summary>
        /// 获取Web请求对象
        /// </summary>
        /// <returns></returns>
        public static HttpContext GetContext()
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                throw new Exception("HttpContext not found");
            }
            return context;
        }

        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
                return "";

            return HttpContext.Current.Request.ServerVariables[strName].ToString();
        }

        ///// <summary>
        /// 返回没有页面错误
        /// </summary>
        /// <param name="Context"></param>
        public static void Return404(HttpContext Context, string reurl)
        {
            string btype = HttpContext.Current.Request.Browser.Type;
            string url = Context.Request.RawUrl;
            if (!string.IsNullOrEmpty(btype) && btype != "Unknown" && url.IndexOf("undefined&op") < 0)
            {
                string path = Utils.GetMapPath("~/errorlog.txt");
                StreamWriter sw = null;
                if (File.Exists(path))
                {
                    sw = File.AppendText(path);
                }
                else
                {
                    sw = File.CreateText(path);
                }
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "                      :webhelper  404错误-访问页面-" + url + "   IP:" + WebHelper.GetClientsIP() + " 浏览器：" + HttpContext.Current.Request.Browser.Type);
                sw.Flush();
                sw.Close();
            }
            Context.Response.Clear();
            Context.Response.StatusCode = 404;
            Context.Response.Redirect(reurl);
        }

        /// <summary>
        /// 设置输出流不缓存
        /// </summary>
        public static void SetNotCache()
        {
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1.0);
            HttpContext.Current.Response.Expires = 0;
            HttpContext.Current.Response.CacheControl = "no-cache";
            HttpContext.Current.Response.AppendHeader("Pragma", "No-Cache");
        }

        /// <summary>
        /// 设置客户端缓存状态
        /// </summary>
        /// <param name="response"></param>
        /// <param name="lastModified"></param>
        //private void SetClientCaching(DateTime lastModified)
        //{
        //HttpContext.Current.Response.Cache.SetETag(lastModified.Ticks.ToString());
        //HttpContext.Current.Response.Cache.SetLastModified(lastModified);
        ////public 以指定响应能由客户端和共享（代理）缓存进行缓存。
        //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
        ////是允许文档在被视为陈旧之前存在的最长绝对时间。
        //HttpContext.Current.Response.Cache.SetMaxAge(new TimeSpan(0, 2, 0, 0));
        ////将缓存过期从绝对时间设置为可调时间
        //HttpContext.Current.Response.Cache.SetSlidingExpiration(true);
        //}

        /// <summary>
        /// 读取配置文件中AppSettings的信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 添加页面脚本
        /// </summary>
        /// <param name="script"></param>
        public static void AppendScript(string script)
        {
            if (HttpContext.Current.Handler is Page)
            {
                Page page = (Page)HttpContext.Current.Handler;
                page.ClientScript.RegisterStartupScript(page.GetType(), page.ToString(), string.Format("<script type='text/javascript'>{0}</script>", script));
            }
        }

        /// <summary>
        /// 页面显示消息
        /// </summary>
        /// <param name="meg">需要显示的消息</param>
        /// <param name="isreload">是否重新加载页面</param>
        public static void ShowMessage(string meg, bool isreload)
        {
            if (HttpContext.Current.Handler is Page)
            {
                string reloadjs = string.Empty;
                Page page = (Page)HttpContext.Current.Handler;
                if (isreload)
                {
                    reloadjs = "window.location.href='" + HttpContext.Current.Request.RawUrl + "'";
                }
                page.ClientScript.RegisterStartupScript(page.GetType(), page.ToString(), string.Format("<script type='text/javascript'>alert(\"{0}\");{1}</script>", meg, reloadjs));
            }
        }


        /// <summary>
        /// 页面显示消息
        /// </summary>
        /// <param name="meg">需要显示的消息</param>
        public static void ShowMessage(string meg)
        {
            ShowMessage(meg, true);
        }

        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet()
        {
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            string curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (curBrowser.IndexOf(BrowserName[i]) >= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否是跨站提交
        /// </summary>
        /// <param name="urlReferrer">上个页面地址</param>
        /// <param name="host">论坛url</param>
        /// <returns>bool</returns>
        public static bool IsCrossSitePost(string urlReferrer, string host)
        {
            if (urlReferrer.Length < 7)
                return true;

            Uri u = new Uri(urlReferrer);

            ////如果使用负载均衡方案布署站点，则使用memcache.config中的站表列表来(SyncCacheUrl)来做'跨站提交'的判断
            ///不过目前nginx已将u.Host改成了本地站点，所以其得到的urlReferrer与host是同一站点，所以这里将下面代码段注释
            //if (MemCachedConfigs.GetConfig() != null && MemCachedConfigs.GetConfig().ApplyMemCached)
            //{
            //    System.Web.HttpContext.Current.Response.Write("uhost:" + u.Host);//uhost:124.207.144.194 
            //    return MemCachedConfigs.GetConfig().SyncCacheUrl.IndexOf(u.Host) < 0; 
            //}
            //else
            return u.Host != host;
        }

        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpContext.Current.Request.UrlReferrer == null)
                return false;

            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
            string tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
            for (int i = 0; i < SearchEngine.Length; i++)
            {
                if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                    return true;
            }
            return false;
        }

        #endregion

        #region 请求参数
        /// <summary>
        /// 返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount()
        {
            return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
        }

        #endregion

        #region 请求方式
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public static bool IsPost()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("POST");
        }

        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public static bool IsGet()
        {
            return HttpContext.Current.Request.HttpMethod.Equals("GET");
        }

        #endregion

        #region ListControl控件方法

        #region 设置ListControl选中
        /// <summary>
        /// 设置ListControl选中项
        /// </summary>
        /// <param name="listControl">ListControl控件</param>
        /// <param name="selectValue">选中的Value</param>
        public static void SetSelectedByValue(ListControl listControl, string selectValue)
        {
            listControl.ClearSelection();
            ListItem li = listControl.Items.FindByValue(selectValue);
            if (li != null)
            {
                li.Selected = true;
            }
        }

        /// <summary>
        /// 设置ListControl选中项
        /// </summary>
        /// <param name="listControl">ListControl控件</param>
        /// <param name="selectText">选中的Text</param>
        public static void SetSelectedByText(ListControl listControl, string selectText)
        {
            listControl.ClearSelection();
            ListItem li = listControl.Items.FindByText(selectText);
            if (li != null)
            {
                li.Selected = true;
            }
        }
        /// <summary>
        /// 设置ListControl选中项
        /// </summary>
        /// <param name="listControl">ListControl控件</param>
        /// <param name="selectValue">选中的Value</param>
        /// <param name="separator">分隔符</param>
        public static void SetMultSelectedByValue(ListControl listControl, string selectValue, string separator)
        {
            listControl.ClearSelection();
            string[] vals = selectValue.Split(new string[]{ separator}, StringSplitOptions.RemoveEmptyEntries);
            foreach (ListItem li in listControl.Items)
            {
                li.Selected = vals.Contains(li.Value);
            }
        }

        /// <summary>
        /// 设置ListControl选中项
        /// </summary>
        /// <param name="listControl">ListControl控件</param>
        /// <param name="selectText">选中的Text</param>
        /// <param name="separator">分隔符</param>
        public static void SetMultSelectedByText(ListControl listControl, string selectText,string separator)
        {
            listControl.ClearSelection();
            string[] vals = selectText.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (ListItem li in listControl.Items)
            {
                li.Selected = vals.Contains(li.Text);
            }
        }

        #endregion

        #region 移除ListControl种的项
        /// <summary>
        /// 从列表控件里删除符合指定值的项
        /// </summary>
        /// <param name="listControl">列表控件</param>
        /// <param name="selectValue">选中项</param>
        public static void RemoveListItemByValue(ListControl listControl, string selectValue)
        {
            ListItem li = listControl.Items.FindByValue(selectValue);
            if (li != null)
            {
                listControl.Items.Remove(li);
            }
        }

        /// <summary>
        /// 从列表控件里删除符合指定值的项
        /// </summary>
        /// <param name="listControl">列表控件</param>
        /// <param name="selectValue">选中项</param>
        public static void RemoveListItemByText(ListControl listControl, string selectValue)
        {
            ListItem li = listControl.Items.FindByText(selectValue);
            if (li != null)
            {
                listControl.Items.Remove(li);
            }
        }

        #endregion

        #region ListControl多选

        /// <summary>
        /// 获得ListControl控件的多个选中的Value值
        /// </summary>
        /// <param name="cbl">ListControl控件</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string GetListControlMoreValue(ListControl cbl, string separator)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;
            foreach (ListItem li in cbl.Items)
            {

                if (li.Selected)
                {
                    if (i != 0)
                    {
                        result.Append(separator);
                    }
                    result.Append(li.Value);
                    i++;
                }

            }
            return result.ToString();
        }

        /// <summary>
        /// 获得ListControl控件的多个选中的Text值
        /// </summary>
        /// <param name="cbl">ListControl控件</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static string GetListControlMoreText(ListControl cbl, string separator)
        {
            StringBuilder result = new StringBuilder();
            int i = 0;
            foreach (ListItem li in cbl.Items)
            {

                if (li.Selected)
                {
                    if (i != 0)
                    {
                        result.Append(separator);
                    }
                    result.Append(li.Text);
                    i++;
                }

            }
            return result.ToString();
        }
        #endregion

        #endregion

        #region 编码
        /// <summary>
        /// 将html编码解码
        /// </summary>
        /// <param name="textToFormat">需要解码的html代码</param>
        /// <returns>解码后的字符串</returns>
        public static string HtmlDecode(String textToFormat)
        {
            if (string.IsNullOrEmpty(textToFormat))
                return textToFormat;
            return System.Web.HttpUtility.HtmlDecode(textToFormat);
        }

        /// <summary>
        /// 将字符串编码为html编码
        /// </summary>
        /// <param name="textToFormat">需要编码的html</param>
        /// <returns>编码后的字符串</returns>
        public static string HtmlEncode(String textToFormat)
        {
            if (string.IsNullOrEmpty(textToFormat))
                return textToFormat;
            return System.Web.HttpUtility.HtmlEncode(textToFormat);
        }

        /// <summary>
        /// 将url编码
        /// </summary>
        /// <param name="urlToEncode">需要编码的url</param>
        /// <returns>编码后的url</returns>
        public static string UrlEncode(string urlToEncode)
        {
            if (string.IsNullOrEmpty(urlToEncode))
                return urlToEncode;

            return System.Web.HttpUtility.UrlEncode(urlToEncode).Replace("'", "%27");

        }

        /// <summary>
        /// 将url解码
        /// </summary>
        /// <param name="urlToDecode">需要解码的url</param>
        /// <returns>解码后的url</returns>
        public static string UrlDecode(string urlToDecode)
        {
            if (string.IsNullOrEmpty(urlToDecode))
                return urlToDecode;

            return System.Web.HttpUtility.UrlDecode(urlToDecode);
        }

        #endregion

        #region 添加url查询
        /// <summary>
        /// 给现有url添加查询参数
        /// </summary>
        /// <param name="url">现有的url</param>
        /// <param name="querystring">查询的字符串</param>
        /// <returns>连接后的url</returns>
        public static string AppendQuerystring(string url, string querystring)
        {
            return AppendQuerystring(url, querystring, false);
        }

        /// <summary>
        /// 给现有url添加查询参数
        /// </summary>
        /// <param name="url">现有的url</param>
        /// <param name="querystring">查询的字符串</param>
        /// <param name="urlEncoded">url是否已经编码</param>
        /// <returns>连接后的url</returns>
        public static string AppendQuerystring(string url, string querystring, bool urlEncoded)
        {
            string seperator = "?";
            if (url.IndexOf('?') > -1)
            {
                if (!urlEncoded)
                    seperator = "&";
                else
                    seperator = "&amp;";
            }
            return string.Concat(url, seperator, querystring);
        }

        /// <summary>
        /// 对当前Url添加查询
        /// </summary>
        /// <param name="name">查询键</param>
        /// <param name="value">查询值</param>
        /// <returns>完整的url</returns>
        public static string AppendQueryToUrl(string name, string value)
        {
            HttpRequest Request = HttpContext.Current.Request;
            NameValueCollection nv = new NameValueCollection(Request.QueryString);
            nv[name] = value;
            StringBuilder sb = new StringBuilder(Request.Path);
            int i = 0;
            foreach (string key in nv.Keys)
            {
                if (!string.IsNullOrEmpty(nv[key]))
                {
                    if (i == 0)
                    {
                        sb.AppendFormat("?{0}={1}", key, WebHelper.UrlEncode(nv[key]));
                    }
                    else
                    {
                        sb.AppendFormat("&{0}={1}", key, WebHelper.UrlEncode(nv[key]));
                    }
                    i++;
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Url相关
        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());

            return request.Url.Host;
        }

        /// <summary>
        /// 得到主机头
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns>上一个页面的地址</returns>
        public static string GetUrlReferrer()
        {
            string retVal = null;

            try
            {
                retVal = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            catch { }

            if (retVal == null)
                return "";

            return retVal;
        }

        /// <summary>
        /// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// </summary>
        /// <returns>原始 URL</returns>
        public static string GetRawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }

        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }

        /// <summary>
        /// 获取根路径
        /// </summary>
        /// <returns></returns>
        public static string GetBasePath()
        {
            return VirtualPathUtility.AppendTrailingSlash(HttpContext.Current.Request.ApplicationPath);
        }

        /// <summary>
        /// 获取站点地址
        /// </summary>
        /// <returns></returns>
        public static string GetFullBasePath()
        {
            HttpRequest request = HttpContext.Current.Request;
            return (request.Url.AbsoluteUri.Replace(request.Url.PathAndQuery, string.Empty) + GetBasePath());
        }

        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string GetPageName()
        {
            string[] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
            return urlArr[urlArr.Length - 1].ToLower();
        }

        /// <summary>
        /// 获取真实路径
        /// </summary>
        /// <param name="path">虚拟路径</param>
        /// <returns></returns>
        public static string MapPath(string path)
        {
            HttpContext context = HttpContext.Current;
            return context.Server.MapPath(path);
        }

        #endregion

        #region 输出响应文件

        /// <summary>
        /// 以指定的ContentType输出指定文件文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filename">输出的文件名</param>
        /// <param name="filetype">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filepath, string filename, string filetype)
        {
            Stream iStream = null;

            // 缓冲区为10k
            byte[] buffer = new Byte[10000];
            // 文件长度
            int length;
            // 需要读的数据长度
            long dataToRead;

            try
            {
                // 打开文件
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                // 需要读的数据长度
                dataToRead = iStream.Length;

                HttpContext.Current.Response.ContentType = filetype;
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + UrlEncode(filename.Trim()).Replace("+", " "));

                while (dataToRead > 0)
                {
                    // 检查客户端是否还处于连接状态
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        // 如果不再连接则跳出死循环
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    // 关闭文件
                    iStream.Close();
                }
            }
            HttpContext.Current.Response.End();
        }

        #endregion

        #region 请求参数以及Form中的参数

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName)
        {
            return GetQueryString(strName, false);
        }

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName, bool sqlSafeCheck)
        {
            try
            {
                if (HttpContext.Current.Request.QueryString[strName] == null)
                    return "";

                if (sqlSafeCheck && !StrHelper.IsSafeSqlString(HttpContext.Current.Request.QueryString[strName]))
                    return "unsafe string";

                return HttpContext.Current.Request.QueryString[strName];
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName)
        {
            return GetFormString(strName, false);
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName, bool sqlSafeCheck)
        {
            try
            {
                if (HttpContext.Current.Request.Form[strName] == null)
                    return "";

                if (sqlSafeCheck && !StrHelper.IsSafeSqlString(HttpContext.Current.Request.Form[strName]))
                    return "unsafe string";

                return HttpContext.Current.Request.Form[strName];
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName)
        {
            return GetString(strName, false);
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName, bool sqlSafeCheck)
        {
            if (string.IsNullOrEmpty(GetQueryString(strName)))
                return GetFormString(strName, sqlSafeCheck);
            else
                return GetQueryString(strName, sqlSafeCheck);
        }

        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName)
        {
            return GetQueryInt(strName, 0);
        }

        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName, int defValue)
        {
            return DataConvert.SafeInt(HttpContext.Current.Request.QueryString[strName], defValue);
        }

        /// <summary>
        /// 获得指定表单参数的int类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的int类型值</returns>
        public static int GetFormInt(string strName)
        {
            return GetFormInt(strName, 0);
        }

        /// <summary>
        /// 获得指定表单参数的int类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的int类型值</returns>
        public static int GetFormInt(string strName, int defValue)
        {
            return DataConvert.SafeInt(HttpContext.Current.Request.Form[strName], defValue);
        }

        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static int GetInt(string strName)
        {
            return GetInt(strName, 0);
        }

        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static int GetInt(string strName, int defValue)
        {
            if (GetQueryInt(strName, defValue) == defValue)
                return GetFormInt(strName, defValue);
            else
                return GetQueryInt(strName, defValue);
        }

        /// <summary>
        /// 获得指定Url参数的float类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的int类型值</returns>
        public static float GetQueryFloat(string strName)
        {
            return GetQueryFloat(strName, 0);
        }

        /// <summary>
        /// 获得指定Url参数的float类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static float GetQueryFloat(string strName, float defValue)
        {
            return DataConvert.SafeFloat(HttpContext.Current.Request.QueryString[strName], defValue);
        }

        /// <summary>
        /// 获得指定表单参数的float类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的float类型值</returns>
        public static float GetFormFloat(string strName)
        {
            return GetFormFloat(strName, 0);
        }

        /// <summary>
        /// 获得指定表单参数的float类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的float类型值</returns>
        public static float GetFormFloat(string strName, float defValue)
        {
            return DataConvert.SafeFloat(HttpContext.Current.Request.Form[strName], defValue);
        }

        /// <summary>
        /// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static float GetFloat(string strName)
        {
            return GetFloat(strName, 0);
        }

        /// <summary>
        /// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static float GetFloat(string strName, float defValue)
        {
            if (GetQueryFloat(strName, defValue) == defValue)
                return GetFormFloat(strName, defValue);
            else
                return GetQueryFloat(strName, defValue);
        }

        public bool IsUrl()
        {
            string str1 = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
            string str2 = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
            return ((str1 != null) && (str1.IndexOf(str2) == 7));
        }

        #endregion

        public static string HtmlToJs(string source)
        {
            return String.Format("document.writeln(\"{0}\");",
       String.Join("\");\r\ndocument.writeln(\"", source.Replace("\\", "\\\\")
                               .Replace("/", "\\/")
                               .Replace("'", "\\'")
                               .Replace("\"", "\\\"")
                               .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                   )); ;
        }

        #region 获取相应扩展名的ContentType类型
        public static string GetContentType(string fileextname)
        {
            switch (fileextname)
            {
                #region 常用文件类型
                case "jpeg": return "image/jpeg";
                case "jpg": return "image/jpeg";
                case "js": return "application/x-javascript";
                case "jsp": return "text/html";
                case "gif": return "image/gif";
                case "htm": return "text/html";
                case "html": return "text/html";
                case "asf": return "video/x-ms-asf";
                case "avi": return "video/avi";
                case "bmp": return "application/x-bmp";
                case "asp": return "text/asp";
                case "wma": return "audio/x-ms-wma";
                case "wav": return "audio/wav";
                case "wmv": return "video/x-ms-wmv";
                case "ra": return "audio/vnd.rn-realaudio";
                case "ram": return "audio/x-pn-realaudio";
                case "rm": return "application/vnd.rn-realmedia";
                case "rmvb": return "application/vnd.rn-realmedia-vbr";
                case "xhtml": return "text/html";
                case "png": return "image/png";
                case "ppt": return "application/x-ppt";
                case "tif": return "image/tiff";
                case "tiff": return "image/tiff";
                case "xls": return "application/x-xls";
                case "xlw": return "application/x-xlw";
                case "xml": return "text/xml";
                case "xpl": return "audio/scpls";
                case "swf": return "application/x-shockwave-flash";
                case "torrent": return "application/x-bittorrent";
                case "dll": return "application/x-msdownload";
                case "asa": return "text/asa";
                case "asx": return "video/x-ms-asf";
                case "au": return "audio/basic";
                case "css": return "text/css";
                case "doc": return "application/msword";
                case "exe": return "application/x-msdownload";
                case "mp1": return "audio/mp1";
                case "mp2": return "audio/mp2";
                case "mp2v": return "video/mpeg";
                case "mp3": return "audio/mp3";
                case "mp4": return "video/mpeg4";
                case "mpa": return "video/x-mpg";
                case "mpd": return "application/vnd.ms-project";
                case "mpe": return "video/x-mpeg";
                case "mpeg": return "video/mpg";
                case "mpg": return "video/mpg";
                case "mpga": return "audio/rn-mpeg";
                case "mpp": return "application/vnd.ms-project";
                case "mps": return "video/x-mpeg";
                case "mpt": return "application/vnd.ms-project";
                case "mpv": return "video/mpg";
                case "mpv2": return "video/mpeg";
                case "wml": return "text/vnd.wap.wml";
                case "wsdl": return "text/xml";
                case "xsd": return "text/xml";
                case "xsl": return "text/xml";
                case "xslt": return "text/xml";
                case "htc": return "text/x-component";
                case "mdb": return "application/msaccess";
                case "zip": return "application/zip";
                case "rar": return "application/x-rar-compressed";
                #endregion

                case "*": return "application/octet-stream";
                case "001": return "application/x-001";
                case "301": return "application/x-301";
                case "323": return "text/h323";
                case "906": return "application/x-906";
                case "907": return "drawing/907";
                case "a11": return "application/x-a11";
                case "acp": return "audio/x-mei-aac";
                case "ai": return "application/postscript";
                case "aif": return "audio/aiff";
                case "aifc": return "audio/aiff";
                case "aiff": return "audio/aiff";
                case "anv": return "application/x-anv";
                case "awf": return "application/vnd.adobe.workflow";
                case "biz": return "text/xml";
                case "bot": return "application/x-bot";
                case "c4t": return "application/x-c4t";
                case "c90": return "application/x-c90";
                case "cal": return "application/x-cals";
                case "cat": return "application/vnd.ms-pki.seccat";
                case "cdf": return "application/x-netcdf";
                case "cdr": return "application/x-cdr";
                case "cel": return "application/x-cel";
                case "cer": return "application/x-x509-ca-cert";
                case "cg4": return "application/x-g4";
                case "cgm": return "application/x-cgm";
                case "cit": return "application/x-cit";
                case "class": return "java/*";
                case "cml": return "text/xml";
                case "cmp": return "application/x-cmp";
                case "cmx": return "application/x-cmx";
                case "cot": return "application/x-cot";
                case "crl": return "application/pkix-crl";
                case "crt": return "application/x-x509-ca-cert";
                case "csi": return "application/x-csi";
                case "cut": return "application/x-cut";
                case "dbf": return "application/x-dbf";
                case "dbm": return "application/x-dbm";
                case "dbx": return "application/x-dbx";
                case "dcd": return "text/xml";
                case "dcx": return "application/x-dcx";
                case "der": return "application/x-x509-ca-cert";
                case "dgn": return "application/x-dgn";
                case "dib": return "application/x-dib";
                case "dot": return "application/msword";
                case "drw": return "application/x-drw";
                case "dtd": return "text/xml";
                case "dwf": return "application/x-dwf";
                case "dwg": return "application/x-dwg";
                case "dxb": return "application/x-dxb";
                case "dxf": return "application/x-dxf";
                case "edn": return "application/vnd.adobe.edn";
                case "emf": return "application/x-emf";
                case "eml": return "message/rfc822";
                case "ent": return "text/xml";
                case "epi": return "application/x-epi";
                case "eps": return "application/x-ps";
                case "etd": return "application/x-ebx";
                case "fax": return "image/fax";
                case "fdf": return "application/vnd.fdf";
                case "fif": return "application/fractals";
                case "fo": return "text/xml";
                case "frm": return "application/x-frm";
                case "g4": return "application/x-g4";
                case "gbr": return "application/x-gbr";
                case "gcd": return "application/x-gcd";

                case "gl2": return "application/x-gl2";
                case "gp4": return "application/x-gp4";
                case "hgl": return "application/x-hgl";
                case "hmr": return "application/x-hmr";
                case "hpg": return "application/x-hpgl";
                case "hpl": return "application/x-hpl";
                case "hqx": return "application/mac-binhex40";
                case "hrf": return "application/x-hrf";
                case "hta": return "application/hta";
                case "htt": return "text/webviewhtml";
                case "htx": return "text/html";
                case "icb": return "application/x-icb";
                case "ico": return "application/x-ico";
                case "iff": return "application/x-iff";
                case "ig4": return "application/x-g4";
                case "igs": return "application/x-igs";
                case "iii": return "application/x-iphone";
                case "img": return "application/x-img";
                case "ins": return "application/x-internet-signup";
                case "isp": return "application/x-internet-signup";
                case "IVF": return "video/x-ivf";
                case "java": return "java/*";
                case "jfif": return "image/jpeg";
                case "jpe": return "application/x-jpe";
                case "la1": return "audio/x-liquid-file";
                case "lar": return "application/x-laplayer-reg";
                case "latex": return "application/x-latex";
                case "lavs": return "audio/x-liquid-secure";
                case "lbm": return "application/x-lbm";
                case "lmsff": return "audio/x-la-lms";
                case "ls": return "application/x-javascript";
                case "ltr": return "application/x-ltr";
                case "m1v": return "video/x-mpeg";
                case "m2v": return "video/x-mpeg";
                case "m3u": return "audio/mpegurl";
                case "m4e": return "video/mpeg4";
                case "mac": return "application/x-mac";
                case "man": return "application/x-troff-man";
                case "math": return "text/xml";
                case "mfp": return "application/x-shockwave-flash";
                case "mht": return "message/rfc822";
                case "mhtml": return "message/rfc822";
                case "mi": return "application/x-mi";
                case "mid": return "audio/mid";
                case "midi": return "audio/mid";
                case "mil": return "application/x-mil";
                case "mml": return "text/xml";
                case "mnd": return "audio/x-musicnet-download";
                case "mns": return "audio/x-musicnet-stream";
                case "mocha": return "application/x-javascript";
                case "movie": return "video/x-sgi-movie";
                case "mpw": return "application/vnd.ms-project";
                case "mpx": return "application/vnd.ms-project";
                case "mtx": return "text/xml";
                case "mxp": return "application/x-mmxp";
                case "net": return "image/pnetvue";
                case "nrf": return "application/x-nrf";
                case "nws": return "message/rfc822";
                case "odc": return "text/x-ms-odc";
                case "out": return "application/x-out";
                case "p10": return "application/pkcs10";
                case "p12": return "application/x-pkcs12";
                case "p7b": return "application/x-pkcs7-certificates";
                case "p7c": return "application/pkcs7-mime";
                case "p7m": return "application/pkcs7-mime";
                case "p7r": return "application/x-pkcs7-certreqresp";
                case "p7s": return "application/pkcs7-signature";
                case "pc5": return "application/x-pc5";
                case "pci": return "application/x-pci";
                case "pcl": return "application/x-pcl";
                case "pcx": return "application/x-pcx";
                case "pdf": return "application/pdf";
                case "pdx": return "application/vnd.adobe.pdx";
                case "pfx": return "application/x-pkcs12";
                case "pgl": return "application/x-pgl";
                case "pic": return "application/x-pic";
                case "pko": return "application/vnd.ms-pki.pko";
                case "pl": return "application/x-perl";
                case "plg": return "text/html";
                case "pls": return "audio/scpls";
                case "plt": return "application/x-plt";
                case "pot": return "application/vnd.ms-powerpoint";
                case "ppa": return "application/vnd.ms-powerpoint";
                case "ppm": return "application/x-ppm";
                case "pps": return "application/vnd.ms-powerpoint";
                case "pr": return "application/x-pr";
                case "prf": return "application/pics-rules";
                case "prn": return "application/x-prn";
                case "prt": return "application/x-prt";
                case "ps": return "application/x-ps";
                case "ptn": return "application/x-ptn";
                case "pwz": return "application/vnd.ms-powerpoint";
                case "r3t": return "text/vnd.rn-realtext3d";
                case "ras": return "application/x-ras";
                case "rat": return "application/rat-file";
                case "rdf": return "text/xml";
                case "rec": return "application/vnd.rn-recording";
                case "red": return "application/x-red";
                case "rgb": return "application/x-rgb";
                case "rjs": return "application/vnd.rn-realsystem-rjs";
                case "rjt": return "application/vnd.rn-realsystem-rjt";
                case "rlc": return "application/x-rlc";
                case "rle": return "application/x-rle";
                case "rmf": return "application/vnd.adobe.rmf";
                case "rmi": return "audio/mid";
                case "rmj": return "application/vnd.rn-realsystem-rmj";
                case "rmm": return "audio/x-pn-realaudio";
                case "rmp": return "application/vnd.rn-rn_music_package";
                case "rms": return "application/vnd.rn-realmedia-secure";
                case "rmx": return "application/vnd.rn-realsystem-rmx";
                case "rnx": return "application/vnd.rn-realplayer";
                case "rp": return "image/vnd.rn-realpix";
                case "rpm": return "audio/x-pn-realaudio-plugin";
                case "rsml": return "application/vnd.rn-rsml";
                case "rt": return "text/vnd.rn-realtext";
                case "rtf": return "application/msword";
                case "rv": return "video/vnd.rn-realvideo";
                case "sam": return "application/x-sam";
                case "sat": return "application/x-sat";
                case "sdp": return "application/sdp";
                case "sdw": return "application/x-sdw";
                case "sit": return "application/x-stuffit";
                case "slb": return "application/x-slb";
                case "sld": return "application/x-sld";
                case "slk": return "drawing/x-slk";
                case "smi": return "application/smil";
                case "smil": return "application/smil";
                case "smk": return "application/x-smk";
                case "snd": return "audio/basic";
                case "sol": return "text/plain";
                case "sor": return "text/plain";
                case "spc": return "application/x-pkcs7-certificates";
                case "spl": return "application/futuresplash";
                case "spp": return "text/xml";
                case "ssm": return "application/streamingmedia";
                case "sst": return "application/vnd.ms-pki.certstore";
                case "stl": return "application/vnd.ms-pki.stl";
                case "stm": return "text/html";
                case "sty": return "application/x-sty";
                case "svg": return "text/xml";
                case "tdf": return "application/x-tdf";
                case "tg4": return "application/x-tg4";
                case "tga": return "application/x-tga";
                case "tld": return "text/xml";
                case "top": return "drawing/x-top";
                case "tsd": return "text/xml";
                case "txt": return "text/plain";
                case "uin": return "application/x-icq";
                case "uls": return "text/iuls";
                case "vcf": return "text/x-vcard";
                case "vda": return "application/x-vda";
                case "vdx": return "application/vnd.visio";
                case "vml": return "text/xml";
                case "vpg": return "application/x-vpeg005";
                case "vsd": return "application/vnd.visio";
                case "vss": return "application/vnd.visio";
                case "vst": return "application/vnd.visio";
                case "vsw": return "application/vnd.visio";
                case "vsx": return "application/vnd.visio";
                case "vtx": return "application/vnd.visio";
                case "vxml": return "text/xml";
                case "wax": return "audio/x-ms-wax";
                case "wb1": return "application/x-wb1";
                case "wb2": return "application/x-wb2";
                case "wb3": return "application/x-wb3";
                case "wbmp": return "image/vnd.wap.wbmp";
                case "wiz": return "application/msword";
                case "wk3": return "application/x-wk3";
                case "wk4": return "application/x-wk4";
                case "wkq": return "application/x-wkq";
                case "wks": return "application/x-wks";
                case "wm": return "video/x-ms-wm";
                case "wmd": return "application/x-ms-wmd";
                case "wmf": return "application/x-wmf";
                case "wmx": return "video/x-ms-wmx";
                case "wmz": return "application/x-ms-wmz";
                case "wp6": return "application/x-wp6";
                case "wpd": return "application/x-wpd";
                case "wpg": return "application/x-wpg";
                case "wpl": return "application/vnd.ms-wpl";
                case "wq1": return "application/x-wq1";
                case "wr1": return "application/x-wr1";
                case "wri": return "application/x-wri";
                case "wrk": return "application/x-wrk";
                case "ws": return "application/x-ws";
                case "ws2": return "application/x-ws";
                case "wsc": return "text/scriptlet";
                case "wvx": return "video/x-ms-wvx";
                case "xdp": return "application/vnd.adobe.xdp";
                case "xdr": return "text/xml";
                case "xfd": return "application/vnd.adobe.xfd";
                case "xfdf": return "application/vnd.adobe.xfdf";
                case "xq": return "text/xml";
                case "xql": return "text/xml";
                case "xquery": return "text/xml";
                case "xwd": return "application/x-xwd";
                case "x_b": return "application/x-x_b";
                case "x_t": return "application/x-x_t";
            }
            return "application/octet-stream";
        }
        #endregion

        public static bool ValidateImage(string fileType, Stream inputStream)
        {
            if (inputStream.Length == 0)
                return false;
            if (!fileType.StartsWith("image"))
                return true;
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(inputStream);
                image.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
