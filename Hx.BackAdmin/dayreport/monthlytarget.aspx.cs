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
using Hx.Car;
using Hx.Car.Entity;
using System.Text;

namespace Hx.BackAdmin.dayreport
{
    public partial class monthlytarget : AdminBase
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
                    string[] deppowers = CurrentUser.MonthlyTargetDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
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
            txtDate.Text = DateTime.Today.ToString("yyyy-MM");

            string[] corppowers = CurrentUser.MonthlyTargetCorpPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            List<CorporationInfo> corps = Corporations.Instance.GetList(true);
            ddlCorp.DataSource = corps.FindAll(c => corppowers.Contains(c.ID.ToString()));
            ddlCorp.DataTextField = "Name";
            ddlCorp.DataValueField = "ID";
            ddlCorp.DataBind();
            ddlCorp.Items.Insert(0, new ListItem("-请选择-", "-1"));
            SetSelectedByValue(ddlCorp, !corppowers.Contains(CurrentUser.CorporationID.ToString()) && corppowers.Length > 0 ? corppowers[0] : CurrentUser.CorporationID.ToString());
        }

        private void LoadData()
        {
            MonthlyTargetInfo target = null;
            DateTime day = DateTime.Today;
            if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
                if (target == null)
                {
                    target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day.AddMonths(-1), true);
                    if (target != null)
                    {
                        lblMsg.Text = string.Format("（本月未设置月度目标，以下是{0}的数据）", day.AddMonths(-1).ToString("yyyy年MM月份"));
                    }
                }
                else
                {
                    lblMsg.Text = string.Empty;
                }
            }
            bool allowmodify = true;
            if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-07", out day))
            {
                if (DateTime.Today > day && CurrentUser.AllowModify != "1" && DateTime.Today > DateTime.Parse("2014-10-11"))
                {
                    allowmodify = false;
                }
            }
            foreach (Control c in up1.ContentTemplateContainer.Controls.Cast<Control>())
            {
                if (c.GetType() == typeof(TextBox) && c.ID != "txtDate")
                {
                    ((TextBox)c).ReadOnly = !allowmodify;
                }
            }

            if (target != null)
            {
                if (CurrentDep == DayReportDep.销售部)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtXSztldl.Text = target.XSztldl;
                    txtXSztcjl.Text = target.XSztcjl;
                    txtXSztzb.Text = target.XSztzb;
                    txtXSspl.Text = target.XSspl;
                    txtXSspdt.Text = target.XSspdt;
                    txtXSbxl.Text = target.XSbxl;
                    txtXSbxdt.Text = target.XSbxdt;
                    txtXSmrjcl.Text = target.XSmrjcl;
                    txtXSmrdt.Text = target.XSmrdt;
                    txtXSybstl.Text = target.XSybstl;
                    txtXSztjpqzl.Text = target.XSztjpqzl;
                    txtXSztjpdt.Text = target.XSztjpdt;
                    txtXSewjpdt.Text = target.XSewjpdt;
                    txtXSescpgl.Text = target.XSescpgl;
                    txtXSajl.Text = target.XSajl;
                    txtXSajpjdt.Text = target.XSajpjdt;
                    txtXSmfbystl.Text = target.XSmfbystl;
                    txtXSmfbypjdt.Text = target.XSmfbypjdt;

                    #endregion

                    #endregion
                }
                else if (CurrentDep == DayReportDep.售后部)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtSHlctc.Text = target.SHlctc;
                    txtSHyyl.Text = target.SHyyl;
                    txtSHczdc.Text = target.SHczdc;
                    txtSHsgczl.Text = target.SHsgczl;
                    txtSHyhbl.Text = target.SHyhbl;
                    txtSHsgsccgl.Text = target.SHsgsccgl;
                    txtSHsgcdxcgl.Text = target.SHsgcdxcgl;
                    txtSHmrbl.Text = target.SHmrbl;
                    txtSHspcgl.Text = target.SHspcgl;
                    txtSHnfl.Text = target.SHnfl;
                    txtSHghjsl.Text = target.SHghjsl;
                    txtSHsgzccgl.Text = target.SHsgzccgl;
                    txtSHdtcz.Text = target.SHdtcz;
                    txtSHbydtcz.Text = target.SHbydtcz;
                    txtSHbytczb.Text = target.SHbytczb;
                    txtSHmrdtcz.Text = target.SHmrdtcz;
                    txtSHcctc.Text = target.SHcctc;
                    txtSHybdcl.Text = target.SHybdcl;

                    #endregion

                    #endregion
                }
                else if (CurrentDep == DayReportDep.市场部)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtSCxsdcl.Text = target.SCxsdcl;
                    txtSCscdfdcl.Text = target.SCscdfdcl;
                    txtSCsfjdl.Text = target.SCsfjdl;
                    txtSCwlhtxsdcl.Text = target.SCwlhtxsdcl;
                    txtSCwlxsjdl.Text = target.SCwlxsjdl;
                    txtSChrdhdcl.Text = target.SChrdhdcl;
                    txtSChrdhjdl.Text = target.SChrdhjdl;
                    txtSCcdqkdcl.Text = target.SCcdqkdcl;
                    txtSChdjkdcl.Text = target.SChdjkdcl;

                    #endregion

                    txtSCxzyxxsl.Text = target.SCxzyxxsl;

                    #endregion
                }
                else if (CurrentDep == DayReportDep.DCC部)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtDCCztzb.Text = target.DCCztzb;
                    txtDCCsdjdl.Text = target.DCCsdjdl;
                    txtDCCwlhtjdl.Text = target.DCCwlhtjdl;
                    txtDCCwlhrjdl.Text = target.DCCwlhrjdl;
                    txtDCCyxhcl.Text = target.DCCyxhcl;
                    txtDCChrhcyyddl.Text = target.DCChrhcyyddl;
                    txtDCCzcyyl.Text = target.DCCzcyyl;
                    txtDCCcjl.Text = target.DCCcjl;

                    #endregion

                    #endregion
                }
                else if (CurrentDep == DayReportDep.二手车部)
                {
                    #region 关键指标

                    txtESCxsyxtjl.Text = target.ESCxsyxtjl;
                    txtESCxhyxtjl.Text = target.ESCshyxtjl;
                    txtESCpgcjl.Text = target.ESCpgcjl;
                    txtESCxscjl.Text = target.ESCxscjl;
                    txtESCpjdtml.Text = target.ESCpjdtml;
                    txtESCzzhl.Text = target.ESCzzhl;
                    txtESCzyxpgl.Text = target.ESCzyxpgl;
                    txtESCzsgl.Text = target.ESCzsgl;
                    txtESCzxsl.Text = target.ESCzxsl;
                    txtESCzml.Text = target.ESCzml;

                    #endregion
                }
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

        }

        private void FillData(MonthlyTargetInfo target)
        {

            #region 各项目标值

            Dictionary<string, string> kvp = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(target.SCReport))
            {
                kvp = json.Deserialize<Dictionary<string, string>>(target.SCReport);
            }
            List<DailyReportModuleInfo> list = DayReportModules.Instance.GetList(true);
            list = list.FindAll(l => l.Department == CurrentDep);
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

            target.SCReport = json.Serialize(kvp);

            #endregion

            if (CurrentDep == DayReportDep.销售部)
            {
                #region 关键指标

                target.XSztldl = txtXSztldl.Text;
                target.XSztcjl = txtXSztcjl.Text;
                target.XSztzb = txtXSztzb.Text;
                target.XSspl = txtXSspl.Text;
                target.XSspdt = txtXSspdt.Text;
                target.XSbxl = txtXSbxl.Text;
                target.XSbxdt = txtXSbxdt.Text;
                target.XSmrjcl = txtXSmrjcl.Text;
                target.XSmrdt = txtXSmrdt.Text;
                target.XSybstl = txtXSybstl.Text;
                target.XSztjpqzl = txtXSztjpqzl.Text;
                target.XSztjpdt = txtXSztjpdt.Text;
                target.XSewjpdt = txtXSewjpdt.Text;
                target.XSescpgl = txtXSescpgl.Text;
                target.XSajl = txtXSajl.Text;
                target.XSajpjdt = txtXSajpjdt.Text;
                target.XSmfbystl = txtXSmfbystl.Text;
                target.XSmfbypjdt = txtXSmfbypjdt.Text;

                #endregion
            }
            else if (CurrentDep == DayReportDep.售后部)
            {
                #region 关键指标

                target.SHlctc = txtSHlctc.Text;
                target.SHyyl = txtSHyyl.Text;
                target.SHczdc = txtSHczdc.Text;
                target.SHsgczl = txtSHsgczl.Text;
                target.SHyhbl = txtSHyhbl.Text;
                target.SHsgsccgl = txtSHsgsccgl.Text;
                target.SHsgcdxcgl = txtSHsgcdxcgl.Text;
                target.SHmrbl = txtSHmrbl.Text;
                target.SHspcgl = txtSHspcgl.Text;
                target.SHnfl = txtSHnfl.Text;
                target.SHghjsl = txtSHghjsl.Text;
                target.SHsgzccgl = txtSHsgzccgl.Text;
                target.SHdtcz = txtSHdtcz.Text;
                target.SHbydtcz = txtSHbydtcz.Text;
                target.SHbytczb = txtSHbytczb.Text;
                target.SHmrdtcz = txtSHmrdtcz.Text;
                target.SHcctc = txtSHcctc.Text;
                target.SHybdcl = txtSHybdcl.Text;

                #endregion
            }
            else if (CurrentDep == DayReportDep.市场部)
            {
                target.SCxzyxxsl = txtSCxzyxxsl.Text;

                #region 关键指标

                target.SCxsdcl = txtSCxsdcl.Text;
                target.SCscdfdcl = txtSCscdfdcl.Text;
                target.SCsfjdl = txtSCsfjdl.Text;
                target.SCwlhtxsdcl = txtSCwlhtxsdcl.Text;
                target.SCwlxsjdl = txtSCwlxsjdl.Text;
                target.SChrdhdcl = txtSChrdhdcl.Text;
                target.SChrdhjdl = txtSChrdhjdl.Text;
                target.SCcdqkdcl = txtSCcdqkdcl.Text;
                target.SChdjkdcl = txtSChdjkdcl.Text;

                #endregion
            }
            else if (CurrentDep == DayReportDep.DCC部)
            {
                #region 关键指标

                target.DCCztzb = txtDCCztzb.Text;
                target.DCCsdjdl = txtDCCsdjdl.Text;
                target.DCCwlhtjdl = txtDCCwlhtjdl.Text;
                target.DCCwlhrjdl = txtDCCwlhrjdl.Text;
                target.DCCyxhcl = txtDCCyxhcl.Text;
                target.DCChrhcyyddl = txtDCChrhcyyddl.Text;
                target.DCCzcyyl = txtDCCzcyyl.Text;
                target.DCCcjl = txtDCCcjl.Text;

                #endregion
            }
            else if (CurrentDep == DayReportDep.二手车部)
            {
                #region 关键指标

                target.ESCxsyxtjl = txtESCxsyxtjl.Text;
                target.ESCshyxtjl = txtESCxhyxtjl.Text;
                target.ESCpgcjl = txtESCpgcjl.Text;
                target.ESCxscjl = txtESCxscjl.Text;
                target.ESCpjdtml = txtESCpjdtml.Text;
                target.ESCzzhl = txtESCzzhl.Text;
                target.ESCzyxpgl = txtESCzyxpgl.Text;
                target.ESCzsgl = txtESCzsgl.Text;
                target.ESCzxsl = txtESCzxsl.Text;
                target.ESCzml = txtESCzml.Text;

                #endregion
            }
        }

        protected void txtDate_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void ddlCorp_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (CurrentDep == DayReportDep.财务部 || CurrentDep == DayReportDep.行政部)
            {
                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? CurrentUrl : FromUrl);
                return;
            }

            DateTime day = DateTime.Today;
            if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
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

                FillData(target);

                MonthlyTargets.Instance.CreateAndUpdate(target);

                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? CurrentUrl : FromUrl);
            }
        }

        private bool CheckUser()
        {
            string[] deppowers = CurrentUser.MonthlyTargetDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deppowers.Contains(((int)CurrentDep).ToString()))
                return false;

            return true;
        }

        protected string GetDepHide(DayReportDep dep)
        {
            string result = string.Empty;

            string[] deparray = CurrentUser.MonthlyTargetDepPowerSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (!deparray.Contains(((int)dep).ToString()))
                result = "style=\"display:none;\"";

            return result;
        }

        protected string GetTableStr()
        {
            StringBuilder strb = new StringBuilder();

            List<DailyReportModuleInfo> list = DayReportModules.Instance.GetList(true);
            list = list.FindAll(l => l.Department == CurrentDep).OrderBy(l => l.Sort).ToList();
            MonthlyTargetInfo target = null;
            bool allowmodify = true;
            DateTime day = DateTime.Today;
            if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-01", out day))
            {
                target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day, true);
                if (target == null)
                    target = MonthlyTargets.Instance.GetModel(DataConvert.SafeInt(ddlCorp.SelectedValue), CurrentDep, day.AddMonths(-1), true);
            }
            if (ddlCorp.SelectedIndex > 0 && DateTime.TryParse(txtDate.Text + "-07", out day))
            {
                if (DateTime.Today > day && CurrentUser.AllowModify != "1" && DateTime.Today > DateTime.Parse("2014-10-11"))
                {
                    allowmodify = false;
                }
            }
            Dictionary<string, string> kvp = new Dictionary<string, string>();
            if (target != null)
            {
                kvp = json.Deserialize<Dictionary<string, string>>(target.SCReport);
            }
            if (kvp == null) kvp = new Dictionary<string, string>();

            foreach (DailyReportModuleInfo m in list.FindAll(l => l.Ismonthlytarget))
            {
                string name = m.Name;
                string value = kvp.Keys.Contains(m.ID.ToString()) ? kvp[m.ID.ToString()] : string.Empty;
                string tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td colspan=\"3\"><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"srk6\" value=\"{2}\" {4} /><span class=\"gray pl10\">{3}</span></td></tr>", name, m.ID, value, m.Description.Replace("当日", "当月"), allowmodify ? string.Empty : "readonly=\"true\"");
                strb.AppendLine(tr);
            }
            strb.AppendLine(string.Format("<input type=\"hidden\" id=\"hdnallowmodify\" name=\"hdnallowmodify\" value=\"{0}\" />", allowmodify ? "1" : "0"));

            return strb.ToString();
        }
    }
}