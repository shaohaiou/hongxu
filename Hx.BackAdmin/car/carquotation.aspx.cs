using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Components.Web;
using Hx.Car;
using Hx.Car.Entity;
using Hx.Tools;
using System.Data;
using Hx.Tools.Web;
using Hx.Car.Enum;
using System.Web.UI.HtmlControls;
using Hx.Components;
using Hx.Components.Entity;

namespace Hx.BackAdmin.car
{
    public partial class carquotation : AdminBase
    {
        protected override void Check()
        {
            if (!HXContext.Current.AdminCheck)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!Admin.Administrator
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售经理) == 0
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.销售员) == 0)
            {
                Response.Clear();
                Response.Write("您没有权限操作！");
                Response.End();
                return;
            }
        }

        private CarQuotationType? cqtype = null;
        protected CarQuotationType CQType
        {
            get
            {
                if (!cqtype.HasValue)
                    cqtype = (CarQuotationType)GetInt("t");
                return cqtype.Value;
            }
        }

        private List<CarQuotationInfo> qlist = null;
        private List<CarQuotationInfo> QList
        {
            get
            {
                if (qlist == null)
                {
                    qlist = CarQuotations.Instance.GetList(txtCustomerMobile.Text, CQType);
                    if (Corp != null)
                        qlist = qlist.FindAll(q=>q.CorporationID == Corp.ID.ToString());
                }
                return qlist;
            }
        }

        private CorporationInfo corp = null;
        private CorporationInfo Corp
        {
            get
            {
                if (corp == null)
                    corp = Corporations.Instance.GetModel(DataConvert.SafeInt(Admin.Corporation), true);
                return corp;
            }
        }

        private static object sync_data = new object();
        private CarInfo _car = null;
        protected CarInfo CurrentCar
        {
            get
            {
                if (_car == null)
                {
                    lock (sync_data)
                    {
                        if (_car == null)
                        {
                            string cChangs = ddlcChangs.SelectedItem.Text;
                            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(cChangs, true);
                            int carid = DataConvert.SafeInt(ddlcCxmc.SelectedValue);
                            _car = carlist.Find(c => c.id == carid);
                        }
                    }
                }
                return _car;
            }
        }

        private List<CarBrandInfo> currentCarbrandlist = null;
        private List<CarBrandInfo> CurrentCarbrandlist
        {
            get
            {
                if (currentCarbrandlist == null)
                {
                    currentCarbrandlist = CarBrands.Instance.GetCarBrandListByCorporation(Admin.Corporation);
                }

                return currentCarbrandlist;
            }
        }

        private CustomerInfo currentcustomer = null;
        private CustomerInfo CurrentCustomer
        {
            get
            {
                if (currentcustomer == null)
                {
                    if (Session[GlobalKey.CUSTOMER_KEY] != null)
                        currentcustomer = (CustomerInfo)Session[GlobalKey.CUSTOMER_KEY];
                }
                return currentcustomer;
            }
        }

        protected string swapDefault = "车型：\r颜色：\r上牌年份：\r公里数：\r是否有重大事故：";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControler();
            }
        }

        private void BindControler()
        {
            hyName.Text = AdminName;
            ddlcChangs.DataSource = CurrentCarbrandlist;
            ddlcChangs.DataTextField = "Name";
            ddlcChangs.DataValueField = "Name";
            ddlcChangs.DataBind();
            ddlcChangs.Items.Insert(0, new ListItem("-车辆品牌-", "-1"));
            ddlcChangs.SelectedIndex = 0;
            txtSwapDetail.Value = swapDefault;

            if (CQType == CarQuotationType.贷款购车)
            {
                DataTable tLoanType = EnumExtensions.ToTable<LoanType>();
                tLoanType.DefaultView.RowFilter = "Value > 0";
                ddlLoanType.DataSource = tLoanType.DefaultView;
                ddlLoanType.DataTextField = "Name";
                ddlLoanType.DataValueField = "Value";
                ddlLoanType.DataBind();

                DataTable tBankingType = EnumExtensions.ToTable<BankingType>();
                ddlBankingType.DataSource = tBankingType;
                ddlBankingType.DataTextField = "Name";
                ddlBankingType.DataValueField = "Value";
                ddlBankingType.DataBind();

                ddlBank.DataSource = Banks.Instance.GetList(true);
                ddlBank.DataTextField = "Name";
                ddlBank.DataValueField = "ID";
                ddlBank.DataBind();
                
                if (Corp != null)
                    SetSelectedByText(ddlBank, Corp.Bank);
                string loanLength = "3";
                if (Corp != null)
                    loanLength = Corp.BankProfitMargin.Replace("年", string.Empty);
                SetSelectedByValue(ddlLoanLength, loanLength);
                ddlBank_SelectedIndexChanged(null, null);
                ddlBankingType_SelectedIndexChanged(null,null);
            }

            if (CurrentCustomer != null)
            {
                txtCustomerName.Text = CurrentCustomer.CustomerName;
                txtCustomerQQ.Text = CurrentCustomer.CustomerQQ;
                txtCustomerEmail.Text = CurrentCustomer.CustomerEmail;
                txtCustomerMicroletter.Text = CurrentCustomer.CustomerMicroletter;
                txtCustomerMobile.Text = CurrentCustomer.CustomerMobile;
                txtCustomerMobile_TextChanged(null, null);
            }
            else if (CurrentCarbrandlist.Count == 1)
            {
                ddlcChangs.SelectedIndex = 1;
                ddlcChangs_SelectedIndexChanged(null, null);
            }
        }

        protected void ddlcChangs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlcChangs.SelectedIndex == 0)
            {
                txtCustomerName.Text = string.Empty;
                txtCustomerQQ.Text = string.Empty;
                txtCustomerEmail.Text = string.Empty;
                txtCustomerMicroletter.Text = string.Empty;
                ddlcCxmc.Items.Clear();
                rptcQcys.DataSource = null;
                rptcQcys.DataBind();
                txtfZdj.Text = "0";
                txtfCjj.Text = "0";

                txtcGzs.Text = "0";
                txtcSpf.Text = "0";
                txtcCcs.Text = "0";
                txtcJqs.Text = "0";

                cbxIsSwap.Checked = false;
                txtLyfxj.Text = "0";

                ddlZkxs.SelectedIndex = 1;
                ddlBlcd.SelectedIndex = 0;
                txtcCsx.Text = "0";
                ddlSztb.SelectedIndex = 2;
                txtcDszrx.Text = "0";
                txtcDqx.Text = "0";
                cbxcSj.Checked = true;
                txtcSj.Text = "0";
                cbxcCk.Checked = true;
                txtcCk.Text = "0";
                txtcZdwx.Text = "0";
                cbxcZrx.Checked = true;
                txtcZrx.Text = "0";
                cbxcBlx.Checked = true;
                txtcBlx.Text = "0";
                txtcHhx.Text = "0";
                txtcSsx.Text = "0";
                cbxcBjmpcs.Checked = true;
                cbxcBjmpsz.Checked = true;
                cbxcBjmpry.Checked = true;
                cbxcBjmpdq.Checked = true;
                txtcBjmp.Text = "0";
                hdncBjmptb.Value = "|车损|三者|人员|盗抢|";
                txtDbsplwf.Text = "0";

                txtRepaymentPerMonth.Text = "0";
                txtFirstPayment.Text = "0";
                txtLoanValue.Text = "0";
                txtInterest.Text = "0";
                txtcXbyj.Text = "0";
                txtLyfxj.Text = "0";
                txtDbfqlwf.Text = "0";
                return;
            }

            if (Corp != null && !string.IsNullOrEmpty(Corp.AutoCdszrxje))
                SetSelectedByValue(ddlSztb, Corp.AutoCdszrxje);

            string cChangs = ddlcChangs.SelectedItem.Text;
            List<CarInfo> carlist = Cars.Instance.GetCarListBycChangs(cChangs, true);
            if (Corp != null && !string.IsNullOrEmpty(Corp.AutomotivetypeSetting))
            {
                string[] settings = Corp.AutomotivetypeSetting.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string settingstr in settings)
                {
                    string[] setting = settingstr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (setting.Length == 2)
                    {
                        string _carbrand = setting[0];
                        string _automotivetypes = setting[1];
                        if (_carbrand.Equals(cChangs, StringComparison.OrdinalIgnoreCase))
                        {
                            carlist = carlist.FindAll(c => _automotivetypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(c.id.ToString()));
                            break;
                        }
                    }
                }
            }

            ddlcCxmc.DataSource = carlist;
            ddlcCxmc.DataTextField = "cCxmc";
            ddlcCxmc.DataValueField = "id";
            ddlcCxmc.DataBind();
            ddlcCxmc.SelectedIndex = 0;
            ddlcCxmc_SelectedIndexChanged(null, null);
        }

        protected void ddlcCxmc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentCar != null)
            {
                if (!string.IsNullOrEmpty(CurrentCar.cQcys))
                {
                    DataTable t = new DataTable();
                    t.Columns.Add("Name");
                    t.Columns.Add("Color");

                    foreach (string colorinfo in CurrentCar.cQcys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] color = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (color.Length == 2)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = color[0];
                            row["Color"] = color[1];
                            t.Rows.Add(row);
                        }
                    }

                    rptcQcys.DataSource = t;
                    rptcQcys.DataBind();
                }
                else
                {
                    rptcQcys.DataSource = null;
                    rptcQcys.DataBind();
                }

                hdnColor.Value = string.Empty;
                hdnZws.Value = CurrentCar.cZws;
                txtfZdj.Text = decimal.Round(CurrentCar.fZdj * 10000, 0).ToString();
                txtfCjj.Text = txtfZdj.Text;
                SetMustSpend();

                ddlZkxs_SelectedIndexChanged(null, null);

                ddlSfbl_SelectedIndexChanged(null, null);
            }
        }

        protected void ddlZkxs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentCar != null)
            {
                double zkxs = DataConvert.SafeDouble(ddlZkxs.SelectedValue);
                double gcj = DataConvert.SafeDouble(txtfCjj.Text);
                txtcCsx.Text = Math.Round((566 + (gcj * 1.35 / 100)) * zkxs, 2).ToString();
                ddlSztb_SelectedIndexChanged(null, null);
                txtcDqx.Text = Math.Round((120 + (gcj * 0.41 / 100)) * zkxs, 2).ToString();
                txtcSj.Text = Math.Round(DataConvert.SafeDouble(txtcSjtb.Text) * 10000 * 0.41 / 100 * zkxs, 2).ToString();
                txtcCk.Text = Math.Round(DataConvert.SafeDouble(txtcCktb.Text) * 10000 * 0.26 / 100 * zkxs * (DataConvert.SafeInt(CurrentCar.cZws, 2) - 1), 2).ToString();
                txtcZdwx.Text = Math.Round((566 + (gcj * 1.35 / 100)) * 0.15 * zkxs, 2).ToString();
                txtcZrx.Text = Math.Round(gcj * 0.15 / 100 * zkxs, 2).ToString();
                ddlBlcd_SelectedIndexChanged(null, null);
                BindBjmp();

                double bxhj = 0;
                bxhj += DataConvert.SafeDouble(txtcCsx.Text);
                bxhj += DataConvert.SafeDouble(txtcDszrx.Text);
                bxhj += DataConvert.SafeDouble(txtcDqx.Text);
                if (cbxcSj.Checked)
                    bxhj += DataConvert.SafeDouble(txtcSj.Text);
                if (cbxcCk.Checked)
                    bxhj += DataConvert.SafeDouble(txtcCk.Text);
                if (cbxcZdwx.Checked)
                    bxhj += DataConvert.SafeDouble(txtcZdwx.Text);
                if (cbxcZrx.Checked)
                    bxhj += DataConvert.SafeDouble(txtcZrx.Text);
                if (cbxcBlx.Checked)
                    bxhj += DataConvert.SafeDouble(txtcBlx.Text);
                bxhj += DataConvert.SafeDouble(txtcBjmp.Text);
                bxhj += DataConvert.SafeDouble(txtcJqs.Text);

                lblBxhj.Text = Math.Round(bxhj, 2).ToString();
            }
        }

        protected void ddlBlcd_SelectedIndexChanged(object sender, EventArgs e)
        {
            double zkxs = DataConvert.SafeDouble(ddlZkxs.SelectedValue);
            double fl = 0.21;
            if (ddlBlcd.SelectedValue == "1")
                fl = 0.36;
            txtcBlx.Text = Math.Round(DataConvert.SafeDouble(txtfCjj.Text) * fl / 100 * zkxs, 2).ToString();
        }

        protected void ddlSztb_SelectedIndexChanged(object sender, EventArgs e)
        {
            double cDszrx = GetDszrx();

            txtcDszrx.Text = (Math.Round(cDszrx * DataConvert.SafeDouble(ddlZkxs.SelectedValue), 2)).ToString();
        }

        protected void txtCustomerMobile_TextChanged(object sender, EventArgs e)
        {
            ddlQuotationHistory.DataSource = QList;
            ddlQuotationHistory.DataTextField = "UCodeName";
            ddlQuotationHistory.DataValueField = "ID";
            ddlQuotationHistory.DataBind();
            if (QList.Count > 0)
            {
                if (QList.Exists(q => !string.IsNullOrEmpty(q.CustomerName)))
                    txtCustomerName.Text = QList.Find(q => !string.IsNullOrEmpty(q.CustomerName)).CustomerName;
                if (QList.Exists(q => !string.IsNullOrEmpty(q.CustomerQQ)))
                    txtCustomerQQ.Text = QList.Find(q => !string.IsNullOrEmpty(q.CustomerQQ)).CustomerQQ;
                if (QList.Exists(q => !string.IsNullOrEmpty(q.CustomerEmail)))
                    txtCustomerEmail.Text = QList.Find(q => !string.IsNullOrEmpty(q.CustomerEmail)).CustomerEmail;
                ddlQuotationHistory.SelectedIndex = 0;
                if (QList.Exists(q => CurrentCarbrandlist.Exists(c => c.Name == q.cChangs)))
                    ddlQuotationHistory.SelectedIndex = QList.IndexOf(QList.Find(q => CurrentCarbrandlist.Exists(c => c.Name == q.cChangs)));
                if (Session[GlobalKey.CUSTOMERQUOTATIONID_KEY] != null && QList.Exists(q => q.ID.ToString() == Session[GlobalKey.CUSTOMERQUOTATIONID_KEY].ToString()))
                    SetSelectedByValue(ddlQuotationHistory, Session[GlobalKey.CUSTOMERQUOTATIONID_KEY].ToString());
            }
            BindData();
        }

        protected void txtfCjj_TextChanged(object sender, EventArgs e)
        {
            if (CurrentCar != null)
            {
                SetMustSpend();

                ddlZkxs_SelectedIndexChanged(null, null);

                ddlSfbl_SelectedIndexChanged(null, null);
            }
        }

        protected void ddlSfbl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CQType == CarQuotationType.贷款购车)
            {
                double gcj = DataConvert.SafeDouble(txtfCjj.Text);
                txtFirstPayment.Text = Math.Round(gcj * DataConvert.SafeDouble(ddlSfbl.SelectedValue), 2).ToString();
                txtLoanValue.Text = (Math.Round((gcj - DataConvert.SafeDouble(txtFirstPayment.Text)) / 1000, 0) * 1000).ToString();
                txtFirstPayment.Text = (gcj - DataConvert.SafeDouble(txtLoanValue.Text)).ToString();

                ddlLoanLength_SelectedIndexChanged(null, null);
            }
        }

        protected void ddlLoanLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            string loanLength = ddlLoanLength.SelectedValue;

            double loanvalue = DataConvert.SafeDouble(txtLoanValue.Text);
            int monthNum = DataConvert.SafeInt(ddlLoanLength.SelectedItem.Text);
            double profitMargin = DataConvert.SafeDouble(txtProfitMargin.Text);
            txtInterest.Text = profitMargin > 0 ? Math.Round(loanvalue / 10000 * profitMargin * monthNum - loanvalue, 2).ToString() : "0";
            txtRepaymentPerMonth.Text = profitMargin > 0 ? Math.Round(loanvalue / 10000 * profitMargin, 2).ToString() : Math.Round(loanvalue / monthNum, 2).ToString();

            txtcXbyj.Text = Math.Round(loanvalue * 0.03, 2).ToString();
            txtLyfxj.Text = txtcXbyj.Text;

            if (Corp != null && !string.IsNullOrEmpty(Corp.AutoClyfxj))
                txtLyfxj.Text = Math.Round(loanvalue * DataConvert.SafeDouble(Corp.AutoClyfxj), 2).ToString();
            if (Corp != null && !string.IsNullOrEmpty(Corp.AutoCxbyj))
                txtcXbyj.Text = Math.Round(loanvalue * DataConvert.SafeDouble(Corp.AutoCxbyj), 2).ToString();
        }

        protected void ddlQuotationHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void txtProfitMargin_TextChanged(object sender, EventArgs e)
        {
            if (Corp != null && !string.IsNullOrEmpty(txtProfitMargin.Text) && txtProfitMargin.Text.Trim() != "0")
            {
                Corp.AutoCyhll = txtProfitMargin.Text;

                Corporations.Instance.Update(Corp);
                Corporations.Instance.ReloadCorporationListCache();
            }
            ddlSfbl_SelectedIndexChanged(null, null);
        }

        protected void ddlBankingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBankingType.SelectedValue == ((int)BankingType.传统金融).ToString())
            {
                if (!string.IsNullOrEmpty(Corp.AutoCyhll))
                    txtProfitMargin.Text = Corp.AutoCyhll;
                else
                {
                    BankInfo bank = Banks.Instance.GetList(true).Find(b => b.Name == ddlBank.SelectedItem.Text);
                    if (bank != null)
                    {
                        switch (ddlLoanLength.SelectedValue)
                        {
                            case "3":
                                txtProfitMargin.Text = bank.BankProfitMargin3y;
                                break;
                            case "2":
                                txtProfitMargin.Text = bank.BankProfitMargin2y;
                                break;
                            case "1":
                                txtProfitMargin.Text = bank.BankProfitMargin1y;
                                break;
                            default:
                                txtProfitMargin.Text = bank.BankProfitMargin3y;
                                break;
                        }
                    }
                }
            }
            else
                txtProfitMargin.Text = "0";
            ddlLoanLength_SelectedIndexChanged(null, null);
        }

        protected void ddlBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlLoanLength_SelectedIndexChanged(null, null);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string checkresult = CheckQuotation();

            if (string.IsNullOrEmpty(checkresult))
            {
                SaveQuotation();
                Response.Redirect("quotation.aspx?from=" + CurrentUrl);
            }
            else
            {
                lblMsg.Text = checkresult;
            }
        }

        #region 自定义方法

        private void SetMustSpend()
        {
            txtcGzs.Text = decimal.Round(DataConvert.SafeDecimal(DataConvert.SafeDouble(txtfZdj.Text) / 1.17 * 0.1), 0, MidpointRounding.AwayFromZero).ToString();

            int zws = DataConvert.SafeInt(CurrentCar.cZws);
            txtcJqs.Text = zws <= 6 ? "950" : "1100";

            txtcSpf.Text = "700";
            txtDbsplwf.Text = "800";
            txtDbfqlwf.Text = "3000";
            if (Corp != null && !string.IsNullOrEmpty(Corp.AutoCspf))
                txtcSpf.Text = Corp.AutoCspf;
            if (Corp != null && !string.IsNullOrEmpty(Corp.AutoCjcf))
                txtDbsplwf.Text = Corp.AutoCjcf;
            if (Corp != null && !string.IsNullOrEmpty(Corp.AutoCdbfqfksxf))
                txtDbfqlwf.Text = Corp.AutoCdbfqfksxf;

            double ccs = 0;
            double pl = Math.Round(DataConvert.SafeDouble(CurrentCar.fPail) / 1000, 1);
            if (pl <= 1.0)
                ccs = 180;
            else if (pl <= 1.6)
                ccs = 300;
            else if (pl <= 2.0)
                ccs = 360;
            else if (pl <= 2.5)
                ccs = 660;
            else if (pl <= 3.0)
                ccs = 1500;
            else if (pl <= 4.0)
                ccs = 3000;
            else
                ccs = 4500;

            txtcCcs.Text = Math.Round(ccs, 0).ToString();
        }

        private double GetDszrx()
        {
            double cDszrx = 0;
            switch (ddlSztb.SelectedValue)
            {
                case "100":
                    cDszrx = 2124;
                    break;
                case "50":
                    cDszrx = 1631;
                    break;
                case "30":
                    cDszrx = 1359;
                    break;
                case "20":
                    cDszrx = 1204;
                    break;
                case "15":
                    cDszrx = 1108;
                    break;
                case "10":
                    cDszrx = 972;
                    break;
                case "5":
                    cDszrx = 673;
                    break;
                default:
                    cDszrx = 0;
                    break;
            }
            return cDszrx;
        }

        private void BindBjmp()
        {
            double zkxs = DataConvert.SafeDouble(ddlZkxs.SelectedValue);
            double gcj = DataConvert.SafeDouble(txtfCjj.Text);
            double bjmp = 0;
            string[] bjmptb = hdncBjmptb.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            if (bjmptb.Contains("车损"))
            {
                cbxcBjmpcs.Checked = true;
                bjmp += (566 + (gcj * 1.35 / 100)) * zkxs * 0.15;
            }
            if (bjmptb.Contains("三者"))
            {
                cbxcBjmpsz.Checked = true;
                bjmp += GetDszrx() * zkxs * 0.15;
            }
            if (bjmptb.Contains("人员"))
            {
                cbxcBjmpry.Checked = true;
                if (cbxcSj.Checked)
                    bjmp += DataConvert.SafeDouble(txtcSjtb.Text) * 10000 * 0.41 / 100 * zkxs * 0.15;
                if (cbxcCk.Checked)
                    bjmp += DataConvert.SafeDouble(txtcCktb.Text) * 10000 * 0.26 / 100 * zkxs * (DataConvert.SafeInt(CurrentCar.cZws, 2) - 1) * 0.15;
            }
            if (bjmptb.Contains("盗抢"))
            {
                cbxcBjmpdq.Checked = true;
                bjmp += (120 + (gcj * 0.41 / 100)) * zkxs * 0.2;
            }

            txtcBjmp.Text = Math.Round(bjmp, 2).ToString();
        }

        private void BindData()
        {
            if (!string.IsNullOrEmpty(txtCustomerMobile.Text))
            {
                if (QList.Exists(q => q.ID.ToString() == ddlQuotationHistory.SelectedValue))
                {
                    Session[GlobalKey.CUSTOMERQUOTATIONID_KEY] = ddlQuotationHistory.SelectedValue;
                    CarQuotationInfo cq = QList.Find(q => q.ID.ToString() == ddlQuotationHistory.SelectedValue);
                    lblTotalPrinces.Text = cq.TotalPrinces;
                    SetSelectedByText(ddlcChangs, cq.cChangs);
                    ddlcChangs_SelectedIndexChanged(null, null);
                    SetSelectedByText(ddlcCxmc, cq.cCxmc);
                    _car = null;
                    ddlcCxmc_SelectedIndexChanged(null, null);
                    hdnColor.Value = cq.cQcys;

                    txtfCjj.Text = cq.fCjj;
                    txtcGzs.Text = cq.cGzs;
                    txtcSpf.Text = cq.cSpf;
                    txtcCcs.Text = cq.cCcs;
                    txtcJqs.Text = cq.cJqs;
                    txtcCsx.Text = cq.cCsx;
                    txtcDqx.Text = cq.cDqx;
                    cbxcSj.Checked = cq.IscSj;
                    txtcSjtb.Text = cq.cSjtb;
                    txtcSj.Text = cq.cSj;
                    cbxcCk.Checked = cq.IscCk;
                    txtcCktb.Text = cq.cCktb;
                    txtcCk.Text = cq.cCk;
                    cbxcZdwx.Checked = cq.IscZdwx;
                    cbxcZrx.Checked = cq.IscZrx;
                    txtcZrx.Text = cq.cZrx;
                    cbxcBlx.Checked = cq.IscBlx;
                    cbxcHhx.Checked = cq.IscHhx;
                    txtcHhx.Text = cq.cHhx;
                    cbxcSsx.Checked = cq.IscSsx;
                    txtcSsx.Text = cq.cSsx;
                    cbxIsSwap.Checked = cq.IsSwap;
                    txtSwapDetail.Value = cq.SwapDetail;
                    trSwap.Attributes["style"] = cbxIsSwap.Checked ? "" : "display:none;";


                    txtcXbyj.Text = cq.cXbyj;
                    txtcBjmp.Text = cq.cBjmp;
                    txtcZdwx.Text = cq.cZdwx;
                    txtLyfxj.Text = cq.Lyfxj;
                    txtDbsplwf.Text = cq.Dbsplwf;
                    txtDbfqlwf.Text = cq.Dbfqlwf;
                    txtZxf.Text = cq.Zxf;
                    txtDcf.Text = cq.Dcf;
                    hdncBjmptb.Value = cq.cBjmptb;

                    txtChoicestGoods.Text = cq.ChoicestGoods;
                    txtChoicestGoodsPrice.Text = cq.ChoicestGoodsPrice;
                    txtGift.Text = cq.Gift;

                    SetSelectedByText(ddlSztb, cq.Sztb);
                    SetSelectedByText(ddlBlcd, cq.Blcd);
                    SetSelectedByValue(ddlZkxs, cq.Zkxs);
                    ddlZkxs_SelectedIndexChanged(null, null);

                    txtProfitMargin.Text = cq.ProfitMargin;
                    SetSelectedByValue(ddlBankingType, ((int)cq.BankingType).ToString());
                    SetSelectedByText(ddlBank, cq.BankName);
                    SetSelectedByText(ddlLoanType, cq.LoanType.ToString());
                    SetSelectedByValue(ddlSfbl, decimal.Round(DataConvert.SafeDecimal(cq.FirstPayment) / DataConvert.SafeDecimal(cq.fCjj), 1).ToString());
                    SetSelectedByValue(ddlLoanLength, cq.LoanLength);
                    ddlSfbl_SelectedIndexChanged(null, null);
                }
                CustomerInfo customer = new CustomerInfo
                {
                    CustomerName = txtCustomerName.Text,
                    CustomerMobile = txtCustomerMobile.Text,
                    CustomerEmail = txtCustomerEmail.Text,
                    CustomerQQ = txtCustomerQQ.Text,
                    CustomerMicroletter = txtCustomerMicroletter.Text
                };
                Session[GlobalKey.CUSTOMER_KEY] = customer;
            }
            else
            {
                ddlcChangs.SelectedIndex = 0;
                ddlcChangs_SelectedIndexChanged(null, null);
            }
        }

        private string CheckQuotation()
        {
            string result = string.Empty;

            if (ddlcChangs.SelectedIndex == 0) result = "请选择车型";
            else if (string.IsNullOrEmpty(txtCustomerName.Text.Trim())) result = "请输入客户姓名";
            else if (string.IsNullOrEmpty(txtCustomerMobile.Text.Trim())) result = "请输入客户联系电话";
            else if (string.IsNullOrEmpty(txtCustomerQQ.Text.Trim()) && string.IsNullOrEmpty(txtCustomerEmail.Text.Trim()) && string.IsNullOrEmpty(txtCustomerMicroletter.Text.Trim())) result = "客户QQ、客户邮箱、客户微信号至少填写一项";
            else if (cbxIsSwap.Checked && txtSwapDetail.Value == swapDefault)
            {
                result = "请完善二手车描述";
            }

            return result;
        }

        private void SaveQuotation()
        {
            CarQuotationInfo info = new CarQuotationInfo
            {
                CorporationID = Admin.Corporation,
                Creator = Admin.UserName,
                CreateTime = DateTime.Now,
                CustomerMobile = txtCustomerMobile.Text,
                CustomerName = txtCustomerName.Text,
                CustomerQQ = txtCustomerQQ.Text,
                CustomerEmail = txtCustomerEmail.Text,
                CustomerMicroletter = txtCustomerMicroletter.Text,
                CarQuotationType = CQType,
                TotalPrinces = lblTotalPrinces.Text,
                TotalFirstPrinces = lblTotalFirstPrinces.Text,
                cChangs = ddlcChangs.SelectedItem.Text,
                cCxmc = ddlcCxmc.SelectedItem.Text,
                cQcys = GetFormString("rdocQcys"),
                fCjj = txtfCjj.Text,
                cGzs = txtcGzs.Text,
                cSpf = txtcSpf.Text,
                cCcs = txtcCcs.Text,
                cJqs = txtcJqs.Text,
                Bxgs = string.Empty,
                Zkxs = ddlZkxs.SelectedItem.Value,
                cCsx = txtcCsx.Text,
                Sztb = ddlSztb.SelectedValue,
                cDszrx = txtcDszrx.Text,
                cDqx = txtcDqx.Text,
                IscSj = cbxcSj.Checked,
                cSjtb = txtcSjtb.Text,
                cSj = txtcSj.Text,
                IscCk = cbxcCk.Checked,
                cCktb = txtcCktb.Text,
                cCk = txtcCk.Text,
                IscZrx = cbxcZrx.Checked,
                cZrx = txtcZrx.Text,
                Blcd = ddlBlcd.SelectedItem.Text,
                IscBlx = cbxcBlx.Checked,
                cBlx = txtcBlx.Text,
                IscHhx = cbxcHhx.Checked,
                cHhx = txtcHhx.Text,
                IscSsx = cbxcSsx.Checked,
                cSsx = txtcSsx.Text,
                LoanType = Car.Enum.LoanType.未设置,
                BankName = string.Empty,
                FirstPayment = string.Empty,
                LoanValue = string.Empty,
                LoanLength = string.Empty,
                RepaymentPerMonth = string.Empty,
                RemainingFund = string.Empty,
                ProfitMargin = string.Empty,
                OtherCost = string.Empty,
                AccountManagementCost = string.Empty,
                ChoicestGoods = txtChoicestGoods.Text,
                ChoicestGoodsPrice = txtChoicestGoodsPrice.Text,
                Gift = txtGift.Text,
                IsSwap = cbxIsSwap.Checked,
                SwapDetail = txtSwapDetail.Value,
                cBjmptb = hdncBjmptb.Value,
                cBjmp = txtcBjmp.Text,
                IscZdwx = cbxcZdwx.Checked,
                cZdwx = txtcZdwx.Text,
                cXbyj = txtcXbyj.Text,
                Lyfxj = txtLyfxj.Text,
                Dbsplwf = txtDbsplwf.Text,
                Dbfqlwf = txtDbfqlwf.Text,
                Zxf = txtZxf.Text,
                Dcf = txtDcf.Text
            };

            if (CQType == CarQuotationType.贷款购车)
            {
                info.LoanType = (LoanType)DataConvert.SafeInt(ddlLoanType.SelectedValue);
                info.BankingType = (BankingType)DataConvert.SafeInt(ddlBankingType.SelectedValue);
                info.BankName = ddlBank.SelectedItem.Text;
                info.FirstPayment = txtFirstPayment.Text;
                info.LoanValue = txtLoanValue.Text;
                info.LoanLength = ddlLoanLength.SelectedValue;
                info.RepaymentPerMonth = txtRepaymentPerMonth.Text;
                info.ProfitMargin = txtProfitMargin.Text;
            }

            if (!string.IsNullOrEmpty(info.CustomerMobile))
                CarQuotations.Instance.Add(info, true);
            if (Corp != null)
            {
                if (CQType == CarQuotationType.贷款购车)
                {
                    Corp.AutoCyhll = string.IsNullOrEmpty(txtProfitMargin.Text) || txtProfitMargin.Text.Trim() == "0" ? Corp.AutoCyhll : txtProfitMargin.Text;
                    Corp.AutoCdbfqfksxf = string.IsNullOrEmpty(txtDbfqlwf.Text) ? Corp.AutoCdbfqfksxf : txtDbfqlwf.Text;
                    Corp.AutoClyfxj = string.IsNullOrEmpty(txtLyfxj.Text) || string.IsNullOrEmpty(txtLoanValue.Text) || txtLoanValue.Text.Trim() == "0" ? Corp.AutoClyfxj : Math.Round(DataConvert.SafeDouble(txtLyfxj.Text) / DataConvert.SafeDouble(txtLoanValue.Text), 2).ToString();
                    Corp.AutoCxbyj = string.IsNullOrEmpty(txtcXbyj.Text) || string.IsNullOrEmpty(txtLoanValue.Text) || txtLoanValue.Text.Trim() == "0" ? Corp.AutoCxbyj : Math.Round(DataConvert.SafeDouble(txtcXbyj.Text) / DataConvert.SafeDouble(txtLoanValue.Text), 2).ToString();
                }
                Corp.AutoCdszrxje = ddlSztb.SelectedValue;
                Corp.AutoCjcf = string.IsNullOrEmpty(txtDbsplwf.Text) ? Corp.AutoCjcf : txtDbsplwf.Text;
                Corp.AutoCspf = string.IsNullOrEmpty(txtcSpf.Text) ? Corp.AutoCspf : txtcSpf.Text;

                Corporations.Instance.Update(Corp);
                Corporations.Instance.ReloadCorporationListCache();
            }

            MangaCache.AddLocal(GlobalKey.QUOTATION_KEY, info, 600);
        }

        #endregion
    }
}