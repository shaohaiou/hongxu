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
using Hx.Car.Entity;
using Hx.Car;

namespace Hx.BackAdmin.dayreport
{
    public partial class dailyreportcheck : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        protected override void Check()
        {
            string Nm = GetString("Nm");
            int Id = GetInt("Id");
            int Mm = GetInt("Mm");

            if (string.IsNullOrEmpty(Nm) || Id == 0 || Mm == 0 || CurrentUser == null || !CheckUser())
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
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
        private string currentquery = string.Empty;
        protected string CurrentQuery
        {
            get
            {
                if (string.IsNullOrEmpty(currentquery))
                {
                    foreach (string q in Request.QueryString.AllKeys)
                    {
                        if (q != "dep")
                        {
                            currentquery += (string.IsNullOrEmpty(currentquery) ? string.Empty : "&") + q + "=" + Request.QueryString[q];
                        }
                    }
                }

                return currentquery;
            }
        }

        private DayReportDep? currentdep = null;
        protected DayReportDep CurrentDep
        {
            get
            {
                if (!currentdep.HasValue)
                {
                    currentdep = (DayReportDep)GetInt("dep");
                    if (string.IsNullOrEmpty(GetString("dep")))
                        currentdep = CurrentUser.DayReportDep;
                    string[] deppowers = CurrentUser.DayReportCheckDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (!deppowers.Contains(((int)currentdep).ToString()) && deppowers.Length > 0)
                        currentdep = (DayReportDep)DataConvert.SafeInt(deppowers.First());
                }
                return currentdep.Value;
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

        private DailyReportInfo currentreportinfo = null;
        protected DailyReportInfo CurrentReportInfo
        {
            get
            {
                if (currentreportinfo == null)
                {
                    int corpid = DataConvert.SafeInt(ddlCorp.SelectedValue);
                    DateTime day = DateTime.Today;
                    if (DateTime.TryParse(txtDate.Text, out day))
                    {
                        currentreportinfo = DailyReports.Instance.GetModel(corpid, CurrentDep, day, true);
                    }
                }
                return currentreportinfo;
            }
        }

        private Dictionary<string, string> currentreportdata = new Dictionary<string, string>();
        protected Dictionary<string, string> CurrentReportData
        {
            get
            {
                if (currentreportdata.Count == 0)
                {
                    if (CurrentReportInfo != null)
                    {
                        string strReport = CurrentReportInfo.SCReport;
                        try
                        {
                            currentreportdata = json.Deserialize<Dictionary<string, string>>(strReport);
                        }
                        catch { };
                    }
                }
                return currentreportdata;
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
            txtDate.Text = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");

            string[] corppowers = CurrentUser.DayReportCheckCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<CorporationInfo> corps = Corporations.Instance.GetList(true);
            ddlCorp.DataSource = corps.FindAll(c => corppowers.Contains(c.ID.ToString()) || (c.ID == CurrentUser.CorporationID && corppowers.Length == 0));
            ddlCorp.DataTextField = "Name";
            ddlCorp.DataValueField = "ID";
            ddlCorp.DataBind();
            SetSelectedByValue(ddlCorp, CurrentUser.CorporationID.ToString());

            if (!string.IsNullOrEmpty(GetString("date")))
                txtDate.Text = GetString("date");
            if (!string.IsNullOrEmpty(GetString("corp")))
                SetSelectedByValue(ddlCorp, GetString("corp"));
        }

        private void LoadData()
        {
            bool hasallsupport = true;
            List<DailyReportModuleInfo> mlist = DayReportModules.Instance.GetList(CurrentDep, true);
            foreach (DailyReportModuleInfo m in mlist)
            {
                string dvalue = CurrentReportData.Count > 0 && CurrentReportData.Keys.Contains(m.ID.ToString()) ? CurrentReportData[m.ID.ToString()] : string.Empty;
                if (string.IsNullOrEmpty(dvalue) && m.Mustinput)
                {
                    hasallsupport = false;
                    break;
                }
            }
            if (hasallsupport)
            {
                spRemind.InnerHtml = "数据完整";
                spRemind.Attributes["class"] = "green";
            }
            else
            {
                spRemind.InnerHtml = "数据不完整";
                spRemind.Attributes["class"] = "red";
            }

            txtDailyReportCheckStatus.Text = string.Empty;
            txtCheckRemark.Text = string.Empty;
            if (CurrentReportInfo != null)
            {
                txtDailyReportCheckStatus.Text = CurrentReportInfo.DailyReportCheckStatus.ToString();
                if (CurrentReportInfo.DailyReportCheckStatus == DailyReportCheckStatus.未审核)
                    txtDailyReportCheckStatus.CssClass = "gray";
                else if (CurrentReportInfo.DailyReportCheckStatus == DailyReportCheckStatus.审核通过)
                {
                    txtDailyReportCheckStatus.CssClass = "green";
                    txtCheckRemark.Text = CurrentReportInfo.DailyReportCheckRemark;
                    btnPass.Visible = false;
                    btnUnPass.Visible = false;
                }
                else if (CurrentReportInfo.DailyReportCheckStatus == DailyReportCheckStatus.审核不通过)
                {
                    txtDailyReportCheckStatus.CssClass = "red";
                    txtCheckRemark.Text = CurrentReportInfo.DailyReportCheckRemark;
                    btnPass.Visible = false;
                    btnUnPass.Visible = false;
                }
            }
            else
            {
                btnPass.Visible = false;
                btnUnPass.Visible = false;
            }
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            string query = string.Empty;
            foreach (string q in Request.QueryString.AllKeys)
            {
                if (q != "date")
                {
                    query += (string.IsNullOrEmpty(query) ? string.Empty : "&") + q + "=" + Request.QueryString[q];
                }
            }
            query += "&date=" + txtDate.Text;
            Response.Redirect("dailyreportcheck.aspx?" + query);
        }

        protected void ddlCorp_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = string.Empty;
            foreach (string q in Request.QueryString.AllKeys)
            {
                if (q != "corp")
                {
                    query += (string.IsNullOrEmpty(query) ? string.Empty : "&") + q + "=" + Request.QueryString[q];
                }
            }
            query += "&corp=" + ddlCorp.SelectedValue;
            Response.Redirect("dailyreportcheck.aspx?" + query);
        }

        protected void btnPass_Click(object sender, EventArgs e)
        {
            if (CurrentReportInfo != null)
            {
                CurrentReportInfo.DailyReportCheckStatus = DailyReportCheckStatus.审核通过;
                CurrentReportInfo.DailyReportCheckRemark = txtCheckRemark.Text;

                DailyReports.Instance.CreateAndUpdate(CurrentReportInfo, CurrentDep);

                DailyReportCheckHistoryInfo checkhistory = new DailyReportCheckHistoryInfo();
                checkhistory.DayUnique = CurrentReportInfo.DayUnique;
                checkhistory.CheckedInfo = CurrentReportInfo;
                checkhistory.Creator = CurrentUser.UserName;
                checkhistory.CreatorCorporationID = CurrentUser.CorporationID;
                checkhistory.CreatorCorporationName = CurrentUser.CorporationName;
                checkhistory.CreatorDepartment = CurrentUser.DayReportDep;
                checkhistory.ReportDepartment = CurrentDep;
                checkhistory.ReportCorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue);
                DailyReports.Instance.CreateCheckHistory(checkhistory);

                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? UrlDecode(CurrentUrl) : FromUrl);
            }
        }

        protected void btnUnPass_Click(object sender, EventArgs e)
        {
            if (CurrentReportInfo != null)
            {
                CurrentReportInfo.DailyReportCheckStatus = DailyReportCheckStatus.审核不通过;
                CurrentReportInfo.DailyReportCheckRemark = txtCheckRemark.Text;

                DailyReports.Instance.CreateAndUpdate(CurrentReportInfo, CurrentDep);

                DailyReportCheckHistoryInfo checkhistory = new DailyReportCheckHistoryInfo();
                checkhistory.DayUnique = CurrentReportInfo.DayUnique;
                checkhistory.CheckedInfo = CurrentReportInfo;
                checkhistory.Creator = CurrentUser.UserName;
                checkhistory.CreatorCorporationID = CurrentUser.CorporationID;
                checkhistory.CreatorCorporationName = CurrentUser.CorporationName;
                checkhistory.CreatorDepartment = CurrentUser.DayReportDep;
                checkhistory.ReportDepartment = CurrentDep;
                checkhistory.ReportCorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue);
                DailyReports.Instance.CreateCheckHistory(checkhistory);

                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? UrlDecode(CurrentUrl) : FromUrl);
            }
        }

        private bool CheckUser()
        {
            string[] deppowers = CurrentUser.DayReportCheckDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deppowers.Contains(((int)CurrentDep).ToString()))
                return false;

            return true;
        }

        protected string GetDepHide(DayReportDep dep)
        {
            string result = string.Empty;

            string[] deparray = CurrentUser.DayReportCheckDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deparray.Contains(((int)dep).ToString()))
                result = "style=\"display:none;\"";

            return result;
        }

        protected string GetDetail()
        {
            string result = string.Empty;

            List<DailyReportModuleInfo> mlist = DayReportModules.Instance.GetList(CurrentDep, true).OrderBy(l => l.Sort).ToList();
            foreach (DailyReportModuleInfo m in mlist)
            {
                string dvalue = CurrentReportData.Count > 0 && CurrentReportData.Keys.Contains(m.ID.ToString()) ? CurrentReportData[m.ID.ToString()] : string.Empty;
                result += "<tr><td class=\"bg4 tr\">" + m.Name + ":</td><td>" + dvalue + "</td></tr>";
            }

            return result;
        }
    }
}