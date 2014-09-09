using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Hx.Components;
using Hx.BjWeb.UI;

namespace Hx.BjWeb
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            try
            {
                Start();
            }
            catch { }

        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码
            Jobs.Instance().Stop();
        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码
            ModuleHelper.OnError(sender, e);
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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            ModuleHelper.AuthenticateRequest(sender, e);
        }

        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            ModuleHelper.AuthorizeRequest(sender, e);
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
            
        }
    }
}
