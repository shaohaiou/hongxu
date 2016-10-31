using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Enumerations;
using Hx.Tools;

namespace Hx.BackAdmin.dayreport
{
    public partial class main_is : AdminBase
    {
        protected override void Check()
        {
            if (string.IsNullOrEmpty(Nm) || Id == 0 || Mm == 0 || CurrentUser == null || !CheckUser())
            {
                Response.Clear();
                Response.Write("您没有权限操作！Nm:" + Nm + " Id:" + Id + " Mm:" + Mm);
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

        private string nm = string.Empty;
        protected string Nm
        {
            get
            {
                if (string.IsNullOrEmpty(nm))
                    nm = GetString("Nm");
                return nm;
            }
        }

        private int id = 0;
        protected int Id
        {
            get
            {
                if (id == 0)
                    id = GetInt("Id");
                return id;
            }
        }

        private int mm = 0;
        protected int Mm
        {
            get
            {
                if (mm == 0)
                    mm = GetInt("Mm");
                return mm;
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

        private CRMReportType? currentcrmreport = null;
        protected CRMReportType CurrentCRMReport
        {
            get
            {
                if (!currentcrmreport.HasValue)
                {
                    currentcrmreport = CRMReportType.客流量登记表;
                    string[] crmrepotpowers = CurrentUser.CRMReportInputPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!crmrepotpowers.Contains(((int)currentcrmreport).ToString()) && crmrepotpowers.Length > 0)
                        currentcrmreport = (CRMReportType)DataConvert.SafeInt(crmrepotpowers.First());
                }
                return currentcrmreport.Value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            dailyreport.Attributes["href"] = string.Format("dailyreport.aspx?Nm={0}&Id={1}&Mm={2}", Nm,Id,Mm);
            monthlytarget.Attributes["href"] = string.Format("monthlytarget.aspx?Nm={0}&Id={1}&Mm={2}", Nm,Id,Mm);
            monthlytargetpre.Attributes["href"] = string.Format("monthlytarget.aspx?Nm={0}&Id={1}&Mm={2}&pre=1", Nm, Id, Mm);
            dailyreportview.Attributes["href"] = string.Format("dailyreportview.aspx?Nm={0}&Id={1}&Mm={2}", Nm,Id,Mm);
            dailyreportviewmul.Attributes["href"] = string.Format("dailyreportviewmul.aspx?Nm={0}&Id={1}&Mm={2}", Nm,Id,Mm);
            dailyreportcheck.Attributes["href"] = string.Format("dailyreportcheck.aspx?Nm={0}&Id={1}&Mm={2}", Nm, Id, Mm);
            crmreportcustomerflow.Attributes["href"] = string.Format("crmreportcustomerflow.aspx?Nm={0}&Id={1}&Mm={2}", Nm,Id,Mm);
            if (CurrentUser.CRMReportExportPowerSetting == "1")
                crmreportcustomerflow.Attributes["href"] = string.Format("crmreportexport.aspx?Nm={0}&Id={1}&Mm={2}", Nm,Id,Mm);
            else
                crmreportcustomerflow.Attributes["href"] = CRMReports.Instance.GetNavUrl(CurrentCRMReport,Nm,Id,Mm);

            if (!string.IsNullOrEmpty(CurrentUser.DayReportModulePowerSetting) || !string.IsNullOrEmpty(CurrentUser.DayReportDepPowerSetting))
                dailyreport.Attributes["class"] = "current";
            else if (!string.IsNullOrEmpty(CurrentUser.DayReportViewCorpPowerSetting) && !string.IsNullOrEmpty(CurrentUser.DayReportViewDepPowerSetting))
                dailyreportview.Attributes["class"] = "current";
            else if (!string.IsNullOrEmpty(CurrentUser.MonthlyTargetCorpPowerSetting) && !string.IsNullOrEmpty(CurrentUser.MonthlyTargetDepPowerSetting))
                monthlytarget.Attributes["class"] = "current";
            else if (CurrentUser.CRMReportExportPowerSetting == "1" || !string.IsNullOrEmpty(CurrentUser.CRMReportInputPowerSetting))
                crmreportcustomerflow.Attributes["class"] = "current";
            else if(!string.IsNullOrEmpty(CurrentUser.DayReportCheckDepPowerSetting))
                dailyreportcheck.Attributes["class"] = "current";

            if (string.IsNullOrEmpty(CurrentUser.DayReportModulePowerSetting) && string.IsNullOrEmpty(CurrentUser.DayReportDepPowerSetting))
                dailyreport.Visible = false;
            if (string.IsNullOrEmpty(CurrentUser.DayReportViewCorpPowerSetting) || string.IsNullOrEmpty(CurrentUser.DayReportViewDepPowerSetting))
                dailyreportview.Visible = false;
            if (CurrentUser.ReportGather != "1")
                dailyreportviewmul.Visible = false;
            if (string.IsNullOrEmpty(CurrentUser.MonthlyTargetCorpPowerSetting) || string.IsNullOrEmpty(CurrentUser.MonthlyTargetDepPowerSetting))
                monthlytarget.Visible = false;
            if (string.IsNullOrEmpty(CurrentUser.DayReportCheckDepPowerSetting))
                dailyreportcheck.Visible = false;
            if (string.IsNullOrEmpty(CurrentUser.DayReportMonthTargetPrePowerSetting) || CurrentUser.DayReportMonthTargetPrePowerSetting == "0")
                monthlytargetpre.Visible = false;
            if (string.IsNullOrEmpty(CurrentUser.CRMReportInputPowerSetting) && CurrentUser.CRMReportExportPowerSetting == "0")
                crmreportcustomerflow.Visible = false;
        }

        private bool CheckUser()
        {
            if (string.IsNullOrEmpty(CurrentUser.DayReportDepPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.DayReportModulePowerSetting)
                && string.IsNullOrEmpty(CurrentUser.DayReportViewCorpPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.DayReportViewDepPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.MonthlyTargetCorpPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.MonthlyTargetDepPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.DayReportCheckDepPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.CRMReportExportPowerSetting)
                && string.IsNullOrEmpty(CurrentUser.CRMReportInputPowerSetting))
                return false;

            return true;
        }
    }
}