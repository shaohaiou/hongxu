using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Tools;

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

                try
                {
                    List<VoteSettingInfo> settinglist = WeixinActs.Instance.GetVoteSettingList(true);
                    foreach (VoteSettingInfo setting in settinglist)
                    {
                        if (setting != null && setting.Switch == 0) continue;
                        WeixinActs.Instance.VoteRecordAccount(setting.ID);
                    }
                }
                catch (Exception ex)
                {
                    ExpLog.Write(ex);
                }
                
                isRunning = false;
            }
        }
    }
}
