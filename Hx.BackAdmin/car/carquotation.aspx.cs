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
                && ((int)Admin.UserRole & (int)Components.Enumerations.UserRoleType.总经理) == 0
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
                        qlist = qlist.FindAll(q => q.CorporationID == Corp.ID.ToString());
                }
                return qlist;
            }
        }

        private CorporationInfo corp = null;
        protected CorporationInfo Corp
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

        private List<ChoicestgoodsInfo> currentChoicestgoodslist = null;
        private List<ChoicestgoodsInfo> CurrentChoicestgoodslist
        {
            get
            {
                if (currentChoicestgoodslist == null)
                {
                    currentChoicestgoodslist = Choicestgoods.Instance.GetList(Corp.ID, true);
                }

                return currentChoicestgoodslist;
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
            hyName.Text = Admin.Name;
            ddlcChangs.DataSource = CurrentCarbrandlist;
            ddlcChangs.DataTextField = "Name";
            ddlcChangs.DataValueField = "ID";
            ddlcChangs.DataBind();
            ddlcChangs.Items.Insert(0, new ListItem("-车辆品牌-", "-1"));
            ddlcChangs.SelectedIndex = 0;
            txtSwapDetail.Value = swapDefault;

            ddlQcyp.DataSource = CurrentChoicestgoodslist;
            ddlQcyp.DataTextField = "Name";
            ddlQcyp.DataValueField = "ID";
            ddlQcyp.DataBind();
            foreach (ListItem item in ddlQcyp.Items)
            { 
                if(CurrentChoicestgoodslist.Exists(c=>c.ID.ToString() == item.Value))
                {
                    item.Attributes.Add("data-price", CurrentChoicestgoodslist.Find(c => c.ID.ToString() == item.Value).Price);
                }
            }
            ddlSybx.DataSource = Sybxs.Instance.GetList(true);
            ddlSybx.DataTextField = "Name";
            ddlSybx.DataValueField = "ID";
            ddlSybx.DataBind();

            if (CQType == CarQuotationType.金融购车)
            {
                DataTable tBankingType = EnumExtensions.ToTable<BankingType>();
                ddlBankingType.DataSource = tBankingType;
                ddlBankingType.DataTextField = "Name";
                ddlBankingType.DataValueField = "Value";
                ddlBankingType.DataBind();

                ddlBank.DataSource = Banks.Instance.GetList(Corp.ID, true);
                ddlBank.DataTextField = "Name";
                ddlBank.DataValueField = "ID";
                ddlBank.DataBind();
            }

            if (CurrentCustomer != null)
            {
                txtCustomerName.Text = CurrentCustomer.CustomerName;
                txtCustomerMicroletter.Text = CurrentCustomer.CustomerMicroletter;
                txtCustomerMobile.Text = CurrentCustomer.CustomerMobile;
                txtcCjh.Text = CurrentCustomer.Cjh;
                txtCustomerMobile_TextChanged(null, null);
            }
            else if (CurrentCarbrandlist != null && CurrentCarbrandlist.Count == 1)
            {
                ddlcChangs.SelectedIndex = 1;
                ddlcChangs_SelectedIndexChanged(null, null);
            }
        }

        protected void ddlcChangs_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                if (!string.IsNullOrEmpty(CurrentCar.cNsys))
                {
                    DataTable t = new DataTable();
                    t.Columns.Add("Name");
                    t.Columns.Add("Color");

                    foreach (string colorinfo in CurrentCar.cNsys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] color = colorinfo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (color.Length == 2)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = color[0];
                            row["Color"] = color[1].Replace("\"", string.Empty);
                            t.Rows.Add(row);
                        }
                        else if (color.Length == 3)
                        {
                            DataRow row = t.NewRow();
                            row["Name"] = color[0];
                            row["Color"] = color[1].Replace("\"", string.Empty) + "," + color[2].Replace("\"", string.Empty);
                            t.Rows.Add(row);
                        }
                    }

                    rptcNsys.DataSource = t;
                    rptcNsys.DataBind();
                }
                else
                {
                    rptcNsys.DataSource = null;
                    rptcNsys.DataBind();
                }


                hdnColor.Value = string.Empty;
                txtfZdj.Text = decimal.Round(CurrentCar.fZdj * 10000, 0).ToString();
                txtfCjj.Text = txtfZdj.Text;
                if (cbxQcypjz.Checked)
                    trQcypjz.Style.Value = "";
                if (cbxWyfw.Checked)
                    trWyfw.Style.Value = "";

                foreach (ListItem item in ddlQcyp.Items)
                {
                    if (CurrentChoicestgoodslist.Exists(c => c.ID.ToString() == item.Value))
                    {
                        item.Attributes.Add("data-price", CurrentChoicestgoodslist.Find(c => c.ID.ToString() == item.Value).Price);
                    }
                }
                if (!string.IsNullOrEmpty(hdnQcyp.Value))
                {
                    string[] goods = hdnQcyp.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    List<ChoicestgoodsInfo> clist = Choicestgoods.Instance.GetList(true);
                    rptQcyp.DataSource = clist.FindAll(c => goods.Contains(c.ID.ToString()));
                    rptQcyp.DataBind();
                }
            }
        }

        protected void txtCustomerMobile_TextChanged(object sender, EventArgs e)
        {
            qlist = null;
            ddlQuotationHistory.DataSource = QList;
            ddlQuotationHistory.DataTextField = "UCodeName";
            ddlQuotationHistory.DataValueField = "ID";
            ddlQuotationHistory.DataBind();
            if (QList.Count > 0)
            {
                if (QList.Exists(q => !string.IsNullOrEmpty(q.CustomerName)))
                    txtCustomerName.Text = QList.Find(q => !string.IsNullOrEmpty(q.CustomerName)).CustomerName;
                ddlQuotationHistory.SelectedIndex = 0;
                if (QList.Exists(q => CurrentCarbrandlist.Exists(c => c.Name == q.cChangs)))
                    ddlQuotationHistory.SelectedIndex = QList.IndexOf(QList.Find(q => CurrentCarbrandlist.Exists(c => c.Name == q.cChangs)));
                if (Session[GlobalKey.CUSTOMERQUOTATIONID_KEY] != null && QList.Exists(q => q.ID.ToString() == Session[GlobalKey.CUSTOMERQUOTATIONID_KEY].ToString()))
                    SetSelectedByValue(ddlQuotationHistory, Session[GlobalKey.CUSTOMERQUOTATIONID_KEY].ToString());
            }
            BindData();
        }

        protected void ddlQuotationHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string checkresult = CheckQuotation();

            if (string.IsNullOrEmpty(checkresult))
            {
                SaveQuotation();
                Response.Redirect("quotationcheck.aspx?from=" + CurrentUrl);
            }
            else
            {
                lblMsg.Text = checkresult;
            }
        }

        #region 自定义方法

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
                    txtCustomerMicroletter.Text = cq.CustomerMicroletter;
                    hdnColor.Value = cq.cQcys;
                    hdnInnerColor.Value = cq.cNsys;
                    hdnQcyp.Value = cq.ChoicestGoods;
                    txtQcypjz.Text = string.IsNullOrEmpty(cq.ChoicestGoodsPrice) ? "0" : cq.ChoicestGoodsPrice;
                    string[] goods = cq.ChoicestGoods.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (ListItem item in ddlQcyp.Items)
                    {
                        if (CurrentChoicestgoodslist.Exists(c => c.ID.ToString() == item.Value))
                        {
                            item.Attributes.Add("data-price", CurrentChoicestgoodslist.Find(c => c.ID.ToString() == item.Value).Price);
                        }
                    }
                    List<ChoicestgoodsInfo> clist = Choicestgoods.Instance.GetList(true);
                    rptQcyp.DataSource = clist.FindAll(c => goods.Contains(c.ID.ToString()));
                    rptQcyp.DataBind();
                    if (DataConvert.SafeInt(cq.ChoicestGoodsPrice) > 0)
                    {
                        trQcypjz.Style.Value = "";
                        cbxQcypjz.Checked = true;
                    }
                    else
                    {
                        trQcypjz.Style.Value = "display:none;";
                        cbxQcypjz.Checked = false;
                    }
                    if (!string.IsNullOrEmpty(cq.Wyfw))
                    {
                        trWyfw.Style.Value = "";
                        cbxWyfw.Checked = true;
                    }
                    else
                    {
                        trWyfw.Style.Value = "display:none;";
                        cbxWyfw.Checked = false;
                    }
                    txtBxhj.Text = cq.Bxhj;
                    txtWyfw.Text = string.IsNullOrEmpty(cq.Wyfw) ? "0" : cq.Wyfw;
                    cbxWyfwjytc.Checked = cq.IsWyfwjytc;
                    txtWyfwjytc.Text = cq.Wyfwjytc;
                    cbxWyfwblwyfw.Checked = cq.IsWyfwblwyfw;
                    txtWyfwblwyfw.Text = cq.Wyfwblwyfw;
                    cbxWyfwhhwyfw.Checked = cq.IsWyfwhhwyfw;
                    txtWyfwhhwyfw.Text = cq.Wyfwhhwyfw;
                    cbxWyfwybwyfw.Checked = cq.IsWyfwybwyfw;
                    txtWyfwybwyfw.Text = cq.Wyfwybwyfw;
                    txtcCjh.Text = cq.cCjh;
                    txtSaleDay.Text = cq.SaleDay.HasValue ? cq.SaleDay.Value.ToString("yyyy-MM-dd") : string.Empty;
                    txtPlaceDay.Text = cq.PlaceDay.HasValue ? cq.PlaceDay.Value.ToString("yyyy-MM-dd") : string.Empty;
                    cbxIslkhzjs.Checked = cq.Islkhzjs == 1;
                    txtfZdj.Text = cq.fZdj;
                    txtfCjj.Text = cq.fCjj;
                    txtcGzs.Text = cq.cGzs;
                    txtcSpf.Text = cq.cSpf;
                    cbxcJqs.Checked = cq.IscJqs;
                    txtcJqs.Text = cq.cJqs;
                    cbxcCsx.Checked = cq.IscCsx;
                    txtcCsx.Text = cq.cCsx;
                    cbxcDszrx.Checked = cq.IscDszrx;
                    txtcDszrx.Text = cq.cDszrx;
                    cbxcDqx.Checked = cq.IscDqx;
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
                    cbxIsDkh.Checked = cq.IsDkh;
                    cbxIsZcyh.Checked = cq.IsZcyh;
                    txtSwapDetail.Value = cq.SwapDetail;
                    trSwap.Attributes["style"] = cbxIsSwap.Checked ? "" : "display:none;";
                    txtQtfy.Text = string.IsNullOrEmpty(cq.Qtfy) ? "0" : cq.Qtfy;
                    txtQtfyms.Text = cq.Qtfyms;
                    txtcXbyj.Text = cq.cXbyj;
                    txtcBjmp.Text = cq.cBjmp;
                    txtcZdwx.Text = cq.cZdwx;
                    txtLyfxj.Text = cq.Lyfxj;
                    txtDbsplwf.Text = string.IsNullOrEmpty(cq.Dbsplwf) ? "0" : cq.Dbsplwf;
                    txtDbfqlwf.Text = string.IsNullOrEmpty(cq.Dbfqlwf) ? "0" : cq.Dbfqlwf;
                    txtGzf.Text = string.IsNullOrEmpty(cq.Gzf) ? "0" : cq.Gzf;
                    hdncBjmptb.Value = cq.cBjmptb;
                    string[] bjmptb = hdncBjmptb.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    cbxcBjmpcs.Checked = bjmptb.Contains("车损");
                    cbxcBjmpsz.Checked = bjmptb.Contains("三者");
                    cbxcBjmpry.Checked = bjmptb.Contains("人员");
                    cbxcBjmpdq.Checked = bjmptb.Contains("盗抢");

                    txtGift.Text = cq.Gift;
                    txtFirstPayment.Text = cq.FirstPayment;

                    SetSelectedByText(ddlSztb, cq.Sztb);
                    SetSelectedByText(ddlSybx, cq.Bxgs);

                    SetSelectedByValue(ddlBankingType, ((int)cq.BankingType).ToString());
                    SetSelectedByText(ddlBank, cq.BankName);
                    SetSelectedByValue(ddlSfbl, decimal.Round(DataConvert.SafeDecimal(cq.FirstPayment) / DataConvert.SafeDecimal(cq.fCjj), 1).ToString());
                    SetSelectedByValue(ddlLoanLength, cq.LoanLength);
                    if (cq.BankingType == BankingType.银行金融)
                        ddlBank.CssClass = "";
                    else
                        ddlBank.CssClass = "hidden";
                }
                CustomerInfo customer = new CustomerInfo
                {
                    CustomerName = txtCustomerName.Text,
                    CustomerMobile = txtCustomerMobile.Text,
                    CustomerMicroletter = txtCustomerMicroletter.Text,
                    Cjh = txtcCjh.Text
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
            else if (cbxIsSwap.Checked && txtSwapDetail.Value == swapDefault)
            {
                result = "请完善二手车描述";
            }

            return result;
        }

        protected string GetInnerColor(string colors)
        {
            string result = string.Empty;

            if (colors.IndexOf(",") > 0)
            {
                string[] colorlist = colors.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                result += "<span>";
                result += "<em class=\"em-top\" style=\"background-color:" + colorlist[0] + ";\"></em>";
                result += "<em class=\"em-bottom\" style=\"background-color:" + colorlist[1] + ";\"></em>";
                result += "<em class=\"em-bg\"></em></span>";
            }
            else
                result = "<span><em style=\"background-color:" + colors + ";\"></em><em class=\"em-bg\"></em></span>";
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
                CustomerMicroletter = txtCustomerMicroletter.Text,
                cCjh = txtcCjh.Text,
                Islkhzjs = cbxIslkhzjs.Checked ? 1 : 0,
                CarQuotationType = CQType,
                TotalPrinces = lblTotalPrinces.Text,
                TotalFirstPrinces = lblTotalFirstPrinces.Text,
                cChangs = ddlcChangs.SelectedItem.Text,
                cCxmc = ddlcCxmc.SelectedItem.Text,
                cQcys = GetFormString("rdocQcys"),
                cNsys = GetFormString("rdocNsys"),
                fZdj = txtfZdj.Text,
                fCjj = txtfCjj.Text,
                cGzs = txtcGzs.Text,
                cSpf = txtcSpf.Text,
                cCcs = string.Empty,
                IscJqs = cbxcJqs.Checked,
                cJqs = txtcJqs.Text,
                Bxgs = ddlSybx.SelectedItem.Text,
                Bxhj = txtBxhj.Text,
                Wyfw = cbxWyfw.Checked ? txtWyfw.Text : string.Empty,
                IsWyfwjytc = cbxWyfwjytc.Checked,
                Wyfwjytc = txtWyfwjytc.Text,
                IsWyfwblwyfw = cbxWyfwblwyfw.Checked,
                Wyfwblwyfw = txtWyfwblwyfw.Text,
                IsWyfwhhwyfw = cbxWyfwhhwyfw.Checked,
                Wyfwhhwyfw = txtWyfwhhwyfw.Text,
                IsWyfwybwyfw = cbxWyfwybwyfw.Checked,
                Wyfwybwyfw = txtWyfwybwyfw.Text,
                Zkxs = string.Empty,
                IscCsx = cbxcCsx.Checked,
                cCsx = txtcCsx.Text,
                Sztb = ddlSztb.SelectedValue,
                IscDszrx = cbxcDszrx.Checked,
                cDszrx = txtcDszrx.Text,
                IscDqx = cbxcDqx.Checked,
                cDqx = txtcDqx.Text,
                IscSj = cbxcSj.Checked,
                cSjtb = txtcSjtb.Text,
                cSj = txtcSj.Text,
                IscCk = cbxcCk.Checked,
                cCktb = txtcCktb.Text,
                cCk = txtcCk.Text,
                IscZrx = cbxcZrx.Checked,
                cZrx = txtcZrx.Text,
                Blcd = string.Empty,
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
                ChoicestGoods = hdnQcyp.Value,
                ChoicestGoodsPrice = cbxQcypjz.Checked ? txtQcypjz.Text : string.Empty,
                Gift = txtGift.Text,
                IsSwap = cbxIsSwap.Checked,
                IsDkh = cbxIsDkh.Checked,
                IsZcyh = cbxIsZcyh.Checked,
                SwapDetail = txtSwapDetail.Value,
                cBjmptb = hdncBjmptb.Value,
                cBjmp = txtcBjmp.Text,
                IscZdwx = cbxcZdwx.Checked,
                cZdwx = txtcZdwx.Text,
                cXbyj = txtcXbyj.Text,
                Lyfxj = txtLyfxj.Text,
                Dbsplwf = DataConvert.SafeInt(txtDbsplwf.Text) > 0 ? txtDbsplwf.Text : string.Empty,
                Dbfqlwf = DataConvert.SafeInt(txtDbfqlwf.Text) > 0 ? txtDbfqlwf.Text : string.Empty,
                Zxf = string.Empty,
                Dcf = string.Empty,
                Gzf = DataConvert.SafeInt(txtGzf.Text) > 0 ? txtGzf.Text : string.Empty,
                Qtfy = DataConvert.SafeInt(txtQtfy.Text) > 0 ? txtQtfy.Text : string.Empty,
                Qtfyms = txtQtfyms.Text
            };
            if (!string.IsNullOrEmpty(txtSaleDay.Text))
                info.SaleDay = DateTime.Parse(txtSaleDay.Text);
            if (!string.IsNullOrEmpty(txtPlaceDay.Text))
                info.PlaceDay = DateTime.Parse(txtPlaceDay.Text);

            if (CQType == CarQuotationType.金融购车)
            {
                info.BankingType = (BankingType)DataConvert.SafeInt(ddlBankingType.SelectedValue);
                info.BankName = ddlBank.SelectedItem.Text;
                info.FirstPayment = txtFirstPayment.Text;
                info.LoanValue = txtLoanValue.Text;
                info.LoanLength = ddlLoanLength.SelectedValue;
                info.RepaymentPerMonth = txtRepaymentPerMonth.Text;
            }

            if (!string.IsNullOrEmpty(info.CustomerMobile))
                CarQuotations.Instance.Add(info, true);

            MangaCache.AddLocal(GlobalKey.QUOTATION_KEY, info, 600);
            Session[GlobalKey.CUSTOMER_KEY] = null;
        }

        #endregion
    }
}