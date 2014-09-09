using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hx.Components.BasePage;
using Hx.Car;
using Hx.Car.Entity;
using Hx.Components;

namespace Hx.BackAdmin.global
{
    public partial class corporationedit : AdminBase
    {
        private CorporationInfo _currentcorporation = null;
        private CorporationInfo CurrentCorporation
        {
            get
            {
                int id = GetInt("id");
                if (_currentcorporation == null && id > 0)
                {
                    _currentcorporation = Corporations.Instance.GetModel(id);
                }
                return _currentcorporation;
            }
        }
        private string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private int CurrentCharIndex = -1;

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
            ddlBank.DataSource = Banks.Instance.GetList();
            ddlBank.DataTextField = "Name";
            ddlBank.DataValueField = "Name";
            ddlBank.DataBind();
            ddlBank.Items.Insert(0, new ListItem("-请选择-", ""));

            List<KeyValuePair<string, string>> bankProfitMarginList = new List<KeyValuePair<string, string>> 
            { 
                new KeyValuePair<string, string>("3年","3年"),
                new KeyValuePair<string, string>("2年","2年"),
                new KeyValuePair<string, string>("1年","1年")
            };

            ddlBankProfitMargin.DataSource = bankProfitMarginList;
            ddlBankProfitMargin.DataTextField = "Key";
            ddlBankProfitMargin.DataValueField = "Value";
            ddlBankProfitMargin.DataBind();

            rptCarbrand.DataSource = CarBrands.Instance.GetCarBrandList();
            rptCarbrand.DataBind();
        }

        private void LoadData()
        {
            if (CurrentCorporation != null)
            {

                txtName.Text = CurrentCorporation.Name;
                hdnCarbrand.Value = CurrentCorporation.CarBrand;

                SetSelectedByText(ddlBank, CurrentCorporation.Bank);
                SetSelectedByText(ddlBankProfitMargin, CurrentCorporation.BankProfitMargin);
            }
            else
            {
                WriteErrorMessage("错误提示", "非法ID", "corporationmg.aspx");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CorporationInfo corporation = Corporations.Instance.GetModel(GetInt("id"), false);
            corporation.Name = txtName.Text;
            corporation.CarBrand = hdnCarbrand.Value;
            corporation.Bank = ddlBank.SelectedValue;
            corporation.BankProfitMargin = ddlBankProfitMargin.SelectedValue;

            Corporations.Instance.Update(corporation);

            CarBrands.Instance.ReloadCarBrandCacheByCorporation(corporation.ID.ToString());

            WriteSuccessMessage("保存成功！", "数据已经成功保存！", string.IsNullOrEmpty(FromUrl) ? "~/global/corporationmg.aspx" : FromUrl);
        }

        protected string SetCarbrand(string name)
        {
            string result = string.Empty;

            if (CurrentCorporation != null)
            {
                string[] brands = CurrentCorporation.CarBrand.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (brands.Contains(name))
                    result = "checked=\"checked\"";
            }

            return result;
        }

        protected string GetNewCharStr(string nameindex)
        {
            string result = string.Empty;

            if (CurrentCharIndex == -1)
            {
                result += string.Format("<ul><li class=\"nh\"><input type=\"checkbox\" class=\"cbxSuball\" checked=\"checked\" />{0}</li>", nameindex);
                CurrentCharIndex = chars.IndexOf(nameindex, StringComparison.OrdinalIgnoreCase);
            }
            else if (CurrentCharIndex > -1 && chars.IndexOf(nameindex, StringComparison.OrdinalIgnoreCase) != CurrentCharIndex)
            {
                result += string.Format("</ul><ul><li class=\"nh\"><input type=\"checkbox\" class=\"cbxSuball\" checked=\"checked\" />{0}</li>", nameindex);
                CurrentCharIndex = chars.IndexOf(nameindex, StringComparison.OrdinalIgnoreCase);
            }


            return result;
        }
    }
}