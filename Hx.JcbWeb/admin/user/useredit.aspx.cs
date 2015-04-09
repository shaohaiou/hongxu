using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools;
using System.Text;
using Hx.Components.Web;

namespace Hx.JcbWeb.admin.user
{
    public partial class useredit : JcbBase
    {
        private JcbUserInfo admin = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int id = GetInt("id");
                if (id > 0)
                {
                    rePassword.Visible = false;
                    reTruePassword.Visible = false;
                    cvPassword.Visible = false;
                    Header.Title = "编辑用户";
                    admin = JcbUsers.Instance.GetUserInfo(id);

                    if (admin != null)
                    {
                        BindData(admin);
                    }
                    else
                    {
                        WriteErrorMessage("操作出错！", "该用户不存在，可能已经被删除！", "~/user/adminlist.aspx");
                    }
                }
                else
                {
                    Header.Title = "添加用户";
                }
            }
        }

        /// <summary>
        /// 绑定页面数据
        /// </summary>
        /// <param name="item"></param>
        protected void BindData(JcbUserInfo admin)
        {
            hdid.Value = admin.ID.ToString();               //管理员ID
            txtUserName.Text = admin.UserName;//账户名
        }

        /// <summary>
        /// 填充实体类属性
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected void FillDate(JcbUserInfo admin)
        {
            admin.ID = DataConvert.SafeInt(hdid.Value);     //管理员ID
            admin.Administrator = false;        //是否是超级管理员
            if (txtPassword.Text.Trim().Length != 0)
                admin.Password = EncryptString.MD5(txtPassword.Text);              //管理员密码
            admin.UserName = txtUserName.Text;//账户名
            admin.LastLoginIP = string.Empty;
        }

        /// <summary>
        /// 保存广告条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btSave_Click(object sender, EventArgs e)
        {
            int id;
            //是否通过页面验证
            if (Page.IsValid)
            {
                id = DataConvert.SafeInt(hdid.Value);

                if (id > 0)
                {
                    admin = JcbUsers.Instance.GetUserInfo(id);
                    if (admin == null)
                    {
                        WriteMessage("~/message/showmessage.aspx", "操作出错！", "该用户不存在，可能已经被删除！", "", FromUrl);
                    }
                    else
                    {
                        FillDate(admin);
                        JcbUsers.Instance.UpdateUser(admin);
                        if (admin.ID == Admin.ID)
                            Admin = admin;
                    }
                }
                else
                {
                    admin = new JcbUserInfo();
                    FillDate(admin);
                    id = JcbUsers.Instance.AddUser(admin);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<span class=\"dalv\">用户保存成功！</span><br />");
                WriteMessage("~/message/showmessage.aspx", "用户保存成功！", sb.ToString(), "", FromUrl);
            }

            return;
        }

        protected override void Check()
        {
            if (!JCBContext.Current.UserCheck)
            {
                Response.Redirect("~/admin/Login.aspx");
                return;
            }
            if (!Admin.Administrator)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }
    }
}