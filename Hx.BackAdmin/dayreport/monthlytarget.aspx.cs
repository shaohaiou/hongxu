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

        private CorporationInfo _currentcorporation = null;
        protected CorporationInfo CurrentCorporation
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
            ddlCorp.DataSource = corps.FindAll(c => c.DailyreportShow == 1 && corppowers.Contains(c.ID.ToString()));
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
                if (CurrentDep == DayReportDep.市场部)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtSCscdfdcl.Text = target.SCscdfdcl;
                    txtSCsyfsl.Text = target.SCsyfsl;

                    #endregion

                    #endregion
                }
                else if (CurrentDep == DayReportDep.销售部)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtXSztzb.Text = target.XSztzb;
                    txtXSztldl.Text = target.XSztldl;
                    txtXSztcjl.Text = target.XSztcjl;
                    txtXSspl.Text = target.XSspl;
                    txtXSztbxl.Text = target.XSztbxl;
                    txtXSmrjcl.Text = target.XSmrjcl;
                    txtXSztjpqzl.Text = target.XSztjpqzl;
                    txtXSajl.Text = target.XSajl;
                    txtXSmfbystl.Text = target.XSmfbystl;

                    txtXSzxstc.Text = target.XSzxstc;
                    txtXSspdt.Text = target.XSspdt;
                    txtXSztbxdt.Text = target.XSztbxdt;
                    txtXSmrdt.Text = target.XSmrdt;
                    txtXSztjppjdt.Text = target.XSztjppjdt;
                    txtXSewjppjdt.Text = target.XSewjppjdt;
                    txtXSxszhtc.Text = target.XSxszhtc;
                    txtXSajpjdt.Text = target.XSajpjdt;
                    txtXSmfbydt.Text = target.XSmfbydt;
                    txtXSzjsl.Text = target.XSzjsl;

                    if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                    {
                        txtXStppxstc.Text = target.XStppxstc;
                        txtXStppdcml.Text = target.XStppdcml;
                        txtXStppzhml.Text = target.XStppzhml;
                        txtXStpppjdt.Text = target.XStpppjdt;
                    }

                    #endregion

                    #endregion
                }
                else if (CurrentDep == DayReportDep.售后部)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtSHwxkhzs.Text = target.SHwxkhzs;

                    #endregion

                    #endregion
                }
                else if (CurrentDep == DayReportDep.无忧产品)
                {
                    #region 绑定数据

                    #region 关键指标

                    txtNXCPshjytcstl.Text = target.NXCPshjytcstl;
                    txtNXCPshblwyfwstl.Text = target.NXCPshblwyfwstl;
                    txtNXCPshhhwyfwstl.Text = target.NXCPshhhwyfwstl;
                    txtNXCPshybwycfwstl.Text = target.NXCPshybwycfwstl;

                    #endregion

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
            if (CurrentDep == DayReportDep.售后部)
            {
                int idjsescpgs = 0;
                int idlctc = 0;
                if (list.Exists(l => l.Department == CurrentDep && l.Name == "介绍二手车评估数"))
                    idjsescpgs = list.Find(l => l.Department == CurrentDep && l.Name == "介绍二手车评估数").ID;
                if (list.Exists(l => l.Department == CurrentDep && l.Name == "来厂台次"))
                    idlctc = list.Find(l => l.Department == CurrentDep && l.Name == "来厂台次").ID;
                if (idjsescpgs > 0 && idlctc > 0 && kvp.Keys.Contains(idlctc.ToString()))
                {
                    if (kvp.Keys.Contains(idjsescpgs.ToString()))
                        kvp[idjsescpgs.ToString()] = Math.Round(DataConvert.SafeDecimal(DataConvert.SafeDouble(kvp[idlctc.ToString()]) * 0.05), 0).ToString();
                    else
                        kvp.Add(idjsescpgs.ToString(), Math.Round(DataConvert.SafeDecimal(DataConvert.SafeDouble(kvp[idlctc.ToString()]) * 0.05), 0).ToString());
                }
            }

            target.SCReport = json.Serialize(kvp);

            #endregion

            if (CurrentDep == DayReportDep.市场部)
            {
                #region 关键指标

                target.SCscdfdcl = txtSCscdfdcl.Text;
                target.SCsyfsl = txtSCsyfsl.Text;

                #endregion
            }
            else if (CurrentDep == DayReportDep.销售部)
            {
                target.XSztzb = txtXSztzb.Text;
                target.XSztldl = txtXSztldl.Text;
                target.XSztcjl = txtXSztcjl.Text;
                target.XSspl = txtXSspl.Text;
                target.XSztbxl = txtXSztbxl.Text;
                target.XSmrjcl = txtXSmrjcl.Text;
                target.XSztjpqzl = txtXSztjpqzl.Text;
                target.XSajl = txtXSajl.Text;
                target.XSmfbystl = txtXSmfbystl.Text;

                target.XSzxstc = txtXSzxstc.Text;
                target.XSspdt = txtXSspdt.Text;
                target.XSztbxdt = txtXSztbxdt.Text;
                target.XSmrdt = txtXSmrdt.Text;
                target.XSztjppjdt = txtXSztjppjdt.Text;
                target.XSewjppjdt = txtXSewjppjdt.Text;
                target.XSxszhtc = txtXSxszhtc.Text;
                target.XSajpjdt = txtXSajpjdt.Text;
                target.XSmfbydt = txtXSmfbydt.Text;
                target.XSzjsl = txtXSzjsl.Text;

                if (CurrentCorporation != null && CurrentCorporation.DailyreportTpp == 1)
                {
                    target.XStppxstc = txtXStppxstc.Text;
                    target.XStppdcml = txtXStppdcml.Text;
                    target.XStppzhml = txtXStppzhml.Text;
                    target.XStpppjdt = txtXStpppjdt.Text;
                }
            }
            else if (CurrentDep == DayReportDep.售后部)
            {
                target.SHwxkhzs = txtSHwxkhzs.Text;
            }
            else if (CurrentDep == DayReportDep.无忧产品)
            {
                target.NXCPshjytcstl = txtNXCPshjytcstl.Text;
                target.NXCPshblwyfwstl = txtNXCPshblwyfwstl.Text;
                target.NXCPshhhwyfwstl = txtNXCPshhhwyfwstl.Text;
                target.NXCPshybwycfwstl = txtNXCPshybwycfwstl.Text;
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
            if (CurrentDep == DayReportDep.财务部)
            {
                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? UrlDecode(CurrentUrl) : FromUrl);
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

                MonthlyTargetInfo monthlytargetmodify = new MonthlyTargetInfo();
                FillData(monthlytargetmodify);
                MonthlyTargetHistoryInfo targethistory = new MonthlyTargetHistoryInfo();
                targethistory.MonthUnique = target.MonthUnique;
                targethistory.Modify = monthlytargetmodify;
                targethistory.Creator = CurrentUser.UserName;
                targethistory.CreatorCorporationID = CurrentUser.CorporationID;
                targethistory.CreatorCorporationName = CurrentUser.CorporationName;
                targethistory.CreatorDepartment = CurrentUser.DayReportDep;
                targethistory.ReportDepartment = CurrentDep;
                targethistory.ReportCorporationID = DataConvert.SafeInt(ddlCorp.SelectedValue);
                MonthlyTargets.Instance.CreateHistory(targethistory);

                WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? UrlDecode(CurrentUrl) : FromUrl);
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

            if (CurrentDep == DayReportDep.无忧产品)
            {
                List<KeyValuePair<string, string>> listcountname = new List<KeyValuePair<string, string>>() 
                { 
                    new KeyValuePair<string, string>("新车延保服务购买个数","countxsybfwgmgs"),
                    new KeyValuePair<string, string>("新车延保服务购买金额","countxsybfwgmje"),
                    new KeyValuePair<string, string>("售后延保服务购买个数","countshybfwgmgs"),
                    new KeyValuePair<string, string>("售后延保服务购买金额","countshybfwgmje")
                };
                List<KeyValuePair<string, string>> listcountsubname = new List<KeyValuePair<string, string>>() 
                { 
                    new KeyValuePair<string, string>("新车延保服务自主购买个数","countxsybfwgmgssub"),
                    new KeyValuePair<string, string>("新车延保服务厂家购买个数","countxsybfwgmgssub"),
                    new KeyValuePair<string, string>("新车延保服务自主购买金额","countxsybfwgmjesub"),
                    new KeyValuePair<string, string>("新车延保服务厂家购买金额","countxsybfwgmjesub"),
                    new KeyValuePair<string, string>("售后延保服务自主购买个数","countshybfwgmgssub"),
                    new KeyValuePair<string, string>("售后延保服务厂家购买个数","countshybfwgmgssub"),
                    new KeyValuePair<string, string>("售后延保服务自主购买金额","countshybfwgmjesub"),
                    new KeyValuePair<string, string>("售后延保服务厂家购买金额","countshybfwgmjesub")
                };
                foreach (DailyReportModuleInfo m in list.FindAll(l => l.Ismonthlytarget))
                {
                    string name = m.Name;
                    string value = kvp.Keys.Contains(m.ID.ToString()) ? kvp[m.ID.ToString()] : string.Empty;
                    string tr = string.Format("<tr {6}><td class=\"bg4 tr\">{0}：</td><td colspan=\"3\"><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"srk6 {5} {7}\" value=\"{2}\" {4} /><span class=\"gray pl10\">{3}</span></td></tr>"
                        , name.Replace("新车", string.Empty).Replace("售后", string.Empty).Replace("续保", string.Empty).Replace("当月来厂基盘车辆数", string.Empty)
                        , m.ID
                        , value
                        , m.Description.Replace("当天", "当月")
                        , allowmodify ? string.Empty : "readonly=\"true\""
                        , listcountname.Exists(l => l.Key == name) ? listcountname.Find(l => l.Key == name).Value : string.Empty
                        , listcountname.Exists(l => l.Key == name) ? "style=\"display:none\"" : string.Empty
                        , listcountsubname.Exists(l => l.Key == name) ? listcountsubname.Find(l => l.Key == name).Value : string.Empty);
                    if (name == "新车机油套餐购买个数")
                        tr = "<tr><td colspan=\"4\" style=\"font-weight:bold;font-size:large;padding-left:20px;\">销售数据</td></tr>" + tr;
                    if (name == "售后机油套餐购买个数")
                        tr = "<tr><td colspan=\"4\" style=\"font-weight:bold;font-size:large;padding-left:20px;\">售后数据</td></tr>" + tr;
                    if (name == "当月来厂基盘车辆数≤18个月")
                        tr = "<tr><td colspan=\"4\" style=\"font-weight:bold;font-size:large;padding-left:20px;\">来厂基盘车辆</td></tr>" + tr;
                    strb.AppendLine(tr);
                }
            }
            else if (CurrentDep == DayReportDep.销售部)
            {
                List<KeyValuePair<string, string>> listcountname = new List<KeyValuePair<string, string>>() 
                { 
                    new KeyValuePair<string, string>("延保无忧车服务购买个数","countxsybwycfwgmgs"),
                    new KeyValuePair<string, string>("延保无忧车服务购买金额","countxsybwycfwgmje")
                };
                List<KeyValuePair<string, string>> listcountsubname = new List<KeyValuePair<string, string>>() 
                { 
                    new KeyValuePair<string, string>("自主延保无忧车服务购买个数","countxsybwycfwgmgssub"),
                    new KeyValuePair<string, string>("厂家延保无忧车服务购买个数","countxsybwycfwgmgssub"),
                    new KeyValuePair<string, string>("自主延保无忧车服务购买金额","countxsybwycfwgmjesub"),
                    new KeyValuePair<string, string>("厂家延保无忧车服务购买金额","countxsybwycfwgmjesub")
                };
                foreach (DailyReportModuleInfo m in list.FindAll(l => l.Ismonthlytarget))
                {
                    string name = m.Name;
                    string value = kvp.Keys.Contains(m.ID.ToString()) ? kvp[m.ID.ToString()] : string.Empty;
                    string tr;
                    if (listcountname.Exists(l => l.Key == name))
                        tr = string.Format("<tr style=\"display:none\"><td class=\"bg4 tr\">{0}：</td><td colspan=\"3\"><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 {5} {7}\" value=\"{2}\" {3} />{6}<span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || allowmodify ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty, listcountname.Find(l => l.Key == name).Value);
                    else if (listcountsubname.Exists(l => l.Key == name))
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td colspan=\"3\"><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 {5} {7}\" value=\"{2}\" {3} />{6}<span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || allowmodify ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty, listcountsubname.Find(l => l.Key == name).Value);
                    else
                        tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td colspan=\"3\"><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"number srk6 {5}\" value=\"{2}\" {3} />{6}<span class=\"gray pl10\">{4}</span></td></tr>", name, m.ID, value, string.IsNullOrEmpty(value) || allowmodify ? string.Empty : "readonly=\"true\"", m.Description, m.Mustinput ? "required" : string.Empty, m.Mustinput ? "<span class=\"red pl10\">*</span>" : string.Empty);
                    strb.AppendLine(tr);
                }
            }
            else
            {
                foreach (DailyReportModuleInfo m in list.FindAll(l => l.Ismonthlytarget))
                {
                    string name = m.Name;
                    string value = kvp.Keys.Contains(m.ID.ToString()) ? kvp[m.ID.ToString()] : string.Empty;
                    string tr = string.Format("<tr><td class=\"bg4 tr\">{0}：</td><td colspan=\"3\"><input id=\"txtmodule{1}\" name=\"txtmodule{1}\" class=\"srk6\" value=\"{2}\" {4} /><span class=\"gray pl10\">{3}</span></td></tr>", name, m.ID, value, m.Description.Replace("当日", "当月"), allowmodify ? string.Empty : "readonly=\"true\"");
                    strb.AppendLine(tr);
                }
                strb.AppendLine(string.Format("<input type=\"hidden\" id=\"hdnallowmodify\" name=\"hdnallowmodify\" value=\"{0}\" />", allowmodify ? "1" : "0"));
            }
            return strb.ToString();
        }
    }
}