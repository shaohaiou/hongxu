using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Hx.Tools.Web;
using Hx.Components.Web;
using Hx.TaskAndJob.Job;
using Hx.Car;
using Hx.Components;
using Hx.Components.Entity;
using Hx.Tools;

namespace Hx.BackAdmin.HttpHandler
{
    public class BackAdminActionHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private HttpResponse Response;
        private HttpRequest Request;
        private string result = "{{\"Value\":\"{0}\",\"Msg\":\"{1}\"}}";

        public void ProcessRequest(HttpContext context)
        {
            Response = context.Response;
            Request = context.Request;

            if (!HXContext.Current.AdminCheck)
            {
                result = string.Format(result, "fail", "您没有权限操作");
            }
            else
            {
                string action = WebHelper.GetString("action");

                if (action == "refreshbackadmincache")
                {
                    RefreshBackadminCache();
                }
                else
                {
                    result = string.Format(result, "fail", "非法操作");
                }
            }

            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        private void RefreshBackadminCache()
        {
            try
            {
                CarBrands.Instance.RecheckCarBrand();
                Cars.Instance.ReloadAllCarList();
                Cars.Instance.ReloadCarListBycChangs();
                Sybxs.Instance.ReloadSybxListCache();
                Banks.Instance.ReloadBankListCache();
                Corporations.Instance.ReloadCorporationListCache();
                CarBrands.Instance.ReloadCarBrandCacheByCorporation();
                DayReportUsers.Instance.ReloadDayReportUserListCache();
                DayReportModules.Instance.ReloadDailyReportModuleListCache();

                result = string.Format(result, "success", "");
            }
            catch { result = string.Format(result, "fail", "执行失败"); }
        }
    }

}