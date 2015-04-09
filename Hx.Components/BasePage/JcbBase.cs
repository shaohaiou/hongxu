using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components.Enumerations;
using System.Web;

namespace Hx.Components.BasePage
{
    public class JcbBase : PageBase
    {
        /// <summary>
        /// 检查当前后台用户是否登陆
        /// </summary>
        protected override void Check()
        {
            if (!JCBContext.Current.UserCheck)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        /// <summary>
        /// 显示操作成功的页面
        /// </summary>
        protected override string SuccessPageUrl
        {
            get
            {
                return "~/message/showmessage.aspx";
            }
        }

        /// <summary>
        /// 显示操作失败的页面
        /// </summary>
        protected override string ErrorPageUrl
        {
            get
            {
                return "~/message/showmessage.aspx";
            }
        }

        /// <summary>
        /// 后台用户名
        /// </summary>
        protected string UserName
        {
            get
            {
                if (JCBContext.Current.UserCheck)
                {
                    return JCBContext.Current.AdminUser.UserName;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 后台用户ID
        /// </summary>
        protected int UserID
        {
            get
            {
                return JCBContext.Current.AdminUserID;
            }
        }

        /// <summary>
        /// 后台用户实体
        /// </summary>
        protected JcbUserInfo Admin
        {
            get
            {
                return JCBContext.Current.AdminUser;
            }
            set
            {
                JCBContext.Current.AdminUser = value;
            }
        }

        protected virtual string FromUrl
        {
            get
            {
                string from = GetString("from");
                if (!string.IsNullOrEmpty(from))
                {
                    return System.Web.HttpUtility.UrlDecode(from);
                }
                return string.Empty;
            }
        }

        protected override void WriteMessage(string showUrl, string messageTitle, string message, string returnTitle, string returnUrl)
        {
            JCBContext.Current.Message = message;
            JCBContext.Current.MessageTitle = messageTitle;
            JCBContext.Current.ReturnTitle = returnTitle;
            JCBContext.Current.ReturnUrl = returnUrl;
            HttpContext.Current.Server.Transfer(showUrl);
        }
    }
}
