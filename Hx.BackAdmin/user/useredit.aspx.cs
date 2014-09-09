using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools;
using Hx.Car;
using System.Text;

namespace Hx.BackAdmin.user
{
    public partial class useredit : AdminBase
    {
        private AdminInfo admin = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindControler();
                int id = GetInt("id");
                if (id > 0)
                {
                    rePassword.Visible = false;
                    reTruePassword.Visible = false;
                    cvPassword.Visible = false;
                    Header.Title = "编辑用户";
                    admin = Admins.Instance.GetAdmin(id);

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

        private void BindControler()
        {
            ddlCorporation.DataSource = Corporations.Instance.GetList(true);
            ddlCorporation.DataTextField = "Name";
            ddlCorporation.DataValueField = "ID";
            ddlCorporation.DataBind();
            ddlCorporation.Items.Insert(0, new ListItem("-请选择-", "-1"));

            if (!Admin.Administrator)
                trCorp.Visible = false;
        }

        /// <summary>
        /// 绑定页面数据
        /// </summary>
        /// <param name="item"></param>
        protected void BindData(AdminInfo admin)
        {
            hdid.Value = admin.ID.ToString();               //管理员ID
            txtUserName.Text = admin.UserName;//账户名
            txtMobile.Text = admin.Mobile;
            SetSelectedByValue(ddlCorporation, admin.Corporation);
        }

        /// <summary>
        /// 填充实体类属性
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected void FillDate(AdminInfo admin)
        {
            admin.ID = DataConvert.SafeInt(hdid.Value);     //管理员ID
            admin.Administrator = false;        //是否是超级管理员
            if (txtPassword.Text.Trim().Length != 0)
                admin.Password = EncryptString.MD5(txtPassword.Text);              //管理员密码
            admin.UserName = txtUserName.Text;//账户名
            admin.LastLoginIP = string.Empty;
            admin.Mobile = txtMobile.Text;
            if (Admin.Administrator)
                admin.Corporation = ddlCorporation.SelectedValue;
            else
                admin.Corporation = Admin.Corporation;
            admin.UserRole = Components.Enumerations.UserRoleType.销售员;
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
                    admin = Admins.Instance.GetAdmin(id);
                    if (admin == null)
                    {
                        WriteMessage("~/message/showmessage.aspx", "操作出错！", "该用户不存在，可能已经被删除！", "", FromUrl);
                    }
                    else
                    {
                        FillDate(admin);
                        Admins.Instance.UpdateAdmin(admin);
                        if (admin.ID == Admin.ID)
                            Admin = admin;
                    }
                }
                else
                {
                    admin = new AdminInfo();
                    FillDate(admin);
                    id = Admins.Instance.AddAdmin(admin);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<span class=\"dalv\">用户保存成功！</span><br />");
                WriteMessage("~/message/showmessage.aspx", "用户保存成功！", sb.ToString(), "", FromUrl);
            }

            return;
        }

        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }
    }
}