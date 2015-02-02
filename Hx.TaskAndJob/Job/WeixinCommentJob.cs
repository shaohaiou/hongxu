using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.TaskAndJob.Job
{
    public class WeixinCommentJob : IJob
    {
        private static bool isRunning = false;

        public void Execute(System.Xml.XmlNode node)
        {
            if (!isRunning)
            {
                JituanvoteSettingInfo setting = WeixinActs.Instance.GetJituanvoteSetting(true);
                BenzvoteSettingInfo setting1 = WeixinActs.Instance.GetBenzvoteSetting(true);
                if (setting != null && setting.Switch == 0 && setting1 != null && setting1.Switch == 0) { return; };

                isRunning = true;
                WeixinActs.Instance.CommentCountAccount();
                WeixinActs.Instance.CommentPraiseCountAccount();
                WeixinActs.Instance.CommentBelittleCountAccount();
                isRunning = false;
            }
        }
    }
}
