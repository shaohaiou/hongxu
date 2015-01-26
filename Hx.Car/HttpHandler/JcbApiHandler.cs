using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.HttpHandler;
using System.Web;
using Hx.Tools.Web;
using Hx.Car.Entity;
using System.Web.Script.Serialization;
using Hx.Components;
using Hx.Components.Entity;

namespace Hx.Car.HttpHandler
{
    public class JcbApiHandler : RemoteConnection
    {
        public override bool IsReusable
        {
            get { return false; }
        }
        private static JavaScriptSerializer json = new JavaScriptSerializer();

        public override void Process(HttpContext context)
        {
            string result = string.Empty;//需要返回的信息

            //如果通过验证
            string methodName = WebHelper.GetString("action");//获取请求类型
            switch (methodName)
            {
                case "getcarinfo":
                    result = GetCarInfo();
                    break;
                case "getaccountinfo":
                    result = GetAccountInfo();
                    break;
                case "getjcbuserinfo":
                    result = GetJcbUserInfo();
                    break;
                case "createmarketrecord":
                    CreateMarketrecord();
                    break;
                default:
                    result = "{msg:'上传出错！没有参数类型'}";
                    break;
            }
            context.Response.Write(result);
        }

        private string GetCarInfo()
        {
            string result = string.Empty;
            int id = WebHelper.GetInt("id");
            if (id > 0)
            {
                JcbCarInfo entity = JcbCars.Instance.GetModel(id, true);
                result = json.Serialize(entity);
            }

            return result;
        }

        private string GetAccountInfo()
        {
            string result = string.Empty;
            int id = WebHelper.GetInt("id");
            if (id > 0)
            {
                JcbAccountInfo entity = Jcbs.Instance.GetAccountModel(id, true);
                result = json.Serialize(entity);
            }

            return result;
        }

        private string GetJcbUserInfo()
        {
            string result = string.Empty;
            string username = WebHelper.GetString("username");
            string password = WebHelper.GetString("password");
#if DEBUG
            JcbUserInfo entity = new JcbUserInfo()
            {
                ID = 1,
                Administrator = true,
                Mobile = "13515871286",
                Name = "红旭集团",
                UserName = "hxjt"
            };
            result = json.Serialize(entity);
#else
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                if (JcbUsers.Instance.ValiUser(username, password) > 0)
                {
                    JcbUserInfo entity = JcbUsers.Instance.GetUserByName(username);
                    result = json.Serialize(entity);
                }
            }
#endif
            return result;
        }

        private void CreateMarketrecord()
        {
            string datastr = WebHelper.GetString("datastr");
            if (!string.IsNullOrEmpty(datastr))
            {
                JcbMarketrecordInfo entity = json.Deserialize<JcbMarketrecordInfo>(datastr);
                Jcbs.Instance.CreateAndUpdateMarketrecord(entity);
                Jcbs.Instance.ReloadMarketrecordListCache();
            }
        }
    }
}
