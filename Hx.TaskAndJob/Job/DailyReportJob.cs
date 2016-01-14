using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hx.Components.Interface;
using Hx.Tools;

namespace Hx.TaskAndJob.Job
{
    public class DailyReportJob : IJob
    {
        private static bool isRunning = false;

        public void Execute(System.Xml.XmlNode node)
        {
            if (!isRunning)
            {
                isRunning = true;

                try
                {
                    string url = "";
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
