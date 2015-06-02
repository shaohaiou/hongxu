using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Tools;
using Hx.Components.Enumerations;
using Hx.Components.Query;
using Hx.Components.Entity;
using Hx.Components;

namespace Hx.BackAdmin.dayreport
{
    public partial class monthlytargethistorymg : AdminBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        private List<CorporationInfo> corplist = null;
        private List<CorporationInfo> Corplist
        {
            get
            {
                if (corplist == null)
                {
                    corplist = Corporations.Instance.GetList(true);
                }
                return corplist;
            }
        }

        public MonthlyTargetHistoryInfo CurrentHistory { get; set; }

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
            int pagesize = GetInt("pagesize", search_fy.PageSize);
            int total = 0;

            DateTime date = DateTime.Now;
            MonthlyTargetHistoryQuery query = new MonthlyTargetHistoryQuery();
            if (DateTime.TryParse(txtDate.Text, out date))
                query.MonthUnique = date.ToString("yyyyMM");
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

            List<MonthlyTargetHistoryInfo> list = MonthlyTargets.Instance.GetHistorys(pageindex, pagesize, query, ref total);
            rpthistory.DataSource = list;
            rpthistory.DataBind();

            search_fy.RecordCount = total;
            search_fy.PageSize = pagesize;
        }

        protected string GetDetail(object history)
        {
            string result = string.Empty;
            try
            {
                CurrentHistory = (MonthlyTargetHistoryInfo)history;
                if (CurrentHistory == null) return string.Empty;
                Dictionary<string, string> data = new Dictionary<string, string>();
                try
                {
                    data = json.Deserialize<Dictionary<string, string>>(CurrentHistory.Detail);
                }
                catch { };
                CorporationInfo corp = Corporations.Instance.GetModel(DataConvert.SafeInt(CurrentHistory.ReportCorporationID), true);
                if (corp != null)
                {
                    result += "<div><span>公司:</span>" + corp.Name + "</div>";
                }
                result += "<div><span>部门:</span>" + CurrentHistory.ReportDepartment + "</div>";
                #region 财务部
                if (CurrentHistory.CreatorDepartment == DayReportDep.财务部)
                {
                    result += "<div><span>资金余额:</span>" + CurrentHistory.Modify.CWzjye + "</div>";
                    result += "<div><span>POS未到帐:</span>" + CurrentHistory.Modify.CWposwdz + "</div>";
                    result += "<div><span>银行帐户余额:</span>" + CurrentHistory.Modify.CWyhzhye + "</div>";
                    result += "<div><span>农行帐户余额:</span>" + CurrentHistory.Modify.CWnhzhye + "</div>";
                    result += "<div><span>中行帐户余额:</span>" + CurrentHistory.Modify.CWzhzhye + "</div>";
                    result += "<div><span>工行帐户余额:</span>" + CurrentHistory.Modify.CWghzhye + "</div>";
                    result += "<div><span>建行帐户余额:</span>" + CurrentHistory.Modify.CWjianhzhye + "</div>";
                    result += "<div><span>交行帐户余额:</span>" + CurrentHistory.Modify.CWjhzhye + "</div>";
                    result += "<div><span>民生帐户余额:</span>" + CurrentHistory.Modify.CWmszhye + "</div>";
                    result += "<div><span>平安帐户余额:</span>" + CurrentHistory.Modify.CWpazhye + "</div>";
                    result += "<div><span>中信帐户余额:</span>" + CurrentHistory.Modify.CWzxzhye + "</div>";
                    result += "<div><span>华夏帐户余额:</span>" + CurrentHistory.Modify.CWhxzhye + "</div>";
                    result += "<div><span>浙商帐户余额:</span>" + CurrentHistory.Modify.CWzszhye + "</div>";
                    result += "<div><span>泰隆帐户余额:</span>" + CurrentHistory.Modify.CWtlzhye + "</div>";
                    result += "<div><span>其他银行帐户余额:</span>" + CurrentHistory.Modify.CWqtyhzhye + "</div>";
                    result += "<div><span>现金合计:</span>" + CurrentHistory.Modify.CWxjczhj + "</div>";
                    result += "<div><span>留存现金:</span>" + CurrentHistory.Modify.CWlcxj + "</div>";
                    result += "<div><span>银承、贷款到期:</span>" + CurrentHistory.Modify.CWycdkdq + "</div>";
                }
                #endregion
                if (data.Count > 0)
                {
                    #region 销售部

                    if (CurrentHistory.CreatorDepartment == DayReportDep.销售部)
                    {
                        result += "<div><span>在途车辆:</span>" + CurrentHistory.Modify.XSztcl + "</div>";
                        result += "<div><span>车辆平均单价:</span>" + CurrentHistory.Modify.XSclpjdj + "</div>";
                        result += "<div><span>周转天数:</span>" + CurrentHistory.Modify.XSzzts + "</div>";
                        result += "<div><span>展厅占比:</span>" + CurrentHistory.Modify.XSztzb + "</div>";
                        result += "<div><span>展厅留档率:</span>" + CurrentHistory.Modify.XSztldl + "</div>";
                        result += "<div><span>展厅成交率:</span>" + CurrentHistory.Modify.XSztcjl + "</div>";
                        result += "<div><span>上牌率:</span>" + CurrentHistory.Modify.XSspl + "</div>";
                        result += "<div><span>展厅保险率:</span>" + CurrentHistory.Modify.XSztbxl + "</div>";
                        result += "<div><span>美容交车率:</span>" + CurrentHistory.Modify.XSmrjcl + "</div>";
                        result += "<div><span>延保渗透率:</span>" + CurrentHistory.Modify.XSybstl + "</div>";
                        result += "<div><span>展厅精品前装率:</span>" + CurrentHistory.Modify.XSztjpqzl + "</div>";
                        result += "<div><span>按揭率:</span>" + CurrentHistory.Modify.XSajl + "</div>";
                        result += "<div><span>免费保养渗透率:</span>" + CurrentHistory.Modify.XSmfbystl + "</div>";
                        result += "<div><span>总销售台次:</span>" + CurrentHistory.Modify.XSzxstc + "</div>";
                        result += "<div><span>上牌单台:</span>" + CurrentHistory.Modify.XSspdt + "</div>";
                        result += "<div><span>展厅保险单台:</span>" + CurrentHistory.Modify.XSztbxdt + "</div>";
                        result += "<div><span>美容单台:</span>" + CurrentHistory.Modify.XSmrdt + "</div>";
                        result += "<div><span>展厅精品平均单台:</span>" + CurrentHistory.Modify.XSztjppjdt + "</div>";
                        result += "<div><span>二网精品平均单台:</span>" + CurrentHistory.Modify.XSewjppjdt + "</div>";
                        result += "<div><span>销售置换台次:</span>" + CurrentHistory.Modify.XSxszhtc + "</div>";
                        result += "<div><span>按揭平均单台:</span>" + CurrentHistory.Modify.XSajpjdt + "</div>";
                        result += "<div><span>免费保养单台:</span>" + CurrentHistory.Modify.XSmfbydt + "</div>";
                        result += "<div><span>他品牌销售台次:</span>" + CurrentHistory.Modify.XStppxstc + "</div>";
                        result += "<div><span>他品牌单车毛利:</span>" + CurrentHistory.Modify.XStppdcml + "</div>";
                        result += "<div><span>他品牌综合毛利:</span>" + CurrentHistory.Modify.XStppzhml + "</div>";
                        result += "<div><span>他品牌平均单台:</span>" + CurrentHistory.Modify.XStpppjdt + "</div>";
                    }

                    #endregion
                    #region 市场部

                    if (CurrentHistory.CreatorDepartment == DayReportDep.市场部)
                    {
                        result += "<div><span>上月粉丝量:</span>" + CurrentHistory.Modify.SCsyfsl + "</div>";
                        result += "<div><span>首次到访达成率:</span>" + CurrentHistory.Modify.SCscdfdcl + "</div>";
                    }
                    #endregion
                    if (CurrentHistory.CreatorDepartment != DayReportDep.财务部)
                    {
                        List<DailyReportModuleInfo> mlist = DayReportModules.Instance.GetList(CurrentHistory.ReportDepartment, true).OrderBy(l => l.Sort).ToList();
                        foreach (DailyReportModuleInfo m in mlist)
                        {
                            if (data.Keys.Contains(m.ID.ToString()))
                            {
                                result += "<div><span>" + m.Name + ":</span>" + data[m.ID.ToString()] + "</div>";
                            }
                        }
                    }
                }
            }
            catch { }
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