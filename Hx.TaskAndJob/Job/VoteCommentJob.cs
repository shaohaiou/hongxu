using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.TaskAndJob.Job
{
    public class VoteCommentJob : IJob
    {
        private static bool isRunning = false;

        public void Execute(System.Xml.XmlNode node)
        {
            if (!isRunning)
            {
                isRunning = true;

                bool hasvoterunning = false;
                List<VoteSettingInfo> settinglist = WeixinActs.Instance.GetVoteSettingList(true);
                foreach (VoteSettingInfo setting in settinglist)
                {
                    if (setting != null && setting.Switch == 1)
                    {
                        WeixinActs.Instance.VoteCommentPraiseCountAccount(setting.ID);
                        WeixinActs.Instance.VoteCommentBelittleCountAccount(setting.ID);
                        hasvoterunning = true;
                    }
                }
                if (hasvoterunning)
                    WeixinActs.Instance.VoteCommentCountAccount();

                isRunning = false;
            }
        }
    }
}
