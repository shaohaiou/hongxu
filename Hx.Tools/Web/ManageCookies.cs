using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;

namespace Hx.Tools.Web
{
    public class ManageCookies
    {
        #region 用户Cookie管理
        /// <summary>
        /// 建立用户票据cookie
        /// </summary>
        /// <param name="authTicket">票据</param>
        /// <param name="isPersistent">是否持久化</param>
        /// <param name="expirationTime">过期日期</param>
        public static void CreateUserCookie(FormsAuthenticationTicket authTicket, bool isPersistent, DateTime expirationTime)
        {
            CreateUserCookie(FormsAuthentication.FormsCookieName, authTicket, isPersistent, expirationTime);
        }

        /// <summary>
        /// 移除用户标示的cookie
        /// </summary>
        public static void RemoveUserCookie()
        {
            RemoveCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.CookieDomain);
        }
        #endregion

        #region Cookie管理

        /// <summary>
        /// 移除cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        public static void RemoveCookie(string cookieName)
        {
            RemoveCookie(cookieName, null);
        }

        /// <summary>
        /// 移除cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        public static void RemoveCookie(string cookieName, string domain)
        {
            HttpCookie cookie = new HttpCookie(cookieName, string.Empty);
            cookie.Expires = DateTime.Now.AddYears(-1);
            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }

            HttpContext.Current.Response.Cookies.Remove(cookieName);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 建立用户票据cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <param name="authTicket">票据</param>
        /// <param name="isPersistent">是否持久化</param>
        /// <param name="expirationTime">过期日期</param>
        public static HttpCookie CreateUserCookie(string cookieName, FormsAuthenticationTicket authTicket, bool isPersistent, DateTime expirationTime)
        {
            return CreateUserCookie(cookieName, authTicket, isPersistent, expirationTime, FormsAuthentication.CookieDomain);
        }

        /// <summary>
        /// 建立用户票据cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <param name="authTicket">票据</param>
        /// <param name="isPersistent">是否持久化</param>
        /// <param name="expirationTime">过期日期</param>
        /// <param name="domain">域</param>
        public static HttpCookie CreateUserCookie(string cookieName, FormsAuthenticationTicket authTicket, bool isPersistent, DateTime expirationTime, string domain)
        {
            string str = FormsAuthentication.Encrypt(authTicket);
            HttpCookie cookie = new HttpCookie(cookieName, str);
            if (isPersistent)
            {
                cookie.Expires = expirationTime;
            }
            cookie.HttpOnly = true;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            cookie.Secure = FormsAuthentication.RequireSSL;
            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
            return cookie;
        }

        /// <summary>
        /// 建立cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <param name="value">cookie值</param>
        /// <param name="isPersistent">是否持久化</param>
        /// <param name="expirationTime">过期日期</param>
        public static HttpCookie CreateCookie(string cookieName, bool isPersistent, DateTime expirationTime, string domain)
        {
            return CreateCookie(cookieName, null, isPersistent, expirationTime, domain);
        }

        /// <summary>
        /// 建立cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <param name="value">cookie值</param>
        /// <param name="isPersistent">是否持久化</param>
        /// <param name="expirationTime">过期时间</param>
        /// <param name="domain">cookie域</param>
        /// <param name="domain">是否允许客户端访问</param>
        public static HttpCookie CreateCookie(string cookieName, string value, bool isPersistent, DateTime expirationTime, string domain)
        {
            return CreateCookie(cookieName, value, isPersistent, expirationTime, domain, false);
        }

        /// <summary>
        /// 建立cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <param name="value">cookie值</param>
        /// <param name="isPersistent">是否持久化</param>
        /// <param name="expirationTime">过期时间</param>
        /// <param name="domain">cookie域</param>
        /// <param name="allowclice">是否允许客户端访问</param>
        public static HttpCookie CreateCookie(string cookieName, string value, bool isPersistent, DateTime expirationTime, string domain, bool allowclice)
        {
            HttpCookie cookie = null;
            if (!string.IsNullOrEmpty(value))
            {
                cookie = new HttpCookie(cookieName, value);
            }
            else
            {
                cookie = new HttpCookie(cookieName);
            }
            if (isPersistent)
            {
                cookie.Expires = expirationTime;
            }
            cookie.HttpOnly = !allowclice;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
            return cookie;
        }



        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookieValue(string strName)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                {
                    return HttpContext.Current.Request.Cookies[strName].Value.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookieValue(string strName, string key)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        return HttpContext.Current.Request.Cookies[strName].Value.ToString();
                    }
                    return HttpContext.Current.Request.Cookies[strName][key];
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 读取cookie
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static HttpCookie GetCookie(string strName)
        {
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    return HttpContext.Current.Request.Cookies[strName];
                }
            }
            catch { return null; }
            return null;
        }

        /// <summary>
        /// 是否存在此cookie
        /// </summary>
        /// <param name="name">cookie名</param>
        /// <returns></returns>
        public static bool IsCookieExist(string name)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[name] != null)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
