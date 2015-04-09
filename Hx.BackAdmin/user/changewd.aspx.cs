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

                List<KeyValuePair<string, string>> zjllist = new List<KeyValuePair<string, string>>() 
                { 
                    new KeyValuePair<string, string>("donghongwei","dhw"),
                    new KeyValuePair<string, string>("dhw","donghongwei"),
                    new KeyValuePair<string, string>("邱海彬","qiuhaibin"),
                    new KeyValuePair<string, string>("qiuhaibin","邱海彬"),
                    new KeyValuePair<string, string>("陈军","chenjun"),
                    new KeyValuePair<string, string>("chenjun","陈军"),
                    new KeyValuePair<string, string>("徐晓萍","xuxiaoping"),
                    new KeyValuePair<string, string>("xuxiaoping","徐晓萍"),
                    new KeyValuePair<string, string>("崔建明","cuijianming"),
                    new KeyValuePair<string, string>("cuijianming","崔建明"),
                    new KeyValuePair<string, string>("孙海望","sunhaiwang"),
                    new KeyValuePair<string, string>("sunhaiwang","孙海望"),
                    new KeyValuePair<string, string>("武仰民","wuyangmin"),
                    new KeyValuePair<string, string>("wuyangmin","武仰民"),
                    new KeyValuePair<string, string>("麻立军","malijun"),
                    new KeyValuePair<string, string>("malijun","麻立军"),
                    new KeyValuePair<string, string>("项公程","xianggongcheng"),
                    new KeyValuePair<string, string>("xianggongcheng","项公程"),
                    new KeyValuePair<string, string>("刘道从","liudaocong"),
                    new KeyValuePair<string, string>("liudaocong","刘道从"),
                    new KeyValuePair<string, string>("刘兴龙","liuxinglong"),
                    new KeyValuePair<string, string>("liuxinglong","刘兴龙"),
                    new KeyValuePair<string, string>("聂军","niejun"),
                    new KeyValuePair<string, string>("niejun","聂军"),
                    new KeyValuePair<string, string>("林德芳","lindefang"),
                    new KeyValuePair<string, string>("lindefang","林德芳"),
                    new KeyValuePair<string, string>("张亮","zhangliang"),
                    new KeyValuePair<string, string>("zhangliang","张亮"),
                    new KeyValuePair<string, string>("林正周","linzhengzhou"),
                    new KeyValuePair<string, string>("linzhengzhou","林正周"),
                    new KeyValuePair<string, string>("武斌","wubin"),
                    new KeyValuePair<string, string>("wubin","武斌"),
                    new KeyValuePair<string, string>("包宗设","baozongshe"),
                    new KeyValuePair<string, string>("baozongshe","包宗设"),
                    new KeyValuePair<string, string>("李长彬","lichangbin"),
                    new KeyValuePair<string, string>("lichangbin","李长彬"),
                    new KeyValuePair<string, string>("邱伟林","qiuweilin"),
                    new KeyValuePair<string, string>("qiuweilin","邱伟林")
                };
                if (zjllist.Exists(p => p.Key == Admin.UserName))
                {
                    AdminInfo sameaccount = Admins.Instance.GetAdminByName(zjllist.Find(p => p.Key == Admin.UserName).Value);
                    if (sameaccount != null)
                        Admins.Instance.ChangePassword(sameaccount.ID, EncryptString.MD5(TxtOldPassword.Text), EncryptString.MD5(TxtNewUserPassword.Text));
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