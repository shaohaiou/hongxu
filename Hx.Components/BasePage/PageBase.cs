using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Hx.Tools;
using Hx.Tools.Web;
using Hx.Components.Web;

namespace Hx.Components.BasePage
{
    /// <summary>
    /// 页面基类
    /// </summary>
    public class PageBase : Page
    {
        protected override void OnInit(EventArgs e)
        {
            Check();

            base.OnInit(e);
        }


        /// <summary>
        /// 当前url
        /// </summary>
        protected virtual string CurrentUrl
        {
            get
            {
                return System.Web.HttpUtility.UrlEncode(System.Web.HttpUtility.UrlDecode(HttpContext.Current.Request.RawUrl));
            }
        }

        private string _currentpath = string.Empty;

        protected virtual string CurrentPath
        {
            get
            {
                if (string.IsNullOrEmpty(_currentpath))
                {
                    _currentpath = CurrentUrl.Split(new char[] { '?' })[0];
                }
                return _currentpath;
            }
        }

        protected virtual void Check() { }

        /// <summary>
        /// 添加Meta
        /// </summary>
        protected virtual void AddMetaTag(string name, string value)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
                return;

            HtmlMeta meta = new HtmlMeta();
            meta.Name = name;
            meta.Content = value;
            Page.Header.Controls.Add(meta);
        }

        /// <summary>
        /// 在头部添加css文件
        /// </summary>
        /// <param name="url">css路径</param>
        public virtual void AddStylesheetInclude(string url)
        {
            HtmlLink link = new HtmlLink();
            link.Attributes["type"] = "text/css";
            link.Attributes["href"] = url;
            link.Attributes["rel"] = "stylesheet";
            Page.Header.Controls.Add(link);
        }

        //返回根路径
        protected string BasePath
        {
            get
            {
                return WebHelper.GetBasePath();
            }
        }

        //返回完整的根路径
        protected string FullBasePath
        {
            get
            {
                return WebHelper.GetFullBasePath();
            }
        }

        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        protected string PageName
        {
            get
            {
                return WebHelper.GetPageName();
            }
        }

        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public string GetServerString(string strName)
        {
            return WebHelper.GetServerString(strName);
        }

        #region 从Request.QueryString中获取值

        /// <summary>
        /// 从querystring里获取int32数据
        /// </summary>
        /// <param name="queryItem">querystring 中的key</param>
        /// <returns></returns>
        protected int GetQueryInt(string queryItem)
        {
            return WebHelper.GetQueryInt(queryItem);
        }

        /// <summary>
        /// 从querystring里获取int32数据
        /// </summary>
        /// <param name="queryItem">querystring 中的key</param>
        /// <param name="defaultValue">存在返回的默认值</param>
        /// <returns></returns>
        protected int GetQueryInt(string queryItem, int defaultValue)
        {
            return WebHelper.GetQueryInt(queryItem, defaultValue);
        }

        /// <summary>
        /// 从querystring里获取数据
        /// </summary>
        /// <param name="queryItem">querystring 中的key</param>
        /// <returns></returns>
        protected string GetQueryString(string queryItem)
        {
            return WebHelper.GetQueryString(queryItem);
        }

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        protected string GetQueryString(string queryItem, bool sqlSafeCheck)
        {
            return WebHelper.GetQueryString(queryItem, sqlSafeCheck);
        }

        #endregion

        #region 从Request.Form中获取值

        /// <summary>
        /// 从querystring里获取数据
        /// </summary>
        /// <param name="queryItem">querystring 中的key</param>
        /// <param name="defaultValue">存在返回的默认值</param>
        /// <returns></returns>
        protected int GetFormInt(string queryItem)
        {
            return WebHelper.GetFormInt(queryItem);
        }

        /// <summary>
        /// 从querystring里获取数据
        /// </summary>
        /// <param name="queryItem">querystring 中的key</param>
        /// <param name="defaultValue">存在返回的默认值</param>
        /// <returns></returns>
        protected int GetFormInt(string queryItem, int defaultValue)
        {
            return WebHelper.GetFormInt(queryItem, defaultValue);
        }

        /// <summary>
        /// 从Request.Form里获取数据
        /// </summary>
        /// <param name="queryItem">Request.Form 中的key</param>
        /// <returns></returns>
        protected string GetFormString(string queryItem)
        {
            return WebHelper.GetFormString(queryItem);
        }

        /// <summary>
        /// 从Request.Form里获取数据
        /// </summary>
        /// <param name="queryItem">Request.Form 中的key</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        protected string GetFormString(string queryItem, string defaultValue)
        {
            return WebHelper.GetFormString(queryItem);
        }
        #endregion

        #region 从Request.Form或Request.QueryString

        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public int GetInt(string queryItem)
        {
            return WebHelper.GetInt(queryItem);
        }

        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public int GetInt(string queryItem, int defValue)
        {
            return WebHelper.GetInt(queryItem, defValue);
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public string GetString(string queryItem)
        {
            return WebHelper.GetString(queryItem);
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url或表单参数的值</returns>
        public string GetString(string queryItem, bool sqlSafeCheck)
        {
            return WebHelper.GetString(queryItem, sqlSafeCheck);
        }
        #endregion

        #region 页面重定向

        /// <summary>
        /// 结束页面的处理，页面重定向
        /// </summary>
        /// <param name="redirecturl">跳转的url</param>
        protected void ResponseRedirect(string redirecturl)
        {
            ResponseRedirect(redirecturl, true);
        }

        /// <summary>
        /// 结束页面的处理，页面重定向
        /// </summary>
        /// <param name="redirecturl">跳转的url</param>
        protected void ResponseRedirect(string redirecturl, bool endResponse)
        {
            HttpContext.Current.Response.Redirect(redirecturl, endResponse);
        }
        #endregion

        #region ListControl控件控制
        /// <summary>
        /// 设置ListControl选中项
        /// </summary>
        /// <param name="listControl">ListControl控件</param>
        /// <param name="selectValue">选中的Value</param>
        protected void SetSelectedByValue(ListControl listControl, string selectValue)
        {
            WebHelper.SetSelectedByValue(listControl, selectValue);
        }

        /// <summary>
        /// 设置ListControl选中项
        /// </summary>
        /// <param name="listControl">ListControl控件</param>
        /// <param name="selectText">选中的Text</param>
        protected void SetSelectedByText(ListControl listControl, string selectText)
        {
            WebHelper.SetSelectedByText(listControl, selectText);
        }

        /// <summary>
        /// 从列表控件里删除符合指定值的项
        /// </summary>
        /// <param name="listControl">列表控件</param>
        /// <param name="selectValue">选中项</param>
        protected void RemoveListItemByValue(ListControl listControl, string selectValue)
        {
            WebHelper.RemoveListItemByValue(listControl, selectValue);
        }

        /// <summary>
        /// 从列表控件里删除符合指定值的项
        /// </summary>
        /// <param name="listControl">列表控件</param>
        /// <param name="selectValue">选中项</param>
        protected void RemoveListItemByText(ListControl listControl, string selectValue)
        {
            WebHelper.RemoveListItemByText(listControl, selectValue);
        }
        #endregion

        #region 输出信息

        /// <summary>
        /// 显示信息页面
        /// </summary>
        /// <param name="messageTitle">跳转链接地址</param>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        protected void WriteMessage(string showUrl, string messageTitle, string message)
        {
            WriteMessage(showUrl, messageTitle, message, null, null);
        }

        /// <summary>
        /// 显示信息页面
        /// </summary>
        /// <param name="messageTitle">跳转链接地址</param>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        /// <param name="returnTitle">返回地址文本</param>
        /// <param name="returnUrl">返回地址</param>
        protected void WriteMessage(string showUrl, string messageTitle, string message, string returnTitle, string returnUrl)
        {
            HXContext.Current.Message = message;
            HXContext.Current.MessageTitle = messageTitle;
            HXContext.Current.ReturnTitle = returnTitle;
            HXContext.Current.ReturnUrl = returnUrl;
            HttpContext.Current.Server.Transfer(showUrl);
        }

        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        protected void WriteSuccessMessage(string messageTitle, string message)
        {
            WriteSuccessMessage(messageTitle, message, null, null);
        }
        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        /// <param name="returnUrl">返回地址</param>
        protected void WriteSuccessMessage(string messageTitle, string message, string returnUrl)
        {
            WriteSuccessMessage(messageTitle, message, null, returnUrl);
        }


        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        /// <param name="returnTitle">返回地址文本</param>
        /// <param name="returnUrl">返回地址</param>
        protected void WriteSuccessMessage(string messageTitle, string message, string returnTitle, string returnUrl)
        {
            WriteMessage(this.SuccessPageUrl, messageTitle, message, returnTitle, returnUrl);
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        /// <param name="mesg">需要显示的Url</param>
        protected void WriteErrorMessage(string messageTitle, string message)
        {
            WriteErrorMessage(messageTitle, message, null, null);
        }



        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        /// <param name="returnUrl">返回地址</param>
        protected void WriteErrorMessage(string messageTitle, string message, string returnUrl)
        {
            WriteErrorMessage(messageTitle, message, null, returnUrl);
        }

        /// <summary>
        /// 显示成功信息
        /// </summary>
        /// <param name="messageTitle">信息标题</param>
        /// <param name="message">需要显示的信息</param>
        /// <param name="returnTitle">返回地址文本</param>
        /// <param name="returnUrl">返回地址</param>
        protected void WriteErrorMessage(string messageTitle, string message, string returnTitle, string returnUrl)
        {
            WriteMessage(this.ErrorPageUrl, messageTitle, message, returnTitle, returnUrl);
        }
        #endregion

        /// <summary>
        /// 显示错误信息的地址
        /// </summary>
        protected virtual string ErrorPageUrl
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 显示操作成功信息的地址
        /// </summary>
        protected virtual string SuccessPageUrl
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        protected bool IsPost()
        {
            return WebHelper.IsPost();
        }

        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        protected bool IsGet()
        {
            return WebHelper.IsGet();
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="msg">消息</param>
        protected void WriteText(string msg)
        {
            Response.Clear();
            Response.Write(msg);
            Response.End();
        }


        /// <summary>
        /// 将html编码解码
        /// </summary>
        /// <param name="textToFormat">需要解码的html代码</param>
        /// <returns>解码后的字符串</returns>
        public static string HtmlDecode(String textToFormat)
        {
            return WebHelper.HtmlDecode(textToFormat);
        }

        /// <summary>
        /// 将字符串编码为html编码
        /// </summary>
        /// <param name="textToFormat">需要编码的html</param>
        /// <returns>编码后的字符串</returns>
        public static string HtmlEncode(String textToFormat)
        {
            return WebHelper.HtmlEncode(textToFormat);
        }

        /// <summary>
        /// 将url编码
        /// </summary>
        /// <param name="urlToEncode">需要编码的url</param>
        /// <returns>编码后的url</returns>
        public static string UrlEncode(string urlToEncode)
        {
            return WebHelper.UrlEncode(urlToEncode);
        }

        /// <summary>
        /// 将url解码
        /// </summary>
        /// <param name="urlToDecode">需要解码的url</param>
        /// <returns>解码后的url</returns>
        public string UrlDecode(string urlToDecode)
        {
            return WebHelper.UrlDecode(urlToDecode);
        }


        public void AppendScript(string script)
        {
            WebHelper.AppendScript(script);
        }

        public void ShowMessage(string meg, bool isreload)
        {
            WebHelper.ShowMessage(meg, isreload);
        }

        public void ShowMessage(string meg)
        {
            WebHelper.ShowMessage(meg);
        }
    }
}
