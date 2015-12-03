using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using System.Data;
using Hx.Components.Enumerations;
using Hx.Tools;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Car;
using Hx.Car.Entity;

namespace Hx.BackAdmin.dayreport
{
    public partial class dayreportuseredit : AdminBase
    {
        private DataTable departmentlist = null;
        private DataTable DepartmentList
        {
            get
            {
                if (departmentlist == null)
                    departmentlist = EnumExtensions.ToTable<DayReportDep>();
                return departmentlist;
            }
        }

        private DayReportUserInfo currentuser = null;
        protected DayReportUserInfo CurrentUser
        {
            get
            {
                if (currentuser == null)
                    currentuser = DayReportUsers.Instance.GetModel(GetString("usertag"));
                return currentuser;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
                LoadData();
            }
        }

        private void BindControler()
        {
            rptDepartment.DataSource = DepartmentList;
            rptDepartment.DataBind();

            rptTargetDep.DataSource = DepartmentList;
            rptTargetDep.DataBind();

            List<CorporationInfo> corplist = Corporations.Instance.GetList(true).FindAll(c=>c.DailyreportShow == 1);
            rptDayReportCorp.DataSource = corplist;
            rptDayReportCorp.DataBind();

            rptTargetCorp.DataSource = corplist;
            rptTargetCorp.DataBind();

            rptDayReportViewCorp.DataSource = corplist;
            rptDayReportViewCorp.DataBind();

            rptDayReportViewDep.DataSource = DepartmentList;
            rptDayReportViewDep.DataBind();

            rptDayReportCheckCorp.DataSource = corplist;
            rptDayReportCheckCorp.DataBind();

            rptDayReportCheckDep.DataSource = DepartmentList;
            rptDayReportCheckDep.DataBind();

            rptCRMReportInput.DataSource = EnumExtensions.ToTable<CRMReportType>();
            rptCRMReportInput.DataBind();
        }

        private void LoadData()
        {
            if (CurrentUser != null)
            {
                cbxAllowModify.Checked = CurrentUser.AllowModify == "1";
                cbxReportGather.Checked = CurrentUser.ReportGather == "1";
                hdnDayReportDep.Value = CurrentUser.DayReportDepPowerSetting;
                hdnModule.Value = CurrentUser.DayReportModulePowerSetting;
                hdnTargetCorp.Value = CurrentUser.MonthlyTargetCorpPowerSetting;
                hdnTargetDep.Value = CurrentUser.MonthlyTargetDepPowerSetting;
                hdnDayReportViewCorp.Value = CurrentUser.DayReportViewCorpPowerSetting;
                hdnDayReportViewDep.Value = CurrentUser.DayReportViewDepPowerSetting;
                hdnDayReportCheckCorp.Value = CurrentUser.DayReportCheckCorpPowerSetting;
                hdnDayReportCheckDep.Value = CurrentUser.DayReportCheckDepPowerSetting;
                hdnDayReportCorp.Value = CurrentUser.DayReportCorpPowerSetting;
                cbxCRMReportExport.Checked = CurrentUser.CRMReportExportPowerSetting == "1";
                hdnCRMReportInput.Value = CurrentUser.CRMReportInputPowerSetting;
            }
            else
            {
                WriteErrorMessage("错误提示", "非法用户",string.IsNullOrEmpty(FromUrl) ? "~/dayreport/dayreportusermg.aspx" : FromUrl);
            }
        }

        protected string SetModule(string id)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] modules = CurrentUser.DayReportModulePowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (modules.Contains(id))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetDayReportDep(string v)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] deps = CurrentUser.DayReportDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (deps.Contains(v))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetDayReportCorp(string id)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] corps = CurrentUser.DayReportCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (corps.Contains(id))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetTargetCorp(string id)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] corps = CurrentUser.MonthlyTargetCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (corps.Contains(id))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetTargetDep(string v)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] deps = CurrentUser.MonthlyTargetDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (deps.Contains(v))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetDayReportViewCorp(string id)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] corps = CurrentUser.DayReportViewCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (corps.Contains(id))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetDayReportViewDep(string v)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] deps = CurrentUser.DayReportViewDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (deps.Contains(v))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetDayReportCheckCorp(string id)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] corps = CurrentUser.DayReportCheckCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (corps.Contains(id))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetDayReportCheckDep(string v)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] deps = CurrentUser.DayReportCheckDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (deps.Contains(v))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string SetCRMReportInput(string v)
        {
            string result = string.Empty;

            if (CurrentUser != null)
            {
                string[] p = CurrentUser.CRMReportInputPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Contains(v))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected void rptDepartment_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Repeater rptDayReportModule = (Repeater)e.Item.FindControl("rptDayReportModule");
                if(rptDayReportModule != null)
                {
                    DataRowView row = (DataRowView)e.Item.DataItem;
                    DayReportDep dep = (DayReportDep)DataConvert.SafeInt(row["Value"]);
                    rptDayReportModule.DataSource = DayReportModules.Instance.GetList(dep).OrderBy(l => l.Sort).ToList();
                    rptDayReportModule.DataBind();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DayReportUserInfo user = CurrentUser;
            user.AllowModify = cbxAllowModify.Checked ? "1" : "0";
            user.ReportGather = cbxReportGather.Checked ? "1" : "0";
            user.DayReportModulePowerSetting = hdnModule.Value;
            user.DayReportDepPowerSetting = hdnDayReportDep.Value;
            user.MonthlyTargetDepPowerSetting = hdnTargetDep.Value;
            user.MonthlyTargetCorpPowerSetting = hdnTargetCorp.Value;
            user.DayReportViewCorpPowerSetting = hdnDayReportViewCorp.Value;
            user.DayReportViewDepPowerSetting = hdnDayReportViewDep.Value;
            user.DayReportCheckCorpPowerSetting = hdnDayReportCheckCorp.Value;
            user.DayReportCheckDepPowerSetting = hdnDayReportCheckDep.Value;
            user.DayReportCorpPowerSetting = hdnDayReportCorp.Value;
            user.CRMReportExportPowerSetting = cbxCRMReportExport.Checked ? "1" : "0";
            user.CRMReportInputPowerSetting = hdnCRMReportInput.Value;

            DayReportUsers.Instance.Update(user);

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/dayreport/dayreportusermg.aspx" : FromUrl);
        }
    }
}