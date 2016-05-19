using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Tools;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Tools.Web;

namespace Hx.BackAdmin
{
    public partial class login : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HXContext.Current.AdminCheck)
            {
                Response.Clear();
                Response.Write(string.Format("你已经登录，请返回<a href='{0}'>首页</a>", "index.aspx"));
                Response.End();
            }
        }

        /// <summary>
        /// 登录页面不需要验证
        /// </summary>
        protected override void Check()
        {

        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string userName = StrHelper.Trim(tbUserName.Text);
                string password = StrHelper.Trim(tbUserPwd.Text);
                string code = StrHelper.Trim(tbCode.Text).ToLower();

                ///用户名，密码，验证码不允许为空
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password)
                    //&& !string.IsNullOrEmpty(code)
                    )
                {
                    //if (Session["CheckCode"] != null && Session["CheckCode"].ToString().ToLower() == code)
                    //{
                        int id = Admins.Instance.ValiUser(userName, EncryptString.MD5(password));//验证用户

                        if (id > 0)
                        {
                            AdminInfo admin = Admins.Instance.GetAdmin(id);
                            admin.LastLoginIP = WebHelper.GetClientsIP();
                            admin.LastLoginTime = DateTime.Now;
                            Admins.Instance.UpdateAdmin(admin);
                            Session[GlobalKey.SESSION_ADMIN] = admin;
                            ManageCookies.CreateCookie(GlobalKey.SESSION_ADMIN, id.ToString(), true, DateTime.Today.AddDays(1), HXContext.Current.CookieDomain);
                            if (admin.UserRole == Components.Enumerations.UserRoleType.销售员)
                                Response.Redirect("car/carquotation.aspx");
                            else if (admin.UserRole == Components.Enumerations.UserRoleType.财务出纳)
                                Response.Redirect("car/carquotationmg.aspx");
                            else if (admin.UserRole == Components.Enumerations.UserRoleType.车型管理员)
                                Response.Redirect("car/cxmg.aspx");
                            else
                                Response.Redirect("index.aspx");
                        }
                        else
                        {
                            lbMsgUser.Text = "用户名或密码错误！";
                            lbMsgUser.Visible = true;
                        }
                    //}
                    //else
                    //{
                    //    lbMsgUser.Text = "验证码不正确!";
                    //    lbMsgUser.Visible = true;
                    //}
                    Session[GlobalKey.SESSION_ADMIN] = null;
                }
                else
                {
                    lbMsgUser.Text = "用户名，密码不能为空！";
                    lbMsgUser.Visible = true;
                }
            }
        }
    }
}