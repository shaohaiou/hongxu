using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.TaskAndJob.Job
{
    public class VoteAccountJob : IJob
    {
        private static bool isRunning = false;

        public void Execute(System.Xml.XmlNode node)
        {
            if (!isRunning)
            {
                isRunning = true;

                List<VoteSettingInfo> settinglist = WeixinActs.Instance.GetVoteSettingList(true);
                foreach (VoteSettingInfo setting in settinglist)
                {
                    if (setting != null && setting.Switch == 0) continue;
                    WeixinActs.Instance.VoteRecordAccount(setting.ID);
                }
                
                isRunning = false;
            }
        }
    }
}
