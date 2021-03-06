﻿using System;
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
using NPOI.HSSF.Util;

namespace Hx.BackAdmin.dayreport
{
    public partial class dailyreportview : AdminBase
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
                        if (q != "dep" && q != "corp" && q != "date")
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

        private CorporationInfo _currentcorporation = null;
        private CorporationInfo CurrentCorporation
        {
            get
            {
                int id = DataConvert.SafeInt(ddlCorp.SelectedValue);
                if (_currentcorporation == null && id > 0)
                {
                    _currentcorporation = Corporations.Instance.GetModel(id);
                }
                return _currentcorporation;
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

        private string lastDayUnique = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
            }
        }

        private void BindControler()
        {
            txtDate.Text = !string.IsNullOrEmpty(GetString("date")) ? GetString("date") : DateTime.Today.ToString("yyyy-MM");

            string[] corppowers = CurrentUser.DayReportViewCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<CorporationInfo> corps = Corporations.Instance.GetList(true);
            ddlCorp.DataSource = corps.FindAll(c => c.DailyreportShow == 1 && corppowers.Contains(c.ID.ToString()));
            ddlCorp.DataTextField = "Name";
            ddlCorp.DataValueField = "ID";
            ddlCorp.DataBind();
            ddlCorp.Items.Insert(0, new ListItem("-请选择-", "-1"));
            SetSelectedByValue(ddlCorp, !string.IsNullOrEmpty(GetString("corp")) ? GetString("corp") : (!corppowers.Contains(CurrentUser.CorporationID.ToString()) && corppowers.Length > 0 ? corppowers[0] : CurrentUser.CorporationID.ToString()));

            if (ddlCorp.SelectedIndex > 0 || !string.IsNullOrEmpty(GetString("date")))
                NewQuery = "&corp=" + ddlCorp.SelectedValue + "&date=" + txtDate.Text;

            if (!string.IsNullOrEmpty(GetString("date")) && !string.IsNullOrEmpty(GetString("corp")))
                LoadData();
        }

        private void LoadData()
        {
            tblView.Attributes.Remove("style");

            DateTime day = DateTime.Today;
            if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = CurrentDep
                };
                query.OrderBy = " [DayUnique] ASC";
                List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);

                spTitle.InnerText = string.Format("{0}年{1}月{2}工作表", day.Year, day.Month, CurrentDep.ToString().Replace("部", string.Empty));
                if (CurrentDep == DayReportDep.行政部)
                    spTitle.InnerText = string.Format("{0}年{1}月{2}工作表", day.Year, day.Month, "人事");

                #region 常规项

                int days = 0;

                if (list.Count > 0)
                    lastDayUnique = list.Last().DayUnique;
                DataTable tbl = GetReport(CurrentDep, list, monthtarget, day, ref days);
                tdData.InnerHtml = GetReportStr(CurrentDep, tbl, days);

                #endregion

                #region 关键指标

                tbl = GetKeyReport(CurrentDep, list, monthtarget, tbl);

                tdKeyData.InnerHtml = GetKeyReportStr(CurrentDep, tbl);

                #endregion

                #region 保存条件

                NewQuery = "&corp=" + ddlCorp.SelectedValue + "&date=" + txtDate.Text;

                #endregion
            }
        }

        private void ExportExcel()
        {
            DateTime day = DateTime.Today;
            int days = 0;
            if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = CurrentDep
                };
                List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                if (CurrentDep == DayReportDep.销售部)
                {
                    string fileName = Utils.GetMapPath(string.Format(@"\App_Data\销售日报表模板{0}.xlsx", CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1 ? string.Empty : "1"));
                    newfile = string.Format(@"销售日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}销售工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                    {
                        for (int i = 0; i < tblReport.Rows.Count; i++)
                        {
                            if (tblReport.Rows[i]["项目"].ToString() == "是否审核") continue;
                            for (int j = 0; j < days; j++)
                            {
                                if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                    sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                            }
                            if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                                sheet.GetRow(i + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                        }
                    }
                    else
                    {
                        int index = 0;
                        string[] tpp = new string[] { "其中他品牌新增订单台次", "他品牌交车台次", "他品牌单车毛利", "他品牌单车综合毛利" };
                        for (int i = 0; i < tblReport.Rows.Count; i++)
                        {
                            if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                            if (!tpp.Contains(tblReport.Rows[i]["项目"].ToString()))
                            {
                                for (int j = 0; j < days; j++)
                                {
                                    if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                        sheet.GetRow(index + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                                }
                                if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                                    sheet.GetRow(index + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                                index++;
                            }
                        }
                    }

                    tblReport.DefaultView.RowFilter = "项目='销售置换数'";
                    decimal mbxszhs = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='展厅交车台数'";
                    decimal mbztjcts = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='二网销售台次'";
                    decimal mbewxstc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='展厅首次来客批次'";
                    decimal mbztsclkpc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='留档批次'";
                    decimal mbldpc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='展厅订单台数'";
                    decimal mbztddts = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='其中老客户转介绍交车台次'";
                    decimal mbqzlkhzjsjctc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='上牌台次'";
                    decimal mbsptc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='上牌总金额'";
                    decimal mbspzje = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='展厅含DCC保险台次'";
                    decimal mbzthdccbxtc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='展厅含DCC保险总金额'";
                    decimal mbzthdccbxzje = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='美容交车台次'";
                    decimal mbmrjctc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='美容交车总金额'";
                    decimal mbmrjczje = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='延保无忧车服务购买个数'";
                    decimal mbybtc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='玻璃无忧服务购买个数'";
                    decimal mbblxtc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='机油套餐购买个数'";
                    decimal mbjytcgmgs = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='划痕无忧服务购买个数'";
                    decimal mbhhwygmgs = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='展厅成交精品台次'";
                    decimal mbztcjjptc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='精品总金额'";
                    decimal mbjpzje = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='二网精品金额'";
                    decimal mbewjpje = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='本地按揭台次'";
                    decimal mbbdajtc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='银行按揭台次'";
                    decimal mbyhajtc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='厂家金融台次'";
                    decimal mbcjjrtc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='按揭净收入'";
                    decimal mbajjsr = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='免费保养台次（含赠送）'";
                    decimal mbzsmfbxtc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='免费保养总金额'";
                    decimal mbzsmfbxzje = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='他品牌交车台次'";
                    decimal mbtppxstc = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='他品牌单车毛利'";
                    decimal mbtppdcml = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='他品牌单车综合毛利'";
                    decimal mbtppzhml = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);
                    tblReport.DefaultView.RowFilter = "项目='试乘试驾率'";
                    decimal mbscsjs = DataConvert.SafeDecimal(tblReport.DefaultView[0]["目标值"]);


                    #endregion

                    #region 关键指标月度目标

                    #region 留单

                    decimal ld = 0;
                    if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                    {
                        DailyReportQuery query_last = new DailyReportQuery()
                        {
                            DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                            CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
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

                    if (monthtarget != null)
                    {
                        sheet.GetRow(4).Cells[39].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSzxstc) ? monthtarget.XSzxstc : (mbztjcts + mbewxstc).ToString()));
                        sheet.GetRow(4).Cells[40].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSztzb) ? monthtarget.XSztzb : ((mbztjcts + mbewxstc) == 0 ? string.Empty : Math.Round(mbztjcts * 100 / (mbztjcts + mbewxstc), 1).ToString()).ToString()) / 100);
                        sheet.GetRow(4).Cells[41].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSztldl) ? monthtarget.XSztldl : (mbztsclkpc == 0 ? string.Empty : Math.Round(mbldpc * 100 / mbztsclkpc, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(4).Cells[42].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSztcjl) ? monthtarget.XSztcjl : (mbldpc == 0 ? string.Empty : Math.Round((mbztddts - ld) * 100 / mbldpc, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(4).Cells[43].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSspl) ? monthtarget.XSspl : (mbztjcts == 0 ? string.Empty : Math.Round(mbsptc * 100 / mbztjcts, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(4).Cells[44].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSspdt) ? monthtarget.XSspdt : (mbsptc == 0 ? string.Empty : Math.Round(mbspzje / mbsptc, 0).ToString()).ToString()));
                        sheet.GetRow(4).Cells[45].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSztbxl) ? monthtarget.XSztbxl : (mbztjcts == 0 ? string.Empty : Math.Round(mbzthdccbxtc * 100 / mbztjcts, 1).ToString()).ToString()) / 100);

                        sheet.GetRow(8).Cells[39].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSztbxdt) ? monthtarget.XSztbxdt : (mbzthdccbxtc == 0 ? string.Empty : Math.Round(mbzthdccbxzje / mbzthdccbxtc, 0).ToString()).ToString()));
                        sheet.GetRow(8).Cells[40].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSmrjcl) ? monthtarget.XSmrjcl : (mbztjcts == 0 ? string.Empty : Math.Round(mbmrjctc * 100 / mbztjcts, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(8).Cells[41].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSmrdt) ? monthtarget.XSmrdt : (mbmrjctc == 0 ? string.Empty : Math.Round(mbmrjczje / mbmrjctc, 0).ToString()).ToString()));
                        sheet.GetRow(8).Cells[42].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSztjpqzl) ? monthtarget.XSztjpqzl : (mbztjcts == 0 ? string.Empty : Math.Round(mbztcjjptc * 100 / mbztjcts, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(8).Cells[43].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSztjppjdt) ? monthtarget.XSztjppjdt : (mbztjcts == 0 ? string.Empty : Math.Round(mbjpzje / mbztjcts, 0).ToString()).ToString()));
                        sheet.GetRow(8).Cells[44].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSewjppjdt) ? monthtarget.XSewjppjdt : (mbewxstc == 0 ? string.Empty : Math.Round(mbewjpje / mbewxstc, 0).ToString()).ToString()));
                        sheet.GetRow(8).Cells[45].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSxszhtc) ? monthtarget.XSxszhtc : mbxszhs.ToString()));
                        
                        sheet.GetRow(12).Cells[39].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSajl) ? monthtarget.XSajl : (mbztjcts == 0 ? string.Empty : Math.Round((mbbdajtc + mbyhajtc + mbcjjrtc) * 100 / mbztjcts, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(12).Cells[40].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSajpjdt) ? monthtarget.XSajpjdt : ((mbbdajtc + mbyhajtc + mbcjjrtc) == 0 ? string.Empty : Math.Round(mbajjsr / (mbbdajtc + mbyhajtc + mbcjjrtc), 0).ToString()).ToString()));
                        sheet.GetRow(12).Cells[41].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSmfbystl) ? monthtarget.XSmfbystl : (mbztjcts == 0 ? string.Empty : Math.Round(mbzsmfbxtc * 100 / mbztjcts, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(12).Cells[42].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSmfbydt) ? monthtarget.XSmfbydt : (mbzsmfbxtc == 0 ? string.Empty : Math.Round(mbzsmfbxzje / mbzsmfbxtc, 0).ToString()).ToString()));

                        sheet.GetRow(16).Cells[39].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSzjsl) ? monthtarget.XSzjsl : (mbztjcts == 0 ? string.Empty : Math.Round(mbqzlkhzjsjctc * 100 / mbztjcts, 1).ToString()).ToString()) / 100);
                        sheet.GetRow(16).Cells[41].SetCellValue(DataConvert.SafeFloat(mbzthdccbxtc == 0 ? string.Empty : Math.Round(mbblxtc * 100 / mbzthdccbxtc, 1).ToString()) / 100);
                        sheet.GetRow(16).Cells[42].SetCellValue(DataConvert.SafeFloat(mbztjcts == 0 ? string.Empty : Math.Round(mbybtc * 100 / mbztjcts, 1).ToString()) / 100);
                        sheet.GetRow(16).Cells[43].SetCellValue(DataConvert.SafeFloat(mbztjcts == 0 ? string.Empty : Math.Round(mbjytcgmgs * 100 / mbztjcts, 1).ToString()) / 100);
                        sheet.GetRow(16).Cells[44].SetCellValue(DataConvert.SafeFloat(mbzthdccbxtc == 0 ? string.Empty : Math.Round(mbhhwygmgs * 100 / mbzthdccbxtc, 1).ToString()) / 100);
                        sheet.GetRow(16).Cells[45].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XSscsjl) ? monthtarget.XSscsjl : ((mbztddts - ld) == 0 ? string.Empty : Math.Round(mbscsjs  * 100 / (mbztddts - ld), 1).ToString()).ToString()) / 100);

                        if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                        {
                            sheet.GetRow(21).Cells[39].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XStppxstc) ? monthtarget.XStppxstc : mbtppxstc.ToString()));
                            sheet.GetRow(21).Cells[40].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XStppdcml) ? monthtarget.XStppdcml : mbtppdcml.ToString()));
                            sheet.GetRow(21).Cells[41].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XStppzhml) ? monthtarget.XStppzhml : mbtppzhml.ToString()));
                            sheet.GetRow(21).Cells[42].SetCellValue(DataConvert.SafeFloat(!string.IsNullOrEmpty(monthtarget.XStpppjdt) ? monthtarget.XStpppjdt : (mbtppxstc == 0 ? string.Empty : Math.Round((mbtppdcml + mbtppzhml) / mbtppxstc, 0).ToString())));

                            if (!string.IsNullOrEmpty(monthtarget.XSclpjdj))
                                sheet.GetRow(24).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSclpjdj));
                            if (!string.IsNullOrEmpty(monthtarget.XSzkcsgytc))
                                sheet.GetRow(26).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSzkcsgytc));
                            if (!string.IsNullOrEmpty(monthtarget.XSztcl))
                                sheet.GetRow(27).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSztcl));
                            sheet.GetRow(29).Cells[39].SetCellValue(DataConvert.SafeFloat(ld));
                            if (!string.IsNullOrEmpty(monthtarget.XSzzts))
                                sheet.GetRow(31).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSzzts));
                            if(!string.IsNullOrEmpty(monthtarget.XScjxctc))
                                sheet.GetRow(33).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XScjxctc)) ;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(monthtarget.XSclpjdj))
                                sheet.GetRow(20).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSclpjdj));
                            if (!string.IsNullOrEmpty(monthtarget.XSzkcsgytc))
                                sheet.GetRow(22).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSzkcsgytc));
                            if (!string.IsNullOrEmpty(monthtarget.XSztcl))
                                sheet.GetRow(23).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSztcl));
                            sheet.GetRow(25).Cells[39].SetCellValue(DataConvert.SafeFloat(ld));
                            if (!string.IsNullOrEmpty(monthtarget.XSzzts))
                                sheet.GetRow(27).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XSzzts));
                            if (!string.IsNullOrEmpty(monthtarget.XScjxctc))
                                sheet.GetRow(28).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.XScjxctc));
                        }
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.售后部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\售后日报表模板.xlsx");
                    newfile = string.Format(@"售后日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置


                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}售后工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"].ToString() == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.客服部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\客服日报表模板.xlsx");
                    newfile = string.Format(@"客服日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}客服工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"].ToString() == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[33].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion

                    #region 关键指标设置

                    DataTable tblKey = GetKeyReport(CurrentDep, list, monthtarget, tblReport);

                    sheet.GetRow(8).Cells[39].SetCellValue(DataConvert.SafeFloat(tblKey.Rows[7]["目标"]) / 100);
                    sheet.GetRow(9).Cells[39].SetCellValue(DataConvert.SafeFloat(tblKey.Rows[7]["实际"]) / 100);

                    if (monthtarget != null)
                    {
                        if (!string.IsNullOrEmpty(monthtarget.KFjpbylhl1year))
                            sheet.GetRow(15).Cells[36].SetCellValue(DataConvert.SafeFloat(monthtarget.KFjpbylhl1year));
                        if (!string.IsNullOrEmpty(monthtarget.KFjpbylhl2year))
                            sheet.GetRow(15).Cells[37].SetCellValue(DataConvert.SafeFloat(monthtarget.KFjpbylhl2year));
                        if (!string.IsNullOrEmpty(monthtarget.KFjpbylhl3year))
                            sheet.GetRow(15).Cells[38].SetCellValue(DataConvert.SafeFloat(monthtarget.KFjpbylhl3year));
                        if (!string.IsNullOrEmpty(monthtarget.KFjpbylhl3_5year))
                            sheet.GetRow(15).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.KFjpbylhl3_5year));
                        if (!string.IsNullOrEmpty(monthtarget.KFjpbylhl5year))
                            sheet.GetRow(15).Cells[40].SetCellValue(DataConvert.SafeFloat(monthtarget.KFjpbylhl5year));

                        if (!string.IsNullOrEmpty(monthtarget.KFyxglnkhl1year))
                            sheet.GetRow(16).Cells[36].SetCellValue(DataConvert.SafeFloat(monthtarget.KFyxglnkhl1year));
                        if (!string.IsNullOrEmpty(monthtarget.KFyxglnkhl2year))
                            sheet.GetRow(16).Cells[37].SetCellValue(DataConvert.SafeFloat(monthtarget.KFyxglnkhl2year));
                        if (!string.IsNullOrEmpty(monthtarget.KFyxglnkhl3year))
                            sheet.GetRow(16).Cells[38].SetCellValue(DataConvert.SafeFloat(monthtarget.KFyxglnkhl3year));
                        if (!string.IsNullOrEmpty(monthtarget.KFyxglnkhl3_5year))
                            sheet.GetRow(16).Cells[39].SetCellValue(DataConvert.SafeFloat(monthtarget.KFyxglnkhl3_5year));
                        if (!string.IsNullOrEmpty(monthtarget.KFyxglnkhl5year))
                            sheet.GetRow(16).Cells[40].SetCellValue(DataConvert.SafeFloat(monthtarget.KFyxglnkhl5year));
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.市场部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\市场日报表模板.xlsx");
                    newfile = string.Format(@"市场日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}市场工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"].ToString() == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion

                    #region 关键指标月度目标

                    #region 展厅上月留存客户数

                    decimal ztsylckhs = 0;
                    if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                    {
                        DailyReportQuery query_last = new DailyReportQuery()
                        {
                            DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                            CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                            DayReportDep = DayReportDep.市场部
                        };
                        List<DailyReportInfo> list_last = DailyReports.Instance.GetList(query_last, true);
                        list_last = list_last.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        List<DailyReportModuleInfo> rlist_sc = DayReportModules.Instance.GetList(true);
                        rlist_sc = rlist_sc.FindAll(l => l.Department == DayReportDep.市场部).OrderBy(l => l.Sort).ToList();
                        List<Dictionary<string, string>> data_last = new List<Dictionary<string, string>>();
                        for (int i = 0; i < list_last.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(list_last[i].SCReport))
                            {
                                data_last.Add(json.Deserialize<Dictionary<string, string>>(list_last[i].SCReport));
                            }
                        }
                        if (rlist_sc.Exists(l => l.Name == "展厅本月留存客户数"))
                        {
                            int idztbylckhs = rlist_sc.Find(l => l.Name == "展厅本月留存客户数").ID;
                            if (data_last.Exists(d => d.ContainsKey(idztbylckhs.ToString()) && !string.IsNullOrEmpty(d[idztbylckhs.ToString()])))
                            {
                                ztsylckhs = DataConvert.SafeDecimal(data_last.FindLast(d => d.ContainsKey(idztbylckhs.ToString()) && !string.IsNullOrEmpty(d[idztbylckhs.ToString()]))[idztbylckhs.ToString()]);
                            }
                        }
                    }
                    #endregion

                    if (monthtarget != null)
                    {
                        if (!string.IsNullOrEmpty(monthtarget.SCscdfdcl))
                            sheet.GetRow(4).Cells[40].SetCellValue(DataConvert.SafeFloat(monthtarget.SCscdfdcl) / 100);

                        if (!string.IsNullOrEmpty(monthtarget.SCsyfsl))
                            sheet.GetRow(8).Cells[41].SetCellValue(DataConvert.SafeFloat(monthtarget.SCsyfsl));
                        sheet.GetRow(10).Cells[41].SetCellValue(DataConvert.SafeFloat(ztsylckhs));
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.财务部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\财务日报表模板.xlsx");
                    newfile = string.Format(@"财务日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}财务工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[3 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["期初余额"].ToString()))
                            sheet.GetRow(i + 3).Cells[2].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["期初余额"]));
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.行政部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\行政人事日报表模板.xlsx");
                    newfile = string.Format(@"行政人事日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}行政\\人事工作表", ddlCorp.SelectedItem.Text + day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 17).Cells[3 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 17).Cells[1].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion

                    #region 关键指标月度目标

                    if (monthtarget != null)
                    {
                        if (!string.IsNullOrEmpty(monthtarget.XZcpfzr))
                        {
                            for (int i = 0; i < monthtarget.XZcpfzr.Split(new char[] { '|' }, StringSplitOptions.None).Length; i++)
                            {
                                sheet.GetRow(3 + i).Cells[1].SetCellValue(monthtarget.XZcpfzr.Split(new char[] { '|' }, StringSplitOptions.None)[i]);
                            }
                        }
                        if (!string.IsNullOrEmpty(monthtarget.XZdyzclwzcs))
                        {
                            for (int i = 0; i < monthtarget.XZdyzclwzcs.Split(new char[] { '|' }, StringSplitOptions.None).Length; i++)
                            {
                                sheet.GetRow(3 + i).Cells[2].SetCellValue(monthtarget.XZdyzclwzcs.Split(new char[] { '|' }, StringSplitOptions.None)[i]);
                            }
                        }
                        if (!string.IsNullOrEmpty(monthtarget.XZdezclwzcs))
                        {
                            for (int i = 0; i < monthtarget.XZdezclwzcs.Split(new char[] { '|' }, StringSplitOptions.None).Length; i++)
                            {
                                sheet.GetRow(3 + i).Cells[3].SetCellValue(monthtarget.XZdezclwzcs.Split(new char[] { '|' }, StringSplitOptions.None)[i]);
                            }
                        }
                        if (!string.IsNullOrEmpty(monthtarget.XZdszclwzcs))
                        {
                            for (int i = 0; i < monthtarget.XZdszclwzcs.Split(new char[] { '|' }, StringSplitOptions.None).Length; i++)
                            {
                                sheet.GetRow(3 + i).Cells[4].SetCellValue(monthtarget.XZdszclwzcs.Split(new char[] { '|' }, StringSplitOptions.None)[i]);
                            }
                        }
                        if (!string.IsNullOrEmpty(monthtarget.XZdsizclwzcs))
                        {
                            for (int i = 0; i < monthtarget.XZdsizclwzcs.Split(new char[] { '|' }, StringSplitOptions.None).Length; i++)
                            {
                                sheet.GetRow(3 + i).Cells[5].SetCellValue(monthtarget.XZdsizclwzcs.Split(new char[] { '|' }, StringSplitOptions.None)[i]);
                            }
                        }
                        if (!string.IsNullOrEmpty(monthtarget.XZsywcl))
                        {
                            for (int i = 0; i < monthtarget.XZsywcl.Split(new char[] { '|' }, StringSplitOptions.None).Length; i++)
                            {
                                sheet.GetRow(3 + i).Cells[6].SetCellValue(monthtarget.XZsywcl.Split(new char[] { '|' }, StringSplitOptions.None)[i]);
                            }
                        }
                        if (!string.IsNullOrEmpty(monthtarget.XZwz))
                        {
                            for (int i = 0; i < monthtarget.XZwz.Split(new char[] { '|' }, StringSplitOptions.None).Length; i++)
                            {
                                sheet.GetRow(3 + i).Cells[7].SetCellValue(monthtarget.XZwz.Split(new char[] { '|' }, StringSplitOptions.None)[i]);
                            }
                        }
                        sheet.GetRow(10).Cells[2].SetCellValue(monthtarget.XZdyzcdrs);
                        sheet.GetRow(10).Cells[3].SetCellValue(monthtarget.XZdezcdrs);
                        sheet.GetRow(10).Cells[4].SetCellValue(monthtarget.XZdszcdrs);
                        sheet.GetRow(10).Cells[5].SetCellValue(monthtarget.XZdsizcdrs);

                        sheet.GetRow(11).Cells[2].SetCellValue(monthtarget.XZdyzqjrs);
                        sheet.GetRow(11).Cells[3].SetCellValue(monthtarget.XZdezqjrs);
                        sheet.GetRow(11).Cells[4].SetCellValue(monthtarget.XZdszqjrs);
                        sheet.GetRow(11).Cells[5].SetCellValue(monthtarget.XZdsizqjrs);

                        sheet.GetRow(12).Cells[2].SetCellValue(monthtarget.XZdyzkgrs);
                        sheet.GetRow(12).Cells[3].SetCellValue(monthtarget.XZdezkgrs);
                        sheet.GetRow(12).Cells[4].SetCellValue(monthtarget.XZdszkgrs);
                        sheet.GetRow(12).Cells[5].SetCellValue(monthtarget.XZdsizkgrs);

                        sheet.GetRow(13).Cells[2].SetCellValue(monthtarget.XZdyzccpxrc);
                        sheet.GetRow(13).Cells[3].SetCellValue(monthtarget.XZdezccpxrc);
                        sheet.GetRow(13).Cells[4].SetCellValue(monthtarget.XZdszccpxrc);
                        sheet.GetRow(13).Cells[5].SetCellValue(monthtarget.XZdsizccpxrc);

                        sheet.GetRow(14).Cells[2].SetCellValue(monthtarget.XZdyzaqsgsse);
                        sheet.GetRow(14).Cells[3].SetCellValue(monthtarget.XZdezaqsgsse);
                        sheet.GetRow(14).Cells[4].SetCellValue(monthtarget.XZdszaqsgsse);
                        sheet.GetRow(14).Cells[5].SetCellValue(monthtarget.XZdsizaqsgsse);
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.DCC部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\DCC日报表模板.xlsx");
                    newfile = string.Format(@"DCC日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);


                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}DCC工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion

                    #region 关键指标月度目标

                    #region 留单

                    decimal ld = 0;
                    if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                    {
                        DailyReportQuery query_last = new DailyReportQuery()
                        {
                            DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                            CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
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
                            int iddcccjzts = rlist_dcc.Find(l => l.Name == "DCC成交总台数").ID;
                            decimal hjdccdds_last = Math.Round(data_last.Sum(d => d.ContainsKey(iddccdds.ToString()) ? DataConvert.SafeDecimal(d[iddccdds.ToString()]) : 0), 0);
                            decimal hjdcccjzts_last = Math.Round(data_last.Sum(d => d.ContainsKey(iddcccjzts.ToString()) ? DataConvert.SafeDecimal(d[iddcccjzts.ToString()]) : 0), 0);

                            ld = hjdccdds_last - hjdcccjzts_last;
                        }
                    }
                    #endregion

                    sheet.GetRow(12).Cells[39].SetCellValue(DataConvert.SafeFloat(ld));

                    #endregion
                }
                else if (CurrentDep == DayReportDep.二手车部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\二手车日报表模板.xlsx");
                    newfile = string.Format(@"二手车日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}二手车工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion

                    #region 关键指标

                    DataTable tblKey = GetKeyReport(CurrentDep, list, monthtarget, tblReport);

                    sheet.GetRow(4).Cells[43].SetCellValue(DataConvert.SafeFloat(tblKey.Rows[5]["目标"]) / 100);
                    sheet.GetRow(5).Cells[43].SetCellValue(DataConvert.SafeFloat(tblKey.Rows[5]["实际"]) / 100);


                    #endregion

                }
                else if (CurrentDep == DayReportDep.精品部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\精品日报表模板.xlsx");
                    newfile = string.Format(@"精品日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}精品工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.金融部)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\金融日报表模板.xlsx");
                    newfile = string.Format(@"金融日报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}金融工作表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[2 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[34].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion

                    #region 关键指标月度目标

                    DataTable tblKey = GetKeyReport(CurrentDep, list, monthtarget, tblReport);
                    for (int i = 0; i < 9; i++)
                    {
                        sheet.GetRow(i + 9).Cells[37].SetCellValue(DataConvert.SafeInt(tblKey.Rows[i]["目标"]));
                    }

                    #endregion
                }
                else if (CurrentDep == DayReportDep.无忧产品)
                {
                    string fileName = Utils.GetMapPath(@"\App_Data\无忧产品日报表模版.xlsx");
                    newfile = string.Format(@"无忧产品报表{0}.xlsx", day.ToString("yyyyMM"));
                    using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    sheet = workbook.GetSheetAt(0);

                    #region 各项值设置

                    sheet.GetRow(0).Cells[0].SetCellValue(string.Format("{0}无忧产品报表", day.ToString("yyyy年MM月")));
                    DataTable tblReport = GetReport(CurrentDep, list, monthtarget, day, ref days);

                    for (int i = 0; i < 16; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 3).Cells[4 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 3).Cells[36].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    if(monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyjytclcsycls))
                        sheet.GetRow(19).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyjytclcsycls));
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyjytclcsycls))
                        sheet.GetRow(20).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyjytcsyje) / 10000);

                    for (int i = 16; i < 20; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 5).Cells[4 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 5).Cells[36].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyhhwyfwdqgs))
                        sheet.GetRow(25).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyhhwyfwdqgs));
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyhhwyfwdqje))
                        sheet.GetRow(26).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyhhwyfwdqje) / 10000);
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyhhwyfwdqnpfgs))
                        sheet.GetRow(27).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyhhwyfwdqnpfgs));
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyhhwyfwdqnpfje))
                        sheet.GetRow(28).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyhhwyfwdqnpfje) / 10000);

                    for (int i = 20; i < 24; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 9).Cells[4 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 9).Cells[36].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyblwyfwdqgs))
                        sheet.GetRow(33).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyblwyfwdqgs));
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyblwyfwdqje))
                        sheet.GetRow(34).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyblwyfwdqje) / 10000);
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyblwyfwdqnpfgs))
                        sheet.GetRow(35).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyblwyfwdqnpfgs));
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyblwyfwdqnpfje))
                        sheet.GetRow(36).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyblwyfwdqnpfje) / 10000);


                    for (int i = 24; i < 33; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 13).Cells[4 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 13).Cells[36].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyybfwdqgs))
                        sheet.GetRow(46).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyybfwdqgs));
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyybfwdqje))
                        sheet.GetRow(47).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyybfwdqje) / 10000);
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyybfwdqnpfgs))
                        sheet.GetRow(48).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyybfwdqnpfgs));
                    if (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPbyybfwdqnpfje))
                        sheet.GetRow(49).Cells[days + 3].SetCellValue(DataConvert.SafeDouble(monthtarget.NXCPbyybfwdqnpfje) / 10000);

                    for (int i = 33; i < tblReport.Rows.Count; i++)
                    {
                        if (tblReport.Rows[i]["项目"] == "是否审核") continue;
                        for (int j = 0; j < days; j++)
                        {
                            if (!string.IsNullOrEmpty(tblReport.Rows[i][(j + 1).ToString()].ToString()))
                                sheet.GetRow(i + 17).Cells[4 + j].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i][(j + 1).ToString()]));
                        }
                        if (!string.IsNullOrEmpty(tblReport.Rows[i]["目标值"].ToString()))
                            sheet.GetRow(i + 17).Cells[36].SetCellValue(DataConvert.SafeDouble(tblReport.Rows[i]["目标值"]));
                    }

                    #endregion

                    #region 关键指标月度目标

                    DataTable tblKey = GetKeyReport(CurrentDep, list, monthtarget, tblReport);

                    sheet.GetRow(6).Cells[40].SetCellValue(DataConvert.SafeInt(tblKey.Rows[0]["目标"]).ToString());
                    sheet.GetRow(7).Cells[40].SetCellValue(DataConvert.SafeInt(tblKey.Rows[0]["实际"]).ToString());

                    sheet.GetRow(17).Cells[40].SetCellValue(DataConvert.SafeInt(tblKey.Rows[13]["目标"]).ToString());
                    sheet.GetRow(18).Cells[40].SetCellValue(DataConvert.SafeInt(tblKey.Rows[13]["实际"]).ToString());


                    #endregion
                }
                else
                {
                    workbook = null;
                    return;
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
        /// 每日数据
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="list"></param>
        /// <param name="monthtarget"></param>
        /// <param name="day"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        private DataTable GetReport(DayReportDep dep, List<DailyReportInfo> list, MonthlyTargetInfo monthtarget, DateTime day, ref int days)
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

                #region 每日数据

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    rowCheck[i] = "1";
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
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                tbl.Rows.Add(rowCheck);
                #endregion
            }
            else if (dep == DayReportDep.售后部 && DateTime.TryParse(txtDate.Text + "-01", out day))
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
                        if (data.Exists(d => d.ContainsKey(rlist[i].ID.ToString()) && !string.IsNullOrEmpty(d[rlist[i].ID.ToString()])))
                        {
                            rows[i]["合计"] = data.FindLast(d => d.ContainsKey(rlist[i].ID.ToString()) && !string.IsNullOrEmpty(d[rlist[i].ID.ToString()]))[rlist[i].ID.ToString()];
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

                #region 每日数据

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    rowCheck[i] = "1";
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
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                tbl.Rows.Add(rowCheck);
                #endregion
            }
            else if (dep == DayReportDep.二手车部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);
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
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.售后部, day, true);
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
                        if (data.Exists(d => d.ContainsKey(rlist[i].ID.ToString()) && !string.IsNullOrEmpty(d[rlist[i].ID.ToString()])))
                        {
                            rows[i + 3]["合计"] = data.FindLast(d => d.ContainsKey(rlist[i].ID.ToString()) && !string.IsNullOrEmpty(d[rlist[i].ID.ToString()]))[rlist[i].ID.ToString()];
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

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    rowCheck[i] = "1";
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
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
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
                tbl.Rows.Add(rowCheck);
                #endregion
            }
            else if (dep == DayReportDep.市场部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);
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

                DataRow[] rows = new DataRow[rlist.Count + 2];

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

                #endregion

                #region 完成率

                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["完成率"] = DataConvert.SafeDecimal(rows[i]["目标值"]) == 0 ? string.Empty : Math.Round(DataConvert.SafeDecimal(rows[i]["合计"]) * 100 / DataConvert.SafeDecimal(rows[i]["目标值"]), 0).ToString();
                }

                #endregion

                #region 每日数据

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    rowCheck[i] = "1";
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            for (int j = 1; j < rlist.Count; j++)
                            {
                                rows[j + 2][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                            }
                        }
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
                    }
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportModuleInfo m = rlist.Find(l => l.Name == "展厅本月留存客户数");
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
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
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_xs.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "留档批次");
                        DailyReportInfo r = list_xs.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            rows[2][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                        }
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                tbl.Rows.Add(rowCheck);
                #endregion
            }
            else if (dep == DayReportDep.DCC部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> xslist = DailyReports.Instance.GetList(query, true);
                xslist = xslist.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo xsmonthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);
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

                #region 每日数据

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    rowCheck[i] = "1";
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            int domun = 0;
                            for (int j = 0; j < rlist.Count; j++)
                            {
                                rows[j + domun][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                                if (rlist[j].Name == "CRM下达线索数")
                                {
                                    int idztdrzr = rlist.Find(l => l.Name == "展厅当日转入").ID;
                                    decimal drztdrzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztdrzr.ToString()) ? reportdate[idztdrzr.ToString()] : string.Empty);
                                    int idztsydrzr = rlist.Find(l => l.Name == "展厅上月当日转入").ID;
                                    decimal drztsydrzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztsydrzr.ToString()) ? reportdate[idztsydrzr.ToString()] : string.Empty);
                                    int idztshydrzr = rlist.Find(l => l.Name == "展厅双月当日转入").ID;
                                    decimal drztshydrzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztshydrzr.ToString()) ? reportdate[idztshydrzr.ToString()] : string.Empty);
                                    int idztsyyqzzr = rlist.Find(l => l.Name == "展厅三月有强制转入").ID;
                                    decimal drztsyyqzzr = DataConvert.SafeDecimal(reportdate.ContainsKey(idztsyyqzzr.ToString()) ? reportdate[idztsyyqzzr.ToString()] : string.Empty);

                                    domun++;
                                    rows[j + domun][i] = drztdrzr + drztsydrzr + drztshydrzr + drztsyyqzzr;
                                }
                                if (rlist[j].Name == "首次邀约到店客户总数")
                                {
                                    int idqczjyydd = rlist.Find(l => l.Name == "汽车之家邀约到店").ID;
                                    decimal drqczjyydd = DataConvert.SafeDecimal(reportdate.ContainsKey(idqczjyydd.ToString()) ? reportdate[idqczjyydd.ToString()] : string.Empty);
                                    int idycwyydd = rlist.Find(l => l.Name == "易车网邀约到店").ID;
                                    decimal drycwyydd = DataConvert.SafeDecimal(reportdate.ContainsKey(idycwyydd.ToString()) ? reportdate[idycwyydd.ToString()] : string.Empty);
                                    int idtpyyydd = rlist.Find(l => l.Name == "太平洋汽车网邀约到店").ID;
                                    decimal drtpyyydd = DataConvert.SafeDecimal(reportdate.ContainsKey(idtpyyydd.ToString()) ? reportdate[idtpyyydd.ToString()] : string.Empty);
                                    int idqtwlyydd = rlist.Find(l => l.Name == "其他网络邀约到店").ID;
                                    decimal drqtwlyydd = DataConvert.SafeDecimal(reportdate.ContainsKey(idqtwlyydd.ToString()) ? reportdate[idqtwlyydd.ToString()] : string.Empty);

                                    domun++;
                                    rows[j + domun][i] = drqczjyydd + drycwyydd + drtpyyydd + drqtwlyydd;
                                }
                                if (rlist[j].Name == "DCC成交总台数")
                                {
                                    int idqczjcjs = rlist.Find(l => l.Name == "汽车之家成交数").ID;
                                    decimal drqczjcjs = DataConvert.SafeDecimal(reportdate.ContainsKey(idqczjcjs.ToString()) ? reportdate[idqczjcjs.ToString()] : string.Empty);
                                    int idycwcjs = rlist.Find(l => l.Name == "易车网成交数").ID;
                                    decimal drycwcjs = DataConvert.SafeDecimal(reportdate.ContainsKey(idycwcjs.ToString()) ? reportdate[idycwcjs.ToString()] : string.Empty);
                                    int idtpycjs = rlist.Find(l => l.Name == "太平洋成交数").ID;
                                    decimal drtpycjs = DataConvert.SafeDecimal(reportdate.ContainsKey(idtpycjs.ToString()) ? reportdate[idtpycjs.ToString()] : string.Empty);
                                    int idqtwlcjs = rlist.Find(l => l.Name == "其他网络成交数").ID;
                                    decimal drqtwlcjs = DataConvert.SafeDecimal(reportdate.ContainsKey(idqtwlcjs.ToString()) ? reportdate[idqtwlcjs.ToString()] : string.Empty);

                                    domun++;
                                    rows[j + domun][i] = drqczjcjs + drycwcjs + drtpycjs + drqtwlcjs;
                                }
                            }
                        }
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
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
                tbl.Rows.Add(rowCheck);
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

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

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
                    rowCheck[i] = "1";
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
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                tbl.Rows.Add(rowCheck);

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

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    rowCheck[i] = "1";
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
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                tbl.Rows.Add(rowCheck);
                #endregion
            }
            else if (dep == DayReportDep.无忧产品 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);
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
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.售后部, day, true);
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

                #region 每日数据

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    rowCheck[i] = "1";
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_xs.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list_xs.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);

                            for (int j = 0; j < nlist_xs.Count; j++)
                            {
                                DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == nlist_xs[j].Key);
                                if (nlist_xs[j].Key.IndexOf("金额") > 0)
                                    rows[j][i] = reportdate.ContainsKey(m.ID.ToString()) ? (DataConvert.SafeDecimal(reportdate[m.ID.ToString()]) / 10000).ToString() : string.Empty;
                                else
                                    rows[j][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                            }
                        }
                    }
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            for (int j = 0; j < rlist.Count; j++)
                            {
                                if(rlist[j].Name.IndexOf("金额") > 0)
                                    rows[j + 13][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? (DataConvert.SafeDecimal(reportdate[rlist[j].ID.ToString()]) / 10000).ToString() : string.Empty;
                                else
                                    rows[j + 13][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                            }
                        }
                        rowCheck[i] = r.DailyReportCheckStatus == DailyReportCheckStatus.未审核 ? "0" : "1";
                    }
                    if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_sh.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                    {
                        DailyReportModuleInfo m = rlist_sh.Find(l => l.Name == "续保数");
                        DailyReportInfo r = list_sh.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                        if (!string.IsNullOrEmpty(r.SCReport))
                        {
                            Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                            rows[rlist.Count + 13][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                        }
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                tbl.Rows.Add(rowCheck);
                #endregion
            }
            else if (dep == DayReportDep.客服部 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                #region 销售数据

                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);
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
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.售后部, day, true);
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
                        rows[i + otherrowcount]["合计"] = hjxshf + hjqkhf+ hjsbtx+ hjdbyy+hjhdzl+hjlszl + hjsghf + hjqxhf;
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

                #region 每日数据

                DataRow rowCheck = tbl.NewRow();
                rowCheck["项目"] = "是否审核";

                for (int i = 1; i <= days; i++)
                {
                    otherrowcount = 0;
                    rowCheck[i] = "1";
                    for (int j = 0; j < rlist.Count; j++)
                    {
                        if (rlist[j].Name == "销售回访成功数")
                        {
                            if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_xs.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                            {
                                DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "展厅交车台数");
                                DailyReportInfo r = list_xs.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                                if (!string.IsNullOrEmpty(r.SCReport))
                                {
                                    Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                                    rows[j + otherrowcount][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                                }
                            }
                            otherrowcount++;
                        }
                        if (rlist[j].Name == "潜客回访数")
                        {
                            if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_xs.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                            {
                                DailyReportModuleInfo m = rlist_xs.Find(l => l.Name == "留档批次");
                                DailyReportInfo r = list_xs.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                                if (!string.IsNullOrEmpty(r.SCReport))
                                {
                                    Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                                    rows[j + otherrowcount][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                                }
                            }
                            otherrowcount++;
                        }
                        if (rlist[j].Name == "首保到期提醒数")
                        {
                            if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_sh.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                            {
                                DailyReportModuleInfo m = rlist_sh.Find(l => l.Name == "来厂台次");
                                DailyReportInfo r = list_sh.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                                if (!string.IsNullOrEmpty(r.SCReport))
                                {
                                    Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                                    rows[j + otherrowcount][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                                }
                            }
                            otherrowcount++;
                        }
                        if (rlist[j].Name == "事故信息回访量")
                        {
                            if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                            {
                                DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                                if (!string.IsNullOrEmpty(r.SCReport))
                                {
                                    Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                                    DailyReportModuleInfo m = rlist.Find(l => l.Name == "销售回访成功数");
                                    decimal xshf = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    m = rlist.Find(l => l.Name == "潜客回访数");
                                    decimal qkhf = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    m = rlist.FindAll(l => l.Name == "呼出电话量")[0];
                                    decimal sbtx = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    m = rlist.FindAll(l => l.Name == "呼出电话量")[1];
                                    decimal dbyy = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    m = rlist.Find(l => l.Name == "活动招揽电话量");
                                    decimal hdzl = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    m = rlist.Find(l => l.Name == "流失客户招揽电话量");
                                    decimal lszl = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    m = rlist.Find(l => l.Name == "事故信息回访量");
                                    decimal sghf = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    m = rlist.Find(l => l.Name == "抢修回访数");
                                    decimal qxhf = DataConvert.SafeDecimal(reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty);
                                    rows[j + otherrowcount][i] = xshf + qkhf + sbtx + dbyy + hdzl + lszl + sghf + qxhf;
                                }
                            }
                            otherrowcount++;

                            if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list_sh.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                            {
                                DailyReportModuleInfo m = rlist_sh.Find(l => l.Name == "收到信息数");
                                DailyReportInfo r = list_sh.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                                if (!string.IsNullOrEmpty(r.SCReport))
                                {
                                    Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                                    rows[j + otherrowcount][i] = reportdate.ContainsKey(m.ID.ToString()) ? reportdate[m.ID.ToString()] : string.Empty;
                                }
                            }
                            otherrowcount++;
                        }
                        if (DateTime.TryParse(txtDate.Text + "-" + i.ToString("00"), out day) && list.Exists(l => l.DayUnique == day.ToString("yyyyMMdd")))
                        {
                            DailyReportInfo r = list.Find(l => l.DayUnique == day.ToString("yyyyMMdd"));
                            if (!string.IsNullOrEmpty(r.SCReport))
                            {
                                Dictionary<string, string> reportdate = json.Deserialize<Dictionary<string, string>>(r.SCReport);
                                rows[j + otherrowcount][i] = reportdate.ContainsKey(rlist[j].ID.ToString()) ? reportdate[rlist[j].ID.ToString()] : string.Empty;
                            }
                        }
                    }
                }

                #endregion

                foreach (DataRow row in rows)
                {
                    tbl.Rows.Add(row);
                }
                tbl.Rows.Add(rowCheck);
                #endregion
            }

            return tbl;
        }

        /// <summary>
        /// 每日数据页面输出
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="tbl"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        private string GetReportStr(DayReportDep dep, DataTable tbl, int days)
        {
            StringBuilder strb = new StringBuilder();
            if (dep == DayReportDep.客服部 || dep == DayReportDep.二手车部 || dep == DayReportDep.DCC部 || dep == DayReportDep.精品部 || dep == DayReportDep.市场部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w160\">项目</td>");
                strb.Append("<td class=\"w80\">目标值</td>");
                strb.Append("<td class=\"w80\">合计</td>");
                strb.Append("<td class=\"w80\">完成率</td>");

                tbl.DefaultView.RowFilter = "项目='是否审核'";
                for (int i = 1; i <= days; i++)
                {
                    string ischeck = tbl.DefaultView[0][i].ToString();
                    strb.AppendFormat("<td class=\"w40\">{0}</td>", ischeck == "1" ? i.ToString() : string.Format("<span class=\"red\">{0}</span>", i));
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["项目"].ToString() == "是否审核") continue;
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", row["项目"]);
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["目标值"].ToString()) ? "&nbsp;" : FormatNum(row["目标值"].ToString()));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["合计"].ToString()) ? "&nbsp;" : GetCellValue(FormatNum(row["合计"].ToString()), string.Empty, !string.IsNullOrEmpty(row["完成率"].ToString()) && DataConvert.SafeDecimal(row["完成率"]) < 100, false));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["完成率"].ToString()) ? "&nbsp;" : (FormatNum(row["完成率"].ToString()) + "%"));
                    for (int i = 1; i <= days; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row[i].ToString()) ? "&nbsp;" : row[i].ToString());
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.销售部)
            {
                #region 页面输出

                string[] tpp = new string[] { "其中他品牌新增订单台次", "他品牌交车台次", "他品牌单车毛利", "他品牌单车综合毛利" };

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w160\">项目</td>");
                strb.Append("<td class=\"w80\">目标值</td>");
                strb.Append("<td class=\"w80\">合计</td>");
                strb.Append("<td class=\"w80\">完成率</td>");

                tbl.DefaultView.RowFilter = "项目='是否审核'";
                for (int i = 1; i <= days; i++)
                {
                    string ischeck = tbl.DefaultView[0][i].ToString();
                    strb.AppendFormat("<td class=\"w40\">{0}</td>", ischeck == "1" ? i.ToString() : string.Format("<span class=\"red\">{0}</span>", i));
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["项目"].ToString() == "是否审核") continue;
                    if (tpp.Contains(row["项目"].ToString()) && CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 0)
                        continue;
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", row["项目"]);
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["目标值"].ToString()) ? "&nbsp;" : FormatNum(row["目标值"].ToString()));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["合计"].ToString()) ? "&nbsp;" : GetCellValue(FormatNum(row["合计"].ToString()), string.Empty, !string.IsNullOrEmpty(row["完成率"].ToString()) && DataConvert.SafeDecimal(row["完成率"]) < 100, false));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["完成率"].ToString()) ? "&nbsp;" : (FormatNum(row["完成率"].ToString()) + "%"));
                    for (int i = 1; i <= days; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row[i].ToString()) ? "&nbsp;" : row[i].ToString());
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.售后部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w160\">项目</td>");
                strb.Append("<td class=\"w80\">目标值</td>");
                strb.Append("<td class=\"w80\">合计</td>");
                strb.Append("<td class=\"w80\">完成率</td>");

                tbl.DefaultView.RowFilter = "项目='是否审核'";
                for (int i = 1; i <= days; i++)
                {
                    string ischeck = tbl.DefaultView[0][i].ToString();
                    strb.AppendFormat("<td class=\"w40\">{0}</td>", ischeck == "1" ? i.ToString() : string.Format("<span class=\"red\">{0}</span>", i));
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["项目"].ToString() == "是否审核") continue;
                    string[] tsjestr = new string[] { "中保理赔", "太保", "平安", "人寿", "大地", "中华联合", "浙商", "大众", "其他", "旧件利用额" };
                    bool ismoney = row["项目"].ToString().IndexOf("金额") > 0 || row["项目"].ToString().IndexOf("产值") > 0 || tsjestr.Contains(row["项目"].ToString());
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", row["项目"]);
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["目标值"].ToString()) ? "&nbsp;" : FormatNum(row["目标值"].ToString()));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["合计"].ToString()) ? "&nbsp;" : GetCellValue(FormatNum(row["合计"].ToString()), string.Empty, DataConvert.SafeDecimal(row["完成率"]) < 100, false));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["完成率"].ToString()) ? "&nbsp;" : (FormatNum(row["完成率"].ToString()) + "%"));
                    for (int i = 1; i <= days; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row[i].ToString()) ? "&nbsp;" : (ismoney ? GetMoneyStr(DataConvert.SafeFloat(row[i])) : row[i].ToString()));
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.金融部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w160\">项目</td>");
                strb.Append("<td class=\"w80\">目标值</td>");
                strb.Append("<td class=\"w80\">合计</td>");
                strb.Append("<td class=\"w80\">完成率</td>");

                tbl.DefaultView.RowFilter = "项目='是否审核'";
                for (int i = 1; i <= days; i++)
                {
                    string ischeck = tbl.DefaultView[0][i].ToString();
                    strb.AppendFormat("<td class=\"w40\">{0}</td>", ischeck == "1" ? i.ToString() : string.Format("<span class=\"red\">{0}</span>", i));
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["项目"].ToString() == "是否审核") continue;
                    bool ismoney = row["项目"].ToString().IndexOf("金额") > 0 || row["项目"].ToString().IndexOf("产值") > 0;
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", row["项目"]);
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["目标值"].ToString()) ? "&nbsp;" : FormatNum(row["目标值"].ToString()));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["合计"].ToString()) ? "&nbsp;" : GetCellValue(FormatNum(row["合计"].ToString()), string.Empty, !string.IsNullOrEmpty(row["完成率"].ToString()) && DataConvert.SafeDecimal(row["完成率"]) < 100, false));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["完成率"].ToString()) ? "&nbsp;" : (FormatNum(row["完成率"].ToString()) + "%"));
                    for (int i = 1; i <= days; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row[i].ToString()) ? "&nbsp;" : (ismoney ? GetMoneyStr(DataConvert.SafeFloat(row[i])) : row[i].ToString()));
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.财务部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w160\">项目（万元）</td>");
                strb.Append("<td class=\"w80\">期初余额</td>");
                strb.Append("<td class=\"w80\">合计</td>");

                tbl.DefaultView.RowFilter = "项目='是否审核'";
                for (int i = 1; i <= days; i++)
                {
                    string ischeck = tbl.DefaultView[0][i].ToString();
                    strb.AppendFormat("<td class=\"w40\">{0}</td>", ischeck == "1" ? i.ToString() : string.Format("<span class=\"red\">{0}</span>", i));
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["项目"].ToString() == "是否审核") continue;
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", row["项目"]);
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["期初余额"].ToString()) ? "&nbsp;" : FormatNum(row["期初余额"].ToString()));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["合计"].ToString()) ? "&nbsp;" : FormatNum(row["合计"].ToString()));
                    for (int i = 1; i <= days; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row[i].ToString()) ? "&nbsp;" : GetMoneyStr(DataConvert.SafeFloat(row[i].ToString())));
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.行政部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w160\">项目</td>");
                strb.Append("<td class=\"w80\">目标值</td>");
                strb.Append("<td class=\"w80\">合计</td>");

                tbl.DefaultView.RowFilter = "项目='是否审核'";
                for (int i = 1; i <= days; i++)
                {
                    string ischeck = tbl.DefaultView[0][i].ToString();
                    strb.AppendFormat("<td class=\"w40\">{0}</td>", ischeck == "1" ? i.ToString() : string.Format("<span class=\"red\">{0}</span>", i));
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["项目"].ToString() == "是否审核") continue;
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", row["项目"]);
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["目标值"].ToString()) ? "&nbsp;" : FormatNum(row["目标值"].ToString()));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["合计"].ToString()) ? "&nbsp;" : row["合计"].ToString());
                    for (int i = 1; i <= days; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row[i].ToString()) ? "&nbsp;" : row[i].ToString());
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.无忧产品)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id=\"tbData\" class=\"datatable\">");
                strb.Append("<tr class=\"bold tc bggray\">");
                strb.Append("<td class=\"w160\">项目</td>");
                strb.Append("<td class=\"w80\">目标值</td>");
                strb.Append("<td class=\"w80\">合计</td>");
                strb.Append("<td class=\"w80\">完成率</td>");

                tbl.DefaultView.RowFilter = "项目='是否审核'";
                for (int i = 1; i <= days; i++)
                {
                    string ischeck = tbl.DefaultView[0][i].ToString();
                    strb.AppendFormat("<td class=\"w40\">{0}</td>", ischeck == "1" ? i.ToString() : string.Format("<span class=\"red\">{0}</span>", i));
                }
                strb.Append("<td></td>");
                strb.Append("</tr>");

                foreach (DataRow row in tbl.Rows)
                {
                    if (row["项目"].ToString() == "是否审核") continue;
                    if (row["项目"].ToString() == "新车机油套餐购买个数")
                        strb.AppendFormat("<tr><td colspan=\"{0}\" style=\"color:White;background-color:gray;font-weight:bold;font-size:large;padding-left:20px;\">销售数据</td></tr>", tbl.Columns.Count + 1);
                    else if (row["项目"].ToString() == "售后机油套餐购买个数")
                        strb.AppendFormat("<tr><td colspan=\"{0}\" style=\"color:White;background-color:gray;font-weight:bold;font-size:large;padding-left:20px;\">售后数据</td></tr>", tbl.Columns.Count + 1);
                    else if (row["项目"].ToString() == "当月来厂基盘车辆数≤18个月")
                        strb.AppendFormat("<tr><td colspan=\"{0}\" style=\"color:White;background-color:gray;font-weight:bold;font-size:large;padding-left:20px;\">来厂基盘车辆</td></tr>", tbl.Columns.Count  + 1);
                    strb.Append("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", row["项目"].ToString() != "续保出单量" ? row["项目"].ToString().Replace("新车", string.Empty).Replace("售后", string.Empty).Replace("续保", string.Empty).Replace("当月来厂基盘车辆数", string.Empty) : row["项目"].ToString());
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["目标值"].ToString()) ? "&nbsp;" : FormatNum(row["目标值"].ToString()));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["合计"].ToString()) ? "&nbsp;" : GetCellValue(FormatNum(row["合计"].ToString()), string.Empty, !string.IsNullOrEmpty(row["完成率"].ToString()) && DataConvert.SafeDecimal(row["完成率"]) < 100, false));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row["完成率"].ToString()) ? "&nbsp;" : (FormatNum(row["完成率"].ToString()) + "%"));
                    for (int i = 1; i <= days; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(row[i].ToString()) ? "&nbsp;" : row[i].ToString());
                    }
                    strb.Append("<td></td>");
                    strb.Append("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }

            return strb.ToString();
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

        /// <summary>
        /// 关键指标数据
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="list"></param>
        /// <param name="monthtarget"></param>
        /// <returns></returns>
        private DataTable GetKeyReport(DayReportDep dep, List<DailyReportInfo> list, MonthlyTargetInfo monthtarget, DataTable data)
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
                tbl.Columns.Add("预算");

                #endregion

                #region 留单,标准总库存

                decimal ld = 0;
                DateTime day = DateTime.Today;
                if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_last = new DailyReportQuery()
                    {
                        DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
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

                MonthlyTargetInfo monthtargetpre = MonthlyTargetPres.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);

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

                decimal hjtppjctc = 0;
                decimal mbtppjctc = 0;
                decimal hjtppdcml = 0;
                decimal mbtppdcml = 0;
                decimal hjtppzhml = 0;
                decimal mbtppzhml = 0;
                decimal hjqztppxzddtc = 0;
                decimal mbqztppxzddtc = 0;
                if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                {
                    data.DefaultView.RowFilter = "项目='他品牌交车台次'";
                    hjtppjctc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                    mbtppjctc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                    data.DefaultView.RowFilter = "项目='他品牌单车毛利'";
                    hjtppdcml = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                    mbtppdcml = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                    data.DefaultView.RowFilter = "项目='他品牌单车综合毛利'";
                    hjtppzhml = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                    mbtppzhml = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                    data.DefaultView.RowFilter = "项目='其中他品牌新增订单台次'";
                    hjqztppxzddtc = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                    mbqztppxzddtc = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "总销售台次";
                    rows[0]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSzxstc)) ? monthtarget.XSzxstc : (mbztjcts + mbewxstc + mbtppjctc).ToString();
                    rows[0]["实际"] = hjztjcts + hjewxstc + hjtppjctc;
                    rows[0]["详细"] = (hjztjcts + hjewxstc + hjtppjctc) == 0 ? string.Empty : string.Format("<br />({0}/{1}/{2})", hjztjcts, hjewxstc, hjtppjctc);
                    rows[0]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSzxstc;

                    rows[1] = tbl.NewRow();
                    rows[1]["关键指标"] = "展厅占比";
                    rows[1]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztzb)) ? monthtarget.XSztzb : ((mbztjcts + mbewxstc + mbtppjctc) == 0 ? string.Empty : Math.Round((mbztjcts + mbtppjctc) * 100 / (mbztjcts + mbewxstc + mbtppjctc), 0).ToString());
                    rows[1]["实际"] = (hjztjcts + hjewxstc + hjtppjctc) == 0 ? string.Empty : Math.Round((hjztjcts + hjtppjctc) * 100 / (hjztjcts + hjewxstc + hjtppjctc), 0).ToString();
                    rows[1]["详细"] = (hjztjcts + hjewxstc + hjtppjctc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztjcts + hjtppjctc, 0), Math.Round(hjztjcts + hjewxstc + hjtppjctc, 0));
                    rows[1]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztzb;
                }
                else
                {

                    rows[0] = tbl.NewRow();
                    rows[0]["关键指标"] = "总销售台次";
                    rows[0]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSzxstc)) ? monthtarget.XSzxstc : (mbztjcts + mbewxstc).ToString();
                    rows[0]["实际"] = hjztjcts + hjewxstc;
                    rows[0]["详细"] = (hjztjcts + hjewxstc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", hjztjcts, hjewxstc);
                    rows[0]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSzxstc;

                    rows[1] = tbl.NewRow();
                    rows[1]["关键指标"] = "展厅占比";
                    rows[1]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztzb)) ? monthtarget.XSztzb : ((mbztjcts + mbewxstc) == 0 ? string.Empty : Math.Round(mbztjcts * 100 / (mbztjcts + mbewxstc), 0).ToString());
                    rows[1]["实际"] = (hjztjcts + hjewxstc) == 0 ? string.Empty : Math.Round(hjztjcts * 100 / (hjztjcts + hjewxstc), 0).ToString();
                    rows[1]["详细"] = (hjztjcts + hjewxstc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztjcts, 0), Math.Round(hjztjcts + hjewxstc, 0));
                    rows[1]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztzb;
                }

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "展厅留档率";
                rows[2]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztldl)) ? monthtarget.XSztldl : (mbztsclkpc == 0 ? string.Empty : Math.Round(mbldpc * 100 / mbztsclkpc, 0).ToString());
                rows[2]["实际"] = hjztsclkpc == 0 ? string.Empty : Math.Round(hjldpc * 100 / hjztsclkpc, 0).ToString();
                rows[2]["详细"] = hjztsclkpc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjldpc, 0), Math.Round(hjztsclkpc, 0));
                rows[2]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztldl;

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "展厅成交率";
                rows[3]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztcjl)) ? monthtarget.XSztcjl : (mbldpc == 0 ? string.Empty : Math.Round((mbztddts - ld) * 100 / mbldpc, 0).ToString());
                rows[3]["实际"] = hjldpc == 0 ? string.Empty : Math.Round((hjztddts - ld) * 100 / hjldpc, 0).ToString();
                rows[3]["详细"] = hjldpc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztddts - ld, 0), Math.Round(hjldpc, 0));
                rows[3]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztcjl;

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "上牌率";
                rows[4]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSspl)) ? monthtarget.XSspl : (mbztjcts == 0 ? string.Empty : Math.Round(mbsptc * 100 / mbztjcts, 0).ToString());
                rows[4]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjsptc * 100 / hjztjcts, 0).ToString();
                rows[4]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsptc, 0), Math.Round(hjztjcts, 0));
                rows[4]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSspl;

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "上牌单台";
                rows[5]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSspdt)) ? monthtarget.XSspdt : (mbsptc == 0 ? string.Empty : Math.Round(mbspzje / mbsptc, 0).ToString());
                rows[5]["实际"] = hjsptc == 0 ? string.Empty : Math.Round(hjspzje / hjsptc, 0).ToString();
                rows[5]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSspdt;

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "展厅保险率";
                rows[6]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztbxl)) ? monthtarget.XSztbxl : (mbztjcts == 0 ? string.Empty : Math.Round(mbbxtc * 100 / mbztjcts, 0).ToString());
                rows[6]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjbxtc * 100 / hjztjcts, 0).ToString();
                rows[6]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjbxtc, 0), Math.Round(hjztjcts, 0));
                rows[6]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztbxl;

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "展厅保险单台";
                rows[7]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztbxdt)) ? monthtarget.XSztbxdt : (mbbxtc == 0 ? string.Empty : Math.Round(mbbxzje / mbbxtc, 0).ToString());
                rows[7]["实际"] = hjbxtc == 0 ? string.Empty : Math.Round(hjbxzje / hjbxtc, 0).ToString();
                rows[7]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztbxdt;

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "美容交车率";
                rows[8]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmrjcl)) ? monthtarget.XSmrjcl : (mbztjcts == 0 ? string.Empty : Math.Round(mbmrjctc * 100 / mbztjcts, 0).ToString());
                rows[8]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjmrjctc * 100 / hjztjcts, 0).ToString();
                rows[8]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjmrjctc, 0), Math.Round(hjztjcts, 0));
                rows[8]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSmrjcl;

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "美容单台";
                rows[9]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmrdt)) ? monthtarget.XSmrdt : (mbmrjctc == 0 ? string.Empty : Math.Round(mbmrjczje / mbmrjctc, 0).ToString());
                rows[9]["实际"] = hjmrjctc == 0 ? string.Empty : Math.Round(hjmrjczje / hjmrjctc, 0).ToString();
                rows[9]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSmrdt;

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "展厅精品前装率";
                rows[10]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztjpqzl)) ? monthtarget.XSztjpqzl : (mbztjcts == 0 ? string.Empty : Math.Round(mbztcjjptc * 100 / mbztjcts, 0).ToString());
                rows[10]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjztcjjptc * 100 / hjztjcts, 0).ToString();
                rows[10]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztcjjptc, 0), Math.Round(hjztjcts, 0));
                rows[10]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztjpqzl;

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "展厅精品平均单台";
                rows[11]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSztjppjdt)) ? monthtarget.XSztjppjdt : (mbztcjjptc == 0 ? string.Empty : Math.Round(mbjpzje / mbztjcts, 0).ToString());
                rows[11]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjjpzje / hjztjcts, 0).ToString();
                rows[11]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSztjppjdt;

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "二网精品平均单台";
                rows[12]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSewjppjdt)) ? monthtarget.XSewjppjdt : (mbewxstc == 0 ? string.Empty : Math.Round(mbewjpje / mbewxstc, 0).ToString());
                rows[12]["实际"] = hjewxstc == 0 ? string.Empty : Math.Round(hjewjpje / hjewxstc, 0).ToString();
                rows[12]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSewjppjdt;

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "销售置换台次";
                rows[13]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSxszhtc)) ? monthtarget.XSxszhtc : mbxszhs.ToString();
                rows[13]["实际"] = hjxszhs;
                rows[13]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSxszhtc;

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "按揭率";
                rows[14]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSajl)) ? monthtarget.XSajl : (mbztjcts == 0 ? string.Empty : Math.Round((mbbdajtc + mbyhajtc + mbcjjrtc) * 100 / mbztjcts, 0).ToString());
                rows[14]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round((hjbdajtc + hjyhajtc + hjcjjrtc) * 100 / hjztjcts, 0).ToString();
                rows[14]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjbdajtc + hjyhajtc + hjcjjrtc, 0), Math.Round(hjztjcts, 0));
                rows[14]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSajl;

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "按揭平均单台";
                rows[15]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSajpjdt)) ? monthtarget.XSajpjdt : ((mbbdajtc + mbyhajtc + mbcjjrtc) == 0 ? string.Empty : Math.Round(mbajjsr / (mbbdajtc + mbyhajtc + mbcjjrtc), 0).ToString());
                rows[15]["实际"] = (hjbdajtc + hjyhajtc + hjcjjrtc) == 0 ? string.Empty : Math.Round(hjajjsr / (hjbdajtc + hjyhajtc + hjcjjrtc), 0).ToString();
                rows[15]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSajpjdt;

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "免费保养渗透率";
                rows[16]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmfbystl)) ? monthtarget.XSmfbystl : (mbztjcts == 0 ? string.Empty : Math.Round(mbmfbytc * 100 / mbztjcts, 0).ToString());
                rows[16]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjmfbytc * 100 / hjztjcts, 0).ToString();
                rows[16]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjmfbytc, 0), Math.Round(hjztjcts, 0));
                rows[16]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSmfbystl;

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "免费保养单台";
                rows[17]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSmfbydt)) ? monthtarget.XSmfbydt : (mbmfbytc == 0 ? string.Empty : Math.Round(mbmfbyzje / mbmfbytc, 0).ToString());
                rows[17]["实际"] = hjmfbytc == 0 ? string.Empty : Math.Round(hjmfbyzje / hjmfbytc, 0).ToString();
                rows[17]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSmfbydt;

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
                rows[21]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})",Math.Round(hjqzlkhzjsjcts,0),Math.Round(hjztjcts, 0));
                rows[21]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSzjsl;

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "展厅新增订单";
                rows[22]["目标"] = Math.Round(mbztddts - ld, 0).ToString();
                rows[22]["实际"] = Math.Round(hjztddts - ld, 0).ToString();

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "无忧玻璃渗透率";
                rows[23]["目标"] = mbbxtc == 0 ? string.Empty : Math.Round(mbblxtc * 100 / mbbxtc, 0).ToString();
                rows[23]["实际"] = hjbxtc == 0 ? string.Empty : Math.Round(hjblxtc * 100 / hjbxtc, 0).ToString();
                rows[23]["详细"] = hjbxtc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjblxtc, 0), Math.Round(hjbxtc, 0));

                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "无忧延保渗透率";
                rows[24]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbybtc * 100 / mbztjcts, 0).ToString();
                rows[24]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjybtc * 100 / hjztjcts, 0).ToString();
                rows[24]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjybtc, 0), Math.Round(hjztjcts, 0));

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "机油套餐渗透率";
                rows[25]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbjytcgmgs * 100 / mbztjcts, 0).ToString();
                rows[25]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjjytcgmgs * 100 / hjztjcts, 0).ToString();
                rows[25]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjjytcgmgs, 0), Math.Round(hjztjcts, 0));

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "无忧划痕渗透率";
                rows[26]["目标"] = mbbxtc == 0 ? string.Empty : Math.Round(mbhhwyfwgmgs * 100 / mbbxtc, 0).ToString();
                rows[26]["实际"] = hjbxtc == 0 ? string.Empty : Math.Round(hjhhwyfwgmgs * 100 / hjbxtc, 0).ToString();
                rows[26]["详细"] = hjbxtc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjhhwyfwgmgs, 0), Math.Round(hjbxtc, 0));

                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "试乘试驾率";
                rows[27]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XSscsjl)) ? monthtarget.XSscsjl : ((mbztddts - ld) == 0 ? string.Empty : Math.Round(mbscsjs * 100 / (mbztddts - ld), 0).ToString());
                rows[27]["实际"] = (hjztddts - ld) == 0 ? string.Empty : Math.Round(hjscsjs * 100 / (hjztddts - ld), 0).ToString();
                rows[27]["详细"] = (hjztddts - ld) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjscsjs,0) , Math.Round(hjztddts - ld,0));
                rows[27]["预算"] = monthtargetpre == null ? string.Empty : monthtargetpre.XSscsjl;

                rows[28] = tbl.NewRow();
                rows[28]["关键指标"] = "在库平均单台成本价";
                rows[28]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSclpjdj;

                rows[29] = tbl.NewRow();
                rows[29]["关键指标"] = "在库库存";
                rows[29]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc;

                rows[30] = tbl.NewRow();
                rows[30]["关键指标"] = "在途";
                rows[30]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSztcl;

                rows[31] = tbl.NewRow();
                rows[31]["关键指标"] = "总库存";
                rows[31]["实际"] = hjrktc - hjztjcts - hjewxstc - hjwlxstc + DataConvert.SafeInt(monthtarget == null ? string.Empty : monthtarget.XSztcl);

                rows[32] = tbl.NewRow();
                rows[32]["关键指标"] = "上月留单";
                rows[32]["实际"] = ld.ToString();

                rows[33] = tbl.NewRow();
                rows[33]["关键指标"] = "本月留单";
                rows[33]["实际"] = hjztddts - hjztjcts;

                rows[34] = tbl.NewRow();
                rows[34]["关键指标"] = "他品牌留单";
                rows[34]["实际"] = hjqztppxzddtc - hjtppjctc;

                rows[35] = tbl.NewRow();
                rows[35]["关键指标"] = "销售台次";
                rows[35]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStppxstc)) ? monthtarget.XStppxstc : mbtppjctc.ToString();
                rows[35]["实际"] = hjtppjctc;

                rows[36] = tbl.NewRow();
                rows[36]["关键指标"] = "单车毛利";
                rows[36]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStppdcml)) ? monthtarget.XStppdcml : mbtppdcml.ToString();
                rows[36]["实际"] = hjtppdcml;

                rows[37] = tbl.NewRow();
                rows[37]["关键指标"] = "综合毛利";
                rows[37]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStppzhml)) ? monthtarget.XStppzhml : mbtppzhml.ToString();
                rows[37]["实际"] = hjtppzhml;

                rows[38] = tbl.NewRow();
                rows[38]["关键指标"] = "平均单台";
                rows[38]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.XStpppjdt)) ? monthtarget.XStpppjdt : (mbtppjctc == 0 ? string.Empty : Math.Round((mbtppdcml + mbtppzhml) / mbtppjctc, 0).ToString());
                rows[38]["实际"] = hjtppjctc == 0 ? string.Empty : Math.Round((hjtppdcml + hjtppzhml) / hjtppjctc, 0).ToString();

                rows[39] = tbl.NewRow();
                rows[39]["关键指标"] = "厂家虚出";
                rows[39]["实际"] = monthtarget == null ? string.Empty : monthtarget.XScjxctc;

                rows[40] = tbl.NewRow();
                rows[40]["关键指标"] = "在库超3个月";
                rows[40]["实际"] = monthtarget == null ? string.Empty : monthtarget.XSzkcsgytc;

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
                //if (ddlCorp.SelectedIndex > 0 )
                //{
                //    DailyReportQuery query_all = new DailyReportQuery()
                //    {
                //        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
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

                DataRow[] rows = new DataRow[31];

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
                data.DefaultView.RowFilter = "项目='延保无忧'";
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
                data.DefaultView.RowFilter = "项目='玻璃无忧'";
                decimal hjblx = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbblx = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
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

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "来厂台次";
                rows[0]["目标"] = mblctc;
                rows[0]["实际"] = hjlctc;

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "预约率";
                rows[1]["目标"] = mblctc == 0 ? string.Empty : Math.Round(mbqzyytc * 100 / mblctc, 1).ToString();
                rows[1]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjqzyytc * 100 / hjlctc, 1).ToString();
                rows[1]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjqzyytc, 0), Math.Round(hjlctc, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "产值达成";
                rows[2]["目标"] = mbdrcz;
                rows[2]["实际"] = hjdrcz;

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "养护比例";

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "美容比例";
                rows[4]["目标"] = mbdrcz == 0 ? string.Empty : Math.Round(mbjpmrcz * 100 / mbdrcz, 1).ToString();
                rows[4]["实际"] = hjdrcz == 0 ? string.Empty : Math.Round(hjjpmrcz * 100 / hjdrcz, 1).ToString();
                rows[4]["详细"] = hjdrcz == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjjpmrcz, 0), Math.Round(hjdrcz, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "单台产值";
                rows[5]["目标"] = mblctc == 0 ? string.Empty : Math.Round(mbdrcz / mblctc, 1).ToString();
                rows[5]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjdrcz / hjlctc, 1).ToString();
                rows[5]["详细"] = string.Empty;

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "事故产值占比";
                rows[6]["目标"] = mbdrcz == 0 ? string.Empty : Math.Round(mbsgzcz * 100 / mbdrcz, 1).ToString();
                rows[6]["实际"] = hjdrcz == 0 ? string.Empty : Math.Round(hjsgzcz * 100 / hjdrcz, 1).ToString();
                rows[6]["详细"] = hjdrcz == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsgzcz, 0), Math.Round(hjdrcz, 0));

                rows[3]["目标"] = (mbdrcz - (DataConvert.SafeDecimal(rows[6]["目标"]) / 100 - (decimal)0.5) * mbdrcz) == 0 ? string.Empty : Math.Round(mbyhcz * 100 / ((mbdrcz - (DataConvert.SafeDecimal(rows[6]["目标"]) / 100 - (decimal)0.5) * mbdrcz)), 1).ToString();
                rows[3]["实际"] = (hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz) == 0 ? string.Empty : Math.Round(hjyhcz * 100 / ((hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz)), 1).ToString();
                rows[3]["详细"] = (hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjyhcz, 0), Math.Round(hjdrcz - (DataConvert.SafeDecimal(rows[6]["实际"]) / 100 - (decimal)0.5) * hjdrcz, 0));

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "事故首次成功率";
                rows[7]["目标"] = mbsdxxs == 0 ? string.Empty : Math.Round(mbcgtc * 100 / mbsdxxs, 1).ToString();
                rows[7]["实际"] = hjsdxxs == 0 ? string.Empty : Math.Round(hjcgtc * 100 / hjsdxxs, 1).ToString();
                rows[7]["详细"] = hjsdxxs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjcgtc, 0), Math.Round(hjsdxxs, 0));

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "事故再次成功率";
                rows[8]["目标"] = mbyxsl == 0 ? string.Empty : Math.Round(mbyxcgs * 100 / mbyxsl, 1).ToString();
                rows[8]["实际"] = hjyxsl == 0 ? string.Empty : Math.Round(hjyxcgs * 100 / hjyxsl, 1).ToString();
                rows[8]["详细"] = hjyxsl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjyxcgs, 0), Math.Round(hjyxsl, 0));

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "索赔成功率";
                rows[9]["目标"] = mbspsbtc == 0 ? string.Empty : Math.Round(mbsppztc * 100 / mbspsbtc, 1).ToString();
                rows[9]["实际"] = hjspsbtc == 0 ? string.Empty : Math.Round(hjsppztc * 100 / hjspsbtc, 1).ToString();
                rows[9]["详细"] = hjspsbtc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsppztc, 0), Math.Round(hjspsbtc, 0));

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "内返率";
                rows[10]["目标"] = mblctc == 0 ? string.Empty : Math.Round((mbjdnf + mbbpnf + mbwftc) * 100 / mblctc, 1).ToString();
                rows[10]["实际"] = hjlctc == 0 ? string.Empty : Math.Round((hjjdnf + hjbpnf + hjwftc) * 100 / hjlctc, 1).ToString();
                rows[10]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjjdnf + hjbpnf + hjwftc, 0), Math.Round(hjlctc, 0));

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "供货及时率";
                rows[11]["目标"] = mblctc == 0 ? string.Empty : Math.Round((1 - mbqjsl / mblctc) * 100, 1).ToString();
                rows[11]["实际"] = hjlctc == 0 ? string.Empty : Math.Round((1 - hjqjsl / hjlctc) * 100, 1).ToString();
                rows[11]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjlctc - hjqjsl, 0), Math.Round(hjlctc, 0));

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "保养单台产值";
                rows[12]["目标"] = mbbytc == 0 ? string.Empty : Math.Round(mbbycz / mbbytc, 1).ToString();
                rows[12]["实际"] = hjbytc == 0 ? string.Empty : Math.Round(hjbycz / hjbytc, 1).ToString();
                rows[12]["详细"] = string.Empty;

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "保养台次占比";
                rows[13]["目标"] = mblctc == 0 ? string.Empty : Math.Round(mbbytc * 100 / mblctc, 1).ToString();
                rows[13]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjbytc * 100 / hjlctc, 1).ToString();
                rows[13]["详细"] = hjlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjbytc, 0), Math.Round(hjlctc, 0));

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "精品美容产值";
                rows[14]["目标"] = mbjpmrcz;
                rows[14]["实际"] = hjjpmrcz;

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "延保达成";
                rows[15]["目标"] = mbybtc;
                rows[15]["实际"] = hjybtc;

                rows[16] = tbl.NewRow();
                rows[16]["关键指标"] = "事故总产值";
                rows[16]["目标"] = mbsgzcz;
                rows[16]["实际"] = hjsgzcz;

                rows[17] = tbl.NewRow();
                rows[17]["关键指标"] = "玻璃无忧";
                rows[17]["目标"] = mbblx;
                rows[17]["实际"] = hjblx;

                rows[18] = tbl.NewRow();
                rows[18]["关键指标"] = "中保";
                rows[18]["目标"] = hjzblp.ToString();
                rows[18]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzblp * 100 / hjsgzcz, 1).ToString();
                rows[18]["详细"] = mbzblp.ToString();

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "太保";
                rows[19]["目标"] = hjtb.ToString();
                rows[19]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjtb * 100 / hjsgzcz, 1).ToString();
                rows[19]["详细"] = mbtb.ToString();

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "平安";
                rows[20]["目标"] = hjpa.ToString();
                rows[20]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjpa * 100 / hjsgzcz, 1).ToString();
                rows[20]["详细"] = mbpa.ToString();

                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "人寿";
                rows[21]["目标"] = hjrs.ToString();
                rows[21]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjrs * 100 / hjsgzcz, 1).ToString();
                rows[21]["详细"] = mbrs.ToString();

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "大地";
                rows[22]["目标"] = hjdd.ToString();
                rows[22]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjdd * 100 / hjsgzcz, 1).ToString();
                rows[22]["详细"] = mbdd.ToString();

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "中华联合";
                rows[23]["目标"] = hjzhlh.ToString();
                rows[23]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzhlh * 100 / hjsgzcz, 1).ToString();
                rows[23]["详细"] = mbzhlh.ToString();

                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "浙商";
                rows[24]["目标"] = hjzs.ToString();
                rows[24]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjzs * 100 / hjsgzcz, 1).ToString();
                rows[24]["详细"] = mbzs.ToString();

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "大众";
                rows[25]["目标"] = hjdz.ToString();
                rows[25]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjdz * 100 / hjsgzcz, 1).ToString();
                rows[25]["详细"] = mbdz.ToString();

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "其他";
                rows[26]["目标"] = hjqt.ToString();
                rows[26]["实际"] = hjsgzcz == 0 ? string.Empty : Math.Round(hjqt * 100 / hjsgzcz, 1).ToString();
                rows[26]["详细"] = mbqt.ToString();

                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "合计";
                rows[27]["目标"] = (hjzblp + hjtb + hjpa + hjrs + hjdd + hjzhlh + hjzs + hjdz + hjqt).ToString();
                rows[27]["实际"] = string.Empty;
                rows[27]["详细"] = (mbzblp + mbtb + mbpa + mbrs + mbdd + mbzhlh + mbzs + mbdz + mbqt).ToString();

                rows[28] = tbl.NewRow();
                rows[28]["关键指标"] = "红旭汇绑定数";
                rows[28]["目标"] = mbqzhxhbdtc;
                rows[28]["实际"] = hjqzhxhbdtc;

                rows[29] = tbl.NewRow();
                rows[29]["关键指标"] = "本月微信客户数";
                rows[29]["目标"] = mbwxkhs.ToString();
                rows[29]["实际"] = hjwxkhs.ToString();

                rows[30] = tbl.NewRow();
                rows[30]["关键指标"] = "绑定率";
                rows[30]["目标"] = string.Empty;
                rows[30]["实际"] = hjlctc == 0 ? string.Empty : Math.Round(hjqzhxhbdtc * 100 / hjlctc,1).ToString();

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

                #region 展厅上月留存客户数

                decimal ztsylckhs = 0;
                DateTime day = DateTime.Today;
                if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_last = new DailyReportQuery()
                    {
                        DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = DayReportDep.市场部
                    };
                    List<DailyReportInfo> list_last = DailyReports.Instance.GetList(query_last, true);
                    list_last = list_last.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                    List<DailyReportModuleInfo> rlist_sc = DayReportModules.Instance.GetList(true);
                    rlist_sc = rlist_sc.FindAll(l => l.Department == DayReportDep.市场部).OrderBy(l => l.Sort).ToList();
                    List<Dictionary<string, string>> data_last = new List<Dictionary<string, string>>();
                    for (int i = 0; i < list_last.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(list_last[i].SCReport))
                        {
                            data_last.Add(json.Deserialize<Dictionary<string, string>>(list_last[i].SCReport));
                        }
                    }
                    if (rlist_sc.Exists(l => l.Name == "展厅本月留存客户数"))
                    {
                        int idztbylckhs = rlist_sc.Find(l => l.Name == "展厅本月留存客户数").ID;
                        if (data_last.Exists(d => d.ContainsKey(idztbylckhs.ToString()) && !string.IsNullOrEmpty(d[idztbylckhs.ToString()])))
                        {
                            ztsylckhs = DataConvert.SafeDecimal(data_last.FindLast(d => d.ContainsKey(idztbylckhs.ToString()) && !string.IsNullOrEmpty(d[idztbylckhs.ToString()]))[idztbylckhs.ToString()]);
                        }
                    }
                }
                #endregion

                #region 表数据

                DataRow[] rows = new DataRow[16];

                data.DefaultView.RowFilter = "项目='展厅首次到店记录数'";
                decimal hjztscdfjls = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztscdfjls = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='展厅首次到店建档数'";
                decimal hjsfjds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbsfjds = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

                data.DefaultView.RowFilter = "项目='自然到店'";
                decimal hjzrdd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='转介绍渠道到店'";
                decimal hjzjsqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='增换购渠道到店'";
                decimal hjzhgqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='DCC邀约到店'";
                decimal hjyydd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='市场活动到店'";
                decimal hjwtdd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='网络渠道到店'";
                decimal hjwlqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='电台渠道到店'";
                decimal hjdtqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='户外广告渠道到店'";
                decimal hjhwggqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='114渠道到店'";
                decimal hj114qddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='其他渠道到店'";
                decimal hjqtqddd = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                data.DefaultView.RowFilter = "项目='新增微信粉丝总量'";
                decimal hjxzwxfszl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbxzwxfszl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);
                data.DefaultView.RowFilter = "项目='粉丝取消数'";
                decimal hjfsqxs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                rows[0] = tbl.NewRow();
                rows[0]["关键指标"] = "展厅首次到店建档数";
                rows[0]["目标"] = mbsfjds == 0 ? string.Empty : mbsfjds.ToString();
                rows[0]["实际"] = hjsfjds;
                rows[0]["详细"] = string.Empty;

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "首次到访达成率";
                rows[1]["目标"] = monthtarget == null ? string.Empty : monthtarget.SCscdfdcl;
                rows[1]["实际"] = mbztscdfjls == 0 ? string.Empty : Math.Round(hjztscdfjls * 100 / mbztscdfjls, 1).ToString();
                rows[1]["详细"] = mbztscdfjls == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjztscdfjls, 0), Math.Round(mbztscdfjls, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "首次到访建档率";
                rows[2]["目标"] = mbztscdfjls == 0 ? string.Empty : Math.Round(mbsfjds * 100 / mbztscdfjls, 1).ToString();
                rows[2]["实际"] = hjztscdfjls == 0 ? string.Empty : Math.Round(hjsfjds * 100 / hjztscdfjls, 1).ToString();
                rows[2]["详细"] = hjztscdfjls == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsfjds, 0), Math.Round(hjztscdfjls, 0));

                decimal ddCount = hjwlqddd + hjhwggqddd + hjdtqddd + hjzhgqddd + hjzjsqddd + hjzrdd + hjwtdd + hjyydd + hj114qddd + hjqtqddd;

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "自然";
                rows[3]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjzrdd, 0).ToString();
                rows[3]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzrdd * 100 / ddCount, 1).ToString();

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "转介绍";
                rows[4]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjzjsqddd, 0).ToString();
                rows[4]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzjsqddd * 100 / ddCount, 1).ToString();

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "增换购";
                rows[5]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjzhgqddd, 0).ToString();
                rows[5]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzhgqddd * 100 / ddCount, 1).ToString();

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "DCC邀约";
                rows[6]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjyydd, 0).ToString();
                rows[6]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjyydd * 100 / ddCount, 1).ToString();

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "市场活动";
                rows[7]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjwtdd, 0).ToString();
                rows[7]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjwtdd * 100 / ddCount, 1).ToString();

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "网络";
                rows[8]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjwlqddd, 0).ToString();
                rows[8]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjwlqddd * 100 / ddCount, 1).ToString();

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "电台";
                rows[9]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjdtqddd, 0).ToString();
                rows[9]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjdtqddd * 100 / ddCount, 1).ToString();

                rows[10] = tbl.NewRow();
                rows[10]["关键指标"] = "户外广告";
                rows[10]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjhwggqddd, 0).ToString();
                rows[10]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjhwggqddd * 100 / ddCount, 1).ToString();

                rows[11] = tbl.NewRow();
                rows[11]["关键指标"] = "114";
                rows[11]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hj114qddd, 0).ToString();
                rows[11]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hj114qddd * 100 / ddCount, 1).ToString();

                rows[12] = tbl.NewRow();
                rows[12]["关键指标"] = "其他";
                rows[12]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjqtqddd, 0).ToString();
                rows[12]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjqtqddd * 100 / ddCount, 1).ToString();

                rows[13] = tbl.NewRow();
                rows[13]["关键指标"] = "上月粉丝量";
                rows[13]["实际"] = monthtarget == null ? string.Empty : monthtarget.SCsyfsl;

                rows[14] = tbl.NewRow();
                rows[14]["关键指标"] = "本月粉丝总量";
                rows[14]["实际"] = hjxzwxfszl + DataConvert.SafeInt(monthtarget == null ? string.Empty : monthtarget.SCsyfsl) - hjfsqxs;

                rows[15] = tbl.NewRow();
                rows[15]["关键指标"] = "展厅上月留存客户数";
                rows[15]["实际"] = ztsylckhs;

                #region DCC部数据(删除)

                //DateTime day = DateTime.Today;
                //if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                //{
                //    DailyReportQuery query_dcc = new DailyReportQuery()
                //    {
                //        DayUnique = day.ToString("yyyyMM"),
                //        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                //        DayReportDep = DayReportDep.DCC部
                //    };
                //    query_dcc.OrderBy = " [DayUnique] ASC";
                //    List<DailyReportInfo> list_dcc = DailyReports.Instance.GetList(query_dcc, true);
                //    MonthlyTargetInfo monthtarget_dcc = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.市场部, day, true);

                //    int days = 0;
                //    DataTable tbl_dcc = GetReport(DayReportDep.DCC部, list_dcc, monthtarget_dcc, day, ref days);

                //    tbl_dcc.DefaultView.RowFilter = "项目='新增网络400呼入建档数'";
                //    decimal hjhrxzjds = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='呼入建档数-汽车之家'";
                //    decimal hjqczjhr = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='呼入建档数-易车网'";
                //    decimal hjycwhr = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='呼入建档数-太平洋'";
                //    decimal hjtpyhr = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='呼入建档数-本地网络'";
                //    decimal hjbdwlhr = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='呼入建档数-其他网络'";
                //    decimal hjqtwlhr = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);

                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交数-汽车之家'";
                //    decimal hjqczjcjs = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交数-易车网'";
                //    decimal hjycwcjs = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交数-太平洋'";
                //    decimal hjtpycjs = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交数-本地网络'";
                //    decimal hjbdwlcjs = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交数-转介绍'";
                //    decimal hjzjscjs = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交数-展厅转入'";
                //    decimal hjztzrcjs = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交数-其他渠道'";
                //    decimal hjqtqdcjs = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);
                //    tbl_dcc.DefaultView.RowFilter = "项目='DCC成交总台数'";
                //    decimal hjcjzts = DataConvert.SafeDecimal(tbl_dcc.DefaultView[0]["合计"]);

                //    rows[22] = tbl.NewRow();
                //    rows[22]["关键指标"] = "汽车之家";
                //    rows[22]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjqczjhr, 0).ToString();
                //    rows[22]["实际"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjqczjhr * 100 / hjhrxzjds, 1).ToString();

                //    rows[23] = tbl.NewRow();
                //    rows[23]["关键指标"] = "易车网";
                //    rows[23]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjycwhr, 0).ToString();
                //    rows[23]["实际"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjycwhr * 100 / hjhrxzjds, 1).ToString();

                //    rows[24] = tbl.NewRow();
                //    rows[24]["关键指标"] = "太平洋";
                //    rows[24]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjtpyhr, 0).ToString();
                //    rows[24]["实际"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjtpyhr * 100 / hjhrxzjds, 1).ToString();

                //    rows[25] = tbl.NewRow();
                //    rows[25]["关键指标"] = "本地网络";
                //    rows[25]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjbdwlhr, 0).ToString();
                //    rows[25]["实际"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjbdwlhr * 100 / hjhrxzjds, 1).ToString();

                //    rows[26] = tbl.NewRow();
                //    rows[26]["关键指标"] = "其他网络";
                //    rows[26]["目标"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjqtwlhr, 0).ToString();
                //    rows[26]["实际"] = hjhrxzjds == 0 ? string.Empty : Math.Round(hjqtwlhr * 100 / hjhrxzjds, 1).ToString();

                //    rows[27] = tbl.NewRow();
                //    rows[27]["关键指标"] = "汽车之家";
                //    rows[27]["目标"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqczjcjs, 0).ToString();
                //    rows[27]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqczjcjs * 100 / hjcjzts, 1).ToString();

                //    rows[28] = tbl.NewRow();
                //    rows[28]["关键指标"] = "易车网";
                //    rows[28]["目标"] = hjcjzts == 0 ? string.Empty : Math.Round(hjycwcjs, 0).ToString();
                //    rows[28]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjycwcjs * 100 / hjcjzts, 1).ToString();

                //    rows[29] = tbl.NewRow();
                //    rows[29]["关键指标"] = "太平洋";
                //    rows[29]["目标"] = hjcjzts == 0 ? string.Empty : Math.Round(hjtpycjs, 0).ToString();
                //    rows[29]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjtpycjs * 100 / hjcjzts, 1).ToString();

                //    rows[30] = tbl.NewRow();
                //    rows[30]["关键指标"] = "本地网络";
                //    rows[30]["目标"] = hjcjzts == 0 ? string.Empty : Math.Round(hjbdwlcjs, 0).ToString();
                //    rows[30]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjbdwlcjs * 100 / hjcjzts, 1).ToString();

                //    rows[31] = tbl.NewRow();
                //    rows[31]["关键指标"] = "转介绍";
                //    rows[31]["目标"] = hjcjzts == 0 ? string.Empty : Math.Round(hjzjscjs, 0).ToString();
                //    rows[31]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjzjscjs * 100 / hjcjzts, 1).ToString();

                //    rows[32] = tbl.NewRow();
                //    rows[32]["关键指标"] = "展厅转入";
                //    rows[32]["目标"] = hjcjzts == 0 ? string.Empty : Math.Round(hjztzrcjs, 0).ToString();
                //    rows[32]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjztzrcjs * 100 / hjcjzts, 1).ToString();

                //    rows[33] = tbl.NewRow();
                //    rows[33]["关键指标"] = "其他渠道";
                //    rows[33]["目标"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqtqdcjs, 0).ToString();
                //    rows[33]["实际"] = hjcjzts == 0 ? string.Empty : Math.Round(hjqtqdcjs * 100 / hjcjzts, 1).ToString();
                //}

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

                #region 留单

                decimal ld = 0;
                DateTime day = DateTime.Today;
                if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_last = new DailyReportQuery()
                    {
                        DayUnique = day.AddMonths(-1).ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
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

                #region 表数据

                DataRow[] rows = new DataRow[32];

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
                data.DefaultView.RowFilter = "项目='易车网留档数'";
                decimal hjycwlds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='太平洋留档数'";
                decimal hjtpylds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='本地网络留档数'";
                decimal hjbdwllds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='其他网络留档数'";
                decimal hjqtwllds = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

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
                data.DefaultView.RowFilter = "项目='触点成交数'";
                decimal hjcdcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='CRm下达成交数'";
                decimal hjcrmxdcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                data.DefaultView.RowFilter = "项目='展厅转入成交数'";
                decimal hjztzrcjs = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);

                data.DefaultView.RowFilter = "项目='展厅销量'";
                decimal hjztxl = DataConvert.SafeDecimal(data.DefaultView[0]["合计"]);
                decimal mbztxl = DataConvert.SafeDecimal(data.DefaultView[0]["目标值"]);

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
                rows[2]["详细"] = hjztxl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjcjzts, 0), Math.Round(hjztxl, 0));

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "前台首电建档率";
                rows[3]["目标"] = mbxzztqtlds == 0 ? string.Empty : Math.Round(mbxzztqtldjds * 100 / mbxzztqtlds, 1).ToString();
                rows[3]["实际"] = hjxzztqtlds == 0 ? string.Empty : Math.Round(hjxzztqtldjds * 100 / hjxzztqtlds, 1).ToString();
                rows[3]["详细"] = hjxzztqtlds == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxzztqtldjds, 0), Math.Round(hjxzztqtlds, 0));

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "成交率";
                rows[4]["目标"] = mbscyyddkhzs == 0 ? string.Empty : Math.Round(mbcjzts * 100 / mbscyyddkhzs, 1).ToString();
                rows[4]["实际"] = hjscyyddkhzs == 0 ? string.Empty : Math.Round(hjcjzts * 100 / hjscyyddkhzs, 1).ToString();
                rows[4]["详细"] = hjscyyddkhzs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjcjzts, 0), Math.Round(hjscyyddkhzs, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "有效呼出率";
                rows[5]["目标"] = mbhczl == 0 ? string.Empty : Math.Round(mbhcyxs * 100 / mbhczl, 1).ToString();
                rows[5]["实际"] = hjhczl == 0 ? string.Empty : Math.Round(hjhcyxs * 100 / hjhczl, 1).ToString();
                rows[5]["详细"] = hjhczl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjhcyxs, 0), Math.Round(hjhczl, 0));

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "首次邀约到店率";
                rows[6]["目标"] = mbxzdccxsjdl == 0 ? string.Empty : Math.Round(mbscyyddkhzs * 100 / mbxzdccxsjdl, 1).ToString();
                rows[6]["实际"] = hjxzdccxsjdl == 0 ? string.Empty : Math.Round(hjscyyddkhzs * 100 / hjxzdccxsjdl, 1).ToString();
                rows[6]["详细"] = hjxzdccxsjdl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjscyyddkhzs, 0), Math.Round(hjxzdccxsjdl, 0));

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "再次邀约到店占比";
                rows[7]["目标"] = (mbscyyddkhzs + mbzcyydds) == 0 ? string.Empty : Math.Round(mbzcyydds * 100 / (mbscyyddkhzs + mbzcyydds), 1).ToString();
                rows[7]["实际"] = (hjscyyddkhzs + hjzcyydds) == 0 ? string.Empty : Math.Round(hjzcyydds * 100 / (hjscyyddkhzs + hjzcyydds), 1).ToString();
                rows[7]["详细"] = (hjscyyddkhzs + hjzcyydds) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjzcyydds, 0), Math.Round(hjscyyddkhzs + hjzcyydds, 0));

                rows[8] = tbl.NewRow();
                rows[8]["关键指标"] = "网络线索建档率";
                rows[8]["目标"] = mbxzwlxszl == 0 ? string.Empty : Math.Round(mbxzwlxszldl * 100 / mbxzwlxszl, 1).ToString();
                rows[8]["实际"] = hjxzwlxszl == 0 ? string.Empty : Math.Round(hjxzwlxszldl * 100 / hjxzwlxszl, 1).ToString();
                rows[8]["详细"] = hjxzwlxszl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxzwlxszldl, 0), Math.Round(hjxzwlxszl, 0));

                rows[9] = tbl.NewRow();
                rows[9]["关键指标"] = "线索转化率";
                rows[9]["目标"] = mbxzdccxszl == 0 ? string.Empty : Math.Round(mbdccdds * 100 / mbxzdccxszl, 1).ToString();
                rows[9]["实际"] = hjxzdccxszl == 0 ? string.Empty : Math.Round(hjdccdds * 100 / hjxzdccxszl, 1).ToString();
                rows[9]["详细"] = hjxzwlxszl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjdccdds, 0), Math.Round(hjxzdccxszl, 0));

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

                #region 市场部数据 (删除)

                //DateTime day = DateTime.Today;
                //if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                //{
                //    DailyReportQuery query_sc = new DailyReportQuery()
                //    {
                //        DayUnique = day.ToString("yyyyMM"),
                //        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                //        DayReportDep = DayReportDep.市场部
                //    };
                //    query_sc.OrderBy = " [DayUnique] ASC";
                //    List<DailyReportInfo> list_sc = DailyReports.Instance.GetList(query_sc, true);
                //    MonthlyTargetInfo monthtarget_sc = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.市场部, day, true);

                //    int days = 0;
                //    DataTable tbl_sc = GetReport(DayReportDep.市场部, list_sc, monthtarget_sc, day, ref days);

                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-网络渠道'";
                //    decimal hjwlqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-户外广告渠道'";
                //    decimal hjhwggqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-电台渠道'";
                //    decimal hjdtqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-增换购渠道'";
                //    decimal hjzhgqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-转介绍渠道'";
                //    decimal hjzjsqddd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-自然到店'";
                //    decimal hjzrdd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-市场外拓活动'";
                //    decimal hjwtdd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='展厅到店-DCC邀约到店'";
                //    decimal hjyydd = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-汽车之家'";
                //    decimal hjqczjxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-易车网'";
                //    decimal hjycwxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-太平洋'";
                //    decimal hjtpyxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-本地网络'";
                //    decimal hjbdwlxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);
                //    tbl_sc.DefaultView.RowFilter = "项目='新增留档数-其他网络'";
                //    decimal hjqtwlxzlds = DataConvert.SafeDecimal(tbl_sc.DefaultView[0]["合计"]);

                //    decimal ddCount = hjwlqddd + hjhwggqddd + hjdtqddd + hjzhgqddd + hjzjsqddd + hjzrdd + hjwtdd + hjyydd;

                //    rows[20] = tbl.NewRow();
                //    rows[20]["关键指标"] = "网络";
                //    rows[20]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjwlqddd, 0).ToString();
                //    rows[20]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjwlqddd * 100 / ddCount, 1).ToString();

                //    rows[21] = tbl.NewRow();
                //    rows[21]["关键指标"] = "户外广告";
                //    rows[21]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjhwggqddd, 0).ToString();
                //    rows[21]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjhwggqddd * 100 / ddCount, 1).ToString();

                //    rows[22] = tbl.NewRow();
                //    rows[22]["关键指标"] = "电台";
                //    rows[22]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjdtqddd, 0).ToString();
                //    rows[22]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjdtqddd * 100 / ddCount, 1).ToString();

                //    rows[23] = tbl.NewRow();
                //    rows[23]["关键指标"] = "增换购";
                //    rows[23]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjzhgqddd, 0).ToString();
                //    rows[23]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzhgqddd * 100 / ddCount, 1).ToString();

                //    rows[24] = tbl.NewRow();
                //    rows[24]["关键指标"] = "转介绍";
                //    rows[24]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjzjsqddd, 0).ToString();
                //    rows[24]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzjsqddd * 100 / ddCount, 1).ToString();

                //    rows[25] = tbl.NewRow();
                //    rows[25]["关键指标"] = "自然";
                //    rows[25]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjzrdd, 0).ToString();
                //    rows[25]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjzrdd * 100 / ddCount, 1).ToString();

                //    rows[26] = tbl.NewRow();
                //    rows[26]["关键指标"] = "外拓";
                //    rows[26]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjwtdd, 0).ToString();
                //    rows[26]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjwtdd * 100 / ddCount, 1).ToString();

                //    rows[27] = tbl.NewRow();
                //    rows[27]["关键指标"] = "邀约";
                //    rows[27]["目标"] = ddCount == 0 ? string.Empty : Math.Round(hjyydd, 0).ToString();
                //    rows[27]["实际"] = ddCount == 0 ? string.Empty : Math.Round(hjyydd * 100 / ddCount, 1).ToString();

                //    decimal wlxsCount = hjqczjxzlds + hjycwxzlds + hjtpyxzlds + hjbdwlxzlds + hjqtwlxzlds;

                //    rows[28] = tbl.NewRow();
                //    rows[28]["关键指标"] = "汽车之家";
                //    rows[28]["目标"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqczjxzlds,0).ToString();
                //    rows[28]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqczjxzlds * 100 / wlxsCount, 1).ToString();

                //    rows[29] = tbl.NewRow();
                //    rows[29]["关键指标"] = "易车网";
                //    rows[29]["目标"] = wlxsCount == 0 ? string.Empty : Math.Round(hjycwxzlds, 0).ToString();
                //    rows[29]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjycwxzlds * 100 / wlxsCount, 1).ToString();

                //    rows[30] = tbl.NewRow();
                //    rows[30]["关键指标"] = "太平洋";
                //    rows[30]["目标"] = wlxsCount == 0 ? string.Empty : Math.Round(hjtpyxzlds, 0).ToString();
                //    rows[30]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjtpyxzlds * 100 / wlxsCount, 1).ToString();

                //    rows[31] = tbl.NewRow();
                //    rows[31]["关键指标"] = "本地网络";
                //    rows[31]["目标"] = wlxsCount == 0 ? string.Empty : Math.Round(hjbdwlxzlds, 0).ToString();
                //    rows[31]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjbdwlxzlds * 100 / wlxsCount, 1).ToString();

                //    rows[32] = tbl.NewRow();
                //    rows[32]["关键指标"] = "其他网络";
                //    rows[32]["目标"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqtwlxzlds, 0).ToString();
                //    rows[32]["实际"] = wlxsCount == 0 ? string.Empty : Math.Round(hjqtwlxzlds * 100 / wlxsCount, 1).ToString();
                //}

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

                DataRow[] rows = new DataRow[13];
                #region 销售数据

                DateTime day = DataConvert.SafeDate(txtDate.Text + "-01");
                DailyReportQuery query_xs = new DailyReportQuery()
                {
                    DayUnique = day.ToString("yyyyMM"),
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);
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
                rows[0]["详细"] = hjxcztxsl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxssjpgtc, 0), Math.Round(hjxcztxsl, 0));

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "售后有效推荐率";
                rows[1]["目标"] = mbshjctc == 0 ? string.Empty : Math.Round(mbshsjpgtc * 100 / mbshjctc, 1).ToString();
                rows[1]["实际"] = hjshjctc == 0 ? string.Empty : Math.Round(hjshsjpgtc * 100 / hjshjctc, 1).ToString();
                rows[1]["详细"] = hjshjctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjshsjpgtc, 0), Math.Round(hjshjctc, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "总评估成交率";
                rows[2]["目标"] = (mbxssjpgtc + mbshsjpgtc + mbqtqdsjpgtc) == 0 ? string.Empty : Math.Round((mbsgtc + mbzhtc) * 100 / (mbxssjpgtc + mbshsjpgtc + mbqtqdsjpgtc), 1).ToString();
                rows[2]["实际"] = (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc) == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc), 1).ToString();
                rows[2]["详细"] = (hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsgtc + hjzhtc, 0), Math.Round(hjxssjpgtc + hjshsjpgtc + hjqtqdsjpgtc, 0));

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "总销售成交率";
                rows[3]["目标"] = (mbscldianpc + mbscldpc) == 0 ? string.Empty : Math.Round(mbxstc * 100 / (mbscldianpc + mbscldpc), 1).ToString();
                rows[3]["实际"] = (hjscldianpc + hjscldpc) == 0 ? string.Empty : Math.Round(hjxstc * 100 / (hjscldianpc + hjscldpc), 1).ToString();
                rows[3]["详细"] = (hjscldianpc + hjscldpc) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxstc, 0), Math.Round(hjscldianpc + hjscldpc, 0));

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "展厅置换率";
                rows[4]["目标"] = mbxcztxsl == 0 ? string.Empty : Math.Round((mbsgtc + mbzhtc) * 100 / mbxcztxsl, 1).ToString();
                rows[4]["实际"] = hjxcztxsl == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / hjxcztxsl, 1).ToString();
                rows[4]["详细"] = hjxcztxsl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsgtc + hjzhtc, 0), Math.Round(hjxcztxsl, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "总置换率";
                rows[5]["目标"] = mbzxstc == 0 ? string.Empty : Math.Round((mbsgtc + mbzhtc) * 100 / mbzxstc, 1).ToString();
                rows[5]["实际"] = hjzxstc == 0 ? string.Empty : Math.Round((hjsgtc + hjzhtc) * 100 / hjzxstc, 1).ToString();
                rows[5]["详细"] = hjzxstc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsgtc + hjzhtc, 0), Math.Round(hjzxstc, 0));

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
                if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    List<DailyReportModuleInfo> rlist = DayReportModules.Instance.GetList(true);
                    rlist = rlist.FindAll(l => l.Department == DayReportDep.售后部).OrderBy(l => l.Sort).ToList();
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
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
                decimal hjztjcts=0, mbztjcts=0;
                decimal hjzthdccbxtc=0, mbzthdccbxtc=0;
                decimal hjlctc=0, mblctc=0,hjxbs = 0,mbxbs = 0;
                DateTime day = DateTime.Today;

                #region 销售部数据

                if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_xs = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = DayReportDep.销售部
                    };
                    query_xs.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                    MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);

                    int days = 0;
                    DataTable tbl_xs = GetReport(DayReportDep.销售部, list_xs, monthtarget_xs, day, ref days);

                    tbl_xs.DefaultView.RowFilter = "项目='展厅交车台数'";
                    hjztjcts = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["合计"]);
                    mbztjcts = DataConvert.SafeDecimal(tbl_xs.DefaultView[0]["目标值"]);
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
                    rows[1]["关键指标"] = "购买个数";
                    rows[1]["目标"] = mbxcjytcgmgs;
                    rows[1]["实际"] = hjxcjytcgmgs;
                    rows[1]["详细"] = string.Empty;

                    rows[2] = tbl.NewRow();
                    rows[2]["关键指标"] = "购买金额";
                    rows[2]["目标"] = mbxcjytcgmje;
                    rows[2]["实际"] = hjxcjytcgmje;

                    rows[3] = tbl.NewRow();
                    rows[3]["关键指标"] = "渗透率";
                    rows[3]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbxcjytcgmgs * 100 / mbztjcts, 0).ToString();
                    rows[3]["实际"] = hjztjcts == 0 ? string.Empty : Math.Round(hjxcjytcgmgs * 100 / hjztjcts, 0).ToString();
                    rows[3]["详细"] = hjztjcts == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxcjytcgmgs, 0), Math.Round(hjztjcts, 0));

                    //新车玻璃无忧
                    rows[4] = tbl.NewRow();
                    rows[4]["关键指标"] = "购买个数";
                    rows[4]["目标"] = mbxcblwyfwgmgs;
                    rows[4]["实际"] = hjxcblwyfwgmgs;

                    rows[5] = tbl.NewRow();
                    rows[5]["关键指标"] = "购买金额";
                    rows[5]["目标"] = mbxcblwyfwgmje;
                    rows[5]["实际"] = hjxcblwyfwgmje;

                    rows[6] = tbl.NewRow();
                    rows[6]["关键指标"] = "渗透率";
                    rows[6]["目标"] = mbzthdccbxtc == 0 ? string.Empty : Math.Round(mbxcblwyfwgmgs * 100 / mbzthdccbxtc, 0).ToString();
                    rows[6]["实际"] = hjzthdccbxtc == 0 ? string.Empty : Math.Round(hjxcblwyfwgmgs * 100 / hjzthdccbxtc, 0).ToString();
                    rows[6]["详细"] = hjzthdccbxtc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxcblwyfwgmgs, 0), Math.Round(hjzthdccbxtc, 0));

                    //新车划痕无忧
                    rows[7] = tbl.NewRow();
                    rows[7]["关键指标"] = "购买个数";
                    rows[7]["目标"] = mbxchhwyfwgmgs;
                    rows[7]["实际"] = hjxchhwyfwgmgs;

                    rows[8] = tbl.NewRow();
                    rows[8]["关键指标"] = "购买金额";
                    rows[8]["目标"] = mbxchhwyfwgmje;
                    rows[8]["实际"] = hjxchhwyfwgmje;

                    rows[9] = tbl.NewRow();
                    rows[9]["关键指标"] = "渗透率";
                    rows[9]["目标"] = mbzthdccbxtc == 0 ? string.Empty : Math.Round(mbxchhwyfwgmgs * 100 / mbzthdccbxtc, 0).ToString();
                    rows[9]["实际"] = hjzthdccbxtc == 0 ? string.Empty : Math.Round(hjxchhwyfwgmgs * 100 / hjzthdccbxtc, 0).ToString();
                    rows[9]["详细"] = hjzthdccbxtc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxchhwyfwgmgs, 0), Math.Round(hjzthdccbxtc, 0));

                    //新车延保无忧
                    rows[10] = tbl.NewRow();
                    rows[10]["关键指标"] = "购买个数";
                    rows[10]["目标"] = mbxcybfwgmgs;
                    rows[10]["实际"] = hjxcybfwgmgs;

                    rows[11] = tbl.NewRow();
                    rows[11]["关键指标"] = "购买金额";
                    rows[11]["目标"] = mbxcybfwgmje;
                    rows[11]["实际"] = hjxcybfwgmje;

                    rows[12] = tbl.NewRow();
                    rows[12]["关键指标"] = "渗透率";
                    rows[12]["目标"] = mbztjcts == 0 ? string.Empty : Math.Round(mbxcybfwgmgs * 100 / mbztjcts, 0).ToString();
                    rows[12]["实际"] = hjzthdccbxtc == 0 ? string.Empty : Math.Round(hjxcybfwgmgs * 100 / hjzthdccbxtc, 0).ToString();
                    rows[12]["详细"] = hjzthdccbxtc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxcybfwgmgs, 0), Math.Round(hjzthdccbxtc, 0));

                    rows[13] = tbl.NewRow();
                    rows[13]["关键指标"] = "自主购买个数";
                    rows[13]["目标"] = mbxcybfwzzgmgs;
                    rows[13]["实际"] = hjxcybfwzzgmgs;

                    rows[14] = tbl.NewRow();
                    rows[14]["关键指标"] = "自主购买金额";
                    rows[14]["目标"] = mbxcybfwzzgmje;
                    rows[14]["实际"] = hjxcybfwzzgmje;

                    rows[15] = tbl.NewRow();
                    rows[15]["关键指标"] = "厂家购买个数";
                    rows[15]["目标"] = mbxcybfwcjgmgs;
                    rows[15]["实际"] = hjxcybfwcjgmgs;

                    rows[16] = tbl.NewRow();
                    rows[16]["关键指标"] = "厂家购买金额";
                    rows[16]["目标"] = mbxcybfwcjgmje;
                    rows[16]["实际"] = hjxcybfwcjgmje;

                }

                #endregion

                #region 售后数据

                if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
                {
                    DailyReportQuery query_sh = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = DayReportDep.售后部
                    };
                    query_sh.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                    MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.售后部, day, true);

                    int days = 0;
                    DataTable tbl_sh = GetReport(DayReportDep.售后部, list_sh, monthtarget_sh, day, ref days);

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
                rows[18]["关键指标"] = "购买个数";
                rows[18]["目标"] = mbshjytcgmgs;
                rows[18]["实际"] = hjshjytcgmgs;

                rows[19] = tbl.NewRow();
                rows[19]["关键指标"] = "购买金额";
                rows[19]["目标"] = mbshjytcgmje;
                rows[19]["实际"] = hjshjytcgmje;

                rows[20] = tbl.NewRow();
                rows[20]["关键指标"] = "渗透率";
                rows[20]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshjytcstl)) ? monthtarget.NXCPshjytcstl : ((mbdylcjpcls3y - mbshjytcyymgkhs) == 0 ? string.Empty : Math.Round(mbshjytcgmgs * 100 / (mbdylcjpcls3y - mbshjytcyymgkhs), 0).ToString());
                rows[20]["实际"] = (hjdylcjpcls3y - hjshjytcyymgkhs) == 0 ? string.Empty : Math.Round(hjshjytcgmgs * 100 / (hjdylcjpcls3y - hjshjytcyymgkhs), 0).ToString();
                rows[20]["详细"] = (hjdylcjpcls3y - hjshjytcyymgkhs) == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjshjytcgmgs, 0), Math.Round(hjdylcjpcls3y - hjshjytcyymgkhs, 0));

                //续保玻璃无忧
                rows[21] = tbl.NewRow();
                rows[21]["关键指标"] = "购买个数";
                rows[21]["目标"] = mbxbblwyfwgmgs;
                rows[21]["实际"] = hjxbblwyfwgmgs;

                rows[22] = tbl.NewRow();
                rows[22]["关键指标"] = "购买金额";
                rows[22]["目标"] = mbxbblwyfwgmje;
                rows[22]["实际"] = hjxbblwyfwgmje;

                rows[23] = tbl.NewRow();
                rows[23]["关键指标"] = "渗透率";
                rows[23]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshblwyfwstl)) ? monthtarget.NXCPshblwyfwstl : (mbxbs == 0 ? string.Empty : Math.Round(mbxbblwyfwgmgs * 100 / mbxbs, 0).ToString());
                rows[23]["实际"] = hjxbs == 0 ? string.Empty : Math.Round(hjxbblwyfwgmgs * 100 / hjxbs, 0).ToString();
                rows[23]["详细"] = hjxbs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxbblwyfwgmgs, 0), Math.Round(hjxbs, 0));
                
                //续保划痕无忧
                rows[24] = tbl.NewRow();
                rows[24]["关键指标"] = "购买个数";
                rows[24]["目标"] = mbxbhhwyfwgmgs;
                rows[24]["实际"] = hjxbhhwyfwgmgs;

                rows[25] = tbl.NewRow();
                rows[25]["关键指标"] = "购买金额";
                rows[25]["目标"] = mbxbhhwyfwgmje;
                rows[25]["实际"] = hjxbhhwyfwgmje;

                rows[26] = tbl.NewRow();
                rows[26]["关键指标"] = "渗透率";
                rows[26]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshhhwyfwstl)) ? monthtarget.NXCPshhhwyfwstl : (mbxbs == 0 ? string.Empty : Math.Round(mbxbhhwyfwgmgs * 100 / mbxbs, 0).ToString());
                rows[26]["实际"] = hjxbs == 0 ? string.Empty : Math.Round(hjxbhhwyfwgmgs * 100 / hjxbs, 0).ToString();
                rows[26]["详细"] = hjxbs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxbhhwyfwgmgs, 0), Math.Round(hjxbs, 0));

                //延保无忧
                rows[27] = tbl.NewRow();
                rows[27]["关键指标"] = "购买个数";
                rows[27]["目标"] = mbxbybwyfwgmgs;
                rows[27]["实际"] = hjxbybwyfwgmgs;

                rows[28] = tbl.NewRow();
                rows[28]["关键指标"] = "购买金额";
                rows[28]["目标"] = mbxbybwyfwgmje;
                rows[28]["实际"] = hjxbybwyfwgmje;

                rows[29] = tbl.NewRow();
                rows[29]["关键指标"] = "渗透率";
                rows[29]["目标"] = (monthtarget != null && !string.IsNullOrEmpty(monthtarget.NXCPshybwycfwstl)) ? monthtarget.NXCPshybwycfwstl : (mbdylcjpcls18m == 0 ? string.Empty : Math.Round(mbxbybwyfwgmgs * 100 / mbdylcjpcls18m, 0).ToString());
                rows[29]["实际"] = hjdylcjpcls18m == 0 ? string.Empty : Math.Round(hjxbybwyfwgmgs * 100 / hjdylcjpcls18m, 0).ToString();
                rows[29]["详细"] = hjdylcjpcls18m == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxbybwyfwgmgs, 0), Math.Round(hjdylcjpcls18m, 0));

                rows[30] = tbl.NewRow();
                rows[30]["关键指标"] = "自主购买个数";
                rows[30]["目标"] = mbxbybwyfwzzgmgs;
                rows[30]["实际"] = hjxbybwyfwzzgmgs;

                rows[31] = tbl.NewRow();
                rows[31]["关键指标"] = "自主购买金额";
                rows[31]["目标"] = mbxbybwyfwzzgmje;
                rows[31]["实际"] = hjxbybwyfwzzgmje;

                rows[32] = tbl.NewRow();
                rows[32]["关键指标"] = "厂家购买个数";
                rows[32]["目标"] = mbxbybwyfwcjgmgs;
                rows[32]["实际"] = hjxbybwyfwcjgmgs;

                rows[33] = tbl.NewRow();
                rows[33]["关键指标"] = "厂家购买金额";
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
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.销售部
                };
                List<DailyReportInfo> list_xs = DailyReports.Instance.GetList(query_xs, true);
                list_xs = list_xs.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_xs = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.销售部, day, true);
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
                    CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                    DayReportDep = DayReportDep.售后部
                };
                List<DailyReportInfo> list_sh = DailyReports.Instance.GetList(query_sh, true);
                list_sh = list_sh.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                MonthlyTargetInfo monthtarget_sh = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), DayReportDep.售后部, day, true);
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
                rows[0]["详细"] = hjztxl == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxshfcgs, 0), Math.Round(hjztxl, 0));

                rows[1] = tbl.NewRow();
                rows[1]["关键指标"] = "售后回访成功率";
                rows[1]["目标"] = mbshhfs == 0 ? string.Empty : Math.Round(mbshhfcgs * 100 / mbshhfs, 1).ToString();
                rows[1]["实际"] = hjshhfs == 0 ? string.Empty : Math.Round(hjshhfcgs * 100 / hjshhfs, 1).ToString();
                rows[1]["详细"] = hjshhfs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjshhfcgs, 0), Math.Round(hjshhfs, 0));

                rows[2] = tbl.NewRow();
                rows[2]["关键指标"] = "销售满意率";
                rows[2]["目标"] = mbxshfcgs == 0 ? string.Empty : Math.Round((mbxshfcgs - mbxsbmyzs) * 100 / mbxshfcgs, 1).ToString();
                rows[2]["实际"] = hjxshfcgs == 0 ? string.Empty : Math.Round((hjxshfcgs - hjxsbmyzs) * 100 / hjxshfcgs, 1).ToString();
                rows[2]["详细"] = hjxshfcgs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjxshfcgs - hjxsbmyzs, 0), Math.Round(hjxshfcgs, 0));

                rows[3] = tbl.NewRow();
                rows[3]["关键指标"] = "售后满意率";
                rows[3]["目标"] = mbshhfcgs == 0 ? string.Empty : Math.Round((mbshhfcgs - mbshbmyzs) * 100 / mbshhfcgs, 1).ToString();
                rows[3]["实际"] = hjshhfcgs == 0 ? string.Empty : Math.Round((hjshhfcgs - hjshbmyzs) * 100 / hjshhfcgs, 1).ToString();
                rows[3]["详细"] = hjshhfcgs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjshhfcgs - hjshbmyzs, 0), Math.Round(hjshhfcgs, 0));

                rows[4] = tbl.NewRow();
                rows[4]["关键指标"] = "销售新车首保招揽回厂率";
                rows[4]["目标"] = mbsbdqtxs == 0 ? string.Empty : Math.Round(mbsblds * 100 / mbsbdqtxs, 1).ToString();
                rows[4]["实际"] = hjsbdqtxs == 0 ? string.Empty : Math.Round(hjsblds * 100 / hjsbdqtxs, 1).ToString();
                rows[4]["详细"] = hjsbdqtxs == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjsblds, 0), Math.Round(hjsbdqtxs, 0));

                rows[5] = tbl.NewRow();
                rows[5]["关键指标"] = "售后定期保养招揽回厂率";
                rows[5]["目标"] = mbdbzls == 0 ? string.Empty : Math.Round(mbdbkhlcs * 100 / mbdbzls, 1).ToString();
                rows[5]["实际"] = hjdbzls == 0 ? string.Empty : Math.Round(hjdbkhlcs * 100 / hjdbzls, 1).ToString();
                rows[5]["详细"] = hjdbzls == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjdbkhlcs, 0), Math.Round(hjdbzls, 0));

                rows[6] = tbl.NewRow();
                rows[6]["关键指标"] = "流失客户招揽回厂率";
                rows[6]["目标"] = mblszls == 0 ? string.Empty : Math.Round(mblskhlds * 100 / mblszls, 1).ToString();
                rows[6]["实际"] = hjlszls == 0 ? string.Empty : Math.Round(hjlskhlds * 100 / hjlszls, 1).ToString();
                rows[6]["详细"] = hjlszls == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjlskhlds, 0), Math.Round(hjlszls, 0));

                rows[7] = tbl.NewRow();
                rows[7]["关键指标"] = "维修预约率";
                rows[7]["目标"] = mbshlctc == 0 ? string.Empty : Math.Round(mbyylctc * 100 / mbshlctc, 1).ToString();
                rows[7]["实际"] = hjshlctc == 0 ? string.Empty : Math.Round(hjyylctc * 100 / hjshlctc, 1).ToString();
                rows[7]["详细"] = hjshlctc == 0 ? string.Empty : string.Format("<br />({0}/{1})", Math.Round(hjyylctc, 0), Math.Round(hjshlctc, 0));

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

        /// <summary>
        /// 关键指标页面输出
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="tbl"></param>
        /// <returns></returns>
        private string GetKeyReportStr(DayReportDep dep, DataTable tbl)
        {
            StringBuilder strb = new StringBuilder();

            strb.AppendLine(string.Format("<div class=\"mb10 mt10\">最后填报日期：{0}</div>", string.IsNullOrEmpty(lastDayUnique) ? "未填报" : (lastDayUnique.Substring(0, 4) + "-" + lastDayUnique.Substring(4, 2) + "-" + lastDayUnique.Substring(6, 2))));
            if (dep == DayReportDep.销售部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"920\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 0; i < 7; i++)
                {
                    strb.AppendFormat("<td class=\"w120\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[0]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[0]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[1]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[1]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[2]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[2]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[3]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[3]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[4]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[4]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[5]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[5]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[6]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[6]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["实际"].ToString(), tbl.Rows[0]["详细"].ToString(), false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["实际"].ToString(), tbl.Rows[1]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[1]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[1]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["实际"].ToString(), tbl.Rows[2]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[2]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[2]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["实际"].ToString(), tbl.Rows[3]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[3]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[3]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["实际"].ToString(), tbl.Rows[4]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[4]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[4]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[5]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[5]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["实际"].ToString(), tbl.Rows[6]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[6]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[6]["完成率"]) < 100, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td>关键指标</td>");
                for (int i = 7; i < 14; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[7]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[7]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[8]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[8]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[9]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[9]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[10]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[10]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[11]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[11]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[12]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[12]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[13]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[13]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[7]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[7]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["实际"].ToString(), tbl.Rows[8]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[8]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[8]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[9]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[9]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["实际"].ToString(), tbl.Rows[10]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[10]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[10]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[11]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[11]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[12]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[12]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[13]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[13]["完成率"]) < 100, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td>关键指标</td>");
                for (int i = 14; i < 21; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[14]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[14]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[15]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[15]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[16]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[16]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[17]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[17]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[18]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[18]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[18]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[19]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[19]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[19]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(GetMoneyStr(DataConvert.SafeFloat(tbl.Rows[20]["目标"])), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[20]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(GetMoneyStr(DataConvert.SafeFloat(tbl.Rows[20]["预算"].ToString())), string.Empty, false, false) + ")")));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["实际"].ToString(), tbl.Rows[14]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[14]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[14]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[15]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[15]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["实际"].ToString(), tbl.Rows[16]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[16]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[16]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[17]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[17]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[18]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[18]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[18]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[19]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[19]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[19]["完成率"]) > 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(GetMoneyStr(DataConvert.SafeFloat(tbl.Rows[20]["实际"])), string.Empty, !string.IsNullOrEmpty(tbl.Rows[20]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[20]["完成率"]) > 100, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[18]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td>关键指标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[21]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[22]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[23]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[24]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[25]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[26]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[27]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[21]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[21]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[21]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[22]["目标"].ToString(), string.Empty, false, false) + (string.IsNullOrEmpty(tbl.Rows[22]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[22]["预算"].ToString(), string.Empty, false, false) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[23]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[23]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[23]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[24]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[24]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[24]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[25]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[25]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[25]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[26]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[26]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[26]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[27]["目标"].ToString(), string.Empty, false, true) + (string.IsNullOrEmpty(tbl.Rows[27]["预算"].ToString()) ? string.Empty : ("(" + GetCellValue(tbl.Rows[27]["预算"].ToString(), string.Empty, false, true) + ")")));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[21]["实际"].ToString(), tbl.Rows[21]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[21]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[21]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[22]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[22]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[22]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[23]["实际"].ToString(), tbl.Rows[23]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[23]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[23]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[24]["实际"].ToString(), tbl.Rows[24]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[24]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[24]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[25]["实际"].ToString(), tbl.Rows[25]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[25]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[25]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[26]["实际"].ToString(), tbl.Rows[26]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[26]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[26]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[27]["实际"].ToString(), tbl.Rows[27]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[27]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[27]["完成率"]) < 100, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[21]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[22]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[23]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[24]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[25]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[26]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[27]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                {
                    strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                    strb.AppendLine("<tr>");
                    strb.AppendLine("<td class=\"tc bold\" style=\"background:#0d9d65;\" colspan=\"5\">他品牌销售</td>");
                    strb.AppendLine("</tr>");
                    strb.AppendLine("<tr class=\"tc bold\" style=\"background:#0d9d65;\">");
                    strb.AppendLine("<td class=\"w80\">关键指标</td>");
                    for (int i = 34; i < 38; i++)
                    {
                        strb.AppendFormat("<td class=\"w80\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                    }
                    strb.AppendLine("</tr>");
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendLine("<td class=\"bold\" style=\"background:#0d9d65;\">目标</td>");
                    for (int i = 34; i < 38; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, false));
                    }
                    strb.AppendLine("</tr>");
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendLine("<td class=\"bold\" style=\"background:#0d9d65;\">实际</td>");
                    for (int i = 34; i < 38; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[i]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[i]["完成率"]) < 100, false));
                    }
                    strb.AppendLine("</tr>");
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendLine("<td class=\"bold\" style=\"background:#0d9d65;\">完成率</td>");
                    for (int i = 34; i < 38; i++)
                    {
                        strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                    }
                    strb.AppendLine("</tr>");
                    strb.AppendLine("</table>");
                }

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendFormat("<td class=\"w120\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[28]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[28]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[29]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[29]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w80\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[40]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[40]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[30]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[30]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[31]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[31]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[32]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[32]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[33]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[33]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[39]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[39]["实际"].ToString(), string.Empty, false, false));
                if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                {
                    strb.AppendFormat("<td class=\"w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[34]["关键指标"].ToString(), string.Empty, false, false));
                    strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[34]["实际"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.售后部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"680\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 0; i < 6; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["目标"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[0]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[0]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["实际"].ToString(), tbl.Rows[1]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[1]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[1]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[2]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[2]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["实际"].ToString(), tbl.Rows[3]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[3]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[3]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["实际"].ToString(), tbl.Rows[4]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[4]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[4]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[5]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[5]["完成率"]) < 100, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 6; i < 12; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["目标"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["实际"].ToString(), tbl.Rows[6]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[6]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[6]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["实际"].ToString(), tbl.Rows[7]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[7]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[7]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["实际"].ToString(), tbl.Rows[8]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[8]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[8]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["实际"].ToString(), tbl.Rows[9]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[9]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[9]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["实际"].ToString(), tbl.Rows[10]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[10]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[10]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["实际"].ToString(), tbl.Rows[11]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[11]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[11]["完成率"]) < 100, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 12; i < 18; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["目标"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[12]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[12]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["实际"].ToString(), tbl.Rows[13]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[13]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[13]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[14]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[14]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[15]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[15]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[16]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[16]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[17]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[17]["完成率"]) < 100, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table width=\"380\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"display: inline-block;*display:inline;*zoom:1;*margin-left:10px;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80 tc\">保险公司</td>");
                strb.AppendLine("<td class=\"w100 tc\">产值占比</td>");
                strb.AppendLine("<td class=\"w100 tc\">产值金额</td>");
                strb.AppendLine("<td class=\"w100 tc\">目标金额</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">中保</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[18]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[18]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[18]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">太保</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[19]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[19]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[19]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">平安</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[20]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[20]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[20]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">人寿</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[21]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[21]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[21]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">大地</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[22]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[22]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[22]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">中华联合</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[23]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[23]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[23]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">浙商</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[24]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[24]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[24]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">大众</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[25]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[25]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[25]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc bggray\">其他</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[26]["实际"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[26]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[26]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"bold tc\">合计</td>");
                strb.AppendLine("<td class=\"tc\">&nbsp;</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[27]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[27]["详细"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"w120\" style=\"background:Yellow;\">红旭汇绑定数目标</td>");
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[28]["目标"].ToString(), string.Empty, false, false));
                strb.AppendLine("<td class=\"w120\" style=\"background:Yellow;\">红旭汇绑定数实际</td>");
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[28]["实际"].ToString(), string.Empty, false, false));
                strb.AppendLine("<td class=\"w60\" style=\"background:Yellow;\">绑定率</td>");
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[30]["实际"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");
                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"w120\" style=\"background:Yellow;\">本月微信客户数目标</td>");
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[29]["目标"].ToString(), string.Empty, false, false));
                strb.AppendLine("<td class=\"w120\" style=\"background:Yellow;\">本月微信客户数实际</td>");
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[29]["实际"].ToString(), string.Empty, false, false));
                strb.AppendLine("<td class=\"w60\" style=\"background:Yellow;\">完成率</td>");
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[29]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.市场部)
            {
                #region 页面输出

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 0; i < 3; i++)
                {
                    strb.AppendFormat("<td class=\"w120\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["目标"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["实际"].ToString(), tbl.Rows[0]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[0]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[0]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["实际"].ToString(), tbl.Rows[1]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[1]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[1]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["实际"].ToString(), tbl.Rows[2]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[2]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[2]["完成率"]) < 100, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                for (int i = 0; i < 3; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">到店渠道</td>");
                for (int i = 3; i < 13; i++)
                {
                    strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"tc bold bggray\">占比</td>");
                for (int i = 3; i < 13; i++)
                {
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"tc bold bggray\">数值</td>");
                for (int i = 3; i < 13; i++)
                {
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendFormat("<td class=\"w80\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[13]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[13]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w80\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[14]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[14]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w120\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[15]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[15]["实际"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.行政部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"480\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\">");
                strb.AppendLine("<tr class=\"bold tc\" style=\"background-color: Orange;\">");
                strb.AppendLine("<td class=\"w120\">项目</td>");
                strb.AppendLine("<td class=\"w120\">车牌、负责人</td>");
                strb.AppendLine("<td class=\"w60\">第一周</td>");
                strb.AppendLine("<td class=\"w60\">第二周</td>");
                strb.AppendLine("<td class=\"w60\">第三周</td>");
                strb.AppendLine("<td class=\"w60\">第四周</td>");
                strb.AppendLine("<td class=\"w120\">上月未处理</td>");
                strb.AppendLine("<td class=\"w40\">违章</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr style=\"background-color: Yellow;\">");
                strb.AppendFormat("<td class=\"tc bold\" rowspan=\"7\" style=\"vertical-align: middle;\">{0}</td>", tbl.Rows[0]["项目"]);
                strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[0]["车牌、负责人"].ToString()) ? "&nbsp;" : tbl.Rows[0]["车牌、负责人"].ToString());
                strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[0]["第一周"].ToString()) ? "&nbsp;" : tbl.Rows[0]["第一周"].ToString());
                strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[0]["第二周"].ToString()) ? "&nbsp;" : tbl.Rows[0]["第二周"].ToString());
                strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[0]["第三周"].ToString()) ? "&nbsp;" : tbl.Rows[0]["第三周"].ToString());
                strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[0]["第四周"].ToString()) ? "&nbsp;" : tbl.Rows[0]["第四周"].ToString());
                strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[0]["上月未处理"].ToString()) ? "&nbsp;" : tbl.Rows[0]["上月未处理"].ToString());
                strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[0]["违章"].ToString()) ? "&nbsp;" : tbl.Rows[0]["违章"].ToString());
                strb.AppendLine("</tr>");
                for (int i = 1; i < 7; i++)
                {
                    strb.AppendLine("<tr style=\"background-color: Yellow;\">");
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["车牌、负责人"].ToString()) ? "&nbsp;" : tbl.Rows[i]["车牌、负责人"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第一周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第一周"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第二周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第二周"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第三周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第三周"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第四周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第四周"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["上月未处理"].ToString()) ? "&nbsp;" : tbl.Rows[i]["上月未处理"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["违章"].ToString()) ? "&nbsp;" : tbl.Rows[i]["违章"].ToString());
                    strb.AppendLine("</tr>");
                }
                for (int i = 7; i < 12; i++)
                {
                    strb.AppendLine("<tr>");
                    strb.AppendFormat("<td class=\"tl bold\" colspan=\"2\">{0}</td>", tbl.Rows[i]["项目"]);
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第一周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第一周"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第二周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第二周"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第三周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第三周"].ToString());
                    strb.AppendFormat("<td class=\"tc\">{0}</td>", string.IsNullOrEmpty(tbl.Rows[i]["第四周"].ToString()) ? "&nbsp;" : tbl.Rows[i]["第四周"].ToString());
                    strb.AppendLine("<td class=\"tc\" colspan=\"2\">&nbsp;</td>");
                    strb.AppendLine("</tr>");
                }
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.DCC部)
            {
                #region 页面输出

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 0; i < 5; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                for (int i = 0; i < 5; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, i > 1));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                for (int i = 0; i < 5; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), tbl.Rows[i]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[i]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[i]["完成率"]) < 100, i > 1));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                for (int i = 0; i < 5; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td>关键指标</td>");
                for (int i = 5; i < 10; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                for (int i = 5; i < 10; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                for (int i = 5; i < 10; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), tbl.Rows[i]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[i]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[i]["完成率"]) < 100, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                for (int i = 5; i < 10; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;*margin-left:10px;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">DCC线索明细</td>");
                strb.AppendLine("<td class=\"w60\">占比</td>");
                strb.AppendLine("</tr>");
                for (int i = 10; i < 17; i++)
                {
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["关键指标"]);
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, true));
                    strb.AppendLine("</tr>");
                }
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;*margin-left:10px;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">网络渠道占比</td>");
                strb.AppendLine("<td class=\"w60\">网络占比</td>");
                strb.AppendLine("</tr>");
                for (int i = 17; i < 22; i++)
                {
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["关键指标"]);
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, true));
                    strb.AppendLine("</tr>");
                }
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;*margin-left:10px;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w100\">成交渠道</td>");
                strb.AppendLine("<td class=\"w60\">成交占比</td>");
                strb.AppendLine("</tr>");
                for (int i = 22; i < 30; i++)
                {
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["关键指标"]);
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, true));
                    strb.AppendLine("</tr>");
                }
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendFormat("<td class=\"w80\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[30]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[30]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w80\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[31]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"w60\">{0}</td>", GetCellValue(tbl.Rows[31]["实际"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.二手车部)
            {
                #region 页面输出

                strb.AppendLine("<table width=\"640\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 0; i < 6; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", tbl.Rows[i]["关键指标"]);
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                for (int i = 0; i < 6; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                for (int i = 0; i < 6; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), tbl.Rows[i]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[i]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[i]["完成率"]) < 100, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                for (int i = 0; i < 6; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 6; i < 11; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", tbl.Rows[i]["关键指标"]);
                }
                strb.Append("<td class=\"w100\">&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                for (int i = 6; i < 11; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, false));
                }
                strb.Append("<td class=\"w100\">&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                for (int i = 6; i < 11; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, false));
                }
                strb.Append("<td class=\"w100\">&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                for (int i = 6; i < 11; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                }
                strb.Append("<td class=\"w100\">&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr>");
                strb.AppendFormat("<td class=\"tc w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[11]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc w60\">{0}</td>", GetCellValue(tbl.Rows[11]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc w60\" style=\"background:Yellow;\">{0}</td>", GetCellValue(tbl.Rows[12]["关键指标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc w60\">{0}</td>", GetCellValue(tbl.Rows[12]["实际"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.金融部)
            {
                #region 页面输出

                string[] listbxgs = new string[] { "中保", "太保", "平安", "人寿", "大地", "中华联合", "浙商", "大众", "其他" };
                decimal hjzbf = 0;
                decimal hjbycz = 0;
                decimal hjxbcz = 0;
                decimal hjxubcz = 0;
                decimal hjewcz = 0;
                for (int i = 0; i < 9; i++)
                {
                    hjbycz += DataConvert.SafeDecimal(tbl.Rows[i]["目标"]);
                    hjxbcz += DataConvert.SafeDecimal(tbl.Rows[i + 9]["目标"]);
                    hjxubcz += DataConvert.SafeDecimal(tbl.Rows[i + 18]["目标"]);
                    hjewcz += DataConvert.SafeDecimal(tbl.Rows[i + 27]["目标"]);
                }
                hjzbf = hjxbcz + hjxubcz + hjewcz;

                strb.AppendLine("<table width=\"800\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">&nbsp;</td>");
                strb.AppendLine("<td colspan=\"2\">目标</td>");
                strb.AppendLine("<td colspan=\"2\">实际完成</td>");
                strb.AppendLine("<td colspan=\"2\">其中新保完成</td>");
                strb.AppendLine("<td colspan=\"2\">其中续保完成</td>");
                strb.AppendLine("<td class=\"w80\" rowspan=\"2\">二网单独保费</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td>保险公司</td>");
                strb.AppendLine("<td class=\"w80\">本月产值</td>");
                strb.AppendLine("<td class=\"w80\">产值占比</td>");
                strb.AppendLine("<td class=\"w80\">总保费</td>");
                strb.AppendLine("<td class=\"w80\">完成比例</td>");
                strb.AppendLine("<td class=\"w80\">新保保费</td>");
                strb.AppendLine("<td class=\"w80\">新保比例</td>");
                strb.AppendLine("<td class=\"w80\">续保保费</td>");
                strb.AppendLine("<td class=\"w80\">续保比例</td>");
                strb.AppendLine("</tr>");
                for (int i = 0; i < listbxgs.Length; i++)
                {

                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", listbxgs[i]);
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, false));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, false));
                    strb.AppendFormat("<td>{0}</td>", string.IsNullOrEmpty(tbl.Rows[i + 9]["目标"].ToString()) && string.IsNullOrEmpty(tbl.Rows[i + 18]["目标"].ToString()) && string.IsNullOrEmpty(tbl.Rows[i + 27]["目标"].ToString()) ? "&nbsp;" : Math.Round(DataConvert.SafeDecimal(tbl.Rows[i + 9]["目标"]) + DataConvert.SafeDecimal(tbl.Rows[i + 18]["目标"]) + DataConvert.SafeDecimal(tbl.Rows[i + 27]["目标"]), 0).ToString());
                    strb.AppendFormat("<td>{0}</td>", hjzbf == 0 ? "&nbsp;" : (Math.Round((DataConvert.SafeDecimal(tbl.Rows[i + 9]["目标"]) + DataConvert.SafeDecimal(tbl.Rows[i + 18]["目标"]) + DataConvert.SafeDecimal(tbl.Rows[i + 27]["目标"])) * 100 / hjzbf, 1).ToString() + "%"));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i + 9]["目标"].ToString(), string.Empty, false, false));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i + 9]["实际"].ToString(), string.Empty, false, false));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i + 18]["目标"].ToString(), string.Empty, false, false));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i + 18]["实际"].ToString(), string.Empty, false, false));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i + 27]["目标"].ToString(), string.Empty, false, false));
                    strb.AppendLine("</tr>");
                }
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td>合计</td>");
                strb.AppendFormat("<td>{0}</td>", hjbycz);
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendFormat("<td>{0}</td>", hjzbf);
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendFormat("<td>{0}</td>", hjxbcz);
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendFormat("<td>{0}</td>", hjxubcz);
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendFormat("<td>{0}</td>", hjewcz);
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table width=\"360\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td colspan=\"3\">续保成功客户</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">一年</td>");
                strb.AppendLine("<td class=\"w80\">二年</td>");
                strb.AppendLine("<td class=\"w120\">三年及以上</td>");
                strb.AppendLine("<td class=\"w80\">合计</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[36]["实际"]);
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[37]["实际"]);
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[38]["实际"]);
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[39]["实际"]);
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td colspan=\"3\" class=\"bold bggray\">传统台次</td>");
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[40]["实际"]);
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td colspan=\"3\" class=\"bold bggray\">按揭台次</td>");
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[41]["实际"]);
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td colspan=\"3\" class=\"bold bggray\">电销保险总数</td>");
                strb.AppendFormat("<td>{0}</td>", tbl.Rows[42]["实际"]);
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td colspan=\"3\" class=\"bold bggray\">传统占总数比</td>");
                strb.AppendFormat("<td>{0}</td>", DataConvert.SafeDecimal(tbl.Rows[39]["实际"]) == 0 ? "&nbsp;" : (Math.Round(DataConvert.SafeDecimal(tbl.Rows[40]["实际"]) * 100 / DataConvert.SafeDecimal(tbl.Rows[39]["实际"]), 1).ToString() + "%"));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.无忧产品)
            {
                #region 页面输出

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;\">");
                strb.AppendLine("<tr><td colspan=\"11\" style=\"color:White;background-color:gray;font-weight:bold;font-size:large;padding-left:20px;\">销售数据</td></tr>");
                strb.AppendLine("<tr class=\"bold tc\" style=\"background-color:#8BC34A;\">");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td>新车销量</td>");
                strb.AppendLine("<td colspan=\"3\">机油套餐</td>");
                strb.AppendLine("<td colspan=\"3\">玻璃无忧服务</td>");
                strb.AppendLine("<td colspan=\"3\">划痕无忧服务</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 0; i < 10; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["目标"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[0]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[0]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[1]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[1]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[2]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[2]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["实际"].ToString(), tbl.Rows[3]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[3]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[3]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[4]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[4]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[5]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[5]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["实际"].ToString(), tbl.Rows[6]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[6]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[6]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[7]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[7]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[8]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[8]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["实际"].ToString(), tbl.Rows[9]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[9]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[9]["完成率"]) < 100, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[0]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[1]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[2]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[3]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[4]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[5]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[6]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[7]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[8]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[9]["目标"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc\" style=\"background-color:#8BC34A;\">");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td colspan=\"7\">延保无忧车服务</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 10; i < 17; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[10]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[10]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[11]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[11]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["实际"].ToString(), tbl.Rows[12]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[12]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[12]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[13]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[13]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[14]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[14]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[15]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[15]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[16]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[16]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[10]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[11]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[12]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[13]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[14]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[15]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[16]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr><td colspan=\"11\" style=\"color:White;background-color:gray;font-weight:bold;font-size:large;padding-left:20px;\">售后数据</td></tr>");
                strb.AppendLine("<tr class=\"bold tc\" style=\"background-color:#8BC34A;\">");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td>来厂台次</td>");
                strb.AppendLine("<td colspan=\"3\">机油套餐</td>");
                strb.AppendLine("<td colspan=\"3\">玻璃无忧服务</td>");
                strb.AppendLine("<td colspan=\"3\">划痕无忧服务</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td>关键指标</td>");
                for (int i = 17; i < 27; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[18]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[19]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[20]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[21]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[22]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[23]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[24]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[25]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[26]["目标"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[17]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[17]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[18]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[18]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[18]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[19]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[19]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[19]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[20]["实际"].ToString(), tbl.Rows[20]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[20]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[20]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[21]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[21]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[21]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[22]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[22]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[22]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[23]["实际"].ToString(), tbl.Rows[23]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[23]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[23]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[24]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[24]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[24]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[25]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[25]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[25]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[26]["实际"].ToString(), tbl.Rows[26]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[26]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[26]["完成率"]) < 100, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[17]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[18]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[19]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[20]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[21]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[22]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[23]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[24]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[25]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[26]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc\" style=\"background-color:#8BC34A;\">");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td colspan=\"7\">延保无忧车服务</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("<td>&nbsp;</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td>关键指标</td>");
                for (int i = 27; i < 34; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["关键指标"].ToString(), string.Empty, false, false));
                }
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[27]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[28]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[29]["目标"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[30]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[31]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[32]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[33]["目标"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[27]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[27]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[27]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[28]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[28]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[28]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[29]["实际"].ToString(), tbl.Rows[29]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[29]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[29]["完成率"]) < 100, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[30]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[30]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[30]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[31]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[31]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[31]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[32]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[32]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[32]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[33]["实际"].ToString(), string.Empty, !string.IsNullOrEmpty(tbl.Rows[33]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[33]["完成率"]) < 100, false));
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[27]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[28]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[29]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[30]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[31]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[32]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[33]["完成率"].ToString(), string.Empty, false, true));
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendFormat("<td>{0}</td>", "&nbsp;");
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }
            else if (dep == DayReportDep.客服部)
            {
                #region 页面输出

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 0; i < 4; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", tbl.Rows[i]["关键指标"]);
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                for (int i = 0; i < 4; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                for (int i = 0; i < 4; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), tbl.Rows[i]["详细"].ToString(), !string.IsNullOrEmpty(tbl.Rows[i]["完成率"].ToString()) && DataConvert.SafeDecimal(tbl.Rows[i]["完成率"]) < 100, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                for (int i = 0; i < 4; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">关键指标</td>");
                for (int i = 4; i < 8; i++)
                {
                    strb.AppendFormat("<td class=\"w100\">{0}</td>", tbl.Rows[i]["关键指标"]);
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">目标</td>");
                for (int i = 4; i < 8; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["目标"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">实际</td>");
                for (int i = 4; i < 8; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr class=\"tc\">");
                strb.AppendLine("<td class=\"bold bggray\">完成率</td>");
                for (int i = 4; i < 8; i++)
                {
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["完成率"].ToString(), string.Empty, false, true));
                }
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;*margin-left:10px;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">项目</td>");
                strb.AppendLine("<td class=\"w100\">销售不满意占比</td>");
                strb.AppendLine("</tr>");
                for (int i = 8; i < 14; i++)
                {
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["关键指标"].ToString().Replace("销售不满意占比", string.Empty));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, true));
                    strb.AppendLine("</tr>");
                }
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable\" style=\"vertical-align: top;display: inline-block;*display:inline;*zoom:1;*margin-left:10px;\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w80\">项目</td>");
                strb.AppendLine("<td class=\"w100\">售后不满意占比</td>");
                strb.AppendLine("</tr>");
                for (int i = 14; i < 22; i++)
                {
                    strb.AppendLine("<tr class=\"tc\">");
                    strb.AppendFormat("<td class=\"bold bggray\">{0}</td>", tbl.Rows[i]["关键指标"].ToString().Replace("售后不满意占比", string.Empty));
                    strb.AppendFormat("<td>{0}</td>", GetCellValue(tbl.Rows[i]["实际"].ToString(), string.Empty, false, true));
                    strb.AppendLine("</tr>");
                }
                strb.AppendLine("</table>");

                strb.AppendLine("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"datatable mt10\">");
                strb.AppendLine("<tr class=\"bold tc bggray\">");
                strb.AppendLine("<td class=\"w160\">类型</td>");
                strb.AppendLine("<td class=\"w60\">1年内</td>");
                strb.AppendLine("<td class=\"w60\">2年内</td>");
                strb.AppendLine("<td class=\"w60\">3年内</td>");
                strb.AppendLine("<td class=\"w80\">3-5年内</td>");
                strb.AppendLine("<td class=\"w60\">5年以上</td>");
                strb.AppendLine("</tr>");
                strb.AppendLine("<tr>");
                strb.AppendLine("<td class=\"tc bold bggray\">基盘保有客户量</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[22]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[23]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[24]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[25]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[26]["实际"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("<td class=\"tc bold bggray\">有效管理内客户量</td>");
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[27]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[28]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[29]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[30]["实际"].ToString(), string.Empty, false, false));
                strb.AppendFormat("<td class=\"tc\">{0}</td>", GetCellValue(tbl.Rows[31]["实际"].ToString(), string.Empty, false, false));
                strb.AppendLine("</tr>");
                strb.AppendLine("</table>");

                #endregion
            }

            return strb.ToString();
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

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            string checkstr = CheckForm();

            if (!string.IsNullOrEmpty(checkstr))
            {
                spMsg.InnerText = checkstr;
                return;
            }

            ExportExcel();
        }

        protected void btnXSYearAnalyze_Click(object sender, EventArgs e)
        {

            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("月份");
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

                for (int i = 0; i < 12; i++)
                {
                    DataRow row = tblresult.NewRow();

                    #region 销售数据

                    day = DateTime.Parse(day.Year.ToString() + "-" + (i + 1).ToString("00") + "-01");
                    DayReportDep dep = DayReportDep.销售部;
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                    if (list != null && list.Count > 0)
                    {
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), dep, day, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, day, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay);

                        row["月份"] = (i + 1);
                        tblKey.DefaultView.RowFilter = "关键指标='总销售台次'";
                        row["目标整车销售量"] = tblKey.DefaultView[0]["目标"];
                        row["实际整车销售量"] = tblKey.DefaultView[0]["实际"];
                        tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                        row["展厅目标数"] = tblDay.DefaultView[0]["目标值"];
                        row["展厅实际数"] = tblDay.DefaultView[0]["合计"];
                        row["整车实际销售额"] = monthtarget == null ? "" : monthtarget.XSbyzcsjxse;
                        row["整车裸车毛利额"] = monthtarget == null ? "" : monthtarget.XSbyzclcsjmle;
                        row["厂方返利收入"] = monthtarget == null ? "" : monthtarget.XSbycfflsjsr;
                        row["含厂家返利的预算毛利率"] = monthtarget == null ? "" : (DataConvert.SafeDecimal(monthtarget.XSbyzcysxse) > 0 ? ((DataConvert.SafeDecimal(monthtarget.XSbycfflyssr) + DataConvert.SafeDecimal(monthtarget.XSbyzclcysmle)) / DataConvert.SafeDecimal(monthtarget.XSbyzcysxse)).ToString() : "");
                        tblDay.DefaultView.RowFilter = "项目='上牌总金额'";
                        row["现金上牌手续费收入"] = tblDay.DefaultView[0]["合计"];
                        decimal ysspzje = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        tblDay.DefaultView.RowFilter = "项目='按揭净收入'";
                        row["按揭手续费净收入"] = tblDay.DefaultView[0]["合计"];
                        decimal ysajjsrsxf = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        row["厂方金融手续费净收入"] = monthtarget == null ? "" : monthtarget.XSbycfjrsxfjsr;
                        tblDay.DefaultView.RowFilter = "项目='二网保险返利'";
                        decimal ewbxfl = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        decimal ysewbxfl = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        tblDay.DefaultView.RowFilter = "项目='展厅含DCC保险返利'";
                        decimal ztbxfl = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        decimal ysztbxfl = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        row["保险返利收入"] = ztbxfl + ewbxfl;
                        tblDay.DefaultView.RowFilter = "项目='美容交车总金额'";
                        row["交车费收入"] = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        decimal ysjcfsr = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        tblDay.DefaultView.RowFilter = "项目='延保总金额'";
                        decimal ybzje = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        decimal ysybzje = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        tblDay.DefaultView.RowFilter = "项目='免费保养总金额'";
                        decimal zsmfbyzje = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        decimal yszsmfbyzje = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        row["销售延保和免费保养净收入"] = ybzje * 30 / 100 + zsmfbyzje;
                        row["销售部精品毛利收入"] = monthtarget == null ? "" : monthtarget.XSbyjpmlsr;
                        row["其他收入"] = monthtarget == null ? "" : monthtarget.XSbyqtsr;
                        decimal yszclcmle = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.XSbyzclcysmle); //整车裸车预算毛利额
                        decimal yscfflsr = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.XSbycfflyssr); //厂方返利预算收入
                        decimal ysjpmlsr = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.XSbyjpmlyssr);//本月精品预算毛利

                        row["整车综合预算毛利率"] =monthtarget == null ? "" : (DataConvert.SafeDecimal(monthtarget.XSbyzcysxse) > 0 ? ((yszclcmle + yscfflsr + ysspzje + ysajjsrsxf + ysewbxfl + ysztbxfl + ysjcfsr + ysybzje * 30 / 100 + yszsmfbyzje + ysjpmlsr + DataConvert.SafeDecimal(row["其他收入"])) /DataConvert.SafeDecimal(monthtarget.XSbyzcysxse)).ToString() : "");
                        tblDay.DefaultView.RowFilter = "项目='二网保险台次'";
                        decimal ewbxtc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        tblDay.DefaultView.RowFilter = "项目='展厅含DCC保险台次'";
                        decimal ztbxtc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        row["保险成交量"] = ewbxtc + ztbxtc;
                        tblDay.DefaultView.RowFilter = "项目='二网保险金额'";
                        decimal ewbxje = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        tblDay.DefaultView.RowFilter = "项目='展厅含DCC保险总金额'";
                        decimal ztbxje = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        row["新车保险交易额"] = ewbxje + ztbxje;
                        tblDay.DefaultView.RowFilter = "项目='上牌台次'";
                        row["上牌量"] = tblDay.DefaultView[0]["合计"];
                        tblDay.DefaultView.RowFilter = "项目='美容交车台次'";
                        row["美容交车台次"] = tblDay.DefaultView[0]["合计"];
                        row["美容交车平均单台净收入"] = monthtarget == null ? 0 : (DataConvert.SafeInt(row["美容交车台次"]) == 0 ? 0 : DataConvert.SafeDecimal(monthtarget.XSbymrjcjsr) / DataConvert.SafeInt(row["美容交车台次"]));
                        tblDay.DefaultView.RowFilter = "项目='本地按揭台次'";
                        decimal bdajtc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        tblDay.DefaultView.RowFilter = "项目='银行按揭台次'";
                        decimal yhajtc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        tblDay.DefaultView.RowFilter = "项目='厂家金融台次'";
                        decimal cjjrtc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        row["按揭台次"] = bdajtc + yhajtc + cjjrtc;
                        row["厂家金融台次"] = cjjrtc;
                        tblKey.DefaultView.RowFilter = "关键指标='按揭率'";
                        row["预算按揭率"] = tblKey.DefaultView[0]["目标"];
                        tblKey.DefaultView.RowFilter = "关键指标='按揭平均单台'";
                        row["预算按揭平均每台净收入"] = tblKey.DefaultView[0]["目标"];
                        tblDay.DefaultView.RowFilter = "项目='延保无忧车服务购买个数'";
                        row["销售延保的台次"] = tblDay.DefaultView[0]["合计"];
                        row["延保平均单车产值"] = DataConvert.SafeInt(row["销售延保的台次"]) > 0 ? (ybzje / DataConvert.SafeInt(row["销售延保的台次"])).ToString() : "";
                        row["美容交车平均单台净收入"] = monthtarget == null ? 0 : (DataConvert.SafeInt(row["销售延保的台次"]) == 0 ? 0 : DataConvert.SafeDecimal(monthtarget.XSbyybml) / DataConvert.SafeInt(row["销售延保的台次"]));
                        tblDay.DefaultView.RowFilter = "项目='免费保养台次（含赠送）'";
                        row["免费保养的台次"] = tblDay.DefaultView[0]["合计"];
                        row["免费保养平均单车产值"] = DataConvert.SafeInt(tblDay.DefaultView[0]["合计"]) > 0 ? (zsmfbyzje / DataConvert.SafeInt(tblDay.DefaultView[0]["合计"])).ToString() : "";
                        row["免费保养平均单台毛利"] = monthtarget == null ? 0 : (DataConvert.SafeInt(row["免费保养的台次"]) == 0 ? 0 : DataConvert.SafeDecimal(monthtarget.XSbyzsmfbyml) / DataConvert.SafeInt(row["免费保养的台次"]));
                        tblKey.DefaultView.RowFilter = "关键指标='附加值合计'";
                        row["展厅附加值净收入的平均单台目标"] = DataConvert.SafeDecimal(row["展厅目标数"]) == 0 ? 0 : DataConvert.SafeDecimal(tblKey.DefaultView[0]["目标"]) / DataConvert.SafeDecimal(row["展厅目标数"]);
                        tblKey.DefaultView.RowFilter = "关键指标='标准在库库存'";
                        row["标准库存量"] = tblKey.DefaultView[0]["目标"];
                        row["现有在库存车辆数"] = tblKey.DefaultView[0]["实际"];
                        tblKey.DefaultView.RowFilter = "关键指标='在途'";
                        row["现有在途库存数"] = tblKey.DefaultView[0]["实际"];
                        tblKey.DefaultView.RowFilter = "关键指标='在库超3个月'";
                        row["3个月以上的库存"] = tblKey.DefaultView[0]["实际"];
                        row["6个月以上的库存"] = monthtarget == null ? "" : monthtarget.XSzkclgytc;
                        row["1年以上的库存"] = monthtarget == null ? "" : monthtarget.XSzkcyntc;
                        row["标准库存周转天数"] = monthtarget == null ? "" : monthtarget.XSzzts;
                        tblKey.DefaultView.RowFilter = "关键指标='库存利息'";
                        row["库存资金占用总成本利息"] = tblKey.DefaultView[0]["实际"];

                    }

                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\年度销售数据分析.xls"));
                newfile = string.Format(@"{0}年度销售数据分析.xls", day.Year);
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

                sheet.GetRow(0).GetCell(0).SetCellValue(day.Year + "年度销售数据分析表");

                int maxrow = 0;
                for (int i = 0; i < tblresult.Rows.Count;i++ )
                {
                    if (!string.IsNullOrEmpty(tblresult.Rows[i]["月份"].ToString()))
                    {
                        maxrow = i;
                        sheet.GetRow(2).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["目标整车销售量"]));
                        sheet.GetRow(3).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["实际整车销售量"]));
                        sheet.GetRow(4).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅目标数"]));
                        sheet.GetRow(5).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅实际数"]));
                        sheet.GetRow(8).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["整车实际销售额"]) / 10000);
                        sheet.GetRow(9).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["整车裸车毛利额"]) / 10000);
                        sheet.GetRow(12).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["厂方返利收入"]) / 10000);
                        sheet.GetRow(15).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["现金上牌手续费收入"]) / 10000);
                        sheet.GetRow(16).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["按揭手续费净收入"]) / 10000);
                        sheet.GetRow(17).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["厂方金融手续费净收入"]) / 10000);
                        sheet.GetRow(18).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["保险返利收入"]) / 10000);
                        sheet.GetRow(19).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["交车费收入"]) / 10000);
                        sheet.GetRow(20).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["销售延保和免费保养净收入"]) / 10000);
                        sheet.GetRow(21).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["销售部精品毛利收入"]) / 10000);
                        sheet.GetRow(22).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["其他收入"]) / 10000);
                        sheet.GetRow(25).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["整车综合预算毛利率"]));
                        sheet.GetRow(27).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["保险成交量"]));
                        sheet.GetRow(29).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["新车保险交易额"]) / 10000);
                        sheet.GetRow(31).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["保险返利收入"]) / 10000);
                        sheet.GetRow(34).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["上牌量"]));
                        sheet.GetRow(37).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["美容交车台次"]));
                        sheet.GetRow(38).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["美容交车平均单台净收入"]) / 10000);
                        sheet.GetRow(40).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["按揭台次"]));
                        sheet.GetRow(41).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["厂家金融台次"]));
                        sheet.GetRow(43).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["预算按揭率"]) / 100);
                        sheet.GetRow(44).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["预算按揭平均每台净收入"]) / 10000);
                        sheet.GetRow(48).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["销售延保的台次"]));
                        sheet.GetRow(50).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["延保平均单车产值"]) / 10000);
                        sheet.GetRow(51).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["延保平均单台毛利"]) / 10000);
                        sheet.GetRow(52).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养的台次"]));
                        sheet.GetRow(54).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养平均单车产值"]) / 10000);
                        sheet.GetRow(55).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养平均单台毛利"]) / 10000);
                        sheet.GetRow(56).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅附加值净收入的平均单台目标"]) / 10000);
                        sheet.GetRow(58).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["标准库存量"]));
                        sheet.GetRow(59).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["现有在库存车辆数"]));
                        sheet.GetRow(60).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["现有在途库存数"]));
                        sheet.GetRow(61).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["3个月以上的库存"]));
                        sheet.GetRow(62).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["6个月以上的库存"]));
                        sheet.GetRow(63).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["1年以上的库存"]));
                        sheet.GetRow(64).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["标准库存周转天数"]));
                        sheet.GetRow(66).GetCell(i + 5).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["库存资金占用总成本利息"]) / 10000);

                    }
                    else if (i < 11 || (i == 11 && maxrow > 0))
                        sheet.SetColumnHidden(i + 5, true);
                }
                if (maxrow > 0)
                {
                    for (int i = 0; i < maxrow; i++)
                    {
                        sheet.SetColumnHidden(i + 5, true);
                    }
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

        protected void btnSHYearAnalyze_Click(object sender, EventArgs e)
        {

            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("月份");
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

                for (int i = 0; i < 12; i++)
                {
                    DataRow row = tblresult.NewRow();

                    DataTable tblDay, tblKey;

                    #region 售后数据

                    day = DateTime.Parse(day.Year.ToString() + "-" + (i + 1).ToString("00") + "-01");
                    DayReportDep dep = DayReportDep.售后部;
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                    if (list != null && list.Count > 0)
                    {
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), dep, day, true);
                        int days = 0;
                        tblDay = GetReport(dep, list, monthtarget, day, ref days);
                        tblKey = GetKeyReport(dep, list, monthtarget, tblDay);

                        row["月份"] = (i + 1);
                        tblDay.DefaultView.RowFilter = "项目='来厂台次'";
                        row["预算台次"] = tblDay.DefaultView[0]["目标值"];
                        row["来厂台次"] = tblDay.DefaultView[0]["合计"];
                        tblDay.DefaultView.RowFilter = "项目='当日产值'";
                        decimal wxcz = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        row["预算产值"] = tblDay.DefaultView[0]["目标值"];
                        row["维修产值"] = wxcz;

                        //一般维修额
                        row["一般维修额"] = monthtarget == null ? "" : monthtarget.SHbyybwxe;

                        //首保索赔额
                        row["首保索赔额"] = monthtarget == null ? "" : monthtarget.SHbysbspe;

                        //事故车收入
                        tblDay.DefaultView.RowFilter = "项目='事故总产值'";
                        decimal sgcsr = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        row["事故车收入"] = sgcsr;

                        //养护产品的收入
                        tblDay.DefaultView.RowFilter = "项目='养护产值'";
                        decimal yhczmb = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        row["养护产品的收入"] = tblDay.DefaultView[0]["合计"];

                        //他品牌收入
                        row["他牌车收入"] = monthtarget == null ? "" : monthtarget.SHbytppsr;

                        //其中事故车台次
                        tblDay.DefaultView.RowFilter = "项目='事故来厂台次'";
                        row["其中事故车台次"] = tblDay.DefaultView[0]["合计"];

                        //维修毛利额
                        row["维修毛利额"] = monthtarget == null ? "" : monthtarget.SHbywxmle;

                        //维修毛利率
                        row["维修毛利率"] = monthtarget == null ? "" : monthtarget.SHbywxmll;

                        //废旧收入
                        row["废旧收入"] = monthtarget == null ? "" : monthtarget.SHbyfjsr;

                        //含废旧的维修实际毛利率
                        row["含废旧的维修实际毛利率"] = monthtarget == null ? "" : monthtarget.SHbyhfjdwxsjmll;

                        //含废旧的维修预算毛利率
                        row["含废旧的维修预算毛利率"] = monthtarget == null ? "" : monthtarget.SHbyhfjdwxysmll;

                        //油漆收入
                        row["油漆收入"] = monthtarget == null ? "" : monthtarget.SHbyyqsr;

                        //油漆成本
                        row["油漆成本"] = monthtarget == null ? "" : monthtarget.SHbyyqcb;

                        //占产值收入比例目标
                        row["占产值收入比例目标"] = wxcz > 0 ? (yhczmb / (wxcz * (1 - ((sgcsr / wxcz) - (decimal)0.5)))).ToString() : string.Empty;

                        //养护产品毛利率
                        row["其中养护产品毛利率"] = monthtarget == null ? "" : monthtarget.SHbyyhcpmll;

                        //标准库存金额
                        row["标准库存金额"] = monthtarget == null ? "" : monthtarget.SHbybzkcje;

                        //期末实际库存额
                        row["期末实际库存额"] = monthtarget == null ? "" : monthtarget.SHbyqmsjkce;

                        //其中一年以上库存额
                        row["其中一年以上库存额"] = monthtarget == null ? "" : monthtarget.SHqzynyskce;

                        //标准的库存度
                        row["标准的库存度"] = monthtarget == null ? "" : monthtarget.SHbybzdkcd;

                        //实际库存度
                        row["实际库存度"] = monthtarget == null ? "" : monthtarget.SHbysjkcd;

                        //配件毛利率
                        row["配件毛利率"] = monthtarget == null ? "" : monthtarget.SHbypjmll;

                        //事故车毛利率
                        row["事故车毛利率"] = monthtarget == null ? "" : monthtarget.SHbysgcmll;

                        //一般维修毛利率
                        row["一般维修毛利率"] = monthtarget == null ? "" : monthtarget.SHbyybwxmll;

                        //他牌车维修毛利率
                        row["他牌车维修毛利率"] = monthtarget == null ? "" : monthtarget.SHbytppwxmll;

                        //续保预算台次
                        tblDay.DefaultView.RowFilter = "项目='延保无忧'";
                        row["续保预算台次"] = tblDay.DefaultView[0]["目标值"];
                        row["续保实际台次"] = tblDay.DefaultView[0]["合计"];

                        //续保返利收入
                        row["续保返利收入"] = monthtarget == null ? "" : monthtarget.SHbyxbflsr;

                        //续保平均单台净收入
                        row["续保平均单台净收入"] = monthtarget == null ? "" : monthtarget.SHbyxbpjdtjsr;

                        //免费保养预算台次
                        tblDay.DefaultView.RowFilter = "项目='免费保养'";
                        row["免费保养预算台次"] = tblDay.DefaultView[0]["目标值"];
                        row["免费保养实际台次"] = tblDay.DefaultView[0]["合计"];

                        //延保返利收入
                        row["延保返利收入"] = monthtarget == null ? "" : monthtarget.SHbyybflsr;

                        //延保平均单台净收入
                        row["延保平均单台净收入"] = monthtarget == null ? "" : monthtarget.SHbyybpjdtjsr;

                        //导航升级业务个数
                        tblDay.DefaultView.RowFilter = "项目='导航升级客户数'";
                        row["导航升级业务个数"] = tblDay.DefaultView[0]["合计"];

                        //导航升级业务平均单台收入
                        row["导航升级业务平均单台收入"] = monthtarget == null ? "" : monthtarget.SHbydhsjywpjdtsr;

                    }

                    #endregion

                    #region 销售数据

                    dep = DayReportDep.销售部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    if (list != null && list.Count > 0)
                    {
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), dep, day, true);
                        int days = 0;
                        tblDay = GetReport(dep, list, monthtarget, day, ref days);
                        tblKey = GetKeyReport(dep, list, monthtarget, tblDay);

                        //续保目标渗透率
                        tblDay.DefaultView.RowFilter = "项目='免费保养总金额'";
                        row["免费保养的收入"] = tblDay.DefaultView[0]["合计"];

                        //续保目标渗透率
                        tblKey.DefaultView.RowFilter = "关键指标='无忧延保渗透率'";
                        row["续保目标渗透率"] = tblKey.DefaultView[0]["目标"];
                        row["续保实际渗透率"] = tblKey.DefaultView[0]["实际"];

                        //续保目标渗透率
                        tblDay.DefaultView.RowFilter = "项目='延保无忧车服务购买金额'";
                        row["续保总额"] = tblDay.DefaultView[0]["合计"];

                        tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                        decimal ztjctsmb = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        decimal ztjcts = DataConvert.SafeDecimal(tblDay.DefaultView[0]["合计"]);
                        row["免费保养目标渗透率"] = ztjctsmb > 0 ? (DataConvert.SafeDecimal(row["免费保养预算台次"]) / ztjctsmb).ToString() : string.Empty;
                        row["免费保养实际渗透率"] = ztjcts > 0 ? (DataConvert.SafeDecimal(row["免费保养实际台次"]) / ztjcts).ToString() : string.Empty;

                        tblDay.DefaultView.RowFilter = "项目='免费保养总金额'";
                        row["免费保养总额"] = tblDay.DefaultView[0]["合计"];
                    }
                    #endregion

                    tblresult.Rows.Add(row);
                }

                IWorkbook workbook = null;
                ISheet sheet = null;
                string newfile = string.Empty;
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\年度售后数据分析.xls"));
                newfile = string.Format(@"{0}年度售后数据分析.xls", day.Year);
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

                sheet.GetRow(0).GetCell(0).SetCellValue(string.Format("单位名称：{0}   {1}年度含税售后收入业绩汇总表     单位:   万元", ddlCorp.SelectedItem.Text, day.Year));

                int maxrow = 0;
                for (int i = 0; i < tblresult.Rows.Count;i++ )
                {
                    if (!string.IsNullOrEmpty(tblresult.Rows[i]["月份"].ToString()))
                    {
                        maxrow = i;
                        sheet.GetRow(3+i).GetCell(1).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["预算台次"]));
                        sheet.GetRow(3 + i).GetCell(2).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["来厂台次"]));
                        sheet.GetRow(3 + i).GetCell(3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["预算产值"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(4).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["维修产值"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(7).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["一般维修额"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(8).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["首保索赔额"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(9).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["事故车收入"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(10).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养的收入"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(11).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["养护产品的收入"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(12).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["他牌车收入"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(13).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["其中事故车台次"]));
                        sheet.GetRow(3 + i).GetCell(15).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["维修毛利额"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(16).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["维修毛利率"]) / 100);
                        sheet.GetRow(3 + i).GetCell(17).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["废旧收入"]) / 10000);
                        sheet.GetRow(3 + i).GetCell(19).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["含废旧的维修实际毛利率"]) / 100);
                        sheet.GetRow(3 + i).GetCell(20).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["含废旧的维修预算毛利率"]) / 100);
                        sheet.GetRow(19 + i).GetCell(1).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["油漆收入"]) / 10000);
                        sheet.GetRow(19 + i).GetCell(2).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["油漆成本"]) / 10000);
                        sheet.GetRow(19 + i).GetCell(4).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["占产值收入比例目标"]));
                        sheet.GetRow(19 + i).GetCell(6).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["其中养护产品毛利率"]));
                        sheet.GetRow(19 + i).GetCell(7).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["标准库存金额"]) / 10000);
                        sheet.GetRow(19 + i).GetCell(8).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["期末实际库存额"]) / 10000);
                        sheet.GetRow(19 + i).GetCell(9).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["其中一年以上库存额"]) / 10000);
                        sheet.GetRow(19 + i).GetCell(11).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["标准的库存度"]));
                        sheet.GetRow(19 + i).GetCell(12).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["实际库存度"]));
                        sheet.GetRow(19 + i).GetCell(14).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["配件毛利率"]) / 100);
                        sheet.GetRow(19 + i).GetCell(15).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["事故车毛利率"]) / 100);
                        sheet.GetRow(19 + i).GetCell(16).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["一般维修毛利率"]) / 100);
                        sheet.GetRow(19 + i).GetCell(17).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["他牌车维修毛利率"]) / 100);
                        sheet.GetRow(35 + i).GetCell(1).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["续保预算台次"]));
                        sheet.GetRow(35 + i).GetCell(2).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["续保实际台次"]));
                        sheet.GetRow(35 + i).GetCell(3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["续保目标渗透率"]) / 100);
                        sheet.GetRow(35 + i).GetCell(6).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["续保总额"]) / 10000);
                        sheet.GetRow(35 + i).GetCell(7).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["续保返利收入"]) / 10000);
                        sheet.GetRow(35 + i).GetCell(8).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["续保平均单台净收入"]) / 10000);
                        sheet.GetRow(35 + i).GetCell(9).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养预算台次"]));
                        sheet.GetRow(35 + i).GetCell(10).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养实际台次"]));
                        sheet.GetRow(35 + i).GetCell(11).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养目标渗透率"]) / 100);
                        sheet.GetRow(35 + i).GetCell(12).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养实际渗透率"]) / 100);
                        sheet.GetRow(35 + i).GetCell(14).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["免费保养总额"]) / 10000);
                        sheet.GetRow(35 + i).GetCell(15).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["延保返利收入"]) / 10000);
                        sheet.GetRow(35 + i).GetCell(16).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["延保平均单台净收入"]) / 10000);
                        sheet.GetRow(35 + i).GetCell(17).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["导航升级业务个数"]));
                        sheet.GetRow(35 + i).GetCell(18).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["导航升级业务平均单台收入"]) / 10000);

                    }
                    else
                    {
                        sheet.GetRow(i + 3).ZeroHeight = true;
                        sheet.GetRow(i + 19).ZeroHeight = true;
                        sheet.GetRow(i + 35).ZeroHeight = true;
                    }
                }
                //if (maxrow > 0)
                //{
                //    for (int i = 0; i < maxrow; i++)
                //    {
                //        sheet.GetRow(i + 3).ZeroHeight = true;
                //        sheet.GetRow(i + 21).ZeroHeight = true;
                //        sheet.GetRow(i + 39).ZeroHeight = true;
                //    }
                //}

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

        protected void btnJPYearAnalyze_Click(object sender, EventArgs e)
        {

            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day))
            {
                DataTable tblresult = new DataTable();

                #region 表结构

                tblresult.Columns.Add("月份");
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

                for (int i = 0; i < 12; i++)
                {
                    DataRow row = tblresult.NewRow();

                    decimal ysjpmle = 0,ysjpcz; //预算精品毛利额,预算精品产值
                    decimal ysztdccz = 0,yswddtcz = 0,ysshdtcz =0; //预算展厅单车产值,预算网点单台产值,预算售后单台产值
                    decimal ysztxl = 0,yswdxl = 0,ysshlctc = 0; //预算展厅销量，预算网点销量，预算售后來厂台次

                    #region 精品数据

                    day = DateTime.Parse(day.Year.ToString() + "-" + (i + 1).ToString("00") + "-01");
                    DayReportDep dep = DayReportDep.精品部;
                    DailyReportQuery query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                    if (list != null && list.Count > 0)
                    {
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), dep, day, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, day, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay);

                        row["月份"] = (i + 1);
                        decimal ysztjpmle = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPysztjpmle); //预算展厅精品毛利额
                        decimal yswdjpmle = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPyswdjpmle); //预算网点精品毛利额
                        decimal ysshjpmle = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPysshjpmle); //预算售后精品毛利额
                        ysjpmle = ysztjpmle + yswdjpmle + ysshjpmle; //预算精品毛利额

                        ysztdccz = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPysztdccz); //预算展厅单车产值
                        yswddtcz = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPyswddtcz); //预算网点单台产值
                        ysshdtcz = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPysshdtcz); //预算售后单台产值
                        //展厅实际精品产值
                        row["展厅实际精品产值"] = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPztsjjpcz);

                        //展厅实际的毛利额
                        row["展厅实际的毛利额"] = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPztsjmle);

                        //展厅单台目标
                        row["展厅单台目标"] = ysztdccz;

                        //网点精品毛利额
                        row["网点精品毛利额"] = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPwdjpmle); //网点精品毛利额

                        //网点目标单台产值
                        row["网点目标单台产值"] = yswddtcz;


                        //售后精品毛利额
                        row["售后精品毛利额"] = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPshjpmle); //售后精品毛利额

                        //售后精品台次
                        tblDay.DefaultView.RowFilter = "项目='服务部安装台次'";
                        row["售后精品台次"] = tblDay.DefaultView[0]["合计"];

                        //售后预算单台产值
                        row["售后预算单台产值"] = ysshdtcz;

                        //期末库存数
                        row["期末库存数"] = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPqmkcs);

                        //其中：3个月以上滞销库存
                        row["3个月以上滞销库存"] = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPsgyyszxkc);

                        //1年以上滞销库存
                        row["1年以上滞销库存"] = monthtarget == null ? 0 : DataConvert.SafeDecimal(monthtarget.JPynyszxkc);
                    }

                    #endregion

                    #region 销售数据

                    day = DateTime.Parse(day.Year.ToString() + "-" + (i + 1).ToString("00") + "-01");
                    dep = DayReportDep.销售部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    if (list != null && list.Count > 0)
                    {
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), dep, day, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, day, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay);


                        //展厅销量
                        tblDay.DefaultView.RowFilter = "项目='展厅交车台数'";
                        ysztxl = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        row["展厅销量"] = tblDay.DefaultView[0]["合计"];

                        //网点销量
                        tblDay.DefaultView.RowFilter = "项目='二网销售台次'";
                        yswdxl = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        row["网点销售量"] = tblDay.DefaultView[0]["合计"];

                        //展厅实际精品台次
                        tblDay.DefaultView.RowFilter = "项目='展厅成交精品台次'";
                        row["展厅实际精品台次"] = tblDay.DefaultView[0]["合计"];

                        //网点实际精品产值
                        tblDay.DefaultView.RowFilter = "项目='二网精品金额'";
                        row["网点实际精品产值"] = tblDay.DefaultView[0]["合计"];

                        //网点精品台次
                        tblDay.DefaultView.RowFilter = "项目='二网精品台次'";
                        row["网点精品台次"] = tblDay.DefaultView[0]["合计"];

                    }

                    #endregion

                    #region 售后数据

                    day = DateTime.Parse(day.Year.ToString() + "-" + (i + 1).ToString("00") + "-01");
                    dep = DayReportDep.售后部;
                    query = new DailyReportQuery()
                    {
                        DayUnique = day.ToString("yyyyMM"),
                        CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                        DayReportDep = dep
                    };
                    query.OrderBy = " [DayUnique] ASC";
                    list = DailyReports.Instance.GetList(query, true);
                    if (list != null && list.Count > 0)
                    {
                        list = list.FindAll(l => l.DailyReportCheckStatus != DailyReportCheckStatus.审核不通过);
                        MonthlyTargetInfo monthtarget = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), dep, day, true);
                        int days = 0;
                        DataTable tblDay = GetReport(dep, list, monthtarget, day, ref days);
                        DataTable tblKey = GetKeyReport(dep, list, monthtarget, tblDay);

                        //售后來厂台次
                        tblDay.DefaultView.RowFilter = "项目='来厂台次'";
                        ysshlctc = DataConvert.SafeDecimal(tblDay.DefaultView[0]["目标值"]);
                        row["售后来厂台次"] = tblDay.DefaultView[0]["合计"];

                        //售后实际精品产值
                        tblDay.DefaultView.RowFilter = "项目='精品美容产值'";
                        row["售后实际精品产值"] = tblDay.DefaultView[0]["合计"];
                    }

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
                string fileName = Utils.GetMapPath(string.Format(@"\App_Data\年度精品数据分析.xls"));
                newfile = string.Format(@"{0}年度精品数据分析.xls", day.Year);
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

                sheet.GetRow(0).GetCell(0).SetCellValue(day.Year + "年度精品数据分析表");
                sheet.GetRow(1).GetCell(3).SetCellValue(ddlCorp.SelectedItem.Text);

                int maxrow = 0;
                for (int i = 0; i < tblresult.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(tblresult.Rows[i]["月份"].ToString()))
                    {
                        maxrow = i;
                        sheet.GetRow(9).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["预算毛利率"]));
                        sheet.GetRow(11).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅按预算月度产值"]) / 10000);
                        sheet.GetRow(13).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅实际精品产值"]) / 10000);
                        sheet.GetRow(14).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅实际的毛利额"]) / 10000);
                        sheet.GetRow(16).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅实际精品台次"]));
                        sheet.GetRow(17).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅销量"]));
                        sheet.GetRow(18).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["展厅单台目标"]) / 10000);
                        sheet.GetRow(22).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["网点按预算月度产值"]) / 10000);
                        sheet.GetRow(24).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["网点实际精品产值"]) / 10000);
                        sheet.GetRow(25).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["网点精品毛利额"]) / 10000);
                        sheet.GetRow(27).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["网点精品台次"]));
                        sheet.GetRow(28).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["网点销售量"]));
                        sheet.GetRow(29).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["网点目标单台产值"]) / 10000);
                        sheet.GetRow(33).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["售后按预算月度产值"]) / 10000);
                        sheet.GetRow(35).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["售后实际精品产值"]) / 10000);
                        sheet.GetRow(36).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["售后精品毛利额"]) / 10000);
                        sheet.GetRow(38).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["售后精品台次"]));
                        sheet.GetRow(42).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["售后来厂台次"]));
                        sheet.GetRow(43).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["售后预算单台产值"]) / 10000);
                        sheet.GetRow(44).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["期末库存数"]));
                        sheet.GetRow(45).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["3个月以上滞销库存"]));
                        sheet.GetRow(46).GetCell(i + 3).SetCellValue(DataConvert.SafeDouble(tblresult.Rows[i]["1年以上滞销库存"]));
                    }
                    else if (i < 11 || (i == 11 && maxrow > 0))
                        sheet.SetColumnHidden(i + 3, true);
                }
                if (maxrow > 0)
                {
                    for (int i = 0; i < maxrow; i++)
                    {
                        sheet.SetColumnHidden(i + 3, true);
                    }
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

        private string CheckForm()
        {
            string result = string.Empty;
            DateTime day = DateTime.Today;
            if (ddlCorp.SelectedIndex == 0)
                result = "请选择公司";
            else if (!DateTime.TryParse(txtDate.Text + "-01", out day))
                result = "请选择月份";

            return result;
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

        protected string GetFlayWidth()
        {
            string result = string.Empty;

            result = "405px";

            switch (CurrentDep)
            {
                case DayReportDep.财务部:
                    result = "324px";
                    break;
                case DayReportDep.行政部:
                    result = "243px";
                    break;
                default:
                    break;
            }

            return result;
        }

        private string GetCellValue(string v, string detail, bool isred, bool ispersent)
        {
            string result = string.Empty;

            int lastday = 0;
            if (!string.IsNullOrEmpty(lastDayUnique))
                lastday = DataConvert.SafeInt(lastDayUnique.Substring(6, 2));
            isred = (lastday >= 25) && isred;
            result = string.IsNullOrEmpty(v) ? "&nbsp;" : string.Format("<span {1}>{0}{3}</span>{2}", v, isred ? "class=\"red\"" : string.Empty, detail, ispersent ? "%" : string.Empty);
            //result = string.IsNullOrEmpty(v) ? "&nbsp;" : string.Format("<span {1}>{0}{3}</span>{2}", v, string.Empty, detail, ispersent ? "%" : string.Empty);

            return result;
        }
    }
}