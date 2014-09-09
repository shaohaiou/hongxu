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
            corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
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

            DateTime day = DateTime.Today;
            DateTime day2 = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day) && DateTime.TryParse(txtDate2.Text, out day2))
            {
                spTitle.InnerText = string.Format("{0}年{1}月{2}关键指标汇总", day.Year, day.Month, CurrentDep.ToString().Replace("部", string.Empty));
                if (CurrentDep == DayReportDep.行政部)
                    spTitle.InnerText = string.Format("{0}年{1}月{2}关键指标汇总", day.Year, day.Month, "人事");

                DataTable tbl = GetKeyReportMul(CurrentDep, day);
                tdData.InnerHtml = GetKeyReportStr(CurrentDep, tbl);
            }
        }

        private DataTable GetReportMul(DayReportDep dep, DateTime day, DateTime day2)
        {
            DataTable tblresult = new DataTable();

            tblresult.Columns.Add("公司");

            List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
            string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
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
                row[0] = corplist[i].Name;
                for (int j = 0; j < tbl.Rows.Count; j++)
                {
                    if (CurrentDep == DayReportDep.财务部)
                        row[j + 1] = tbl.Rows[j]["期初余额"].ToString() + "|" + tbl.Rows[j]["合计"].ToString();
                    else
                        row[j + 1] = tbl.Rows[j]["目标值"].ToString() + "|" + tbl.Rows[j]["合计"].ToString();
                }
                tblresult.Rows.Add(row);
            }

            return tblresult;
        }

        private string GetReportStr(DayReportDep dep, DataTable tbl)
        {
            StringBuilder strb = new StringBuilder();
            string tbwidth = "6000px";
            switch (dep)
            {
                case DayReportDep.销售部:
                    tbwidth = "6300px";
                    break;
                case DayReportDep.售后部:
                    tbwidth = "7200px";
                    break;
                case DayReportDep.市场部:
                    tbwidth = "5300px";
                    break;
                case DayReportDep.财务部:
                    tbwidth = "6100px";
                    break;
                case DayReportDep.行政部:
                    tbwidth = "1800px";
                    break;
                case DayReportDep.精品部:
                    tbwidth = "2900px";
                    break;
                case DayReportDep.客服部:
                    tbwidth = "3000px";
                    break;
                case DayReportDep.二手车部:
                    tbwidth = "5800px";
                    break;
                case DayReportDep.金融部:
                    tbwidth = "7700px";
                    break;
                case DayReportDep.DCC部:
                    tbwidth = "7400px";
                    break;
                default:
                    break;
            }
            tblView.Width = tbwidth;

            #region 页面输出
            Regex reg = new Regex(@"\d+");

            strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
            strb.Append("<tr class=\"bold tc bggray\">");
            strb.Append("<td class=\"w240\" rowspan=\"2\">公司</td>");
            for (int i = 1; i < tbl.Columns.Count; i++)
            {
                strb.AppendFormat("<td class=\"w160\" colspan=\"2\">{0}</td>", reg.Replace(tbl.Columns[i].ToString(), string.Empty));
            }
            strb.Append("<td></td>");
            strb.Append("</tr>");
            strb.Append("<tr class=\"bold tc bggray\">");
            for (int i = 1; i < tbl.Columns.Count; i++)
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
                for (int j = 1; j < tbl.Columns.Count; j++)
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

            if (dep == DayReportDep.销售部 || dep == DayReportDep.售后部 || dep == DayReportDep.客服部 || dep == DayReportDep.精品部 || dep == DayReportDep.金融部)
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

                #region 每日数据

                for (int i = 1; i <= days; i++)
                {
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            for (int j = 0; j < rlist.Count; j++)
                            {
                                rows[j][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
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
            else if (dep == DayReportDep.二手车部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(corpid),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(corpid), DayReportDep.销售部, day, true);
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
                    CorporationID = DataConvert.SafeInt(corpid),
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(corpid), DayReportDep.售后部, day, true);
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

                #region 每日数据

                for (int i = 1; i <= days; i++)
                {
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            DailyReportModuleInfo m = rlist.Find(l => l.Name == "销售推荐评估台次");
                            rows[2][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                            m = rlist.Find(l => l.Name == "实际评估台次");
                            rows[3][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                            for (int j = 2; j < rlist.Count; j++)
                            {
                                rows[j + 3][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                            }
                        }
                    }
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_xs.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "展厅交车台数");
                        DailyReportInfo r = list_xs.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            rows[0][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                        }
                    }
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_xs.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "展厅首次来客批次");
                        DailyReportInfo r = list_xs.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            rows[1][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                        }
                    }
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_sh.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportModuleInfo m = rlist_sh.Find(l => l.Name == "来厂台次");
                        DailyReportInfo r = list_sh.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            rows[4][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
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
            else if (dep == DayReportDep.市场部)
            {
                #region 表数据

                DataRow[] rows = new DataRow[rlist.Count];

                #region 项目、合计、目标

                //rows[0] = tbl.NewRow();
                //rows[0]["项目"] = "新增总线索量";
                //rows[0]["合计"] = string.Empty;
                //rows[0]["目标值"] = monthtarget == null ? string.Empty : monthtarget.SCxzzxsl;

                //rows[1] = tbl.NewRow();
                //rows[1]["项目"] = "新增有效线索量";
                //rows[1]["合计"] = string.Empty;
                //rows[1]["目标值"] = monthtarget == null ? string.Empty : monthtarget.SCxzyxxsl;

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

                #region 每日数据

                for (int i = 1; i <= days; i++)
                {
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            for (int j = 0; j < rlist.Count; j++)
                            {
                                rows[j][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                            }
                            //string[] slistzxsl = new string[] { "展厅首次到访记录数", "新增网络后台线索数", "新增呼入数", "触点", "活动新增潜客数", "新增网络400呼入数" };
                            //string[] slistyxxsl = new string[] { "首访建档数", "新增网络后台线索留档数", "新增呼入建档数" };
                            //int zxsl = 0;
                            //int yxxsl = 0;
                            //foreach (string s in slistzxsl)
                            //{
                            //    zxsl += !rlist.Exists(n => n.Name == s) ? 0 : (reportdate.ContainsKey(rlist.Find(n => n.Name == s).ID.ToString()) ? DataConvert.SafeInt(reportdate[rlist.Find(n => n.Name == s).ID.ToString()]) : 0);
                            //}
                            //foreach (string s in slistyxxsl)
                            //{
                            //    yxxsl += !rlist.Exists(n => n.Name == s) ? 0 : (reportdate.ContainsKey(rlist.Find(n => n.Name == s).ID.ToString()) ? DataConvert.SafeInt(reportdate[rlist.Find(n => n.Name == s).ID.ToString()]) : 0);
                            //}

                            //rows[0][i] = zxsl;
                            //rows[0]["合计"] = DataConvert.SafeInt(rows[0]["合计"]) + DataConvert.SafeInt(rows[0][i]);

                            //rows[1][i] = yxxsl;
                            //rows[1]["合计"] = DataConvert.SafeInt(rows[1]["合计"]) + DataConvert.SafeInt(rows[1][i]);
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
            else if (dep == DayReportDep.DCC部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(corpid),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> xslist = DailyReports.Instance.GetList(query, true);
                MonthlyTargetInfo xsmonthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(corpid), DayReportDep.销售部, day, true);
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

                #region 项目、合计、目标

                #region 新增线索建档总数

                int idxzztqtldjds = rlist.Find(l => l.Name == "新增展厅前台来电建档数").ID;
                int idxzwlhtxslds = rlist.Find(l => l.Name == "新增网络后台线索留档数").ID;
                int idxzwl400hrjds = rlist.Find(l => l.Name == "新增网络400呼入建档数").ID;
                int idzjsxss = rlist.Find(l => l.Name == "转介绍线索数").ID;
                int idcd = rlist.Find(l => l.Name == "触点").ID;
                int idcrmxdxss = rlist.Find(l => l.Name == "CRM下达线索数").ID;
                int idztdrzr = rlist.Find(l => l.Name == "展厅当日转入").ID;
                int idztsydrzr = rlist.Find(l => l.Name == "展厅上月当日转入").ID;
                int idztshydrzr = rlist.Find(l => l.Name == "展厅双月当日转入").ID;
                int idztsyyqzzr = rlist.Find(l => l.Name == "展厅三月有强制转入").ID;
                int idzxwlhtxszcs = rlist.Find(l => l.Name == "新增网络后台线索转出数").ID;
                int idibhczl = rlist.Find(l => l.Name == "IB呼出总量").ID;
                int idobhczl = rlist.Find(l => l.Name == "OB呼出总量").ID;
                int idibyxhcs = rlist.FindAll(l => l.Name == "有效呼出数")[0].ID;
                int idobyxhcs = rlist.FindAll(l => l.Name == "有效呼出数")[1].ID;

                decimal hjxzztqtldjds = Math.Round(data.Sum(d => d.ContainsKey(idxzztqtldjds.ToString()) ? DataConvert.SafeDecimal(d[idxzztqtldjds.ToString()]) : 0), 0);
                decimal hjxzwlhtxslds = Math.Round(data.Sum(d => d.ContainsKey(idxzwlhtxslds.ToString()) ? DataConvert.SafeDecimal(d[idxzwlhtxslds.ToString()]) : 0), 0);
                decimal hjxzwl400hrjds = Math.Round(data.Sum(d => d.ContainsKey(idxzwl400hrjds.ToString()) ? DataConvert.SafeDecimal(d[idxzwl400hrjds.ToString()]) : 0), 0);
                decimal hjzjsxss = Math.Round(data.Sum(d => d.ContainsKey(idzjsxss.ToString()) ? DataConvert.SafeDecimal(d[idzjsxss.ToString()]) : 0), 0);
                decimal hjcd = Math.Round(data.Sum(d => d.ContainsKey(idcd.ToString()) ? DataConvert.SafeDecimal(d[idcd.ToString()]) : 0), 0);
                decimal hjcrmxdxss = Math.Round(data.Sum(d => d.ContainsKey(idcrmxdxss.ToString()) ? DataConvert.SafeDecimal(d[idcrmxdxss.ToString()]) : 0), 0);
                decimal hjztdrzr = Math.Round(data.Sum(d => d.ContainsKey(idztdrzr.ToString()) ? DataConvert.SafeDecimal(d[idztdrzr.ToString()]) : 0), 0);
                decimal hjztsydrzr = Math.Round(data.Sum(d => d.ContainsKey(idztshydrzr.ToString()) ? DataConvert.SafeDecimal(d[idztshydrzr.ToString()]) : 0), 0);
                decimal hjztshydrzr = Math.Round(data.Sum(d => d.ContainsKey(idztshydrzr.ToString()) ? DataConvert.SafeDecimal(d[idztshydrzr.ToString()]) : 0), 0);
                decimal hjztsyyqzzr = Math.Round(data.Sum(d => d.ContainsKey(idztsyyqzzr.ToString()) ? DataConvert.SafeDecimal(d[idztsyyqzzr.ToString()]) : 0), 0);
                decimal hjzxwlhtxszcs = Math.Round(data.Sum(d => d.ContainsKey(idzxwlhtxszcs.ToString()) ? DataConvert.SafeDecimal(d[idzxwlhtxszcs.ToString()]) : 0), 0);

                decimal hjibhczl = Math.Round(data.Sum(d => d.ContainsKey(idibhczl.ToString()) ? DataConvert.SafeDecimal(d[idibhczl.ToString()]) : 0), 0);
                decimal hjobhczl = Math.Round(data.Sum(d => d.ContainsKey(idobhczl.ToString()) ? DataConvert.SafeDecimal(d[idobhczl.ToString()]) : 0), 0);
                decimal hjibyxhcs = Math.Round(data.Sum(d => d.ContainsKey(idibyxhcs.ToString()) ? DataConvert.SafeDecimal(d[idibyxhcs.ToString()]) : 0), 0);
                decimal hjobyxhcs = Math.Round(data.Sum(d => d.ContainsKey(idobyxhcs.ToString()) ? DataConvert.SafeDecimal(d[idobyxhcs.ToString()]) : 0), 0);

                decimal mbxzztqtldjds = targetdata.ContainsKey(idxzztqtldjds.ToString()) ? DataConvert.SafeDecimal(targetdata[idxzztqtldjds.ToString()]) : 0;
                decimal mbxzwlhtxslds = targetdata.ContainsKey(idxzwlhtxslds.ToString()) ? DataConvert.SafeDecimal(targetdata[idxzwlhtxslds.ToString()]) : 0;
                decimal mbxzwl400hrjds = targetdata.ContainsKey(idxzwl400hrjds.ToString()) ? DataConvert.SafeDecimal(targetdata[idxzwl400hrjds.ToString()]) : 0;
                decimal mbzjsxss = targetdata.ContainsKey(idzjsxss.ToString()) ? DataConvert.SafeDecimal(targetdata[idzjsxss.ToString()]) : 0;
                decimal mbcd = targetdata.ContainsKey(idcd.ToString()) ? DataConvert.SafeDecimal(targetdata[idcd.ToString()]) : 0;
                decimal mbcrmxdxss = targetdata.ContainsKey(idcrmxdxss.ToString()) ? DataConvert.SafeDecimal(targetdata[idcrmxdxss.ToString()]) : 0;
                decimal mbztdrzr = targetdata.ContainsKey(idztdrzr.ToString()) ? DataConvert.SafeDecimal(targetdata[idztdrzr.ToString()]) : 0;
                decimal mbztsydrzr = targetdata.ContainsKey(idztsydrzr.ToString()) ? DataConvert.SafeDecimal(targetdata[idztsydrzr.ToString()]) : 0;
                decimal mbztshydrzr = targetdata.ContainsKey(idztshydrzr.ToString()) ? DataConvert.SafeDecimal(targetdata[idztshydrzr.ToString()]) : 0;
                decimal mbztsyyqzzr = targetdata.ContainsKey(idztsyyqzzr.ToString()) ? DataConvert.SafeDecimal(targetdata[idztsyyqzzr.ToString()]) : 0;
                decimal mbzxwlhtxszcs = targetdata.ContainsKey(idzxwlhtxszcs.ToString()) ? DataConvert.SafeDecimal(targetdata[idzxwlhtxszcs.ToString()]) : 0;

                decimal mbibhczl = targetdata.ContainsKey(idibhczl.ToString()) ? DataConvert.SafeDecimal(targetdata[idibhczl.ToString()]) : 0;
                decimal mbobhczl = targetdata.ContainsKey(idobhczl.ToString()) ? DataConvert.SafeDecimal(targetdata[idobhczl.ToString()]) : 0;
                decimal mbibyxhcs = targetdata.ContainsKey(idibyxhcs.ToString()) ? DataConvert.SafeDecimal(targetdata[idibyxhcs.ToString()]) : 0;
                decimal mbobyxhcs = targetdata.ContainsKey(idobyxhcs.ToString()) ? DataConvert.SafeDecimal(targetdata[idobyxhcs.ToString()]) : 0;

                rows[0] = tbl.NewRow();
                rows[0]["项目"] = "新增线索建档总数";
                rows[0]["合计"] = hjxzztqtldjds + hjxzwlhtxslds + hjxzwl400hrjds + hjzjsxss + hjcd + hjcrmxdxss + hjztdrzr + hjztsydrzr + hjztshydrzr + hjztsyyqzzr - hjzxwlhtxszcs;
                rows[0]["目标值"] = mbxzztqtldjds + mbxzwlhtxslds + mbxzwl400hrjds + mbzjsxss + mbcd + mbcrmxdxss + mbztdrzr + mbztsydrzr + mbztshydrzr + mbztsyyqzzr - mbzxwlhtxszcs;

                #endregion

                int indexibhczl = rlist.IndexOf(rlist.Find(l => l.Name == "IB呼出总量"));
                for (int i = 0; i < indexibhczl; i++)
                {
                    rows[i + 1] = tbl.NewRow();
                    rows[i + 1]["项目"] = rlist[i].Name;
                    rows[i + 1]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i + 1]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
                }

                rows[indexibhczl + 1] = tbl.NewRow();
                rows[indexibhczl + 1]["项目"] = "呼出总量";
                rows[indexibhczl + 1]["合计"] = hjibhczl + hjobhczl;
                rows[indexibhczl + 1]["目标值"] = mbibhczl + mbobhczl;

                rows[indexibhczl + 2] = tbl.NewRow();
                rows[indexibhczl + 2]["项目"] = "呼出有效数";
                rows[indexibhczl + 2]["合计"] = hjibyxhcs + hjobyxhcs;
                rows[indexibhczl + 2]["目标值"] = mbibyxhcs + mbobyxhcs;

                for (int i = indexibhczl; i < rlist.Count; i++)
                {
                    rows[i + 3] = tbl.NewRow();
                    rows[i + 3]["项目"] = rlist[i].Name;
                    rows[i + 3]["合计"] = !rlist[i].Iscount ? string.Empty : Math.Round(data.Sum(d => d.ContainsKey(rlist[i].ID.ToString()) ? DataConvert.SafeDecimal(d[rlist[i].ID.ToString()]) : 0), 0).ToString();
                    rows[i + 3]["目标值"] = targetdata.ContainsKey(rlist[i].ID.ToString()) ? targetdata[rlist[i].ID.ToString()] : string.Empty;
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

                #region 每日数据

                for (int i = 1; i <= days; i++)
                {

                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);

                            #region 新增线索建档总数

                            decimal drxzztqtldjds = DataConvert.SafeDecimal(reportdate.ContainsKey(idxzztqtldjds.ToString()) ? reportdate[idxzztqtldjds.ToString()] : string.Empty);
                            decimal drxzwlhtxslds = DataConvert.SafeDecimal(reportdate.ContainsKey(idxzwlhtxslds.ToString()) ? reportdate[idxzwlhtxslds.ToString()] : string.Empty);
                            decimal drxzwl400hrjds = DataConvert.SafeDecimal(reportdate.ContainsKey(idxzwl400hrjds.ToString()) ? reportdate[idxzwl400hrjds.ToString()] : string.Empty);
                            decimal drzjsxss = DataConvert.SafeDecimal(reportdate.ContainsKey(idzjsxss.ToString()) ? reportdate[idzjsxss.ToString()] : string.Empty);
                            decimal drcd = DataConvert.SafeDecimal(reportdate.ContainsKey(idcd.ToString()) ? reportdate[idcd.ToString()] : string.Empty);
                            decimal drcrmxdxss = DataConvert.SafeDecimal(reportdate.ContainsKey(idcrmxdxss.ToString()) ? reportdate[idcrmxdxss.ToString()] : string.Empty);
                            decimal drztdrzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztdrzr.ToString()) ? reportdate[idztdrzr.ToString()] : string.Empty);
                            decimal drztsydrzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztsydrzr.ToString()) ? reportdate[idztsydrzr.ToString()] : string.Empty);
                            decimal drztshydrzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztshydrzr.ToString()) ? reportdate[idztshydrzr.ToString()] : string.Empty);
                            decimal drztsyyqzzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztsyyqzzr.ToString()) ? reportdate[idztsyyqzzr.ToString()] : string.Empty);
                            decimal drzxwlhtxszcs = DataConvert.SafeDecimal(reportdate.ContainsKey(idzxwlhtxszcs.ToString()) ? reportdate[idzxwlhtxszcs.ToString()] : string.Empty);
                            decimal dribhczl = DataConvert.SafeDecimal(reportdate.ContainsKey(idibhczl.ToString()) ? reportdate[idibhczl.ToString()] : string.Empty);
                            decimal drobhczl = DataConvert.SafeDecimal(reportdate.ContainsKey(idobhczl.ToString()) ? reportdate[idobhczl.ToString()] : string.Empty);
                            decimal dribyxhcs = DataConvert.SafeDecimal(reportdate.ContainsKey(idibyxhcs.ToString()) ? reportdate[idibyxhcs.ToString()] : string.Empty);
                            decimal drobyxhcs = DataConvert.SafeDecimal(reportdate.ContainsKey(idobyxhcs.ToString()) ? reportdate[idobyxhcs.ToString()] : string.Empty);


                            rows[0][i] = drxzztqtldjds + drxzwlhtxslds + drxzwl400hrjds + drzjsxss + drcd + drcrmxdxss + drztdrzr + drztsydrzr + drztshydrzr + drztsyyqzzr - drzxwlhtxszcs;

                            #endregion

                            for (int j = 0; j < indexibhczl; j++)
                            {
                                rows[j + 1][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                            }
                            rows[indexibhczl + 1][i] = dribhczl + drobhczl;
                            rows[indexibhczl + 2][i] = dribyxhcs + drobyxhcs;
                            for (int j = indexibhczl; j < rlist.Count; j++)
                            {
                                rows[j + 3][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                            }
                        }
                    }
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && xslist.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportModuleInfo m = xsrlist.Find(l => l.Name == "展厅交车台数");
                        DailyReportInfo r = xslist.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            rows[rlist.Count + 3][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
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
            else if (dep == DayReportDep.财务部)
            {
                tbl.Columns["目标值"].ColumnName = "期初余额";
                DataRow[] rows = new DataRow[rlist.Count + 4];
                string[] listignore = { "集团内部资金借出", "其他支出", "POS未到帐", "银行帐户余额", "其中农行", "中行", "工行", "建行", "交行", "民生", "平安", "中信", "华夏", "浙商", "泰隆", "其他银行", "现金合计", "留存现金", "本日预计支付合计数", "其中：购车（配件）款", "工资", "税款", "其他大额款项" };

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
                            rows[index][i] = Math.Round(DataConvert.SafeDecimal(monthtarget == null ? string.Empty : monthtarget.CWzjye) + hjdrzjzsr - hjdrzjzzc, 2);
                            index++;

                            //POS未到帐、银行帐户余额、农行、中行、民生、交行、其他银行、现金合计、留存现金 每日数据
                            for (int k = 0; k < listignore.Length; k++)
                            {
                                if (listignore[k] != "集团内部资金借出" && listignore[k] != "其他支出")
                                {
                                    rows[index][i] = reportdate.ContainsKey(rlist.Find(l => l.Name == listignore[k]).ID.ToString()) ? reportdate[rlist.Find(l => l.Name == listignore[k]).ID.ToString()] : string.Empty; ;
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

                #region 每日数据

                for (int i = 1; i <= days; i++)
                {
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            for (int j = 0; j < rlist.Count; j++)
                            {
                                rows[j][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
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

            return tbl;
        }

        private DataTable GetKeyReportMul(DayReportDep dep, DateTime day)
        {
            DataTable tblresult = new DataTable();

            tblresult.Columns.Add("公司");

            List<CorporationInfo> corplist = Corporations.Instance.GetList(true);
            string[] corppower = hdnDayReportCorp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            corplist = corplist.FindAll(c => corppower.Contains(c.ID.ToString()));
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
                    row[j + 1] = tbl.Rows[j]["目标"].ToString() + (string.IsNullOrEmpty(tbl.Rows[j]["目标"].ToString()) ? string.Empty : fh) + "|" + tbl.Rows[j]["实际"].ToString() + (string.IsNullOrEmpty(tbl.Rows[j]["实际"].ToString()) ? string.Empty : fh);
                }
                tblresult.Rows.Add(row);
            }

            return tblresult;
        }

        private string GetKeyReportStr(DayReportDep dep, DataTable tbl)
        {
            StringBuilder strb = new StringBuilder();
            string tbwidth = "6000px";
            switch (dep)
            {
                case DayReportDep.销售部:
                    tbwidth = "4700px";
                    break;
                case DayReportDep.售后部:
                    tbwidth = "4800px";
                    break;
                case DayReportDep.市场部:
                    tbwidth = "3700px";
                    break;
                case DayReportDep.财务部:
                    tbwidth = "6100px";
                    break;
                case DayReportDep.行政部:
                    tbwidth = "1800px";
                    break;
                case DayReportDep.精品部:
                    tbwidth = "2900px";
                    break;
                case DayReportDep.客服部:
                    tbwidth = "3000px";
                    break;
                case DayReportDep.二手车部:
                    tbwidth = "2200px";
                    break;
                case DayReportDep.金融部:
                    tbwidth = "7700px";
                    break;
                case DayReportDep.DCC部:
                    tbwidth = "4800px";
                    break;
                default:
                    break;
            }
            tblView.Width = tbwidth;

            #region 页面输出
            Regex reg = new Regex(@"\d+");

            strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
            strb.Append("<tr class=\"bold tc bggray\">");
            strb.Append("<td class=\"w240\" rowspan=\"2\">公司</td>");
            for (int i = 1; i < tbl.Columns.Count; i++)
            {
                strb.AppendFormat("<td class=\"w160\" colspan=\"2\">{0}</td>", reg.Replace(tbl.Columns[i].ToString(), string.Empty));
            }
            strb.Append("<td></td>");
            strb.Append("</tr>");
            strb.Append("<tr class=\"bold tc bggray\">");
            for (int i = 1; i < tbl.Columns.Count; i++)
            {
                strb.Append("<td class=\"w80\">目标</td>");
                strb.Append("<td class=\"w80\">实际</td>");
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

                DataRow[] rows = new DataRow[28];

                data.DefaultView.RowFilter = "项目='展厅首次来客批次'";
                decimal hjztsclkpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='留档批次'";
                decimal hjldpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅订单台数'";
                decimal hjztddts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅交车台数'";
                decimal hjztjcts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztjcts = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='上牌台次'";
                decimal hjsptc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='上牌总金额'";
                decimal hjspzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbspzje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='美容交车台次'";
                decimal hjmrjctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='美容交车总金额'";
                decimal hjmrjczje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbmrjczje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='延保台次'";
                decimal hjybtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='延保总金额'";
                decimal hjybzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbybzje = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjybml = hjybzje * 30 / 100;
                decimal mbybml = mbybzje * 30 / 100;
                data.DefaultView.RowFilter = "项目='展厅成交精品台次'";
                decimal hjztcjjptc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
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
                data.DefaultView.RowFilter = "项目='二网保险返利'";
                decimal hjewbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbewbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='外流销售台次'";
                decimal hjwlxstc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='介绍二手车评估数'";
                decimal hjjsescpgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='销售置换数'";
                decimal mbxszhs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjxszhs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='按揭净收入'";
                decimal hjajjsr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbajjsr = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='本地按揭台次'";
                decimal hjbdajtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='银行按揭台次'";
                decimal hjyhajtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='厂家金融台次'";
                decimal hjcjjrtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅含DCC保险台次'";
                decimal hjbxtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅含DCC保险总金额'";
                decimal hjbxzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅含DCC保险返利'";
                decimal hjbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbbxfl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='入库台次'";
                decimal hjrktc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='免费保养台次（含赠送）'";
                decimal hjmfbytc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='免费保养总金额'";
                decimal hjmfbyzje = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "总销售台次";
                rows[0]["目标"] = mbztjcts + mbewxstc;
                rows[0]["实际"] = hjztjcts + hjewxstc;
                rows[0]["详细"] = string.Empty;

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "展厅占比";
                rows[1]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSztzb;
                rows[1]["实际"] = (hjztjcts + hjewxstc) == 0 ? string.Empty : Math.Round(hjztjcts * 100 / (hjztjcts + hjewxstc), 0).ToString();
                rows[1]["详细"] = (hjztjcts + hjewxstc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztjcts, 0), Math.Round(hjztjcts + hjewxstc, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "展厅留档率";
                rows[2]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSztldl;
                rows[2]["实际"] = hjztsclkpc == 0 ? string.Empty : Math.Round(hjldpc * 100 / hjztsclkpc, 0).ToString();
                rows[2]["详细"] = hjztsclkpc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjldpc, 0), Math.Round(hjztsclkpc, 0));

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "展厅成交率";
                rows[3]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSztcjl;
                rows[3]["实际"] = hjldpc == 0 ? string.Empty : Math.Round((hjztddts - ld) * 100 / hjldpc, 0).ToString();
                rows[3]["详细"] = hjldpc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztddts - ld, 0), Math.Round(hjldpc, 0));

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "上牌率";
                rows[4]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSspl;
                rows[4]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjsptc * 100 / hjztjcts, 0).ToString();
                rows[4]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsptc, 0), Math.Round(hjztjcts, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "上牌单台";
                rows[5]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSspdt;
                rows[5]["实际"] = hjsptc == 0 ? string.Empty : Math.Round(hjspzje / hjsptc, 0).ToString();
                rows[5]["详细"] = string.Empty;

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "展厅保险率";
                rows[6]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSbxl;
                rows[6]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjbxtc * 100 / hjztjcts, 0).ToString();
                rows[6]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjbxtc, 0), Math.Round(hjztjcts, 0));

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "展厅保险单台";
                rows[7]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSbxdt;
                rows[7]["实际"] = hjbxtc == 0 ? string.Empty : Math.Round(hjbxzje / hjbxtc, 0).ToString();
                rows[7]["详细"] = string.Empty;

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "美容交车率";
                rows[8]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSmrjcl;
                rows[8]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjmrjctc * 100 / hjztjcts, 0).ToString();
                rows[8]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjmrjctc, 0), Math.Round(hjztjcts, 0));

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "美容单台";
                rows[9]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSmrdt;
                rows[9]["实际"] = hjmrjctc == 0 ? string.Empty : Math.Round(hjmrjczje / hjmrjctc, 0).ToString();
                rows[9]["详细"] = string.Empty;

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "延保渗透率";
                rows[10]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSybstl;
                rows[10]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjybtc * 100 / hjztjcts, 0).ToString();
                rows[10]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjybtc, 0), Math.Round(hjztjcts, 0));

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "展厅精品前装率";
                rows[11]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSztjpqzl;
                rows[11]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjztcjjptc * 100 / hjztjcts, 0).ToString();
                rows[11]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztcjjptc, 0), Math.Round(hjztjcts, 0));

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "展厅精品平均单台";
                rows[12]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSztjpdt;
                rows[12]["实际"] = hjztcjjptc == 0 ? string.Empty : Math.Round(hjjpzje / hjztjcts, 0).ToString();
                rows[12]["详细"] = string.Empty;

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "二网精品平均单台";
                rows[13]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSewjpdt;
                rows[13]["实际"] = hjewxstc == 0 ? string.Empty : Math.Round(hjewjpje / hjewxstc, 0).ToString();
                rows[13]["详细"] = string.Empty;

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "销售置换台次";
                rows[14]["目标"] = mbxszhs;
                rows[14]["实际"] = hjxszhs;
                rows[14]["详细"] = string.Empty;

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "按揭率";
                rows[15]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSajl;
                rows[15]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round((hjbdajtc + hjyhajtc + hjcjjrtc) * 100 / hjztjcts, 0).ToString();
                rows[15]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjbdajtc + hjyhajtc + hjcjjrtc, 0), Math.Round(hjztjcts, 0));

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "按揭平均单台";
                rows[16]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSajpjdt;
                rows[16]["实际"] = (hjbdajtc + hjyhajtc + hjcjjrtc) == 0 ? string.Empty : Math.Round(hjajjsr / (hjbdajtc + hjyhajtc + hjcjjrtc), 0).ToString();
                rows[16]["详细"] = string.Empty;

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "免费保养渗透率";
                rows[17]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSmfbystl;
                rows[17]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjmfbytc * 100 / hjztjcts, 0).ToString();
                rows[17]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjmfbytc, 0), Math.Round(hjztjcts, 0));

                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "免费保养单台";
                rows[18]["目标"] = monthtarget == null ? string.Empty : monthtarget.XSmfbypjdt;
                rows[18]["实际"] = hjmfbytc == 0 ? string.Empty : Math.Round(hjmfbyzje / hjmfbytc, 0).ToString();
                rows[18]["详细"] = string.Empty;

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "标准在库库存";
                rows[19]["目标"] = monthtarget == null ? string.Empty : Math.Round((mbztjcts + mbewxstc) * DataConvert.SafeDecimal(monthtarget.XSzzts) / 30, 0).ToString();
                rows[19]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc;

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "库存利息";
                rows[20]["目标"] = monthtarget == null ? string.Empty : Math.Round(DataConvert.SafeInt(rows[19]["目标"]) * DataConvert.SafeDecimal(monthtarget.XSclpjdj) * (decimal)0.0075, 0).ToString();
                rows[20]["实际"] = monthtarget == null ? string.Empty : Math.Round(DataConvert.SafeInt(rows[19]["实际"]) * DataConvert.SafeDecimal(monthtarget.XSclpjdj) * (decimal)0.0075, 0).ToString();

                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "附加值合计";
                rows[21]["目标"] = mbspzje + mbmrjczje + mbajjsr + mbjpml + mbybml + mbewbxfl + mbbxfl;
                rows[21]["实际"] = hjspzje + hjmrjczje + hjajjsr + hjjpml + hjybml + hjewbxfl + hjbxfl;

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "在库平均单台成本价";
                rows[22]["目标"] = string.Empty;
                rows[22]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSclpjdj;

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "在库库存";
                rows[23]["目标"] = string.Empty;
                rows[23]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc;

                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "在途";
                rows[24]["目标"] = string.Empty;
                rows[24]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSztcl;

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "总库存";
                rows[25]["目标"] = string.Empty;
                rows[25]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc + DataConvert.SafeInt(monthtarget == null ? string.Empty : monthtarget.XSztcl);

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "上月留单";
                rows[26]["目标"] = string.Empty;
                rows[26]["实际"] = ld.ToString();

                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "本月留单";
                rows[27]["目标"] = string.Empty;
                rows[27]["实际"] = hjztddts - hjztjcts;

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

                #region 表数据

                DataRow[] rows = new DataRow[27];

                data.DefaultView.RowFilter = "项目='来厂台次'";
                decimal hjlctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='其中预约台次'";
                decimal hjqzyytc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='当日产值'";
                decimal hjdrcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='事故总产值'";
                decimal hjsgzcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsgzcz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='养护产值'";
                decimal hjyhcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='收到信息数'";
                decimal hjsdxxs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='成功台次'";
                decimal hjcgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='精品美容产值'";
                decimal mbjpmrcz = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                decimal hjjpmrcz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='索赔上报台次'";
                decimal hjspsbtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='索赔批准台次'";
                decimal hjsppztc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='延保台次'";
                decimal hjybtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='机电内返台次'";
                decimal hjjdnf = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbjdnf = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='钣喷内返台次'";
                decimal hjbpnf = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='外返台次'";
                decimal hjwftc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='缺件数量'";
                decimal hjqjsl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='意向数量'";
                decimal hjyxsl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='意向成功数'";
                decimal hjyxcgs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='保养台次'";
                decimal hjbytc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='保养产值'";
                decimal hjbycz = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
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

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "来厂台次";
                rows[0]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHlctc;
                rows[0]["实际"] = hjlctc;
                rows[0]["详细"] = string.Empty;

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "预约率";
                rows[1]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHyyl;
                rows[1]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjqzyytc * 100 / hjlctc, 2).ToString();
                rows[1]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjqzyytc, 0), Math.Round(hjlctc, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "产值达成";
                rows[2]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHczdc;
                rows[2]["实际"] = hjdrcz;
                rows[2]["详细"] = string.Empty;

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "养护比例";
                rows[3]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHyhbl;

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "美容比例";
                rows[4]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHmrbl;
                rows[4]["实际"] = hjdrcz == 0 ? string.Empty : Math.Round(hjjpmrcz * 100 / hjdrcz, 2).ToString();
                rows[4]["详细"] = hjdrcz == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjjpmrcz, 0), Math.Round(hjdrcz, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "单台产值";
                rows[5]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHdtcz;
                rows[5]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjdrcz / hjlctc, 2).ToString();
                rows[5]["详细"] = string.Empty;

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "事故产值占比";
                rows[6]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHsgczl;
                rows[6]["实际"] = hjdrcz == 0 ? string.Empty : Math.Round(hjsgzcz * 100 / hjdrcz, 2).ToString();
                rows[6]["详细"] = hjdrcz == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsgzcz, 0), Math.Round(hjdrcz, 0));

                rows[3]["实际"] = (hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz) == 0 ? string.Empty : Math.Round(hjyhcz * 100 / ((hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz)), 2).ToString();
                rows[3]["详细"] = (hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjyhcz, 0), Math.Round(hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz, 0));

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "事故首次成功率";
                rows[7]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHsgsccgl;
                rows[7]["实际"] = hjsdxxs == 0 ? string.Empty : Math.Round(hjcgtc * 100 / hjsdxxs, 2).ToString();
                rows[7]["详细"] = hjsdxxs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjcgtc, 0), Math.Round(hjsdxxs, 0));

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "事故再次成功率";
                rows[8]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHsgzccgl;
                rows[8]["实际"] = hjyxsl == 0 ? string.Empty : Math.Round(hjyxcgs * 100 / hjyxsl, 2).ToString();
                rows[8]["详细"] = hjyxsl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjyxcgs, 0), Math.Round(hjyxsl, 0));

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "索赔成功率";
                rows[9]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHspcgl;
                rows[9]["实际"] = hjspsbtc == 0 ? string.Empty : Math.Round(hjsppztc * 100 / hjspsbtc, 2).ToString();
                rows[9]["详细"] = hjspsbtc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsppztc, 0), Math.Round(hjspsbtc, 0));

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "内返率";
                rows[10]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHnfl;
                rows[10]["实际"] = hjlctc == 0 ? string.Empty : Math.Round((hjjdnf + hjbpnf + hjwftc) * 100 / hjlctc, 2).ToString();
                rows[10]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjjdnf + hjbpnf + hjwftc, 0), Math.Round(hjlctc, 0));

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "供货及时率";
                rows[11]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHghjsl;
                rows[11]["实际"] = hjlctc == 0 ? string.Empty : Math.Round((1 - hjqjsl / hjlctc) * 100, 2).ToString();
                rows[11]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjlctc - hjqjsl, 0), Math.Round(hjlctc, 0));

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "保养单台产值";
                rows[12]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHbydtcz;
                rows[12]["实际"] = hjbytc == 0 ? string.Empty : Math.Round(hjbycz / hjbytc, 2).ToString();
                rows[12]["详细"] = string.Empty;

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "保养台次占比";
                rows[13]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHbytczb;
                rows[13]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjbytc * 100 / hjlctc, 2).ToString();
                rows[13]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjbytc, 0), Math.Round(hjlctc, 0));

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "精品美容产值";
                rows[14]["目标"] = mbjpmrcz;
                rows[14]["实际"] = hjjpmrcz;
                rows[14]["详细"] = string.Empty;

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "延保达成";
                rows[15]["目标"] = monthtarget == null ? string.Empty : monthtarget.SHybdcl;
                rows[15]["实际"] = hjybtc;

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "事故总产值";
                rows[16]["目标"] = mbsgzcz;
                rows[16]["实际"] = hjsgzcz;

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "中保";
                rows[17]["目标"] = hjzblp.ToString();
                rows[17]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzblp * 100 / hjsgzcz, 2).ToString();
                rows[17]["详细"] = mbzblp.ToString();

                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "太保";
                rows[18]["目标"] = hjtb.ToString();
                rows[18]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjtb * 100 / hjsgzcz, 2).ToString();
                rows[18]["详细"] = mbtb.ToString();

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "平安";
                rows[19]["目标"] = hjpa.ToString();
                rows[19]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjpa * 100 / hjsgzcz, 2).ToString();
                rows[19]["详细"] = mbpa.ToString();

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "人寿";
                rows[20]["目标"] = hjrs.ToString();
                rows[20]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjrs * 100 / hjsgzcz, 2).ToString();
                rows[20]["详细"] = mbrs.ToString();

                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "大地";
                rows[21]["目标"] = hjdd.ToString();
                rows[21]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjdd * 100 / hjsgzcz, 2).ToString();
                rows[21]["详细"] = mbdd.ToString();

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "中华联合";
                rows[22]["目标"] = hjzhlh.ToString();
                rows[22]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzhlh * 100 / hjsgzcz, 2).ToString();
                rows[22]["详细"] = mbzhlh.ToString();

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "浙商";
                rows[23]["目标"] = hjzs.ToString();
                rows[23]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzs * 100 / hjsgzcz, 2).ToString();
                rows[23]["详细"] = mbzs.ToString();

                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "大众";
                rows[24]["目标"] = hjdz.ToString();
                rows[24]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjdz * 100 / hjsgzcz, 2).ToString();
                rows[24]["详细"] = mbdz.ToString();

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "其他";
                rows[25]["目标"] = hjqt.ToString();
                rows[25]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjqt * 100 / hjsgzcz, 2).ToString();
                rows[25]["详细"] = mbqt.ToString();

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "合计";
                rows[26]["目标"] = (hjzblp + hjtb + hjpa + hjrs + hjdd + hjzhlh + hjzs + hjdz + hjqt).ToString();
                rows[26]["实际"] = string.Empty;
                rows[26]["详细"] = (mbzblp + mbtb + mbpa + mbrs + mbdd + mbzhlh + mbzs + mbdz + mbqt).ToString();

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

                DataRow[] rows = new DataRow[21];

                data.DefaultView.RowFilter = "项目='新增总线索量'";
                decimal hjxzzxsl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzzxsl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅首次到访记录数'";
                decimal hjztscdfjls = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅首次到访建档数'";
                decimal hjsfjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsfjds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增网络后台线索留档数'";
                decimal hjxzwlhtxslds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzwlhtxslds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增网络后台线索数'";
                decimal hjxzwlhtxss = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增展厅前台呼入建档数'";
                decimal hjxzhrjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增网络400呼入建档数'";
                decimal hjxzwlhrjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增展厅前台呼入数'";
                decimal hjxzhrs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzhrs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='新增网络400呼入数'";
                decimal hjxzwlhrs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzwlhrs = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='触点'";
                decimal hjcd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbcd = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='活动新增潜客数'";
                decimal hjhdxzqks = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbhdxzqks = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅到店-网络渠道'";
                decimal hjwlqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅到店-户外广告渠道'";
                decimal hjhwggqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅到店-电台渠道'";
                decimal hjdtqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅到店-增换购渠道'";
                decimal hjzhgqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅到店-转介绍渠道'";
                decimal hjzjsqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅到店-自然到店'";
                decimal hjzrdd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增留档数-汽车之家'";
                decimal hjqczjxzlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增留档数-易车网'";
                decimal hjycwxzlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增留档数-太平洋'";
                decimal hjtpyxzlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增留档数-本地网络'";
                decimal hjbdwlxzlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增留档数-其他网络'";
                decimal hjqtwlxzlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "总线索达成率";
                rows[0]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCxsdcl;
                rows[0]["实际"] = DataConvert.SafeDecimal(monthtarget == null ? string.Empty : mbxzzxsl.ToString()) == 0 ? string.Empty : Math.Round(hjxzzxsl * 100 / mbxzzxsl, 2).ToString();
                rows[0]["详细"] = DataConvert.SafeDecimal(monthtarget == null ? string.Empty : mbxzzxsl.ToString()) == 0 ? string.Empty : string.Format("({0}/{1})", hjxzzxsl, mbxzzxsl);

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "首次到访达成率";
                rows[1]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCscdfdcl;
                rows[1]["实际"] = mbsfjds == 0 ? string.Empty : Math.Round(hjsfjds * 100 / mbsfjds, 2).ToString();
                rows[1]["详细"] = mbsfjds == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsfjds, 0), Math.Round(mbsfjds, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "首次到访建档率";
                rows[2]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCsfjdl;
                rows[2]["实际"] = hjztscdfjls == 0 ? string.Empty : Math.Round(hjsfjds * 100 / hjztscdfjls, 2).ToString();
                rows[2]["详细"] = hjztscdfjls == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsfjds, 0), Math.Round(hjztscdfjls, 0));

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "网络后台线索达成率";
                rows[3]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCwlhtxsdcl;
                rows[3]["实际"] = mbxzwlhtxslds == 0 ? string.Empty : Math.Round(hjxzwlhtxslds * 100 / mbxzwlhtxslds, 2).ToString();
                rows[3]["详细"] = mbxzwlhtxslds == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxzwlhtxslds, 0), Math.Round(mbxzwlhtxslds, 0));

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "网络线索建档率";
                rows[4]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCwlxsjdl;
                rows[4]["实际"] = hjxzwlhtxss == 0 ? string.Empty : Math.Round(hjxzwlhtxslds * 100 / hjxzwlhtxss, 2).ToString();
                rows[4]["详细"] = hjxzwlhtxss == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxzwlhtxslds, 0), Math.Round(hjxzwlhtxss, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "呼入电话达成率";
                rows[5]["目标"] = monthtarget == null ? string.Empty : monthtarget.SChrdhdcl;
                rows[5]["实际"] = (mbxzhrs + mbxzwlhrs) == 0 ? string.Empty : Math.Round((hjxzhrs + hjxzwlhrs) * 100 / (mbxzhrs + mbxzwlhrs), 2).ToString();
                rows[5]["详细"] = (mbxzhrs + mbxzwlhrs) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxzhrs + hjxzwlhrs, 0), Math.Round(mbxzhrs + mbxzwlhrs, 0));

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "呼入电话建档率";
                rows[6]["目标"] = monthtarget == null ? string.Empty : monthtarget.SChrdhjdl;
                rows[6]["实际"] = (hjxzwlhrs + hjxzhrs) == 0 ? string.Empty : Math.Round((hjxzhrjds + hjxzwlhrjds) * 100 / (hjxzwlhrs + hjxzhrs), 2).ToString();
                rows[6]["详细"] = (hjxzwlhrs + hjxzhrs) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxzhrjds + hjxzwlhrjds, 0), Math.Round(hjxzwlhrs + hjxzhrs, 0));

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "触点潜客达成率";
                rows[7]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCcdqkdcl;
                rows[7]["实际"] = mbcd == 0 ? string.Empty : Math.Round(hjcd * 100 / mbcd, 2).ToString();
                rows[7]["详细"] = mbcd == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjcd, 0), Math.Round(mbcd, 0));

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "活动集客达成率";
                rows[8]["目标"] = monthtarget == null ? string.Empty : monthtarget.SChdjkdcl;
                rows[8]["实际"] = mbhdxzqks == 0 ? string.Empty : Math.Round(hjhdxzqks * 100 / mbhdxzqks, 2).ToString();
                rows[8]["详细"] = mbhdxzqks == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjhdxzqks, 0), Math.Round(mbhdxzqks, 0));

                decimal ddCount = hjwlqddd + hjhwggqddd + hjdtqddd + hjzhgqddd + hjzjsqddd + hjzrdd;

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "网络";
                rows[9]["目标"] = string.Empty;
                rows[9]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjwlqddd * 100 / ddCount, 2).ToString();

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "户外广告";
                rows[10]["目标"] = string.Empty;
                rows[10]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjhwggqddd * 100 / ddCount, 2).ToString();

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "电台";
                rows[11]["目标"] = string.Empty;
                rows[11]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjdtqddd * 100 / ddCount, 2).ToString();

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "增换购";
                rows[12]["目标"] = string.Empty;
                rows[12]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzhgqddd * 100 / ddCount, 2).ToString();

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "转介绍";
                rows[13]["目标"] = string.Empty;
                rows[13]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzjsqddd * 100 / ddCount, 2).ToString();

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "自然";
                rows[14]["目标"] = string.Empty;
                rows[14]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzrdd * 100 / ddCount, 2).ToString();

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "其他";
                rows[15]["目标"] = string.Empty;
                rows[15]["实际"] = string.Empty;

                decimal wlxsCount = hjqczjxzlds + hjycwxzlds + hjtpyxzlds + hjbdwlxzlds + hjqtwlxzlds;

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "汽车之家";
                rows[16]["目标"] = string.Empty;
                rows[16]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqczjxzlds * 100 / wlxsCount, 2).ToString();

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "易车网";
                rows[17]["目标"] = string.Empty;
                rows[17]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjycwxzlds * 100 / wlxsCount, 2).ToString();

                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "太平洋汽车";
                rows[18]["目标"] = string.Empty;
                rows[18]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjtpyxzlds * 100 / wlxsCount, 2).ToString();

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "本地网络";
                rows[19]["目标"] = string.Empty;
                rows[19]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjbdwlxzlds * 100 / wlxsCount, 2).ToString();

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "其他网络";
                rows[20]["目标"] = string.Empty;
                rows[20]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqtwlxzlds * 100 / wlxsCount, 2).ToString();

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

                DataRow[] rows = new DataRow[27];

                data.DefaultView.RowFilter = "项目='新增线索建档总数'";
                decimal hjxzxsjdzs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增展厅前台来电数'";
                decimal hjztsclds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增展厅前台来电建档数'";
                decimal hjztsdjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增网络后台线索数'";
                decimal hjwlhtxzxss = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增网络后台线索留档数'";
                decimal hjwlhtxzxslds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增网络400呼入数'";
                decimal hjzxwl400hrs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='新增网络400呼入建档数'";
                decimal hjhrxzjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='呼出总量'";
                decimal hjhczl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='呼出有效数'";
                decimal hjhcyxs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='首次邀约到店客户总数'";
                decimal hjscyyddkhzs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='首次邀约到店-网络潜客'";
                decimal hjscwlqkyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='首次邀约到店-销售转入潜客'";
                decimal hjscxszrqkyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='首次邀约到店-展厅台前呼入潜客'";
                decimal hjschrqkyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='再邀到店约数'";
                decimal hjzyys = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                data.DefaultView.RowFilter = "项目='呼入建档数-汽车之家'";
                decimal hjqczjhr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='呼入建档数-易车网'";
                decimal hjycwhr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='呼入建档数-太平洋'";
                decimal hjtpyhr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='呼入建档数-本地网络'";
                decimal hjbdwlhr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='呼入建档数-其他网络'";
                decimal hjqtwlhr = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交数-汽车之家'";
                decimal hjqczjcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交数-易车网'";
                decimal hjycwcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交数-太平洋'";
                decimal hjtpycjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交数-本地网络'";
                decimal hjbdwlcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交数-转介绍'";
                decimal hjzjscjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交数-展厅转入'";
                decimal hjztzrcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交数-其他渠道'";
                decimal hjqtqdcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC成交总台数'";
                decimal hjcjzts = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                data.DefaultView.RowFilter = "项目='展厅销量'";
                decimal hjztxl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "销量占展厅比";
                rows[0]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCCztzb;
                rows[0]["实际"] = hjztxl == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjztxl, 2).ToString();
                rows[0]["详细"] = hjztxl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjcjzts, 0), Math.Round(hjztxl, 0));

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "前台首电建档率";
                rows[1]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCCsdjdl;
                rows[1]["实际"] = hjztsclds == 0 ? string.Empty : Math.Round(hjztsdjds * 100 / hjztsclds, 2).ToString();
                rows[1]["详细"] = hjztsclds == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztsdjds, 0), Math.Round(hjztsclds, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "网络后台建档率";
                rows[2]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCCwlhtjdl;
                rows[2]["实际"] = hjwlhtxzxss == 0 ? string.Empty : Math.Round(hjwlhtxzxslds * 100 / hjwlhtxzxss, 2).ToString();
                rows[2]["详细"] = hjwlhtxzxss == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjwlhtxzxslds, 0), Math.Round(hjwlhtxzxss, 0));

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "网络呼入建档率";
                rows[3]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCCwlhrjdl;
                rows[3]["实际"] = hjzxwl400hrs == 0 ? string.Empty : Math.Round(hjhrxzjds * 100 / hjzxwl400hrs, 2).ToString();
                rows[3]["详细"] = hjzxwl400hrs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjhrxzjds, 0), Math.Round(hjzxwl400hrs, 0));

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "有效呼出率";
                rows[4]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCCyxhcl;
                rows[4]["实际"] = hjhczl == 0 ? string.Empty : Math.Round(hjhcyxs * 100 / hjhczl, 2).ToString();
                rows[4]["详细"] = hjhczl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjhcyxs, 0), Math.Round(hjhczl, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "呼入呼出邀约到店率";
                rows[5]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCChrhcyyddl;
                rows[5]["实际"] = hjxzxsjdzs == 0 ? string.Empty : Math.Round(hjscyyddkhzs * 100 / hjxzxsjdzs, 2).ToString();
                rows[5]["详细"] = hjxzxsjdzs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjscyyddkhzs, 0), Math.Round(hjxzxsjdzs, 0));

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "再次邀约占比";
                rows[6]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCCzcyyl;
                rows[6]["实际"] = (hjscyyddkhzs + hjzyys) == 0 ? string.Empty : Math.Round(hjzyys * 100 / (hjscyyddkhzs + hjzyys), 2).ToString();
                rows[6]["详细"] = (hjscyyddkhzs + hjzyys) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjzyys, 0), Math.Round(hjscyyddkhzs + hjzyys, 0));

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "成交率";
                rows[7]["目标"] = monthtarget == null ? string.Empty : monthtarget.DCCcjl;
                rows[7]["实际"] = hjscyyddkhzs == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjscyyddkhzs, 2).ToString();
                rows[7]["详细"] = hjscyyddkhzs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjcjzts, 0), Math.Round(hjscyyddkhzs, 0));

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "汽车之家";
                rows[8]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjqczjhr * 100 / hjhrxzjds, 2).ToString();
                rows[8]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqczjcjs * 100 / hjcjzts, 2).ToString();

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "易车网";
                rows[9]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjycwhr * 100 / hjhrxzjds, 2).ToString();
                rows[9]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjycwcjs * 100 / hjcjzts, 2).ToString();

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "太平洋汽车";
                rows[10]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjtpyhr * 100 / hjhrxzjds, 2).ToString();
                rows[10]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjtpycjs * 100 / hjcjzts, 2).ToString();

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "本地网络";
                rows[11]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjbdwlhr * 100 / hjhrxzjds, 2).ToString();
                rows[11]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjbdwlcjs * 100 / hjcjzts, 2).ToString();

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "其他网络";
                rows[12]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjqtwlhr * 100 / hjhrxzjds, 2).ToString();
                rows[12]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjzjscjs * 100 / hjcjzts, 2).ToString();

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "展厅转入成交数";
                rows[13]["目标"] = string.Empty;
                rows[13]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjztzrcjs * 100 / hjcjzts, 2).ToString();

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "其他渠道成交数";
                rows[14]["目标"] = string.Empty;
                rows[14]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqtqdcjs * 100 / hjcjzts, 2).ToString();

                #region 市场部数据

                DateTime day = DateTime.Today;
                if (corpid > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_sc = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(corpid),
                        DayReportDep = DayReportDep.市场部
                    };
                    query_sc.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list_sc = DailyReports.Instance.GetList(query_sc, true);
                    MonthlyTargetInfo monthtarget_sc = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(corpid), DayReportDep.市场部, day, true);

                    int days = 0;
                    DataTable tbl_sc = GetReport(DayReportDep.市场部, list_sc, monthtarget_sc, day, corpid, ref days);

                    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-网络渠道'";
                    decimal hjwlqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-户外广告渠道'";
                    decimal hjhwggqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-电台渠道'";
                    decimal hjdtqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-增换购渠道'";
                    decimal hjzhgqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-转介绍渠道'";
                    decimal hjzjsqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-自然到店'";
                    decimal hjzrdd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-汽车之家'";
                    decimal hjqczjxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-易车网'";
                    decimal hjycwxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-太平洋'";
                    decimal hjtpyxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-本地网络'";
                    decimal hjbdwlxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-其他网络'";
                    decimal hjqtwlxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);

                    decimal ddCount = hjwlqddd + hjhwggqddd + hjdtqddd + hjzhgqddd + hjzjsqddd + hjzrdd;

                    rows[15] = tbl.NewRow();
                    rows[15]["关键指标"] = "网络";
                    rows[15]["目标"] = string.Empty;
                    rows[15]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjwlqddd * 100 / ddCount, 2).ToString();

                    rows[16] = tbl.NewRow();
                    rows[16]["关键指标"] = "户外广告";
                    rows[16]["目标"] = string.Empty;
                    rows[16]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjhwggqddd * 100 / ddCount, 2).ToString();

                    rows[17] = tbl.NewRow();
                    rows[17]["关键指标"] = "电台";
                    rows[17]["目标"] = string.Empty;
                    rows[17]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjdtqddd * 100 / ddCount, 2).ToString();

                    rows[18] = tbl.NewRow();
                    rows[18]["关键指标"] = "增换购";
                    rows[18]["目标"] = string.Empty;
                    rows[18]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzhgqddd * 100 / ddCount, 2).ToString();

                    rows[19] = tbl.NewRow();
                    rows[19]["关键指标"] = "转介绍";
                    rows[19]["目标"] = string.Empty;
                    rows[19]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzjsqddd * 100 / ddCount, 2).ToString();

                    rows[20] = tbl.NewRow();
                    rows[20]["关键指标"] = "自然";
                    rows[20]["目标"] = string.Empty;
                    rows[20]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzrdd * 100 / ddCount, 2).ToString();

                    rows[21] = tbl.NewRow();
                    rows[21]["关键指标"] = "其他";
                    rows[21]["目标"] = string.Empty;
                    rows[21]["实际"] = string.Empty;

                    decimal wlxsCount = hjqczjxzlds + hjycwxzlds + hjtpyxzlds + hjbdwlxzlds + hjqtwlxzlds;

                    rows[22] = tbl.NewRow();
                    rows[22]["关键指标"] = "汽车之家";
                    rows[22]["目标"] = string.Empty;
                    rows[22]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqczjxzlds * 100 / wlxsCount, 2).ToString();

                    rows[23] = tbl.NewRow();
                    rows[23]["关键指标"] = "易车网";
                    rows[23]["目标"] = string.Empty;
                    rows[23]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjycwxzlds * 100 / wlxsCount, 2).ToString();

                    rows[24] = tbl.NewRow();
                    rows[24]["关键指标"] = "太平洋汽车";
                    rows[24]["目标"] = string.Empty;
                    rows[24]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjtpyxzlds * 100 / wlxsCount, 2).ToString();

                    rows[25] = tbl.NewRow();
                    rows[25]["关键指标"] = "本地网络";
                    rows[25]["目标"] = string.Empty;
                    rows[25]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjbdwlxzlds * 100 / wlxsCount, 2).ToString();

                    rows[26] = tbl.NewRow();
                    rows[26]["关键指标"] = "其他网络";
                    rows[26]["目标"] = string.Empty;
                    rows[26]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqtwlxzlds * 100 / wlxsCount, 2).ToString();
                }

                #endregion

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

                DataRow[] rows = new DataRow[12];

                data.DefaultView.RowFilter = "项目='新车展厅销售量'";
                decimal hjxcztxsl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='销售推荐评估台次'";
                decimal hjxstjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='实际评估台次'";
                decimal hjxssjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='售后进厂台次'";
                decimal hjshjctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='售后推荐评估台次'";
                decimal hjshtjpgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='实际评估台次'";
                decimal hjshsjpgtc = DataConvert.SafeDecimal(data.DefaultView[1]["合计"]);
                data.DefaultView.RowFilter = "项目='实际评估台次'";
                decimal hjqtqdsjpgtc = DataConvert.SafeDecimal(data.DefaultView[2]["合计"]);
                data.DefaultView.RowFilter = "项目='销售台次'";
                decimal hjxstc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='销售毛利'";
                decimal hjxsml = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='纯收购台次'";
                decimal hjsgtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='置换台次'";
                decimal hjzhtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='首次来电批次'";
                decimal hjscldpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='首次来店批次'";
                decimal hjscldianpc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='库存数'";
                decimal hjkcs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='在库超30天车辆'";
                decimal hjzkc30tcl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "销售有效推荐率";
                rows[0]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCxsyxtjl;
                rows[0]["实际"] = hjxcztxsl == 0 ? string.Empty : Math.Round(hjxssjpgtc * 100 / hjxcztxsl, 2).ToString();
                rows[0]["详细"] = hjxcztxsl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxssjpgtc, 0), Math.Round(hjxcztxsl, 0));

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "售后有效推荐率";
                rows[1]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCshyxtjl;
                rows[1]["实际"] = hjshjctc == 0 ? string.Empty : Math.Round(hjshsjpgtc * 100 / hjshjctc, 2).ToString();
                rows[1]["详细"] = hjshjctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjshsjpgtc, 0), Math.Round(hjshjctc, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "总评估成交率";
                rows[2]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCpgcjl;
                rows[2]["实际"] = (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc) == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc), 2).ToString();
                rows[2]["详细"] = (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsgtc + hjzhtc, 0), Math.Round(hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc, 0));

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "总销售成交率";
                rows[3]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCxscjl;
                rows[3]["实际"] = (hjscldianpc + hjscldpc) == 0 ? string.Empty : Math.Round(hjxstc * 100 / (hjscldianpc + hjscldpc), 2).ToString();
                rows[3]["详细"] = (hjscldianpc + hjscldpc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxstc, 0), Math.Round(hjscldianpc + hjscldpc, 0));

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "总置换率";
                rows[4]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCzzhl;
                rows[4]["实际"] = hjxcztxsl == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / hjxcztxsl, 2).ToString();
                rows[4]["详细"] = hjxcztxsl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsgtc + hjzhtc, 0), Math.Round(hjxcztxsl, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "总有效评估量";
                rows[5]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCzyxpgl;
                rows[5]["实际"] = hjxstjpgtc + hjshsjpgtc + hjqtqdsjpgtc;
                rows[5]["详细"] = string.Empty;

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "总收购量";
                rows[6]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCzsgl;
                rows[6]["实际"] = hjsgtc + hjzhtc;
                rows[6]["详细"] = string.Empty;

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "总销售量";
                rows[7]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCzxsl;
                rows[7]["实际"] = hjxstc;
                rows[7]["详细"] = string.Empty;

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "总毛利";
                rows[8]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCzml;
                rows[8]["实际"] = hjxsml;
                rows[8]["详细"] = string.Empty;

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "平均单台毛利";
                rows[9]["目标"] = monthtarget == null ? string.Empty : monthtarget.ESCpjdtml;
                rows[9]["实际"] = hjxstc == 0 ? string.Empty : Math.Round(hjxsml / hjxstc, 2).ToString();
                rows[9]["详细"] = string.Empty;

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "库存";
                rows[10]["目标"] = string.Empty;
                rows[10]["实际"] = hjkcs;
                rows[10]["详细"] = string.Empty;

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "在库超30天";
                rows[11]["目标"] = string.Empty;
                rows[11]["实际"] = hjzkc30tcl;
                rows[11]["详细"] = string.Empty;

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
                        rows[i]["实际"] = hjbycz == 0 ? string.Empty : (Math.Round(hjby * 100 / hjbycz, 2).ToString() + "%");

                        data.DefaultView.RowFilter = "项目='" + listbxgs[i] + "（新保产值）'";
                        decimal hjxb = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                        rows[i + 9] = tbl.NewRow();
                        rows[i + 9]["目标"] = hjxb == 0 ? string.Empty : hjxb.ToString();
                        rows[i + 9]["实际"] = hjxbbf == 0 ? string.Empty : (Math.Round(hjxb * 100 / hjxbbf, 2).ToString() + "%");

                        data.DefaultView.RowFilter = "项目='" + listbxgs[i] + "（续保产值）'";
                        decimal hjxub = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                        rows[i + 18] = tbl.NewRow();
                        rows[i + 18]["目标"] = hjxub == 0 ? string.Empty : hjxub.ToString();
                        rows[i + 18]["实际"] = hjxubbf == 0 ? string.Empty : (Math.Round(hjxub * 100 / hjxubbf, 2).ToString() + "%");

                        data.DefaultView.RowFilter = "项目='" + listbxgs[i] + "（二网产值）'";
                        decimal hjew = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                        rows[i + 27] = tbl.NewRow();
                        rows[i + 27]["目标"] = hjew == 0 ? string.Empty : hjew.ToString();
                        rows[i + 27]["实际"] = hjxbbf == 0 ? string.Empty : (Math.Round(hjew * 100 / hjxbbf, 2).ToString() + "%");
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

            return tbl;
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
            Regex reg_colspan = new Regex("colspan=\"?(\\d)\"?",RegexOptions.IgnoreCase);
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
                    int colindex = td + colspancount + (i == 1 ? 1 : 0);
                    ICell cell = row.CreateCell(colindex);
                    cell.SetCellValue(columnCollection[td].Groups[1].Value == "&nbsp;" ? string.Empty : columnCollection[td].Groups[1].Value);

                    colspancount += colspan;
                }
            }
            CellRangeAddress cellRangeAddress = new CellRangeAddress(0, 1, 0, 0);
            sheet1.AddMergedRegion(cellRangeAddress);
            colspancount = 0;
            rowContent = rowCollection[0].Value;
            MatchCollection _columnCollection = Regex.Matches(rowContent, @"<td[^>]*>([\s\S]*?)<\/td>", RegexOptions.IgnoreCase ); //对td进行筛选
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