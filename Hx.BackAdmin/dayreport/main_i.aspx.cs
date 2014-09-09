using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.dayreport
{
    public partial class main_i : AdminBase
    {
        protected override void Check()
        {
            string Nm = GetString("Nm");
            int Id = GetInt("Id");
            int Mm = GetInt("Mm");

            if (string.IsNullOrEmpty(Nm) || Id == 0 || Mm == 0 || CurrentUser == null || !CheckUser())
            {
                Response.Clear();
                Response.Write(string.Format("您没有权限操作！"));
                Response.End();
                return;
            }
            if (Mm != (DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day) * Id * 3)
            {
                Response.Clear();
                Response.Write("非法操作！");
                Response.End();
                return;
            }
        }

        private DayReportUserInfo currentuser = null;
        protected DayReportUserInfo CurrentUser
        {
            get
            {
                if (currentuser == null)
                    currentuser = DayReportUsers.Instance.GetModel(GetInt("Id").ToString(), true);
                return currentuser;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            sk.Attributes["src"] = string.Format("main_is.aspx?Nm={0}&Id={1}&Mm={2}", GetString("Nm"), GetString("Id"), GetString("Mm"));

            if (!string.IsNullOrEmpty(CurrentUser.DayReportModulePowerSetting) || !string.IsNullOrEmpty(CurrentUser.DayReportDepPowerSetting))
                ztk.Attributes["src"] = string.Format("dailyreport.aspx?Nm={0}&Id={1}&Mm={2}",GetString("Nm"),GetString("Id"),GetString("Mm"));
            else if(!string.IsNullOrEmpty(CurrentUser.DayReportViewCorpPowerSetting) && !string.IsNullOrEmpty(CurrentUser.DayReportViewDepPowerSetting))
                ztk.Attributes["src"] = string.Format("dailyreportview.aspx?Nm={0}&Id={1}&Mm={2}", GetString("Nm"), GetString("Id"), GetString("Mm"));
            else if (!string.IsNullOrEmpty(CurrentUser.MonthlyTargetCorpPowerSetting) && !string.IsNullOrEmpty(CurrentUser.MonthlyTargetDepPowerSetting))
                ztk.Attributes["src"] = string.Format("monthlytarget.aspx?Nm={0}&Id={1}&Mm={2}", GetString("Nm"), GetString("Id"), GetString("Mm"));
            else
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private bool CheckUser()
        {
            if (string.IsNullOrEmpty(CurrentUser.DayReportDepPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.DayReportModulePowerSetting)
                && string.IsNullOrEmpty(CurrentUser.DayReportViewCorpPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.DayReportViewDepPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.MonthlyTargetCorpPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.MonthlyTargetDepPowerSetting))
                return false;

            return true;
        }
    }
}