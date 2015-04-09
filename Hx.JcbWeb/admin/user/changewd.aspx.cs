using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.Web;
using System.Text;
using Hx.Components;
using Hx.Components.BasePage;
using Hx.Tools;

namespace Hx.JcbWeb.admin.user
{
    public partial class changewd : JcbBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool result = JcbUsers.Instance.ChangePassword(HXContext.Current.AdminUserID, EncryptString.MD5(TxtOldPassword.Text), EncryptString.MD5(TxtNewUserPassword.Text));
                if (!result)
                {
                    lerrorMes.Text = "原密码错误";
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("<span class=\"dalv\">密码修改成功,请使用新密码登录！</span><br />");
                WriteMessage("~/message/showmessage.aspx", "保存成功！", sb.ToString(), "", "~/admin/logout.aspx");

            }
        }

        protected override void Check()
        {
            if (!JCBContext.Current.UserCheck)
            {
                Response.Redirect("~/admin/Login.aspx");
                return;
            }
        }
    }
}