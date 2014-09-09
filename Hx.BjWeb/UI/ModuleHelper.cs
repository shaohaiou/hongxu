using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using System.Net;
using System;
using Hx.Components;
using Hx.Components.Config;
using Hx.Components.Web;
using Hx.Tools.Web;
using Hx.URLRewriter.Config;

namespace Hx.BjWeb.UI
{
    public class ModuleHelper
    {

        internal static void AuthenticateRequest(object sender, EventArgs e)
        {

        }

        internal static void OnError(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            Exception ex = context.Server.GetLastError();
            if (ex is System.ArgumentOutOfRangeException)
            {
                ClearCookies(context);
                return;
            }

            if (ex != null)
            {
                ex = ex.GetBaseException();
            }
            else
            {
                EventLogs.WebError("发生未知错误", 1000, 0, ex);
                return;
            }
            string msg = ex.Message;
            #region
            if (ex is FileNotFoundException || ex is FileLoadException || ex.GetType().Name.Equals("HttpException"))
            {
                if (ex is System.Web.HttpException)
                {
                    System.Web.HttpException httpex = (System.Web.HttpException)ex;
                    int httpcode = httpex.GetHttpCode();
                    string ur = "";
                    if (context.Request.UrlReferrer != null)
                    {
                        ur = context.Request.UrlReferrer.ToString();
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("HttpException错误-访问页面:{0}   上个页面地址:{1}    IP:{2}     浏览器：{3}\n", application.Request.RawUrl, ur, HXContext.Current.IP, HttpContext.Current.Request.Browser.Type);
                    sb.AppendFormat("ex:{0}                  exname:{1}", ex.StackTrace, msg);
                    Logs.WriteErrorUrlLog(sb.ToString());//写入到错误日志文件
                    if (httpcode == 404 || application.Response.StatusCode == 404)
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = 404;
                        context.Response.End();
                        application.Server.ClearError();
                        return;
                    }
                }
                else
                {
                    context.Response.Clear();
                    context.Response.StatusCode = 404;
                    context.Response.End();
                    application.Server.ClearError();
                    return;
                }
            }
            else
            {
                EventLogs.WebError("页面发生错误:" + application.Request.RawUrl + "   错误原因：" + ex.Message + "-" + ex.StackTrace, 1500, 0);
                application.Response.Clear();
                application.Response.StatusCode = 500;
                application.Response.Write("<html xmlns=\"http://www.w3.org/1999/xhtml/\" ><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>页面出错</title></head><body style=\"font-size:14px;\">错误信息：");
                application.Response.Write("网站发生错误");
                application.Response.Write("</body></html>");
                application.Response.End();
            }
            application.Server.ClearError();
            #endregion
        }

        #region 收集 "没有此 Unicode 字符可以映射到的字符" 错误信息

        private static void ClearCookies(HttpContext context)
        {

        }

        #endregion

        internal static void AuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            Rewrite(app.Request.Path, app);
        }

        /// <summary>
        /// 异步委托更新
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private delegate void AsyncGetUser(string username);

        #region Url重写

        /// <summary>
        /// Url重写
        /// </summary>
        /// <param name="requestedRawUrl">Request.Path</param>
        /// <param name="app">HttpApplication</param>
        private static void Rewrite(string requestedPath, System.Web.HttpApplication app)
        {
            CommConfig config = CommConfig.GetConfig();
            ///过滤不需要重写的url规则
            if (requestedPath.IndexOf(".") > 0 && !requestedPath.EndsWith("/") && requestedPath.IndexOf(".asp") < 0)
            {
                if (requestedPath.ToLower().IndexOf("rss.ashx") >= 0)
                {
                    return;
                }
                int duration = 60;
                if (requestedPath.EndsWith(".ashx") || requestedPath.EndsWith(".js") || requestedPath.EndsWith(".asmx") || requestedPath.EndsWith(".svc") || requestedPath.EndsWith("qu.html") || requestedPath.EndsWith("quest.html") || requestedPath.EndsWith("quest2.html") || requestedPath.EndsWith("wxhfm.html"))
                {
                    return;
                }
            }

            //主页跳转
            if (requestedPath.StartsWith("/index.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                RewriteUrl(app.Context, CreateTemplateUrl(requestedPath));
                return;
            }
            if (requestedPath == "/")
            {
                RewriteUrl(app.Context, CreateTemplateUrl("/index.aspx"));
                return;
            }

            #region url重写

            RewriterRuleCollection rules = RewriterConfiguration.GetConfig().Rules;
            for (int i = 0; i < rules.Count; i++)
            {
                string lookFor = "^" + ResolveUrl(app.Context.Request.ApplicationPath, rules[i].LookFor) + "$";
                Regex re = new Regex(lookFor, RegexOptions.IgnoreCase);
                if (re.IsMatch(requestedPath))
                {
                    string sendto = rules[i].SendTo;
                    string sendToUrl = CreateTemplateUrl(ResolveUrl(app.Context.Request.ApplicationPath, re.Replace(requestedPath, sendto)));
                    RewriteUrl(app.Context, sendToUrl);
                    return;
                }
            }



            #endregion

            return;
        }


        #region RewriteUrl
        /// <summary>
        /// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
        /// </summary>
        /// <param name="context">The HttpContext object to rewrite the URL to.</param>
        /// <param name="sendToUrl">The URL to rewrite to.</param>
        internal static void RewriteUrl(HttpContext context, string sendToUrl)
        {
            string x, y;
            RewriteUrl(context, sendToUrl, out x, out y);
        }

        /// <summary>
        /// Rewrite's a URL using <b>HttpContext.RewriteUrl()</b>.
        /// </summary>
        /// <param name="context">The HttpContext object to rewrite the URL to.</param>
        /// <param name="sendToUrl">The URL to rewrite to.</param>
        /// <param name="sendToUrlLessQString">Returns the value of sendToUrl stripped of the querystring.</param>
        /// <param name="filePath">Returns the physical file path to the requested page.</param>
        internal static void RewriteUrl(HttpContext context, string sendToUrl, out string sendToUrlLessQString, out string filePath)
        {
            // see if we need to add any extra querystring information
            if (context.Request.QueryString.Count > 0)
            {
                if (sendToUrl.IndexOf('?') != -1)
                    sendToUrl += "&" + context.Request.QueryString.ToString();
                else
                    sendToUrl += "?" + context.Request.QueryString.ToString();
            }

            // first strip the querystring, if any
            string queryString = String.Empty;
            sendToUrlLessQString = sendToUrl;
            if (sendToUrl.IndexOf('?') > 0)
            {
                sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf('?'));
                queryString = sendToUrl.Substring(sendToUrl.IndexOf('?') + 1);
            }

            // grab the file's physical path
            filePath = string.Empty;
            filePath = context.Server.MapPath(sendToUrlLessQString);

            // rewrite the path...
            context.RewritePath(sendToUrlLessQString, String.Empty, queryString);


            // NOTE!  The above RewritePath() overload is only supported in the .NET Framework 1.1
            // If you are using .NET Framework 1.0, use the below form instead:
            // context.RewritePath(sendToUrl);
        }
        #endregion

        /// <summary>
        /// Converts a URL into one that is usable on the requesting client.
        /// </summary>
        /// <remarks>Converts ~ to the requesting application path.  Mimics the behavior of the 
        /// <b>Control.ResolveUrl()</b> method, which is often used by control developers.</remarks>
        /// <param name="appPath">The application path.</param>
        /// <param name="url">The URL, which might contain ~.</param>
        /// <returns>A resolved URL.  If the input parameter <b>url</b> contains ~, it is replaced with the
        /// value of the <b>appPath</b> parameter.</returns>
        internal static string ResolveUrl(string appPath, string url)
        {
            if (url.Length == 0 || url[0] != '~')
                return url;		// there is no ~ in the first character position, just return the url
            else
            {
                if (url.Length == 1)
                    return appPath;  // there is just the ~ in the URL, return the appPath
                if (url[1] == '/' || url[1] == '\\')
                {
                    // url looks like ~/ or ~\
                    if (appPath.Length > 1)
                        return appPath + "/" + url.Substring(2);
                    else
                        return "/" + url.Substring(2);
                }
                else
                {
                    // url looks like ~something
                    if (appPath.Length > 1)
                        return appPath + "/" + url.Substring(1);
                    else
                        return appPath + url.Substring(1);
                }
            }
        }

        #endregion

        #region 创建模板

        /// <summary>
        /// 通过请求路径生成模板
        /// </summary>
        /// <param name="requestPath">请求路径</param>
        /// <returns>返回真实的路径</returns>
        private static string CreateTemplateUrl(string requestPath)
        {
            string basepath = WebHelper.GetBasePath();                              //根路径
            HttpContext context = HttpContext.Current;                              //当前上下文
            string rpath = requestPath.Split(new char[] { '?' })[0];                //当前请求路径（不包含请求查询参数）    
            if (rpath.IndexOf("/aspx/") < 0 && rpath.EndsWith("aspx"))              //在/aspx/*.aspx文件不执模板生成
            {
                requestPath = requestPath.StartsWith("/") ? requestPath : "/" + requestPath;
                string[] path = requestPath.Replace(basepath, "/").Split('/');
                //返回生成后的模板地址
                return basepath + requestPath;

            }
            return requestPath;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 字母集合
        /// </summary>
        private static NameValueCollection _classcollection = null;

        private static NameValueCollection ClassCollection
        {
            get
            {
                if (_classcollection == null)
                {
                    _classcollection = new NameValueCollection();
                    _classcollection["3"] = "A";
                    _classcollection["4"] = "B";
                    _classcollection["5"] = "C";
                    _classcollection["6"] = "D";
                    _classcollection["7"] = "E";
                    _classcollection["8"] = "F";
                    _classcollection["9"] = "G";
                    _classcollection["10"] = "H";
                    _classcollection["11"] = "I";
                    _classcollection["12"] = "J";
                    _classcollection["13"] = "K";
                    _classcollection["14"] = "M";
                    _classcollection["15"] = "L";
                    _classcollection["16"] = "N";
                    _classcollection["17"] = "O";
                    _classcollection["18"] = "P";
                    _classcollection["19"] = "Q";
                    _classcollection["20"] = "R";
                    _classcollection["21"] = "S";
                    _classcollection["22"] = "T";
                    _classcollection["24"] = "Y";
                    _classcollection["26"] = "X";
                    _classcollection["27"] = "Z";
                    _classcollection["28"] = "0-9";
                }
                return _classcollection;
            }

        }

        #endregion
    }
}