using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components;
using Hx.Components.Web;
using Hx.Tools;
using System.Text;
using Hx.Components.Entity;

namespace Hx.BackAdmin.user
{
    public partial class changewd : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool result = Admins.Instance.ChangePassword(HXContext.Current.AdminUserID, EncryptString.MD5(TxtOldPassword.Text), EncryptString.MD5(TxtNewUserPassword.Text));
                if (!result)
                {
                    lerrorMes.Text = "原密码错误";
                    return;
                }

                if (Admin.UserRole == Components.Enumerations.UserRoleType.总经理)
                {
                    List<AdminInfo> adminlist = Admins.Instance.GetAllAdmins();
                    if (adminlist.Exists(p => p.ID != AdminID && p.UserRole == Components.Enumerations.UserRoleType.总经理 && p.Name == Admin.Name))
                    {
                        AdminInfo sameaccount = adminlist.Find(p => p.ID != AdminID && p.UserRole == Components.Enumerations.UserRoleType.总经理 && p.Name == Admin.Name);
                        if (sameaccount != null)
                            Admins.Instance.ChangePassword(sameaccount.ID, EncryptString.MD5(TxtOldPassword.Text), EncryptString.MD5(TxtNewUserPassword.Text));
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<span class=\"dalv\">密码修改成功,请使用新密码登录！</span><br />");
                WriteMessage("~/message/showmessage.aspx", "保存成功！", sb.ToString(), "", "/logout.aspx");

            }
        }

        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
        }
    }
}