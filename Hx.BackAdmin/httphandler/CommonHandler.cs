using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Hx.Tools.Web;
using System.IO;
using Hx.Tools;

namespace Hx.BackAdmin.HttpHandler
{
    public class CommonHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
        private HttpResponse Response;
        private HttpRequest Request;
        private static object sync_helper = new object();
        private string result = "{{\"Value\":\"{0}\",\"Msg\":\"{1}\"}}";

        public void ProcessRequest(HttpContext context)
        {
            Response = context.Response;
            Request = context.Request;

            string action = WebHelper.GetString("action");
            if (action == "cnmaccheck")
            {
                ChebaoMacCheck();
            }
            else
            {
                result = string.Format(result, "fail", "非法操作");
            }

            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        #region 侯牌器网卡验证

        private void ChebaoMacCheck()
        {
            result = "0";

            string mac = WebHelper.GetString("mac");
            if (!string.IsNullOrEmpty(mac))
            {
                lock (sync_helper)
                {
                    string macstr = File.ReadAllText(Utils.GetMapPath("/cnmac.txt"), System.Text.Encoding.UTF8);
                    string[] macs = macstr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (macs.Contains(mac))
                        result = "1";
                }
            }
        }

        #endregion
    }
}