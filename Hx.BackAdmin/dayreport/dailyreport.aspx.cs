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
using System.Text;
using Hx.Components.Query;
using Hx.Car.Entity;
using Hx.Car;

namespace Hx.BackAdmin.dayreport
{
    public partial class dailyreport : AdminBase
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
                    string[] deppowers = CurrentUser.DayReportDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
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

        private static object sync_helper = new object();
        private bool allowmodify = true;

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

            string[] corppowers = CurrentUser.DayReportCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<CorporationInfo> corps = Corporations.Instance.GetList(true);
            ddlCorp.DataSource = corps.FindAll(c => corppowers.Contains(c.ID.ToString()) || (c.ID == CurrentUser.CorporationID && corppowers.Length == 0));
            ddlCorp.DataTextField = "Name";
            ddlCorp.DataValueField = "ID";
            ddlCorp.DataBind();
            SetSelectedByValue(ddlCorp, CurrentUser.CorporationID.ToString());
        }

        private void LoadData()
        {
            DailyReportInfo report = null;
            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day))
            {
                report = DailyReports.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
            }

            if (report != null)
            {

            }
            else
            {
                foreach (Control c in up1.ContentTemplateContainer.Controls.Cast<Control>())
                {
                    if (c.GetType() == typeof(TextBox) && c.ID != "txtDate")
                    {
                        ((TextBox)c).Text = string.Empty;
                    }
                }
            }

            #region 销售部特殊处理

            if (CurrentDep == DayReportDep.销售部)
            {
                MonthlyTargetInfo target = null;
                DateTime daytarget = DateTime.Today;
                if (DateTime.TryParse(txtDate.Text, out daytarget))
                {
                    target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, daytarget, true);
                }
                if (target == null)
                    target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, daytarget.AddMonths(-1), true);
                if (target != null)
                {
                    txtXSztcl.Text = target.XSztcl;
                    txtXSclpjdj.Text = target.XSclpjdj;
                    txtXSzzts.Text = target.XSzzts;
                }
            }

            #endregion

            #region 财务部特殊处理

            if (CurrentDep == DayReportDep.财务部 && HasMonthlyTargetPower())
            {
                foreach (Control c in up1.ContentTemplateContainer.Controls.Cast<Control>())
                {
                    if (c.GetType() == typeof(TextBox) && c.ID.StartsWith("txtCWycdkdq"))
                    {
                        ((TextBox)c).Text = DataConvert.SafeInt(c.ID.Replace("txtCWycdkdq", string.Empty)).ToString() + "号";
                    }
                }

                MonthlyTargetInfo target = null;
                DateTime daytarget = DateTime.Today;
                if (DateTime.TryParse(txtDate.Text, out daytarget))
                {
                    target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, daytarget, true);
                }
                if (target == null)
                {
                    target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, daytarget.AddMonths(-1), true);
                    if (target != null)
                    {
                        DailyReportQuery query = new DailyReportQuery()
                        {
                            DayUnique = daytarget.AddMonths(-1).ToString("yyyyMM"),
                            CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                            DayReportDep = CurrentDep
                        };
                        List<DailyReportInfo> list = DailyReports.Instance.GetList(query, true);
                        List<DailyReportModuleInfo> rlist = DayReportModules.Instance.GetList(true);

                        #region 计算资金余额

                        List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                        foreach (DailyReportInfo r in list)
                        {
                            if (!string.IsNullOrEmpty(r.SCReport))
                            {
                                data.Add(json.Deserialize<Dictionary<string, string>>(r.SCReport));
                            }
                        }
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
                        decimal hjdxssr = data.Sum(d => d.ContainsKey(midxssr.ToString()) ? DataConvert.SafeDecimal(d[midxssr.ToString()]) : 0);
                        decimal hjajfkyj = data.Sum(d => d.ContainsKey(midajfkyj.ToString()) ? DataConvert.SafeDecimal(d[midajfkyj.ToString()]) : 0);
                        decimal hjwxsr = data.Sum(d => d.ContainsKey(midwxsr.ToString()) ? DataConvert.SafeDecimal(d[midwxsr.ToString()]) : 0);
                        decimal hjyhfd = data.Sum(d => d.ContainsKey(midyhfd.ToString()) ? DataConvert.SafeDecimal(d[midyhfd.ToString()]) : 0);
                        decimal hjjtnbzjjr = data.Sum(d => d.ContainsKey(midjtnbzjjr.ToString()) ? DataConvert.SafeDecimal(d[midjtnbzjjr.ToString()]) : 0);
                        decimal hjqtsr = data.Sum(d => d.ContainsKey(midqtsr.ToString()) ? DataConvert.SafeDecimal(d[midqtsr.ToString()]) : 0);

                        decimal hjzczc = data.Sum(d => d.ContainsKey(midzczc.ToString()) ? DataConvert.SafeDecimal(d[midzczc.ToString()]) : 0);
                        decimal hjbjzc = data.Sum(d => d.ContainsKey(midbjzc.ToString()) ? DataConvert.SafeDecimal(d[midbjzc.ToString()]) : 0);
                        decimal hjyhdkdq = 0;
                        if (target != null && !string.IsNullOrEmpty(target.CWycdkdq))
                        {
                            string[] ycdkdq = target.CWycdkdq.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string strnumber in ycdkdq)
                            {
                                hjyhdkdq += DataConvert.SafeDecimal(strnumber, 0);
                            }
                        }
                        decimal hjjtnbzjjc = data.Sum(d => d.ContainsKey(midjtnbzjjc.ToString()) ? DataConvert.SafeDecimal(d[midjtnbzjjc.ToString()]) : 0);
                        decimal hjqtzc = data.Sum(d => d.ContainsKey(midqtzc.ToString()) ? DataConvert.SafeDecimal(d[midqtzc.ToString()]) : 0);

                        decimal hjsr = hjdxssr + hjajfkyj + hjwxsr + hjyhfd + hjjtnbzjjr + hjqtsr;
                        decimal hjzc = hjzczc + hjbjzc + hjyhdkdq + hjjtnbzjjc + hjqtzc;
                        txtCWzjye.Text = Math.Round(DataConvert.SafeDecimal(target.CWzjye) + hjsr - hjzc, 2).ToString();

                        #endregion

                        #region 余额分项

                        DailyReportInfo lastmonthlastreport = list.Find(l => l.DayUnique == DateTime.Parse(daytarget.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyyMMdd") && !string.IsNullOrEmpty(l.SCReport));
                        if (lastmonthlastreport != null)
                        {
                            int midposwdz = rlist.Find(l => l.Department == CurrentDep && l.Name == "POS未到帐").ID;
                            int midyhzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "银行帐户余额").ID;
                            int midnhzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "其中农行").ID;
                            int midzhzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "中行").ID;
                            int midghzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "工行").ID;
                            int midjianhzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "建行").ID;
                            int midjhzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "交行").ID;
                            int midmszhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "民生").ID;
                            int midpazhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "平安").ID;
                            int midzxzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "中信").ID;
                            int midhxzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "华夏").ID;
                            int midzszhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "浙商").ID;
                            int midtlzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "泰隆").ID;
                            int midqtyhzhye = rlist.Find(l => l.Department == CurrentDep && l.Name == "其他银行").ID;
                            int midxjczhj = rlist.Find(l => l.Department == CurrentDep && l.Name == "现金合计").ID;
                            int midlcxj = rlist.Find(l => l.Department == CurrentDep && l.Name == "留存现金").ID;
                            if (!string.IsNullOrEmpty(lastmonthlastreport.SCReport))
                            {
                                Dictionary<string, string> lastmonthlastdata = json.Deserialize<Dictionary<string, string>>(lastmonthlastreport.SCReport);
                                txtCWposwdz.Text = lastmonthlastdata.ContainsKey(midposwdz.ToString()) ? lastmonthlastdata[midposwdz.ToString()] : string.Empty;
                                txtCWyhzhye.Text = lastmonthlastdata.ContainsKey(midyhzhye.ToString()) ? lastmonthlastdata[midyhzhye.ToString()] : string.Empty;
                                txtCWnhzhye.Text = lastmonthlastdata.ContainsKey(midnhzhye.ToString()) ? lastmonthlastdata[midnhzhye.ToString()] : string.Empty;
                                txtCWzhzhye.Text = lastmonthlastdata.ContainsKey(midzhzhye.ToString()) ? lastmonthlastdata[midzhzhye.ToString()] : string.Empty;
                                txtCWghzhye.Text = lastmonthlastdata.ContainsKey(midghzhye.ToString()) ? lastmonthlastdata[midghzhye.ToString()] : string.Empty;
                                txtCWjianhzhye.Text = lastmonthlastdata.ContainsKey(midjianhzhye.ToString()) ? lastmonthlastdata[midjianhzhye.ToString()] : string.Empty;
                                txtCWjhzhye.Text = lastmonthlastdata.ContainsKey(midjhzhye.ToString()) ? lastmonthlastdata[midjhzhye.ToString()] : string.Empty;
                                txtCWmszhye.Text = lastmonthlastdata.ContainsKey(midmszhye.ToString()) ? lastmonthlastdata[midmszhye.ToString()] : string.Empty;
                                txtCWpazhye.Text = lastmonthlastdata.ContainsKey(midpazhye.ToString()) ? lastmonthlastdata[midpazhye.ToString()] : string.Empty;
                                txtCWzxzhye.Text = lastmonthlastdata.ContainsKey(midzxzhye.ToString()) ? lastmonthlastdata[midzxzhye.ToString()] : string.Empty;
                                txtCWhxzhye.Text = lastmonthlastdata.ContainsKey(midhxzhye.ToString()) ? lastmonthlastdata[midhxzhye.ToString()] : string.Empty;
                                txtCWzszhye.Text = lastmonthlastdata.ContainsKey(midzszhye.ToString()) ? lastmonthlastdata[midzszhye.ToString()] : string.Empty;
                                txtCWtlzhye.Text = lastmonthlastdata.ContainsKey(midtlzhye.ToString()) ? lastmonthlastdata[midtlzhye.ToString()] : string.Empty;
                                txtCWqtyhzhye.Text = lastmonthlastdata.ContainsKey(midqtyhzhye.ToString()) ? lastmonthlastdata[midqtyhzhye.ToString()] : string.Empty;
                                txtCWxjczhj.Text = lastmonthlastdata.ContainsKey(midxjczhj.ToString()) ? lastmonthlastdata[midxjczhj.ToString()] : string.Empty;
                                txtCWlcxj.Text = lastmonthlastdata.ContainsKey(midlcxj.ToString()) ? lastmonthlastdata[midlcxj.ToString()] : string.Empty; ;
                            }
                        }

                        #endregion
                    }
                }
                else
                {
                    txtCWzjye.Text = target.CWzjye;
                    txtCWposwdz.Text = target.CWposwdz;
                    txtCWyhzhye.Text = target.CWyhzhye;
                    txtCWnhzhye.Text = target.CWnhzhye;
                    txtCWzhzhye.Text = target.CWzhzhye;
                    txtCWghzhye.Text = target.CWghzhye;
                    txtCWjianhzhye.Text = target.CWjianhzhye;
                    txtCWjhzhye.Text = target.CWjhzhye;
                    txtCWmszhye.Text = target.CWmszhye;
                    txtCWpazhye.Text = target.CWpazhye;
                    txtCWzxzhye.Text = target.CWzxzhye;
                    txtCWhxzhye.Text = target.CWhxzhye;
                    txtCWzszhye.Text = target.CWzszhye;
                    txtCWtlzhye.Text = target.CWtlzhye;
                    txtCWqtyhzhye.Text = target.CWqtyhzhye;
                    txtCWxjczhj.Text = target.CWxjczhj;
                    txtCWlcxj.Text = target.CWlcxj;

                    hdnCWycdkdq.Value = target.CWycdkdq;

                    if (!string.IsNullOrEmpty(target.CWycdkdq))
                    {
                        string[] ycdkdq = target.CWycdkdq.Split(new char[] { ',' }, StringSplitOptions.None);
                        foreach (Control c in up1.ContentTemplateContainer.Controls.Cast<Control>())
                        {
                            if (c.GetType() == typeof(TextBox) && c.ID.StartsWith("txtCWycdkdq"))
                            {
                                if (!string.IsNullOrEmpty(ycdkdq[DataConvert.SafeInt(c.ID.Replace("txtCWycdkdq", string.Empty), 1) - 1]))
                                {
                                    ((TextBox)c).Text = ycdkdq[DataConvert.SafeInt(c.ID.Replace("txtCWycdkdq", string.Empty), 1) - 1];
                                    ((TextBox)c).Attributes["class"] = "srk7 tr cwycdkdq";
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region 行政部特殊处理

            if (CurrentDep == DayReportDep.行政部 && HasMonthlyTargetPower())
            {
                MonthlyTargetInfo target = null;
                DateTime daytarget = DateTime.Today;
                if (DateTime.TryParse(txtDate.Text, out daytarget))
                {
                    target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, daytarget, true);
                }
                if (target != null)
                {
                    if (!string.IsNullOrEmpty(target.XZcpfzr))
                    {
                        string[] cpfzr = target.XZcpfzr.Split(new char[] { '|' }, StringSplitOptions.None);
                        txtXZcpfzr1.Text = cpfzr[0];
                        txtXZcpfzr2.Text = cpfzr[1];
                        txtXZcpfzr3.Text = cpfzr[2];
                        txtXZcpfzr4.Text = cpfzr[3];
                        txtXZcpfzr5.Text = cpfzr[4];
                        txtXZcpfzr6.Text = cpfzr.Length > 5 ? cpfzr[5] : string.Empty;
                        txtXZcpfzr7.Text = cpfzr.Length > 5 ? cpfzr[6] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(target.XZdyzclwzcs))
                    {
                        string[] dyzclwzcs = target.XZdyzclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                        txtXZdyzclwzcs1.Text = dyzclwzcs[0];
                        txtXZdyzclwzcs2.Text = dyzclwzcs[1];
                        txtXZdyzclwzcs3.Text = dyzclwzcs[2];
                        txtXZdyzclwzcs4.Text = dyzclwzcs[3];
                        txtXZdyzclwzcs5.Text = dyzclwzcs[4];
                        txtXZdyzclwzcs6.Text = dyzclwzcs.Length > 5 ? dyzclwzcs[5] : string.Empty;
                        txtXZdyzclwzcs7.Text = dyzclwzcs.Length > 5 ? dyzclwzcs[6] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(target.XZdezclwzcs))
                    {
                        string[] dezclwzcs = target.XZdezclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                        txtXZdezclwzcs1.Text = dezclwzcs[0];
                        txtXZdezclwzcs2.Text = dezclwzcs[1];
                        txtXZdezclwzcs3.Text = dezclwzcs[2];
                        txtXZdezclwzcs4.Text = dezclwzcs[3];
                        txtXZdezclwzcs5.Text = dezclwzcs[4];
                        txtXZdezclwzcs6.Text = dezclwzcs.Length > 5 ? dezclwzcs[5] : string.Empty;
                        txtXZdezclwzcs7.Text = dezclwzcs.Length > 5 ? dezclwzcs[6] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(target.XZdszclwzcs))
                    {
                        string[] dszclwzcs = target.XZdszclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                        txtXZdszclwzcs1.Text = dszclwzcs[0];
                        txtXZdszclwzcs2.Text = dszclwzcs[1];
                        txtXZdszclwzcs3.Text = dszclwzcs[2];
                        txtXZdszclwzcs4.Text = dszclwzcs[3];
                        txtXZdszclwzcs5.Text = dszclwzcs[4];
                        txtXZdszclwzcs6.Text = dszclwzcs.Length > 5 ? dszclwzcs[5] : string.Empty;
                        txtXZdszclwzcs7.Text = dszclwzcs.Length > 5 ? dszclwzcs[6] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(target.XZdsizclwzcs))
                    {
                        string[] dsizclwzcs = target.XZdsizclwzcs.Split(new char[] { '|' }, StringSplitOptions.None);
                        txtXZdsizclwzcs1.Text = dsizclwzcs[0];
                        txtXZdsizclwzcs2.Text = dsizclwzcs[1];
                        txtXZdsizclwzcs3.Text = dsizclwzcs[2];
                        txtXZdsizclwzcs4.Text = dsizclwzcs[3];
                        txtXZdsizclwzcs5.Text = dsizclwzcs[4];
                        txtXZdsizclwzcs6.Text = dsizclwzcs.Length > 5 ? dsizclwzcs[5] : string.Empty;
                        txtXZdsizclwzcs7.Text = dsizclwzcs.Length > 5 ? dsizclwzcs[6] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(target.XZsywcl))
                    {
                        string[] sywcl = target.XZsywcl.Split(new char[] { '|' }, StringSplitOptions.None);
                        txtXZsywcl1.Text = sywcl[0];
                        txtXZsywcl2.Text = sywcl[1];
                        txtXZsywcl3.Text = sywcl[2];
                        txtXZsywcl4.Text = sywcl[3];
                        txtXZsywcl5.Text = sywcl[4];
                        txtXZsywcl6.Text = sywcl.Length > 5 ? sywcl[5] : string.Empty;
                        txtXZsywcl7.Text = sywcl.Length > 5 ? sywcl[6] : string.Empty;
                    }
                    if (!string.IsNullOrEmpty(target.XZwz))
                    {
                        string[] wz = target.XZwz.Split(new char[] { '|' }, StringSplitOptions.None);
                        txtXZwz1.Text = wz[0];
                        txtXZwz2.Text = wz[1];
                        txtXZwz3.Text = wz[2];
                        txtXZwz4.Text = wz[3];
                        txtXZwz5.Text = wz[4];
                        txtXZwz6.Text = wz.Length > 5 ? wz[5] : string.Empty;
                        txtXZwz7.Text = wz.Length > 5 ? wz[6] : string.Empty;
                    }
                    txtXZdyzcdrs.Text = target.XZdyzcdrs;
                    txtXZdezcdrs.Text = target.XZdezcdrs;
                    txtXZdszcdrs.Text = target.XZdszcdrs;
                    txtXZdsizcdrs.Text = target.XZdsizcdrs;

                    txtXZdyzqjrs.Text = target.XZdyzqjrs;
                    txtXZdezqjrs.Text = target.XZdezqjrs;
                    txtXZdszqjrs.Text = target.XZdszqjrs;
                    txtXZdsizqjrs.Text = target.XZdsizqjrs;

                    txtXZdyzkgrs.Text = target.XZdyzkgrs;
                    txtXZdezkgrs.Text = target.XZdezkgrs;
                    txtXZdszkgrs.Text = target.XZdszkgrs;
                    txtXZdsizkgrs.Text = target.XZdsizkgrs;

                    txtXZdyzccpxrc.Text = target.XZdyzccpxrc;
                    txtXZdezccpxrc.Text = target.XZdezccpxrc;
                    txtXZdszccpxrc.Text = target.XZdszccpxrc;
                    txtXZdsizccpxrc.Text = target.XZdsizccpxrc;

                    txtXZdyzaqsgsse.Text = target.XZdyzaqsgsse;
                    txtXZdezaqsgsse.Text = target.XZdezaqsgsse;
                    txtXZdszaqsgsse.Text = target.XZdszaqsgsse;
                    txtXZdsizaqsgsse.Text = target.XZdsizaqsgsse;
                }
            }

            #endregion

        }

        private void FillData(DailyReportInfo report)
        {
            #region 数据填充

            Dictionary<string, string> kvp = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(report.SCReport))
            {
                kvp = json.Deserialize<Dictionary<string, string>>(report.SCReport);
            }
            string[] mp = CurrentUser.DayReportModulePowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<DailyReportModuleInfo> list = DayReportModules.Instance.GetList(true);
            list = list.FindAll(l => l.Department == CurrentDep && mp.Contains(l.ID.ToString()));
            foreach (DailyReportModuleInfo m in list)
            {
                if (!string.IsNullOrEmpty(GetString("txtmodule" + m.ID)))
                {
                    if (kvp.Keys.Contains(m.ID.ToString()))
                        kvp[m.ID.ToString()] = GetString("txtmodule" + m.ID);
                    else
                        kvp.Add(m.ID.ToString(), GetString("txtmodule" + m.ID));
                }
            }

            report.SCReport = json.Serialize(kvp);

            #endregion
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void ddlCorp_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private bool CheckUser()
        {
            string[] deppowers = CurrentUser.DayReportDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deppowers.Contains(((int)CurrentDep).ToString()))
                return false;

            return true;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day))
            {
                lock (sync_helper)
                {
                    DailyReportInfo report = null;
                    report = DailyReports.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
                    if (report == null)
                    {
                        report = new DailyReportInfo()
                        {
                            CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                            DayUnique = day.ToString("yyyyMMdd"),
                            Creator = CurrentUser.UserName,
                            LastUpdateUser = CurrentUser.UserName
                        };
                    }
                    else
                        report.LastUpdateUser = CurrentUser.UserName;

                    FillData(report);

                    DailyReports.Instance.CreateAndUpdate(report, CurrentDep);

                    DailyReportInfo reportmodify = new DailyReportInfo();
                    FillData(reportmodify);
                    DailyReportHistoryInfo reporthistory = new DailyReportHistoryInfo();
                    reporthistory.DayUnique = report.DayUnique;
                    reporthistory.Modify = reportmodify;
                    reporthistory.Creator = CurrentUser.UserName;
                    reporthistory.CreatorCorporationID = CurrentUser.CorporationID;
                    reporthistory.CreatorCorporationName = CurrentUser.CorporationName;
                    reporthistory.CreatorDepartment = CurrentUser.DayReportDep;
                    reporthistory.ReportDepartment = CurrentDep;
                    reporthistory.ReportCorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue);
                    DailyReports.Instance.CreateHistory(reporthistory);

                    #region 销售部特殊处理

                    if (CurrentDep == DayReportDep.销售部)
                    {
                        MonthlyTargetInfo target = null;
                        target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
                        if (target == null)
                        {
                            target = new MonthlyTargetInfo()
                            {
                                CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                                Department = CurrentDep,
                                MonthUnique = day.ToString("yyyyMM"),
                                Creator = CurrentUser.UserName,
                                LastUpdateUser = CurrentUser.UserName
                            };
                        }
                        else
                        {
                            target.LastUpdateUser = CurrentUser.UserName;
                        }
                        target.XSztcl = txtXSztcl.Text;
                        target.XSclpjdj = txtXSclpjdj.Text;
                        target.XSzzts = txtXSzzts.Text;
                        MonthlyTargets.Instance.CreateAndUpdate(target);
                    }

                    #endregion

                    #region 财务部特殊处理

                    if (CurrentDep == DayReportDep.财务部 && HasMonthlyTargetPower())
                    {
                        MonthlyTargetInfo target = null;
                        target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
                        if (target == null)
                        {
                            target = new MonthlyTargetInfo()
                            {
                                CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                                Department = CurrentDep,
                                MonthUnique = day.ToString("yyyyMM"),
                                Creator = CurrentUser.UserName,
                                LastUpdateUser = CurrentUser.UserName
                            };
                        }
                        else
                        {
                            target.LastUpdateUser = CurrentUser.UserName;
                        }
                        target.CWzjye = txtCWzjye.Text;
                        target.CWposwdz = txtCWposwdz.Text;
                        target.CWyhzhye = GetString("txtCWyhzhye");
                        target.CWnhzhye = txtCWnhzhye.Text;
                        target.CWzhzhye = txtCWzhzhye.Text;
                        target.CWghzhye = txtCWghzhye.Text;
                        target.CWjianhzhye = txtCWjianhzhye.Text;
                        target.CWjhzhye = txtCWjhzhye.Text;
                        target.CWmszhye = txtCWmszhye.Text;
                        target.CWpazhye = txtCWpazhye.Text;
                        target.CWzxzhye = txtCWzxzhye.Text;
                        target.CWhxzhye = txtCWhxzhye.Text;
                        target.CWzszhye = txtCWzszhye.Text;
                        target.CWtlzhye = txtCWtlzhye.Text;
                        target.CWqtyhzhye = txtCWqtyhzhye.Text;
                        target.CWxjczhj = txtCWxjczhj.Text;
                        target.CWlcxj = txtCWlcxj.Text;
                        target.CWycdkdq = hdnCWycdkdq.Value;
                        MonthlyTargets.Instance.CreateAndUpdate(target);
                    }

                    #endregion

                    #region 行政部特殊处理

                    if (CurrentDep == DayReportDep.行政部 && HasMonthlyTargetPower())
                    {
                        MonthlyTargetInfo target = null;
                        target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
                        if (target == null)
                        {
                            target = new MonthlyTargetInfo()
                            {
                                CorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue),
                                Department = CurrentDep,
                                MonthUnique = day.ToString("yyyyMM"),
                                Creator = CurrentUser.UserName,
                                LastUpdateUser = CurrentUser.UserName
                            };
                        }
                        else
                        {
                            target.LastUpdateUser = CurrentUser.UserName;
                        }
                        target.XZdyzcdrs = txtXZdyzcdrs.Text;
                        target.XZdezcdrs = txtXZdezcdrs.Text;
                        target.XZdszcdrs = txtXZdszcdrs.Text;
                        target.XZdsizcdrs = txtXZdsizcdrs.Text;
                        target.XZdyzqjrs = txtXZdyzqjrs.Text;
                        target.XZdezqjrs = txtXZdezqjrs.Text;
                        target.XZdszqjrs = txtXZdszqjrs.Text;
                        target.XZdsizqjrs = txtXZdsizqjrs.Text;
                        target.XZdyzkgrs = txtXZdyzkgrs.Text;
                        target.XZdezkgrs = txtXZdezkgrs.Text;
                        target.XZdszkgrs = txtXZdszkgrs.Text;
                        target.XZdsizkgrs = txtXZdsizkgrs.Text;
                        target.XZdyzccpxrc = txtXZdyzccpxrc.Text;
                        target.XZdezccpxrc = txtXZdezccpxrc.Text;
                        target.XZdszccpxrc = txtXZdszccpxrc.Text;
                        target.XZdsizccpxrc = txtXZdsizccpxrc.Text;
                        target.XZdyzaqsgsse = txtXZdyzaqsgsse.Text;
                        target.XZdezaqsgsse = txtXZdezaqsgsse.Text;
                        target.XZdszaqsgsse = txtXZdszaqsgsse.Text;
                        target.XZdsizaqsgsse = txtXZdsizaqsgsse.Text;
                        target.XZdyzclwzcs = string.Join("|", new string[] { txtXZdyzclwzcs1.Text, txtXZdyzclwzcs2.Text, txtXZdyzclwzcs3.Text, txtXZdyzclwzcs4.Text, txtXZdyzclwzcs5.Text, txtXZdyzclwzcs6.Text, txtXZdyzclwzcs7.Text, });
                        target.XZdezclwzcs = string.Join("|", new string[] { txtXZdezclwzcs1.Text, txtXZdezclwzcs2.Text, txtXZdezclwzcs3.Text, txtXZdezclwzcs4.Text, txtXZdezclwzcs5.Text, txtXZdezclwzcs6.Text, txtXZdezclwzcs7.Text, });
                        target.XZdszclwzcs = string.Join("|", new string[] { txtXZdszclwzcs1.Text, txtXZdszclwzcs2.Text, txtXZdszclwzcs3.Text, txtXZdszclwzcs4.Text, txtXZdszclwzcs5.Text, txtXZdszclwzcs6.Text, txtXZdszclwzcs7.Text, });
                        target.XZdsizclwzcs = string.Join("|", new string[] { txtXZdsizclwzcs1.Text, txtXZdsizclwzcs2.Text, txtXZdsizclwzcs3.Text, txtXZdsizclwzcs4.Text, txtXZdsizclwzcs5.Text, txtXZdsizclwzcs6.Text, txtXZdsizclwzcs7.Text, });
                        target.XZcpfzr = string.Join("|", new string[] { txtXZcpfzr1.Text, txtXZcpfzr2.Text, txtXZcpfzr3.Text, txtXZcpfzr4.Text, txtXZcpfzr5.Text, txtXZcpfzr6.Text, txtXZcpfzr7.Text, });
                        target.XZsywcl = string.Join("|", new string[] { txtXZsywcl1.Text, txtXZsywcl2.Text, txtXZsywcl3.Text, txtXZsywcl4.Text, txtXZsywcl5.Text, txtXZsywcl6.Text, txtXZsywcl7.Text, });
                        target.XZwz = string.Join("|", new string[] { txtXZwz1.Text, txtXZwz2.Text, txtXZwz3.Text, txtXZwz4.Text, txtXZwz5.Text, txtXZwz6.Text, txtXZwz7.Text, });
                        MonthlyTargets.Instance.CreateAndUpdate(target);
                    }

                    #endregion
                }

                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? CurrentUrl : FromUrl);
            }
        }

        protected bool GetTrClass(string id)
        {
            bool result = true;

            string[] idarray = CurrentUser.DayReportModulePowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!idarray.Contains(id))
                result = false;

            return result;
        }

        protected string GetDepHide(DayReportDep dep)
        {
            string result = string.Empty;

            string[] deparray = CurrentUser.DayReportDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deparray.Contains(((int)dep).ToString()))
                result = "style=\"display:none;\"";

            return result;
        }

        protected bool HasMonthlyTargetPower()
        {
            string[] deppowers = CurrentUser.MonthlyTargetDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deppowers.Contains(((int)CurrentDep).ToString()))
                return false;
            return true;
        }

        protected string GetTableStr()
        {
            StringBuilder strb = new StringBuilder();

            List<DailyReportModuleInfo> list = DayReportModules.Instance.GetList(true);
            list = list.FindAll(l => l.Department == CurrentDep).OrderBy(l => l.Sort).ToList();
            DailyReportInfo report = null;
            DateTime day = DateTime.Today;
            if (DateTime.TryParse(txtDate.Text, out day))
            {
                report = DailyReports.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
            }
            Dictionary<string, string> kvp = new Dictionary<string, string>();
            if (report != null)
            {
                kvp = json.Deserialize<Dictionary<string, string>>(report.SCReport);
            }
            string[] mp = CurrentUser.DayReportModulePowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            allowmodify = false;
            if (mp.Length > 0)
            {
                foreach (string key in mp)
                {
                    if (!kvp.ContainsKey(key) && list.Exists(l => l.ID.ToString() == key))
                    {
                        allowmodify = true;
                        break;
                    }
                }
            }
            else allowmodify = true;

            if (CurrentUser.AllowModify == "1") allowmodify = true;

            if (CurrentDep == DayReportDep.财务部)
            {
                string[] count_yhzhye = { "其中农行", "中行", "工行", "建行", "交行", "民生", "平安", "中信", "华夏", "浙商", "泰隆", "其他银行" };
                string[] count_bryjzf = { "其中：购车（配件）款", "工资", "税款", "其他大额款项" };
                foreach (DailyReportModuleInfo m in list.FindAll(l => mp.Contains(l.ID.ToString())))
                {
                    string name = m.Name;
                    string value = kvp.Keys.Contains(m.ID.ToString()) ? kvp[m.ID.ToString()] : string.Empty;
                    string tr;
                    if (name == "银行帐户余额")
                    {
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"countyhzhye number srk6 tr {5}\" value=\"{2}\" {3} /> 万元{6} <span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    }
                    else if (count_yhzhye.Contains(name))
                    {
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"countyhzhyesub number srk6 tr {5}\" value=\"{2}\" {3} /> 万元{6} <span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || CurrentUser.AllowModify == "1" ? string.Empty : "disabled=\"disabled\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    }
                    else if (name == "本日预计支付合计数")
                    {
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"countbryjzf number srk6 tr {5}\" value=\"{2}\" {3} /> 万元{6} <span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    }
                    else if (count_bryjzf.Contains(name))
                    {
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"countbryjzfsub number srk6 tr {5}\" value=\"{2}\" {3} /> 万元{6} <span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || CurrentUser.AllowModify == "1" ? string.Empty : "disabled=\"disabled\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    }
                    else
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 tr {5}\" value=\"{2}\" {3} /> 万元{6} <span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || CurrentUser.AllowModify == "1" ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    strb.AppendLine(tr);
                }
            }
            else if (CurrentDep == DayReportDep.销售部 && day.ToString("dd") == "01")
            { 
                foreach (DailyReportModuleInfo m in list.FindAll(l => mp.Contains(l.ID.ToString())))
                {
                    string name = m.Name;
                    string value = kvp.Keys.Contains(m.ID.ToString()) ? kvp[m.ID.ToString()] : string.Empty;
                    string tr;
                    if(name=="展厅订单台数")
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 {5} {7}\" value=\"{2}\" {3} />{6}<span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, string.IsNullOrEmpty(value) ? "需包含上月留单" : value, string.IsNullOrEmpty(value) || CurrentUser.AllowModify == "1" ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty,string.IsNullOrEmpty(value) ? "remind gray" : string.Empty);
                    else if (name == "入库台次")
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 {5} {7}\" value=\"{2}\" {3} />{6}<span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, string.IsNullOrEmpty(value) ? "需包含上月在库数" : value, string.IsNullOrEmpty(value) || CurrentUser.AllowModify == "1" ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty, string.IsNullOrEmpty(value) ? "remind gray" : string.Empty);
                    else
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 {5}\" value=\"{2}\" {3} />{6}<span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || CurrentUser.AllowModify == "1" ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    strb.AppendLine(tr);
                }
            }
            else
            {
                foreach (DailyReportModuleInfo m in list.FindAll(l => mp.Contains(l.ID.ToString())))
                {
                    string name = m.Name;
                    string value = kvp.Keys.Contains(m.ID.ToString()) ? kvp[m.ID.ToString()] : string.Empty;
                    string tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 {5}\" value=\"{2}\" {3} />{6}<span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || CurrentUser.AllowModify == "1" ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    strb.AppendLine(tr);
                }
            }
            strb.AppendLine(string.Format("<input type=\"hidden\" id=\"hdnallowmodify\" name=\"hdnallowmodify\" value=\"{0}\" />", allowmodify ? "1" : "0"));

            return strb.ToString();
        }
    }
}