using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components.Enumerations;

namespace Hx.Components.BasePage
{
    public class JcbBase : PageBase
    {
        /// <summary>
        /// 检查当前后台用户是否登陆
        /// </summary>
        protected override void Check()
        {

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
        protected string AdminName
        {
            get
            {
                if (HXContext.Current.AdminCheck)
                {
                    return HXContext.Current.AdminUser.UserName;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 后台用户ID
        /// </summary>
        protected int AdminID
        {
            get
            {
                return 1;
                //return HXContext.Current.AdminUserID;
            }
        }

        /// <summary>
        /// 后台用户实体
        /// </summary>
        protected AdminInfo Admin
        {
            get
            {
                return HXContext.Current.AdminUser;
            }
            set
            {
                HXContext.Current.AdminUser = value;
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
    }
}
