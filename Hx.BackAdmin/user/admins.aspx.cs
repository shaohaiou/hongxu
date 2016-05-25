using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Tools;
using System.Text;
using Hx.Car;
using Hx.Components.Enumerations;
using System.Data;

namespace Hx.BackAdmin.user
{
    public partial class admins : AdminBase
    {
        private AdminInfo admin = null;
        private AdminInfo CurrentAdmin
        {
            get
            {
                if (admin == null)
                { 
                    int id = GetInt("id");
                    if (id > 0)
                    {
                        admin = Admins.Instance.GetAdmin(id);
                    }
                }

                return admin;
            }
        }
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
                    Header.Title = "编辑管理员";
                    admin = Admins.Instance.GetAdmin(id);

                    if (admin != null)
                    {
                        BindData(admin);
                    }
                    else
                    {
                        WriteErrorMessage("操作出错！", "该管理员不存在，可能已经被删除！", "~/user/adminlist.aspx");
                    }
                }
                else
                {
                    Header.Title = "添加管理员";
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

            DataTable t = EnumExtensions.ToTable<UserRoleType>();
            t.DefaultView.RowFilter = "Value > 0";
            rptUserRoleType.DataSource = t.DefaultView;
            rptUserRoleType.DataBind();
        }

        /// <summary>
        /// 绑定页面数据
        /// </summary>
        /// <param name="item"></param>
        protected void BindData(AdminInfo admin)
        {
            hdid.Value = admin.ID.ToString();               //管理员ID
            cbIsAdmin.Checked = admin.Administrator;        //是否是超级管理员
            txtUserName.Text = admin.UserName;//账户名
            txtName.Text = admin.Name;
            txtMobile.Text = admin.Mobile;
            txtOAID.Text = admin.OAID;
            SetSelectedByValue(ddlCorporation, admin.Corporation);
            hdnUserRoleType.Value = ((int)admin.UserRole).ToString();
        }

        /// <summary>
        /// 填充实体类属性
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected void FillDate(AdminInfo admin)
        {
            admin.ID = DataConvert.SafeInt(hdid.Value);     //管理员ID
            admin.Administrator = cbIsAdmin.Checked;        //是否是超级管理员
            if (txtPassword.Text.Trim().Length != 0)
                admin.Password = EncryptString.MD5(txtPassword.Text);              //管理员密码
            admin.UserName = txtUserName.Text;//账户名
            admin.Name = txtName.Text;
            admin.LastLoginIP = string.Empty;
            admin.Mobile = txtMobile.Text;
            admin.Corporation = ddlCorporation.SelectedValue;
            admin.OAID = txtOAID.Text;
            admin.UserRole = (UserRoleType)DataConvert.SafeInt(hdnUserRoleType.Value);
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
                        WriteMessage("~/message/showmessage.aspx", "操作出错！", "该管理员不存在，可能已经被删除！", "", FromUrl);
                    }
                    else
                    {
                        FillDate(admin);
                        Admins.Instance.UpdateAdmin(admin);
                        if(admin.ID == Admin.ID)
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
                sb.Append("<span class=\"dalv\">管理员保存成功！</span><br />");
                WriteMessage("~/message/showmessage.aspx", "管理员保存成功！", sb.ToString(), "", FromUrl);
            }

            return;
        }

        protected string SetUserRoleType(string v)
        {
            string result = string.Empty;

            if (CurrentAdmin != null)
            {
                if (((int)CurrentAdmin.UserRole & DataConvert.SafeInt(v)) > 0)
                    result = "checked=\"checked\"";
            }

            return result;
        }
    }
}