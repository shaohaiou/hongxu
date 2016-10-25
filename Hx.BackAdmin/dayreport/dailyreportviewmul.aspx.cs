using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Entity;
using Hx.Components;
using Hx.Components.Query;
using Hx.Components.Enumerations;
using Hx.Car.Entity;
using Hx.Car;
using Hx.Tools;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using NPOI.HPSF;
using NPOI.SS.Util;
using NPOI.HSSF.Util;

namespace Hx.BackAdmin.dayreport
{
    public partial class dailyreportviewmul : AdminBase
    {
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

        private System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        private string currentquery = string.Empty;
        protected string CurrentQuery
        {
            get
            {
                if (string.IsNullOrEmpty(currentquery))
                {
                    foreach (string q in Request.QueryString.AllKeys)
                    {
                        if (q != "dep" && q != "corp" && q != "date" && q != "date2")
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
                    string[] deppowers = CurrentUser.DayReportViewDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
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

        private string newquery = string.Empty;
        protected string NewQuery
        {
            get
            {
                return newquery;
            }
            set
            {
                newquery = value;
            }
        }

        private string skeyfirstcellrowcount = "skeyfirstcellrowcount";
        public int FirstCellRowCount
        {
            get
            {
                return DataConvert.SafeInt(Session[skeyfirstcellrowcount]);
            }
            set
            {
                Session[skeyfirstcellrowcount] = value;
            }
        }

        private string skeytowrowcount = "skeytowrowcount";
        public int TowRowCount
        {
            get
            {
                return DataConvert.SafeInt(Session[skeytowrowcount]);
            }
            set
            {
                Session[skeytowrowcount] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                BindControler();
            }
            else
            {

                #region 保存查询条件

                NewQuery = "&date=" + txtDate.Text + "&date2=" + txtDate2.Text + "&corp=" + hdnDayReportCorp.Value;

                #endregion
            }
            if (GetString("act") == "tabletoexcel")
            {
                TableToExcel();
            }
            List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
            string[] corppower = CurrentUser.DayReportViewCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            corplist = corplist.FindAll(c => c.DailyreportShow == 1 && corppower.Contains(c.ID.ToString()));
            rptDayReportCorp.DataSource = corplist;
            rptDayReportCorp.DataBind();
        }

        private void BindControler()
        {
            txtDate.Text = !string.IsNullOrEmpty(GetString("date")) ? GetString("date") : (DateTime.Today.ToString("yyyy-MM") + "-01");
            txtDate2.Text = !string.IsNullOrEmpty(GetString("date2")) ? GetString("date2") : DateTime.Today.ToString("yyyy-MM-dd");

            hdnDayReportCorp.Value = !string.IsNullOrEmpty(GetString("corp")) ? GetString("corp") : CurrentUser.DayReportViewCorpPowerSetting;

            NewQuery = "&date=" + txtDate.Text + "&date2=" + txtDate2.Text + "&corp=" + hdnDayReportCorp.Value;

            cbxAllDayReportCorp.Checked = hdnDayReportCorp.Value == CurrentUser.DayReportViewCorpPowerSetting;

            if (CurrentDep == DayReportDep.行政部 || CurrentDep == DayReportDep.财务部 || CurrentDep == DayReportDep.精品部 || CurrentDep == DayReportDep.客服部 || CurrentDep == DayReportDep.金融部)
            {
                btnKeyTarget.Visible = false;
            }
        }

        private void LoadData()
        {
            tblView.Attributes.Remove("style");
            FirstCellRowCount = 2;
            TowRowCount = 5;

            DateTime day = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                spTitle.InnerText = string.Format("{0}年{1}{2}日报汇总", day.Year, string.Format("{0}至{1}", day.ToString("MM月dd日"), day2.ToString("MM月dd日")), CurrentDep.ToString().Replace("部", string.Empty));
                if (CurrentDep == DayReportDep.行政部)
                    spTitle.InnerText = string.Format("{0}年{1}月{2}日报汇总", day.Year, string.Format("{0}至{1}", day.ToString("MM月dd日"), day2.ToString("MM月dd日")), "人事");

                DataTable tbl = GetReportMul(CurrentDep, day, day2);
                tdData.InnerHtml = GetReportStr(CurrentDep, tbl);
            }
        }

        private void LoadKeyData()
        {
            tblView.Attributes.Remove("style");
            FirstCellRowCount = 2;
            TowRowCount = 0;

            DateTime day = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                spTitle.InnerText = string.Format("{0}年{1}月{2}关键指标汇总", day.Year, day.Month, CurrentDep.ToString().Replace("部", string.Empty));
                if (CurrentDep == DayReportDep.行政部)
                    spTitle.InnerText = string.Format("{0}年{1}月{2}关键指标汇总", day.Year, day.Month, "人事");
                if (CurrentDep == DayReportDep.DCC部 && hdnKeyReportType.Value == "dccqdzhl")
                    spTitle.InnerText = string.Format("{0}年{1}月{2}渠道汇总", day.Year, day.Month, CurrentDep.ToString().Replace("部", string.Empty));
                else if (CurrentDep == DayReportDep.DCC部 && hdnKeyReportType.Value == "dccjqdzb")
                    spTitle.InnerText = string.Format("{0}年{1}月{2}网络汇总", day.Year, day.Month, CurrentDep.ToString().Replace("部", string.Empty));
                else if (CurrentDep == DayReportDep.DCC部 && hdnKeyReportType.Value == "dcczhhz")
                    spTitle.InnerText = string.Format("{0}年{1}月{2}综合汇总", day.Year, day.Month, CurrentDep.ToString().Replace("部", string.Empty));

                DataTable tbl = GetKeyReportMul(CurrentDep, day);
                tdData.InnerHtml = GetKeyReportStr(CurrentDep, tbl);
            }
        }

        private void LoadReportCountData()
        {
            tblView.Attributes.Remove("style");
            spTitle.Attributes["style"] = "font-size: 20px;";
            lblReportCount.Text = string.Format("应填报：{0}", DataConvert.SafeDate(txtDate2.Text).Subtract(DataConvert.SafeDate(txtDate.Text)).Days);
            FirstCellRowCount = 1;

            DateTime day = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                spTitle.InnerText = string.Format("{0}年{1}{2}日报填报统计", day.Year, string.Format("{0}至{1}", day.ToString("MM月dd日"), day2.ToString("MM月dd日")), CurrentDep.ToString().Replace("部", string.Empty));
                if (CurrentDep == DayReportDep.行政部)
                    spTitle.InnerText = string.Format("{0}年{1}月{2}日报填报统计", day.Year, string.Format("{0}至{1}", day.ToString("MM月dd日"), day2.ToString("MM月dd日")), "人事");

                DataTable tbl = GetReportCountMul(CurrentDep, day, day2);
                tdData.InnerHtml = GetReportCountStr(tbl);
            }
        }

        private DataTable GetReportMul(DayReportDep dep, DateTime day, DateTime day2)
        {
            DataTable tblresult = new DataTable();

            tblresult.Columns.Add("公司");
            tblresult.Columns.Add("录入");
            tblresult.Columns.Add("差值");
            tblresult.Columns.Add("差值合计");
            tblresult.Columns.Add("未及时填报");

            List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
            string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            corplist = corplist.FindAll(c => c.DailyreportShow == 1 && corppower.Contains(c.ID.ToString()));
            for (int i = 0; i < corplist.Count; i++)
            {
                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUniqueStart = day.ToString("yyyyMMdd"),
                    DayUniqueEnd = day2.ToString("yyyyMMdd"),
                    CorporationID = corplist[i].ID,
                    DayReportDep = CurrentDep
                };
                query.OrderBy = " [DayUnique] ASC";
                List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, CurrentDep, day, true);
                int days = 0;
                DataTable tbl = GetReport(CurrentDep, list, monthtarget, day, corplist[i].ID, ref days);
                if (i == 0)
                {
                    for (int j = 0; j < tbl.Rows.Count; j++)
                    {
                        string colname = tbl.Rows[j]["项目"].ToString();
                        int index = 1;
                        string colnamecache = colname;
                        while (tblresult.Columns.Contains(colnamecache))
                        {
                            colnamecache = colname + index.ToString();
                            index++;
                        }
                        tblresult.Columns.Add(colnamecache);
                    }
                }
                DataRow row = tblresult.NewRow();
                row["公司"] = corplist[i].Name;
                row["录入"] = list.Count;
                row["差值"] = list.Count - day2.Subtract(day).Days - 1; 
                int subcount = 0;
                int subcount1 = 0;
                for (int j = 0; j <= day2.Subtract(day).Days; j++)
                {
                    subcount += list.FindAll(l => l.CreateTime > day && l.CreateTime < day.AddDays(j + 1) && DataConvert.SafeInt(l.DayUnique) >= DataConvert.SafeInt(day.ToString("yyyyMMdd")) && DataConvert.SafeInt(l.DayUnique) <= DataConvert.SafeInt(day.AddDays(j + 1).ToString("yyyyMMdd"))).Count - j - 1;
                    subcount1 += list.FindAll(l => l.CreateTime > day.AddDays(j) && l.CreateTime < day.AddDays(j + 1) && DataConvert.SafeInt(l.DayUnique) >= DataConvert.SafeInt(day.AddDays(j).ToString("yyyyMMdd")) && DataConvert.SafeInt(l.DayUnique) <= DataConvert.SafeInt(day.AddDays(j + 1).ToString("yyyyMMdd"))).Count > 0 ? 0 : 1;
                }
                row["差值合计"] = subcount;
                row["未及时填报"] = subcount1;

                for (int j = 0; j < tbl.Rows.Count; j++)
                {
                    if (CurrentDep == DayReportDep.财务部)
                        row[j + 5] = tbl.Rows[j]["期初余额"].ToString() + "|" + tbl.Rows[j]["合计"].ToString();
                    else
                        row[j + 5] = tbl.Rows[j]["目标值"].ToString() + "|" + tbl.Rows[j]["合计"].ToString();
                }
                tblresult.Rows.Add(row);
            }

            return tblresult;
        }

        private string GetReportStr(DayReportDep dep, DataTable tbl)
        {
            StringBuilder strb = new StringBuilder();
            tblView.Width = (120 + 160 * tbl.Columns.Count) + "px";

            #region 页面输出
            Regex reg = new Regex(@"^[^\d]+\d+$");
            Regex regreplace = new Regex(@"\d+");

            strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
            strb.Append("<tr class=\"bold tc bggray\">");
            strb.Append("<td class=\"w120\" rowspan=\"2\">公司</td>");
            strb.Append("<td class=\"w80\" rowspan=\"2\">录入</td>");
            strb.Append("<td class=\"w80\" rowspan=\"2\">差值</td>");
            strb.Append("<td class=\"w80\" rowspan=\"2\">差值合计</td>");
            strb.Append("<td class=\"w100\" rowspan=\"2\">未及时填报</td>");
            for (int i = 5; i < tbl.Columns.Count; i++)
            {
                strb.AppendFormat("<td class=\"w160\" colspan=\"2\">{0}</td>", reg.IsMatch(tbl.Columns[i].ToString()) ? regreplace.Replace(tbl.Columns[i].ToString(), string.Empty) : tbl.Columns[i].ToString());
            }
            strb.Append("<td></td>");
            strb.Append("</tr>");
            strb.Append("<tr class=\"bold tc bggray\">");
            for (int i = 5; i < tbl.Columns.Count; i++)
            {
                if (CurrentDep == DayReportDep.财务部)
                    strb.Append("<td class=\"w80\">期初余额</td>");
                else
                    strb.Append("<td class=\"w80\">目标值</td>");
                strb.Append("<td class=\"w80\">合计</td>");
            }
            strb.Append("<td></td>");
            strb.Append("</tr>");

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                strb.Append("<tr class=\"tc\">");
                strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["公司"]);
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[i]["录入"]);
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[i]["差值"]);
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[i]["差值合计"]);
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[i]["未及时填报"]);
                for (int j = 5; j < tbl.Columns.Count; j++)
                {
                    string[] vals = tbl.Rows[i][tbl.Columns[j].ToString()].ToString().Split(new char[] { '|' }, StringSplitOptions.None);
                    strb.AppendFormat("<td class=\"w80\">{0}</td>", string.IsNullOrEmpty(vals[0]) ? "&nbsp;" : vals[0]);
                    strb.AppendFormat("<td class=\"w80\">{0}</td>", string.IsNullOrEmpty(vals[1]) ? "&nbsp;" : vals[1]);
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");
            }

            strb.AppendLine("</table>");

            #endregion

            return strb.ToString();
        }

        /// <summary>
        /// 基本项数据
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="list"></param>
        /// <param name="monthtarget"></param>
        /// <param name="day"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        private DataTable GetReport(DayReportDep dep, List<DailyReportInfo> list, MonthlyTargetInfo monthtarget, DateTime day, int corpid, ref int days)
        {
            DataTable tbl = new DataTable();

            List<DailyReportModuleInfo> rlist = DayReportModules.Instance.GetList(true);
            rlist = rlist.FindAll(l => l.Department == dep).OrderBy(l => l.Sort).ToList();
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            for (int i = 0; i < list.Count; i++)
            {
                if (!string.IsNullOrEmpty(list[i].SCReport))
                {
                    data.Add(json.Deserialize<Dictionary<string, string>>(list[i].SCReport));
                }
            }
            Dictionary<string, string> targetdata = new Dictionary<string, string>();
            if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.SCReport))
                targetdata = json.Deserialize<Dictionary<string, string>>(monthtarget.SCReport);

            #region 表结构

            tbl.Columns.Add("项目");
            days = 0;

            for (int i = 1; i <= 31; i++)
            {
                if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day))
                {
                    tbl.Columns.Add(i.ToString());
                    days++;
                }
            }
            tbl.Columns.Add("合计");
            tbl.Columns.Add("目标值");
            tbl.Columns.Add("完成率");

            #endregion

            if (dep == DayReportDep.销售部 || dep == DayReportDep.精品部 || dep == DayReportDep.金融部)
            {
                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count];

                #region 项目、合计、目标

                for (int i = 0; i < rlist.Count; i++)
                {
                    rows[i] = tbl.NewRow();
                    rows[i]["项目"] = rlist[i].Name;
                    rows[i]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }
            else if (dep == DayReportDep.售后部)
            {
                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count];

                #region 项目、合计、目标

                for (int i = 0; i < rlist.Count; i++)
                {
                    rows[i] = tbl.NewRow();
                    rows[i]["项目"] = rlist[i].Name;

                    if (rlist[i].Name == "事故在厂台次")
                    {
                        if (data.Exists(d => d.ContainsKey(rlist[i].ID.ToString())))
                        {
                            decimal lastvalue = 0;
                            for (int j = 0; j < data.Count; j++)
                            {
                                if (data[j].ContainsKey(rlist[i].ID.ToString()))
                                {
                                    lastvalue = DataConvert.SafeDecimal(data[j][rlist[i].ID.ToString()]);
                                }
                            }
                            rows[i]["合计"] = Math.Round(lastvalue, 0).ToString();
                        }
                        else
                            rows[i]["合计"] = string.Empty;
                    }
                    else
                        rows[i]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();

                    rows[i]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }
            else if (dep == DayReportDep.市场部)
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);
                List<DailyReportModuleInfo> rlist_xs = DayReportModules.Instance.GetList(true);
                rlist_xs = rlist_xs.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_xs = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_xs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_xs[i].SCReport))
                    {
                        data_xs.Add(json.Deserialize<Dictionary<string, string>>(list_xs[i].SCReport));
                    }
                } Dictionary<string, string> targetdata_xs = new Dictionary<string, string>();
                if (monthtarget_xs != null && !string.IsNullOrEmpty(monthtarget_xs.SCReport))
                    targetdata_xs = json.Deserialize<Dictionary<string, string>>(monthtarget_xs.SCReport);

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count + 3];

                #region 项目、合计、目标

                if (rlist.Exists(l => l.Name == "展厅本月留存客户数"))
                {
                    DailyReportModuleInfo m = rlist.Find(l => l.Name == "展厅本月留存客户数");
                    rows[0] = tbl.NewRow();
                    rows[0]["项目"] = m.Name;
                    if (data.Exists(d => d.ContainsKey(m.ID.ToString()) && !string.IsNullOrEmpty(d[m.ID.ToString()])))
                    {
                        rows[0]["合计"] = data.FindLast(d => d.ContainsKey(m.ID.ToString()) && !string.IsNullOrEmpty(d[m.ID.ToString()]))[m.ID.ToString()];
                    }
                    else
                        rows[0]["合计"] = string.Empty;
                }

                if (rlist_xs.Exists(l => l.Name == "展厅首次来客批次"))
                {
                    DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "展厅首次来客批次");
                    rows[1] = tbl.NewRow();
                    rows[1]["项目"] = "展厅首次到店记录数";
                    rows[1]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[1]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? targetdata_xs[m.ID.ToString()] : string.Empty;
                }

                if (rlist_xs.Exists(l => l.Name == "留档批次"))
                {
                    DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "留档批次");
                    rows[2] = tbl.NewRow();
                    rows[2]["项目"] = "展厅首次到店建档数";
                    rows[2]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[2]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? targetdata_xs[m.ID.ToString()] : string.Empty;
                }

                for (int i = 1; i < rlist.Count; i++)
                {
                    rows[i + 2] = tbl.NewRow();
                    rows[i + 2]["项目"] = rlist[i].Name;
                    rows[i + 2]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i + 2]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                }

                int idxzwxfszl = 0;
                if (rlist.Exists(r => r.Name == "新增微信粉丝总量" && r.Department == DayReportDep.市场部))
                    idxzwxfszl = rlist.Find(r => r.Name == "新增微信粉丝总量" && r.Department == DayReportDep.市场部).ID;
                rows[rlist.Count + 2] = tbl.NewRow();
                rows[rlist.Count + 2]["项目"] = "累计微信粉丝量";
                rows[rlist.Count + 2]["合计"] = (monthtarget == null ? 0 : DataConvert.SafeInt(monthtarget.SCsyfsl)) + DataConvert.SafeInt(Math.Round(data.Sum(d => d.ContainsKey(idxzwxfszl.ToString()) ? DataConvert.SafeDecimal(d[idxzwxfszl.ToString()]) : 0), 0).ToString());
                rows[rlist.Count + 2]["目标值"] = (monthtarget == null ? 0 : DataConvert.SafeInt(monthtarget.SCsyfsl)) + DataConvert.SafeInt(idxzwxfszl > 0 ? (targetdata.ContainsKey(idxzwxfszl.ToString()) ? targetdata[idxzwxfszl.ToString()] : string.Empty) : string.Empty);

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }
            else if (dep == DayReportDep.二手车部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);
                List<DailyReportModuleInfo> rlist_xs = DayReportModules.Instance.GetList(true);
                rlist_xs = rlist_xs.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_xs = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_xs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_xs[i].SCReport))
                    {
                        data_xs.Add(json.Deserialize<Dictionary<string, string>>(list_xs[i].SCReport));
                    }
                } Dictionary<string, string> targetdata_xs = new Dictionary<string, string>();
                if (monthtarget_xs != null && !string.IsNullOrEmpty(monthtarget_xs.SCReport))
                    targetdata_xs = json.Deserialize<Dictionary<string, string>>(monthtarget_xs.SCReport);

                #endregion

                #region 售后数据

                DailyReportQuery query_sh = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.售后部, day, true);
                List<DailyReportModuleInfo> rlist_sh = DayReportModules.Instance.GetList(true);
                rlist_sh = rlist_sh.FindAll(l => l.Department == DayReportDep.售后部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_sh = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_sh.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_sh[i].SCReport))
                    {
                        data_sh.Add(json.Deserialize<Dictionary<string, string>>(list_sh[i].SCReport));
                    }
                } Dictionary<string, string> targetdata_sh = new Dictionary<string, string>();
                if (monthtarget_sh != null && !string.IsNullOrEmpty(monthtarget_sh.SCReport))
                    targetdata_sh = json.Deserialize<Dictionary<string, string>>(monthtarget_sh.SCReport);

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count + 3];

                #region 项目、合计、目标

                if (rlist_xs.Exists(l => l.Name == "展厅交车台数"))
                {
                    DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "展厅交车台数");
                    rows[0] = tbl.NewRow();
                    rows[0]["项目"] = "新车展厅销售量";
                    rows[0]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[0]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? targetdata_xs[m.ID.ToString()] : string.Empty;
                }

                if (rlist_xs.Exists(l => l.Name == "展厅首次来客批次"))
                {
                    DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "展厅首次来客批次");
                    rows[1] = tbl.NewRow();
                    rows[1]["项目"] = "展厅首次来客批次";
                    rows[1]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[1]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? targetdata_xs[m.ID.ToString()] : string.Empty;
                }

                if (rlist.Exists(l => l.Name == "销售推荐评估台次"))
                {
                    DailyReportModuleInfo m = rlist.Find(l => l.Name == "销售推荐评估台次");
                    rows[2] = tbl.NewRow();
                    rows[2]["项目"] = m.Name;
                    rows[2]["合计"] = !m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[2]["目标值"] = targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty;
                }

                if (rlist.Exists(l => l.Name == "实际评估台次"))
                {
                    DailyReportModuleInfo m = rlist.Find(l => l.Name == "实际评估台次");
                    rows[3] = tbl.NewRow();
                    rows[3]["项目"] = m.Name;
                    rows[3]["合计"] = !m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[3]["目标值"] = targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty;
                }

                if (rlist_sh.Exists(l => l.Name == "来厂台次"))
                {
                    DailyReportModuleInfo m = rlist_sh.Find(l => l.Name == "来厂台次");
                    rows[4] = tbl.NewRow();
                    rows[4]["项目"] = "售后进厂台次";
                    rows[4]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_sh.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[4]["目标值"] = targetdata_sh.ContainsKey(m.ID.ToString()) ? targetdata_sh[m.ID.ToString()] : string.Empty;
                }

                for (int i = 2; i < rlist.Count; i++)
                {
                    rows[i + 3] = tbl.NewRow();
                    rows[i + 3]["项目"] = rlist[i].Name;
                    if (rlist[i].Name == "库存数" || rlist[i].Name == "在库超30天车辆")
                    {
                        if (data.Exists(d => d.ContainsKey(rlist[i].ID.ToString())))
                        {
                            decimal lastvalue = 0;
                            for (int j = 0; j < data.Count; j++)
                            {
                                if (data[j].ContainsKey(rlist[i].ID.ToString()))
                                {
                                    lastvalue = DataConvert.SafeDecimal(data[j][rlist[i].ID.ToString()]);
                                }
                            }
                            rows[i + 3]["合计"] = Math.Round(lastvalue, 0).ToString();
                        }
                        else
                            rows[i + 3]["合计"] = string.Empty;
                    }
                    else
                        rows[i + 3]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i + 3]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }
            else if (dep == DayReportDep.DCC部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> xslist = DailyReports.Instance.GetList(query, true);
                xslist = xslist.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo xsmonthtarget = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);
                List<DailyReportModuleInfo> xsrlist = DayReportModules.Instance.GetList(true);
                xsrlist = xsrlist.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> xsdata = new List<Dictionary<string, string>>();
                for (int i = 0; i < xslist.Count; i++)
                {
                    if (!string.IsNullOrEmpty(xslist[i].SCReport))
                    {
                        xsdata.Add(json.Deserialize<Dictionary<string, string>>(xslist[i].SCReport));
                    }
                } Dictionary<string, string> xstargetdata = new Dictionary<string, string>();
                if (xsmonthtarget != null && !string.IsNullOrEmpty(xsmonthtarget.SCReport))
                    xstargetdata = json.Deserialize<Dictionary<string, string>>(xsmonthtarget.SCReport);

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count + 4];
                int onum = 0;

                #region 项目、合计、目标

                for (int i = 0; i < rlist.Count; i++)
                {
                    rows[i + onum] = tbl.NewRow();
                    rows[i + onum]["项目"] = rlist[i].Name;
                    rows[i + onum]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i + onum]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                    if (rlist[i].Name == "CRM下达线索数")
                    {
                        int idztdrzr = rlist.Find(r => r.Name == "展厅当日转入").ID;
                        decimal hjztdrzr = Math.Round(data.Sum(d => d.ContainsKey(idztdrzr.ToString()) ? DataConvert.SafeDecimal(d[idztdrzr.ToString()]) : 0), 0);
                        decimal mbztdrzr = DataConvert.SafeDecimal(targetdata.ContainsKey(idztdrzr.ToString()) ? targetdata[idztdrzr.ToString()] : string.Empty);
                        int idztsydrzr = rlist.Find(r => r.Name == "展厅上月当日转入").ID;
                        decimal hjztsydrzr = Math.Round(data.Sum(d => d.ContainsKey(idztsydrzr.ToString()) ? DataConvert.SafeDecimal(d[idztsydrzr.ToString()]) : 0), 0);
                        decimal mbztsydrzr = DataConvert.SafeDecimal(targetdata.ContainsKey(idztsydrzr.ToString()) ? targetdata[idztsydrzr.ToString()] : string.Empty);
                        int idztshydrzr = rlist.Find(r => r.Name == "展厅双月当日转入").ID;
                        decimal hjztshydrzr = Math.Round(data.Sum(d => d.ContainsKey(idztshydrzr.ToString()) ? DataConvert.SafeDecimal(d[idztshydrzr.ToString()]) : 0), 0);
                        decimal mbztshydrzr = DataConvert.SafeDecimal(targetdata.ContainsKey(idztshydrzr.ToString()) ? targetdata[idztshydrzr.ToString()] : string.Empty);
                        int idztsyyqzzr = rlist.Find(r => r.Name == "展厅三月有强制转入").ID;
                        decimal hjztsyyqzzr = Math.Round(data.Sum(d => d.ContainsKey(idztsyyqzzr.ToString()) ? DataConvert.SafeDecimal(d[idztsyyqzzr.ToString()]) : 0), 0);
                        decimal mbztsyyqzzr = DataConvert.SafeDecimal(targetdata.ContainsKey(idztsyyqzzr.ToString()) ? targetdata[idztsyyqzzr.ToString()] : string.Empty);

                        onum++;
                        rows[i + onum] = tbl.NewRow();
                        rows[i + onum]["项目"] = "展厅转入总客源";
                        rows[i + onum]["合计"] = hjztdrzr + hjztsydrzr + hjztshydrzr + hjztsyyqzzr;
                        rows[i + onum]["目标值"] = mbztdrzr + mbztsydrzr + mbztshydrzr + mbztsyyqzzr;
                    }
                    if (rlist[i].Name == "首次邀约到店客户总数")
                    {
                        int idqczjyydd = rlist.Find(r => r.Name == "汽车之家邀约到店").ID;
                        decimal hjqczjyydd = Math.Round(data.Sum(d => d.ContainsKey(idqczjyydd.ToString()) ? DataConvert.SafeDecimal(d[idqczjyydd.ToString()]) : 0), 0);
                        decimal mbqczjyydd = DataConvert.SafeDecimal(targetdata.ContainsKey(idqczjyydd.ToString()) ? targetdata[idqczjyydd.ToString()] : string.Empty);
                        int idycwyydd = rlist.Find(r => r.Name == "易车网邀约到店").ID;
                        decimal hjycwyydd = Math.Round(data.Sum(d => d.ContainsKey(idycwyydd.ToString()) ? DataConvert.SafeDecimal(d[idycwyydd.ToString()]) : 0), 0);
                        decimal mbycwyydd = DataConvert.SafeDecimal(targetdata.ContainsKey(idycwyydd.ToString()) ? targetdata[idycwyydd.ToString()] : string.Empty);
                        int idtpyyydd = rlist.Find(r => r.Name == "太平洋汽车网邀约到店").ID;
                        decimal hjtpyyydd = Math.Round(data.Sum(d => d.ContainsKey(idtpyyydd.ToString()) ? DataConvert.SafeDecimal(d[idtpyyydd.ToString()]) : 0), 0);
                        decimal mbtpyyydd = DataConvert.SafeDecimal(targetdata.ContainsKey(idtpyyydd.ToString()) ? targetdata[idtpyyydd.ToString()] : string.Empty);
                        int idqtwlyydd = rlist.Find(r => r.Name == "其他网络邀约到店").ID;
                        decimal hjqtwlyydd = Math.Round(data.Sum(d => d.ContainsKey(idqtwlyydd.ToString()) ? DataConvert.SafeDecimal(d[idqtwlyydd.ToString()]) : 0), 0);
                        decimal mbqtwlyydd = DataConvert.SafeDecimal(targetdata.ContainsKey(idqtwlyydd.ToString()) ? targetdata[idqtwlyydd.ToString()] : string.Empty);

                        onum++;
                        rows[i + onum] = tbl.NewRow();
                        rows[i + onum]["项目"] = "首约到店网络客户数";
                        rows[i + onum]["合计"] = hjqczjyydd + hjycwyydd + hjtpyyydd + hjqtwlyydd;
                        rows[i + onum]["目标值"] = mbqczjyydd + mbycwyydd + mbtpyyydd + mbqtwlyydd;
                    }
                    if (rlist[i].Name == "DCC成交总台数")
                    {
                        int idqczjcjs = rlist.Find(r => r.Name == "汽车之家成交数").ID;
                        decimal hjqczjcjs = Math.Round(data.Sum(d => d.ContainsKey(idqczjcjs.ToString()) ? DataConvert.SafeDecimal(d[idqczjcjs.ToString()]) : 0), 0);
                        decimal mbqczjcjs = DataConvert.SafeDecimal(targetdata.ContainsKey(idqczjcjs.ToString()) ? targetdata[idqczjcjs.ToString()] : string.Empty);
                        int idycwcjs = rlist.Find(r => r.Name == "易车网成交数").ID;
                        decimal hjycwcjs = Math.Round(data.Sum(d => d.ContainsKey(idycwcjs.ToString()) ? DataConvert.SafeDecimal(d[idycwcjs.ToString()]) : 0), 0);
                        decimal mbycwcjs = DataConvert.SafeDecimal(targetdata.ContainsKey(idycwcjs.ToString()) ? targetdata[idycwcjs.ToString()] : string.Empty);
                        int idtpycjs = rlist.Find(r => r.Name == "太平洋成交数").ID;
                        decimal hjtpycjs = Math.Round(data.Sum(d => d.ContainsKey(idtpycjs.ToString()) ? DataConvert.SafeDecimal(d[idtpycjs.ToString()]) : 0), 0);
                        decimal mbtpycjs = DataConvert.SafeDecimal(targetdata.ContainsKey(idtpycjs.ToString()) ? targetdata[idtpycjs.ToString()] : string.Empty);
                        int idqtwlcjs = rlist.Find(r => r.Name == "其他网络成交数").ID;
                        decimal hjqtwlcjs = Math.Round(data.Sum(d => d.ContainsKey(idqtwlcjs.ToString()) ? DataConvert.SafeDecimal(d[idqtwlcjs.ToString()]) : 0), 0);
                        decimal mbqtwlcjs = DataConvert.SafeDecimal(targetdata.ContainsKey(idqtwlcjs.ToString()) ? targetdata[idqtwlcjs.ToString()] : string.Empty);

                        onum++;
                        rows[i + onum] = tbl.NewRow();
                        rows[i + onum]["项目"] = "网络成交数";
                        rows[i + onum]["合计"] = hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs;
                        rows[i + onum]["目标值"] = mbqczjcjs + mbycwcjs + mbtpycjs + mbqtwlcjs;
                    }
                }

                if (xsrlist.Exists(l => l.Name == "展厅交车台数"))
                {
                    DailyReportModuleInfo m = xsrlist.Find(l => l.Name == "展厅交车台数");
                    rows[rlist.Count + 3] = tbl.NewRow();
                    rows[rlist.Count + 3]["项目"] = "展厅销量";
                    rows[rlist.Count + 3]["合计"] = !m.Iscount ? string.Empty : Math.Round(xsdata.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[rlist.Count + 3]["目标值"] = xstargetdata.ContainsKey(m.ID.ToString()) ? xstargetdata[m.ID.ToString()] : string.Empty;
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }
            else if (dep == DayReportDep.财务部)
            {
                tbl.Columns["目标值"].ColumnName = "期初余额";
                DataRow[] rows = new DataRow[rlist.Count + 5];
                string[] listignore = { "集团内部资金借出", "其他支出", "POS未到帐", "银行帐户余额", "其中农行", "中行", "工行", "建行", "交行", "民生", "平安", "中信", "华夏", "浙商", "泰隆", "其他银行", "现金合计", "留存现金", "资金余额与现金校对数", "本日预计支付合计数", "其中：购车（配件）款", "工资", "税款", "其他大额款项" };

                int midxssr = rlist.Find(l => l.Department == CurrentDep && l.Name == "销售收入").ID;
                int midajfkyj = rlist.Find(l => l.Department == CurrentDep && l.Name == "按揭放款、押金").ID;
                int midwxsr = rlist.Find(l => l.Department == CurrentDep && l.Name == "维修收入").ID;
                int midyhfd = rlist.Find(l => l.Department == CurrentDep && l.Name == "银行放贷").ID;
                int midjtnbzjjr = rlist.Find(l => l.Department == CurrentDep && l.Name == "集团内部资金借入").ID;
                int midqtsr = rlist.Find(l => l.Department == CurrentDep && l.Name == "其他收入").ID;

                int midzczc = rlist.Find(l => l.Department == CurrentDep && l.Name == "整车支付").ID;
                int midbjzc = rlist.Find(l => l.Department == CurrentDep && l.Name == "配件支付").ID;
                //int midyhdkdq = rlist.Find(l => l.Department == CurrentDep && l.Name == "银承、贷款到期").ID;
                int midjtnbzjjc = rlist.Find(l => l.Department == CurrentDep && l.Name == "集团内部资金借出").ID;
                int midqtzc = rlist.Find(l => l.Department == CurrentDep && l.Name == "其他支出").ID;

                #region 表数据

                #region 项目、合计、期初余额

                int index = 0;
                for (int i = 0; i < rlist.Count; i++)
                {
                    if (!listignore.Contains(rlist[i].Name))
                    {
                        rows[index] = tbl.NewRow();
                        rows[index]["项目"] = rlist[i].Name;
                        rows[index]["合计"] = Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 2);
                        rows[index]["期初余额"] = string.Empty;
                        index++;
                    }
                }

                decimal hjdxssr = data.Sum(d => d.ContainsKey(midxssr.ToString()) ? DataConvert.SafeDecimal(d[midxssr.ToString()]) : 0);
                decimal hjajfkyj = data.Sum(d => d.ContainsKey(midajfkyj.ToString()) ? DataConvert.SafeDecimal(d[midajfkyj.ToString()]) : 0);
                decimal hjwxsr = data.Sum(d => d.ContainsKey(midwxsr.ToString()) ? DataConvert.SafeDecimal(d[midwxsr.ToString()]) : 0);
                decimal hjyhfd = data.Sum(d => d.ContainsKey(midyhfd.ToString()) ? DataConvert.SafeDecimal(d[midyhfd.ToString()]) : 0);
                decimal hjjtnbzjjr = data.Sum(d => d.ContainsKey(midjtnbzjjr.ToString()) ? DataConvert.SafeDecimal(d[midjtnbzjjr.ToString()]) : 0);
                decimal hjqtsr = data.Sum(d => d.ContainsKey(midqtsr.ToString()) ? DataConvert.SafeDecimal(d[midqtsr.ToString()]) : 0);

                decimal hjzczc = data.Sum(d => d.ContainsKey(midzczc.ToString()) ? DataConvert.SafeDecimal(d[midzczc.ToString()]) : 0);
                decimal hjbjzc = data.Sum(d => d.ContainsKey(midbjzc.ToString()) ? DataConvert.SafeDecimal(d[midbjzc.ToString()]) : 0);
                decimal hjyhdkdq = 0;
                if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.CWycdkdq))
                {
                    string[] ycdkdq = monthtarget.CWycdkdq.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string strnumber in ycdkdq)
                    {
                        hjyhdkdq += DataConvert.SafeDecimal(strnumber, 0);
                    }
                }
                decimal hjjtnbzjjc = data.Sum(d => d.ContainsKey(midjtnbzjjc.ToString()) ? DataConvert.SafeDecimal(d[midjtnbzjjc.ToString()]) : 0);
                decimal hjqtzc = data.Sum(d => d.ContainsKey(midqtzc.ToString()) ? DataConvert.SafeDecimal(d[midqtzc.ToString()]) : 0);

                decimal hjsr = Math.Round(hjdxssr + hjajfkyj + hjwxsr + hjyhfd + hjjtnbzjjr + hjqtsr, 2);
                decimal hjzc = Math.Round(hjzczc + hjbjzc + hjyhdkdq + hjjtnbzjjc + hjqtzc, 2);

                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "银承、贷款到期";
                rows[index]["合计"] = hjyhdkdq;
                rows[index]["期初余额"] = string.Empty;
                index++;
                int jtnbzjjcid = rlist.Find(l => l.Department == CurrentDep && l.Name == "集团内部资金借出").ID;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "集团内部资金借出";
                rows[index]["合计"] = Math.Round(data.Sum(d => d.ContainsKey(jtnbzjjcid.ToString()) ? DataConvert.SafeDecimal(d[jtnbzjjcid.ToString()]) : 0), 2);
                rows[index]["期初余额"] = string.Empty;
                index++;
                int qtzcid = rlist.Find(l => l.Department == CurrentDep && l.Name == "其他支出").ID;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "其他支出";
                rows[index]["合计"] = Math.Round(data.Sum(d => d.ContainsKey(qtzcid.ToString()) ? DataConvert.SafeDecimal(d[qtzcid.ToString()]) : 0), 2);
                rows[index]["期初余额"] = string.Empty;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "当日资金总收入";
                rows[index]["合计"] = hjsr;
                rows[index]["期初余额"] = string.Empty;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "当日资金总支出";
                rows[index]["合计"] = hjzc;
                rows[index]["期初余额"] = string.Empty;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "资金余额";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWzjye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "POS未到帐";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWposwdz;
                index++;
                double ycyhzhye = 0;
                if (monthtarget != null)
                {
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWnhzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWzhzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWghzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWjianhzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWjhzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWmszhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWpazhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWzxzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWhxzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWzszhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWtlzhye);
                    ycyhzhye += DataConvert.SafeDouble(monthtarget.CWqtyhzhye);
                }
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "银行帐户余额";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = Math.Round(ycyhzhye, 2);
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "其中农行";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWnhzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "中行";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWzhzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "工行";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWghzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "建行";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWjianhzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "交行";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWjhzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "民生";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWmszhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "平安";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWpazhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "中信";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWzxzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "华夏";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWhxzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "浙商";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWzszhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "泰隆";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWtlzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "其他银行";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWqtyhzhye;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "现金合计";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWxjczhj;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "留存现金";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = monthtarget == null ? string.Empty : monthtarget.CWlcxj;
                index++;
                decimal qczjye = DataConvert.SafeDecimal(monthtarget == null ? string.Empty : monthtarget.CWzjye);
                decimal qcpos = DataConvert.SafeDecimal(monthtarget == null ? string.Empty : monthtarget.CWposwdz);
                decimal qcyhzhye = DataConvert.SafeDecimal(ycyhzhye);
                decimal qcxjhj = DataConvert.SafeDecimal(monthtarget == null ? string.Empty : monthtarget.CWxjczhj);
                decimal qclcxj = DataConvert.SafeDecimal(monthtarget == null ? string.Empty : monthtarget.CWlcxj);
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "资金余额与现金校对数";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = qczjye - qcpos - qcyhzhye - qcxjhj - qclcxj;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "本日预计支付合计数";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = string.Empty;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "其中：购车（配件）款";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = string.Empty;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "工资";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = string.Empty;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "税款";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = string.Empty;
                index++;
                rows[index] = tbl.NewRow();
                rows[index]["项目"] = "其他大额款项";
                rows[index]["合计"] = string.Empty;
                rows[index]["期初余额"] = string.Empty;

                #endregion

                #region 每日数据

                string[] mrycdkdq = null;
                if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.CWycdkdq))
                {
                    mrycdkdq = monthtarget.CWycdkdq.Split(new char[] { ',' }, StringSplitOptions.None);
                }

                //银承、贷款到期
                for (int i = 1; i <= days; i++)
                {
                    index = 9;
                    rows[index][i] = mrycdkdq == null ? string.Empty : mrycdkdq[i - 1].ToString();
                }

                for (int i = 1; i <= days; i++)
                {
                    index = 0;
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            //其他项每日数据
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            for (int j = 0; j < rlist.Count; j++)
                            {
                                if (!listignore.Contains(rlist[j].Name))
                                {
                                    rows[index][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                                    index++;
                                }
                            }

                            //银承、贷款到期
                            index++;

                            //集团内部资金借出
                            rows[index][i] = reportdate.ContainsKey(jtnbzjjcid.ToString()) ? reportdate[jtnbzjjcid.ToString()] : string.Empty;
                            index++;

                            //其他支出
                            rows[index][i] = reportdate.ContainsKey(qtzcid.ToString()) ? reportdate[qtzcid.ToString()] : string.Empty;
                            index++;

                            //当日资金总收入
                            decimal drxssr = reportdate.ContainsKey(midxssr.ToString()) ? DataConvert.SafeDecimal(reportdate[midxssr.ToString()]) : 0;
                            decimal drajfkyj = reportdate.ContainsKey(midajfkyj.ToString()) ? DataConvert.SafeDecimal(reportdate[midajfkyj.ToString()]) : 0;
                            decimal drwxsr = reportdate.ContainsKey(midwxsr.ToString()) ? DataConvert.SafeDecimal(reportdate[midwxsr.ToString()]) : 0;
                            decimal dryhfd = reportdate.ContainsKey(midyhfd.ToString()) ? DataConvert.SafeDecimal(reportdate[midyhfd.ToString()]) : 0;
                            decimal drjtnbzjjr = reportdate.ContainsKey(midjtnbzjjr.ToString()) ? DataConvert.SafeDecimal(reportdate[midjtnbzjjr.ToString()]) : 0;
                            decimal drqtsr = reportdate.ContainsKey(midqtsr.ToString()) ? DataConvert.SafeDecimal(reportdate[midqtsr.ToString()]) : 0;

                            rows[index][i] = drxssr + drajfkyj + drwxsr + dryhfd + drjtnbzjjr + drqtsr;
                            index++;

                            //当日资金总支出
                            decimal drzczc = reportdate.ContainsKey(midzczc.ToString()) ? DataConvert.SafeDecimal(reportdate[midzczc.ToString()]) : 0;
                            decimal drbjzc = reportdate.ContainsKey(midbjzc.ToString()) ? DataConvert.SafeDecimal(reportdate[midbjzc.ToString()]) : 0;
                            decimal dryhdkdq = mrycdkdq == null ? 0 : DataConvert.SafeDecimal(mrycdkdq[i - 1]);
                            decimal drjtnbzjjc = reportdate.ContainsKey(midjtnbzjjc.ToString()) ? DataConvert.SafeDecimal(reportdate[midjtnbzjjc.ToString()]) : 0;
                            decimal drqtzc = reportdate.ContainsKey(midqtzc.ToString()) ? DataConvert.SafeDecimal(reportdate[midqtzc.ToString()]) : 0;

                            rows[index][i] = drzczc + drbjzc + dryhdkdq + drjtnbzjjc + drqtzc;
                            index++;

                            //资金余额每日数据
                            List<Dictionary<string, string>> dataday = new List<Dictionary<string, string>>();
                            foreach (DailyReportInfo dr in list.FindAll(l => DataConvert.SafeInt(l.DayUnique) <= DataConvert.SafeInt(day.ToString("yyyyMMdd"))))
                            {
                                if (!string.IsNullOrEmpty(dr.SCReport))
                                {
                                    dataday.Add(json.Deserialize<Dictionary<string, string>>(dr.SCReport));
                                }
                            }
                            decimal hjdrxssr = dataday.Sum(d => d.ContainsKey(midxssr.ToString()) ? DataConvert.SafeDecimal(d[midxssr.ToString()]) : 0);
                            decimal hjdrajfkyj = dataday.Sum(d => d.ContainsKey(midajfkyj.ToString()) ? DataConvert.SafeDecimal(d[midajfkyj.ToString()]) : 0);
                            decimal hjdrwxsr = dataday.Sum(d => d.ContainsKey(midwxsr.ToString()) ? DataConvert.SafeDecimal(d[midwxsr.ToString()]) : 0);
                            decimal hjdryhfd = dataday.Sum(d => d.ContainsKey(midyhfd.ToString()) ? DataConvert.SafeDecimal(d[midyhfd.ToString()]) : 0);
                            decimal hjdrjtnbzjjr = dataday.Sum(d => d.ContainsKey(midjtnbzjjr.ToString()) ? DataConvert.SafeDecimal(d[midjtnbzjjr.ToString()]) : 0);
                            decimal hjdrqtsr = dataday.Sum(d => d.ContainsKey(midqtsr.ToString()) ? DataConvert.SafeDecimal(d[midqtsr.ToString()]) : 0);

                            decimal hjdrzczc = dataday.Sum(d => d.ContainsKey(midzczc.ToString()) ? DataConvert.SafeDecimal(d[midzczc.ToString()]) : 0);
                            decimal hjdrbjzc = dataday.Sum(d => d.ContainsKey(midbjzc.ToString()) ? DataConvert.SafeDecimal(d[midbjzc.ToString()]) : 0);
                            decimal hjdryhdkdq = 0;
                            if (mrycdkdq != null)
                            {
                                for (int j = 0; j < i; j++)
                                {
                                    hjdryhdkdq += DataConvert.SafeDecimal(mrycdkdq[j]);
                                }
                            }
                            decimal hjdrjtnbzjjc = dataday.Sum(d => d.ContainsKey(midjtnbzjjc.ToString()) ? DataConvert.SafeDecimal(d[midjtnbzjjc.ToString()]) : 0);
                            decimal hjdrqtzc = dataday.Sum(d => d.ContainsKey(midqtzc.ToString()) ? DataConvert.SafeDecimal(d[midqtzc.ToString()]) : 0);

                            decimal hjdrzjzsr = hjdrxssr + hjdrajfkyj + hjdrwxsr + hjdryhfd + hjdrjtnbzjjr + hjdrqtsr;
                            decimal hjdrzjzzc = hjdrzczc + hjdrbjzc + hjdryhdkdq + hjdrjtnbzjjc + hjdrqtzc;
                            decimal drzjye = Math.Round(DataConvert.SafeDecimal(monthtarget == null ? string.Empty : monthtarget.CWzjye) + hjdrzjzsr - hjdrzjzzc, 2);
                            rows[index][i] = drzjye;
                            index++;

                            //POS未到帐、银行帐户余额、农行、中行、民生、交行、其他银行、现金合计、留存现金 每日数据
                            for (int k = 0; k < listignore.Length; k++)
                            {
                                if (listignore[k] != "集团内部资金借出" && listignore[k] != "其他支出" && listignore[k] != "资金余额与现金校对数")
                                {
                                    rows[index][i] = reportdate.ContainsKey(rlist.Find(l => l.Name == listignore[k]).ID.ToString()) ? reportdate[rlist.Find(l => l.Name == listignore[k]).ID.ToString()] : string.Empty;
                                    index++;
                                }
                                else if (listignore[k] == "资金余额与现金校对数")
                                {
                                    decimal drpos = DataConvert.SafeDecimal(reportdate.ContainsKey(rlist.Find(l => l.Name == "POS未到帐").ID.ToString()) ? reportdate[rlist.Find(l => l.Name == "POS未到帐").ID.ToString()] : string.Empty);
                                    decimal dryhzhye = DataConvert.SafeDecimal(reportdate.ContainsKey(rlist.Find(l => l.Name == "银行帐户余额").ID.ToString()) ? reportdate[rlist.Find(l => l.Name == "银行帐户余额").ID.ToString()] : string.Empty);
                                    decimal drxjhj = DataConvert.SafeDecimal(reportdate.ContainsKey(rlist.Find(l => l.Name == "现金合计").ID.ToString()) ? reportdate[rlist.Find(l => l.Name == "现金合计").ID.ToString()] : string.Empty);
                                    decimal drlcxj = DataConvert.SafeDecimal(reportdate.ContainsKey(rlist.Find(l => l.Name == "留存现金").ID.ToString()) ? reportdate[rlist.Find(l => l.Name == "留存现金").ID.ToString()] : string.Empty);
                                    rows[index][i] = drzjye - drpos - dryhzhye - drxjhj - drlcxj;
                                    index++;
                                }
                            }
                        }
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }

                #endregion
            }
            else if (dep == DayReportDep.行政部)
            {
                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count];

                #region 项目、合计、目标

                for (int i = 0; i < rlist.Count; i++)
                {
                    rows[i] = tbl.NewRow();
                    rows[i]["项目"] = rlist[i].Name;
                    if (rlist[i].Name == "总人数")
                    {
                        if (data.Exists(d => d.ContainsKey(rlist[i].ID.ToString())))
                        {
                            decimal lastvalue = 0;
                            for (int j = 0; j < data.Count; j++)
                            {
                                if (data[j].ContainsKey(rlist[i].ID.ToString()))
                                {
                                    lastvalue = DataConvert.SafeDecimal(data[j][rlist[i].ID.ToString()]);
                                }
                            }
                            rows[i]["合计"] = Math.Round(lastvalue, 0).ToString();
                        }
                        else
                            rows[i]["合计"] = string.Empty;
                    }
                    else
                        rows[i]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }
            else if (dep == DayReportDep.无忧产品 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);
                List<DailyReportModuleInfo> rlist_xs = DayReportModules.Instance.GetList(true);
                rlist_xs = rlist_xs.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_xs = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_xs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_xs[i].SCReport))
                    {
                        data_xs.Add(json.Deserialize<Dictionary<string, string>>(list_xs[i].SCReport));
                    }
                } Dictionary<string, string> targetdata_xs = new Dictionary<string, string>();
                if (monthtarget_xs != null && !string.IsNullOrEmpty(monthtarget_xs.SCReport))
                    targetdata_xs = json.Deserialize<Dictionary<string, string>>(monthtarget_xs.SCReport);

                #endregion

                #region 售后数据

                DailyReportQuery query_sh = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.售后部, day, true);
                List<DailyReportModuleInfo> rlist_sh = DayReportModules.Instance.GetList(true);
                rlist_sh = rlist_sh.FindAll(l => l.Department == DayReportDep.售后部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_sh = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_sh.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_sh[i].SCReport))
                    {
                        data_sh.Add(json.Deserialize<Dictionary<string, string>>(list_sh[i].SCReport));
                    }
                } Dictionary<string, string> targetdata_sh = new Dictionary<string, string>();
                if (monthtarget_sh != null && !string.IsNullOrEmpty(monthtarget_sh.SCReport))
                    targetdata_sh = json.Deserialize<Dictionary<string, string>>(monthtarget_sh.SCReport);

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count + 14];

                #region 项目、合计、目标

                List<KeyValuePair<string, string>> nlist_xs = new List<KeyValuePair<string, string>>() 
                {
                    new KeyValuePair<string, string>("机油套餐购买个数","新车机油套餐购买个数"),
                    new KeyValuePair<string, string>("机油套餐购买金额","新车机油套餐购买金额"),
                    new KeyValuePair<string, string>("划痕无忧服务购买个数","新车划痕无忧服务购买个数"),
                    new KeyValuePair<string, string>("划痕无忧服务购买金额","新车划痕无忧服务购买金额"),
                    new KeyValuePair<string, string>("玻璃无忧服务购买个数","新车玻璃无忧服务购买个数"),
                    new KeyValuePair<string, string>("玻璃无忧服务购买金额","新车玻璃无忧服务购买金额"),
                    new KeyValuePair<string, string>("延保无忧车服务购买个数","新车延保服务购买个数"),
                    new KeyValuePair<string, string>("延保无忧车服务购买金额","新车延保服务购买金额"),
                    new KeyValuePair<string, string>("自主延保无忧车服务购买个数","新车自主延保服务购买个数"),
                    new KeyValuePair<string, string>("自主延保无忧车服务购买金额","新车自主延保服务购买金额"),
                    new KeyValuePair<string, string>("厂家延保无忧车服务购买个数","新车厂家延保服务购买个数"),
                    new KeyValuePair<string, string>("厂家延保无忧车服务购买金额","新车厂家延保服务购买金额"),
                    new KeyValuePair<string, string>("展厅含DCC保险台次","新保出单量"),
                };

                for (int i = 0; i < nlist_xs.Count; i++)
                {
                    if (rlist_xs.Exists(l => l.Name == nlist_xs[i].Key))
                    {
                        DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == nlist_xs[i].Key);
                        rows[i] = tbl.NewRow();
                        rows[i]["项目"] = nlist_xs[i].Value;
                        if (nlist_xs[i].Value.IndexOf("金额") > 0)
                        {
                            rows[i]["合计"] = !m.Iscount ? string.Empty : (Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0) / 10000).ToString();
                            rows[i]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? (DataConvert.SafeDecimal(targetdata_xs[m.ID.ToString()]) / 10000).ToString() : string.Empty;
                        }
                        else
                        {
                            rows[i]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                            rows[i]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? targetdata_xs[m.ID.ToString()] : string.Empty;
                        }
                    }
                }

                for (int i = 0; i < rlist.Count; i++)
                {
                    rows[i + 13] = tbl.NewRow();
                    rows[i + 13]["项目"] = rlist[i].Name;
                    if (rlist[i].Name.IndexOf("金额") > 0)
                    {
                        rows[i + 13]["合计"] = !rlist[i].Iscount ? string.Empty : (Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0) / 10000).ToString();
                        rows[i + 13]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? (DataConvert.SafeDecimal(targetdata[rlist[i].ID.ToString()]) / 10000).ToString() : string.Empty;
                    }
                    else
                    {
                        rows[i + 13]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                        rows[i + 13]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                    }
                }

                if (rlist_sh.Exists(l => l.Name == "续保数"))
                {
                    DailyReportModuleInfo m = rlist_sh.Find(l => l.Name == "续保数");
                    rows[rlist.Count + 13] = tbl.NewRow();
                    rows[rlist.Count + 13]["项目"] = "续保出单量";
                    rows[rlist.Count + 13]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_sh.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                    rows[rlist.Count + 13]["目标值"] = targetdata_sh.ContainsKey(m.ID.ToString()) ? targetdata_sh[m.ID.ToString()] : string.Empty;
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }
            else if (dep == DayReportDep.客服部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);
                List<DailyReportModuleInfo> rlist_xs = DayReportModules.Instance.GetList(true);
                rlist_xs = rlist_xs.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_xs = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_xs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_xs[i].SCReport))
                    {
                        data_xs.Add(json.Deserialize<Dictionary<string, string>>(list_xs[i].SCReport));
                    }
                } Dictionary<string, string> targetdata_xs = new Dictionary<string, string>();
                if (monthtarget_xs != null && !string.IsNullOrEmpty(monthtarget_xs.SCReport))
                    targetdata_xs = json.Deserialize<Dictionary<string, string>>(monthtarget_xs.SCReport);

                #endregion

                #region 售后数据

                DailyReportQuery query_sh = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.售后部, day, true);
                List<DailyReportModuleInfo> rlist_sh = DayReportModules.Instance.GetList(true);
                rlist_sh = rlist_sh.FindAll(l => l.Department == DayReportDep.售后部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_sh = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_sh.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_sh[i].SCReport))
                    {
                        data_sh.Add(json.Deserialize<Dictionary<string, string>>(list_sh[i].SCReport));
                    }
                } Dictionary<string, string> targetdata_sh = new Dictionary<string, string>();
                if (monthtarget_sh != null && !string.IsNullOrEmpty(monthtarget_sh.SCReport))
                    targetdata_sh = json.Deserialize<Dictionary<string, string>>(monthtarget_sh.SCReport);

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count + 5];

                #region 项目、合计、目标

                int otherrowcount = 0;

                for (int i = 0; i < rlist.Count; i++)
                {
                    if (rlist[i].Name == "销售回访成功数")
                    {
                        if (rlist_xs.Exists(l => l.Name == "展厅交车台数"))
                        {
                            DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "展厅交车台数");
                            rows[i + otherrowcount] = tbl.NewRow();
                            rows[i + otherrowcount]["项目"] = "展厅销量";
                            rows[i + otherrowcount]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                            rows[i + otherrowcount]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? targetdata_xs[m.ID.ToString()] : string.Empty;
                        }
                        otherrowcount++;
                    }
                    if (rlist[i].Name == "潜客回访数")
                    {
                        if (rlist_xs.Exists(l => l.Name == "留档批次"))
                        {
                            DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "留档批次");
                            rows[i + otherrowcount] = tbl.NewRow();
                            rows[i + otherrowcount]["项目"] = "潜客数";
                            rows[i + otherrowcount]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                            rows[i + otherrowcount]["目标值"] = targetdata_xs.ContainsKey(m.ID.ToString()) ? targetdata_xs[m.ID.ToString()] : string.Empty;
                        }
                        otherrowcount++;
                    }
                    if (rlist[i].Name == "首保到期提醒数")
                    {
                        if (rlist_sh.Exists(l => l.Name == "来厂台次"))
                        {
                            DailyReportModuleInfo m = rlist_sh.Find(l => l.Name == "来厂台次");
                            rows[i + otherrowcount] = tbl.NewRow();
                            rows[i + otherrowcount]["项目"] = "售后进厂台次";
                            rows[i + otherrowcount]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_sh.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                            rows[i + otherrowcount]["目标值"] = targetdata_sh.ContainsKey(m.ID.ToString()) ? targetdata_sh[m.ID.ToString()] : string.Empty;
                        }
                        otherrowcount++;
                    }
                    if (rlist[i].Name == "事故信息回访量")
                    {
                        rows[i + otherrowcount] = tbl.NewRow();
                        rows[i + otherrowcount]["项目"] = "电话呼出总量";
                        DailyReportModuleInfo m = rlist.Find(l => l.Name == "销售回访成功数");
                        decimal hjxshf = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mbxshf = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        m = rlist.Find(l => l.Name == "潜客回访数");
                        decimal hjqkhf = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mbqkhf = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        m = rlist.FindAll(l => l.Name == "呼出电话量")[0];
                        decimal hjsbtx = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mbsbtx = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        m = rlist.FindAll(l => l.Name == "呼出电话量")[1];
                        decimal hjdbyy = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mbdbyy = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        m = rlist.Find(l => l.Name == "活动招揽电话量");
                        decimal hjhdzl = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mbhdzl = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        m = rlist.Find(l => l.Name == "流失客户招揽电话量");
                        decimal hjlszl = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mblszl = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        m = rlist.Find(l => l.Name == "事故信息回访量");
                        decimal hjsghf = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mbsghf = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        m = rlist.Find(l => l.Name == "抢修回访数");
                        decimal hjqxhf = DataConvert.SafeDecimal(!m.Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString());
                        decimal mbqxhf = DataConvert.SafeDecimal(targetdata.ContainsKey(m.ID.ToString()) ? targetdata[m.ID.ToString()] : string.Empty);
                        rows[i + otherrowcount]["合计"] = hjxshf + hjqkhf + hjsbtx + hjdbyy + hjhdzl + hjlszl + hjsghf + hjqxhf;
                        rows[i + otherrowcount]["目标值"] = mbxshf + mbqkhf + mbsbtx + mbdbyy + mbhdzl + mblszl + mbsghf + mbqxhf;

                        otherrowcount++;

                        if (rlist_sh.Exists(l => l.Name == "收到信息数"))
                        {
                            m = rlist_sh.Find(l => l.Name == "收到信息数");
                            rows[i + otherrowcount] = tbl.NewRow();
                            rows[i + otherrowcount]["项目"] = "事故信息数";
                            rows[i + otherrowcount]["合计"] = !m.Iscount ? string.Empty : Math.Round(data_sh.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0).ToString();
                            rows[i + otherrowcount]["目标值"] = targetdata_sh.ContainsKey(m.ID.ToString()) ? targetdata_sh[m.ID.ToString()] : string.Empty;
                        }
                        otherrowcount++;
                    }

                    rows[i + otherrowcount] = tbl.NewRow();
                    rows[i + otherrowcount]["项目"] = rlist[i].Name;
                    rows[i + otherrowcount]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i + otherrowcount]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                #endregion
            }

            return tbl;
        }

        private DataTable GetKeyReportMul(DayReportDep dep, DateTime day)
        {
            DataTable tblresult = new DataTable();

            tblresult.Columns.Add("公司");

            List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
            string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            corplist = corplist.FindAll(c => c.DailyreportShow == 1 && corppower.Contains(c.ID.ToString()));
            for (int i = 0; i < corplist.Count; i++)
            {
                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corplist[i].ID,
                    DayReportDep = CurrentDep
                };
                query.OrderBy = " [DayUnique] ASC";
                List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, CurrentDep, day, true);
                int days = 0;
                DataTable tbl = GetReport(CurrentDep, list, monthtarget, day, corplist[i].ID, ref days);
                tbl = GetKeyReport(CurrentDep, list, monthtarget, tbl, corplist[i].ID);

                if (i == 0)
                {
                    for (int j = 0; j < tbl.Rows.Count; j++)
                    {
                        string colname = tbl.Rows[j]["关键指标"].ToString();
                        int index = 1;
                        string colnamecache = colname;
                        while (tblresult.Columns.Contains(colnamecache))
                        {
                            colnamecache = colname + index.ToString();
                            index++;
                        }
                        tblresult.Columns.Add(colnamecache);
                    }
                }
                DataRow row = tblresult.NewRow();
                row[0] = corplist[i].Name;
                for (int j = 0; j < tbl.Rows.Count; j++)
                {
                    string fh = tblresult.Columns[j + 1].ColumnName.EndsWith("率")
                        || tblresult.Columns[j + 1].ColumnName.EndsWith("比")
                    || tblresult.Columns[j + 1].ColumnName.EndsWith("比例") ? "%" : string.Empty;
                    row[j + 1] = tbl.Rows[j]["目标"].ToString() + (string.IsNullOrEmpty(tbl.Rows[j]["目标"].ToString()) ? string.Empty : fh) + "|" + tbl.Rows[j]["实际"].ToString() + (string.IsNullOrEmpty(tbl.Rows[j]["实际"].ToString()) ? string.Empty : fh) + "|" + tbl.Rows[j]["详细"].ToString() + "|" + tbl.Rows[j]["完成率"].ToString() + (string.IsNullOrEmpty(tbl.Rows[j]["完成率"].ToString()) ? string.Empty : "%");
                }
                tblresult.Rows.Add(row);
            }

            return tblresult;
        }

        private string GetKeyReportStr(DayReportDep dep, DataTable tbl)
        {
            StringBuilder strb = new StringBuilder();
            tblView.Width = (120 + 180 * tbl.Columns.Count) + "px";

            #region 页面输出

            if (dep == DayReportDep.DCC部 && hdnKeyReportType.Value == "dccjqdzb")
            {
                tblView.Width = (120 + 100 * tbl.Columns.Count) + "px";

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w120\" rowspan=\"2\">公司</td>");
                strb.Append("<td colspan=\"5\">线索量</td>");
                strb.Append("<td colspan=\"5\">建档量</td>");
                strb.Append("<td colspan=\"3\">建档率</td>");
                strb.Append("<td colspan=\"5\">到店量</td>");
                strb.Append("<td colspan=\"3\">到店率</td>");
                strb.Append("<td colspan=\"5\">成交量</td>");
                strb.Append("<td colspan=\"3\">成交率</td>");
                strb.Append("<td colspan=\"3\">转化率</td>");
                strb.Append("<td colspan=\"4\">网络媒体成交贡献</td>");
                strb.Append("<td></td>");
                strb.Append("</tr>");
                strb.Append("<tr class=\"bold tc bggray\">");
                for (int i = 1; i < tbl.Columns.Count; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", tbl.Columns[i].ToString());
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["公司"]);
                    for (int j = 1; j < tbl.Columns.Count; j++)
                    {
                        string[] vals = tbl.Rows[i][tbl.Columns[j].ToString()].ToString().Split(new char[] { '|' }, StringSplitOptions.None);
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(vals[1]) ? "&nbsp;" : vals[1]);
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }

                strb.AppendLine("</table>");
            }
            else
            {
                Regex reg = new Regex(@"^[^\d]+\d+$");
                Regex regreplace = new Regex(@"\d+");

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w120\" rowspan=\"2\">公司</td>");
                for (int i = 1; i < tbl.Columns.Count; i++)
                {
                    strb.AppendFormat("<td class=\"w180\" colspan=\"3\">{0}</td>", reg.IsMatch(tbl.Columns[i].ToString()) ? regreplace.Replace(tbl.Columns[i].ToString(), string.Empty) : tbl.Columns[i].ToString());
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");
                strb.Append("<tr class=\"bold tc bggray\">");
                for (int i = 1; i < tbl.Columns.Count; i++)
                {
                    string[] vals = tbl.Rows[0][tbl.Columns[i].ToString()].ToString().Split(new char[] { '|' }, StringSplitOptions.None);
                    strb.AppendFormat("<td class=\"w60\">{0}</td>", string.IsNullOrEmpty(vals[2]) ? "目标" : vals[2].Split(new char[] { ',' }, StringSplitOptions.None)[0]);
                    strb.AppendFormat("<td class=\"w60\">{0}</td>", string.IsNullOrEmpty(vals[2]) ? "实际" : vals[2].Split(new char[] { ',' }, StringSplitOptions.None)[1]);
                    strb.AppendFormat("<td class=\"w60\">{0}</td>", "完成率");
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["公司"]);
                    for (int j = 1; j < tbl.Columns.Count; j++)
                    {
                        string[] vals = tbl.Rows[i][tbl.Columns[j].ToString()].ToString().Split(new char[] { '|' }, StringSplitOptions.None);
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(vals[0]) ? "&nbsp;" : vals[0]);
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(vals[1]) ? "&nbsp;" : vals[1]);
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(vals[3]) ? "&nbsp;" : vals[3]);
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }

                strb.AppendLine("</table>");
            }

            #endregion

            return strb.ToString();
        }

        /// <summary>
        /// 关键指标数据
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="list"></param>
        /// <param name="monthtarget"></param>
        /// <returns></returns>
        private DataTable GetKeyReport(DayReportDep dep, List<DailyReportInfo> list, MonthlyTargetInfo monthtarget, DataTable data, int corpid)
        {
            DataTable tbl = new DataTable();

            if (dep == DayReportDep.销售部)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 留单,标准总库存

                decimal ld = 0;
                DateTime day = DateTime.Today;
                if (corpid > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_last = new DailyReportQuery()
                    {
                        DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(corpid),
                        DayReportDep = DayReportDep.销售部
                    };
                    List<DailyReportInfo> list_last = DailyReports.Instance.GetList(query_last, true);
                    list_last = list_last.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    List<DailyReportModuleInfo> rlist_xs = DayReportModules.Instance.GetList(true);
                    rlist_xs = rlist_xs.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                    List<Dictionary<string, string>> data_last = new List<Dictionary<string, string>>();
                    for (int i = 0; i < list_last.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(list_last[i].SCReport))
                        {
                            data_last.Add(json.Deserialize<Dictionary<string, string>>(list_last[i].SCReport));
                        }
                    }
                    if (rlist_xs.Exists(l => l.Name == "展厅订单台数") && rlist_xs.Exists(l => l.Name == "展厅交车台数"))
                    {
                        int idztddts = rlist_xs.Find(l => l.Name == "展厅订单台数").ID;
                        int idztjcts = rlist_xs.Find(l => l.Name == "展厅交车台数").ID;
                        decimal hjztddts_last = Math.Round(data_last.Sum(d => d.ContainsKey(idztddts.ToString()) ? DataConvert.SafeDecimal(d[idztddts.ToString()]) : 0), 0);
                        decimal hjztjcts_last = Math.Round(data_last.Sum(d => d.ContainsKey(idztjcts.ToString()) ? DataConvert.SafeDecimal(d[idztjcts.ToString()]) : 0), 0);

                        ld = hjztddts_last - hjztjcts_last;
                    }
                }
                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[41];

                data.DefaultView.RowFilter = "项目='展厅首次来客批次'";
                decimal hjztsclkpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztsclkpc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='留档批次'";
                decimal hjldpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbldpc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅订单台数'";
                decimal hjztddts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztddts = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='试乘试驾次数'";
                decimal hjscsjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbscsjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅交车台数'";
                decimal hjztjcts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztjcts = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其中老客户转介绍交车台次'";
                decimal hjqzlkhzjsjcts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqzlkhzjsjcts = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='上牌台次'";
                decimal hjsptc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsptc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='上牌总金额'";
                decimal hjspzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbspzje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='美容交车台次'";
                decimal hjmrjctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbmrjctc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='美容交车总金额'";
                decimal hjmrjczje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbmrjczje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='延保无忧车服务购买个数'";
                decimal hjybtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbybtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='延保无忧车服务购买金额'";
                decimal hjybzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbybzje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjybml = hjybzje * 30 / 100;
                decimal mbybml = mbybzje * 30 / 100;
                data.DefaultView.RowFilter = "项目='展厅成交精品台次'";
                decimal hjztcjjptc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztcjjptc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='精品总金额'";
                decimal hjjpzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbjpzje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjjpml = hjjpzje / 2;
                decimal mbjpml = mbjpzje / 2;
                data.DefaultView.RowFilter = "项目='二网销售台次'";
                decimal hjewxstc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbewxstc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='二网精品金额'";
                decimal hjewjpje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbewjpje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='二网保险返利'";
                decimal hjewbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbewbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='外流销售台次'";
                decimal hjwlxstc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='推荐二手车评估数'";
                decimal hjjsescpgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='销售置换数'";
                decimal mbxszhs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjxszhs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='按揭净收入'";
                decimal hjajjsr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbajjsr = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='本地按揭台次'";
                decimal hjbdajtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbdajtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='银行按揭台次'";
                decimal hjyhajtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbyhajtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='厂家金融台次'";
                decimal hjcjjrtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbcjjrtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅含DCC保险台次'";
                decimal hjbxtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbxtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅含DCC保险总金额'";
                decimal hjbxzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbxzje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅含DCC保险返利'";
                decimal hjbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='入库台次'";
                decimal hjrktc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='免费保养台次（含赠送）'";
                decimal hjmfbytc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbmfbytc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='免费保养总金额'";
                decimal hjmfbyzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbmfbyzje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='玻璃无忧服务购买个数'";
                decimal hjblxtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbblxtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='机油套餐购买个数'";
                decimal hjjytcgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbjytcgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='划痕无忧服务购买个数'";
                decimal hjhhwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbhhwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='他品牌交车台次'";
                decimal hjtppjctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbtppjctc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='他品牌单车毛利'";
                decimal hjtppdcml = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbtppdcml = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='他品牌单车综合毛利'";
                decimal hjtppzhml = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbtppzhml = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其中他品牌新增订单台次'";
                decimal hjqztppxzddtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqztppxzddtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "总销售台次";
                rows[0]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSzxstc)) ? monthtarget.XSzxstc : (mbztjcts + mbewxstc + mbtppjctc).ToString();
                rows[0]["实际"] = hjztjcts + hjewxstc + hjtppjctc;

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "展厅占比";
                rows[1]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztzb)) ? monthtarget.XSztzb : ((mbztjcts + mbewxstc + mbtppjctc) == 0 ? string.Empty : Math.Round((mbztjcts + mbtppjctc) * 100 / (mbztjcts + mbewxstc + mbtppjctc), 0).ToString());
                rows[1]["实际"] = (hjztjcts + hjewxstc + hjtppjctc) == 0 ? string.Empty : Math.Round((hjztjcts + hjtppjctc) * 100 / (hjztjcts + hjewxstc + hjtppjctc), 0).ToString();

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "展厅留档率";
                rows[2]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztldl)) ? monthtarget.XSztldl : (mbztsclkpc == 0 ? string.Empty : Math.Round(mbldpc * 100 / mbztsclkpc, 0).ToString());
                rows[2]["实际"] = hjztsclkpc == 0 ? string.Empty : Math.Round(hjldpc * 100 / hjztsclkpc, 0).ToString();

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "展厅成交率";
                rows[3]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztcjl)) ? monthtarget.XSztcjl : (mbldpc == 0 ? string.Empty : Math.Round((mbztddts - ld) * 100 / mbldpc, 0).ToString());
                rows[3]["实际"] = hjldpc == 0 ? string.Empty : Math.Round((hjztddts - ld) * 100 / hjldpc, 0).ToString();

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "上牌率";
                rows[4]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSspl)) ? monthtarget.XSspl : (mbztjcts == 0 ? string.Empty : Math.Round(mbsptc * 100 / mbztjcts, 0).ToString());
                rows[4]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjsptc * 100 / hjztjcts, 0).ToString();

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "上牌单台";
                rows[5]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSspdt)) ? monthtarget.XSspdt : (mbsptc == 0 ? string.Empty : Math.Round(mbspzje / mbsptc, 0).ToString());
                rows[5]["实际"] = hjsptc == 0 ? string.Empty : Math.Round(hjspzje / hjsptc, 0).ToString();

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "展厅保险率";
                rows[6]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztbxl)) ? monthtarget.XSztbxl : (mbztjcts == 0 ? string.Empty : Math.Round(mbbxtc * 100 / mbztjcts, 0).ToString());
                rows[6]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjbxtc * 100 / hjztjcts, 0).ToString();

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "展厅保险单台";
                rows[7]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztbxdt)) ? monthtarget.XSztbxdt : (mbbxtc == 0 ? string.Empty : Math.Round(mbbxzje / mbbxtc, 0).ToString());
                rows[7]["实际"] = hjbxtc == 0 ? string.Empty : Math.Round(hjbxzje / hjbxtc, 0).ToString();

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "美容交车率";
                rows[8]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmrjcl)) ? monthtarget.XSmrjcl : (mbztjcts == 0 ? string.Empty : Math.Round(mbmrjctc * 100 / mbztjcts, 0).ToString());
                rows[8]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjmrjctc * 100 / hjztjcts, 0).ToString();

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "美容单台";
                rows[9]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmrdt)) ? monthtarget.XSmrdt : (mbmrjctc == 0 ? string.Empty : Math.Round(mbmrjczje / mbmrjctc, 0).ToString());
                rows[9]["实际"] = hjmrjctc == 0 ? string.Empty : Math.Round(hjmrjczje / hjmrjctc, 0).ToString();

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "展厅精品前装率";
                rows[10]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztjpqzl)) ? monthtarget.XSztjpqzl : (mbztjcts == 0 ? string.Empty : Math.Round(mbztcjjptc * 100 / mbztjcts, 0).ToString());
                rows[10]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjztcjjptc * 100 / hjztjcts, 0).ToString();

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "展厅精品平均单台";
                rows[11]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztjppjdt)) ? monthtarget.XSztjppjdt : (mbztcjjptc == 0 ? string.Empty : Math.Round(mbjpzje / mbztjcts, 0).ToString());
                rows[11]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjjpzje / hjztjcts, 0).ToString();

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "二网精品平均单台";
                rows[12]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSewjppjdt)) ? monthtarget.XSewjppjdt : (mbewxstc == 0 ? string.Empty : Math.Round(mbewjpje / mbewxstc, 0).ToString());
                rows[12]["实际"] = hjewxstc == 0 ? string.Empty : Math.Round(hjewjpje / hjewxstc, 0).ToString();

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "销售置换台次";
                rows[13]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSxszhtc)) ? monthtarget.XSxszhtc : mbxszhs.ToString();
                rows[13]["实际"] = hjxszhs;

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "按揭率";
                rows[14]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSajl)) ? monthtarget.XSajl : (mbztjcts == 0 ? string.Empty : Math.Round((mbbdajtc + mbyhajtc + mbcjjrtc) * 100 / mbztjcts, 0).ToString());
                rows[14]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round((hjbdajtc + hjyhajtc + hjcjjrtc) * 100 / hjztjcts, 0).ToString();

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "按揭平均单台";
                rows[15]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSajpjdt)) ? monthtarget.XSajpjdt : ((mbbdajtc + mbyhajtc + mbcjjrtc) == 0 ? string.Empty : Math.Round(mbajjsr / (mbbdajtc + mbyhajtc + mbcjjrtc), 0).ToString());
                rows[15]["实际"] = (hjbdajtc + hjyhajtc + hjcjjrtc) == 0 ? string.Empty : Math.Round(hjajjsr / (hjbdajtc + hjyhajtc + hjcjjrtc), 0).ToString();

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "免费保养渗透率";
                rows[16]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmfbystl)) ? monthtarget.XSmfbystl : (mbztjcts == 0 ? string.Empty : Math.Round(mbmfbytc * 100 / mbztjcts, 0).ToString());
                rows[16]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjmfbytc * 100 / hjztjcts, 0).ToString();

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "免费保养单台";
                rows[17]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmfbydt)) ? monthtarget.XSmfbydt : (mbmfbytc == 0 ? string.Empty : Math.Round(mbmfbyzje / mbmfbytc, 0).ToString());
                rows[17]["实际"] = hjmfbytc == 0 ? string.Empty : Math.Round(hjmfbyzje / hjmfbytc, 0).ToString();

                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "标准在库库存";
                rows[18]["目标"] = monthtarget == null ? string.Empty : Math.Round((mbztjcts + mbewxstc) * DataConvert.SafeDecimal(monthtarget.XSzzts) / 30, 0).ToString();
                rows[18]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc;

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "库存利息";
                rows[19]["目标"] = monthtarget == null ? string.Empty : Math.Round(DataConvert.SafeInt(rows[18]["目标"]) * DataConvert.SafeDecimal(monthtarget.XSclpjdj) * (decimal)0.0075, 0).ToString();
                rows[19]["实际"] = monthtarget == null ? string.Empty : Math.Round(DataConvert.SafeInt(rows[18]["实际"]) * DataConvert.SafeDecimal(monthtarget.XSclpjdj) * (decimal)0.0075, 0).ToString();

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "附加值合计";
                rows[20]["目标"] = mbspzje + mbmrjczje + mbajjsr + mbjpml + mbybml + mbewbxfl + mbbxfl;
                rows[20]["实际"] = hjspzje + hjmrjczje + hjajjsr + hjjpml + hjybml + hjewbxfl + hjbxfl;

                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "转介绍率";
                rows[21]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSzjsl)) ? monthtarget.XSzjsl : (mbztjcts == 0 ? string.Empty : Math.Round(mbqzlkhzjsjcts * 100 / mbztjcts, 0).ToString());
                rows[21]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjqzlkhzjsjcts * 100 / hjztjcts, 0).ToString();

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "展厅新增订单";
                rows[22]["目标"] = Math.Round(mbztddts - ld, 0).ToString();
                rows[22]["实际"] = Math.Round(hjztddts - ld, 0).ToString();

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "无忧玻璃渗透率";
                rows[23]["目标"] = mbbxtc == 0 ? string.Empty : Math.Round(mbblxtc * 100 / mbbxtc, 0).ToString();
                rows[23]["实际"] = hjbxtc == 0 ? string.Empty : Math.Round(hjblxtc * 100 / hjbxtc, 0).ToString();

                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "无忧延保渗透率";
                rows[24]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbybtc * 100 / mbztjcts, 0).ToString();
                rows[24]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjybtc * 100 / hjztjcts, 0).ToString();

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "机油套餐渗透率";
                rows[25]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbjytcgmgs * 100 / mbztjcts, 0).ToString();
                rows[25]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjjytcgmgs * 100 / hjztjcts, 0).ToString();

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "无忧划痕渗透率";
                rows[26]["目标"] = mbbxtc == 0 ? string.Empty : Math.Round(mbhhwyfwgmgs * 100 / mbbxtc, 0).ToString();
                rows[26]["实际"] = hjbxtc == 0 ? string.Empty : Math.Round(hjhhwyfwgmgs * 100 / hjbxtc, 0).ToString();

                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "试乘试驾率";
                rows[27]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSscsjl)) ? monthtarget.XSzjsl : ((mbztddts - ld) == 0 ? string.Empty : Math.Round(mbscsjs * 100 / (mbztddts - ld), 0).ToString());
                rows[27]["实际"] = (hjztddts - ld) == 0 ? string.Empty : Math.Round(hjscsjs * 100 / hjztddts - ld, 0).ToString();

                rows[28] = tbl.NewRow();
                rows[28]["关键指标"] = "在库平均单台成本价";
                rows[28]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSclpjdj;

                rows[29] = tbl.NewRow();
                rows[29]["关键指标"] = "在库库存";
                rows[29]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc;

                rows[30] = tbl.NewRow();
                rows[30]["关键指标"] = "在库超3个月";
                rows[30]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSzkcsgytc;

                rows[31] = tbl.NewRow();
                rows[31]["关键指标"] = "在途";
                rows[31]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSztcl;

                rows[32] = tbl.NewRow();
                rows[32]["关键指标"] = "总库存";
                rows[32]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc + DataConvert.SafeInt(monthtarget == null ? string.Empty : monthtarget.XSztcl);

                rows[33] = tbl.NewRow();
                rows[33]["关键指标"] = "上月留单";
                rows[33]["实际"] = ld.ToString();

                rows[34] = tbl.NewRow();
                rows[34]["关键指标"] = "本月留单";
                rows[34]["实际"] = hjztddts - hjztjcts;

                rows[35] = tbl.NewRow();
                rows[35]["关键指标"] = "厂家虚出";
                rows[35]["实际"] = monthtarget == null ? string.Empty : monthtarget.XScjxctc;

                rows[36] = tbl.NewRow();
                rows[36]["关键指标"] = "他品牌留单";
                rows[36]["实际"] = hjqztppxzddtc - hjtppjctc;

                rows[37] = tbl.NewRow();
                rows[37]["关键指标"] = "他品牌销售台次";
                rows[37]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStppxstc)) ? monthtarget.XStppxstc : mbtppjctc.ToString();
                rows[37]["实际"] = hjtppjctc;

                rows[38] = tbl.NewRow();
                rows[38]["关键指标"] = "他品牌单车毛利";
                rows[38]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStppdcml)) ? monthtarget.XStppdcml : mbtppdcml.ToString();
                rows[38]["实际"] = hjtppdcml;

                rows[39] = tbl.NewRow();
                rows[39]["关键指标"] = "他品牌综合毛利";
                rows[39]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStppzhml)) ? monthtarget.XStppzhml : mbtppzhml.ToString();
                rows[39]["实际"] = hjtppzhml;

                rows[40] = tbl.NewRow();
                rows[40]["关键指标"] = "他品牌平均单台";
                rows[40]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStpppjdt)) ? monthtarget.XStpppjdt : (mbtppjctc == 0 ? string.Empty : Math.Round((mbtppdcml + mbtppzhml) / mbtppjctc, 0).ToString());
                rows[40]["实际"] = hjtppjctc == 0 ? string.Empty : Math.Round((hjtppdcml + hjtppzhml) / hjtppjctc, 0).ToString();

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.售后部)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 微信客户总数

                //decimal hjwxkhzs = 0;
                //if (corpid > 0)
                //{
                //    DailyReportQuery query_all = new DailyReportQuery()
                //    {
                //        CorporationID = corpid,
                //        DayReportDep = DayReportDep.售后部
                //    };
                //    List<DailyReportInfo> list_all = DailyReports.Instance.GetList(query_all, false);
                //    list_all = list_all.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                //    List<DailyReportModuleInfo> rlist_sh = DayReportModules.Instance.GetList(true);
                //    rlist_sh = rlist_sh.FindAll(l => l.Department == DayReportDep.售后部).OrderBy(l => l.Sort).ToList();
                //    List<Dictionary<string, string>> data_all = new List<Dictionary<string, string>>();
                //    for (int i = 0; i < list_all.Count; i++)
                //    {
                //        if (!string.IsNullOrEmpty(list_all[i].SCReport))
                //        {
                //            data_all.Add(json.Deserialize<Dictionary<string, string>>(list_all[i].SCReport));
                //        }
                //    }
                //    if (rlist_sh.Exists(l => l.Name == "微信客户数"))
                //    {
                //        int idwxkhzs = rlist_sh.Find(l => l.Name == "微信客户数").ID;
                //        hjwxkhzs = Math.Round(data_all.Sum(d => d.ContainsKey(idwxkhzs.ToString()) ? DataConvert.SafeDecimal(d[idwxkhzs.ToString()]) : 0), 0);
                //    }
                //}

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[40];

                data.DefaultView.RowFilter = "项目='来厂台次'";
                decimal hjlctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mblctc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其中预约台次'";
                decimal hjqzyytc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqzyytc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='红旭汇绑定订单数'";
                decimal hjqzhxhbdtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqzhxhbdtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='当日产值'";
                decimal hjdrcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdrcz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='事故总产值'";
                decimal hjsgzcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsgzcz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='养护产值'";
                decimal hjyhcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbyhcz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='收到信息数'";
                decimal hjsdxxs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsdxxs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='成功台次'";
                decimal hjcgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbcgtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='精品美容产值'";
                decimal mbjpmrcz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjjpmrcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='索赔上报台次'";
                decimal hjspsbtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbspsbtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='索赔批准台次'";
                decimal hjsppztc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsppztc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='延保台次'";
                decimal hjybtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbybtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='机电内返台次'";
                decimal hjjdnf = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbjdnf = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='钣喷内返台次'";
                decimal hjbpnf = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbpnf = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='外返台次'";
                decimal hjwftc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbwftc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='缺件数量'";
                decimal hjqjsl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqjsl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='意向数量'";
                decimal hjyxsl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbyxsl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='意向成功数'";
                decimal hjyxcgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbyxcgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='保养台次'";
                decimal hjbytc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbytc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='保养产值'";
                decimal hjbycz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbycz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='玻璃险'";
                decimal hjblx = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbblx = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='发动机下护板'";
                decimal hjfdjxhb = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbfdjxhb = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='划痕险'";
                decimal hjhhx = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbhhx = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='微信客户数'";
                decimal hjwxkhs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbwxkhs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='中保理赔'";
                decimal hjzblp = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzblp = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='太保'";
                decimal hjtb = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbtb = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='平安'";
                decimal hjpa = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbpa = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='人寿'";
                decimal hjrs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbrs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='大地'";
                decimal hjdd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='中华联合'";
                decimal hjzhlh = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzhlh = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='浙商'";
                decimal hjzs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='大众'";
                decimal hjdz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其他保险'";
                decimal hjqt = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqt = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='轮胎数'";
                decimal hjlts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mblts = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='电瓶数'";
                decimal hjdps = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdps = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='导航升级客户数'";
                decimal hjdhsjkhs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdhsjkhs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='空调滤清器'";
                decimal hjktlqq = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbktlqq = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='免费保养'";
                decimal hjzsmfby = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzsmfby = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='介绍二手车评估数'";
                decimal hjjsescpgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbjsescpgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "来厂台次";
                rows[0]["目标"] = mblctc;
                rows[0]["实际"] = hjlctc;

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "预约率";
                rows[1]["目标"] = mblctc == 0 ? string.Empty : Math.Round(mbqzyytc * 100 / mblctc, 1).ToString();
                rows[1]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjqzyytc * 100 / hjlctc, 1).ToString();

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "产值达成";
                rows[2]["目标"] = mbdrcz;
                rows[2]["实际"] = hjdrcz;

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "养护比例";

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "精品美容产值";
                rows[4]["目标"] = mbjpmrcz;
                rows[4]["实际"] = hjjpmrcz;

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "延保销售";
                rows[5]["目标"] = mbybtc;
                rows[5]["实际"] = hjybtc;

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "轮胎数";
                rows[6]["目标"] = mblts;
                rows[6]["实际"] = hjlts;

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "电瓶数";
                rows[7]["目标"] = mbdps;
                rows[7]["实际"] = hjdps;

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "微信客户数";
                rows[8]["目标"] = mbwxkhs;
                rows[8]["实际"] = hjwxkhs;

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "导航升级客户数";
                rows[9]["目标"] = mbdhsjkhs;
                rows[9]["实际"] = hjdhsjkhs;

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "空调滤清器";
                rows[10]["目标"] = mbktlqq;
                rows[10]["实际"] = hjktlqq;

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "免费保养";
                rows[11]["目标"] = mbzsmfby;
                rows[11]["实际"] = hjzsmfby;

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "介绍二手车评估数";
                rows[12]["目标"] = mbjsescpgs;
                rows[12]["实际"] = hjjsescpgs;

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "内返率";
                rows[13]["目标"] = mblctc == 0 ? string.Empty : Math.Round((mbjdnf + mbbpnf + mbwftc) * 100 / mblctc, 1).ToString();
                rows[13]["实际"] = hjlctc == 0 ? string.Empty : Math.Round((hjjdnf + hjbpnf + hjwftc) * 100 / hjlctc, 1).ToString();

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "供货及时率";
                rows[14]["目标"] = mblctc == 0 ? string.Empty : Math.Round((1 - mbqjsl / mblctc) * 100, 1).ToString();
                rows[14]["实际"] = hjlctc == 0 ? string.Empty : Math.Round((1 - hjqjsl / hjlctc) * 100, 1).ToString();

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "保养单台产值";
                rows[15]["目标"] = mbbytc == 0 ? string.Empty : Math.Round(mbbycz / mbbytc, 1).ToString();
                rows[15]["实际"] = hjbytc == 0 ? string.Empty : Math.Round(hjbycz / hjbytc, 1).ToString();

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "保养台次占比";
                rows[16]["目标"] = mblctc == 0 ? string.Empty : Math.Round(mbbytc * 100 / mblctc, 1).ToString();
                rows[16]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjbytc * 100 / hjlctc, 1).ToString();

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "事故总产值";
                rows[17]["目标"] = mbsgzcz;
                rows[17]["实际"] = hjsgzcz;

                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "中保";
                rows[18]["目标"] = hjzblp.ToString();
                rows[18]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzblp * 100 / hjsgzcz, 1).ToString();

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "太保";
                rows[19]["目标"] = hjtb.ToString();
                rows[19]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjtb * 100 / hjsgzcz, 1).ToString();

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "平安";
                rows[20]["目标"] = hjpa.ToString();
                rows[20]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjpa * 100 / hjsgzcz, 1).ToString();

                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "人寿";
                rows[21]["目标"] = hjrs.ToString();
                rows[21]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjrs * 100 / hjsgzcz, 1).ToString();

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "大地";
                rows[22]["目标"] = hjdd.ToString();
                rows[22]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjdd * 100 / hjsgzcz, 1).ToString();

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "中华联合";
                rows[23]["目标"] = hjzhlh.ToString();
                rows[23]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzhlh * 100 / hjsgzcz, 1).ToString();

                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "浙商";
                rows[24]["目标"] = hjzs.ToString();
                rows[24]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzs * 100 / hjsgzcz, 1).ToString();

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "大众";
                rows[25]["目标"] = hjdz.ToString();
                rows[25]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjdz * 100 / hjsgzcz, 1).ToString();

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "其他";
                rows[26]["目标"] = hjqt.ToString();
                rows[26]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjqt * 100 / hjsgzcz, 1).ToString();

                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "合计";
                rows[27]["目标"] = (hjzblp + hjtb + hjpa + hjrs + hjdd + hjzhlh + hjzs + hjdz + hjqt).ToString();

                rows[28] = tbl.NewRow();
                rows[28]["关键指标"] = "美容比例";
                rows[28]["目标"] = mbdrcz == 0 ? string.Empty : Math.Round(mbjpmrcz * 100 / mbdrcz, 1).ToString();
                rows[28]["实际"] = hjdrcz == 0 ? string.Empty : Math.Round(hjjpmrcz * 100 / hjdrcz, 1).ToString();

                rows[29] = tbl.NewRow();
                rows[29]["关键指标"] = "单台产值";
                rows[29]["目标"] = mblctc == 0 ? string.Empty : Math.Round(mbdrcz / mblctc, 1).ToString();
                rows[29]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjdrcz / hjlctc, 1).ToString();

                rows[30] = tbl.NewRow();
                rows[30]["关键指标"] = "事故产值占比";
                rows[30]["目标"] = mbdrcz == 0 ? string.Empty : Math.Round(mbsgzcz * 100 / mbdrcz, 1).ToString();
                rows[30]["实际"] = hjdrcz == 0 ? string.Empty : Math.Round(hjsgzcz * 100 / hjdrcz, 1).ToString();

                rows[3]["目标"] = (mbdrcz - (DataConvert.SafeDecimal(rows[30]["目标"]) / 100 - (decimal)0.5) * mbdrcz) == 0 ? string.Empty : Math.Round(mbyhcz * 100 / ((mbdrcz - (DataConvert.SafeDecimal(rows[30]["目标"]) / 100 - (decimal)0.5) * mbdrcz)), 1).ToString();
                rows[3]["实际"] = (hjdrcz - (DataConvert.SafeDecimal(rows[30]["实际"]) / 100 - (decimal)0.5) * hjdrcz) == 0 ? string.Empty : Math.Round(hjyhcz * 100 / ((hjdrcz - (DataConvert.SafeDecimal(rows[30]["实际"]) / 100 - (decimal)0.5) * hjdrcz)), 1).ToString();

                rows[31] = tbl.NewRow();
                rows[31]["关键指标"] = "事故首次成功率";
                rows[31]["目标"] = mbsdxxs == 0 ? string.Empty : Math.Round(mbcgtc * 100 / mbsdxxs, 1).ToString();
                rows[31]["实际"] = hjsdxxs == 0 ? string.Empty : Math.Round(hjcgtc * 100 / hjsdxxs, 1).ToString();

                rows[32] = tbl.NewRow();
                rows[32]["关键指标"] = "事故再次成功率";
                rows[32]["目标"] = mbyxsl == 0 ? string.Empty : Math.Round(mbyxcgs * 100 / mbyxsl, 1).ToString();
                rows[32]["实际"] = hjyxsl == 0 ? string.Empty : Math.Round(hjyxcgs * 100 / hjyxsl, 1).ToString();

                rows[33] = tbl.NewRow();
                rows[33]["关键指标"] = "索赔成功率";
                rows[33]["目标"] = mbspsbtc == 0 ? string.Empty : Math.Round(mbsppztc * 100 / mbspsbtc, 1).ToString();
                rows[33]["实际"] = hjspsbtc == 0 ? string.Empty : Math.Round(hjsppztc * 100 / hjspsbtc, 1).ToString();

                rows[34] = tbl.NewRow();
                rows[34]["关键指标"] = "玻璃险";
                rows[34]["目标"] = mbblx;
                rows[34]["实际"] = hjblx;

                rows[35] = tbl.NewRow();
                rows[35]["关键指标"] = "发动机下护板";
                rows[35]["目标"] = mbfdjxhb;
                rows[35]["实际"] = hjfdjxhb;

                rows[36] = tbl.NewRow();
                rows[36]["关键指标"] = "划痕险";
                rows[36]["目标"] = mbhhx;
                rows[36]["实际"] = hjhhx;

                rows[37] = tbl.NewRow();
                rows[37]["关键指标"] = "红旭汇绑定数";
                rows[37]["目标"] = mbqzhxhbdtc;
                rows[37]["实际"] = hjqzhxhbdtc;

                rows[38] = tbl.NewRow();
                rows[38]["关键指标"] = "红旭汇绑定率";
                rows[38]["目标"] = string.Empty;
                rows[38]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjqzhxhbdtc * 100 / hjlctc, 1).ToString();

                rows[39] = tbl.NewRow();
                rows[39]["关键指标"] = "本月微信客户数";
                rows[39]["目标"] = mbwxkhs.ToString();
                rows[39]["实际"] = hjwxkhs.ToString();

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.市场部)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[13];

                data.DefaultView.RowFilter = "项目='展厅首次到店记录数'";
                decimal hjztscdfjls = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztscdfjls = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅首次到店建档数'";
                decimal hjsfjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsfjds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='自然到店'";
                decimal hjzrdd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzrdd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='转介绍渠道到店'";
                decimal hjzjsqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzjsqddd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='增换购渠道到店'";
                decimal hjzhgqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzhgqddd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='DCC邀约到店'";
                decimal hjyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbyydd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='市场活动到店'";
                decimal hjwtdd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbwtdd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='网络渠道到店'";
                decimal hjwlqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbwlqddd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='电台渠道到店'";
                decimal hjdtqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdtqddd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='户外广告渠道到店'";
                decimal hjhwggqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbhwggqddd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='114渠道到店'";
                decimal hj114qddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mb114qddd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其他渠道到店'";
                decimal hjqtqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqtqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "展厅首次到店建档数";
                rows[0]["目标"] = mbsfjds == 0 ? string.Empty : mbsfjds.ToString();
                rows[0]["实际"] = hjsfjds;

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "首次到访达成率";
                rows[1]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCscdfdcl;
                rows[1]["实际"] = mbztscdfjls == 0 ? string.Empty : Math.Round(hjztscdfjls * 100 / mbztscdfjls, 1).ToString();

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "首次到访建档率";
                rows[2]["目标"] = mbztscdfjls == 0 ? string.Empty : Math.Round(mbsfjds * 100 / mbztscdfjls, 1).ToString();
                rows[2]["实际"] = hjztscdfjls == 0 ? string.Empty : Math.Round(hjsfjds * 100 / hjztscdfjls, 1).ToString();

                decimal ddCount = hjwlqddd + hjhwggqddd + hjdtqddd + hjzhgqddd + hjzjsqddd + hjzrdd + hjwtdd + hjyydd + hj114qddd + hjqtqddd;
                decimal mbddCount = mbwlqddd + mbhwggqddd + mbdtqddd + mbzhgqddd + mbzjsqddd + mbzrdd + mbwtdd + mbyydd + mb114qddd + mbqtqddd;

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "自然到店";
                rows[3]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbzrdd * 100 / mbddCount, 1).ToString() + "%");
                rows[3]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjzrdd * 100 / ddCount, 1).ToString() + "%");
                rows[3]["详细"] = "目标占比,实际占比";

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "转介绍渠道到店";
                rows[4]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbzjsqddd * 100 / mbddCount, 1).ToString() + "%");
                rows[4]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjzjsqddd * 100 / ddCount, 1).ToString() + "%");
                rows[4]["详细"] = "目标占比,实际占比";

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "增换购渠道到店";
                rows[5]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbzhgqddd * 100 / mbddCount, 1).ToString() + "%");
                rows[5]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjzhgqddd * 100 / ddCount, 1).ToString() + "%");
                rows[5]["详细"] = "目标占比,实际占比";

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "DCC邀约到店";
                rows[6]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbyydd * 100 / mbddCount, 1).ToString() + "%");
                rows[6]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjyydd * 100 / ddCount, 1).ToString() + "%");
                rows[6]["详细"] = "目标占比,实际占比";

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "市场外拓活动到店";
                rows[7]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbwtdd * 100 / mbddCount, 1).ToString() + "%");
                rows[7]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjwtdd * 100 / ddCount, 1).ToString() + "%");
                rows[7]["详细"] = "目标占比,实际占比";

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "网络渠道到店";
                rows[8]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbwlqddd * 100 / mbddCount, 1).ToString() + "%");
                rows[8]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjwlqddd * 100 / ddCount, 1).ToString() + "%");
                rows[8]["详细"] = "目标占比,实际占比";

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "电台渠道到店";
                rows[9]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbdtqddd * 100 / mbddCount, 1).ToString() + "%");
                rows[9]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjdtqddd * 100 / ddCount, 1).ToString() + "%");
                rows[9]["详细"] = "目标占比,实际占比";

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "户外广告渠道到店";
                rows[10]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbhwggqddd * 100 / mbddCount, 1).ToString() + "%");
                rows[10]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjhwggqddd * 100 / ddCount, 1).ToString() + "%");
                rows[10]["详细"] = "目标占比,实际占比";

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "114渠道到店";
                rows[11]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mb114qddd * 100 / mbddCount, 1).ToString() + "%");
                rows[11]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hj114qddd * 100 / ddCount, 1).ToString() + "%");
                rows[11]["详细"] = "目标占比,实际占比";

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "其他渠道到店";
                rows[12]["目标"] = mbddCount == 0 ? string.Empty : (Math.Round(mbqtqddd * 100 / mbddCount, 1).ToString() + "%");
                rows[12]["实际"] = ddCount == 0 ? string.Empty : (Math.Round(hjqtqddd * 100 / ddCount, 1).ToString() + "%");
                rows[12]["详细"] = "目标占比,实际占比";

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.行政部)
            {
                #region 表结构

                tbl.Columns.Add("项目");
                tbl.Columns.Add("车牌、负责人");
                tbl.Columns.Add("第一周");
                tbl.Columns.Add("第二周");
                tbl.Columns.Add("第三周");
                tbl.Columns.Add("第四周");
                tbl.Columns.Add("上月未处理");
                tbl.Columns.Add("违章");

                #endregion

                #region 表数据

                string[] cpfzr = new string[7] { "", "", "", "", "", "", "" };
                if (!string.IsNullOrEmpty(monthtarget == null ? string.Empty : monthtarget.XZcpfzr))
                    cpfzr = monthtarget.XZcpfzr.Split(new char[] { '|' }, StringSplitOptions.None);
                string[] dyzclwzcs = new string[7] { "", "", "", "", "", "", "" };
                if (!string.IsNullOrEmpty(monthtarget == null ? string.Empty : monthtarget.XZdyzclwzcs))
                    dyzclwzcs = monthtarget.XZdyzclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                string[] dezclwzcs = new string[7] { "", "", "", "", "", "", "" };
                if (!string.IsNullOrEmpty(monthtarget == null ? string.Empty : monthtarget.XZdezclwzcs))
                    dezclwzcs = monthtarget.XZdezclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                string[] dszclwzcs = new string[7] { "", "", "", "", "", "", "" };
                if (!string.IsNullOrEmpty(monthtarget == null ? string.Empty : monthtarget.XZdszclwzcs))
                    dszclwzcs = monthtarget.XZdszclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                string[] dsizclwzcs = new string[7] { "", "", "", "", "", "", "" };
                if (!string.IsNullOrEmpty(monthtarget == null ? string.Empty : monthtarget.XZdsizclwzcs))
                    dsizclwzcs = monthtarget.XZdsizclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                string[] sywcl = new string[7] { "", "", "", "", "", "", "" };
                if (!string.IsNullOrEmpty(monthtarget == null ? string.Empty : monthtarget.XZsywcl))
                    sywcl = monthtarget.XZsywcl.Split(new char[] { '|' }, StringSplitOptions.None);
                string[] wz = new string[7] { "", "", "", "", "", "", "" };
                if (!string.IsNullOrEmpty(monthtarget == null ? string.Empty : monthtarget.XZwz))
                    wz = monthtarget.XZwz.Split(new char[] { '|' }, StringSplitOptions.None);

                DataRow[] rows = new DataRow[12];

                for (int i = 0; i < 7; i++)
                {
                    rows[i] = tbl.NewRow();
                    rows[i]["项目"] = "车辆违章次数";
                    rows[i]["车牌、负责人"] = i >= cpfzr.Length ? string.Empty : cpfzr[i];
                    rows[i]["第一周"] = i >= dyzclwzcs.Length ? string.Empty : dyzclwzcs[i];
                    rows[i]["第二周"] = i >= dezclwzcs.Length ? string.Empty : dezclwzcs[i];
                    rows[i]["第三周"] = i >= dszclwzcs.Length ? string.Empty : dszclwzcs[i];
                    rows[i]["第四周"] = i >= dsizclwzcs.Length ? string.Empty : dsizclwzcs[i];
                    rows[i]["上月未处理"] = i >= sywcl.Length ? string.Empty : sywcl[i];
                    rows[i]["违章"] = i >= wz.Length ? string.Empty : wz[i];
                }

                rows[7] = tbl.NewRow();
                rows[7]["项目"] = "迟到人数";
                rows[7]["车牌、负责人"] = string.Empty;
                rows[7]["第一周"] = monthtarget == null ? string.Empty : monthtarget.XZdyzcdrs;
                rows[7]["第二周"] = monthtarget == null ? string.Empty : monthtarget.XZdezcdrs;
                rows[7]["第三周"] = monthtarget == null ? string.Empty : monthtarget.XZdszcdrs;
                rows[7]["第四周"] = monthtarget == null ? string.Empty : monthtarget.XZdsizcdrs;

                rows[8] = tbl.NewRow();
                rows[8]["项目"] = "请假人数";
                rows[8]["车牌、负责人"] = string.Empty;
                rows[8]["第一周"] = monthtarget == null ? string.Empty : monthtarget.XZdyzqjrs;
                rows[8]["第二周"] = monthtarget == null ? string.Empty : monthtarget.XZdezqjrs;
                rows[8]["第三周"] = monthtarget == null ? string.Empty : monthtarget.XZdszqjrs;
                rows[8]["第四周"] = monthtarget == null ? string.Empty : monthtarget.XZdsizqjrs;

                rows[9] = tbl.NewRow();
                rows[9]["项目"] = "旷工人数";
                rows[9]["车牌、负责人"] = string.Empty;
                rows[9]["第一周"] = monthtarget == null ? string.Empty : monthtarget.XZdyzkgrs;
                rows[9]["第二周"] = monthtarget == null ? string.Empty : monthtarget.XZdezkgrs;
                rows[9]["第三周"] = monthtarget == null ? string.Empty : monthtarget.XZdszkgrs;
                rows[9]["第四周"] = monthtarget == null ? string.Empty : monthtarget.XZdsizkgrs;

                rows[10] = tbl.NewRow();
                rows[10]["项目"] = "出差培训人次";
                rows[10]["车牌、负责人"] = string.Empty;
                rows[10]["第一周"] = monthtarget == null ? string.Empty : monthtarget.XZdyzccpxrc;
                rows[10]["第二周"] = monthtarget == null ? string.Empty : monthtarget.XZdezccpxrc;
                rows[10]["第三周"] = monthtarget == null ? string.Empty : monthtarget.XZdszccpxrc;
                rows[10]["第四周"] = monthtarget == null ? string.Empty : monthtarget.XZdsizccpxrc;

                rows[11] = tbl.NewRow();
                rows[11]["项目"] = "安全事故损失额";
                rows[11]["车牌、负责人"] = string.Empty;
                rows[11]["第一周"] = monthtarget == null ? string.Empty : monthtarget.XZdyzaqsgsse;
                rows[11]["第二周"] = monthtarget == null ? string.Empty : monthtarget.XZdezaqsgsse;
                rows[11]["第三周"] = monthtarget == null ? string.Empty : monthtarget.XZdszaqsgsse;
                rows[11]["第四周"] = monthtarget == null ? string.Empty : monthtarget.XZdsizaqsgsse;

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.DCC部)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 表数据

                int rownum = 32;
                if (hdnKeyReportType.Value == "dccqdzhl") rownum = 28;
                else if (hdnKeyReportType.Value == "dccjqdzb") rownum = 36;
                else if (hdnKeyReportType.Value == "dcczhhz") rownum = 13;

                DataRow[] rows = new DataRow[rownum];

                #region 数据准备

                data.DefaultView.RowFilter = "项目='DCC订单数'";
                decimal hjdccdds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdccdds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增展厅前台来电数'";
                decimal hjxzztqtlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzztqtlds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增展厅前台来电建档数'";
                decimal hjxzztqtldjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzztqtldjds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增网络线索总量'";
                decimal hjxzwlxszl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzwlxszl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增网络线索总留档量'";
                decimal hjxzwlxszldl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzwlxszldl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='呼出总量'";
                decimal hjhczl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbhczl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='呼出有效数'";
                decimal hjhcyxs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbhcyxs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='首次邀约到店客户总数'";
                decimal hjscyyddkhzs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbscyyddkhzs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='再次邀约到店数'";
                decimal hjzcyydds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzcyydds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='新增DCC线索总量'";
                decimal hjxzdccxszl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzdccxszl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增DCC线索建档量'";
                decimal hjxzdccxsjdl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzdccxsjdl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='未建档-重复数'";
                decimal hjwjdcfs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='未建档-找售后'";
                decimal hjwjdzsh = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='未建档-外区域'";
                decimal hjwjdwqy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='未建档-信息错误'";
                decimal hjwjdxxcw = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='未建档-二网经销商'";
                decimal hjwjdewjxs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='未建档-找人'";
                decimal hjwjdzr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                data.DefaultView.RowFilter = "项目='汽车之家留档数'";
                decimal hjqczjlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqczjlds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='易车网留档数'";
                decimal hjycwlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbycwlds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='太平洋留档数'";
                decimal hjtpylds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbtpylds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='本地网络留档数'";
                decimal hjbdwllds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbdwllds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其他网络留档数'";
                decimal hjqtwllds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqtwllds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='转介绍线索数'";
                decimal hjzjsyss = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzjsyss = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='展厅当日转入'";
                decimal hjztdrzr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztdrzr = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅上月当日转入'";
                decimal hjztsydrzr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztsydrzr = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅双月当日转入'";
                decimal hjztshydrzr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztshydrzr = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅三月有强制转入'";
                decimal hjztsyyqzzr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztsyyqzzr = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                //展厅转入总客源
                decimal hjztzrzky = hjztdrzr + hjztsydrzr + hjztshydrzr + hjztsyyqzzr;
                decimal mbztzrzky = mbztdrzr + mbztsydrzr + mbztshydrzr + mbztsyyqzzr;

                data.DefaultView.RowFilter = "项目='汽车之家邀约到店'";
                decimal hjqczjyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqczjyydd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='易车网邀约到店'";
                decimal hjycwyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbycwyydd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='太平洋汽车网邀约到店'";
                decimal hjtpyyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbtpyyydd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其他网络邀约到店'";
                decimal hjqtwlyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqtwlyydd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='转介绍邀约到店'";
                decimal hjzjsyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzjsyydd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅转入邀约到店'";
                decimal hjztzryydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztzryydd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='DCC成交总台数'";
                decimal hjcjzts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbcjzts = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='汽车之家成交数'";
                decimal hjqczjcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqczjcjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='易车网成交数'";
                decimal hjycwcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbycwcjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='太平洋成交数'";
                decimal hjtpycjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbtpycjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='其他网络成交数'";
                decimal hjqtwlcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbqtwlcjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='转介绍成交数'";
                decimal hjzjscjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzjscjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='触点成交数'";
                decimal hjcdcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbcdcjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='CRm下达成交数'";
                decimal hjcrmxdcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbcrmxdcjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅转入成交数'";
                decimal hjztzrcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztzrcjs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='展厅销量'";
                decimal hjztxl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztxl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                #endregion

                if (string.IsNullOrEmpty(hdnKeyReportType.Value))
                {
                    #region 留单

                    decimal ld = 0;
                    DateTime day = DateTime.Today;
                    if (corpid > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                    {
                        DailyReportQuery query_last = new DailyReportQuery()
                        {
                            DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                            CorporationID = corpid,
                            DayReportDep = DayReportDep.DCC部
                        };
                        List<DailyReportInfo> list_last = DailyReports.Instance.GetList(query_last, true);
                        list_last = list_last.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        List<DailyReportModuleInfo> rlist_dcc = DayReportModules.Instance.GetList(true);
                        rlist_dcc = rlist_dcc.FindAll(l => l.Department == DayReportDep.DCC部).OrderBy(l => l.Sort).ToList();
                        List<Dictionary<string, string>> data_last = new List<Dictionary<string, string>>();
                        for (int i = 0; i < list_last.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(list_last[i].SCReport))
                            {
                                data_last.Add(json.Deserialize<Dictionary<string, string>>(list_last[i].SCReport));
                            }
                        }
                        if (rlist_dcc.Exists(l => l.Name == "DCC订单数") && rlist_dcc.Exists(l => l.Name == "DCC成交总台数"))
                        {
                            int iddccdds = rlist_dcc.Find(l => l.Name == "DCC订单数").ID;
                            int iddcccjztts = rlist_dcc.Find(l => l.Name == "DCC成交总台数").ID;
                            decimal hjdccdds_last = Math.Round(data_last.Sum(d => d.ContainsKey(iddccdds.ToString()) ? DataConvert.SafeDecimal(d[iddccdds.ToString()]) : 0), 0);
                            decimal hjdcccjztts_last = Math.Round(data_last.Sum(d => d.ContainsKey(iddcccjztts.ToString()) ? DataConvert.SafeDecimal(d[iddcccjztts.ToString()]) : 0), 0);

                            ld = hjdccdds_last - hjdcccjztts_last;
                        }
                    }
                    #endregion

                    #region 关键指标汇总

                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "DCC订单量";
                    rows[0]["目标"] = mbdccdds;
                    rows[0]["实际"] = hjdccdds == 0 ? string.Empty : hjdccdds.ToString();

                    rows[1] = tbl.NewRow();
                    rows[1]["关键指标"] = "DCC成交量";
                    rows[1]["目标"] = mbcjzts;
                    rows[1]["实际"] = hjcjzts == 0 ? string.Empty : hjcjzts.ToString();

                    rows[2] = tbl.NewRow();
                    rows[2]["关键指标"] = "销量占展厅比";
                    rows[2]["目标"] = mbztxl == 0 ? string.Empty : Math.Round(mbcjzts * 100 / mbztxl, 1).ToString();
                    rows[2]["实际"] = hjztxl == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjztxl, 1).ToString();

                    rows[3] = tbl.NewRow();
                    rows[3]["关键指标"] = "前台首电建档率";
                    rows[3]["目标"] = mbxzztqtlds == 0 ? string.Empty : Math.Round(mbxzztqtldjds * 100 / mbxzztqtlds, 1).ToString();
                    rows[3]["实际"] = hjxzztqtlds == 0 ? string.Empty : Math.Round(hjxzztqtldjds * 100 / hjxzztqtlds, 1).ToString();

                    rows[4] = tbl.NewRow();
                    rows[4]["关键指标"] = "成交率";
                    rows[4]["目标"] = mbscyyddkhzs == 0 ? string.Empty : Math.Round(mbcjzts * 100 / mbscyyddkhzs, 1).ToString();
                    rows[4]["实际"] = hjscyyddkhzs == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjscyyddkhzs, 1).ToString();

                    rows[5] = tbl.NewRow();
                    rows[5]["关键指标"] = "有效呼出率";
                    rows[5]["目标"] = mbhczl == 0 ? string.Empty : Math.Round(mbhcyxs * 100 / mbhczl, 1).ToString();
                    rows[5]["实际"] = hjhczl == 0 ? string.Empty : Math.Round(hjhcyxs * 100 / hjhczl, 1).ToString();

                    rows[6] = tbl.NewRow();
                    rows[6]["关键指标"] = "首次邀约到店率";
                    rows[6]["目标"] = mbxzdccxsjdl == 0 ? string.Empty : Math.Round(mbscyyddkhzs * 100 / mbxzdccxsjdl, 1).ToString();
                    rows[6]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : Math.Round(hjscyyddkhzs * 100 / hjxzdccxsjdl, 1).ToString();

                    rows[7] = tbl.NewRow();
                    rows[7]["关键指标"] = "再次邀约到店占比";
                    rows[7]["目标"] = (mbscyyddkhzs + mbzcyydds) == 0 ? string.Empty : Math.Round(mbzcyydds * 100 / (mbscyyddkhzs + mbzcyydds), 1).ToString();
                    rows[7]["实际"] = (hjscyyddkhzs + hjzcyydds) == 0 ? string.Empty : Math.Round(hjzcyydds * 100 / (hjscyyddkhzs + hjzcyydds), 1).ToString();

                    rows[8] = tbl.NewRow();
                    rows[8]["关键指标"] = "网络线索建档率";
                    rows[8]["目标"] = mbxzwlxszl == 0 ? string.Empty : Math.Round(mbxzwlxszldl * 100 / mbxzwlxszl, 1).ToString();
                    rows[8]["实际"] = hjxzwlxszl == 0 ? string.Empty : Math.Round(hjxzwlxszldl * 100 / hjxzwlxszl, 1).ToString();

                    rows[9] = tbl.NewRow();
                    rows[9]["关键指标"] = "线索转化率";
                    rows[9]["目标"] = mbxzwlxszl == 0 ? string.Empty : Math.Round(mbdccdds * 100 / mbxzdccxszl, 1).ToString();
                    rows[9]["实际"] = hjxzwlxszl == 0 ? string.Empty : Math.Round(hjdccdds * 100 / hjxzdccxszl, 1).ToString();

                    rows[10] = tbl.NewRow();
                    rows[10]["关键指标"] = "建档";
                    rows[10]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjxzdccxsjdl * 100 / hjxzdccxszl, 1).ToString();

                    rows[11] = tbl.NewRow();
                    rows[11]["关键指标"] = "重复数";
                    rows[11]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjwjdcfs * 100 / hjxzdccxszl, 1).ToString();

                    rows[12] = tbl.NewRow();
                    rows[12]["关键指标"] = "找售后";
                    rows[12]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjwjdzsh * 100 / hjxzdccxszl, 1).ToString();

                    rows[13] = tbl.NewRow();
                    rows[13]["关键指标"] = "外区域";
                    rows[13]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjwjdwqy * 100 / hjxzdccxszl, 1).ToString();

                    rows[14] = tbl.NewRow();
                    rows[14]["关键指标"] = "信息错误";
                    rows[14]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjwjdxxcw * 100 / hjxzdccxszl, 1).ToString();

                    rows[15] = tbl.NewRow();
                    rows[15]["关键指标"] = "二网经销商";
                    rows[15]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjwjdewjxs * 100 / hjxzdccxszl, 1).ToString();

                    rows[16] = tbl.NewRow();
                    rows[16]["关键指标"] = "找人";
                    rows[16]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjwjdzr * 100 / hjxzdccxszl, 1).ToString();

                    rows[17] = tbl.NewRow();
                    rows[17]["关键指标"] = "汽车之家";
                    rows[17]["实际"] = hjxzwlxszldl == 0 ? string.Empty : Math.Round(hjqczjlds * 100 / hjxzwlxszldl, 1).ToString();

                    rows[18] = tbl.NewRow();
                    rows[18]["关键指标"] = "易车网";
                    rows[18]["实际"] = hjxzwlxszldl == 0 ? string.Empty : Math.Round(hjycwlds * 100 / hjxzwlxszldl, 1).ToString();

                    rows[19] = tbl.NewRow();
                    rows[19]["关键指标"] = "太平洋";
                    rows[19]["实际"] = hjxzwlxszldl == 0 ? string.Empty : Math.Round(hjtpylds * 100 / hjxzwlxszldl, 1).ToString();

                    rows[20] = tbl.NewRow();
                    rows[20]["关键指标"] = "本地网络";
                    rows[20]["实际"] = hjxzwlxszldl == 0 ? string.Empty : Math.Round(hjbdwllds * 100 / hjxzwlxszldl, 1).ToString();

                    rows[21] = tbl.NewRow();
                    rows[21]["关键指标"] = "其他网络";
                    rows[21]["实际"] = hjxzwlxszldl == 0 ? string.Empty : Math.Round(hjqtwllds * 100 / hjxzwlxszldl, 1).ToString();

                    rows[22] = tbl.NewRow();
                    rows[22]["关键指标"] = "汽车之家成交数";
                    rows[22]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqczjcjs * 100 / hjcjzts, 1).ToString();

                    rows[23] = tbl.NewRow();
                    rows[23]["关键指标"] = "易车网成交数";
                    rows[23]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjycwcjs * 100 / hjcjzts, 1).ToString();

                    rows[24] = tbl.NewRow();
                    rows[24]["关键指标"] = "太平洋成交数";
                    rows[24]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjtpycjs * 100 / hjcjzts, 1).ToString();

                    rows[25] = tbl.NewRow();
                    rows[25]["关键指标"] = "其他网络成交数";
                    rows[25]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqtwlcjs * 100 / hjcjzts, 1).ToString();

                    rows[26] = tbl.NewRow();
                    rows[26]["关键指标"] = "转介绍成交数";
                    rows[26]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjzjscjs * 100 / hjcjzts, 1).ToString();

                    rows[27] = tbl.NewRow();
                    rows[27]["关键指标"] = "触点成交数";
                    rows[27]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjcdcjs * 100 / hjcjzts, 1).ToString();

                    rows[28] = tbl.NewRow();
                    rows[28]["关键指标"] = "CRM下发成交数";
                    rows[28]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjcrmxdcjs * 100 / hjcjzts, 1).ToString();

                    rows[29] = tbl.NewRow();
                    rows[29]["关键指标"] = "展厅转入成交数";
                    rows[29]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjztzrcjs * 100 / hjcjzts, 1).ToString();

                    rows[30] = tbl.NewRow();
                    rows[30]["关键指标"] = "上月留单数";
                    rows[30]["实际"] = ld;

                    rows[31] = tbl.NewRow();
                    rows[31]["关键指标"] = "本月留单数";
                    rows[31]["实际"] = hjdccdds - hjcjzts;

                    #endregion
                }
                else if (hdnKeyReportType.Value == "dcczhhz")
                {
                    #region 综合汇总

                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "DCC订单总台次";
                    rows[0]["目标"] = mbdccdds;
                    rows[0]["实际"] = hjdccdds == 0 ? string.Empty : hjdccdds.ToString();
                    rows[0]["详细"] = "目标值,合计";

                    rows[1] = tbl.NewRow();
                    rows[1]["关键指标"] = "DCC成交总台数";
                    rows[1]["目标"] = mbcjzts;
                    rows[1]["实际"] = hjcjzts == 0 ? string.Empty : hjcjzts.ToString();
                    rows[1]["详细"] = "目标值,合计";

                    rows[2] = tbl.NewRow();
                    rows[2]["关键指标"] = "展厅销量";
                    rows[2]["目标"] = mbztxl;
                    rows[2]["实际"] = hjztxl == 0 ? string.Empty : hjztxl.ToString();
                    rows[2]["详细"] = "目标值,合计";

                    rows[3] = tbl.NewRow();
                    rows[3]["关键指标"] = "销量占展厅比";
                    rows[3]["目标"] = mbztxl == 0 ? string.Empty : Math.Round(mbcjzts * 100 / mbztxl, 1).ToString();
                    rows[3]["实际"] = hjztxl == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjztxl, 1).ToString();

                    rows[4] = tbl.NewRow();
                    rows[4]["关键指标"] = "新增线索总数";
                    rows[4]["目标"] = mbxzdccxszl;
                    rows[4]["实际"] = hjxzdccxszl == 0 ? string.Empty : hjxzdccxszl.ToString();
                    rows[4]["详细"] = "目标值,合计";

                    rows[5] = tbl.NewRow();
                    rows[5]["关键指标"] = "建档总数";
                    rows[5]["目标"] = mbxzdccxsjdl;
                    rows[5]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : hjxzdccxsjdl.ToString();
                    rows[5]["详细"] = "目标值,合计";

                    rows[6] = tbl.NewRow();
                    rows[6]["关键指标"] = "建档率";
                    rows[6]["目标"] = mbxzdccxszl == 0 ? string.Empty : Math.Round(mbxzdccxsjdl * 100 / mbxzdccxszl, 1).ToString();
                    rows[6]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjxzdccxsjdl * 100 / hjxzdccxszl, 1).ToString();

                    rows[7] = tbl.NewRow();
                    rows[7]["关键指标"] = "首次邀约到店总数";
                    rows[7]["目标"] = mbscyyddkhzs;
                    rows[7]["实际"] = hjscyyddkhzs == 0 ? string.Empty : hjscyyddkhzs.ToString();
                    rows[7]["详细"] = "目标值,合计";

                    rows[8] = tbl.NewRow();
                    rows[8]["关键指标"] = "首次邀约到店率";
                    rows[8]["目标"] = mbxzdccxsjdl == 0 ? string.Empty : Math.Round(mbscyyddkhzs * 100 / mbxzdccxsjdl, 1).ToString();
                    rows[8]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : Math.Round(hjscyyddkhzs * 100 / hjxzdccxsjdl, 1).ToString();

                    rows[9] = tbl.NewRow();
                    rows[9]["关键指标"] = "再次邀约占比";
                    rows[9]["目标"] = (mbscyyddkhzs + mbzcyydds) == 0 ? string.Empty : Math.Round(mbzcyydds * 100 / (mbscyyddkhzs + mbzcyydds), 1).ToString();
                    rows[9]["实际"] = (hjscyyddkhzs + hjzcyydds) == 0 ? string.Empty : Math.Round(hjzcyydds * 100 / (hjscyyddkhzs + hjzcyydds), 1).ToString();

                    rows[10] = tbl.NewRow();
                    rows[10]["关键指标"] = "成交率";
                    rows[10]["目标"] = mbscyyddkhzs == 0 ? string.Empty : Math.Round(mbcjzts * 100 / mbscyyddkhzs, 1).ToString();
                    rows[10]["实际"] = hjscyyddkhzs == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjscyyddkhzs, 1).ToString();

                    rows[11] = tbl.NewRow();
                    rows[11]["关键指标"] = "订单转化率";
                    rows[11]["目标"] = mbxzwlxszl == 0 ? string.Empty : Math.Round(mbdccdds * 100 / mbxzdccxszl, 1).ToString();
                    rows[11]["实际"] = hjxzwlxszl == 0 ? string.Empty : Math.Round(hjdccdds * 100 / hjxzdccxszl, 1).ToString();

                    rows[12] = tbl.NewRow();
                    rows[12]["关键指标"] = "成交转化率";
                    rows[12]["目标"] = mbxzwlxszl == 0 ? string.Empty : Math.Round(mbcjzts * 100 / mbxzdccxszl, 1).ToString();
                    rows[12]["实际"] = hjxzwlxszl == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjxzdccxszl, 1).ToString();

                    #endregion
                }
                else if (hdnKeyReportType.Value == "dccqdzhl")
                {
                    #region 渠道汇总

                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "DCC成交总台次";
                    rows[0]["目标"] = mbcjzts;
                    rows[0]["实际"] = hjcjzts == 0 ? string.Empty : hjcjzts.ToString();

                    rows[1] = tbl.NewRow();
                    rows[1]["关键指标"] = "网络线索建档总量";
                    rows[1]["目标"] = mbxzwlxszldl;
                    rows[1]["实际"] = hjxzwlxszldl == 0 ? string.Empty : hjxzwlxszldl.ToString();

                    rows[2] = tbl.NewRow();
                    rows[2]["关键指标"] = "汽车之家线索量";
                    rows[2]["目标"] = mbqczjlds;
                    rows[2]["实际"] = hjqczjlds == 0 ? string.Empty : hjqczjlds.ToString();

                    rows[3] = tbl.NewRow();
                    rows[3]["关键指标"] = "汽车之家到店率";
                    rows[3]["目标"] = mbqczjlds == 0 ? string.Empty : Math.Round(mbqczjyydd * 100 / mbqczjlds, 1).ToString();
                    rows[3]["实际"] = hjqczjlds == 0 ? string.Empty : Math.Round(hjqczjyydd * 100 / hjqczjlds, 1).ToString();

                    rows[4] = tbl.NewRow();
                    rows[4]["关键指标"] = "汽车之家成交率";
                    rows[4]["目标"] = mbcjzts == 0 ? string.Empty : Math.Round(mbqczjcjs * 100 / mbcjzts, 1).ToString();
                    rows[4]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqczjcjs * 100 / hjcjzts, 1).ToString();

                    rows[5] = tbl.NewRow();
                    rows[5]["关键指标"] = "汽车之家成交量";
                    rows[5]["目标"] = mbqczjcjs;
                    rows[5]["实际"] = hjqczjcjs == 0 ? string.Empty : hjqczjcjs.ToString();

                    rows[6] = tbl.NewRow();
                    rows[6]["关键指标"] = "易车网线索量";
                    rows[6]["目标"] = mbycwlds;
                    rows[6]["实际"] = hjycwlds == 0 ? string.Empty : hjycwlds.ToString();

                    rows[7] = tbl.NewRow();
                    rows[7]["关键指标"] = "易车网到店率";
                    rows[7]["目标"] = mbycwlds == 0 ? string.Empty : Math.Round(mbycwyydd * 100 / mbycwlds, 1).ToString();
                    rows[7]["实际"] = hjycwlds == 0 ? string.Empty : Math.Round(hjycwyydd * 100 / hjycwlds, 1).ToString();

                    rows[8] = tbl.NewRow();
                    rows[8]["关键指标"] = "易车网成交率";
                    rows[8]["目标"] = mbcjzts == 0 ? string.Empty : Math.Round(mbycwcjs * 100 / mbcjzts, 1).ToString();
                    rows[8]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjycwcjs * 100 / hjcjzts, 1).ToString();

                    rows[9] = tbl.NewRow();
                    rows[9]["关键指标"] = "易车网成交量";
                    rows[9]["目标"] = mbycwcjs;
                    rows[9]["实际"] = hjycwcjs == 0 ? string.Empty : hjycwcjs.ToString();

                    rows[10] = tbl.NewRow();
                    rows[10]["关键指标"] = "太平洋汽车网线索量";
                    rows[10]["目标"] = mbtpylds;
                    rows[10]["实际"] = hjtpylds == 0 ? string.Empty : hjtpylds.ToString();

                    rows[11] = tbl.NewRow();
                    rows[11]["关键指标"] = "太平洋汽车网到店率";
                    rows[11]["目标"] = mbtpylds == 0 ? string.Empty : Math.Round(mbtpyyydd * 100 / mbtpylds, 1).ToString();
                    rows[11]["实际"] = hjtpylds == 0 ? string.Empty : Math.Round(hjtpyyydd * 100 / hjtpylds, 1).ToString();

                    rows[12] = tbl.NewRow();
                    rows[12]["关键指标"] = "太平洋汽车网成交率";
                    rows[12]["目标"] = mbcjzts == 0 ? string.Empty : Math.Round(mbtpycjs * 100 / mbcjzts, 1).ToString();
                    rows[12]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjtpycjs * 100 / hjcjzts, 1).ToString();

                    rows[13] = tbl.NewRow();
                    rows[13]["关键指标"] = "太平洋汽车网成交量";
                    rows[13]["目标"] = mbtpycjs;
                    rows[13]["实际"] = hjtpycjs == 0 ? string.Empty : hjtpycjs.ToString();

                    rows[14] = tbl.NewRow();
                    rows[14]["关键指标"] = "其他网络线索量";
                    rows[14]["目标"] = mbqtwllds;
                    rows[14]["实际"] = hjqtwllds == 0 ? string.Empty : hjqtwllds.ToString();

                    rows[15] = tbl.NewRow();
                    rows[15]["关键指标"] = "其他网络到店率";
                    rows[15]["目标"] = mbqtwllds == 0 ? string.Empty : Math.Round(mbqtwlyydd * 100 / mbqtwllds, 1).ToString();
                    rows[15]["实际"] = hjqtwllds == 0 ? string.Empty : Math.Round(hjqtwlyydd * 100 / hjqtwllds, 1).ToString();

                    rows[16] = tbl.NewRow();
                    rows[16]["关键指标"] = "其他网络成交率";
                    rows[16]["目标"] = mbcjzts == 0 ? string.Empty : Math.Round(mbqtwlcjs * 100 / mbcjzts, 1).ToString();
                    rows[16]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqtwlcjs * 100 / hjcjzts, 1).ToString();

                    rows[17] = tbl.NewRow();
                    rows[17]["关键指标"] = "其他网络成交量";
                    rows[17]["目标"] = mbqtwlcjs;
                    rows[17]["实际"] = hjqtwlcjs == 0 ? string.Empty : hjqtwlcjs.ToString();

                    rows[18] = tbl.NewRow();
                    rows[18]["关键指标"] = "转介绍线索量";
                    rows[18]["目标"] = mbzjsyss;
                    rows[18]["实际"] = hjzjsyss == 0 ? string.Empty : hjzjsyss.ToString();

                    rows[19] = tbl.NewRow();
                    rows[19]["关键指标"] = "转介绍邀约到店率";
                    rows[19]["目标"] = mbzjsyss == 0 ? string.Empty : Math.Round(mbzjsyydd * 100 / mbzjsyss, 1).ToString();
                    rows[19]["实际"] = hjzjsyss == 0 ? string.Empty : Math.Round(hjzjsyydd * 100 / hjzjsyss, 1).ToString();

                    rows[20] = tbl.NewRow();
                    rows[20]["关键指标"] = "转介绍成交率";
                    rows[20]["目标"] = mbcjzts == 0 ? string.Empty : Math.Round(mbzjscjs * 100 / mbcjzts, 1).ToString();
                    rows[20]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjzjscjs * 100 / hjcjzts, 1).ToString();

                    rows[21] = tbl.NewRow();
                    rows[21]["关键指标"] = "转介绍成交量";
                    rows[21]["目标"] = mbzjscjs;
                    rows[21]["实际"] = hjzjscjs == 0 ? string.Empty : hjzjscjs.ToString();

                    rows[22] = tbl.NewRow();
                    rows[22]["关键指标"] = "展厅转入客户数";
                    rows[22]["目标"] = mbztzrzky;
                    rows[22]["实际"] = hjztzrzky == 0 ? string.Empty : hjztzrzky.ToString();

                    rows[23] = tbl.NewRow();
                    rows[23]["关键指标"] = "展厅转入客户邀约到店率";
                    rows[23]["目标"] = mbztzrzky == 0 ? string.Empty : Math.Round(mbztzryydd * 100 / mbztzrzky, 1).ToString();
                    rows[23]["实际"] = hjztzrzky == 0 ? string.Empty : Math.Round(hjzjsyydd * 100 / hjztzrzky, 1).ToString();

                    rows[24] = tbl.NewRow();
                    rows[24]["关键指标"] = "展厅转入客户成交率";
                    rows[24]["目标"] = mbcjzts == 0 ? string.Empty : Math.Round(mbztzrcjs * 100 / mbcjzts, 1).ToString();
                    rows[24]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjztzrcjs * 100 / hjcjzts, 1).ToString();

                    rows[25] = tbl.NewRow();
                    rows[25]["关键指标"] = "展厅转入客户成交量";
                    rows[25]["目标"] = mbztzrcjs;
                    rows[25]["实际"] = hjztzrcjs == 0 ? string.Empty : hjztzrcjs.ToString();

                    rows[26] = tbl.NewRow();
                    rows[26]["关键指标"] = "触点成交数";
                    rows[26]["目标"] = mbcdcjs;
                    rows[26]["实际"] = hjztzrcjs == 0 ? string.Empty : hjztzrcjs.ToString();

                    rows[27] = tbl.NewRow();
                    rows[27]["关键指标"] = "CRm下达成交数";
                    rows[27]["目标"] = mbcrmxdcjs;
                    rows[27]["实际"] = hjcrmxdcjs == 0 ? string.Empty : hjcrmxdcjs.ToString();

                    #endregion
                }
                else if (hdnKeyReportType.Value == "dccjqdzb")
                {
                    #region 网络汇总

                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "DCC线索总量";
                    rows[0]["实际"] = hjxzdccxszl == 0 ? string.Empty : hjxzdccxszl.ToString();

                    rows[1] = tbl.NewRow();
                    rows[1]["关键指标"] = "网络线索目标";
                    rows[1]["实际"] = mbxzwlxszl == 0 ? string.Empty : mbxzwlxszl.ToString();

                    rows[2] = tbl.NewRow();
                    rows[2]["关键指标"] = "网络线索总量";
                    rows[2]["实际"] = hjxzwlxszl == 0 ? string.Empty : hjxzwlxszl.ToString();

                    rows[3] = tbl.NewRow();
                    rows[3]["关键指标"] = "网络线索达成率";
                    rows[3]["实际"] = mbxzwlxszl == 0 ? string.Empty : Math.Round(hjxzwlxszl * 100 / mbxzwlxszl, 1).ToString();

                    rows[4] = tbl.NewRow();
                    rows[4]["关键指标"] = "网络线索占比";
                    rows[4]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjxzwlxszl * 100 / hjxzdccxszl, 1).ToString();

                    rows[5] = tbl.NewRow();
                    rows[5]["关键指标"] = "DCC建档总量";
                    rows[5]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : hjxzdccxsjdl.ToString();

                    rows[6] = tbl.NewRow();
                    rows[6]["关键指标"] = "网络建档目标";
                    rows[6]["实际"] = mbxzwlxszldl == 0 ? string.Empty : mbxzwlxszldl.ToString();

                    rows[7] = tbl.NewRow();
                    rows[7]["关键指标"] = "网络建档总数";
                    rows[7]["实际"] = hjxzwlxszldl == 0 ? string.Empty : hjxzwlxszldl.ToString();

                    rows[8] = tbl.NewRow();
                    rows[8]["关键指标"] = "网络建档达成率";
                    rows[8]["实际"] = mbxzwlxszldl == 0 ? string.Empty : Math.Round(hjxzwlxszldl * 100 / mbxzwlxszldl, 1).ToString();

                    rows[9] = tbl.NewRow();
                    rows[9]["关键指标"] = "网络建档占比";
                    rows[9]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : Math.Round(hjxzwlxszldl * 100 / hjxzdccxsjdl, 1).ToString();

                    rows[10] = tbl.NewRow();
                    rows[10]["关键指标"] = "DCC建档率";
                    rows[10]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjxzdccxsjdl * 100 / hjxzdccxszl, 1).ToString();

                    rows[11] = tbl.NewRow();
                    rows[11]["关键指标"] = "网络目标建档率";
                    rows[11]["实际"] = mbxzwlxszl == 0 ? string.Empty : Math.Round(mbxzwlxszldl * 100 / mbxzwlxszl, 1).ToString();

                    rows[12] = tbl.NewRow();
                    rows[12]["关键指标"] = "网络建档率";
                    rows[12]["实际"] = hjxzwlxszl == 0 ? string.Empty : Math.Round(hjxzwlxszldl * 100 / hjxzwlxszl, 1).ToString();

                    rows[13] = tbl.NewRow();
                    rows[13]["关键指标"] = "DCC到店量";
                    rows[13]["实际"] = (hjscyyddkhzs + hjzcyydds) == 0 ? string.Empty : (hjscyyddkhzs + hjzcyydds).ToString();

                    rows[14] = tbl.NewRow();
                    rows[14]["关键指标"] = "网络到店目标";
                    rows[14]["实际"] = (mbqczjyydd + mbycwyydd + mbtpyyydd + mbqtwlyydd) == 0 ? string.Empty : (mbqczjyydd + mbycwyydd + mbtpyyydd + mbqtwlyydd).ToString();

                    rows[15] = tbl.NewRow();
                    rows[15]["关键指标"] = "网络到店总数";
                    rows[15]["实际"] = (hjqczjyydd + hjycwyydd + hjtpyyydd + hjqtwlyydd) == 0 ? string.Empty : (hjqczjyydd + hjycwyydd + hjtpyyydd + hjqtwlyydd).ToString();

                    rows[16] = tbl.NewRow();
                    rows[16]["关键指标"] = "网络到店达成率";
                    rows[16]["实际"] = (mbqczjyydd + mbycwyydd + mbtpyyydd + mbqtwlyydd) == 0 ? string.Empty : Math.Round((hjqczjyydd + hjycwyydd + hjtpyyydd + hjqtwlyydd) * 100 / (mbqczjyydd + mbycwyydd + mbtpyyydd + mbqtwlyydd), 1).ToString();

                    rows[17] = tbl.NewRow();
                    rows[17]["关键指标"] = "网络到店占比";
                    rows[17]["实际"] = (hjscyyddkhzs + hjzcyydds) == 0 ? string.Empty : Math.Round((hjqczjyydd + hjycwyydd + hjtpyyydd + hjqtwlyydd) * 100 / (hjscyyddkhzs + hjzcyydds), 1).ToString();

                    rows[18] = tbl.NewRow();
                    rows[18]["关键指标"] = "DCC到店率";
                    rows[18]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : Math.Round((hjscyyddkhzs + hjzcyydds) * 100 / hjxzdccxsjdl, 1).ToString();

                    rows[19] = tbl.NewRow();
                    rows[19]["关键指标"] = "网络到店率目标";
                    rows[19]["实际"] = (mbscyyddkhzs + mbzcyydds) == 0 ? string.Empty : Math.Round((mbqczjyydd + mbycwyydd + mbtpyyydd + mbqtwlyydd) * 100 / (mbscyyddkhzs + mbzcyydds), 1).ToString();

                    rows[20] = tbl.NewRow();
                    rows[20]["关键指标"] = "网络到店率";
                    rows[20]["实际"] = (hjscyyddkhzs + hjzcyydds) == 0 ? string.Empty : Math.Round((hjqczjyydd + hjycwyydd + hjtpyyydd + hjqtwlyydd) * 100 / (hjscyyddkhzs + hjzcyydds), 1).ToString();

                    rows[21] = tbl.NewRow();
                    rows[21]["关键指标"] = "DCC成交量";
                    rows[21]["实际"] = hjcjzts == 0 ? string.Empty : hjcjzts.ToString();

                    rows[22] = tbl.NewRow();
                    rows[22]["关键指标"] = "网络成交目标";
                    rows[22]["实际"] = (mbqczjcjs + mbycwcjs + mbtpycjs + mbqtwlcjs) == 0 ? string.Empty : (mbqczjcjs + mbycwcjs + mbtpycjs + mbqtwlcjs).ToString();

                    rows[23] = tbl.NewRow();
                    rows[23]["关键指标"] = "网络成交总数";
                    rows[23]["实际"] = (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) == 0 ? string.Empty : (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs).ToString();

                    rows[24] = tbl.NewRow();
                    rows[24]["关键指标"] = "网络成交达成率";
                    rows[24]["实际"] = (mbqczjcjs + mbycwcjs + mbtpycjs + mbqtwlcjs) == 0 ? string.Empty : Math.Round((hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) * 100 / (mbqczjcjs + mbycwcjs + mbtpycjs + mbqtwlcjs), 1).ToString();

                    rows[25] = tbl.NewRow();
                    rows[25]["关键指标"] = "网络占DCC成交比";
                    rows[25]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round((hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) * 100 / hjcjzts, 1).ToString();

                    rows[26] = tbl.NewRow();
                    rows[26]["关键指标"] = "DCC成交率";
                    rows[26]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjxzdccxsjdl, 1).ToString();

                    rows[27] = tbl.NewRow();
                    rows[27]["关键指标"] = "网络成交率目标";
                    rows[27]["实际"] = (mbqczjlds + mbycwlds + mbtpylds + mbqtwlcjs) == 0 ? string.Empty : Math.Round((mbqczjcjs + mbycwcjs + mbtpycjs + mbqtwlcjs) * 100 / (mbqczjlds + mbycwlds + mbtpylds + mbqtwlcjs), 1).ToString();

                    rows[28] = tbl.NewRow();
                    rows[28]["关键指标"] = "网络成交率";
                    rows[28]["实际"] = (hjqczjlds + hjycwlds + hjtpylds + hjqtwlcjs) == 0 ? string.Empty : Math.Round((hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) * 100 / (hjqczjlds + hjycwlds + hjtpylds + hjqtwlcjs), 1).ToString();

                    rows[29] = tbl.NewRow();
                    rows[29]["关键指标"] = "DCC转化率";
                    rows[29]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjxzdccxszl, 1).ToString();

                    rows[30] = tbl.NewRow();
                    rows[30]["关键指标"] = "网络转化率目标";
                    rows[30]["实际"] = mbxzwlxszl == 0 ? string.Empty : Math.Round((mbqczjcjs + mbycwcjs + mbtpycjs + mbqtwlcjs) * 100 / mbxzwlxszl, 1).ToString();

                    rows[31] = tbl.NewRow();
                    rows[31]["关键指标"] = "网络转化率";
                    rows[31]["实际"] = hjxzwlxszl == 0 ? string.Empty : Math.Round((hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) * 100 / hjxzwlxszl, 1).ToString();

                    rows[32] = tbl.NewRow();
                    rows[32]["关键指标"] = "汽车之家成交占比";
                    rows[32]["实际"] = (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) == 0 ? string.Empty : Math.Round(hjqczjcjs * 100 / (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs), 1).ToString();

                    rows[33] = tbl.NewRow();
                    rows[33]["关键指标"] = "易车网成交占比";
                    rows[33]["实际"] = (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) == 0 ? string.Empty : Math.Round(hjycwcjs * 100 / (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs), 1).ToString();

                    rows[34] = tbl.NewRow();
                    rows[34]["关键指标"] = "太平洋汽车网成交占比";
                    rows[34]["实际"] = (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) == 0 ? string.Empty : Math.Round(hjtpycjs * 100 / (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs), 1).ToString();

                    rows[35] = tbl.NewRow();
                    rows[35]["关键指标"] = "其他网络成交占比";
                    rows[35]["实际"] = (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs) == 0 ? string.Empty : Math.Round(hjqtwlcjs * 100 / (hjqczjcjs + hjycwcjs + hjtpycjs + hjqtwlcjs), 1).ToString();

                    #endregion
                }

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.二手车部)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 表数据

                #region 销售数据

                DateTime day = DataConvert.SafeDate(txtDate.Text);
                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);
                List<DailyReportModuleInfo> rlist_xs = DayReportModules.Instance.GetList(true);
                rlist_xs = rlist_xs.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_xs = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_xs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_xs[i].SCReport))
                    {
                        data_xs.Add(json.Deserialize<Dictionary<string, string>>(list_xs[i].SCReport));
                    }
                }

                Dictionary<string, string> targetdata_xs = new Dictionary<string, string>();
                if (monthtarget_xs != null && !string.IsNullOrEmpty(monthtarget_xs.SCReport))
                    targetdata_xs = json.Deserialize<Dictionary<string, string>>(monthtarget_xs.SCReport);

                decimal hjztjcts = 0;
                decimal mbztjcts = 0;
                decimal hjewxstc = 0;
                decimal mbewxstc = 0;
                decimal hjtppjctc = 0;
                decimal mbtppjctc = 0;

                CorporationInfo CurrentCorporation = Corporations.Instance.GetModel(corpid);
                DailyReportModuleInfo m;
                if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                {
                    m = rlist_xs.Find(l => l.Name == "他品牌交车台次");
                    hjtppjctc = m == null ? 0 : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0);
                    mbtppjctc = targetdata_xs.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(targetdata_xs[m.ID.ToString()]) : 0;
                }
                m = rlist_xs.Find(l => l.Name == "展厅交车台数");
                hjztjcts = m == null ? 0 : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0);
                mbztjcts = targetdata_xs.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(targetdata_xs[m.ID.ToString()]) : 0;
                m = rlist_xs.Find(l => l.Name == "二网销售台次");
                hjewxstc = m == null ? 0 : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0);
                mbewxstc = targetdata_xs.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(targetdata_xs[m.ID.ToString()]) : 0;

                decimal hjzxstc = hjztjcts + hjewxstc + hjtppjctc;
                decimal mbzxstc = mbztjcts + hjewxstc + hjtppjctc;
                #endregion

                DataRow[] rows = new DataRow[13];

                data.DefaultView.RowFilter = "项目='新车展厅销售量'";
                decimal hjxcztxsl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxcztxsl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='销售推荐评估台次'";
                decimal hjxstjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxstjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='实际评估台次'";
                decimal hjxssjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxssjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后进厂台次'";
                decimal hjshjctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbshjctc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后推荐评估台次'";
                decimal hjshtjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='实际评估台次'";
                decimal hjshsjpgtc = DataConvert.SafeDecimal(data.DefaultView[1]["合计"]);
                decimal mbshsjpgtc = DataConvert.SafeDecimal(data.DefaultView[1]["目标值"]);
                data.DefaultView.RowFilter = "项目='实际评估台次'";
                decimal hjqtqdsjpgtc = DataConvert.SafeDecimal(data.DefaultView[2]["合计"]);
                decimal mbqtqdsjpgtc = DataConvert.SafeDecimal(data.DefaultView[2]["目标值"]);
                data.DefaultView.RowFilter = "项目='销售台次'";
                decimal hjxstc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxstc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='销售毛利'";
                decimal hjxsml = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxsml = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='纯收购台次'";
                decimal hjsgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsgtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='置换台次'";
                decimal hjzhtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbzhtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='首次来电批次'";
                decimal hjscldpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbscldpc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='首次来店批次'";
                decimal hjscldianpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbscldianpc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='库存数'";
                decimal hjkcs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='在库超30天车辆'";
                decimal hjzkc30tcl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "销售有效推荐率";
                rows[0]["目标"] = mbxcztxsl == 0 ? string.Empty : Math.Round(mbxssjpgtc * 100 / mbxcztxsl, 1).ToString();
                rows[0]["实际"] = hjxcztxsl == 0 ? string.Empty : Math.Round(hjxssjpgtc * 100 / hjxcztxsl, 1).ToString();

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "售后有效推荐率";
                rows[1]["目标"] = mbshjctc == 0 ? string.Empty : Math.Round(mbshsjpgtc * 100 / mbshjctc, 1).ToString();
                rows[1]["实际"] = hjshjctc == 0 ? string.Empty : Math.Round(hjshsjpgtc * 100 / hjshjctc, 1).ToString();

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "总评估成交率";
                rows[2]["目标"] = (mbxssjpgtc + mbshsjpgtc + mbqtqdsjpgtc) == 0 ? string.Empty : Math.Round((mbsgtc + mbzhtc) * 100 / (mbxssjpgtc + mbshsjpgtc + mbqtqdsjpgtc), 1).ToString();
                rows[2]["实际"] = (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc) == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc), 1).ToString();

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "总销售成交率";
                rows[3]["目标"] = (mbscldianpc + mbscldpc) == 0 ? string.Empty : Math.Round(mbxstc * 100 / (mbscldianpc + mbscldpc), 1).ToString();
                rows[3]["实际"] = (hjscldianpc + hjscldpc) == 0 ? string.Empty : Math.Round(hjxstc * 100 / (hjscldianpc + hjscldpc), 1).ToString();

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "展厅置换率";
                rows[4]["目标"] = mbxcztxsl == 0 ? string.Empty : Math.Round((mbsgtc + mbzhtc) * 100 / mbxcztxsl, 1).ToString();
                rows[4]["实际"] = hjxcztxsl == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / hjxcztxsl, 1).ToString();

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "总置换率";
                rows[5]["目标"] = mbzxstc == 0 ? string.Empty : Math.Round((mbsgtc + mbzhtc) * 100 / mbzxstc, 1).ToString();
                rows[5]["实际"] = hjzxstc == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / hjzxstc, 1).ToString();

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "总有效评估量";
                rows[6]["目标"] = mbxssjpgtc + mbshsjpgtc + mbqtqdsjpgtc;
                rows[6]["实际"] = hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc;

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "总收购量";
                rows[7]["目标"] = mbsgtc + mbzhtc;
                rows[7]["实际"] = hjsgtc + hjzhtc;

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "总销售量";
                rows[8]["目标"] = mbxstc;
                rows[8]["实际"] = hjxstc;

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "总毛利";
                rows[9]["目标"] = mbxsml;
                rows[9]["实际"] = hjxsml;

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "平均单台毛利";
                rows[10]["目标"] = mbxstc == 0 ? string.Empty : Math.Round(mbxsml / mbxstc, 1).ToString();
                rows[10]["实际"] = hjxstc == 0 ? string.Empty : Math.Round(hjxsml / hjxstc, 1).ToString();

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "库存";
                rows[11]["实际"] = hjkcs;

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "在库超30天";
                rows[12]["实际"] = hjzkc30tcl;

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.金融部)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[43];

                DateTime day = DateTime.Today;
                if (corpid > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    List<DailyReportModuleInfo> rlist = DayReportModules.Instance.GetList(true);
                    rlist = rlist.FindAll(l => l.Department == DayReportDep.售后部).OrderBy(l => l.Sort).ToList();
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(corpid),
                        DayReportDep = DayReportDep.售后部
                    };
                    List<DailyReportInfo> listSHB = DailyReports.Instance.GetList(query, true);
                    listSHB = listSHB.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    List<Dictionary<string, string>> dataSHB = new List<Dictionary<string, string>>();
                    for (int i = 0; i < listSHB.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(listSHB[i].SCReport))
                        {
                            dataSHB.Add(json.Deserialize<Dictionary<string, string>>(listSHB[i].SCReport));
                        }
                    }
                    string[] listbxgs = new string[] { "中保", "太保", "平安", "人寿", "大地", "中华联合", "浙商", "大众", "其他" };

                    decimal hjbycz = 0;
                    decimal hjxbbf = 0;
                    decimal hjxubbf = 0;
                    decimal hjewbf = 0;
                    foreach (string s in listbxgs)
                    {
                        int id = rlist.Exists(r => r.Name == (s == "中保" ? "中保理赔" : s)) ? rlist.Find(r => r.Name == (s == "中保" ? "中保理赔" : s)).ID : 0;
                        hjbycz += dataSHB.Sum(d => d.ContainsKey(id.ToString()) ? DataConvert.SafeDecimal(d[id.ToString()]) : 0);

                        data.DefaultView.RowFilter = "项目='" + s + "（新保产值）'";
                        hjxbbf += DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                        data.DefaultView.RowFilter = "项目='" + s + "（续保产值）'";
                        hjxubbf += DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                        data.DefaultView.RowFilter = "项目='" + s + "（二网产值）'";
                        hjewbf += DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                    }

                    for (int i = 0; i < listbxgs.Length; i++)
                    {
                        int id = rlist.Exists(r => r.Name == (listbxgs[i] == "中保" ? "中保理赔" : listbxgs[i])) ? rlist.Find(r => r.Name == (listbxgs[i] == "中保" ? "中保理赔" : listbxgs[i])).ID : 0;
                        decimal hjby = dataSHB.Sum(d => d.ContainsKey(id.ToString()) ? DataConvert.SafeDecimal(d[id.ToString()]) : 0);
                        rows[i] = tbl.NewRow();
                        rows[i]["目标"] = hjby == 0 ? string.Empty : hjby.ToString();
                        rows[i]["实际"] = hjbycz == 0 ? string.Empty : (Math.Round(hjby * 100 / hjbycz, 1).ToString() + "%");

                        data.DefaultView.RowFilter = "项目='" + listbxgs[i] + "（新保产值）'";
                        decimal hjxb = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                        rows[i + 9] = tbl.NewRow();
                        rows[i + 9]["目标"] = hjxb == 0 ? string.Empty : hjxb.ToString();
                        rows[i + 9]["实际"] = hjxbbf == 0 ? string.Empty : (Math.Round(hjxb * 100 / hjxbbf, 1).ToString() + "%");

                        data.DefaultView.RowFilter = "项目='" + listbxgs[i] + "（续保产值）'";
                        decimal hjxub = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                        rows[i + 18] = tbl.NewRow();
                        rows[i + 18]["目标"] = hjxub == 0 ? string.Empty : hjxub.ToString();
                        rows[i + 18]["实际"] = hjxubbf == 0 ? string.Empty : (Math.Round(hjxub * 100 / hjxubbf, 1).ToString() + "%");

                        data.DefaultView.RowFilter = "项目='" + listbxgs[i] + "（二网产值）'";
                        decimal hjew = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                        rows[i + 27] = tbl.NewRow();
                        rows[i + 27]["目标"] = hjew == 0 ? string.Empty : hjew.ToString();
                        rows[i + 27]["实际"] = hjxbbf == 0 ? string.Empty : (Math.Round(hjew * 100 / hjxbbf, 1).ToString() + "%");
                    }

                    data.DefaultView.RowFilter = "项目='一年客户续保数'";
                    rows[36] = tbl.NewRow();
                    rows[36]["实际"] = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                    data.DefaultView.RowFilter = "项目='二年客户续保数'";
                    rows[37] = tbl.NewRow();
                    rows[37]["实际"] = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                    data.DefaultView.RowFilter = "项目='三年及以上客户续保数'";
                    rows[38] = tbl.NewRow();
                    rows[38]["实际"] = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                    rows[39] = tbl.NewRow();
                    rows[39]["实际"] = DataConvert.SafeDecimal(rows[36]["实际"]) + DataConvert.SafeDecimal(rows[37]["实际"]) + DataConvert.SafeDecimal(rows[38]["实际"]);

                    data.DefaultView.RowFilter = "项目='传统保险总数'";
                    rows[40] = tbl.NewRow();
                    rows[40]["实际"] = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                    data.DefaultView.RowFilter = "项目='按揭续保数'";
                    rows[41] = tbl.NewRow();
                    rows[41]["实际"] = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                    data.DefaultView.RowFilter = "项目='电销保险总数'";
                    rows[42] = tbl.NewRow();
                    rows[42]["实际"] = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                }
                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.无忧产品)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[34];
                decimal hjztjcts = 0, mbztjcts = 0;
                decimal hjzthdccbxtc = 0, mbzthdccbxtc = 0;
                decimal hjlctc = 0, mblctc = 0, hjxbs = 0, mbxbs = 0;
                DateTime day = DateTime.Today;

                #region 销售部数据

                if (corpid > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_xs = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corpid,
                        DayReportDep = DayReportDep.销售部
                    };
                    query_xs.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                    MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);

                    int days = 0;
                    DataTable tbl_xs = GetReport(DayReportDep.销售部, list_xs, monthtarget_xs, day, corpid, ref days);

                    tbl_xs.DefaultView.RowFilter = "项目='展厅交车台数'";
                    hjztjcts = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    mbztjcts = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='展厅含DCC保险台次'";
                    hjzthdccbxtc = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    mbzthdccbxtc = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);

                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "新车展厅销量";
                    rows[0]["目标"] = mbztjcts;
                    rows[0]["实际"] = hjztjcts;
                    tbl_xs.DefaultView.RowFilter = "项目='展厅含DCC保险台次'";
                    hjzthdccbxtc = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    mbzthdccbxtc = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='机油套餐购买个数'";
                    decimal hjxcjytcgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    decimal mbxcjytcgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='机油套餐购买金额'";
                    decimal hjxcjytcgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]) / 10000;
                    decimal mbxcjytcgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]) / 10000;
                    tbl_xs.DefaultView.RowFilter = "项目='玻璃无忧服务购买个数'";
                    decimal hjxcblwyfwgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    decimal mbxcblwyfwgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='玻璃无忧服务购买金额'";
                    decimal hjxcblwyfwgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]) / 10000;
                    decimal mbxcblwyfwgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]) / 10000;
                    tbl_xs.DefaultView.RowFilter = "项目='划痕无忧服务购买个数'";
                    decimal hjxchhwyfwgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    decimal mbxchhwyfwgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='划痕无忧服务购买金额'";
                    decimal hjxchhwyfwgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]) / 10000;
                    decimal mbxchhwyfwgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]) / 10000;
                    tbl_xs.DefaultView.RowFilter = "项目='延保无忧车服务购买个数'";
                    decimal hjxcybfwgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    decimal mbxcybfwgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='延保无忧车服务购买金额'";
                    decimal hjxcybfwgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]) / 10000;
                    decimal mbxcybfwgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]) / 10000;
                    tbl_xs.DefaultView.RowFilter = "项目='自主延保无忧车服务购买个数'";
                    decimal hjxcybfwzzgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    decimal mbxcybfwzzgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='自主延保无忧车服务购买金额'";
                    decimal hjxcybfwzzgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]) / 10000;
                    decimal mbxcybfwzzgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]) / 10000;
                    tbl_xs.DefaultView.RowFilter = "项目='厂家延保无忧车服务购买个数'";
                    decimal hjxcybfwcjgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    decimal mbxcybfwcjgmgs = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
                    tbl_xs.DefaultView.RowFilter = "项目='厂家延保无忧车服务购买金额'";
                    decimal hjxcybfwcjgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]) / 10000;
                    decimal mbxcybfwcjgmje = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]) / 10000;

                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "新车展厅销量";
                    rows[0]["目标"] = mbztjcts;
                    rows[0]["实际"] = hjztjcts;

                    //新车机油套餐
                    rows[1] = tbl.NewRow();
                    rows[1]["关键指标"] = "新车机油套餐购买个数";
                    rows[1]["目标"] = mbxcjytcgmgs;
                    rows[1]["实际"] = hjxcjytcgmgs;
                    rows[1]["详细"] = string.Empty;

                    rows[2] = tbl.NewRow();
                    rows[2]["关键指标"] = "新车机油套餐购买金额";
                    rows[2]["目标"] = mbxcjytcgmje;
                    rows[2]["实际"] = hjxcjytcgmje;

                    rows[3] = tbl.NewRow();
                    rows[3]["关键指标"] = "新车机油套餐渗透率";
                    rows[3]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbxcjytcgmgs * 100 / mbztjcts, 0).ToString();
                    rows[3]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjxcjytcgmgs * 100 / hjztjcts, 0).ToString();

                    //新车玻璃无忧
                    rows[4] = tbl.NewRow();
                    rows[4]["关键指标"] = "新车玻璃无忧服务购买个数";
                    rows[4]["目标"] = mbxcblwyfwgmgs;
                    rows[4]["实际"] = hjxcblwyfwgmgs;

                    rows[5] = tbl.NewRow();
                    rows[5]["关键指标"] = "新车玻璃无忧服务购买金额";
                    rows[5]["目标"] = mbxcblwyfwgmje;
                    rows[5]["实际"] = hjxcblwyfwgmje;

                    rows[6] = tbl.NewRow();
                    rows[6]["关键指标"] = "新车玻璃无忧服务渗透率";
                    rows[6]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbxcblwyfwgmgs * 100 / mbztjcts, 0).ToString();
                    rows[6]["实际"] = hjzthdccbxtc == 0 ? string.Empty : Math.Round(hjxcblwyfwgmgs * 100 / hjzthdccbxtc, 0).ToString();

                    //新车划痕无忧
                    rows[7] = tbl.NewRow();
                    rows[7]["关键指标"] = "新车划痕无忧服务购买个数";
                    rows[7]["目标"] = mbxchhwyfwgmgs;
                    rows[7]["实际"] = hjxchhwyfwgmgs;

                    rows[8] = tbl.NewRow();
                    rows[8]["关键指标"] = "新车划痕无忧服务购买金额";
                    rows[8]["目标"] = mbxchhwyfwgmje;
                    rows[8]["实际"] = hjxchhwyfwgmje;

                    rows[9] = tbl.NewRow();
                    rows[9]["关键指标"] = "新车划痕无忧服务渗透率";
                    rows[9]["目标"] = mbzthdccbxtc == 0 ? string.Empty : Math.Round(mbxchhwyfwgmgs * 100 / mbzthdccbxtc, 0).ToString();
                    rows[9]["实际"] = hjzthdccbxtc == 0 ? string.Empty : Math.Round(hjxchhwyfwgmgs * 100 / hjzthdccbxtc, 0).ToString();

                    //新车延保无忧
                    rows[10] = tbl.NewRow();
                    rows[10]["关键指标"] = "新车延保无忧车服务购买个数";
                    rows[10]["目标"] = mbxcybfwgmgs;
                    rows[10]["实际"] = hjxcybfwgmgs;

                    rows[11] = tbl.NewRow();
                    rows[11]["关键指标"] = "新车延保无忧车服务购买金额";
                    rows[11]["目标"] = mbxcybfwgmje;
                    rows[11]["实际"] = hjxcybfwgmje;

                    rows[12] = tbl.NewRow();
                    rows[12]["关键指标"] = "新车延保无忧车服务渗透率";
                    rows[12]["目标"] = mbzthdccbxtc == 0 ? string.Empty : Math.Round(mbxcybfwgmgs * 100 / mbzthdccbxtc, 0).ToString();
                    rows[12]["实际"] = hjzthdccbxtc == 0 ? string.Empty : Math.Round(hjxcybfwgmgs * 100 / hjzthdccbxtc, 0).ToString();

                    rows[13] = tbl.NewRow();
                    rows[13]["关键指标"] = "新车自主延保无忧车服务购买个数";
                    rows[13]["目标"] = mbxcybfwzzgmgs;
                    rows[13]["实际"] = hjxcybfwzzgmgs;

                    rows[14] = tbl.NewRow();
                    rows[14]["关键指标"] = "新车自主延保无忧车服务购买金额";
                    rows[14]["目标"] = mbxcybfwzzgmje;
                    rows[14]["实际"] = hjxcybfwzzgmje;

                    rows[15] = tbl.NewRow();
                    rows[15]["关键指标"] = "新车厂家延保无忧车服务购买个数";
                    rows[15]["目标"] = mbxcybfwcjgmgs;
                    rows[15]["实际"] = hjxcybfwcjgmgs;

                    rows[16] = tbl.NewRow();
                    rows[16]["关键指标"] = "新车厂家延保无忧车服务购买金额";
                    rows[16]["目标"] = mbxcybfwcjgmje;
                    rows[16]["实际"] = hjxcybfwcjgmje;
                }

                #endregion

                #region 售后数据

                if (corpid > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_sh = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corpid,
                        DayReportDep = DayReportDep.售后部
                    };
                    query_sh.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                    MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.售后部, day, true);

                    int days = 0;
                    DataTable tbl_sh = GetReport(DayReportDep.售后部, list_sh, monthtarget_sh, day, corpid, ref days);

                    tbl_sh.DefaultView.RowFilter = "项目='来厂台次'";
                    hjlctc = DataConvert.SafeDecimal(tbl_sh.DefaultView[0]["合计"]);
                    mblctc = DataConvert.SafeDecimal(tbl_sh.DefaultView[0]["目标值"]);
                    tbl_sh.DefaultView.RowFilter = "项目='续保数'";
                    hjxbs = DataConvert.SafeDecimal(tbl_sh.DefaultView[0]["合计"]);
                    mbxbs = DataConvert.SafeDecimal(tbl_sh.DefaultView[0]["目标值"]);
                }

                #endregion

                data.DefaultView.RowFilter = "项目='售后机油套餐购买个数'";
                decimal hjshjytcgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbshjytcgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后机油套餐购买金额'";
                decimal hjshjytcgmje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbshjytcgmje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后机油套餐原已购买客户数'";
                decimal hjshjytcyymgkhs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbshjytcyymgkhs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='当月来厂基盘车辆数≤18个月'";
                decimal hjdylcjpcls18m = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdylcjpcls18m = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='当月来厂基盘车辆数≤1年'";
                decimal hjdylcjpcls1y = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdylcjpcls1y = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='当月来厂基盘车辆数＞1年≤2年'";
                decimal hjdylcjpcls1y_2y = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdylcjpcls1y_2y = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='当月来厂基盘车辆数＞2年≤3年'";
                decimal hjdylcjpcls2y_3y = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdylcjpcls2y_3y = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjdylcjpcls3y = hjdylcjpcls1y + hjdylcjpcls1y_2y + hjdylcjpcls2y_3y;
                decimal mbdylcjpcls3y = mbdylcjpcls1y + mbdylcjpcls1y_2y + mbdylcjpcls2y_3y;
                data.DefaultView.RowFilter = "项目='续保玻璃无忧服务购买个数'";
                decimal hjxbblwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbblwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='续保玻璃无忧服务购买金额'";
                decimal hjxbblwyfwgmje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbblwyfwgmje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='续保划痕无忧服务购买个数'";
                decimal hjxbhhwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbhhwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='续保划痕无忧服务购买金额'";
                decimal hjxbhhwyfwgmje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbhhwyfwgmje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后延保服务购买个数'";
                decimal hjxbybwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbybwyfwgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后延保服务购买金额'";
                decimal hjxbybwyfwgmje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbybwyfwgmje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后延保服务自主购买个数'";
                decimal hjxbybwyfwzzgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbybwyfwzzgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后延保服务自主购买金额'";
                decimal hjxbybwyfwzzgmje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbybwyfwzzgmje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后延保服务厂家购买个数'";
                decimal hjxbybwyfwcjgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbybwyfwcjgmgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后延保服务厂家购买金额'";
                decimal hjxbybwyfwcjgmje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxbybwyfwcjgmje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                //售后來厂台次
                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "总来厂次";
                rows[17]["目标"] = mblctc;
                rows[17]["实际"] = hjlctc;

                //售后机油套餐
                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "售后机油套餐购买个数";
                rows[18]["目标"] = mbshjytcgmgs;
                rows[18]["实际"] = hjshjytcgmgs;

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "售后机油套餐购买金额";
                rows[19]["目标"] = mbshjytcgmje;
                rows[19]["实际"] = hjshjytcgmje;

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "售后机油套餐渗透率";
                rows[20]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshjytcstl)) ? monthtarget.NXCPshjytcstl : ((mbdylcjpcls3y - mbshjytcyymgkhs) == 0 ? string.Empty : Math.Round(mbshjytcgmgs * 100 / (mbdylcjpcls3y - mbshjytcyymgkhs), 0).ToString());
                rows[20]["实际"] = (hjdylcjpcls3y - hjshjytcyymgkhs) == 0 ? string.Empty : Math.Round(hjshjytcgmgs * 100 / (hjdylcjpcls3y - hjshjytcyymgkhs), 0).ToString();

                //续保玻璃无忧
                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "续保玻璃无忧服务购买个数";
                rows[21]["目标"] = mbxbblwyfwgmgs;
                rows[21]["实际"] = hjxbblwyfwgmgs;

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "续保玻璃无忧服务购买金额";
                rows[22]["目标"] = mbxbblwyfwgmje;
                rows[22]["实际"] = hjxbblwyfwgmje;

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "续保玻璃无忧服务渗透率";
                rows[23]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshblwyfwstl)) ? monthtarget.NXCPshblwyfwstl : (mbxbs == 0 ? string.Empty : Math.Round(mbxbblwyfwgmgs * 100 / mbxbs, 0).ToString());
                rows[23]["实际"] = hjxbs == 0 ? string.Empty : Math.Round(hjxbblwyfwgmgs * 100 / hjxbs, 0).ToString();

                //续保划痕无忧
                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "续保划痕无忧服务购买个数";
                rows[24]["目标"] = mbxbhhwyfwgmgs;
                rows[24]["实际"] = hjxbhhwyfwgmgs;

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "续保划痕无忧服务购买金额";
                rows[25]["目标"] = mbxbhhwyfwgmje;
                rows[25]["实际"] = hjxbhhwyfwgmje;

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "续保划痕无忧服务渗透率";
                rows[26]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshhhwyfwstl)) ? monthtarget.NXCPshhhwyfwstl : (mbxbs == 0 ? string.Empty : Math.Round(mbxbhhwyfwgmgs * 100 / mbxbs, 0).ToString());
                rows[26]["实际"] = hjxbs == 0 ? string.Empty : Math.Round(hjxbhhwyfwgmgs * 100 / hjxbs, 0).ToString();

                //延保无忧
                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "售后延保服务购买个数";
                rows[27]["目标"] = mbxbybwyfwgmgs;
                rows[27]["实际"] = hjxbybwyfwgmgs;

                rows[28] = tbl.NewRow();
                rows[28]["关键指标"] = "售后延保服务购买金额";
                rows[28]["目标"] = mbxbybwyfwgmje;
                rows[28]["实际"] = hjxbybwyfwgmje;

                rows[29] = tbl.NewRow();
                rows[29]["关键指标"] = "售后延保服务渗透率";
                rows[29]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshybwycfwstl)) ? monthtarget.NXCPshybwycfwstl : (mbdylcjpcls18m == 0 ? string.Empty : Math.Round(mbxbybwyfwgmgs * 100 / mbdylcjpcls18m, 0).ToString());
                rows[29]["实际"] = hjdylcjpcls18m == 0 ? string.Empty : Math.Round(hjxbybwyfwgmgs * 100 / hjdylcjpcls18m, 0).ToString();

                rows[30] = tbl.NewRow();
                rows[30]["关键指标"] = "售后延保服务自主购买个数";
                rows[30]["目标"] = mbxbybwyfwzzgmgs;
                rows[30]["实际"] = hjxbybwyfwzzgmgs;

                rows[31] = tbl.NewRow();
                rows[31]["关键指标"] = "售后延保服务自主购买金额";
                rows[31]["目标"] = mbxbybwyfwzzgmje;
                rows[31]["实际"] = hjxbybwyfwzzgmje;

                rows[32] = tbl.NewRow();
                rows[32]["关键指标"] = "售后延保服务厂家购买个数";
                rows[32]["目标"] = mbxbybwyfwcjgmgs;
                rows[32]["实际"] = hjxbybwyfwcjgmgs;

                rows[33] = tbl.NewRow();
                rows[33]["关键指标"] = "售后延保服务厂家购买金额";
                rows[33]["目标"] = mbxbybwyfwcjgmje;
                rows[33]["实际"] = hjxbybwyfwcjgmje;

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }
            else if (dep == DayReportDep.客服部)
            {
                #region 表结构

                tbl.Columns.Add("关键指标");
                tbl.Columns.Add("目标");
                tbl.Columns.Add("实际");
                tbl.Columns.Add("完成率");
                tbl.Columns.Add("详细");

                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[32];
                #region 销售数据

                DateTime day = DataConvert.SafeDate(txtDate.Text + "-01");
                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.销售部, day, true);
                List<DailyReportModuleInfo> rlist_xs = DayReportModules.Instance.GetList(true);
                rlist_xs = rlist_xs.FindAll(l => l.Department == DayReportDep.销售部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_xs = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_xs.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_xs[i].SCReport))
                    {
                        data_xs.Add(json.Deserialize<Dictionary<string, string>>(list_xs[i].SCReport));
                    }
                }

                Dictionary<string, string> targetdata_xs = new Dictionary<string, string>();
                if (monthtarget_xs != null && !string.IsNullOrEmpty(monthtarget_xs.SCReport))
                    targetdata_xs = json.Deserialize<Dictionary<string, string>>(monthtarget_xs.SCReport);

                decimal hjztxl = 0;
                decimal mbztxl = 0;
                DailyReportModuleInfo m;
                m = rlist_xs.Find(l => l.Name == "展厅交车台数");
                hjztxl = m == null ? 0 : Math.Round(data_xs.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0);
                mbztxl = targetdata_xs.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(targetdata_xs[m.ID.ToString()]) : 0;

                #endregion
                #region 售后数据

                DailyReportQuery query_sh = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = corpid,
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(corpid, DayReportDep.售后部, day, true);
                List<DailyReportModuleInfo> rlist_sh = DayReportModules.Instance.GetList(true);
                rlist_sh = rlist_sh.FindAll(l => l.Department == DayReportDep.售后部).OrderBy(l => l.Sort).ToList();
                List<Dictionary<string, string>> data_sh = new List<Dictionary<string, string>>();
                for (int i = 0; i < list_sh.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list_sh[i].SCReport))
                    {
                        data_sh.Add(json.Deserialize<Dictionary<string, string>>(list_sh[i].SCReport));
                    }
                }

                Dictionary<string, string> targetdata_sh = new Dictionary<string, string>();
                if (monthtarget_sh != null && !string.IsNullOrEmpty(monthtarget_sh.SCReport))
                    targetdata_sh = json.Deserialize<Dictionary<string, string>>(monthtarget_sh.SCReport);

                decimal hjyylctc = 0;
                decimal mbyylctc = 0;
                decimal hjshlctc = 0;
                decimal mbshlctc = 0;
                m = rlist_sh.Find(l => l.Name == "其中预约台次");
                hjyylctc = m == null ? 0 : Math.Round(data_sh.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0);
                mbyylctc = targetdata_sh.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(targetdata_sh[m.ID.ToString()]) : 0;

                m = rlist_sh.Find(l => l.Name == "来厂台次");
                hjshlctc = m == null ? 0 : Math.Round(data_sh.Sum(d => d.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(d[m.ID.ToString()]) : 0), 0);
                mbshlctc = targetdata_sh.ContainsKey(m.ID.ToString()) ? DataConvert.SafeDecimal(targetdata_sh[m.ID.ToString()]) : 0;

                #endregion


                data.DefaultView.RowFilter = "项目='销售回访成功数'";
                decimal hjxshfcgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxshfcgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后回访成功数'";
                decimal hjshhfcgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbshhfcgs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='售后回访数'";
                decimal hjshhfs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbshhfs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='不满意总数'";
                decimal hjxsbmyzs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxsbmyzs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='不满意总数'";
                decimal hjshbmyzs = DataConvert.SafeDecimal(data.DefaultView[1]["合计"]);
                decimal mbshbmyzs = DataConvert.SafeDecimal(data.DefaultView[1]["目标值"]);
                data.DefaultView.RowFilter = "项目='首保来店数'";
                decimal hjsblds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsblds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='首保到期提醒数'";
                decimal hjsbdqtxs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsbdqtxs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='定保客户来厂数'";
                decimal hjdbkhlcs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdbkhlcs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='定保招揽数'";
                decimal hjdbzls = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbdbzls = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='流失客户来店数'";
                decimal hjlskhlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mblskhlds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='流失招揽数'";
                decimal hjlszls = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mblszls = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='服务态度'";
                decimal hjxsfwtdbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='车辆质量'";
                decimal hjxsclzlbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='试乘试驾'";
                decimal hjxsscsjbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='交车仪式'";
                decimal hjxsjcysbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='收费价格'";
                decimal hjxssfjgbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='洗车'";
                decimal hjxsxcbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='收费价格'";
                decimal hjshsfjgbmy = DataConvert.SafeDecimal(data.DefaultView[1]["合计"]);
                data.DefaultView.RowFilter = "项目='维修品质'";
                decimal hjshwxpzbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='车辆品质'";
                decimal hjshclpzbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='配件待料'";
                decimal hjshpjdlbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='服务态度'";
                decimal hjshfwtdbmy = DataConvert.SafeDecimal(data.DefaultView[1]["合计"]);
                data.DefaultView.RowFilter = "项目='等待时间太长'";
                decimal hjshddsjtcbmy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='洗车'";
                decimal hjshxcbmy = DataConvert.SafeDecimal(data.DefaultView[1]["合计"]);
                data.DefaultView.RowFilter = "项目='建议'";
                decimal hjshjy = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "销售回访成功率";
                rows[0]["目标"] = mbztxl == 0 ? string.Empty : Math.Round(mbxshfcgs * 100 / mbztxl, 1).ToString();
                rows[0]["实际"] = hjztxl == 0 ? string.Empty : Math.Round(hjxshfcgs * 100 / hjztxl, 1).ToString();

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "售后回访成功率";
                rows[1]["目标"] = mbshhfs == 0 ? string.Empty : Math.Round(mbshhfcgs * 100 / mbshhfs, 1).ToString();
                rows[1]["实际"] = hjshhfs == 0 ? string.Empty : Math.Round(hjshhfcgs * 100 / hjshhfs, 1).ToString();

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "销售满意率";
                rows[2]["目标"] = mbxshfcgs == 0 ? string.Empty : Math.Round((mbxshfcgs - mbxsbmyzs) * 100 / mbxshfcgs, 1).ToString();
                rows[2]["实际"] = hjxshfcgs == 0 ? string.Empty : Math.Round((hjxshfcgs - hjxsbmyzs) * 100 / hjxshfcgs, 1).ToString();

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "售后满意率";
                rows[3]["目标"] = mbshhfcgs == 0 ? string.Empty : Math.Round((mbshhfcgs - mbshbmyzs) * 100 / mbshhfcgs, 1).ToString();
                rows[3]["实际"] = hjshhfcgs == 0 ? string.Empty : Math.Round((hjshhfcgs - hjshbmyzs) * 100 / hjshhfcgs, 1).ToString();

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "销售新车首保招揽回厂率";
                rows[4]["目标"] = mbsbdqtxs == 0 ? string.Empty : Math.Round(mbsblds * 100 / mbsbdqtxs, 1).ToString();
                rows[4]["实际"] = hjsbdqtxs == 0 ? string.Empty : Math.Round(hjsblds * 100 / hjsbdqtxs, 1).ToString();

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "售后定期保养招揽回厂率";
                rows[5]["目标"] = mbdbzls == 0 ? string.Empty : Math.Round(mbdbkhlcs * 100 / mbdbzls, 1).ToString();
                rows[5]["实际"] = hjdbzls == 0 ? string.Empty : Math.Round(hjdbkhlcs * 100 / hjdbzls, 1).ToString();

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "流失客户招揽回厂率";
                rows[6]["目标"] = mblszls == 0 ? string.Empty : Math.Round(mblskhlds * 100 / mblszls, 1).ToString();
                rows[6]["实际"] = hjlszls == 0 ? string.Empty : Math.Round(hjlskhlds * 100 / hjlszls, 1).ToString();

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "维修预约率";
                rows[7]["目标"] = mbshlctc == 0 ? string.Empty : Math.Round(mbyylctc * 100 / mbshlctc, 1).ToString();
                rows[7]["实际"] = hjshlctc == 0 ? string.Empty : Math.Round(hjyylctc * 100 / hjshlctc, 1).ToString();

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "销售不满意占比服务态度";
                rows[8]["实际"] = hjxsbmyzs == 0 ? string.Empty : Math.Round(hjxsfwtdbmy * 100 / hjxsbmyzs, 1).ToString();

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "销售不满意占比车辆质量";
                rows[9]["实际"] = hjxsbmyzs == 0 ? string.Empty : Math.Round(hjxsclzlbmy * 100 / hjxsbmyzs, 1).ToString();

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "销售不满意占比试乘试驾";
                rows[10]["实际"] = hjxsbmyzs == 0 ? string.Empty : Math.Round(hjxsscsjbmy * 100 / hjxsbmyzs, 1).ToString();

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "销售不满意占比交车仪式";
                rows[11]["实际"] = hjxsbmyzs == 0 ? string.Empty : Math.Round(hjxsjcysbmy * 100 / hjxsbmyzs, 1).ToString();

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "销售不满意占比收费价格";
                rows[12]["实际"] = hjxsbmyzs == 0 ? string.Empty : Math.Round(hjxssfjgbmy * 100 / hjxsbmyzs, 1).ToString();

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "销售不满意占比洗车";
                rows[13]["实际"] = hjxsbmyzs == 0 ? string.Empty : Math.Round(hjxsxcbmy * 100 / hjxsbmyzs, 1).ToString();

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "售后不满意占比收费价格";
                rows[14]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshsfjgbmy * 100 / hjshbmyzs, 1).ToString();

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "售后不满意占比维修品质";
                rows[15]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshwxpzbmy * 100 / hjshbmyzs, 1).ToString();

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "售后不满意占比车辆品质";
                rows[16]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshclpzbmy * 100 / hjshbmyzs, 1).ToString();

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "售后不满意占比配件待料";
                rows[17]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshpjdlbmy * 100 / hjshbmyzs, 1).ToString();

                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "售后不满意占比服务态度";
                rows[18]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshfwtdbmy * 100 / hjshbmyzs, 1).ToString();

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "售后不满意占比等待时间太长";
                rows[19]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshddsjtcbmy * 100 / hjshbmyzs, 1).ToString();

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "售后不满意占比洗车";
                rows[20]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshxcbmy * 100 / hjshbmyzs, 1).ToString();

                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "售后不满意占比建议";
                rows[21]["实际"] = hjshbmyzs == 0 ? string.Empty : Math.Round(hjshjy * 100 / hjshbmyzs, 1).ToString();

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "基盘保有客户量1年内";
                rows[22]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFjpbylhl1year.ToString();

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "基盘保有客户量2年内";
                rows[23]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFjpbylhl2year.ToString();

                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "基盘保有客户量3年内";
                rows[24]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFjpbylhl3year.ToString();

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "基盘保有客户量3-5年内";
                rows[25]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFjpbylhl3_5year.ToString();

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "基盘保有客户量5年以上";
                rows[26]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFjpbylhl5year.ToString();

                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "有效管理内客户量1年内";
                rows[27]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFyxglnkhl1year.ToString();

                rows[28] = tbl.NewRow();
                rows[28]["关键指标"] = "有效管理内客户量2年内";
                rows[28]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFyxglnkhl2year.ToString();

                rows[29] = tbl.NewRow();
                rows[29]["关键指标"] = "有效管理内客户量3年内";
                rows[29]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFyxglnkhl3year.ToString();

                rows[30] = tbl.NewRow();
                rows[30]["关键指标"] = "有效管理内客户量3-5年内";
                rows[30]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFyxglnkhl3_5year.ToString();

                rows[31] = tbl.NewRow();
                rows[31]["关键指标"] = "有效管理内客户量5年以上";
                rows[31]["实际"] = monthtarget == null ? string.Empty : monthtarget.KFyxglnkhl5year.ToString();

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["实际"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标"]), 0).ToString();
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
            }

            return tbl;
        }

        private DataTable GetReportCountMul(DayReportDep dep, DateTime day, DateTime day2)
        {
            DataTable tblresult = new DataTable();

            tblresult.Columns.Add("公司");
            tblresult.Columns.Add("录入");
            tblresult.Columns.Add("差值");
            tblresult.Columns.Add("差值合计");
            tblresult.Columns.Add("未及时填报");

            List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
            string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            corplist = corplist.FindAll(c => c.DailyreportShow == 1 && corppower.Contains(c.ID.ToString()));
            for (int i = 0; i < corplist.Count; i++)
            {
                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUniqueStart = day.ToString("yyyyMMdd"),
                    DayUniqueEnd = day2.ToString("yyyyMMdd"),
                    CorporationID = corplist[i].ID,
                    DayReportDep = CurrentDep
                };
                query.OrderBy = " [DayUnique] ASC";
                List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                DataRow row = tblresult.NewRow();
                row["公司"] = corplist[i].Name;
                row["录入"] = list.Count;
                row["差值"] = list.Count - day2.Subtract(day).Days - 1;
                int subcount = 0;
                int subcount1 = 0;
                for (int j = 0; j <= day2.Subtract(day).Days; j++)
                {
                    subcount += list.FindAll(l => l.CreateTime > day && l.CreateTime < day.AddDays(j + 1) && DataConvert.SafeInt(l.DayUnique) >= DataConvert.SafeInt(day.ToString("yyyyMMdd")) && DataConvert.SafeInt(l.DayUnique) <= DataConvert.SafeInt(day.AddDays(j + 1).ToString("yyyyMMdd"))).Count - j - 1;
                    subcount1 += list.FindAll(l => l.CreateTime > day.AddDays(j) && l.CreateTime < day.AddDays(j + 1) && DataConvert.SafeInt(l.DayUnique) >= DataConvert.SafeInt(day.AddDays(j).ToString("yyyyMMdd")) && DataConvert.SafeInt(l.DayUnique) <= DataConvert.SafeInt(day.AddDays(j + 1).ToString("yyyyMMdd"))).Count > 0 ? 0 : 1;
                }


                row["差值合计"] = subcount;
                row["未及时填报"] = subcount1;

                tblresult.Rows.Add(row);
            }

            return tblresult;
        }

        private string GetReportCountStr(DataTable tbl)
        {
            StringBuilder strb = new StringBuilder();
            tblView.Width = (120 + 120 * (tbl.Columns.Count - 1)) + "px";

            #region 页面输出
            Regex reg = new Regex(@"^[^\d]+\d+$");
            Regex regreplace = new Regex(@"\d+");
            strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
            strb.Append("<tr class=\"bold tc bggray\">");
            strb.Append("<td class=\"w120\">公司</td>");
            for (int i = 1; i < tbl.Columns.Count; i++)
            {
                strb.AppendFormat("<td class=\"w120\">{0}</td>", reg.IsMatch(tbl.Columns[i].ToString()) ? regreplace.Replace(tbl.Columns[i].ToString(), string.Empty) : tbl.Columns[i].ToString());
            }
            strb.Append("</tr>");

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                strb.Append("<tr class=\"tc\">");
                strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["公司"]);
                for (int j = 1; j < tbl.Columns.Count; j++)
                {
                    strb.AppendFormat("<td class=\"w80\">{0}</td>", tbl.Rows[i][tbl.Columns[j].ToString()].ToString());
                }
                strb.Append("</tr>");
            }

            strb.AppendLine("</table>");

            #endregion

            return strb.ToString();
        }

        private string CheckForm()
        {
            string result = string.Empty;
            DateTime day = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (!DateTime.TryParse(txtDate.Text, out day))
                result = "请选择起始日期";
            if (!DateTime.TryParse(txtDate2.Text, out day2))
                result = "请选择结束日期";
            if (day2.Month != day.Month)
                result = "不能跨月进行汇总";
            if (string.IsNullOrEmpty(hdnDayReportCorp.Value))
                result = "请至少选择一家公司";
            return result;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string checkstr = CheckForm();

            if (!string.IsNullOrEmpty(checkstr))
            {
                spMsg.InnerText = checkstr;
                return;
            }
            spMsg.InnerText = string.Empty;

            LoadData();
        }

        protected void btnKeyTarget_Click(object sender, EventArgs e)
        {
            string checkstr = CheckForm();

            if (!string.IsNullOrEmpty(checkstr))
            {
                spMsg.InnerText = checkstr;
                return;
            }
            spMsg.InnerText = string.Empty;
            if (CurrentDep == DayReportDep.行政部 || CurrentDep == DayReportDep.财务部 || CurrentDep == DayReportDep.精品部 || CurrentDep == DayReportDep.客服部 || CurrentDep == DayReportDep.金融部)
            {
                spMsg.InnerText = "此部门没有关键指标数据";
            }
            else
                LoadKeyData();
        }

        protected void btnReportCount_Click(object sender, EventArgs e)
        {
            string checkstr = CheckForm();

            if (!string.IsNullOrEmpty(checkstr))
            {
                spMsg.InnerText = checkstr;
                return;
            }
            spMsg.InnerText = string.Empty;

            LoadReportCountData();
        }

        protected void btnSCFollow_Click(object sender, EventArgs e)
        {
            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate2.Text, out day))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("店名");
                tblresult.Columns.Add("展厅成交率目标值");
                tblresult.Columns.Add("展厅成交率实际");
                tblresult.Columns.Add("展厅订单台数目标值");
                tblresult.Columns.Add("展厅订单台数合计");
                tblresult.Columns.Add("展厅订单台数完成率");
                tblresult.Columns.Add("展厅交车台数目标值");
                tblresult.Columns.Add("展厅交车台数合计");
                tblresult.Columns.Add("展厅交车台数完成率");
                tblresult.Columns.Add("其中DCC交车台次目标值");
                tblresult.Columns.Add("其中DCC交车台次合计");
                tblresult.Columns.Add("其中DCC交车台次完成率");
                tblresult.Columns.Add("展厅首次来客批次目标值");
                tblresult.Columns.Add("展厅首次来客批次合计");
                tblresult.Columns.Add("展厅首次来客批次完成率");
                tblresult.Columns.Add("留档批次目标值");
                tblresult.Columns.Add("留档批次合计");
                tblresult.Columns.Add("留档批次完成率");
                tblresult.Columns.Add("展厅首次到店记录数目标值");
                tblresult.Columns.Add("展厅首次到店记录数合计");
                tblresult.Columns.Add("展厅首次到店记录数完成率");
                tblresult.Columns.Add("展厅首次到店建档数目标值");
                tblresult.Columns.Add("展厅首次到店建档数合计");
                tblresult.Columns.Add("展厅首次到店建档数完成率");
                tblresult.Columns.Add("DCC邀约到店目标值");
                tblresult.Columns.Add("DCC邀约到店合计");
                tblresult.Columns.Add("DCC邀约到店完成率");
                tblresult.Columns.Add("新增DCC线索总量目标值");
                tblresult.Columns.Add("新增DCC线索总量合计");
                tblresult.Columns.Add("新增DCC线索总量完成率");
                tblresult.Columns.Add("新增DCC线索建档量目标值");
                tblresult.Columns.Add("新增DCC线索建档量合计");
                tblresult.Columns.Add("新增DCC线索建档量完成率");
                tblresult.Columns.Add("首次邀约到店客户总数目标值");
                tblresult.Columns.Add("首次邀约到店客户总数合计");
                tblresult.Columns.Add("首次邀约到店客户总数完成率");
                tblresult.Columns.Add("DCC订单数目标值");
                tblresult.Columns.Add("DCC订单数合计");
                tblresult.Columns.Add("DCC订单数完成率");
                tblresult.Columns.Add("DCC成交总台数目标值");
                tblresult.Columns.Add("DCC成交总台数合计");
                tblresult.Columns.Add("DCC成交总台数完成率");
                tblresult.Columns.Add("DCC建档率目标值");
                tblresult.Columns.Add("DCC建档率合计");
                tblresult.Columns.Add("DCC首次邀约到店率目标值");
                tblresult.Columns.Add("DCC首次邀约到店率合计");
                tblresult.Columns.Add("成交率目标值");
                tblresult.Columns.Add("成交率合计");
                tblresult.Columns.Add("订单转化率目标值");
                tblresult.Columns.Add("订单转化率合计");
                tblresult.Columns.Add("成交转化率目标值");
                tblresult.Columns.Add("成交转化率合计");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    #region 销售数据

                    DayReportDep dep = DayReportDep.销售部;
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    int days = 0;
                    DataTable tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);
                    DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);

                    row["店名"] = corplist[i].Name;
                    tblKey.DefaultView.RowFilter = "关键指标='展厅成交率'";
                    row["展厅成交率目标值"] = tblKey.DefaultView[0]["目标"];
                    row["展厅成交率实际"] = tblKey.DefaultView[0]["实际"];
                    tblDay.DefaultView.RowFilter = "项目='展厅订单台数'";
                    row["展厅订单台数目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅订单台数合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅订单台数完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                    row["展厅交车台数目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅交车台数合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅交车台数完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='其中DCC交车台次'";
                    row["其中DCC交车台次目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["其中DCC交车台次合计"] = tblDay.DefaultView[0]["合计"];
                    row["其中DCC交车台次完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='展厅首次来客批次'";
                    row["展厅首次来客批次目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅首次来客批次合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅首次来客批次完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='留档批次'";
                    row["留档批次目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["留档批次合计"] = tblDay.DefaultView[0]["合计"];
                    row["留档批次完成率"] = tblDay.DefaultView[0]["完成率"];

                    #endregion

                    #region 市场数据

                    dep = DayReportDep.市场部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    days = 0;
                    tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);

                    tblDay.DefaultView.RowFilter = "项目='展厅首次到店记录数'";
                    row["展厅首次到店记录数目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅首次到店记录数合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅首次到店记录数完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='展厅首次到店建档数'";
                    row["展厅首次到店建档数目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅首次到店建档数合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅首次到店建档数完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='DCC邀约到店'";
                    row["DCC邀约到店目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["DCC邀约到店合计"] = tblDay.DefaultView[0]["合计"];
                    row["DCC邀约到店完成率"] = tblDay.DefaultView[0]["完成率"];

                    #endregion

                    #region DCC数据

                    dep = DayReportDep.DCC部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    days = 0;
                    tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);
                    hdnKeyReportType.Value = "dcczhhz";
                    tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);

                    tblDay.DefaultView.RowFilter = "项目='新增DCC线索总量'";
                    row["新增DCC线索总量目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["新增DCC线索总量合计"] = tblDay.DefaultView[0]["合计"];
                    row["新增DCC线索总量完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='新增DCC线索建档量'";
                    row["新增DCC线索建档量目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["新增DCC线索建档量合计"] = tblDay.DefaultView[0]["合计"];
                    row["新增DCC线索建档量完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='首次邀约到店客户总数'";
                    row["首次邀约到店客户总数目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["首次邀约到店客户总数合计"] = tblDay.DefaultView[0]["合计"];
                    row["首次邀约到店客户总数完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='DCC订单数'";
                    row["DCC订单数目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["DCC订单数合计"] = tblDay.DefaultView[0]["合计"];
                    row["DCC订单数完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='DCC成交总台数'";
                    row["DCC成交总台数目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["DCC成交总台数合计"] = tblDay.DefaultView[0]["合计"];
                    row["DCC成交总台数完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='建档率'";
                    row["DCC建档率目标值"] = tblKey.DefaultView[0]["目标"];
                    row["DCC建档率合计"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='首次邀约到店率'";
                    row["DCC首次邀约到店率目标值"] = tblKey.DefaultView[0]["目标"];
                    row["DCC首次邀约到店率合计"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='成交率'";
                    row["成交率目标值"] = tblKey.DefaultView[0]["目标"];
                    row["成交率合计"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='订单转化率'";
                    row["订单转化率目标值"] = tblKey.DefaultView[0]["目标"];
                    row["订单转化率合计"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='成交转化率'";
                    row["成交转化率目标值"] = tblKey.DefaultView[0]["目标"];
                    row["成交转化率合计"] = tblKey.DefaultView[0]["实际"];

                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\销售客源、DCC线索周度进度跟进表模版.xls"));
                newfile = string.Format(@"销售客源、DCC线索周度进度跟进表{0}.xls", day.ToString("yyyyMM"));
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new HSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);
                double degree = Math.Round(double.Parse(day.Day.ToString()) / Utils.MonthDays(day), 2);
                double degreeint = degree * 100;
                sheet.GetRow(1).GetCell(1).SetCellValue(degree);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleRed = workbook.CreateCellStyle();
                cellStyleRed.SetFont(fontblack);
                cellStyleRed.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.TopBorderColor = HSSFColor.Black.Index;
                cellStyleRed.RightBorderColor = HSSFColor.Black.Index;
                cellStyleRed.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleRed.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleRed.FillForegroundColor = HSSFColor.Red.Index;
                cellStyleRed.FillPattern = FillPattern.SolidForeground;

                #endregion

                int index = 5;
                foreach (DataRow drow in tblresult.Rows)
                {
                    HSSFRow row = (HSSFRow)sheet.CreateRow(index);
                    row.CreateCell(0).SetCellValue(drow["店名"].ToString());
                    row.CreateCell(1).SetCellValue(string.Empty);
                    row.CreateCell(2).SetCellValue(string.IsNullOrEmpty(drow["展厅成交率目标值"].ToString()) ? string.Empty : (drow["展厅成交率目标值"].ToString() + "%"));
                    row.CreateCell(3).SetCellValue(string.IsNullOrEmpty(drow["展厅成交率实际"].ToString()) ? string.Empty : (drow["展厅成交率实际"].ToString() + "%"));
                    row.CreateCell(4).SetCellValue(drow["展厅订单台数目标值"].ToString());
                    row.CreateCell(5).SetCellValue(drow["展厅订单台数合计"].ToString());
                    row.CreateCell(6).SetCellValue(string.IsNullOrEmpty(drow["展厅订单台数完成率"].ToString()) ? string.Empty : (drow["展厅订单台数完成率"].ToString() + "%"));
                    row.CreateCell(7).SetCellValue(drow["展厅交车台数目标值"].ToString());
                    row.CreateCell(8).SetCellValue(drow["展厅交车台数合计"].ToString());
                    row.CreateCell(9).SetCellValue(string.IsNullOrEmpty(drow["展厅交车台数完成率"].ToString()) ? string.Empty : (drow["展厅交车台数完成率"].ToString() + "%"));
                    row.CreateCell(10).SetCellValue(drow["其中DCC交车台次目标值"].ToString());
                    row.CreateCell(11).SetCellValue(drow["其中DCC交车台次合计"].ToString());
                    row.CreateCell(12).SetCellValue(string.IsNullOrEmpty(drow["其中DCC交车台次完成率"].ToString()) ? string.Empty : (drow["其中DCC交车台次完成率"].ToString() + "%"));
                    row.CreateCell(13).SetCellValue(drow["展厅首次来客批次目标值"].ToString());
                    row.CreateCell(14).SetCellValue(drow["展厅首次来客批次合计"].ToString());
                    row.CreateCell(15).SetCellValue(string.IsNullOrEmpty(drow["展厅首次来客批次完成率"].ToString()) ? string.Empty : (drow["展厅首次来客批次完成率"].ToString() + "%"));
                    row.CreateCell(16).SetCellValue(drow["留档批次目标值"].ToString());
                    row.CreateCell(17).SetCellValue(drow["留档批次合计"].ToString());
                    row.CreateCell(18).SetCellValue(string.IsNullOrEmpty(drow["留档批次完成率"].ToString()) ? string.Empty : (drow["留档批次完成率"].ToString() + "%"));
                    row.CreateCell(19).SetCellValue(drow["展厅首次到店记录数目标值"].ToString());
                    row.CreateCell(20).SetCellValue(drow["展厅首次到店记录数合计"].ToString());
                    row.CreateCell(21).SetCellValue(string.IsNullOrEmpty(drow["展厅首次到店记录数完成率"].ToString()) ? string.Empty : (drow["展厅首次到店记录数完成率"].ToString() + "%"));
                    row.CreateCell(22).SetCellValue(drow["展厅首次到店建档数目标值"].ToString());
                    row.CreateCell(23).SetCellValue(drow["展厅首次到店建档数合计"].ToString());
                    row.CreateCell(24).SetCellValue(string.IsNullOrEmpty(drow["展厅首次到店建档数完成率"].ToString()) ? string.Empty : (drow["展厅首次到店建档数完成率"].ToString() + "%"));
                    row.CreateCell(25).SetCellValue(drow["DCC邀约到店目标值"].ToString());
                    row.CreateCell(26).SetCellValue(drow["DCC邀约到店合计"].ToString());
                    row.CreateCell(27).SetCellValue(string.IsNullOrEmpty(drow["DCC邀约到店完成率"].ToString()) ? string.Empty : (drow["DCC邀约到店完成率"].ToString() + "%"));
                    row.CreateCell(28).SetCellValue(drow["新增DCC线索总量目标值"].ToString());
                    row.CreateCell(29).SetCellValue(drow["新增DCC线索总量合计"].ToString());
                    row.CreateCell(30).SetCellValue(string.IsNullOrEmpty(drow["新增DCC线索总量完成率"].ToString()) ? string.Empty : (drow["新增DCC线索总量完成率"].ToString() + "%"));
                    row.CreateCell(31).SetCellValue(drow["新增DCC线索建档量目标值"].ToString());
                    row.CreateCell(32).SetCellValue(drow["新增DCC线索建档量合计"].ToString());
                    row.CreateCell(33).SetCellValue(string.IsNullOrEmpty(drow["新增DCC线索建档量完成率"].ToString()) ? string.Empty : (drow["新增DCC线索建档量完成率"].ToString() + "%"));
                    row.CreateCell(34).SetCellValue(drow["首次邀约到店客户总数目标值"].ToString());
                    row.CreateCell(35).SetCellValue(drow["首次邀约到店客户总数合计"].ToString());
                    row.CreateCell(36).SetCellValue(string.IsNullOrEmpty(drow["首次邀约到店客户总数完成率"].ToString()) ? string.Empty : (drow["首次邀约到店客户总数完成率"].ToString() + "%"));
                    row.CreateCell(37).SetCellValue(drow["DCC订单数目标值"].ToString());
                    row.CreateCell(38).SetCellValue(drow["DCC订单数合计"].ToString());
                    row.CreateCell(39).SetCellValue(string.IsNullOrEmpty(drow["DCC订单数完成率"].ToString()) ? string.Empty : (drow["DCC订单数完成率"].ToString() + "%"));
                    row.CreateCell(40).SetCellValue(drow["DCC成交总台数目标值"].ToString());
                    row.CreateCell(41).SetCellValue(drow["DCC成交总台数合计"].ToString());
                    row.CreateCell(42).SetCellValue(string.IsNullOrEmpty(drow["DCC成交总台数完成率"].ToString()) ? string.Empty : (drow["DCC成交总台数完成率"].ToString() + "%"));
                    row.CreateCell(43).SetCellValue(string.IsNullOrEmpty(drow["DCC建档率目标值"].ToString()) ? string.Empty : (drow["DCC建档率目标值"].ToString() + "%"));
                    row.CreateCell(44).SetCellValue(string.IsNullOrEmpty(drow["DCC建档率合计"].ToString()) ? string.Empty : (drow["DCC建档率合计"].ToString() + "%"));
                    row.CreateCell(45).SetCellValue(string.IsNullOrEmpty(drow["DCC首次邀约到店率目标值"].ToString()) ? string.Empty : (drow["DCC首次邀约到店率目标值"].ToString() + "%"));
                    row.CreateCell(46).SetCellValue(string.IsNullOrEmpty(drow["DCC首次邀约到店率合计"].ToString()) ? string.Empty : (drow["DCC首次邀约到店率合计"].ToString() + "%"));
                    row.CreateCell(47).SetCellValue(string.IsNullOrEmpty(drow["成交率目标值"].ToString()) ? string.Empty : (drow["成交率目标值"].ToString() + "%"));
                    row.CreateCell(48).SetCellValue(string.IsNullOrEmpty(drow["成交率合计"].ToString()) ? string.Empty : (drow["成交率合计"].ToString() + "%"));
                    row.CreateCell(49).SetCellValue(string.IsNullOrEmpty(drow["订单转化率目标值"].ToString()) ? string.Empty : (drow["订单转化率目标值"].ToString() + "%"));
                    row.CreateCell(50).SetCellValue(string.IsNullOrEmpty(drow["订单转化率合计"].ToString()) ? string.Empty : (drow["订单转化率合计"].ToString() + "%"));
                    row.CreateCell(51).SetCellValue(string.IsNullOrEmpty(drow["成交转化率目标值"].ToString()) ? string.Empty : (drow["成交转化率目标值"].ToString() + "%"));
                    row.CreateCell(52).SetCellValue(string.IsNullOrEmpty(drow["成交转化率合计"].ToString()) ? string.Empty : (drow["成交转化率合计"].ToString() + "%"));

                    for (int i = 0; i <= 1; i++)
                    {
                        sheet.GetRow(index).Cells[i].CellStyle = cellStyleBlack;
                    }
                    for (int i = 1; i <= 51; i++)
                    {
                        if (tblresult.Columns[i].ColumnName.IndexOf("完成率") >= 0 && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) >= degreeint)
                            sheet.GetRow(index).Cells[i + 1].CellStyle = cellStyleGreen;
                        else if (tblresult.Columns[i].ColumnName.IndexOf("完成率") >= 0 && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) >= ((degreeint < 5 ? 5 : degreeint) - 5) && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) < degreeint)
                            sheet.GetRow(index).Cells[i + 1].CellStyle = cellStyleYellow;
                        else if (tblresult.Columns[i].ColumnName.IndexOf("完成率") >= 0 && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) > 0 && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) < ((degreeint < 5 ? 5 : degreeint) - 5))
                            sheet.GetRow(index).Cells[i + 1].CellStyle = cellStyleRed;
                        else
                            sheet.GetRow(index).Cells[i + 1].CellStyle = cellStyleBlack;
                    }
                    index++;
                }
                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        protected void btnSCdzpxfa_Click(object sender, EventArgs e)
        {
            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate2.Text, out day))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("品牌");
                tblresult.Columns.Add("展厅销量完成率");
                tblresult.Columns.Add("来厂台次");
                tblresult.Columns.Add("售后产值完成率");
                tblresult.Columns.Add("展厅新增留档潜客到店");
                tblresult.Columns.Add("展厅成交率");
                tblresult.Columns.Add("展厅二次到店率");
                tblresult.Columns.Add("DCC建档线索完成率");
                tblresult.Columns.Add("DCC订单转换率");
                tblresult.Columns.Add("单车附加总产值");
                tblresult.Columns.Add("续保完成率");
                tblresult.Columns.Add("二手车评估数");
                tblresult.Columns.Add("二手车收购量");
                tblresult.Columns.Add("老客户转介绍占比");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    #region 销售数据

                    DayReportDep dep = DayReportDep.销售部;
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    int days = 0;
                    DataTable tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);
                    DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);

                    row["品牌"] = corplist[i].Name;
                    tblDay.DefaultView.RowFilter = "项目='展厅首次来客批次'";
                    decimal ztsclkpc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                    tblDay.DefaultView.RowFilter = "项目='再次来客批次'";
                    decimal zclkpc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);

                    row["展厅二次到店率"] = (zclkpc + ztsclkpc) == 0 ? string.Empty : Math.Round(zclkpc * 100 / (zclkpc + ztsclkpc), 2).ToString();
                    tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                    decimal ztjctsmb = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                    decimal ztjctshj = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                    row["展厅销量完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='展厅首次来客批次'";
                    row["展厅新增留档潜客到店"] = tblDay.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅成交率'";
                    row["展厅成交率"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='附加值合计'";
                    decimal fjzhjmb = DataConvert.SafeDecimal(tblKey.DefaultView[0]["目标"]);
                    decimal fjzhjsj = DataConvert.SafeDecimal(tblKey.DefaultView[0]["实际"]);
                    decimal fjzdtmb = ztjctsmb == 0 ? 0 : fjzhjmb / ztjctsmb; //附加值单台目标
                    decimal fjzdtsj = ztjctshj == 0 ? 0 : fjzhjsj / ztjctshj; //附加值单台实际
                    row["单车附加总产值"] = fjzdtmb == 0 ? string.Empty : Math.Round(fjzdtsj * 100 / fjzdtmb, 2).ToString();
                    tblDay.DefaultView.RowFilter = "项目='其中老客户转介绍交车台次'";
                    decimal lkhzjstc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                    tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                    decimal ztjctc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                    row["老客户转介绍占比"] = ztjctc == 0 ? string.Empty : Math.Round(lkhzjstc * 100 / ztjctc, 2).ToString();
                    tblDay.DefaultView.RowFilter = "项目='推荐二手车评估数'";
                    decimal tjescpgs = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);

                    #endregion

                    #region 售后数据

                    dep = DayReportDep.售后部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    days = 0;
                    tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);

                    tblDay.DefaultView.RowFilter = "项目='来厂台次'";
                    row["来厂台次"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='当日产值'";
                    row["售后产值完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='续保数'";
                    row["续保完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='介绍二手车评估数'";
                    decimal jsescpgs = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);

                    row["二手车评估数"] = (jsescpgs + tjescpgs).ToString();
                    #endregion

                    #region 二手车数据

                    dep = DayReportDep.二手车部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    days = 0;
                    tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);
                    tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);

                    tblKey.DefaultView.RowFilter = "关键指标='总收购量'";
                    row["二手车收购量"] = tblKey.DefaultView[0]["实际"];

                    #endregion

                    #region DCC数据

                    dep = DayReportDep.DCC部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    days = 0;
                    tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);
                    hdnKeyReportType.Value = "dcczhhz";
                    tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);

                    tblDay.DefaultView.RowFilter = "项目='新增DCC线索建档量'";
                    row["DCC建档线索完成率"] = tblDay.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='订单转化率'";
                    row["DCC订单转换率"] = tblKey.DefaultView[0]["实际"];

                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\总经理关键指标排名.xlsx"));
                newfile = string.Format(@"{0}总经理关键指标排名.xlsx", day.ToString("yyyy年MM月"));
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new XSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleRed = workbook.CreateCellStyle();
                cellStyleRed.SetFont(fontblack);
                cellStyleRed.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.TopBorderColor = HSSFColor.Black.Index;
                cellStyleRed.RightBorderColor = HSSFColor.Black.Index;
                cellStyleRed.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleRed.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleRed.FillForegroundColor = HSSFColor.Red.Index;
                cellStyleRed.FillPattern = FillPattern.SolidForeground;

                #endregion

                int index = 2;
                foreach (DataRow drow in tblresult.Rows)
                {
                    XSSFRow row = (XSSFRow)sheet.CreateRow(index);
                    row.CreateCell(0).SetCellValue(drow["品牌"].ToString());
                    row.CreateCell(1).SetCellValue(string.IsNullOrEmpty(drow["展厅销量完成率"].ToString()) ? string.Empty : (drow["展厅销量完成率"].ToString() + "%"));
                    row.CreateCell(2).SetCellValue(string.IsNullOrEmpty(drow["来厂台次"].ToString()) ? string.Empty : (drow["来厂台次"].ToString() + "%"));
                    row.CreateCell(3).SetCellValue(string.IsNullOrEmpty(drow["售后产值完成率"].ToString()) ? string.Empty : (drow["售后产值完成率"].ToString() + "%"));
                    row.CreateCell(4).SetCellValue(string.IsNullOrEmpty(drow["展厅新增留档潜客到店"].ToString()) ? string.Empty : (drow["展厅新增留档潜客到店"].ToString() + "%"));
                    row.CreateCell(5).SetCellValue(string.IsNullOrEmpty(drow["展厅成交率"].ToString()) ? string.Empty : (drow["展厅成交率"].ToString() + "%"));
                    row.CreateCell(6).SetCellValue(string.IsNullOrEmpty(drow["展厅二次到店率"].ToString()) ? string.Empty : (drow["展厅二次到店率"].ToString() + "%"));
                    row.CreateCell(7).SetCellValue(string.IsNullOrEmpty(drow["DCC建档线索完成率"].ToString()) ? string.Empty : (drow["DCC建档线索完成率"].ToString() + "%"));
                    row.CreateCell(8).SetCellValue(string.IsNullOrEmpty(drow["DCC订单转换率"].ToString()) ? string.Empty : (drow["DCC订单转换率"].ToString() + "%"));
                    row.CreateCell(9).SetCellValue(string.IsNullOrEmpty(drow["单车附加总产值"].ToString()) ? string.Empty : (drow["单车附加总产值"].ToString() + "%"));
                    row.CreateCell(10).SetCellValue(string.IsNullOrEmpty(drow["续保完成率"].ToString()) ? string.Empty : (drow["续保完成率"].ToString() + "%"));
                    row.CreateCell(11).SetCellValue(drow["二手车评估数"].ToString());
                    row.CreateCell(12).SetCellValue(drow["二手车收购量"].ToString());
                    row.CreateCell(13).SetCellValue(string.IsNullOrEmpty(drow["老客户转介绍占比"].ToString()) ? string.Empty : (drow["老客户转介绍占比"].ToString() + "%"));

                    for (int i = 0; i < tblresult.Columns.Count; i++)
                    {
                        sheet.GetRow(index).Cells[i].CellStyle = cellStyleBlack;
                    }

                    index++;
                }
                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        protected void btnDCCMonthly_Click(object sender, EventArgs e)
        {
            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate2.Text, out day))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("店名");
                tblresult.Columns.Add("当月新增潜客总量");
                tblresult.Columns.Add("当月新增潜客有效建档量");
                tblresult.Columns.Add("当月新增潜客渠道细分网络E接触");
                tblresult.Columns.Add("当月新增潜客渠道细分触点");
                tblresult.Columns.Add("当月新增潜客渠道细分展厅转入");
                tblresult.Columns.Add("当月新增潜客渠道细分转介绍");
                tblresult.Columns.Add("当月新增潜客渠道细分销售热线");
                tblresult.Columns.Add("当月新增潜客渠道细分其他");
                tblresult.Columns.Add("当月呼出总量");
                tblresult.Columns.Add("当月有效呼出总量");
                tblresult.Columns.Add("当月首次邀约到店量");
                tblresult.Columns.Add("当月再次邀约量");
                tblresult.Columns.Add("实际订单");
                tblresult.Columns.Add("实际交车量");
                tblresult.Columns.Add("公司展厅零售销量");
                tblresult.Columns.Add("当月公司总销量");
                tblresult.Columns.Add("当月网络订单渠道细分汽车之家");
                tblresult.Columns.Add("当月网络订单渠道细分易车网");
                tblresult.Columns.Add("当月网络订单渠道细分太平洋");
                tblresult.Columns.Add("当月网络订单渠道细分其他网络");
                tblresult.Columns.Add("当月网络订单渠道细分转介绍");
                tblresult.Columns.Add("当月网络订单渠道细分触点");
                tblresult.Columns.Add("当月网络订单渠道细分其他");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    #region 销售数据

                    DayReportDep dep = DayReportDep.销售部;
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    int days = 0;
                    DataTable tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);
                    DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);

                    row["店名"] = corplist[i].Name;
                    tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                    row["公司展厅零售销量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='二网销售台次'";
                    row["当月公司总销量"] = DataConvert.SafeInt(tblDay.DefaultView[0]["合计"]) + DataConvert.SafeInt(row["公司展厅零售销量"]);


                    #endregion

                    #region DCC数据

                    dep = DayReportDep.DCC部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    days = 0;
                    tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);

                    tblDay.DefaultView.RowFilter = "项目='新增DCC线索总量'";
                    row["当月新增潜客总量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='新增DCC线索建档量'";
                    row["当月新增潜客有效建档量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='新增网络线索总量'";
                    row["当月新增潜客渠道细分网络E接触"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='触点'";
                    row["当月新增潜客渠道细分触点"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='展厅当日转入'";
                    row["当月新增潜客渠道细分展厅转入"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='转介绍线索数'";
                    row["当月新增潜客渠道细分转介绍"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='新增展厅前台来电数'";
                    row["当月新增潜客渠道细分销售热线"] = tblDay.DefaultView[0]["合计"];
                    row["当月新增潜客渠道细分其他"] = "";
                    tblDay.DefaultView.RowFilter = "项目='呼出总量'";
                    row["当月呼出总量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='呼出有效数'";
                    row["当月有效呼出总量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='首次邀约到店客户总数'";
                    row["当月首次邀约到店量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='再次邀约到店数'";
                    row["当月再次邀约量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='DCC订单数'";
                    row["实际订单"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='DCC成交总台数'";
                    row["实际交车量"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='汽车之家订单数'";
                    row["当月网络订单渠道细分汽车之家"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='易车网订单数'";
                    row["当月网络订单渠道细分易车网"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='太平洋订单数'";
                    row["当月网络订单渠道细分太平洋"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='其他网络订单数'";
                    row["当月网络订单渠道细分其他网络"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='转介绍订单数'";
                    row["当月网络订单渠道细分转介绍"] = tblDay.DefaultView[0]["合计"];
                    tblDay.DefaultView.RowFilter = "项目='触点订单数'";
                    row["当月网络订单渠道细分触点"] = tblDay.DefaultView[0]["合计"];
                    row["当月网络订单渠道细分其他"] = "";

                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(@"\App_Data\DCC月报.xlsx");
                newfile = string.Format(@"{0}DCC月报汇总-{1}.xlsx", day.ToString("yyyy年M月"), DateTime.Now.ToString("M.dd"));
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new XSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleRed = workbook.CreateCellStyle();
                cellStyleRed.SetFont(fontblack);
                cellStyleRed.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleRed.TopBorderColor = HSSFColor.Black.Index;
                cellStyleRed.RightBorderColor = HSSFColor.Black.Index;
                cellStyleRed.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleRed.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleRed.FillForegroundColor = HSSFColor.Red.Index;
                cellStyleRed.FillPattern = FillPattern.SolidForeground;

                #endregion

                int index = 3;
                sheet.GetRow(0).GetCell(1).SetCellValue(day.ToString("yyyy年  M  月") + "DCC月报");
                foreach (DataRow drow in tblresult.Rows)
                {
                    if (index >= 22) continue;
                    XSSFRow row = (XSSFRow)sheet.GetRow(index);
                    row.GetCell(2).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客总量"].ToString()));
                    row.GetCell(3).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客有效建档量"].ToString()));
                    row.GetCell(6).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客渠道细分网络E接触"].ToString()));
                    row.GetCell(7).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客渠道细分触点"].ToString()));
                    row.GetCell(8).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客渠道细分展厅转入"].ToString()));
                    row.GetCell(9).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客渠道细分转介绍"].ToString()));
                    row.GetCell(10).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客渠道细分销售热线"].ToString()));
                    row.GetCell(11).SetCellValue(DataConvert.SafeDouble(drow["当月新增潜客渠道细分其他"].ToString()));
                    row.GetCell(12).SetCellValue(DataConvert.SafeDouble(drow["当月呼出总量"].ToString()));
                    row.GetCell(13).SetCellValue(DataConvert.SafeDouble(drow["当月有效呼出总量"].ToString()));
                    row.GetCell(14).SetCellValue(DataConvert.SafeDouble(drow["当月首次邀约到店量"].ToString()));
                    row.GetCell(16).SetCellValue(DataConvert.SafeDouble(drow["当月再次邀约量"].ToString()));
                    row.GetCell(18).SetCellValue(DataConvert.SafeDouble(drow["实际订单"].ToString()));
                    row.GetCell(19).SetCellValue(DataConvert.SafeDouble(drow["实际交车量"].ToString()));
                    row.GetCell(21).SetCellValue(DataConvert.SafeDouble(drow["公司展厅零售销量"].ToString()));
                    row.GetCell(23).SetCellValue(DataConvert.SafeDouble(drow["当月公司总销量"].ToString()));
                    row.GetCell(25).SetCellValue(DataConvert.SafeDouble(drow["当月网络订单渠道细分汽车之家"].ToString()));
                    row.GetCell(26).SetCellValue(DataConvert.SafeDouble(drow["当月网络订单渠道细分易车网"].ToString()));
                    row.GetCell(27).SetCellValue(DataConvert.SafeDouble(drow["当月网络订单渠道细分太平洋"].ToString()));
                    row.GetCell(28).SetCellValue(DataConvert.SafeDouble(drow["当月网络订单渠道细分其他网络"].ToString()));
                    row.GetCell(29).SetCellValue(DataConvert.SafeDouble(drow["当月网络订单渠道细分转介绍"].ToString()));
                    row.GetCell(30).SetCellValue(DataConvert.SafeDouble(drow["当月网络订单渠道细分触点"].ToString()));
                    row.GetCell(31).SetCellValue(DataConvert.SafeDouble(drow["当月网络订单渠道细分其他"].ToString()));

                    index++;
                }
                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        /// <summary>
        /// 销售日报汇总
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnXSDayGather_Click(object sender, EventArgs e)
        {
            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate2.Text, out day))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("公司");
                tblresult.Columns.Add("总销售台次月目标");
                tblresult.Columns.Add("总销售台次实际");
                tblresult.Columns.Add("总销售台次完成率");
                tblresult.Columns.Add("在库库存实际");
                tblresult.Columns.Add("在途实际");
                tblresult.Columns.Add("总库存实际");
                tblresult.Columns.Add("上月留单实际");
                tblresult.Columns.Add("本月留单实际");
                tblresult.Columns.Add("展厅占比月目标");
                tblresult.Columns.Add("展厅占比实际");
                tblresult.Columns.Add("展厅占比完成率");
                tblresult.Columns.Add("展厅首次来客批次月目标值");
                tblresult.Columns.Add("展厅首次来客批次合计");
                tblresult.Columns.Add("展厅首次来客批次完成率");
                tblresult.Columns.Add("展厅留档率月目标");
                tblresult.Columns.Add("展厅留档率实际");
                tblresult.Columns.Add("展厅留档率完成率");
                tblresult.Columns.Add("展厅成交率月目标");
                tblresult.Columns.Add("展厅成交率实际");
                tblresult.Columns.Add("展厅成交率完成率");
                tblresult.Columns.Add("展厅订单台数月目标值");
                tblresult.Columns.Add("展厅订单台数合计");
                tblresult.Columns.Add("展厅订单台数完成率");
                tblresult.Columns.Add("展厅交车台数月目标值");
                tblresult.Columns.Add("展厅交车台数合计");
                tblresult.Columns.Add("展厅交车台数完成率");
                tblresult.Columns.Add("其中DCC交车台次月目标值");
                tblresult.Columns.Add("其中DCC交车台次合计");
                tblresult.Columns.Add("其中DCC交车台次完成率");
                tblresult.Columns.Add("其中老客户转介绍交车台次月目标值");
                tblresult.Columns.Add("其中老客户转介绍交车台次合计");
                tblresult.Columns.Add("其中老客户转介绍交车台次完成率");
                tblresult.Columns.Add("留微信客户数月目标值");
                tblresult.Columns.Add("留微信客户数合计");
                tblresult.Columns.Add("留微信客户数完成率");
                tblresult.Columns.Add("再次来客批次月目标值");
                tblresult.Columns.Add("再次来客批次合计");
                tblresult.Columns.Add("再次来客批次完成率");
                tblresult.Columns.Add("上牌率月目标");
                tblresult.Columns.Add("上牌率实际");
                tblresult.Columns.Add("上牌率完成率");
                tblresult.Columns.Add("上牌单台月目标");
                tblresult.Columns.Add("上牌单台实际");
                tblresult.Columns.Add("上牌单台完成率");
                tblresult.Columns.Add("展厅保险率月目标");
                tblresult.Columns.Add("展厅保险率实际");
                tblresult.Columns.Add("展厅保险率完成率");
                tblresult.Columns.Add("展厅保险单台月目标");
                tblresult.Columns.Add("展厅保险单台实际");
                tblresult.Columns.Add("展厅保险单台完成率");
                tblresult.Columns.Add("美容交车率月目标");
                tblresult.Columns.Add("美容交车率实际");
                tblresult.Columns.Add("美容交车率完成率");
                tblresult.Columns.Add("美容单台月目标");
                tblresult.Columns.Add("美容单台实际");
                tblresult.Columns.Add("美容单台完成率");
                tblresult.Columns.Add("延保渗透率月目标");
                tblresult.Columns.Add("延保渗透率实际");
                tblresult.Columns.Add("延保渗透率完成率");
                tblresult.Columns.Add("展厅精品前装率月目标");
                tblresult.Columns.Add("展厅精品前装率实际");
                tblresult.Columns.Add("展厅精品前装率完成率");
                tblresult.Columns.Add("展厅精品平均单台月目标");
                tblresult.Columns.Add("展厅精品平均单台实际");
                tblresult.Columns.Add("展厅精品平均单台完成率");
                tblresult.Columns.Add("二网精品平均单台月目标");
                tblresult.Columns.Add("二网精品平均单台实际");
                tblresult.Columns.Add("二网精品平均单台完成率");
                tblresult.Columns.Add("推荐二手车评估数月目标值");
                tblresult.Columns.Add("推荐二手车评估数合计");
                tblresult.Columns.Add("推荐二手车评估数完成率");
                tblresult.Columns.Add("销售置换台次月目标");
                tblresult.Columns.Add("销售置换台次实际");
                tblresult.Columns.Add("销售置换台次完成率");
                tblresult.Columns.Add("按揭率月目标");
                tblresult.Columns.Add("按揭率实际");
                tblresult.Columns.Add("按揭率完成率");
                tblresult.Columns.Add("按揭平均单台月目标");
                tblresult.Columns.Add("按揭平均单台实际");
                tblresult.Columns.Add("按揭平均单台完成率");
                tblresult.Columns.Add("免费保养渗透率月目标");
                tblresult.Columns.Add("免费保养渗透率实际");
                tblresult.Columns.Add("免费保养渗透率完成率");
                tblresult.Columns.Add("免费保养单台月目标");
                tblresult.Columns.Add("免费保养单台实际");
                tblresult.Columns.Add("免费保养单台完成率");
                tblresult.Columns.Add("附加值合计月目标");
                tblresult.Columns.Add("附加值合计实际");
                tblresult.Columns.Add("附加值合计完成率");
                tblresult.Columns.Add("转介绍率月目标");
                tblresult.Columns.Add("转介绍率实际");
                tblresult.Columns.Add("转介绍率完成率");
                tblresult.Columns.Add("展厅新增订单月目标");
                tblresult.Columns.Add("展厅新增订单合计");
                tblresult.Columns.Add("展厅新增订单完成率");
                tblresult.Columns.Add("无忧玻璃渗透率月目标");
                tblresult.Columns.Add("无忧玻璃渗透率实际");
                tblresult.Columns.Add("无忧玻璃渗透率完成率");
                tblresult.Columns.Add("无忧延保渗透率月目标");
                tblresult.Columns.Add("无忧延保渗透率实际");
                tblresult.Columns.Add("无忧延保渗透率完成率");
                tblresult.Columns.Add("机油套餐渗透率月目标");
                tblresult.Columns.Add("机油套餐渗透率实际");
                tblresult.Columns.Add("机油套餐渗透率完成率");
                tblresult.Columns.Add("无忧划痕渗透率月目标");
                tblresult.Columns.Add("无忧划痕渗透率实际");
                tblresult.Columns.Add("无忧划痕渗透率完成率");
                tblresult.Columns.Add("大客户拜访数月目标值");
                tblresult.Columns.Add("大客户拜访数合计");
                tblresult.Columns.Add("大客户拜访数完成率");
                tblresult.Columns.Add("忠诚客户数月目标值");
                tblresult.Columns.Add("忠诚客户数合计");
                tblresult.Columns.Add("忠诚客户数完成率");
                tblresult.Columns.Add("大用户数月目标值");
                tblresult.Columns.Add("大用户数合计");
                tblresult.Columns.Add("大用户数完成率");
                tblresult.Columns.Add("入库台次月目标值");
                tblresult.Columns.Add("入库台次合计");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                double bl = double.Parse(day.Day.ToString()) / Utils.MonthDays(day);
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    #region 销售数据

                    DayReportDep dep = DayReportDep.销售部;
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = corplist[i].ID,
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                    list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, day, true);
                    int days = 0;
                    DataTable tblDay = GetReport(dep, list, monthtarget, day, corplist[i].ID, ref days);
                    DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);

                    row["公司"] = corplist[i].Name;
                    tblKey.DefaultView.RowFilter = "关键指标='总销售台次'";
                    row["总销售台次月目标"] = tblKey.DefaultView[0]["目标"];
                    row["总销售台次实际"] = tblKey.DefaultView[0]["实际"];
                    row["总销售台次完成率"] = Math.Round(DataConvert.SafeDouble(tblKey.DefaultView[0]["完成率"]) / bl, 0);
                    tblKey.DefaultView.RowFilter = "关键指标='在库库存'";
                    row["在库库存实际"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='在途'";
                    row["在途实际"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='总库存'";
                    row["总库存实际"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='上月留单'";
                    row["上月留单实际"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='本月留单'";
                    row["本月留单实际"] = tblKey.DefaultView[0]["实际"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅占比'";
                    row["展厅占比月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅占比实际"] = tblKey.DefaultView[0]["实际"];
                    row["展厅占比完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='展厅首次来客批次'";
                    row["展厅首次来客批次月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅首次来客批次合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅首次来客批次完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblKey.DefaultView.RowFilter = "关键指标='展厅留档率'";
                    row["展厅留档率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅留档率实际"] = tblKey.DefaultView[0]["实际"];
                    row["展厅留档率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅成交率'";
                    row["展厅成交率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅成交率实际"] = tblKey.DefaultView[0]["实际"];
                    row["展厅成交率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='展厅订单台数'";
                    row["展厅订单台数月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅订单台数合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅订单台数完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                    row["展厅交车台数月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["展厅交车台数合计"] = tblDay.DefaultView[0]["合计"];
                    row["展厅交车台数完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='其中DCC交车台次'";
                    row["其中DCC交车台次月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["其中DCC交车台次合计"] = tblDay.DefaultView[0]["合计"];
                    row["其中DCC交车台次完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='其中老客户转介绍交车台次'";
                    row["其中老客户转介绍交车台次月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["其中老客户转介绍交车台次合计"] = tblDay.DefaultView[0]["合计"];
                    row["其中老客户转介绍交车台次完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='留微信客户数'";
                    row["留微信客户数月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["留微信客户数合计"] = tblDay.DefaultView[0]["合计"];
                    row["留微信客户数完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='再次来客批次'";
                    row["再次来客批次月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["再次来客批次合计"] = tblDay.DefaultView[0]["合计"];
                    row["再次来客批次完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblKey.DefaultView.RowFilter = "关键指标='上牌率'";
                    row["上牌率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["上牌率实际"] = tblKey.DefaultView[0]["实际"];
                    row["上牌率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='上牌单台'";
                    row["上牌单台月目标"] = tblKey.DefaultView[0]["目标"];
                    row["上牌单台实际"] = tblKey.DefaultView[0]["实际"];
                    row["上牌单台完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅保险率'";
                    row["展厅保险率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅保险率实际"] = tblKey.DefaultView[0]["实际"];
                    row["展厅保险率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅保险单台'";
                    row["展厅保险单台月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅保险单台实际"] = tblKey.DefaultView[0]["实际"];
                    row["展厅保险单台完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='美容交车率'";
                    row["美容交车率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["美容交车率实际"] = tblKey.DefaultView[0]["实际"];
                    row["美容交车率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='美容单台'";
                    row["美容单台月目标"] = tblKey.DefaultView[0]["目标"];
                    row["美容单台实际"] = tblKey.DefaultView[0]["实际"];
                    row["美容单台完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='无忧延保渗透率'";
                    row["延保渗透率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["延保渗透率实际"] = tblKey.DefaultView[0]["实际"];
                    row["延保渗透率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅精品前装率'";
                    row["展厅精品前装率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅精品前装率实际"] = tblKey.DefaultView[0]["实际"];
                    row["展厅精品前装率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅精品平均单台'";
                    row["展厅精品平均单台月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅精品平均单台实际"] = tblKey.DefaultView[0]["实际"];
                    row["展厅精品平均单台完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='二网精品平均单台'";
                    row["二网精品平均单台月目标"] = tblKey.DefaultView[0]["目标"];
                    row["二网精品平均单台实际"] = tblKey.DefaultView[0]["实际"];
                    row["二网精品平均单台完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='推荐二手车评估数'";
                    row["推荐二手车评估数月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["推荐二手车评估数合计"] = tblDay.DefaultView[0]["合计"];
                    row["推荐二手车评估数完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblKey.DefaultView.RowFilter = "关键指标='销售置换台次'";
                    row["销售置换台次月目标"] = tblKey.DefaultView[0]["目标"];
                    row["销售置换台次实际"] = tblKey.DefaultView[0]["实际"];
                    row["销售置换台次完成率"] = Math.Round(DataConvert.SafeDouble(tblKey.DefaultView[0]["完成率"]) / bl, 0);
                    tblKey.DefaultView.RowFilter = "关键指标='按揭率'";
                    row["按揭率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["按揭率实际"] = tblKey.DefaultView[0]["实际"];
                    row["按揭率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='按揭平均单台'";
                    row["按揭平均单台月目标"] = tblKey.DefaultView[0]["目标"];
                    row["按揭平均单台实际"] = tblKey.DefaultView[0]["实际"];
                    row["按揭平均单台完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='免费保养渗透率'";
                    row["免费保养渗透率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["免费保养渗透率实际"] = tblKey.DefaultView[0]["实际"];
                    row["免费保养渗透率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='免费保养单台'";
                    row["免费保养单台月目标"] = tblKey.DefaultView[0]["目标"];
                    row["免费保养单台实际"] = tblKey.DefaultView[0]["实际"];
                    row["免费保养单台完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='附加值合计'";
                    row["附加值合计月目标"] = tblKey.DefaultView[0]["目标"];
                    row["附加值合计实际"] = tblKey.DefaultView[0]["实际"];
                    row["附加值合计完成率"] = Math.Round(DataConvert.SafeDouble(tblKey.DefaultView[0]["完成率"]) / bl, 0);
                    tblKey.DefaultView.RowFilter = "关键指标='转介绍率'";
                    row["转介绍率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["转介绍率实际"] = tblKey.DefaultView[0]["实际"];
                    row["转介绍率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='展厅新增订单'";
                    row["展厅新增订单月目标"] = tblKey.DefaultView[0]["目标"];
                    row["展厅新增订单合计"] = tblKey.DefaultView[0]["实际"];
                    row["展厅新增订单完成率"] = Math.Round(DataConvert.SafeDouble(tblKey.DefaultView[0]["完成率"]) / bl, 0);
                    tblKey.DefaultView.RowFilter = "关键指标='无忧玻璃渗透率'";
                    row["无忧玻璃渗透率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["无忧玻璃渗透率实际"] = tblKey.DefaultView[0]["实际"];
                    row["无忧玻璃渗透率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='无忧延保渗透率'";
                    row["无忧延保渗透率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["无忧延保渗透率实际"] = tblKey.DefaultView[0]["实际"];
                    row["无忧延保渗透率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='机油套餐渗透率'";
                    row["机油套餐渗透率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["机油套餐渗透率实际"] = tblKey.DefaultView[0]["实际"];
                    row["机油套餐渗透率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblKey.DefaultView.RowFilter = "关键指标='无忧划痕渗透率'";
                    row["无忧划痕渗透率月目标"] = tblKey.DefaultView[0]["目标"];
                    row["无忧划痕渗透率实际"] = tblKey.DefaultView[0]["实际"];
                    row["无忧划痕渗透率完成率"] = tblKey.DefaultView[0]["完成率"];
                    tblDay.DefaultView.RowFilter = "项目='大客户拜访数'";
                    row["大客户拜访数月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["大客户拜访数合计"] = tblDay.DefaultView[0]["合计"];
                    row["大客户拜访数完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='忠诚客户数'";
                    row["忠诚客户数月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["忠诚客户数合计"] = tblDay.DefaultView[0]["合计"];
                    row["忠诚客户数完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='大用户数'";
                    row["大用户数月目标值"] = tblDay.DefaultView[0]["目标值"];
                    //row["大用户数周目标值"] = Math.Round(DataConvert.SafeInt(tblDay.DefaultView[0]["目标值"]) * bl, 0);
                    row["大用户数合计"] = tblDay.DefaultView[0]["合计"];
                    row["大用户数完成率"] = Math.Round(DataConvert.SafeDouble(tblDay.DefaultView[0]["完成率"]) / bl, 0);
                    tblDay.DefaultView.RowFilter = "项目='入库台次'";
                    row["入库台次月目标值"] = tblDay.DefaultView[0]["目标值"];
                    row["入库台次合计"] = tblDay.DefaultView[0]["合计"];

                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\销售日报汇总模版.xls"));
                newfile = string.Format(@"oa截止{0}销售日报汇总.xls", day.ToString("MM月dd"));
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new HSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                #endregion

                int index = 5;
                foreach (DataRow drow in tblresult.Rows)
                {
                    HSSFRow row = (HSSFRow)sheet.CreateRow(index);
                    row.CreateCell(0).SetCellValue(drow["公司"].ToString());
                    row.CreateCell(1).SetCellValue(GetCellValue(drow["总销售台次月目标"].ToString(), false));
                    row.CreateCell(2).SetCellValue(GetCellValue(drow["总销售台次实际"].ToString(), false));
                    row.CreateCell(3).SetCellValue(GetCellValue(drow["总销售台次完成率"].ToString(), true));
                    row.CreateCell(4).SetCellValue(GetCellValue(drow["在库库存实际"].ToString(), false));
                    row.CreateCell(5).SetCellValue(GetCellValue(drow["在途实际"].ToString(), false));
                    row.CreateCell(6).SetCellValue(GetCellValue(drow["总库存实际"].ToString(), false));
                    row.CreateCell(7).SetCellValue(GetCellValue(drow["上月留单实际"].ToString(), false));
                    row.CreateCell(8).SetCellValue(GetCellValue(drow["本月留单实际"].ToString(), false));
                    row.CreateCell(9).SetCellValue(GetCellValue(drow["展厅占比月目标"].ToString(), true));
                    row.CreateCell(10).SetCellValue(GetCellValue(drow["展厅占比实际"].ToString(), true));
                    row.CreateCell(11).SetCellValue(GetCellValue(drow["展厅占比完成率"].ToString(), true));
                    row.CreateCell(12).SetCellValue(GetCellValue(drow["展厅首次来客批次月目标值"].ToString(), false));
                    row.CreateCell(13).SetCellValue(GetCellValue(drow["展厅首次来客批次合计"].ToString(), false));
                    row.CreateCell(14).SetCellValue(GetCellValue(drow["展厅首次来客批次完成率"].ToString(), true));
                    row.CreateCell(15).SetCellValue(GetCellValue(drow["展厅留档率月目标"].ToString(), true));
                    row.CreateCell(16).SetCellValue(GetCellValue(drow["展厅留档率实际"].ToString(), true));
                    row.CreateCell(17).SetCellValue(GetCellValue(drow["展厅留档率完成率"].ToString(), true));
                    row.CreateCell(18).SetCellValue(GetCellValue(drow["展厅成交率月目标"].ToString(), true));
                    row.CreateCell(19).SetCellValue(GetCellValue(drow["展厅成交率实际"].ToString(), true));
                    row.CreateCell(20).SetCellValue(GetCellValue(drow["展厅成交率完成率"].ToString(), true));
                    row.CreateCell(21).SetCellValue(GetCellValue(drow["展厅订单台数月目标值"].ToString(), false));
                    row.CreateCell(22).SetCellValue(GetCellValue(drow["展厅订单台数合计"].ToString(), false));
                    row.CreateCell(23).SetCellValue(GetCellValue(drow["展厅订单台数完成率"].ToString(), true));
                    row.CreateCell(24).SetCellValue(GetCellValue(drow["展厅交车台数月目标值"].ToString(), false));
                    row.CreateCell(25).SetCellValue(GetCellValue(drow["展厅交车台数合计"].ToString(), false));
                    row.CreateCell(26).SetCellValue(GetCellValue(drow["展厅交车台数完成率"].ToString(), true));
                    row.CreateCell(27).SetCellValue(GetCellValue(drow["其中DCC交车台次月目标值"].ToString(), false));
                    row.CreateCell(28).SetCellValue(GetCellValue(drow["其中DCC交车台次合计"].ToString(), false));
                    row.CreateCell(29).SetCellValue(GetCellValue(drow["其中DCC交车台次完成率"].ToString(), true));
                    row.CreateCell(30).SetCellValue(GetCellValue(drow["其中老客户转介绍交车台次月目标值"].ToString(), false));
                    row.CreateCell(31).SetCellValue(GetCellValue(drow["其中老客户转介绍交车台次合计"].ToString(), false));
                    row.CreateCell(32).SetCellValue(GetCellValue(drow["其中老客户转介绍交车台次完成率"].ToString(), true));
                    row.CreateCell(33).SetCellValue(GetCellValue(drow["留微信客户数月目标值"].ToString(), false));
                    row.CreateCell(34).SetCellValue(GetCellValue(drow["留微信客户数合计"].ToString(), false));
                    row.CreateCell(35).SetCellValue(GetCellValue(drow["留微信客户数完成率"].ToString(), true));
                    row.CreateCell(36).SetCellValue(GetCellValue(drow["再次来客批次月目标值"].ToString(), false));
                    row.CreateCell(37).SetCellValue(GetCellValue(drow["再次来客批次合计"].ToString(), false));
                    row.CreateCell(38).SetCellValue(GetCellValue(drow["再次来客批次完成率"].ToString(), true));
                    row.CreateCell(39).SetCellValue(GetCellValue(drow["上牌率月目标"].ToString(), true));
                    row.CreateCell(40).SetCellValue(GetCellValue(drow["上牌率实际"].ToString(), true));
                    row.CreateCell(41).SetCellValue(GetCellValue(drow["上牌率完成率"].ToString(), true));
                    row.CreateCell(42).SetCellValue(GetCellValue(drow["上牌单台月目标"].ToString(), false));
                    row.CreateCell(43).SetCellValue(GetCellValue(drow["上牌单台实际"].ToString(), false));
                    row.CreateCell(44).SetCellValue(GetCellValue(drow["上牌单台完成率"].ToString(), true));
                    row.CreateCell(45).SetCellValue(GetCellValue(drow["展厅保险率月目标"].ToString(), true));
                    row.CreateCell(46).SetCellValue(GetCellValue(drow["展厅保险率实际"].ToString(), true));
                    row.CreateCell(47).SetCellValue(GetCellValue(drow["展厅保险率完成率"].ToString(), true));
                    row.CreateCell(48).SetCellValue(GetCellValue(drow["展厅保险单台月目标"].ToString(), false));
                    row.CreateCell(49).SetCellValue(GetCellValue(drow["展厅保险单台实际"].ToString(), false));
                    row.CreateCell(50).SetCellValue(GetCellValue(drow["展厅保险单台完成率"].ToString(), true));
                    row.CreateCell(51).SetCellValue(GetCellValue(drow["美容交车率月目标"].ToString(), true));
                    row.CreateCell(52).SetCellValue(GetCellValue(drow["美容交车率实际"].ToString(), true));
                    row.CreateCell(53).SetCellValue(GetCellValue(drow["美容交车率完成率"].ToString(), true));
                    row.CreateCell(54).SetCellValue(GetCellValue(drow["美容单台月目标"].ToString(), false));
                    row.CreateCell(55).SetCellValue(GetCellValue(drow["美容单台实际"].ToString(), false));
                    row.CreateCell(56).SetCellValue(GetCellValue(drow["美容单台完成率"].ToString(), true));
                    row.CreateCell(57).SetCellValue(GetCellValue(drow["延保渗透率月目标"].ToString(), true));
                    row.CreateCell(58).SetCellValue(GetCellValue(drow["延保渗透率实际"].ToString(), true));
                    row.CreateCell(59).SetCellValue(GetCellValue(drow["延保渗透率完成率"].ToString(), true));
                    row.CreateCell(60).SetCellValue(GetCellValue(drow["展厅精品前装率月目标"].ToString(), true));
                    row.CreateCell(61).SetCellValue(GetCellValue(drow["展厅精品前装率实际"].ToString(), true));
                    row.CreateCell(62).SetCellValue(GetCellValue(drow["展厅精品前装率完成率"].ToString(), true));
                    row.CreateCell(63).SetCellValue(GetCellValue(drow["展厅精品平均单台月目标"].ToString(), false));
                    row.CreateCell(64).SetCellValue(GetCellValue(drow["展厅精品平均单台实际"].ToString(), false));
                    row.CreateCell(65).SetCellValue(GetCellValue(drow["展厅精品平均单台完成率"].ToString(), true));
                    row.CreateCell(66).SetCellValue(GetCellValue(drow["二网精品平均单台月目标"].ToString(), false));
                    row.CreateCell(67).SetCellValue(GetCellValue(drow["二网精品平均单台实际"].ToString(), false));
                    row.CreateCell(68).SetCellValue(GetCellValue(drow["二网精品平均单台完成率"].ToString(), true));
                    row.CreateCell(69).SetCellValue(GetCellValue(drow["推荐二手车评估数月目标值"].ToString(), false));
                    row.CreateCell(70).SetCellValue(GetCellValue(drow["推荐二手车评估数合计"].ToString(), false));
                    row.CreateCell(71).SetCellValue(GetCellValue(drow["推荐二手车评估数完成率"].ToString(), true));
                    row.CreateCell(72).SetCellValue(GetCellValue(drow["销售置换台次月目标"].ToString(), false));
                    row.CreateCell(73).SetCellValue(GetCellValue(drow["销售置换台次实际"].ToString(), false));
                    row.CreateCell(74).SetCellValue(GetCellValue(drow["销售置换台次完成率"].ToString(), true));
                    row.CreateCell(75).SetCellValue(GetCellValue(drow["按揭率月目标"].ToString(), true));
                    row.CreateCell(76).SetCellValue(GetCellValue(drow["按揭率实际"].ToString(), true));
                    row.CreateCell(77).SetCellValue(GetCellValue(drow["按揭率完成率"].ToString(), true));
                    row.CreateCell(78).SetCellValue(GetCellValue(drow["按揭平均单台月目标"].ToString(), false));
                    row.CreateCell(79).SetCellValue(GetCellValue(drow["按揭平均单台实际"].ToString(), false));
                    row.CreateCell(80).SetCellValue(GetCellValue(drow["按揭平均单台完成率"].ToString(), true));
                    row.CreateCell(81).SetCellValue(GetCellValue(drow["免费保养渗透率月目标"].ToString(), true));
                    row.CreateCell(82).SetCellValue(GetCellValue(drow["免费保养渗透率实际"].ToString(), true));
                    row.CreateCell(83).SetCellValue(GetCellValue(drow["免费保养渗透率完成率"].ToString(), true));
                    row.CreateCell(84).SetCellValue(GetCellValue(drow["免费保养单台月目标"].ToString(), false));
                    row.CreateCell(85).SetCellValue(GetCellValue(drow["免费保养单台实际"].ToString(), false));
                    row.CreateCell(86).SetCellValue(GetCellValue(drow["免费保养单台完成率"].ToString(), true));
                    row.CreateCell(87).SetCellValue(GetCellValue(drow["附加值合计月目标"].ToString(), false));
                    row.CreateCell(88).SetCellValue(GetCellValue(drow["附加值合计实际"].ToString(), false));
                    row.CreateCell(89).SetCellValue(GetCellValue(drow["附加值合计完成率"].ToString(), true));
                    row.CreateCell(90).SetCellValue(GetCellValue(drow["转介绍率月目标"].ToString(), true));
                    row.CreateCell(91).SetCellValue(GetCellValue(drow["转介绍率实际"].ToString(), true));
                    row.CreateCell(92).SetCellValue(GetCellValue(drow["转介绍率完成率"].ToString(), true));
                    row.CreateCell(93).SetCellValue(GetCellValue(drow["展厅新增订单月目标"].ToString(), false));
                    row.CreateCell(94).SetCellValue(GetCellValue(drow["展厅新增订单合计"].ToString(), false));
                    row.CreateCell(95).SetCellValue(GetCellValue(drow["展厅新增订单完成率"].ToString(), true));
                    row.CreateCell(96).SetCellValue(GetCellValue(drow["无忧玻璃渗透率月目标"].ToString(), true));
                    row.CreateCell(97).SetCellValue(GetCellValue(drow["无忧玻璃渗透率实际"].ToString(), true));
                    row.CreateCell(98).SetCellValue(GetCellValue(drow["无忧玻璃渗透率完成率"].ToString(), true));
                    row.CreateCell(99).SetCellValue(GetCellValue(drow["无忧延保渗透率月目标"].ToString(), true));
                    row.CreateCell(100).SetCellValue(GetCellValue(drow["无忧延保渗透率实际"].ToString(), true));
                    row.CreateCell(101).SetCellValue(GetCellValue(drow["无忧延保渗透率完成率"].ToString(), true));
                    row.CreateCell(102).SetCellValue(GetCellValue(drow["机油套餐渗透率月目标"].ToString(), true));
                    row.CreateCell(103).SetCellValue(GetCellValue(drow["机油套餐渗透率实际"].ToString(), true));
                    row.CreateCell(104).SetCellValue(GetCellValue(drow["机油套餐渗透率完成率"].ToString(), true));
                    row.CreateCell(105).SetCellValue(GetCellValue(drow["无忧划痕渗透率月目标"].ToString(), true));
                    row.CreateCell(106).SetCellValue(GetCellValue(drow["无忧划痕渗透率实际"].ToString(), true));
                    row.CreateCell(107).SetCellValue(GetCellValue(drow["无忧划痕渗透率完成率"].ToString(), true));
                    row.CreateCell(108).SetCellValue(GetCellValue(drow["大客户拜访数月目标值"].ToString(), false));
                    row.CreateCell(109).SetCellValue(GetCellValue(drow["大客户拜访数合计"].ToString(), false));
                    row.CreateCell(110).SetCellValue(GetCellValue(drow["大客户拜访数完成率"].ToString(), true));
                    row.CreateCell(111).SetCellValue(GetCellValue(drow["忠诚客户数月目标值"].ToString(), false));
                    row.CreateCell(112).SetCellValue(GetCellValue(drow["忠诚客户数合计"].ToString(), false));
                    row.CreateCell(113).SetCellValue(GetCellValue(drow["忠诚客户数完成率"].ToString(), true));
                    row.CreateCell(114).SetCellValue(GetCellValue(drow["大用户数月目标值"].ToString(), false));
                    row.CreateCell(115).SetCellValue(GetCellValue(drow["大用户数合计"].ToString(), false));
                    row.CreateCell(116).SetCellValue(GetCellValue(drow["大用户数完成率"].ToString(), true));
                    row.CreateCell(117).SetCellValue(GetCellValue(drow["入库台次月目标值"].ToString(), false));
                    row.CreateCell(118).SetCellValue(GetCellValue(drow["入库台次合计"].ToString(), false));

                    for (int i = 0; i <= 118; i++)
                    {
                        if (tblresult.Columns[i].ColumnName.IndexOf("完成率") >= 0 && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) >= 80)
                            sheet.GetRow(index).Cells[i].CellStyle = cellStyleGreen;
                        else if (tblresult.Columns[i].ColumnName.IndexOf("完成率") >= 0 && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) >= 0 && DataConvert.SafeDouble(drow[tblresult.Columns[i].ColumnName]) <= 40)
                            sheet.GetRow(index).Cells[i].CellStyle = cellStyleYellow;
                        else
                            sheet.GetRow(index).Cells[i].CellStyle = cellStyleBlack;
                    }
                    index++;
                }
                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        protected void btnXSYearGather_Click(object sender, EventArgs e)
        {
            DateTime day1 = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day1) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("公司");
                tblresult.Columns.Add("目标整车销售量");
                tblresult.Columns.Add("实际整车销售量");
                tblresult.Columns.Add("展厅目标数");
                tblresult.Columns.Add("展厅实际数");
                tblresult.Columns.Add("整车实际销售额");
                tblresult.Columns.Add("整车裸车毛利额");
                tblresult.Columns.Add("厂方返利收入");
                tblresult.Columns.Add("含厂家返利的预算毛利率");
                tblresult.Columns.Add("现金上牌手续费收入");
                tblresult.Columns.Add("按揭手续费净收入");
                tblresult.Columns.Add("厂方金融手续费净收入");
                tblresult.Columns.Add("保险返利收入");
                tblresult.Columns.Add("交车费收入");
                tblresult.Columns.Add("销售延保和免费保养净收入");
                tblresult.Columns.Add("销售部精品毛利收入");
                tblresult.Columns.Add("其他收入");
                tblresult.Columns.Add("整车综合预算毛利率");
                tblresult.Columns.Add("保险成交量");
                tblresult.Columns.Add("新车保险交易额");
                tblresult.Columns.Add("上牌量");
                tblresult.Columns.Add("美容交车台次");
                tblresult.Columns.Add("美容交车平均单台净收入");
                tblresult.Columns.Add("按揭台次");
                tblresult.Columns.Add("厂家金融台次");
                tblresult.Columns.Add("预算按揭率");
                tblresult.Columns.Add("预算按揭平均每台净收入");
                tblresult.Columns.Add("销售延保的台次");
                tblresult.Columns.Add("延保平均单车产值");
                tblresult.Columns.Add("延保平均单台毛利");
                tblresult.Columns.Add("免费保养的台次");
                tblresult.Columns.Add("免费保养平均单车产值");
                tblresult.Columns.Add("免费保养平均单台毛利");
                tblresult.Columns.Add("展厅附加值净收入的平均单台目标");
                tblresult.Columns.Add("标准库存量");
                tblresult.Columns.Add("现有在库存车辆数");
                tblresult.Columns.Add("现有在途库存数");
                tblresult.Columns.Add("3个月以上的库存");
                tblresult.Columns.Add("6个月以上的库存");
                tblresult.Columns.Add("1年以上的库存");
                tblresult.Columns.Add("标准库存周转天数");
                tblresult.Columns.Add("库存资金占用总成本利息");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    #region 销售数据

                    List<DataTable> tblDays = new List<DataTable>();
                    List<DataTable> tblKeys = new List<DataTable>();
                    List<MonthlyTargetInfo> monthtargets = new List<MonthlyTargetInfo>();
                    DayReportDep dep = DayReportDep.销售部;
                    DateTime daytemp = day1;
                    while (DateTime.Parse(daytemp.ToString("yyyy-MM") + "-01") <= DateTime.Parse(day2.ToString("yyyy-MM") + "-01"))
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytemp.ToString("yyyyMM"),
                            CorporationID = corplist[i].ID,
                            DayReportDep = dep
                        };
                        query.OrderBy = " [DayUnique] ASC";
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, daytemp, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, daytemp, corplist[i].ID, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);
                        tblDays.Add(tblDay);
                        tblKeys.Add(tblKey);
                        if (monthtarget != null)
                            monthtargets.Add(monthtarget);
                        daytemp = daytemp.AddMonths(1);

                    }

                    row["公司"] = corplist[i].Name;
                    row["目标整车销售量"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='总销售台次'")["目标"]));
                    row["实际整车销售量"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='总销售台次'")["实际"]));
                    row["展厅目标数"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅交车台数'")["目标值"]));
                    row["展厅实际数"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅交车台数'")["合计"]));
                    row["整车实际销售额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzcsjxse));
                    row["整车裸车毛利额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzclcsjmle));
                    row["厂方返利收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbycfflsjsr));
                    row["含厂家返利的预算毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzcysxse)) > 0 ? ((monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbycfflyssr)) + monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzclcysmle))) / monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzcysxse))).ToString() : "";
                    row["现金上牌手续费收入"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='上牌总金额'")["合计"]));
                    decimal ysspzje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='上牌总金额'")["目标值"]));
                    row["按揭手续费净收入"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='按揭净收入'")["合计"]));
                    decimal ysajjsrsxf = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='按揭净收入'")["目标值"]));
                    row["厂方金融手续费净收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbycfjrsxfjsr));
                    decimal ewbxfl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网保险返利'")["合计"]));
                    decimal ysewbxfl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网保险返利'")["目标值"]));
                    decimal ztbxfl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅含DCC保险返利'")["合计"]));
                    decimal ysztbxfl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅含DCC保险返利'")["目标值"]));
                    row["保险返利收入"] = ztbxfl + ewbxfl;
                    row["交车费收入"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='美容交车总金额'")["合计"]));
                    decimal ysjcfsr = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='美容交车总金额'")["目标值"]));
                    decimal ybzje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='延保无忧车服务购买金额'")["合计"]));
                    decimal ysybzje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='延保无忧车服务购买金额'")["目标值"]));
                    decimal zsmfbyzje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='免费保养总金额'")["合计"]));
                    decimal yszsmfbyzje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='免费保养总金额'")["目标值"]));
                    row["销售延保和免费保养净收入"] = ybzje * 30 / 100 + zsmfbyzje;
                    row["销售部精品毛利收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyjpmlsr));
                    row["其他收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyqtsr));
                    decimal yszclcmle = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzclcysmle)); //整车裸车预算毛利额
                    decimal yscfflsr = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbycfflyssr)); //厂方返利预算收入
                    decimal ysjpmlsr = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyjpmlyssr));//本月精品预算毛利

                    row["整车综合预算毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzcysxse)) > 0 ? ((yszclcmle + yscfflsr + ysspzje + ysajjsrsxf + ysewbxfl + ysztbxfl + ysjcfsr + ysybzje * 30 / 100 + yszsmfbyzje + ysjpmlsr + DataConvert.SafeDecimal(row["其他收入"])) / monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzcysxse))).ToString() : "";
                    decimal ewbxtc = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网保险台次'")["合计"]));
                    decimal ztbxtc = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅含DCC保险台次'")["合计"]));
                    row["保险成交量"] = ewbxtc + ztbxtc;
                    decimal ewbxje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网保险金额'")["合计"]));
                    decimal ztbxje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅含DCC保险总金额'")["合计"]));
                    row["新车保险交易额"] = ewbxje + ztbxje;
                    row["上牌量"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='上牌台次'")["合计"]));
                    row["美容交车台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='美容交车台次'")["合计"]));
                    row["美容交车平均单台净收入"] = DataConvert.SafeInt(row["美容交车台次"]) > 0 ? (monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbymrjcjsr)) / DataConvert.SafeInt(row["美容交车台次"])).ToString() : "";
                    decimal bdajtc = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='本地按揭台次'")["合计"]));
                    decimal yhajtc = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='银行按揭台次'")["合计"]));
                    decimal cjjrtc = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='厂家金融台次'")["合计"]));
                    row["按揭台次"] = bdajtc + yhajtc + cjjrtc;
                    row["厂家金融台次"] = cjjrtc;
                    row["预算按揭率"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='按揭率'")["目标"]));
                    row["预算按揭平均每台净收入"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='按揭平均单台'")["目标"]));
                    row["销售延保的台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='延保无忧车服务购买个数'")["合计"]));
                    row["延保平均单车产值"] = DataConvert.SafeInt(row["销售延保的台次"]) > 0 ? (ybzje / DataConvert.SafeInt(row["销售延保的台次"])).ToString() : "";
                    row["美容交车平均单台净收入"] = DataConvert.SafeInt(row["销售延保的台次"]) > 0 ? (monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyybml)) / DataConvert.SafeInt(row["销售延保的台次"])).ToString() : "";
                    row["免费保养的台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='免费保养台次（含赠送）'")["合计"]));
                    row["免费保养平均单车产值"] = DataConvert.SafeInt(row["免费保养的台次"]) > 0 ? (zsmfbyzje / DataConvert.SafeInt(row["免费保养的台次"])).ToString() : "";
                    row["免费保养平均单台毛利"] = DataConvert.SafeInt(row["免费保养的台次"]) > 0 ? (monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSbyzsmfbyml)) / DataConvert.SafeInt(row["免费保养的台次"])).ToString() : "";
                    row["展厅附加值净收入的平均单台目标"] = DataConvert.SafeDecimal(row["展厅目标数"]) > 0 ? (tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='附加值合计'")["目标"])) / DataConvert.SafeDecimal(row["展厅目标数"])).ToString() : "";
                    row["标准库存量"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='标准在库库存'")["目标"]));
                    row["现有在库存车辆数"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='标准在库库存'")["实际"]));
                    row["现有在途库存数"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='在途'")["实际"]));
                    row["3个月以上的库存"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='在库超3个月'")["实际"]));
                    row["6个月以上的库存"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSzkclgytc));
                    row["1年以上的库存"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSzkcyntc));
                    row["标准库存周转天数"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.XSzzts));
                    row["库存资金占用总成本利息"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='库存利息'")["实际"]));

                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\销售数据分析汇总.xls"));
                newfile = string.Format(@"{0}{1}销售数据分析汇总.xls", day1.ToString("yyyy年M月"), DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "");
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new HSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                #endregion

                sheet.GetRow(0).GetCell(0).SetCellValue(day1.ToString("yyyy年M月") + (DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "") + "销售数据分析表");

                int index = 5;
                foreach (DataRow drow in tblresult.Rows)
                {
                    sheet.GetRow(1).GetCell(index).SetCellValue(drow["公司"].ToString());
                    sheet.GetRow(2).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["目标整车销售量"]));
                    sheet.GetRow(3).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["实际整车销售量"]));
                    sheet.GetRow(4).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅目标数"]));
                    sheet.GetRow(5).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅实际数"]));
                    sheet.GetRow(8).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["整车实际销售额"]) / 10000);
                    sheet.GetRow(9).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["整车裸车毛利额"]) / 10000);
                    sheet.GetRow(12).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["厂方返利收入"]) / 10000);
                    sheet.GetRow(15).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["现金上牌手续费收入"]) / 10000);
                    sheet.GetRow(16).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["按揭手续费净收入"]) / 10000);
                    sheet.GetRow(17).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["厂方金融手续费净收入"]) / 10000);
                    sheet.GetRow(18).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["保险返利收入"]) / 10000);
                    sheet.GetRow(19).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["交车费收入"]) / 10000);
                    sheet.GetRow(20).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["销售延保和免费保养净收入"]) / 10000);
                    sheet.GetRow(21).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["销售部精品毛利收入"]) / 10000);
                    sheet.GetRow(22).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["其他收入"]) / 10000);
                    sheet.GetRow(25).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["整车综合预算毛利率"]));
                    sheet.GetRow(27).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["保险成交量"]));
                    sheet.GetRow(29).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["新车保险交易额"]) / 10000);
                    sheet.GetRow(31).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["保险返利收入"]) / 10000);
                    sheet.GetRow(34).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["上牌量"]));
                    sheet.GetRow(37).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["美容交车台次"]));
                    sheet.GetRow(38).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["美容交车平均单台净收入"]) / 10000);
                    sheet.GetRow(40).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["按揭台次"]));
                    sheet.GetRow(41).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["厂家金融台次"]));
                    sheet.GetRow(43).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["预算按揭率"]) / 100);
                    sheet.GetRow(44).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["预算按揭平均每台净收入"]) / 10000);
                    sheet.GetRow(48).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["销售延保的台次"]));
                    sheet.GetRow(50).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["延保平均单车产值"]) / 10000);
                    sheet.GetRow(51).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["延保平均单台毛利"]) / 10000);
                    sheet.GetRow(52).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["免费保养的台次"]));
                    sheet.GetRow(54).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["免费保养平均单车产值"]) / 10000);
                    sheet.GetRow(55).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["免费保养平均单台毛利"]) / 10000);
                    sheet.GetRow(56).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅附加值净收入的平均单台目标"]) / 10000);
                    sheet.GetRow(58).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["标准库存量"]));
                    sheet.GetRow(59).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["现有在库存车辆数"]));
                    sheet.GetRow(60).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["现有在途库存数"]));
                    sheet.GetRow(61).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["3个月以上的库存"]));
                    sheet.GetRow(62).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["6个月以上的库存"]));
                    sheet.GetRow(63).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["1年以上的库存"]));
                    sheet.GetRow(64).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["标准库存周转天数"]));
                    sheet.GetRow(66).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["库存资金占用总成本利息"]) / 10000);

                    index++;
                }
                for (int i = index; i < 35; i++)
                {
                    sheet.SetColumnHidden(i, true);
                }

                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        protected void btnSHYearGather_Click(object sender, EventArgs e)
        {
            DateTime day1 = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day1) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("公司");
                tblresult.Columns.Add("预算台次");
                tblresult.Columns.Add("来厂台次");
                tblresult.Columns.Add("预算产值");
                tblresult.Columns.Add("维修产值");
                tblresult.Columns.Add("一般维修额");
                tblresult.Columns.Add("首保索赔额");
                tblresult.Columns.Add("事故车收入");
                tblresult.Columns.Add("免费保养的收入");
                tblresult.Columns.Add("养护产品的收入");
                tblresult.Columns.Add("他牌车收入");
                tblresult.Columns.Add("其中事故车台次");
                tblresult.Columns.Add("维修毛利额");
                tblresult.Columns.Add("维修毛利率");
                tblresult.Columns.Add("废旧收入");
                tblresult.Columns.Add("含废旧的维修实际毛利率");
                tblresult.Columns.Add("含废旧的维修预算毛利率");
                tblresult.Columns.Add("油漆收入");
                tblresult.Columns.Add("油漆成本");
                tblresult.Columns.Add("占产值收入比例目标");
                tblresult.Columns.Add("其中养护产品毛利率");
                tblresult.Columns.Add("标准库存金额");
                tblresult.Columns.Add("期末实际库存额");
                tblresult.Columns.Add("其中一年以上库存额");
                tblresult.Columns.Add("标准的库存度");
                tblresult.Columns.Add("实际库存度");
                tblresult.Columns.Add("配件毛利率");
                tblresult.Columns.Add("事故车毛利率");
                tblresult.Columns.Add("一般维修毛利率");
                tblresult.Columns.Add("他牌车维修毛利率");
                tblresult.Columns.Add("续保预算台次");
                tblresult.Columns.Add("续保实际台次");
                tblresult.Columns.Add("续保目标渗透率");
                tblresult.Columns.Add("续保实际渗透率");
                tblresult.Columns.Add("续保总额");
                tblresult.Columns.Add("续保返利收入");
                tblresult.Columns.Add("续保平均单台净收入");
                tblresult.Columns.Add("免费保养预算台次");
                tblresult.Columns.Add("免费保养实际台次");
                tblresult.Columns.Add("免费保养目标渗透率");
                tblresult.Columns.Add("免费保养实际渗透率");
                tblresult.Columns.Add("免费保养总额");
                tblresult.Columns.Add("延保返利收入");
                tblresult.Columns.Add("延保平均单台净收入");
                tblresult.Columns.Add("导航升级业务个数");
                tblresult.Columns.Add("导航升级业务平均单台收入");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    #region 售后数据

                    List<DataTable> tblDays = new List<DataTable>();
                    List<DataTable> tblKeys = new List<DataTable>();
                    List<MonthlyTargetInfo> monthtargets = new List<MonthlyTargetInfo>();
                    DayReportDep dep = DayReportDep.售后部;
                    DateTime daytemp = day1;
                    while (DateTime.Parse(daytemp.ToString("yyyy-MM") + "-01") <= DateTime.Parse(day2.ToString("yyyy-MM") + "-01"))
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytemp.ToString("yyyyMM"),
                            CorporationID = corplist[i].ID,
                            DayReportDep = dep
                        };
                        query.OrderBy = " [DayUnique] ASC";
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, daytemp, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, daytemp, corplist[i].ID, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);
                        tblDays.Add(tblDay);
                        tblKeys.Add(tblKey);
                        if (monthtarget != null)
                            monthtargets.Add(monthtarget);
                        daytemp = daytemp.AddMonths(1);

                    }

                    row["公司"] = corplist[i].Name;
                    row["预算台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='来厂台次'")["目标值"]));
                    row["来厂台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='来厂台次'")["合计"]));
                    decimal wxcz = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当日产值'")["合计"]));
                    row["预算产值"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当日产值'")["目标值"]));
                    row["维修产值"] = wxcz;
                    row["一般维修额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyybwxe));
                    row["首保索赔额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbysbspe));
                    decimal sgcsr = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='事故总产值'")["合计"]));
                    row["事故车收入"] = sgcsr;
                    decimal yhczmb = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='养护产值'")["目标值"]));
                    row["养护产品的收入"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='养护产值'")["合计"]));
                    row["他牌车收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbytppsr));
                    row["其中事故车台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='事故来厂台次'")["合计"]));
                    row["维修毛利额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbywxmle));
                    row["维修毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbywxmll));
                    row["废旧收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyfjsr));
                    row["含废旧的维修实际毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyhfjdwxsjmll));
                    row["含废旧的维修预算毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyhfjdwxysmll));
                    row["油漆收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyyqsr));
                    row["油漆成本"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyyqcb));
                    row["占产值收入比例目标"] = wxcz > 0 ? (yhczmb / (wxcz * (1 - ((sgcsr / wxcz) - (decimal)0.5)))).ToString() : string.Empty;
                    row["其中养护产品毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyyhcpmll));
                    row["标准库存金额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbybzkcje));
                    row["期末实际库存额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyqmsjkce));
                    row["其中一年以上库存额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHqzynyskce));
                    row["标准的库存度"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbybzdkcd));
                    row["实际库存度"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbysjkcd));
                    row["配件毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbypjmll));
                    row["事故车毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbysgcmll));
                    row["一般维修毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyybwxmll));
                    row["他牌车维修毛利率"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbytppwxmll));
                    row["续保预算台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='延保台次'")["目标值"]));
                    row["续保实际台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='延保台次'")["合计"]));
                    row["续保返利收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyxbflsr));
                    row["续保平均单台净收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyxbpjdtjsr));
                    row["免费保养预算台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='免费保养'")["目标值"]));
                    row["免费保养实际台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='免费保养'")["合计"]));
                    row["延保返利收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyybflsr));
                    row["延保平均单台净收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbyybpjdtjsr));
                    row["导航升级业务个数"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='导航升级客户数'")["合计"]));
                    row["导航升级业务平均单台收入"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.SHbydhsjywpjdtsr));

                    #endregion

                    #region 销售数据

                    tblDays = new List<DataTable>();
                    tblKeys = new List<DataTable>();
                    monthtargets = new List<MonthlyTargetInfo>();
                    dep = DayReportDep.销售部;
                    daytemp = day1;
                    while (DateTime.Parse(daytemp.ToString("yyyy-MM") + "-01") <= DateTime.Parse(day2.ToString("yyyy-MM") + "-01"))
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytemp.ToString("yyyyMM"),
                            CorporationID = corplist[i].ID,
                            DayReportDep = dep
                        };
                        query.OrderBy = " [DayUnique] ASC";
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, daytemp, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, daytemp, corplist[i].ID, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);
                        tblDays.Add(tblDay);
                        tblKeys.Add(tblKey);
                        if (monthtarget != null)
                            monthtargets.Add(monthtarget);
                        daytemp = daytemp.AddMonths(1);
                    }

                    row["免费保养的收入"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='免费保养总金额'")["合计"]));
                    row["续保目标渗透率"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='无忧延保渗透率'")["目标"]));
                    row["续保实际渗透率"] = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='无忧延保渗透率'")["实际"]));
                    row["续保总额"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='延保总金额'")["合计"]));
                    decimal ztjctsmb = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅交车台数'")["目标值"]));
                    decimal ztjcts = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅交车台数'")["合计"]));
                    row["免费保养目标渗透率"] = ztjctsmb > 0 ? (DataConvert.SafeDecimal(row["免费保养预算台次"]) / ztjctsmb).ToString() : string.Empty;
                    row["免费保养实际渗透率"] = ztjcts > 0 ? (DataConvert.SafeDecimal(row["免费保养实际台次"]) / ztjcts).ToString() : string.Empty;
                    row["免费保养总额"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='免费保养总金额'")["合计"]));


                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\售后数据分析汇总.xls"));
                newfile = string.Format(@"{0}{1}售后数据分析汇总.xls", day1.ToString("yyyy年M月"), DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "");
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new HSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                #endregion

                sheet.GetRow(0).GetCell(0).SetCellValue(day1.ToString("yyyy年M月") + (DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "") + "含税售后收入业绩汇总表     单位:   万元");

                int index = 3;
                foreach (DataRow drow in tblresult.Rows)
                {
                    sheet.GetRow(index).GetCell(0).SetCellValue(drow["公司"].ToString());
                    sheet.GetRow(index).GetCell(1).SetCellValue(DataConvert.SafeDouble(drow["预算台次"]));
                    sheet.GetRow(index).GetCell(2).SetCellValue(DataConvert.SafeDouble(drow["来厂台次"]));
                    sheet.GetRow(index).GetCell(3).SetCellValue(DataConvert.SafeDouble(drow["预算产值"]) / 10000);
                    sheet.GetRow(index).GetCell(4).SetCellValue(DataConvert.SafeDouble(drow["维修产值"]) / 10000);
                    sheet.GetRow(index).GetCell(7).SetCellValue(DataConvert.SafeDouble(drow["一般维修额"]) / 10000);
                    sheet.GetRow(index).GetCell(8).SetCellValue(DataConvert.SafeDouble(drow["首保索赔额"]) / 10000);
                    sheet.GetRow(index).GetCell(9).SetCellValue(DataConvert.SafeDouble(drow["事故车收入"]) / 10000);
                    sheet.GetRow(index).GetCell(10).SetCellValue(DataConvert.SafeDouble(drow["免费保养的收入"]) / 10000);
                    sheet.GetRow(index).GetCell(11).SetCellValue(DataConvert.SafeDouble(drow["养护产品的收入"]) / 10000);
                    sheet.GetRow(index).GetCell(12).SetCellValue(DataConvert.SafeDouble(drow["他牌车收入"]) / 10000);
                    sheet.GetRow(index).GetCell(13).SetCellValue(DataConvert.SafeDouble(drow["其中事故车台次"]));
                    sheet.GetRow(index).GetCell(15).SetCellValue(DataConvert.SafeDouble(drow["维修毛利额"]) / 10000);
                    sheet.GetRow(index).GetCell(16).SetCellValue(DataConvert.SafeDouble(drow["维修毛利率"]) / 100);
                    sheet.GetRow(index).GetCell(17).SetCellValue(DataConvert.SafeDouble(drow["废旧收入"]) / 10000);
                    sheet.GetRow(index).GetCell(19).SetCellValue(DataConvert.SafeDouble(drow["含废旧的维修实际毛利率"]) / 100);
                    sheet.GetRow(index).GetCell(20).SetCellValue(DataConvert.SafeDouble(drow["含废旧的维修预算毛利率"]) / 100);
                    sheet.GetRow(33 + index).GetCell(0).SetCellValue(drow["公司"].ToString());
                    sheet.GetRow(33 + index).GetCell(1).SetCellValue(DataConvert.SafeDouble(drow["油漆收入"]) / 10000);
                    sheet.GetRow(33 + index).GetCell(2).SetCellValue(DataConvert.SafeDouble(drow["油漆成本"]) / 10000);
                    sheet.GetRow(33 + index).GetCell(4).SetCellValue(DataConvert.SafeDouble(drow["占产值收入比例目标"]));
                    sheet.GetRow(33 + index).GetCell(6).SetCellValue(DataConvert.SafeDouble(drow["其中养护产品毛利率"]));
                    sheet.GetRow(33 + index).GetCell(7).SetCellValue(DataConvert.SafeDouble(drow["标准库存金额"]) / 10000);
                    sheet.GetRow(33 + index).GetCell(8).SetCellValue(DataConvert.SafeDouble(drow["期末实际库存额"]) / 10000);
                    sheet.GetRow(33 + index).GetCell(9).SetCellValue(DataConvert.SafeDouble(drow["其中一年以上库存额"]) / 10000);
                    sheet.GetRow(33 + index).GetCell(11).SetCellValue(DataConvert.SafeDouble(drow["标准的库存度"]));
                    sheet.GetRow(33 + index).GetCell(12).SetCellValue(DataConvert.SafeDouble(drow["实际库存度"]));
                    sheet.GetRow(33 + index).GetCell(14).SetCellValue(DataConvert.SafeDouble(drow["配件毛利率"]) / 100);
                    sheet.GetRow(33 + index).GetCell(15).SetCellValue(DataConvert.SafeDouble(drow["事故车毛利率"]) / 100);
                    sheet.GetRow(33 + index).GetCell(16).SetCellValue(DataConvert.SafeDouble(drow["一般维修毛利率"]) / 100);
                    sheet.GetRow(33 + index).GetCell(17).SetCellValue(DataConvert.SafeDouble(drow["他牌车维修毛利率"]) / 100);
                    sheet.GetRow(66 + index).GetCell(0).SetCellValue(drow["公司"].ToString());
                    sheet.GetRow(66 + index).GetCell(1).SetCellValue(DataConvert.SafeDouble(drow["续保预算台次"]));
                    sheet.GetRow(66 + index).GetCell(2).SetCellValue(DataConvert.SafeDouble(drow["续保实际台次"]));
                    sheet.GetRow(66 + index).GetCell(3).SetCellValue(DataConvert.SafeDouble(drow["续保目标渗透率"]) / 100);
                    sheet.GetRow(66 + index).GetCell(6).SetCellValue(DataConvert.SafeDouble(drow["续保总额"]) / 10000);
                    sheet.GetRow(66 + index).GetCell(7).SetCellValue(DataConvert.SafeDouble(drow["续保返利收入"]) / 10000);
                    sheet.GetRow(66 + index).GetCell(8).SetCellValue(DataConvert.SafeDouble(drow["续保平均单台净收入"]) / 10000);
                    sheet.GetRow(66 + index).GetCell(9).SetCellValue(DataConvert.SafeDouble(drow["免费保养预算台次"]));
                    sheet.GetRow(66 + index).GetCell(10).SetCellValue(DataConvert.SafeDouble(drow["免费保养实际台次"]));
                    sheet.GetRow(66 + index).GetCell(11).SetCellValue(DataConvert.SafeDouble(drow["免费保养目标渗透率"]) / 100);
                    sheet.GetRow(66 + index).GetCell(12).SetCellValue(DataConvert.SafeDouble(drow["免费保养实际渗透率"]) / 100);
                    sheet.GetRow(66 + index).GetCell(14).SetCellValue(DataConvert.SafeDouble(drow["免费保养总额"]) / 10000);
                    sheet.GetRow(66 + index).GetCell(15).SetCellValue(DataConvert.SafeDouble(drow["延保返利收入"]) / 10000);
                    sheet.GetRow(66 + index).GetCell(16).SetCellValue(DataConvert.SafeDouble(drow["延保平均单台净收入"]) / 10000);
                    sheet.GetRow(66 + index).GetCell(17).SetCellValue(DataConvert.SafeDouble(drow["导航升级业务个数"]));
                    sheet.GetRow(66 + index).GetCell(18).SetCellValue(DataConvert.SafeDouble(drow["导航升级业务平均单台收入"]) / 10000);

                    index++;
                }
                for (int i = index; i < 33; i++)
                {
                    sheet.GetRow(i).ZeroHeight = true;
                    sheet.GetRow(i + 33).ZeroHeight = true;
                    sheet.GetRow(i + 66).ZeroHeight = true;
                }

                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        protected void btnJPYearGather_Click(object sender, EventArgs e)
        {
            DateTime day1 = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day1) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("公司");
                tblresult.Columns.Add("预算毛利率");
                tblresult.Columns.Add("展厅按预算月度产值");
                tblresult.Columns.Add("展厅实际精品产值");
                tblresult.Columns.Add("展厅实际的毛利额");
                tblresult.Columns.Add("展厅实际精品台次");
                tblresult.Columns.Add("展厅销量");
                tblresult.Columns.Add("展厅单台目标");
                tblresult.Columns.Add("网点按预算月度产值");
                tblresult.Columns.Add("网点实际精品产值");
                tblresult.Columns.Add("网点精品毛利额");
                tblresult.Columns.Add("网点精品台次");
                tblresult.Columns.Add("网点销售量");
                tblresult.Columns.Add("网点目标单台产值");
                tblresult.Columns.Add("售后按预算月度产值");
                tblresult.Columns.Add("售后实际精品产值");
                tblresult.Columns.Add("售后精品毛利额");
                tblresult.Columns.Add("售后精品台次");
                tblresult.Columns.Add("售后来厂台次");
                tblresult.Columns.Add("售后预算单台产值");
                tblresult.Columns.Add("期末库存数");
                tblresult.Columns.Add("3个月以上滞销库存");
                tblresult.Columns.Add("1年以上滞销库存");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    List<DataTable> tblDays = new List<DataTable>();
                    List<DataTable> tblKeys = new List<DataTable>();
                    List<MonthlyTargetInfo> monthtargets = new List<MonthlyTargetInfo>();

                    decimal ysjpmle = 0, ysjpcz; //预算精品毛利额,预算精品产值
                    decimal ysztdccz = 0, yswddtcz = 0, ysshdtcz = 0; //预算展厅单车产值,预算网点单台产值,预算售后单台产值
                    decimal ysztxl = 0, yswdxl = 0, ysshlctc = 0; //预算展厅销量，预算网点销量，预算售后來厂台次

                    #region 精品数据

                    DayReportDep dep = DayReportDep.精品部;
                    DateTime daytemp = day1;
                    while (DateTime.Parse(daytemp.ToString("yyyy-MM") + "-01") <= DateTime.Parse(day2.ToString("yyyy-MM") + "-01"))
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytemp.ToString("yyyyMM"),
                            CorporationID = corplist[i].ID,
                            DayReportDep = dep
                        };
                        query.OrderBy = " [DayUnique] ASC";
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, daytemp, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, daytemp, corplist[i].ID, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);
                        tblDays.Add(tblDay);
                        tblKeys.Add(tblKey);
                        if (monthtarget != null)
                            monthtargets.Add(monthtarget);
                        daytemp = daytemp.AddMonths(1);

                    }

                    row["公司"] = corplist[i].Name;

                    decimal ysztjpmle = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPysztjpmle)); //预算展厅精品毛利额
                    decimal yswdjpmle = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPyswdjpmle)); //预算网点精品毛利额
                    decimal ysshjpmle = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPysshjpmle)); //预算售后精品毛利额
                    ysjpmle = ysztjpmle + yswdjpmle + ysshjpmle; //预算精品毛利额

                    ysztdccz = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPysztdccz)); //预算展厅单车产值
                    yswddtcz = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPyswddtcz)); //预算网点单台产值
                    ysshdtcz = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPysshdtcz)); //预算售后单台产值
                    //展厅实际精品产值
                    row["展厅实际精品产值"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPztsjjpcz));

                    //展厅实际的毛利额
                    row["展厅实际的毛利额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPztsjmle));

                    //展厅单台目标
                    row["展厅单台目标"] = ysztdccz;

                    //网点精品毛利额
                    row["网点精品毛利额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPwdjpmle)); //网点精品毛利额

                    //网点目标单台产值
                    row["网点目标单台产值"] = yswddtcz;


                    //售后精品毛利额
                    row["售后精品毛利额"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPshjpmle)); //售后精品毛利额

                    //售后精品台次
                    row["售后精品台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='服务部安装台次'")["合计"]));

                    //售后预算单台产值
                    row["售后预算单台产值"] = ysshdtcz;

                    //期末库存数
                    row["期末库存数"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPqmkcs));

                    //其中：3个月以上滞销库存
                    row["3个月以上滞销库存"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPsgyyszxkc));

                    //1年以上滞销库存
                    row["1年以上滞销库存"] = monthtargets.Sum(m => DataConvert.SafeDecimal(m.JPynyszxkc));

                    #endregion

                    #region 销售数据

                    tblDays = new List<DataTable>();
                    tblKeys = new List<DataTable>();
                    monthtargets = new List<MonthlyTargetInfo>();
                    dep = DayReportDep.销售部;
                    daytemp = day1;
                    while (DateTime.Parse(daytemp.ToString("yyyy-MM") + "-01") <= DateTime.Parse(day2.ToString("yyyy-MM") + "-01"))
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytemp.ToString("yyyyMM"),
                            CorporationID = corplist[i].ID,
                            DayReportDep = dep
                        };
                        query.OrderBy = " [DayUnique] ASC";
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, daytemp, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, daytemp, corplist[i].ID, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);
                        tblDays.Add(tblDay);
                        tblKeys.Add(tblKey);
                        if (monthtarget != null)
                            monthtargets.Add(monthtarget);
                        daytemp = daytemp.AddMonths(1);

                    }

                    //展厅销量
                    ysztxl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅交车台数'")["目标值"]));
                    row["展厅销量"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅交车台数'")["合计"]));

                    //网点销量
                    yswdxl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网销售台次'")["目标值"]));
                    row["网点销售量"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网销售台次'")["合计"]));

                    //展厅实际精品台次
                    row["展厅实际精品台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='展厅成交精品台次'")["合计"]));

                    //网点实际精品产值
                    row["网点实际精品产值"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网精品金额'")["合计"]));

                    //网点精品台次
                    row["网点精品台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='二网精品台次'")["合计"]));

                    #endregion

                    #region 售后数据

                    tblDays = new List<DataTable>();
                    tblKeys = new List<DataTable>();
                    monthtargets = new List<MonthlyTargetInfo>();
                    dep = DayReportDep.售后部;
                    daytemp = day1;
                    while (DateTime.Parse(daytemp.ToString("yyyy-MM") + "-01") <= DateTime.Parse(day2.ToString("yyyy-MM") + "-01"))
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytemp.ToString("yyyyMM"),
                            CorporationID = corplist[i].ID,
                            DayReportDep = dep
                        };
                        query.OrderBy = " [DayUnique] ASC";
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, daytemp, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, daytemp, corplist[i].ID, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);
                        tblDays.Add(tblDay);
                        tblKeys.Add(tblKey);
                        if (monthtarget != null)
                            monthtargets.Add(monthtarget);
                        daytemp = daytemp.AddMonths(1);
                    }

                    //售后來厂台次
                    ysshlctc = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='来厂台次'")["目标值"]));
                    row["售后来厂台次"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='来厂台次'")["合计"]));

                    //售后实际精品产值
                    row["售后实际精品产值"] = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='精品美容产值'")["合计"]));

                    #endregion


                    //预算毛利率
                    ysjpcz = ysztxl * ysztdccz + yswdxl * yswddtcz + ysshlctc * ysshdtcz; //预算精品产值
                    row["预算毛利率"] = ysjpcz == 0 ? string.Empty : (ysjpmle / ysjpcz).ToString();

                    //展厅按预算月度产值
                    row["展厅按预算月度产值"] = ysztxl * ysztdccz;
                    //网点按预算月度产值
                    row["网点按预算月度产值"] = yswdxl * yswddtcz;
                    //售后按预算月度产值
                    row["售后按预算月度产值"] = ysshlctc * ysshdtcz;

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\精品数据分析汇总.xls"));
                newfile = string.Format(@"{0}{1}精品数据分析汇总.xls", day1.ToString("yyyy年M月"), DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "");
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new HSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                #endregion

                sheet.GetRow(0).GetCell(0).SetCellValue(day1.ToString("yyyy年M月") + (DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "") + "精品数据分析表");

                int index = 3;
                foreach (DataRow drow in tblresult.Rows)
                {
                    sheet.GetRow(2).GetCell(index).SetCellValue(drow["公司"].ToString());
                    sheet.GetRow(9).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["预算毛利率"]));
                    sheet.GetRow(11).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅按预算月度产值"]) / 10000);
                    sheet.GetRow(13).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅实际精品产值"]) / 10000);
                    sheet.GetRow(14).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅实际的毛利额"]) / 10000);
                    sheet.GetRow(16).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅实际精品台次"]));
                    sheet.GetRow(17).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅销量"]));
                    sheet.GetRow(18).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["展厅单台目标"]) / 10000);
                    sheet.GetRow(22).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["网点按预算月度产值"]) / 10000);
                    sheet.GetRow(24).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["网点实际精品产值"]) / 10000);
                    sheet.GetRow(25).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["网点精品毛利额"]) / 10000);
                    sheet.GetRow(27).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["网点精品台次"]));
                    sheet.GetRow(28).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["网点销售量"]));
                    sheet.GetRow(29).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["网点目标单台产值"]) / 10000);
                    sheet.GetRow(33).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["售后按预算月度产值"]) / 10000);
                    sheet.GetRow(35).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["售后实际精品产值"]) / 10000);
                    sheet.GetRow(36).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["售后精品毛利额"]) / 10000);
                    sheet.GetRow(38).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["售后精品台次"]));
                    sheet.GetRow(42).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["售后来厂台次"]));
                    sheet.GetRow(43).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["售后预算单台产值"]) / 10000);
                    sheet.GetRow(44).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["期末库存数"]));
                    sheet.GetRow(45).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["3个月以上滞销库存"]));
                    sheet.GetRow(46).GetCell(index).SetCellValue(DataConvert.SafeDouble(drow["1年以上滞销库存"]));

                    index++;
                }
                for (int i = index; i < 33; i++)
                {
                    sheet.SetColumnHidden(i, true);
                }

                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        protected void btnNXCPDataGather_Click(object sender, EventArgs e)
        {
            DateTime day1 = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day1) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("公司");
                tblresult.Columns.Add("机油套餐购买个数目标");
                tblresult.Columns.Add("机油套餐购买个数实际");
                tblresult.Columns.Add("机油套餐购买金额目标");
                tblresult.Columns.Add("机油套餐购买金额实际");
                tblresult.Columns.Add("机油套餐销售渗透率目标");
                tblresult.Columns.Add("机油套餐销售渗透率实际");
                tblresult.Columns.Add("机油套餐售后渗透率目标");
                tblresult.Columns.Add("机油套餐售后渗透率实际");
                tblresult.Columns.Add("机油套餐个数使用率目标");
                tblresult.Columns.Add("机油套餐个数使用率实际");
                tblresult.Columns.Add("机油套餐金额使用率目标");
                tblresult.Columns.Add("机油套餐金额使用率实际");

                tblresult.Columns.Add("玻璃无忧服务购买个数目标");
                tblresult.Columns.Add("玻璃无忧服务购买个数实际");
                tblresult.Columns.Add("玻璃无忧服务购买金额目标");
                tblresult.Columns.Add("玻璃无忧服务购买金额实际");
                tblresult.Columns.Add("玻璃无忧服务销售渗透率目标");
                tblresult.Columns.Add("玻璃无忧服务销售渗透率实际");
                tblresult.Columns.Add("玻璃无忧服务续保渗透率目标");
                tblresult.Columns.Add("玻璃无忧服务续保渗透率实际");
                tblresult.Columns.Add("玻璃无忧服务个数赔付率目标");
                tblresult.Columns.Add("玻璃无忧服务个数赔付率实际");
                tblresult.Columns.Add("玻璃无忧服务金额赔付率目标");
                tblresult.Columns.Add("玻璃无忧服务金额赔付率实际");

                tblresult.Columns.Add("划痕无忧服务购买个数目标");
                tblresult.Columns.Add("划痕无忧服务购买个数实际");
                tblresult.Columns.Add("划痕无忧服务购买金额目标");
                tblresult.Columns.Add("划痕无忧服务购买金额实际");
                tblresult.Columns.Add("划痕无忧服务销售渗透率目标");
                tblresult.Columns.Add("划痕无忧服务销售渗透率实际");
                tblresult.Columns.Add("划痕无忧服务续保渗透率目标");
                tblresult.Columns.Add("划痕无忧服务续保渗透率实际");
                tblresult.Columns.Add("划痕无忧服务个数赔付率目标");
                tblresult.Columns.Add("划痕无忧服务个数赔付率实际");
                tblresult.Columns.Add("划痕无忧服务金额赔付率目标");
                tblresult.Columns.Add("划痕无忧服务金额赔付率实际");

                tblresult.Columns.Add("延保无忧车服务购买个数目标");
                tblresult.Columns.Add("延保无忧车服务购买个数实际");
                tblresult.Columns.Add("延保无忧车服务购买金额目标");
                tblresult.Columns.Add("延保无忧车服务购买金额实际");
                tblresult.Columns.Add("延保无忧车服务自主购买个数目标");
                tblresult.Columns.Add("延保无忧车服务自主购买个数实际");
                tblresult.Columns.Add("延保无忧车服务自主购买金额目标");
                tblresult.Columns.Add("延保无忧车服务自主购买金额实际");
                tblresult.Columns.Add("延保无忧车服务厂家购买个数目标");
                tblresult.Columns.Add("延保无忧车服务厂家购买个数实际");
                tblresult.Columns.Add("延保无忧车服务厂家购买金额目标");
                tblresult.Columns.Add("延保无忧车服务厂家购买金额实际");
                tblresult.Columns.Add("延保无忧车服务销售渗透率目标");
                tblresult.Columns.Add("延保无忧车服务销售渗透率实际");
                tblresult.Columns.Add("延保无忧车服务售后渗透率目标");
                tblresult.Columns.Add("延保无忧车服务售后渗透率实际");
                tblresult.Columns.Add("延保无忧车服务个数赔付率目标");
                tblresult.Columns.Add("延保无忧车服务个数赔付率实际");
                tblresult.Columns.Add("延保无忧车服务金额赔付率目标");
                tblresult.Columns.Add("延保无忧车服务金额赔付率实际");

                #endregion

                List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
                string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
                for (int i = 0; i < corplist.Count; i++)
                {
                    DataRow row = tblresult.NewRow();

                    List<DataTable> tblDays = new List<DataTable>();
                    List<DataTable> tblKeys = new List<DataTable>();
                    List<MonthlyTargetInfo>  monthtargets = new List<MonthlyTargetInfo>();
                    DayReportDep dep = DayReportDep.无忧产品;
                    DateTime daytemp = day1;

                    #region 无忧产品数据

                    while (DateTime.Parse(daytemp.ToString("yyyy-MM") + "-01") <= DateTime.Parse(day2.ToString("yyyy-MM") + "-01"))
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytemp.ToString("yyyyMM"),
                            CorporationID = corplist[i].ID,
                            DayReportDep = dep
                        };
                        query.OrderBy = " [DayUnique] ASC";
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(corplist[i].ID, dep, daytemp, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, daytemp, corplist[i].ID, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay, corplist[i].ID);
                        tblDays.Add(tblDay);
                        tblKeys.Add(tblKey);
                        if (monthtarget != null)
                            monthtargets.Add(monthtarget);
                        daytemp = daytemp.AddMonths(1);
                    }

                    decimal mbxcztxl = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='新车展厅销量'")["目标"]));
                    decimal sjxcztxl = tblKeys.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "关键指标='新车展厅销量'")["实际"]));
                    decimal mbnbcdl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新保出单量'")["目标值"]));
                    decimal hjnbcdl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新保出单量'")["合计"]));
                    decimal mbxbcdl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保出单量'")["目标值"]));
                    decimal hjxbcdl = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保出单量'")["合计"]));
                    decimal mbdylcjpcls18m = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数≤18个月'")["目标值"]));
                    decimal hjdylcjpcls18m = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数≤18个月'")["合计"]));
                    decimal mbdylcjpcls1y = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数≤1年'")["目标值"]));
                    decimal hjdylcjpcls1y = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数≤1年'")["合计"]));
                    decimal mbdylcjpcls1y_2y = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数＞1年≤2年'")["目标值"]));
                    decimal hjdylcjpcls1y_2y = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数＞1年≤2年'")["合计"]));
                    decimal mbdylcjpcls2y_3y = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数＞2年≤3年'")["目标值"]));
                    decimal hjdylcjpcls2y_3y = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='当月来厂基盘车辆数＞2年≤3年'")["合计"]));
                    decimal mbdylcjpcls3y = mbdylcjpcls1y +mbdylcjpcls1y_2y+mbdylcjpcls2y_3y;
                    decimal hjdylcjpcls3y = hjdylcjpcls1y + hjdylcjpcls1y_2y + hjdylcjpcls2y_3y;

                    row["公司"] = corplist[i].Name;
                    //机油套餐
                    decimal mbxcjytcgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车机油套餐购买个数'")["目标值"]));
                    decimal hjxcjytcgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车机油套餐购买个数'")["合计"]));
                    decimal mbxcjytcgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车机油套餐购买金额'")["目标值"]));
                    decimal hjxcjytcgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车机油套餐购买金额'")["合计"]));
                    decimal mbshjytcgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后机油套餐购买个数'")["目标值"]));
                    decimal hjshjytcgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后机油套餐购买个数'")["合计"]));
                    decimal mbshjytcgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后机油套餐购买金额'")["目标值"]));
                    decimal hjshjytcgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后机油套餐购买金额'")["合计"]));
                    decimal mbshjytcyygmkhs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后机油套餐原已购买客户数'")["目标值"]));
                    decimal hjshjytcyygmkhs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后机油套餐原已购买客户数'")["合计"]));
                    decimal hjsycls = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyjytclcsycls)); //机油套餐使用数
                    decimal hjsyje = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyjytcsyje)); //机油套餐使用金额
                    row["机油套餐购买个数目标"] = mbxcjytcgmgs + mbshjytcgmgs;
                    row["机油套餐购买个数实际"] = hjxcjytcgmgs + hjshjytcgmgs;
                    row["机油套餐购买金额目标"] = mbxcjytcgmje + mbshjytcgmgs;
                    row["机油套餐购买金额实际"] = hjxcjytcgmje + hjshjytcgmje;
                    row["机油套餐销售渗透率目标"] = mbxcztxl == 0 ? string.Empty : Math.Round(mbxcjytcgmgs / mbxcztxl, 2).ToString();
                    row["机油套餐销售渗透率实际"] = sjxcztxl == 0 ? string.Empty : Math.Round(hjxcjytcgmgs / sjxcztxl, 2).ToString();
                    row["机油套餐售后渗透率目标"] = (mbdylcjpcls3y - mbshjytcyygmkhs) == 0 ? string.Empty : Math.Round(mbshjytcgmgs / (mbdylcjpcls3y - mbshjytcyygmkhs), 2).ToString();
                    row["机油套餐售后渗透率实际"] = (hjdylcjpcls3y - hjshjytcyygmkhs) == 0 ? string.Empty : Math.Round(hjshjytcgmgs / (hjdylcjpcls3y - hjshjytcyygmkhs), 2).ToString();
                    row["机油套餐个数使用率目标"] = string.Empty;
                    row["机油套餐个数使用率实际"] = (hjxcjytcgmgs + hjshjytcgmgs) == 0 ? string.Empty : Math.Round(hjsycls / (hjxcjytcgmgs + hjshjytcgmgs), 2).ToString();
                    row["机油套餐金额使用率目标"] = string.Empty;
                    row["机油套餐金额使用率实际"] = (hjxcjytcgmje + hjshjytcgmje) == 0 ? string.Empty : Math.Round(hjsyje / (hjxcjytcgmje + hjshjytcgmje), 2).ToString();
                    //玻璃无忧服务
                    decimal mbxcblwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车玻璃无忧服务购买个数'")["目标值"]));
                    decimal hjxcblwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车玻璃无忧服务购买个数'")["合计"]));
                    decimal mbxbblwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保玻璃无忧服务购买个数'")["目标值"]));
                    decimal hjxbblwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保玻璃无忧服务购买个数'")["合计"]));
                    decimal mbxcblwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车玻璃无忧服务购买金额'")["目标值"]));
                    decimal hjxcblwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车玻璃无忧服务购买金额'")["合计"]));
                    decimal mbxbblwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保玻璃无忧服务赔付金额'")["目标值"]));
                    decimal hjxbblwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保玻璃无忧服务赔付金额'")["合计"]));
                    decimal hjbldqcls = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyblwyfwdqgs)); //到期车辆数
                    decimal hjbldqje = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyblwyfwdqje)); //到期金额
                    decimal hjbldqpfs = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyblwyfwdqnpfgs)); //到期赔付车辆数
                    decimal hjbldqpfje = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyblwyfwdqnpfje)); //到期赔付金额
                    row["玻璃无忧服务购买个数目标"] = mbxcblwyfwgmgs + mbxbblwyfwgmgs;
                    row["玻璃无忧服务购买个数实际"] = hjxcblwyfwgmgs + hjxbblwyfwgmgs;
                    row["玻璃无忧服务购买金额目标"] = mbxcblwyfwgmje + mbxbblwyfwgmje;
                    row["玻璃无忧服务购买金额实际"] = hjxcblwyfwgmje + hjxbblwyfwgmje;
                    row["玻璃无忧服务销售渗透率目标"] = mbnbcdl == 0 ? string.Empty : Math.Round(mbxcblwyfwgmgs / mbnbcdl, 2).ToString();
                    row["玻璃无忧服务销售渗透率实际"] = hjnbcdl == 0 ? string.Empty : Math.Round(hjxcblwyfwgmgs / hjnbcdl, 2).ToString();
                    row["玻璃无忧服务续保渗透率目标"] = mbxbcdl == 0 ? string.Empty : Math.Round(mbxbblwyfwgmgs / mbxbcdl, 2).ToString();
                    row["玻璃无忧服务续保渗透率实际"] = hjxbcdl == 0 ? string.Empty : Math.Round(hjxbblwyfwgmgs / hjxbcdl, 2).ToString();
                    row["玻璃无忧服务个数赔付率目标"] = string.Empty;
                    row["玻璃无忧服务个数赔付率实际"] = hjbldqcls == 0 ? string.Empty : Math.Round(hjbldqpfs / hjbldqcls, 2).ToString();
                    row["玻璃无忧服务金额赔付率目标"] = string.Empty;
                    row["玻璃无忧服务金额赔付率实际"] = hjbldqje == 0 ? string.Empty : Math.Round(hjbldqpfje / hjbldqje, 2).ToString();
                    //划痕无忧服务
                    decimal mbxchhwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车划痕无忧服务购买个数'")["目标值"]));
                    decimal hjxchhwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车划痕无忧服务购买个数'")["合计"]));
                    decimal mbxbhhwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保划痕无忧服务购买个数'")["目标值"]));
                    decimal hjxbhhwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保划痕无忧服务购买个数'")["合计"]));
                    decimal mbxchhwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车划痕无忧服务购买金额'")["目标值"]));
                    decimal hjxchhwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车划痕无忧服务购买金额'")["合计"]));
                    decimal mbxbhhwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保划痕无忧服务赔付金额'")["目标值"]));
                    decimal hjxbhhwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='续保划痕无忧服务赔付金额'")["合计"]));
                    decimal hjhhdqcls = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyhhwyfwdqgs)); //到期车辆数
                    decimal hjhhdqje = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyhhwyfwdqje)); //到期金额
                    decimal hjhhdqpfs = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyhhwyfwdqnpfgs)); //到期赔付车辆数
                    decimal hjhhdqpfje = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyhhwyfwdqnpfje)); //到期赔付金额
                    row["划痕无忧服务购买个数目标"] = mbxchhwyfwgmgs + mbxbhhwyfwgmgs;
                    row["划痕无忧服务购买个数实际"] = hjxchhwyfwgmgs + hjxbhhwyfwgmgs;
                    row["划痕无忧服务购买金额目标"] = mbxchhwyfwgmje + mbxbhhwyfwgmje;
                    row["划痕无忧服务购买金额实际"] = hjxchhwyfwgmje + hjxbhhwyfwgmje;
                    row["划痕无忧服务销售渗透率目标"] = mbnbcdl == 0 ? string.Empty : Math.Round(mbxchhwyfwgmgs / mbnbcdl, 2).ToString();
                    row["划痕无忧服务销售渗透率实际"] = hjnbcdl == 0 ? string.Empty : Math.Round(hjxchhwyfwgmgs / hjnbcdl, 2).ToString();
                    row["划痕无忧服务续保渗透率目标"] = mbxbcdl == 0 ? string.Empty : Math.Round(mbxbhhwyfwgmgs / mbxbcdl, 2).ToString();
                    row["划痕无忧服务续保渗透率实际"] = hjxbcdl == 0 ? string.Empty : Math.Round(hjxbhhwyfwgmgs / hjxbcdl, 2).ToString();
                    row["划痕无忧服务个数赔付率目标"] = string.Empty;
                    row["划痕无忧服务个数赔付率实际"] = hjhhdqcls == 0 ? string.Empty : Math.Round(hjhhdqpfs / hjhhdqcls, 2).ToString();
                    row["划痕无忧服务金额赔付率目标"] = string.Empty;
                    row["划痕无忧服务金额赔付率实际"] = hjhhdqje == 0 ? string.Empty : Math.Round(hjhhdqpfje / hjhhdqje, 2).ToString();
                    //延保无忧车服务
                    decimal mbxcybwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车延保服务购买个数'")["目标值"]));
                    decimal hjxcybwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车延保服务购买个数'")["合计"]));
                    decimal mbxcybwyfwzzgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车自主延保服务购买个数'")["目标值"]));
                    decimal hjxcybwyfwzzgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车自主延保服务购买个数'")["合计"]));
                    decimal mbxcybwyfwcjgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车厂家延保服务购买个数'")["目标值"]));
                    decimal hjxcybwyfwcjgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车厂家延保服务购买个数'")["合计"]));
                    decimal mbxbybwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务购买个数'")["目标值"]));
                    decimal hjxbybwyfwgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务购买个数'")["合计"]));
                    decimal mbxbybwyfwzzgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务自主购买个数'")["目标值"]));
                    decimal hjxbybwyfwzzgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务自主购买个数'")["合计"]));
                    decimal mbxbybwyfwcjgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务厂家购买个数'")["目标值"]));
                    decimal hjxbybwyfwcjgmgs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务厂家购买个数'")["合计"]));
                    decimal mbxcybwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车延保服务购买金额'")["目标值"]));
                    decimal hjxcybwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车延保服务购买金额'")["合计"]));
                    decimal mbxcybwyfwzzgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车自主延保服务购买金额'")["目标值"]));
                    decimal hjxcybwyfwzzgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车自主延保服务购买金额'")["合计"]));
                    decimal mbxcybwyfwcjgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车厂家延保服务购买金额'")["目标值"]));
                    decimal hjxcybwyfwcjgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='新车厂家延保服务购买金额'")["合计"]));
                    decimal mbxbybwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务购买金额'")["目标值"]));
                    decimal hjxbybwyfwgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务购买金额'")["合计"]));
                    decimal mbxbybwyfwzzgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务自主购买金额'")["目标值"]));
                    decimal hjxbybwyfwzzgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务自主购买金额'")["合计"]));
                    decimal mbxbybwyfwcjgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务厂家购买金额'")["目标值"]));
                    decimal hjxbybwyfwcjgmje = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务厂家购买金额'")["合计"]));
                    decimal mbshybfwygmkhs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务已购买客户数'")["目标值"]));
                    decimal hjshybfwygmkhs = tblDays.Sum(t => DataConvert.SafeDecimal(GetRowView(t, "项目='售后延保服务已购买客户数'")["合计"]));
                    decimal hjybdqcls = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyybfwdqgs)); //到期车辆数
                    decimal hjybdqje = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyybfwdqje)); //到期金额
                    decimal hjybdqpfs = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyybfwdqnpfgs)); //到期赔付车辆数
                    decimal hjybdqpfje = monthtargets.Sum(m => DataConvert.SafeDecimal(m.NXCPbyybfwdqnpfje)); //到期赔付金额
                    row["延保无忧车服务购买个数目标"] = mbxcybwyfwgmgs + mbxbybwyfwgmgs;
                    row["延保无忧车服务购买个数实际"] = hjxcybwyfwgmgs + hjxbybwyfwgmgs;
                    row["延保无忧车服务购买金额目标"] = mbxcybwyfwgmje + mbxbybwyfwgmje;
                    row["延保无忧车服务购买金额实际"] = hjxcybwyfwgmje + hjxbybwyfwgmje;
                    row["延保无忧车服务自主购买个数目标"] = mbxcybwyfwzzgmgs + mbxbybwyfwzzgmgs;
                    row["延保无忧车服务自主购买个数实际"] = hjxcybwyfwzzgmgs + hjxbybwyfwzzgmgs;
                    row["延保无忧车服务自主购买金额目标"] = mbxcybwyfwzzgmje + mbxbybwyfwzzgmje;
                    row["延保无忧车服务自主购买金额实际"] = hjxcybwyfwzzgmje + hjxbybwyfwzzgmje;
                    row["延保无忧车服务厂家购买个数目标"] = mbxcybwyfwcjgmgs + mbxbybwyfwcjgmgs;
                    row["延保无忧车服务厂家购买个数实际"] = hjxcybwyfwcjgmgs + hjxbybwyfwcjgmgs;
                    row["延保无忧车服务厂家购买金额目标"] = mbxcybwyfwcjgmje + mbxbybwyfwcjgmje;
                    row["延保无忧车服务厂家购买金额实际"] = hjxcybwyfwcjgmje + hjxbybwyfwcjgmje;
                    row["延保无忧车服务销售渗透率目标"] = mbnbcdl == 0 ? string.Empty : Math.Round(mbxcybwyfwgmgs / mbnbcdl, 2).ToString();
                    row["延保无忧车服务销售渗透率实际"] = hjnbcdl == 0 ? string.Empty : Math.Round(hjxcybwyfwgmgs / hjnbcdl, 2).ToString();
                    row["延保无忧车服务售后渗透率目标"] = (mbdylcjpcls18m - mbshybfwygmkhs) == 0 ? string.Empty : Math.Round(mbxbybwyfwgmgs / (mbdylcjpcls18m - mbshybfwygmkhs), 2).ToString();
                    row["延保无忧车服务售后渗透率实际"] = (hjdylcjpcls18m - hjshybfwygmkhs) == 0 ? string.Empty : Math.Round(hjxbybwyfwgmgs / (hjdylcjpcls18m - hjshybfwygmkhs), 2).ToString();
                    row["延保无忧车服务个数赔付率目标"] = string.Empty;
                    row["延保无忧车服务个数赔付率实际"] = hjybdqcls == 0 ? string.Empty : Math.Round(hjybdqpfs / hjybdqcls, 2).ToString();
                    row["延保无忧车服务金额赔付率目标"] = string.Empty;
                    row["延保无忧车服务金额赔付率实际"] = hjybdqje == 0 ? string.Empty : Math.Round(hjybdqpfje / hjybdqje, 2).ToString();
                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\无忧产品数据分析汇总.xls"));
                newfile = string.Format(@"{0}{1}无忧产品数据分析汇总.xls", day1.ToString("yyyy年M月"), DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "");
                using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    workbook = new HSSFWorkbook(file);
                }
                sheet = workbook.GetSheetAt(0);

                #region 颜色

                IFont fontblack = workbook.CreateFont();
                fontblack.Color = HSSFColor.Black.Index;

                ICellStyle cellStyleBlack = workbook.CreateCellStyle();
                cellStyleBlack.SetFont(fontblack);
                cellStyleBlack.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleBlack.TopBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.RightBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleBlack.LeftBorderColor = HSSFColor.Black.Index;

                ICellStyle cellStyleGreen = workbook.CreateCellStyle();
                cellStyleGreen.SetFont(fontblack);
                cellStyleGreen.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleGreen.TopBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.RightBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
                cellStyleGreen.FillPattern = FillPattern.SolidForeground;

                ICellStyle cellStyleYellow = workbook.CreateCellStyle();
                cellStyleYellow.SetFont(fontblack);
                cellStyleYellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyleYellow.TopBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.RightBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.BottomBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.LeftBorderColor = HSSFColor.Black.Index;
                cellStyleYellow.FillForegroundColor = HSSFColor.Yellow.Index;
                cellStyleYellow.FillPattern = FillPattern.SolidForeground;

                #endregion

                sheet.GetRow(0).GetCell(0).SetCellValue(day1.ToString("yyyy年M月") + (DataConvert.SafeInt(day1.ToString("yyyyMM")) < DataConvert.SafeInt(day2.ToString("yyyyMM")) ? ("至" + day2.ToString("yyyy年M月")) : "") + "无忧产品数据分析");

                int index = 5;
                foreach (DataRow drow in tblresult.Rows)
                {
                    sheet.GetRow(1).GetCell(index).SetCellValue(drow["公司"].ToString());
                    for(int i = 0;i< 28;i++)
                    {
                        sheet.GetRow(3 * i + 2).GetCell(index).SetCellValue(DataConvert.SafeFloat(drow[2 * i + 1]));
                        sheet.GetRow(3 * i + 3).GetCell(index).SetCellValue(DataConvert.SafeFloat(drow[2 * i + 2]));
                    }

                    index++;
                }
                for (int i = index; i < 35; i++)
                {
                    sheet.SetColumnHidden(i, true);
                }

                sheet.ForceFormulaRecalculation = true;
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    workbook.Write(ms);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(newfile, Encoding.UTF8).ToString() + "");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                    workbook = null;
                }
            }
        }

        /// <summary>
        /// 检查用户是否有权限
        /// </summary>
        /// <returns></returns>
        private bool CheckUser()
        {
            string[] deppowers = CurrentUser.DayReportViewDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deppowers.Contains(((int)CurrentDep).ToString()))
                return false;
            if (CurrentUser.ReportGather != "1")
                return false;

            return true;
        }

        protected string GetDepHide(DayReportDep dep)
        {
            string result = string.Empty;

            string[] deparray = CurrentUser.DayReportViewDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deparray.Contains(((int)dep).ToString()))
                result = "style=\"display:none;\"";

            return result;
        }

        private string GetMoneyStr(float money, bool showall = false)
        {
            string result = string.Empty;

            if (!showall)
            {
                if (Math.Abs(money) > 10000)
                    result = Math.Round(money / 10000, 1).ToString() + "万";
                else
                    result = Math.Round(money, 1).ToString();
            }
            else
            {
                result = FormatNum(money.ToString());
            }

            return result;
        }

        private string FormatNum(string num)
        {
            string newstr = string.Empty;
            Regex r = new Regex(@"(-?\d+?)(\d{3})*(\.\d+|$)");
            Match m = r.Match(num);
            newstr += m.Groups[1].Value;
            for (int i = 0; i < m.Groups[2].Captures.Count; i++)
            {
                newstr += "," + m.Groups[2].Captures[i].Value;
            }
            newstr += m.Groups[3].Value;
            return newstr;
        }

        protected string SetDayReportCorp(string id)
        {
            string result = string.Empty;

            string[] corps = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (corps.Contains(id))
                result = "checked=\"checked\"";

            return result;
        }

        private string GetCellValue(string v, bool ispersent)
        {
            string result = string.Empty;

            result = string.IsNullOrEmpty(v) ? string.Empty : string.Format("{0}{1}", v, ispersent ? "%" : string.Empty);

            return result;
        }

        private DataRowView GetRowView(DataTable t, string filter)
        {
            t.DefaultView.RowFilter = filter;
            return t.DefaultView[0];
        }

        #region 导出Excel

        public void TableToExcel()
        {
            string tableHtml = GetString("html");    //接受前台table 数值字符串
            string filename = GetString("fn");
            if (string.IsNullOrEmpty(tableHtml.Trim()))
            {
                spMsg.InnerText = "请先生成汇总信息";
                return;
            }

            InitializeWorkbook();
            HSSFSheet sheet1 = (HSSFSheet)hssfworkbook.CreateSheet("Sheet1");

            string rowContent = string.Empty;
            MatchCollection rowCollection = Regex.Matches(tableHtml, @"<tr[^>]*>[\s\S]*?<\/tr>",
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture); //对tr进行筛选

            NPOI.SS.UserModel.IFont fontSubTitle = hssfworkbook.CreateFont();
            fontSubTitle.Boldweight = 800;//加粗

            NPOI.SS.UserModel.IFont fontBody = hssfworkbook.CreateFont();
            fontBody.Boldweight = 500;//加粗

            int colspancount = 0;
            Regex reg_colspan = new Regex("colspan=\"?(\\d)\"?", RegexOptions.IgnoreCase);
            List<string> emptyrecord = new List<string>();
            for (int i = 0; i < rowCollection.Count; i++)
            {
                colspancount = 0;
                HSSFRow row = (HSSFRow)sheet1.CreateRow(i);
                rowContent = rowCollection[i].Value;

                MatchCollection columnCollection = Regex.Matches(rowContent, @"<td[^>]*>([\s\S]*?)<\/td>", RegexOptions.IgnoreCase); //对td进行筛选
                for (int td = 0; td < columnCollection.Count; td++)
                {
                    int colspan = 0;
                    if (reg_colspan.IsMatch(columnCollection[td].Groups[0].Value))
                        colspan = DataConvert.SafeInt(reg_colspan.Match(columnCollection[td].Groups[0].Value).Groups[1].Value) - 1;
                    int colindex = td + colspancount + (i == 1 && FirstCellRowCount == 2 ? (TowRowCount > 0 ? TowRowCount : 1) : 0);
                    ICell cell = row.CreateCell(colindex);
                    cell.SetCellValue(columnCollection[td].Groups[1].Value == "&nbsp;" ? string.Empty : columnCollection[td].Groups[1].Value);

                    colspancount += colspan;
                }
            }
            if (FirstCellRowCount == 2)
            {
                if (TowRowCount > 0)
                {
                    for (int i = 0; i < TowRowCount; i++)
                    {
                        CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 1, i, i);
                        sheet1.AddMergedRegion(cellRangeAddress);
                    }
                }
                else
                {
                    CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 1, 0, 0);
                    sheet1.AddMergedRegion(cellRangeAddress);
                }
            }
            colspancount = 0;
            rowContent = rowCollection[0].Value;
            MatchCollection _columnCollection = Regex.Matches(rowContent, @"<td[^>]*>([\s\S]*?)<\/td>", RegexOptions.IgnoreCase); //对td进行筛选
            for (int td = 0; td < _columnCollection.Count; td++)
            {
                int colspan = 0;
                if (reg_colspan.IsMatch(_columnCollection[td].Groups[0].Value))
                {
                    colspan = DataConvert.SafeInt(reg_colspan.Match(_columnCollection[td].Groups[0].Value).Groups[1].Value) - 1;
                }
                CellRangeAddress _cellRangeAddress = new CellRangeAddress(0, 0, td + colspancount, td + colspancount + colspan);
                sheet1.AddMergedRegion(_cellRangeAddress);

                sheet1.SetColumnWidth(td + colspancount, ((td + colspancount) == 0 ? 35 : 9) * 256);

                colspancount += colspan;
            }

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                hssfworkbook.Write(ms);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, Encoding.UTF8).ToString() + ".xls");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                hssfworkbook = null;
            }
        }

        static HSSFWorkbook hssfworkbook;

        public void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();
            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "红旭集团";
            hssfworkbook.DocumentSummaryInformation = dsi;
            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "xxx";
            hssfworkbook.SummaryInformation = si;
        }

        #endregion
    }
}