using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components;
using Hx.Components.Entity;

namespace Hx.TaskAndJob.Job
{
    public class BenzvoteAccountJob : IJob
    {
        private static bool isRunning = false;

        public void Execute(System.Xml.XmlNode node)
        {
            if (!isRunning)
            {
                BenzvoteSettingInfo setting = WeixinActs.Instance.GetBenzvoteSetting(true);
                if (setting != null && setting.Switch == 0) { return; };
                //EventLogs.JobLog("开始结算奔驰投票活动投票：BenzvoteAccountJob");
                isRunning = true;
                WeixinActs.Instance.BenzvoteAccount();
                WeixinActs.Instance.ReloadBenzvotePothunterListCache();
                isRunning = false;
                //EventLogs.JobLog("结束结算奔驰投票活动投票：DiskCacheJob");
            }
        }
    }
}
