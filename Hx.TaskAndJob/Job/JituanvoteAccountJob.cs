using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components;
using Hx.Components.Entity;

namespace Hx.TaskAndJob.Job
{
    public class JituanvoteAccountJob : IJob
    {
        private static bool isRunning = false;

        public void Execute(System.Xml.XmlNode node)
        {
            if (!isRunning)
            {
                JituanvoteSettingInfo setting = WeixinActs.Instance.GetJituanvoteSetting(true);
                if (setting != null && setting.Switch == 0) { return; };

                isRunning = true;
                WeixinActs.Instance.JituanvoteAccount();
                WeixinActs.Instance.ReloadJituanvotePothunterListCache();
                isRunning = false;
            }
        }
    }
}
