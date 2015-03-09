using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Hx.Components;
using Hx.Car;
using Hx.TaskAndJob.Job;
using Hx.Tools;
using Hx.Components.Entity;

namespace Hx.BackAdmin
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码

            try
            {
                Start();
            }
            catch { }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码

        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码
            HttpApplication app = (HttpApplication)sender;
            ExpLog.Write(app.Context.Server.GetLastError());
        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码

        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。

        }

        private void Start()
        {
            Jobs.Instance().Start();                        //任务启动
#if DEBUG
            EventLogs.WebLog("网站启动");//写入系统日志信息
#endif

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                CarBrands.Instance.ReloadCarBrandCache();
                Cars.Instance.ReloadAllCarList();
                Cars.Instance.ReloadCarListBycChangs();
                Sybxs.Instance.ReloadSybxListCache();
                Banks.Instance.ReloadBankListCache();
                Corporations.Instance.ReloadCorporationListCache();
                CarBrands.Instance.ReloadCarBrandCacheByCorporation();
                DayReportUsers.Instance.ReloadDayReportUserListCache();
                DayReportModules.Instance.ReloadDailyReportModuleListCache();
                JobOffers.Instance.ReloadJobOfferListCache();
                CorpMiens.Instance.ReloadCorpMienListCache();

                BenzvoteSettingInfo benzvotesetting = WeixinActs.Instance.GetBenzvoteSetting();
                if (benzvotesetting != null && benzvotesetting.Switch == 1)
                {
                    WeixinActs.Instance.ReloadBenzvoteSetting();
                    WeixinActs.Instance.ReloadAllBenzvote();
                    WeixinActs.Instance.ReloadBenzvotePothunterListCache();
                }
                JituanvoteSettingInfo jituanvotesetting = WeixinActs.Instance.GetJituanvoteSetting();
                if (jituanvotesetting != null && jituanvotesetting.Switch == 1)
                {
                    WeixinActs.Instance.ReloadJituanvoteSetting();
                    WeixinActs.Instance.ReloadAllJituanvote();
                    WeixinActs.Instance.ReloadJituanvotePothunterListCache();
                }
                if ((jituanvotesetting != null && jituanvotesetting.Switch == 1) || (benzvotesetting != null && benzvotesetting.Switch == 1))
                {
                    WeixinActs.Instance.ReloadComments();
                }
            }
            catch { }
        }

    }
}
