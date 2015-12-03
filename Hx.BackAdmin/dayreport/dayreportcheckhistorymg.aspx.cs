using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car;
using Hx.Tools;
using Hx.Components.Enumerations;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Query;
using Hx.Car.Entity;

namespace Hx.BackAdmin.dayreport
{
    public partial class dayreportcheckhistorymg : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        private List<CorporationInfo> corplist = null;
        private List<CorporationInfo> Corplist
        {
            get
            {
                if (corplist == null)
                {
                    corplist = Corporations.Instance.GetList(true).FindAll(c => c.DailyreportShow == 1);
                }
                return corplist;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControlor();
                LoadData();
            }
        }

        private void BindControlor()
        {
            ddlCorporation.DataSource = Corplist;
            ddlCorporation.DataTextField = "Name";
            ddlCorporation.DataValueField = "ID";
            ddlCorporation.DataBind();
            ddlCorporation.Items.Insert(0, new ListItem("-所属公司-", "-1"));

            ddlReportCorporation.DataSource = Corplist;
            ddlReportCorporation.DataTextField = "Name";
            ddlReportCorporation.DataValueField = "ID";
            ddlReportCorporation.DataBind();
            ddlReportCorporation.Items.Insert(0, new ListItem("-所属公司-", "-1"));

            ddlDepartment.DataSource = EnumExtensions.ToTable<DayReportDep>();
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataValueField = "Value";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("-所属部门-", "-1"));

            ddlReportDepartment.DataSource = EnumExtensions.ToTable<DayReportDep>();
            ddlReportDepartment.DataTextField = "Name";
            ddlReportDepartment.DataValueField = "Value";
            ddlReportDepartment.DataBind();
            ddlReportDepartment.Items.Insert(0, new ListItem("-所属部门-", "-1"));

            if (GetInt("rcorp") > 0)
                SetSelectedByValue(ddlReportCorporation, GetString("rcorp"));
            if (GetInt("corp") > 0)
                SetSelectedByValue(ddlCorporation, GetString("corp"));
            if (GetInt("dep", -1) >= 0)
                SetSelectedByValue(ddlDepartment, GetString("dep"));
            if (GetInt("rdep", -1) >= 0)
                SetSelectedByValue(ddlReportDepartment, GetString("rdep"));

            txtOperator.Text = GetString("oper");
            txtDate.Text = GetString("date");
        }

        private void LoadData()
        {
            int pageindex = GetInt("page", 1);
            if (pageindex < 1)
            {
                pageindex = 1;
            }
            int pagesize = GetInt("pagesize", 10);
            int total = 0;

            DateTime date = DateTime.Now;
            DailyReportCheckHistoryQuery query = new DailyReportCheckHistoryQuery();
            if (DateTime.TryParse(txtDate.Text, out date))
                query.DayUnique = date.ToString("yyyyMMdd");
            if (ddlCorporation.SelectedIndex > 0)
                query.CorporationID = DataConvert.SafeInt(ddlCorporation.SelectedValue);
            if (ddlReportCorporation.SelectedIndex > 0)
                query.ReportCorprationID = DataConvert.SafeInt(ddlReportCorporation.SelectedValue);
            if (ddlDepartment.SelectedIndex > 0)
                query.Department = (DayReportDep)DataConvert.SafeInt(ddlDepartment.SelectedValue);
            if (ddlReportDepartment.SelectedIndex > 0)
                query.ReportDepartment = (DayReportDep)DataConvert.SafeInt(ddlReportDepartment.SelectedValue);
            if (!string.IsNullOrEmpty(txtOperator.Text.Trim()))
                query.Operator = txtOperator.Text.Trim();

            List<DailyReportCheckHistoryInfo> list = DailyReports.Instance.GetCheckHistorys(pageindex, pagesize, query, ref total);
            rpthistory.DataSource = list;
            rpthistory.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;
        }

        protected string GetDetail(string reportCorporationID, string reportDepartment, object checkedinfo)
        {
            string result = string.Empty;
            DailyReportInfo reportinfo = checkedinfo as DailyReportInfo;
            if (reportinfo != null)
            {
                result += "<div><span>审核备注:</span>" + reportinfo.DailyReportCheckRemark + "</div>";

                Dictionary<string, string> data = new Dictionary<string, string>();
                try
                {
                    data = json.Deserialize<Dictionary<string, string>>(reportinfo.SCReport);
                }
                catch { };
                if (data.Count > 0)
                {
                    CorporationInfo corp = Corporations.Instance.GetModel(DataConvert.SafeInt(reportCorporationID), true);
                    if (corp != null)
                    {
                        result += "<div><span>公司:</span>" + corp.Name + "</div>";
                    }
                    result += "<div><span>部门:</span>" + reportDepartment + "</div>";
                    List<DailyReportModuleInfo> mlist = DayReportModules.Instance.GetList((DayReportDep)Enum.Parse(typeof(DayReportDep), reportDepartment), true).OrderBy(l => l.Sort).ToList();
                    foreach (DailyReportModuleInfo m in mlist)
                    {
                        string dvalue = data.Count > 0 && data.Keys.Contains(m.ID.ToString()) ? data[m.ID.ToString()] : string.Empty;
                        result += "<div><span>" + m.Name + ":</span>" + dvalue + "</div>";
                    }
                }
            }
            return result;
        }

        protected string GetCorpname(string corpid)
        {
            string result = string.Empty;
            int id = DataConvert.SafeInt(corpid);
            if (id > 0 && Corplist.Exists(c => c.ID == id))
            {
                result = Corplist.Find(c => c.ID == id).Name;
            }

            return result;
        }
    }
}