using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Tools;

namespace Hx.BackAdmin
{
    public partial class index : AdminBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (Admin.UserRole == Components.Enumerations.UserRoleType.销售员)
            {
                Response.Redirect("~/car/carquotation.aspx");
            }
            List<KeyValuePair<string, string>> zjllist = new List<KeyValuePair<string, string>>() 
            { 
                new KeyValuePair<string, string>("donghongwei","董红卫|346"),
                new KeyValuePair<string, string>("dhw","董红卫|346"),
                new KeyValuePair<string, string>("xxl","徐象龙|347"),
                new KeyValuePair<string, string>("邱海彬","邱海彬|361"),
                new KeyValuePair<string, string>("qiuhaibin","邱海彬|361"),
                new KeyValuePair<string, string>("陈军","陈军|588"),
                new KeyValuePair<string, string>("chenjun","陈军|588"),
                new KeyValuePair<string, string>("徐晓萍","徐晓萍|704"),
                new KeyValuePair<string, string>("xuxiaoping","徐晓萍|704"),
                new KeyValuePair<string, string>("崔建明","崔建明|818"),
                new KeyValuePair<string, string>("cuijianming","崔建明|818"),
                new KeyValuePair<string, string>("孙海望","孙海望|469"),
                new KeyValuePair<string, string>("sunhaiwang","孙海望|469"),
                new KeyValuePair<string, string>("武仰民","武仰民|2563"),
                new KeyValuePair<string, string>("wuyangmin","武仰民|2563"),
                new KeyValuePair<string, string>("麻立军","麻立军|1161"),
                new KeyValuePair<string, string>("malijun","麻立军|1161"),
                new KeyValuePair<string, string>("项公程","项公程|1238"),
                new KeyValuePair<string, string>("xianggongcheng","项公程|1238"),
                new KeyValuePair<string, string>("刘道从","刘道从|48"),
                new KeyValuePair<string, string>("liudaocong","刘道从|48"),
                new KeyValuePair<string, string>("刘兴龙","刘兴龙|136"),
                new KeyValuePair<string, string>("liuxinglong","刘兴龙|136"),
                new KeyValuePair<string, string>("聂军","聂军|217"),
                new KeyValuePair<string, string>("niejun","聂军|217"),
                new KeyValuePair<string, string>("林德芳","林德芳|1358"),
                new KeyValuePair<string, string>("lindefang","林德芳|1358"),
                new KeyValuePair<string, string>("张亮","张亮|3239"),
                new KeyValuePair<string, string>("zhangliang","张亮|3239"),
                new KeyValuePair<string, string>("林正周","林正周|1567"),
                new KeyValuePair<string, string>("linzhengzhou","林正周|1567"),
                new KeyValuePair<string, string>("武斌","武斌|1025"),
                new KeyValuePair<string, string>("wubin","武斌|1025"),
                new KeyValuePair<string, string>("包宗设","包宗设|2672"),
                new KeyValuePair<string, string>("baozongshe","包宗设|2672"),
                new KeyValuePair<string, string>("李长彬","李长彬|218"),
                new KeyValuePair<string, string>("lichangbin","李长彬|218"),
                new KeyValuePair<string, string>("邱伟林","邱伟林|423"),
                new KeyValuePair<string, string>("qiuweilin","邱伟林|423")
            };
            if (zjllist.Exists(p => p.Key == Admin.UserName))
            {
                if (Admin.Password == EncryptString.MD5("123456"))
                {
                    Response.Redirect("~/user/changewd.aspx");
                }
                else
                {
                    string vstr = zjllist.Find(p => p.Key == Admin.UserName).Value;
                    string nm = vstr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    int id = DataConvert.SafeInt(vstr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[1]);
                    int mn = (DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day) * id * 3;
                    Response.Redirect("~/dayreport/main_i.aspx?Nm=" + nm + "&Id=" + id + "&Mm=" + mn);
                }
            }
        }
    }
}